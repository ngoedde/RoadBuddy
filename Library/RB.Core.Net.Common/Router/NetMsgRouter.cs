using RB.Core.Net.Common.Messaging.Allocation;
using RB.Core.Net.Common.Messaging.Posting;

namespace RB.Core.Net.Common.Router;

public class NetMsgRouter : INetMsgRouter
{
    private readonly IMessageAllocator _alloctor;
    private readonly IMessagePoster _poster;

    public NetMsgRouter(IMessageAllocator alloctor, IMessagePoster poster)
    {
        _poster = poster;
        _alloctor = alloctor;
    }

    public bool PostLocalNetConnected(int id)
    {
        using var msg = _alloctor.NewLocalMsg(NetMsgId.LocalNetConnected);
        if (!msg.TryWrite(id)) return false;

        return _poster.PostMsg(msg);
    }

    public bool PostLocalNetDisconnected(int id, DisconnectReason reason)
    {
        using var msg = _alloctor.NewLocalMsg(NetMsgId.LocalNetDisconnected);
        if (!msg.TryWrite(id)) return false;
        if (!msg.TryWrite(reason)) return false;

        return _poster.PostMsg(msg);
    }

    public bool PostLocalNetKeyExchanged(int id)
    {
        using var msg = _alloctor.NewLocalMsg(NetMsgId.LocalNetKeyExchanged);
        if (!msg.TryWrite(id)) return false;

        return _poster.PostMsg(msg);
    }
}