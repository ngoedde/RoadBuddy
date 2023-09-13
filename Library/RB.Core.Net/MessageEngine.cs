using System.Runtime.CompilerServices;
using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Messaging.Allocation;
using RB.Core.Net.Common.Messaging.Handling;
using RB.Core.Net.Common.Messaging.Posting;
using RB.Core.Net.Common.Messaging.Pumping;

namespace RB.Core.Net;

public abstract class MessageEngine : IMessageAllocator
{
    protected readonly IDGenerator32 _generator;
    protected readonly IMessageAllocator _msgAllocator;
    private readonly Handling.MessageHandlerManager<Message> _msgHandlerManager;
    protected readonly IMessagePoster _msgPoster;

    protected readonly IMessagePump _pump;


    protected MessageEngine()
    {
        _generator = new IDGenerator32();
        Id = _generator.Next();

        //Regular
        _pump = new MessagePump();
        _msgAllocator = new MessageAllocator(Id);
        _msgPoster = new MessagePoster(_pump);
        _msgHandlerManager = new Handling.MessageHandlerManager<Message>();
    }

    public int Id { get; }

    public Message NewMsg([CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = -1)
    {
        return _msgAllocator.NewMsg(memberName, filePath, lineNumber);
    }

    public Message NewMsg(MessageID id, int receiverID = -1, [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        return _msgAllocator.NewMsg(id, receiverID, memberName, filePath, lineNumber);
    }

    public Message NewLocalMsg(MessageID id, [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        return _msgAllocator.NewLocalMsg(id, memberName, filePath, lineNumber);
    }

    public virtual void Update()
    {
        while (_pump.TryGetMessage(out var msg))
            using (msg)
            {
                if (msg.ReceiverID == Id)
                    //Console.WriteLine($"{nameof(Message)} from #{msg.SenderID}: {msg.ToLoggerString()}");
                    OnMessage(msg);
                else
                    //Console.WriteLine($"{nameof(Message)} to #{msg.ReceiverID}: {msg.ToLoggerString()}");
                    SendMessage(msg);
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
}