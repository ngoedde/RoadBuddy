using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using RB.Core.Net.Common.Messaging.Allocation;
using RB.Core.Net.Common.Messaging.Memory;
using RB.Core.Net.Common.Threading;
using Serilog;

namespace RB.Core.Net.Common.Messaging;

public class Message : MessageStream
{
    public const ushort BUFFER_SIZE = 4096;
    public const ushort HEADER_SIZE = 6;
    public const ushort DATA_SIZE = BUFFER_SIZE - HEADER_SIZE;

    public const ushort HEADER_OFFSET = 0;
    public const ushort META_OFFSET = HEADER_OFFSET + 0;
    public const ushort ID_OFFSET = HEADER_OFFSET + 2;
    public const ushort SEQUENCE_OFFSET = HEADER_OFFSET + 4;
    public const ushort CHECKSUM_OFFSET = HEADER_OFFSET + 5;
    public const ushort DATA_OFFSET = HEADER_OFFSET + 6;

    public const ushort HEADER_ENC_OFFSET = HEADER_OFFSET + 2;
    public const ushort HEADER_ENC_SIZE = HEADER_SIZE - HEADER_ENC_OFFSET;
    public const ushort HEADER_ENC_MASK = 0x8000;

    private readonly ReferenceCounter _referenceCounter;
    private readonly IMessagePool _pool;
    private readonly Memory<byte> _memory;

    public object _lock = new object();

    public MessageMeta Meta
    {
        get => MemoryMarshal.Read<MessageMeta>(this.GetSpan(META_OFFSET, Unsafe.SizeOf<MessageMeta>()));
        set => MemoryMarshal.Write(this.GetSpan(META_OFFSET, Unsafe.SizeOf<MessageMeta>()), ref value);
    }

    public ushort DataSize
    {
        get
        {
            return (ushort)(MemoryMarshal.Read<ushort>(this.GetSpan(META_OFFSET, Unsafe.SizeOf<ushort>())) & ~HEADER_ENC_MASK);
        }
        set
        {
            var span = this.GetSpan(META_OFFSET, Unsafe.SizeOf<ushort>());
            var size = (ushort)(value | (MemoryMarshal.Read<ushort>(span) & HEADER_ENC_MASK));
            MemoryMarshal.Write(this.GetSpan(META_OFFSET, Unsafe.SizeOf<ushort>()), ref size);
        }
    }

    public bool Encrypted
    {
        get => (MemoryMarshal.Read<ushort>(this.GetSpan(META_OFFSET, Unsafe.SizeOf<ushort>())) & HEADER_ENC_MASK) != 0;
        set
        {
            var span = this.GetSpan(META_OFFSET, Unsafe.SizeOf<ushort>());
            var size = (ushort)((MemoryMarshal.Read<ushort>(span) & ~HEADER_ENC_MASK) | (value ? HEADER_ENC_MASK : default));
            MemoryMarshal.Write(this.GetSpan(META_OFFSET, Unsafe.SizeOf<ushort>()), ref size);
        }
    }

    public ushort ReadPosition { get; set; }

    public ushort WritePosition { get; set; }

    public override MessageID ID
    {
        get => MemoryMarshal.Read<MessageID>(this.GetSpan(ID_OFFSET, Unsafe.SizeOf<MessageID>()));
        set => MemoryMarshal.Write(this.GetSpan(ID_OFFSET, Unsafe.SizeOf<MessageID>()), ref value);
    }

    public byte Sequence
    {
        get => _memory.Span[SEQUENCE_OFFSET];
        set => _memory.Span[SEQUENCE_OFFSET] = value;
    }

    public byte Checksum
    {
        get => _memory.Span[CHECKSUM_OFFSET];
        set => _memory.Span[CHECKSUM_OFFSET] = value;
    }

    public int RemainingRead => this.WritePosition - this.ReadPosition;
    public int RemainingWrite => BUFFER_SIZE - this.WritePosition;
    public bool IsRented { get; set; }

    internal Message(IMessagePool pool, Memory<byte> memory)
    {
        _pool = pool;
        _memory = memory;
        _referenceCounter = new ReferenceCounter();
        this.WritePosition = 6;
        this.ReadPosition = 6;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateDataSize()
    {
        // Optimized data size update that keeps the encryption bit.
        var metaSpan = this.GetSpan(META_OFFSET, Unsafe.SizeOf<ushort>());
        var metaValue = MemoryMarshal.Read<ushort>(metaSpan);
        if ((this.WritePosition - HEADER_SIZE) > (metaValue & ~HEADER_ENC_MASK))
        {
            metaValue = (ushort)((this.WritePosition - HEADER_SIZE) | (metaValue & HEADER_ENC_MASK));
            MemoryMarshal.Write(metaSpan, ref metaValue);
        }
    }

    public string ToLoggerString() => $"{this.ID} [{this.Meta} bytes]\n{this.HexDump()}";

    public string HexDump()
    {
        const ushort MAX_DUMP_LENGTH = 512;

        var dump = HexDump(this.GetSpan(DATA_OFFSET, Math.Min(this.WritePosition - HEADER_SIZE, MAX_DUMP_LENGTH)));
        if (this.WritePosition > MAX_DUMP_LENGTH)
            return $"{dump}...larger than {MAX_DUMP_LENGTH}...";

        return dump;
    }

    public static string HexDump(Span<byte> buffer)
    {
        const int bytesPerLine = 16;
        StringBuilder output = new StringBuilder();
        StringBuilder ascii_output = new StringBuilder();
        int length = buffer.Length;
        if (length % bytesPerLine != 0)
        {
            length += bytesPerLine - (length % bytesPerLine);
        }
        for (int x = 0; x <= length; ++x)
        {
            if (x % bytesPerLine == 0)
            {
                if (x > 0)
                {
                    output.Append($"  {ascii_output.ToString()}{Environment.NewLine}");
                    ascii_output.Clear();
                }
                if (x != length)
                {
                    output.Append($"{x:d10}   ");
                }
            }
            if (x < buffer.Length)
            {
                output.Append($"{buffer[x]:X2} ");
                var ch = (char)buffer[x];
                if (!char.IsControl(ch))
                {
                    ascii_output.Append($"{ch}");
                }
                else
                {
                    ascii_output.Append('.');
                }
            }
            else
            {
                output.Append("   ");
                ascii_output.Append('.');
            }
        }
        return output.ToString();
    }

    public Span<byte> GetSpan() => _memory.Span;

    public Span<byte> GetSpan(int start) => _memory.Span[start..];

    public Span<byte> GetSpan(int start, int length) => _memory.Span.Slice(start, length);

    public Memory<byte> GetWrittenMemory() => _memory.Slice(0, this.WritePosition);

    public bool TryCopyTo(Message destination) => this.GetSpan(0, this.WritePosition).TryCopyTo(destination.GetSpan(0, this.WritePosition));

    // ------------------------------------------------

    public override bool TryRead<T>(out T value, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        var size = (ushort)Unsafe.SizeOf<T>();
        if (this.ReadPosition + size > this.WritePosition)
        {
            value = default;
            return false;
        }

        if (!MemoryMarshal.TryRead(this.GetSpan(this.ReadPosition, size), out value))
        {
            value = default;
            return false;
        }

        this.ReadPosition += size;
        return true;
    }

    public override bool TryRead<T>(Span<T> values, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        var valueBytes = MemoryMarshal.AsBytes(values);

        var size = (ushort)valueBytes.Length;
        if (this.ReadPosition + size > this.WritePosition)
        {
            return false;
        }

        if (!this.GetSpan(this.ReadPosition, size).TryCopyTo(valueBytes))
        {
            return false;
        }

        this.ReadPosition += size;
        return true;
    }

    public override bool TryRead([NotNullWhen(true)] out string value, int length, Encoding encoding, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        if (this.ReadPosition + length > this.WritePosition)
        {
            value = null!;
            return false;
        }

        var span = this.GetSpan(this.ReadPosition, length);
        var terminator = span.IndexOf((byte)'\0');
        span = span.Slice(0, terminator == -1 ? length : terminator);
        value = encoding.GetString(span);

        this.ReadPosition += (ushort)length;
        return true;
    }

    // --------------------------------------------------

    public override bool TryWrite<T>(ref T value)
    {
        if (MemoryMarshal.TryWrite(this.GetSpan(this.WritePosition), ref value))
        {
            this.WritePosition += (ushort)Unsafe.SizeOf<T>();
            this.UpdateDataSize();
            return true;
        }
        return false;
    }

    public override bool TryWrite<T>(ReadOnlySpan<T> values)
    {
        var valueBytes = MemoryMarshal.AsBytes(values);
        if (valueBytes.TryCopyTo(this.GetSpan(this.WritePosition)))
        {
            this.WritePosition += (ushort)valueBytes.Length;
            this.UpdateDataSize();
            return true;
        }
        return false;
    }

    public override bool TryWrite(string? value, int length, Encoding encoding)
    {
        if (value is null || length == 0)
            return true; // there is nothing to write, we're good.

        var result = encoding.GetBytes(value, this.GetSpan(this.WritePosition));
        if (result <= 0)
            return false;

        this.WritePosition += (ushort)length;
        this.UpdateDataSize();
        return true;
    }

    public void Reset()
    {
        _referenceCounter.Reset();
        _memory.Span.Clear();
        this.WritePosition = 6;
        this.ReadPosition = 6;
    }

    public void Retain() => _referenceCounter.Retain();
    public void Release() => this.Dispose();
    
    public Message Clone(IMessageAllocator allocator)
    {
        var clone = allocator.NewMsg(this.CallerMemberName, this.CallerFilePath);
        clone.ReceiverID = this.ReceiverID;
        clone.SenderID = this.SenderID;
        clone.WritePosition = this.WritePosition;
        clone.ReadPosition = this.ReadPosition;
        this.TryCopyTo(clone);

        return clone;
    }

    public override void Dispose()
    {
        if (_referenceCounter.Release())
        {
            GC.SuppressFinalize(this);
            _pool.Return(this);
            //MessagePool2.Shared.Return(this);
        }
    }

    ~Message()
    {
        if (Environment.HasShutdownStarted)
            return;

        Log.Warning($"Message was leaked. Created in {this.CallerMemberName}\n({this.CallerFilePath}:{this.CallerFileLine})");
    }
}