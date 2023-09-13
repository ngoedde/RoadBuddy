using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using RB.Core.Net.Common.Messaging.Allocation;

namespace RB.Core.Net.Common.Messaging;

public class MassiveMsg : MessageStream, IEnumerable<Message>
{
    private readonly IMassiveMsgAllocator _massiveMsgAllocator;

    private readonly IMessageAllocator _msgAllocator;
    private readonly List<Message> _msgList;
    private Message? _currentMsg;

    private int _currentMsgIndex;

    public MassiveMsg(IMassiveMsgAllocator massiveMsgAllocator, IMessageAllocator msgAllocator,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        _massiveMsgAllocator = massiveMsgAllocator;
        _msgAllocator = msgAllocator;
        _msgList = new List<Message>();
    }

    public override MessageID ID { get; set; }

    public int Size
    {
        get
        {
            var value = 0;
            for (var i = 0; i < _msgList.Count; i++)
                value += _msgList[i].WritePosition;
            return value;
        }
    }

    public ushort RemainMsgCount { get; set; }

    public ushort MsgCount => (ushort)_msgList.Count;

    public IEnumerator<Message> GetEnumerator()
    {
        return _msgList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _msgList.GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Message NextMsg()
    {
        return _msgList[_currentMsgIndex++];
    }

    public MassiveMsg Clone()
    {
        var clone = _massiveMsgAllocator.NewMassiveMsg(CallerMemberName, CallerFilePath);
        clone.ID = ID;
        clone.ReceiverID = ReceiverID;
        clone.SenderID = SenderID;
        clone.RemainMsgCount = MsgCount;

        foreach (var msg in _msgList)
            clone.AppendMessage(msg.Clone(_msgAllocator));

        return clone;
    }

    private Message AllocateDataMsg()
    {
        var msg = _msgAllocator.NewMsg(CallerMemberName, CallerFilePath);
        msg.ID = NetMsgId.FrameworkMassiveReq;
        msg.ReceiverID = ReceiverID;
        msg.SenderID = SenderID;

        msg.TryWrite(MassiveMsgType.Data);

        _msgList.Add(msg);

        _currentMsgIndex++;
        return msg;
    }

    internal Message AllocateHeaderMsg()
    {
        var msg = _msgAllocator.NewMsg(CallerMemberName, CallerFilePath);
        msg.ID = NetMsgId.FrameworkMassiveReq;
        msg.ReceiverID = ReceiverID;
        msg.SenderID = SenderID;

        msg.TryWrite(MassiveMsgType.Header);
        msg.TryWrite(MsgCount);
        msg.TryWrite(ID);

        return msg;
    }

    public bool AppendMessage(Message msg)
    {
        //ToDo: Evaluate if this is the correct way.
        msg.Retain();

        _msgList.Add(msg);

        return --RemainMsgCount == 0;
    }

    public override bool TryRead<T>(out T value, [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        value = default;
        var span = MemoryMarshal.CreateSpan(ref value, 1);
        return TryRead(span, memberName, filePath, lineNumber);
    }

    public override bool TryRead<T>(Span<T> values, [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        var valueBytes = MemoryMarshal.AsBytes(values);
        while (valueBytes.Length > 0)
        {
            if (_currentMsg is null || _currentMsg.RemainingRead == 0)
                _currentMsg = NextMsg();

            var numberOfBytesToRead = Math.Min(_currentMsg.RemainingRead, valueBytes.Length);
            if (!_currentMsg.TryRead(valueBytes.Slice(0, numberOfBytesToRead))) return false;

            valueBytes = valueBytes[numberOfBytesToRead..];
        }

        return true;
    }

    public override bool TryRead([NotNullWhen(true)] out string value, int length, Encoding encoding,
        [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = -1)
    {
        if (unchecked((uint)length) > 4096)
        {
            value = null!;
            return false;
        }

        Span<byte> buffer = stackalloc byte[length];
        if (!TryRead(buffer, memberName, filePath, lineNumber))
        {
            value = null!;
            return false;
        }

        var terminator = buffer.IndexOf((byte)'\0');
        buffer = buffer[..(terminator == -1 ? length : terminator)];
        value = encoding.GetString(buffer);
        return true;
    }

    public override bool TryWrite<T>(ref T value)
    {
        var span = MemoryMarshal.CreateReadOnlySpan(ref value, 1);
        return TryWrite(span);
    }

    public override bool TryWrite<T>(ReadOnlySpan<T> values)
    {
        var valueBytes = MemoryMarshal.AsBytes(values);
        while (valueBytes.Length > 0)
        {
            if (_currentMsg is null || _currentMsg.RemainingWrite == 0)
                _currentMsg = AllocateDataMsg();

            var numberOfBytesToWrite = Math.Min(_currentMsg.RemainingWrite, valueBytes.Length);
            if (!_currentMsg.TryWrite(valueBytes.Slice(0, numberOfBytesToWrite)))
                return false;

            valueBytes = valueBytes[numberOfBytesToWrite..];
        }

        return true;
    }

    public override bool TryWrite(string? value, int length, Encoding encoding)
    {
        if (value is null || length == 0)
            return true; // there is nothing to write, we're good.

        if (unchecked((uint)length) > 4096)
            return false;

        Span<byte> buffer = stackalloc byte[length];
        var numberOfBytesEncoded = encoding.GetBytes(value, buffer);

        return TryWrite(buffer);
    }

    public override void Dispose()
    {
        foreach (var msg in _msgList)
            msg.Release();

        base.Dispose();
    }
}