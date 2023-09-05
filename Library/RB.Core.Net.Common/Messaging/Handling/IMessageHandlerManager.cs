namespace RB.Core.Net.Common.Messaging.Handling;

public delegate bool MsgHandler<in TMsg>(TMsg msg) where
    TMsg : MessageStream;

public delegate ValueTask<bool> AsyncMsgHandler<in TMsg>(TMsg msg, CancellationToken cancellationToken = default) where
    TMsg : MessageStream;

public interface IMessageHandlerManager<TMsg> where TMsg : MessageStream
{
    MsgHandler<TMsg> this[MessageID id] { get; set; }

    bool Handle(TMsg msg);

    void SetMsgHandler(MessageID id, MsgHandler<TMsg> handler);
}

public interface IAsyncMessageHandlerManager<TMsg> where TMsg : MessageStream
{
    AsyncMsgHandler<TMsg> this[MessageID id] { get; set; }

    ValueTask<bool> Handle(TMsg msg, CancellationToken cancellationToken = default);

    void SetMsgHandler(MessageID id, AsyncMsgHandler<TMsg> handler);
}