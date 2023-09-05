using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Messaging.Allocation;
using RB.Core.Net.Common.Messaging.Posting;
using RB.Core.Net.Common.Protocol;
using RB.Core.Net.Common.Protocol.Decoding;
using RB.Core.Net.Common.Protocol.Encoding;
using RB.Core.Net.Common.Protocol.KeyExchange;

using System.Diagnostics.CodeAnalysis;

namespace RB.Core.Net;

public abstract class Protocol : IProtocol
{
    public KeyExchangeCompletedEventHandler? Accepted { get; set; }

    protected readonly IMessageAllocator _allocator;
    protected readonly IMessagePoster _poster;

    protected readonly IProtocolContext _context;
    private readonly IMessageBuilder _builder;
    private readonly IMessageDecoder _decoder;
    private readonly IMessageEncoder _encoder;

    protected Protocol(IMessageAllocator allocator, IMessagePoster poster, IMessageDecoder decoder, IMessageEncoder encoder)
    {
        _allocator = allocator;
        _poster = poster;
        _context = new ProtocolContext();

        _builder = new MessageBuilder(_allocator, _context);
        _decoder = decoder;
        _encoder = encoder;
    }

    public bool Receive(Span<byte> segment) => _builder.Build(segment);

    public bool TryGetMessage([MaybeNullWhen(false)] out Message message) => _builder.TryGet(out message);

    public bool Decode(Message msg) => _decoder.Decode(_context, msg) == DecodeResult.Success;
    public bool Encode(Message msg) => _encoder.Encode(_context, msg) == EncodeResult.Success;

    public bool SetTrusted(bool trusted) => _context.IsTrusted = trusted;

    protected abstract KeyExchangeResult OnKeyExchangeReq(Message msg);

    protected abstract KeyExchangeResult OnKeyExchangeAck(Message msg);

    protected virtual void OnKeyExchangeAccepted() => this.Accepted?.Invoke();

    public bool ProcessReq(Message msg)
    {
        var result = this.OnKeyExchangeReq(msg);
        return result == KeyExchangeResult.Success;
    }

    public bool ProcessAck(Message msg)
    {
        var result = this.OnKeyExchangeAck(msg);
        return result == KeyExchangeResult.Success;
    }

    public abstract bool Initialize(int receiverId);

    public bool PostKeyChallenge(int receiverId, uint localPublic, Span<byte> localSignature)
    {
        using var msg = _allocator.NewMsg(NetMsgID.NET_KEYEXCHANGE_REQ, receiverId);
        if (!msg.TryWrite(localPublic)) return false;
        if (!msg.TryWrite(localSignature)) return false;

        return _poster.PostMsg(msg);
    }

    public bool PostKeyChallenge(int receiverId, Span<byte> localSignature)
    {
        using var msg = _allocator.NewMsg(NetMsgID.NET_KEYEXCHANGE_REQ, receiverId);
        if (!msg.TryWrite(ProtocolOptions.KeyChallenge)) return false;
        if (!msg.TryWrite(localSignature)) return false;

        return _poster.PostMsg(msg);
    }

    public bool PostKeyAccepted(int receiverId)
    {
        using var msg = _allocator.NewMsg(NetMsgID.NET_KEYEXCHANGE_ACK, receiverId);
        return _poster.PostMsg(msg);
    }

    protected bool PostLocalKeyAccepted(int sessionId)
    {
        using var msg = _allocator.NewLocalMsg(NetMsgID.LOCAL_NET_KEYEXCHANGED);
        if (!msg.TryWrite(sessionId)) return false;

        return _poster.PostMsg(msg);
    }
}
