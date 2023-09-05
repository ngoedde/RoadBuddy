using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Messaging.Allocation;
using RB.Core.Net.Common.Messaging.Posting;
using RB.Core.Net.Common.Messaging.Pumping;

using System.Runtime.CompilerServices;
using RB.Core.Net.Handling;

namespace RB.Core.Net;

public abstract class MessageEngine : IMessageAllocator
{
    protected readonly IDGenerator32 _generator;

    public int Id { get; }

    protected readonly IMessagePump _pump;
    protected readonly IMessageAllocator _allocator;
    protected readonly IMessagePoster _poster;

    protected IMessageHandlerManager<Message> _msgHandlerManager;

    protected MessageEngine()
    {
        _generator = new IDGenerator32();
        this.Id = _generator.Next();

        _pump = new MessagePump();
        _allocator = new MessageAllocator(this.Id);
        _poster = new MessagePoster(_pump);
        _msgHandlerManager = new MessageHandlerManager<Message>();
    }

    public virtual void Update()
    {
        while (_pump.TryGetMessage(out var msg))
        {
            using (msg)
            {
                if (msg.ReceiverID == this.Id)
                {
                    //Console.WriteLine($"{nameof(Message)} from #{msg.SenderID}: {msg.ToLoggerString()}");

                    this.OnMessage(msg);
                }
                else
                {
                    //Console.WriteLine($"{nameof(Message)} to #{msg.ReceiverID}: {msg.ToLoggerString()}");

                    this.SendMessage(msg);
                }
            }
        }
    }

    public void SetMsgHandler(MessageID id, MsgHandler<Message> handler)
    {
        _msgHandlerManager.SetMsgHandler(id, handler);
    }

    protected virtual bool OnMessage(Message msg)
    {
        return _msgHandlerManager.Handle(msg);
    }

    protected abstract void SendMessage(Message msg);

    public Message NewMsg([CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        return _allocator.NewMsg(memberName, filePath, lineNumber);
    }

    public Message NewMsg(MessageID id, int receiverID = -1, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        return _allocator.NewMsg(id, receiverID, memberName, filePath, lineNumber);
    }

    public Message NewLocalMsg(MessageID id, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        return _allocator.NewLocalMsg(id, memberName, filePath, lineNumber);
    }
}
