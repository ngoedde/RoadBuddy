using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Messaging.Handling;

namespace RB.Core.Net.Handling;

public class MessageHandlerManager<TMsg> : IMessageHandlerManager<TMsg>
    where TMsg : MessageStream
{
    private readonly IDictionary<MessageID, MsgHandler<TMsg>> _handlerMap;

    public MessageHandlerManager()
    {
        _handlerMap = new Dictionary<MessageID, MsgHandler<TMsg>>();
    }

    public MsgHandler<TMsg> this[MessageID id]
    {
        get => _handlerMap[id];
        set => _handlerMap[id] = value;
    }

    public void SetMsgHandler(MessageID id, MsgHandler<TMsg> handler)
    {
        _handlerMap[id] = handler;
    }

    public bool Handle(TMsg msg)
    {
        if (!_handlerMap.TryGetValue(msg.ID, out var handler))
            return true;

        return handler(msg);
    }
}