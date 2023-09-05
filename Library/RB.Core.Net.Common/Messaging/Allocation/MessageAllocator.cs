using System.Runtime.CompilerServices;
using RB.Core.Net.Common.Messaging.Memory;

namespace RB.Core.Net.Common.Messaging.Allocation;

public class MessageAllocator : IMessageAllocator
{
    private readonly MessagePool _messagePool;

    public MessageAllocator(int id)
    {
        this.Id = id;
        _messagePool = new MessagePool();
        _messagePool.Allocate(1024);
    }

    public int Id { get; set; }

    public Message NewMsg([CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        var msg = _messagePool.Rent();
        //var msg = MessagePool2.Shared.Get();
        msg.CallerMemberName = memberName;
        msg.CallerFilePath = filePath;
        msg.CallerFileLine = lineNumber;
        return msg;
    }

    public Message NewMsg(MessageID id, int receiverId = -1, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        var msg = this.NewMsg(memberName, filePath, lineNumber);
        msg.ID = id;
        msg.SenderID = this.Id;
        msg.ReceiverID = receiverId;

        return msg;
    }

    public Message NewLocalMsg(MessageID id, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        var msg = this.NewMsg(memberName, filePath, lineNumber);
        msg.ID = id;
        msg.ReceiverID = this.Id;
        msg.SenderID = this.Id;

        return msg;
    }
}