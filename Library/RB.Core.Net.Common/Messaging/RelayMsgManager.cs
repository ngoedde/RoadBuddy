using RB.Core.Net.Common.Messaging.Allocation;

namespace RB.Core.Net.Common.Messaging;

//ToDo: Proper support for massive packets as well?
public class RelayMsgManager
{
    private readonly IMessageAllocator _msgAllocator;
    private readonly List<RelayMsg> _messages;

    public RelayMsgManager(IMessageAllocator msgAllocator)
    {
        _msgAllocator = msgAllocator;
        _messages = new List<RelayMsg>();
    }

    public void Add(RelayMsg msg)
    {
        _messages.Add(msg);
    }

    public void Handle(Message msg)
    {
        //Only 1 opcode can be registered at the same time!
        var relayMsg = _messages.FirstOrDefault(m => m.ResponseMsgId == msg.ID);

        if (relayMsg == null)
            return;

        var clone = msg.Clone(_msgAllocator);
        _messages.Remove(relayMsg);

        relayMsg.TrySetResult(clone);
    }
}