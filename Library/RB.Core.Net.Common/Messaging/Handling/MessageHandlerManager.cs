namespace RB.Core.Net.Common.Messaging.Handling;

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

public class AsyncMessageHandlerManager<TMsg> : IAsyncMessageHandlerManager<TMsg>
    where TMsg : MessageStream
{
    private readonly IDictionary<MessageID, AsyncMsgHandler<TMsg>> _handlerMap;

    public AsyncMessageHandlerManager()
    {
        _handlerMap = new Dictionary<MessageID, AsyncMsgHandler<TMsg>>();
    }

    public AsyncMsgHandler<TMsg> this[MessageID id]
    {
        get => _handlerMap[id];
        set => _handlerMap[id] = value;
    }

    public void SetMsgHandler(MessageID id, AsyncMsgHandler<TMsg> handler)
    {
        _handlerMap[id] = handler;
    }

    public ValueTask<bool> Handle(TMsg msg, CancellationToken cancellationToken = default)
    {
        if (!_handlerMap.TryGetValue(msg.ID, out var handler))
            return ValueTask.FromResult(true);

        return handler(msg, cancellationToken);
    }
}