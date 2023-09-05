using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Router;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using RB.Core.Net.Network;
using RB.Core.Net.Network.Memory;
using RB.Core.Net.Network.Tcp;

namespace RB.Core.Net;

public class NetEngine : MessageEngine
{
    protected readonly INetMsgRouter _router;
    protected readonly ISocketPool _socketPool;

    private readonly INetConnector _connector;
    private readonly INetAcceptor _acceptor;
    private readonly INetDisconnecter _disconnector;
    private readonly INetReceiver _receiver;
    private readonly INetSender _sender;


    public INetReceiver Receiver => _receiver;
    public INetSender Sender => _sender;

    private readonly ISessionManager _sessionManager;
    public IReadOnlyCollection<Session> SessionList => _sessionManager;

    public NetEngine()
    {
        _router = new NetMsgRouter(_allocator, _poster);
        _socketPool = new SocketPool();

        _disconnector = new NetDisconnecter(_socketPool, this.Disconnector_Disconnected);
        _connector = new NetConnector(_socketPool, this.Connector_Connected);
        _acceptor = new NetAcceptor(_socketPool, this.Acceptor_Accepted);
        _receiver = new NetReceiver(_disconnector, this.Receiver_Received);
        _sender = new NetSender(_disconnector, this.Sender_Sent);

        _sessionManager = new SessionManager(_generator);
    }

    public void Start(ushort port) => _acceptor.Listen("0.0.0.0", port);

    public void Connect(EndPoint endpoint)
    {
        _connector.Connect(endpoint);
    }

    public bool PostMsg(Message msg) => _poster.PostMsg(msg);

    public bool PostConnect(string? hostOrIP, ushort port)
    {
        using var msg = _allocator.NewLocalMsg(NetMsgID.LOCAL_NET_CONNECT);
        if (!msg.TryWrite(hostOrIP)) return false;
        if (!msg.TryWrite(port)) return false;

        return this.PostMsg(msg);
    }

    public bool PostDisconnect(int id, DisconnectReason reason = DisconnectReason.Intentional)
    {
        using var msg = _allocator.NewLocalMsg(NetMsgID.LOCAL_NET_DISCONNECT);
        if (!msg.TryWrite(id)) return false;
        if (!msg.TryWrite(reason)) return false;

        return this.PostMsg(msg);
    }

    public bool TryFindSessionById(int sessionId, [MaybeNullWhen(false)] out Session session) => _sessionManager.TryFindSessionById(sessionId, out session);

    private void Connector_Connected(Socket socket) => this.OnConnectedSocket(socket);
    private void Acceptor_Accepted(Socket socket) => this.OnAcceptedSocket(socket);

    protected virtual void OnConnectedSocket(Socket socket)
    {
        var session = _sessionManager.CreateSession(socket, new ClientProtocol(_allocator, _poster));
        _router.PostLocalNetConnected(session.Id);
        session.Protocol.Initialize(session.Id); // TODO: Move into handler of NetConnected for thread safety
        _receiver.Receive(session);
        
        Console.WriteLine("Socket connected!");
    }

    protected virtual void OnAcceptedSocket(Socket socket)
    {
        var session = _sessionManager.CreateSession(socket, new ServerProtocol(_allocator, _poster));
        _router.PostLocalNetConnected(this.Id);
        session.Protocol.Initialize(session.Id); // TODO: Move into handler of NetConnected for thread safety
        _receiver.Receive(session);
    }

    private void Sender_Sent(Session session, int bytesTransferred)
    {
    }

    private void Receiver_Received(Session session, Memory<byte> buffer, int bytesTransferred)
    {
        var protocol = session.Protocol;
        if (!protocol.Receive(buffer.Span.Slice(0, bytesTransferred)))
        {
            Console.WriteLine($"{nameof(this.Receiver_Received)}: Failed to build msg.");
            _disconnector.Disconnect(session, DisconnectReason.Intentional);
            return;
        }

        session.KeepAliveInfo.ReportAlive();

        while (protocol.TryGetMessage(out var msg))
        {
            using (msg)
            {
                msg.ReceiverID = this.Id;
                msg.SenderID = session.Id;

                if (!protocol.Decode(msg))
                {
                    Console.WriteLine($"{nameof(this.Receiver_Received)}: Failed to decode {msg.ID}.");
                    _disconnector.Disconnect(session, DisconnectReason.Intentional);
                    return;
                }

                if (!this.PostMsg(msg))
                {
                    Console.WriteLine($"{nameof(this.Receiver_Received)}: Failed to post {msg.ID}.");
                    _disconnector.Disconnect(session, DisconnectReason.Intentional);
                    return;
                }
            }
        }
    }

    private void Disconnector_Disconnected(Session session, DisconnectReason reason)
    {
        _sessionManager.TryRemoveSessionById(session.Id);

        Console.WriteLine($"#{session.Id} ({session.RemoteAddress}) disconnected ({reason})");
        _router.PostLocalNetDisconnected(session.Id, reason);
    }

    protected override bool OnMessage(Message msg)
    {
        if (msg.ID == NetMsgID.NET_KEYEXCHANGE_REQ)
        {
            if (!this.OnKeyExchangeReq(msg))
            {
                this.PostDisconnect(msg.SenderID);
                return false;
            }
        }
        else if (msg.ID == NetMsgID.NET_KEYEXCHANGE_ACK)
        {
            if (!this.OnKeyExchangeAck(msg))
            {
                this.PostDisconnect(msg.SenderID);
                return false;
            }
        }

        if (!base.OnMessage(msg))
        {
            this.PostDisconnect(msg.SenderID);
            return false;
        }

        return true;
    }

    protected override void SendMessage(Message msg)
    {
        if (!_sessionManager.TryFindSessionById(msg.ReceiverID, out var session))
        {
            Console.WriteLine($"{nameof(this.SendMessage)}: Receiver not found.");
            return;
        }

        var protocol = session.Protocol;
        if (!protocol.Encode(msg))
        {
            Console.WriteLine($"{nameof(this.SendMessage)}: Failed to send {msg}.");
            _disconnector.Disconnect(session, DisconnectReason.Intentional);
            return;
        }

        _sender.Send(session, msg);
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
