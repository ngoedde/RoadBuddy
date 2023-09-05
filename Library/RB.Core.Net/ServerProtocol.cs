using RB.Core.Net.Common;
using RB.Core.Net.Common.Extensions;
using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Messaging.Allocation;
using RB.Core.Net.Common.Messaging.Posting;
using RB.Core.Net.Common.Protocol;
using RB.Core.Net.Common.Protocol.Decoding;
using RB.Core.Net.Common.Protocol.Encoding;
using RB.Core.Net.Common.Protocol.KeyExchange;

using System.Security.Cryptography;

namespace RB.Core.Net;

public class ServerProtocol : Protocol
{
    public ServerProtocol(IMessageAllocator allocator, IMessagePoster poster) : base(allocator, poster, ServerMessageDecoder.Shared, ServerMessageEncoder.Shared)
    {

    }

    public override bool Initialize(int receiverId)
    {
        _context.Options = ProtocolOptions.Encryption | ProtocolOptions.ErrorDetection | ProtocolOptions.KeyExchange;

        using var msg = _allocator.NewMsg(NetMsgID.NET_KEYEXCHANGE_REQ, receiverId);
        msg.TryWrite(_context.Options);

        if ((_context.Options & ProtocolOptions.Encryption) != 0)
        {
            Span<byte> key = stackalloc byte[sizeof(ulong)];
            Random.Shared.NextBytes(key);
            _context.Blowfish.Initialize(key);
            msg.TryWrite(key);
        }

        if ((_context.Options & ProtocolOptions.ErrorDetection) != 0)
        {
            uint sequenceSeed = Random.Shared.NextByte();
            uint checksumSeed = Random.Shared.NextByte();

            _context.Sequencer.Initialize(sequenceSeed);
            _context.Checksummer.Initialize(checksumSeed);

            msg.TryWrite(sequenceSeed);
            msg.TryWrite(checksumSeed);
        }

        if ((_context.Options & ProtocolOptions.KeyExchange) != 0)
        {
            _context.InitialKey = Random.Shared.NextULong();
            _context.Generator = Random.Shared.NextUInt();
            _context.Prime = Random.Shared.NextUInt();
            _context.Private = Random.Shared.NextUInt();
            _context.LocalPublic = KeyExchangeHelper.G_pow_X_mod_P(_context.Generator, _context.Private, _context.Prime);

            msg.TryWrite(_context.InitialKey);
            msg.TryWrite(_context.Generator);
            msg.TryWrite(_context.Prime);
            msg.TryWrite(_context.LocalPublic);
        }

        _context.KeyState = KeyExchangeState.Initialized;
        return _poster.PostMsg(msg);
    }

    protected override KeyExchangeResult OnKeyExchangeReq(Message msg)
    {
        if (_context.KeyState != KeyExchangeState.Initialized)
            return KeyExchangeResult.InvalidState;

        if (!msg.TryRead(out uint recipentPublic))
            return KeyExchangeResult.InvalidMsg;

        _context.RemotePublic = recipentPublic;
        _context.SharedSecret = KeyExchangeHelper.G_pow_X_mod_P(_context.RemotePublic, _context.Private, _context.Prime);

        // Build common secret & local signature
        Span<byte> key = stackalloc byte[sizeof(ulong)];
        KeyExchangeHelper.CalculateKey(key, _context.SharedSecret, _context.LocalPublic, _context.RemotePublic);
        _context.Blowfish.Initialize(key);

        Span<byte> remoteSignature = stackalloc byte[sizeof(ulong)];
        if (!msg.TryRead(remoteSignature)) return KeyExchangeResult.InvalidMsg;

        Span<byte> localSignature = stackalloc byte[sizeof(ulong)];
        KeyExchangeHelper.CalculateSignature(localSignature, _context.SharedSecret, _context.RemotePublic, _context.LocalPublic);
        _context.Blowfish.Encode(localSignature);

        // Compare local and remote signature
        CryptographicOperations.FixedTimeEquals(localSignature, remoteSignature);

        KeyExchangeHelper.CalculateSignature(localSignature, _context.SharedSecret, _context.LocalPublic, _context.RemotePublic);
        _context.Blowfish.Encode(localSignature);

        KeyExchangeHelper.CalculateFinalKey(key, _context.SharedSecret, _context.InitialKey);
        _context.Blowfish.Initialize(key);

        if (!this.PostKeyChallenge(msg.SenderID, localSignature))
            return KeyExchangeResult.InvalidState;

        _context.KeyState = KeyExchangeState.Challenged;
        return KeyExchangeResult.Success;
    }
    protected override KeyExchangeResult OnKeyExchangeAck(Message msg)
    {
        if (_context.KeyState != KeyExchangeState.Challenged)
            return KeyExchangeResult.InvalidState;

        _context.KeyState = KeyExchangeState.Accepted;

        this.PostLocalKeyAccepted(msg.SenderID);
        this.OnKeyExchangeAccepted();

        return KeyExchangeResult.Success;
    }
}
