using RB.Core.Net.Common.Messaging;

namespace RB.Core.Net.Handling;

public interface IMessageHandlerManager<TMsg> where TMsg : MessageStream
{
    MsgHandler<TMsg> this[MessageID id] { get; set; }

    bool Handle(TMsg msg);

    void SetMsgHandler(MessageID id, MsgHandler<TMsg> handler);
}