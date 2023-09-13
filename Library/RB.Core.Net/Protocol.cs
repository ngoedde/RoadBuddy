using System.Diagnostics.CodeAnalysis;
using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Messaging.Allocation;
using RB.Core.Net.Common.Messaging.Posting;
using RB.Core.Net.Common.Protocol;
using RB.Core.Net.Common.Protocol.Decoding;
using RB.Core.Net.Common.Protocol.Encoding;
using RB.Core.Net.Common.Protocol.KeyExchange;

namespace RB.Core.Net;

public abstract class Protocol : IProtocol
{
    protected readonly IMessageAllocator _allocator;
    private readonly IMessageBuilder _builder;

    protected readonly IProtocolContext _context;
    private readonly IMessageDecoder _decoder;
    private readonly IMessageEncoder _encoder;
    protected readonly IMessagePoster _poster;

    protected Protocol(IMessageAllocator allocator, IMessagePoster poster, IMessageDecoder decoder,
        IMessageEncoder encoder)
    {
        _allocator = allocator;
        _poster = poster;
        _context = new ProtocolContext();

        _builder = new MessageBuilder(_allocator, _context);
        _decoder = decoder;
        _encoder = encoder;
    }

    public KeyExchangeCompletedEventHandler? Accepted { get; set; }

    public bool Receive(Span<byte> segment)
    {
        return _builder.Build(segment);
    }

    public bool TryGetMessage([MaybeNullWhen(false)] out Message message)
    {
        return _builder.TryGet(out message);
    }

    public bool Decode(Message msg)
    {
        return _decoder.Decode(_context, msg) == DecodeResult.Success;
    }

    public bool Encode(Message msg)
    {
        return _encoder.Encode(_context, msg) == EncodeResult.Success;
    }

    public bool SetTrusted(bool trusted)
    {
        return _context.IsTrusted = trusted;
    }

    public bool ProcessReq(Message msg)
    {
        var result = OnKeyExchangeReq(msg);
        return result == KeyExchangeResult.Success;
    }

    public bool ProcessAck(Message msg)
    {
        var result = OnKeyExchangeAck(msg);
        return result == KeyExchangeResult.Success;
    }

    public abstract bool Initialize(int receiverId);

    protected abstract KeyExchangeResult OnKeyExchangeReq(Message msg);

    protected abstract KeyExchangeResult OnKeyExchangeAck(Message msg);

    protected virtual void OnKeyExchangeAccepted()
    {
        Accepted?.Invoke();
    }

    public bool PostKeyChallenge(int receiverId, uint localPublic, Span<byte> localSignature)
    {
        using var msg = _allocator.NewMsg(NetMsgId.NetKeyExchangeReq, receiverId);
        if (!msg.TryWrite(localPublic)) return false;
        if (!msg.TryWrite(localSignature)) return false;

        return _poster.PostMsg(msg);
    }

    public bool PostKeyChallenge(int receiverId, Span<byte> localSignature)
    {
        using var msg = _allocator.NewMsg(NetMsgId.NetKeyExchangeReq, receiverId);
        if (!msg.TryWrite(ProtocolOptions.KeyChallenge)) return false;
        if (!msg.TryWrite(localSignature)) return false;

        return _poster.PostMsg(msg);
    }

    public bool PostKeyAccepted(int receiverId)
    {
        using var msg = _allocator.NewMsg(NetMsgId.NetKeyExchangeAck, receiverId);

        return _poster.PostMsg(msg);
    }

    protected bool PostLocalKeyAccepted(int sessionId)
    {
        using var msg = _allocator.NewLocalMsg(NetMsgId.LocalNetKeyExchanged);
        if (!msg.TryWrite(sessionId)) return false;

        return _poster.PostMsg(msg);
    }
}