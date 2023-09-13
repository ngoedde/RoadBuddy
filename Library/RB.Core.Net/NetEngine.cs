using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Router;
using RB.Core.Net.Network;
using RB.Core.Net.Network.Memory;
using RB.Core.Net.Network.Tcp;
using Serilog;

namespace RB.Core.Net;

public class NetEngine : MessageEngine
{
    private readonly INetAcceptor _acceptor;

    private readonly INetConnector _connector;
    protected readonly INetDisconnecter _disconnector;
    protected readonly INetMsgRouter _router;

    private readonly ISessionManager _sessionManager;
    protected readonly ISocketPool _socketPool;

    public NetEngine()
    {
        _router = new NetMsgRouter(_msgAllocator, _msgPoster);
        _socketPool = new SocketPool();

        _disconnector = new NetDisconnecter(_socketPool, Disconnector_Disconnected);
        _connector = new NetConnector(_socketPool, Connector_Connected);
        _acceptor = new NetAcceptor(_socketPool, Acceptor_Accepted);
        Receiver = new NetReceiver(_disconnector, Receiver_Received);
        Sender = new NetSender(_disconnector, Sender_Sent);
        _sessionManager = new SessionManager(_generator);
    }

    public INetReceiver Receiver { get; }

    public INetSender Sender { get; }

    public IReadOnlyCollection<Session> SessionList => _sessionManager;

    public void Start(ushort port)
    {
        _acceptor.Listen("0.0.0.0", port);
    }

    public void Connect(EndPoint endpoint)
    {
        _connector.Connect(endpoint);
    }

    public bool PostMsg(Message msg)
    {
        return _msgPoster.PostMsg(msg);
    }

    public bool PostConnect(string? hostOrIP, ushort port)
    {
        using var msg = _msgAllocator.NewLocalMsg(NetMsgId.LocalNetConnect);
        if (!msg.TryWrite(hostOrIP)) return false;
        if (!msg.TryWrite(port)) return false;

        return PostMsg(msg);
    }

    public bool PostDisconnect(int id, DisconnectReason reason = DisconnectReason.Intentional)
    {
        using var msg = _msgAllocator.NewLocalMsg(NetMsgId.LocalNetDisconnect);

        if (!msg.TryWrite(id)) return false;
        if (!msg.TryWrite(reason)) return false;

        return PostMsg(msg);
    }

    public bool TryFindSessionById(int sessionId, [MaybeNullWhen(false)] out Session session)
    {
        return _sessionManager.TryFindSessionById(sessionId, out session);
    }

    private void Connector_Connected(Socket socket)
    {
        OnConnectedSocket(socket);
    }

    private void Acceptor_Accepted(Socket socket)
    {
        OnAcceptedSocket(socket);
    }

    protected virtual void OnConnectedSocket(Socket socket)
    {
        var session = _sessionManager.CreateSession(socket, new ClientProtocol(_msgAllocator, _msgPoster));
        _router.PostLocalNetConnected(session.Id);
        session.Protocol.Initialize(session.Id); // TODO: Move into handler of NetConnected for thread safety
        Receiver.Receive(session);
    }

    protected virtual void OnAcceptedSocket(Socket socket)
    {
        var session = _sessionManager.CreateSession(socket, new ServerProtocol(_msgAllocator, _msgPoster));
        _router.PostLocalNetConnected(Id);
        session.Protocol.Initialize(session.Id); // TODO: Move into handler of NetConnected for thread safety
        Receiver.Receive(session);
    }

    private void Sender_Sent(Session session, int bytesTransferred)
    {
    }

    private void Receiver_Received(Session session, Memory<byte> buffer, int bytesTransferred)
    {
        var protocol = session.Protocol;
        if (!protocol.Receive(buffer.Span.Slice(0, bytesTransferred)))
        {
            Log.Warning($"{nameof(Receiver_Received)}: Failed to build msg.");
            _disconnector.Disconnect(session, DisconnectReason.Intentional);
            return;
        }

        session.KeepAliveInfo.ReportAlive();

        while (protocol.TryGetMessage(out var msg))
            using (msg)
            {
                msg.ReceiverID = Id;
                msg.SenderID = session.Id;

                if (!protocol.Decode(msg))
                {
                    Log.Error($"{nameof(Receiver_Received)}: Failed to decode {msg.ID}.");
                    _disconnector.Disconnect(session, DisconnectReason.Intentional);
                    return;
                }

                if (!PostMsg(msg))
                {
                    Console.WriteLine($"{nameof(Receiver_Received)}: Failed to post {msg.ID}.");
                    _disconnector.Disconnect(session, DisconnectReason.Intentional);
                    return;
                }
            }
    }

    private void Disconnector_Disconnected(Session session, DisconnectReason reason)
    {
        _sessionManager.TryRemoveSessionById(session.Id);

        Log.Debug($"#{session.Id} ({session.RemoteAddress}) disconnected ({reason})");
        _router.PostLocalNetDisconnected(session.Id, reason);
    }

    protected override bool OnMessage(Message msg)
    {
        if (msg.ID == NetMsgId.NetKeyExchangeReq)
        {
            if (!OnKeyExchangeReq(msg))
            {
                PostDisconnect(msg.SenderID);
                return false;
            }
        }
        else if (msg.ID == NetMsgId.NetKeyExchangeAck)
        {
            if (!OnKeyExchangeAck(msg))
            {
                PostDisconnect(msg.SenderID);
                return false;
            }
        }

        if (!base.OnMessage(msg))
        {
            PostDisconnect(msg.SenderID);
            return false;
        }

        return true;
    }

    protected override void SendMessage(Message msg)
    {
        if (!_sessionManager.TryFindSessionById(msg.ReceiverID, out var session))
        {
            Log.Warning($"{nameof(SendMessage)}: Receiver not found.");
            return;
        }

        var protocol = session.Protocol;
        if (!protocol.Encode(msg))
        {
            Log.Error($"{nameof(SendMessage)}: Failed to send {msg}.");
            _disconnector.Disconnect(session, DisconnectReason.Intentional);
            return;
        }

        Sender.Send(session, msg);
    }

    private bool OnKeyExchangeReq(Message msg)
    {
        if (!_sessionManager.TryFindSessionById(msg.SenderID, out var session))
            return false;

        if (!session.Protocol.ProcessReq(msg))
        {
            _disconnector.Disconnect(session, DisconnectReason.Intentional);
            return false;
        }

        return true;
    }

    private bool OnKeyExchangeAck(Message msg)
    {
        if (!_sessionManager.TryFindSessionById(msg.SenderID, out var session))
            return false;

        var result = session.Protocol.ProcessAck(msg);
        if (!result)
        {
            _disconnector.Disconnect(session, DisconnectReason.Intentional);
            return false;
        }

        return true;
    }
}