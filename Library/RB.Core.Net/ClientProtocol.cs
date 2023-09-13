using RB.Core.Net.Common.Extensions;
using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Messaging.Allocation;
using RB.Core.Net.Common.Messaging.Posting;
using RB.Core.Net.Common.Protocol;
using RB.Core.Net.Common.Protocol.Decoding;
using RB.Core.Net.Common.Protocol.Encoding;
using RB.Core.Net.Common.Protocol.KeyExchange;

namespace RB.Core.Net;

internal class ClientProtocol : Protocol
{
    public ClientProtocol(IMessageAllocator allocator, IMessagePoster poster) : base(allocator, poster,
        ClientMessageDecoder.Shared, ClientMessageEncoder.Shared)
    {
    }

    public override bool Initialize(int receiverId)
    {
        _context.Private = Random.Shared.NextUInt();
        return true;
    }

    protected override KeyExchangeResult OnKeyExchangeReq(Message msg)
    {
        if (!msg.TryRead(out ProtocolOptions option)) return KeyExchangeResult.InvalidMsg;

        // Set encoding options if we have none
        if (_context.Options == ProtocolOptions.None)
            _context.Options = option;

        if ((option & ProtocolOptions.Encryption) != 0)
        {
            Span<byte> insecureKey = stackalloc byte[sizeof(ulong)];
            if (!msg.TryRead(insecureKey)) return KeyExchangeResult.InvalidMsg;

            _context.Blowfish.Initialize(insecureKey);
        }

        if ((option & ProtocolOptions.ErrorDetection) != 0)
        {
            if (!msg.TryRead(out uint sequenceSeed)) return KeyExchangeResult.InvalidMsg;
            if (!msg.TryRead(out uint checksumSeed)) return KeyExchangeResult.InvalidMsg;

            _context.Sequencer.Initialize(sequenceSeed);
            _context.Checksummer.Initialize(checksumSeed);
        }

        if ((option & ProtocolOptions.KeyExchange) != 0)
            return KeyExchange(msg);

        if ((option & ProtocolOptions.KeyChallenge) != 0)
            return KeyChallenge(msg);

        PostKeyAccepted(msg.SenderID);
        _context.KeyState = KeyExchangeState.Accepted;

        PostLocalKeyAccepted(msg.SenderID);
        OnKeyExchangeAccepted();

        return KeyExchangeResult.Success;
    }

    protected override KeyExchangeResult OnKeyExchangeAck(Message msg)
    {
        // We're not supposed to receive 0x9000, they preferred to answer 0x5000 with 0x5000 -.-
        return KeyExchangeResult.InvalidMsg;
    }

    private KeyExchangeResult KeyExchange(Message msg)
    {
        if (_context.KeyState != KeyExchangeState.Uninitialized)
            return KeyExchangeResult.InvalidState;

        if (!msg.TryRead(out ServerKeyInfo senderInfo))
            return KeyExchangeResult.InvalidMsg;

        _context.InitialKey = senderInfo.Key;
        _context.Generator = senderInfo.Generator;
        _context.Prime = senderInfo.Prime;
        _context.LocalPublic = KeyExchangeHelper.G_pow_X_mod_P(_context.Generator, _context.Private, _context.Prime);

        _context.RemotePublic = senderInfo.Public;
        _context.SharedSecret =
            KeyExchangeHelper.G_pow_X_mod_P(_context.RemotePublic, _context.Private, _context.Prime);

        Span<byte> key = stackalloc byte[sizeof(ulong)];
        KeyExchangeHelper.CalculateKey(key, _context.SharedSecret, _context.RemotePublic, _context.LocalPublic);
        _context.Blowfish.Initialize(key);

        Span<byte> localSignature = stackalloc byte[sizeof(ulong)];
        KeyExchangeHelper.CalculateSignature(localSignature, _context.SharedSecret, _context.LocalPublic,
            _context.RemotePublic);
        _context.Blowfish.Encode(localSignature);

        if (!PostKeyChallenge(msg.SenderID, _context.LocalPublic, localSignature))
            return KeyExchangeResult.InvalidState;

        _context.KeyState = KeyExchangeState.Initialized;
        return KeyExchangeResult.Success;
    }

    private KeyExchangeResult KeyChallenge(Message msg)
    {
        if (_context.KeyState != KeyExchangeState.Initialized)
            return KeyExchangeResult.InvalidState;

        Span<byte> remoteSignature = stackalloc byte[sizeof(ulong)];
        if (!msg.TryRead(remoteSignature)) return KeyExchangeResult.InvalidMsg;

        Span<byte> localSignature = stackalloc byte[sizeof(ulong)];
        KeyExchangeHelper.CalculateSignature(localSignature, _context.SharedSecret, _context.RemotePublic,
            _context.LocalPublic);
        _context.Blowfish.Encode(localSignature);

        // Compare local and remote signature
        for (var i = 0; i < sizeof(ulong); i++)
            if (localSignature[i] != remoteSignature[i])
                return KeyExchangeResult.InvalidSignature;

        Span<byte> key = stackalloc byte[sizeof(ulong)];
        KeyExchangeHelper.CalculateFinalKey(key, _context.SharedSecret, _context.InitialKey);
        _context.Blowfish.Initialize(key);

        PostKeyAccepted(msg.SenderID);
        _context.KeyState = KeyExchangeState.Accepted;

        PostLocalKeyAccepted(msg.SenderID);
        OnKeyExchangeAccepted();

        return KeyExchangeResult.Success;
    }
}