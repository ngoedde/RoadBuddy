using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using RB.Core.Net.Common.Messaging.Serialization;
using RB.Core.Net.Common.Messaging.Serialization.Collection;

namespace RB.Core.Net.Common.Messaging;

public abstract class MessageStream : IMessageReader, IMessageWriter, IDisposable
{
    private const int NS_IN_MS = 1000000;

    public abstract MessageID ID { get; set; }

    public int ReceiverID { get; set; } = -1;
    public int SenderID { get; set; } = -1;

    public string? CallerMemberName { get; set; } = null!;
    public string? CallerFilePath { get; set; } = null!;
    public int CallerFileLine { get; set; } = -1;

    protected MessageStream()
    {
    }

    #region IMessageReader

    public abstract bool TryRead<T>(out T value, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1) where T : unmanaged;

    public abstract bool TryRead<T>(Span<T> values, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1) where T : unmanaged;

    public bool TryRead(out DateTime value, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        value = default;

        if (!this.TryRead(out short year, memberName, filePath, lineNumber)) return false;
        if (!this.TryRead(out short month, memberName, filePath, lineNumber)) return false;
        if (!this.TryRead(out short day, memberName, filePath, lineNumber)) return false;
        if (!this.TryRead(out short hour, memberName, filePath, lineNumber)) return false;
        if (!this.TryRead(out short minute, memberName, filePath, lineNumber)) return false;
        if (!this.TryRead(out short second, memberName, filePath, lineNumber)) return false;
        if (!this.TryRead(out int nanosecond, memberName, filePath, lineNumber)) return false;

        value = new DateTime(year, month, day, hour, minute, second, nanosecond / NS_IN_MS);
        return true;
    }

    public abstract bool TryRead([NotNullWhen(true)] out string value, int length, Encoding encoding, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1);

    public bool TryRead([NotNullWhen(true)] out string value, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
        => this.TryRead(out value, Encoding.ASCII, memberName, filePath, lineNumber);

    public bool TryRead([NotNullWhen(true)] out string value, int length, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
        => this.TryRead(out value, length, Encoding.ASCII, memberName, filePath, lineNumber);

    public bool TryRead([NotNullWhen(true)] out string value, Encoding encoding, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        if (!this.TryRead(out ushort length, memberName, filePath, lineNumber))
        {
            value = null!;
            return false;
        }

        return this.TryRead(out value, length, encoding, memberName, filePath, lineNumber);
    }

    public bool TryDeserialize<T>([NotNullWhen(true)] out T deserializeable, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
        where T : IMessageDeserializer, new()
    {
        deserializeable = new T();
        return deserializeable.TryDeserialize(this);
    }

    public bool TryDeserialize<T>(ICollection<T> collection, IMessageCollectionDeserializer collectionDeserializer, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
        where T : IMessageDeserializer, new()
        => collectionDeserializer.Deserialize(this, collection);

    #endregion IMessageReader

    #region IMessageWriter

    public abstract bool TryWrite<T>(ref T value) where T : unmanaged;

    public bool TryWrite<T>(Span<T> values) where T : unmanaged
        => this.TryWrite((ReadOnlySpan<T>)values);

    public abstract bool TryWrite<T>(ReadOnlySpan<T> values) where T : unmanaged;

    public abstract bool TryWrite(string? value, int length, Encoding encoding);

    public bool TryWrite<T>(T value) where T : unmanaged => this.TryWrite(ref value);

    public bool TryWrite(DateTime value)
    {
        if (!this.TryWrite((short)value.Year)) return false;
        if (!this.TryWrite((short)value.Month)) return false;
        if (!this.TryWrite((short)value.Day)) return false;
        if (!this.TryWrite((short)value.Hour)) return false;
        if (!this.TryWrite((short)value.Minute)) return false;
        if (!this.TryWrite((short)value.Second)) return false;
        if (!this.TryWrite(value.Millisecond * NS_IN_MS)) return false;

        return true;
    }

    public bool TryWrite(string? value) => this.TryWrite(value, Encoding.ASCII);

    public bool TryWrite(string? value, Encoding encoding)
    {
        var length = value?.Length ?? 0;
        if (!this.TryWrite((ushort)length)) return false;
        if (!this.TryWrite(value, length, encoding)) return false;
        return true;
    }

    public bool TryWrite(string? value, int length) => this.TryWrite(value, length, Encoding.ASCII);

    public bool TrySerialize<T>(in T serializeable)
        where T : IMessageSerializer
    {
        if (serializeable == null)
            return false;

        return serializeable.TrySerialize(this);
    }

    public bool TrySerialize<T>(IReadOnlyCollection<T> collection, IMessageCollectionSerializer collectionSerializer)
        where T : IMessageSerializer
        => collectionSerializer.Serialize(this, collection);

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #endregion IMessageWriter
}