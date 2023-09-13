using System.Runtime.CompilerServices;

namespace RB.Core.Net.Common.Messaging.Allocation;

public class MassiveMsgAllocator : IMassiveMsgAllocator
{
    private readonly IMessageAllocator _msgAllocator;

    public MassiveMsgAllocator(int id, IMessageAllocator msgAllocator)
    {
        Id = id;
        _msgAllocator = msgAllocator;
    }

    public int Id { get; set; }

    public MassiveMsg NewMassiveMsg(
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = -1)
    {
        return new MassiveMsg(this, _msgAllocator, memberName, filePath, lineNumber);
    }

    public MassiveMsg NewLocalMassiveMsg(
        MessageID id,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = -1)
    {
        var msg = NewMassiveMsg(memberName, filePath, lineNumber);
        msg.ID = id;
        msg.SenderID = Id;
        msg.ReceiverID = Id;
        return msg;
    }

    public MassiveMsg NewMassiveMsg(
        MessageID id,
        int receiverID = -1,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = -1)
    {
        var msg = NewMassiveMsg(memberName, filePath, lineNumber);
        msg.ID = id;
        msg.SenderID = Id;
        msg.ReceiverID = receiverID;
        return msg;
    }
}