using System.Runtime.CompilerServices;
using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Messaging.Allocation;
using RB.Core.Net.Common.Messaging.Handling;
using RB.Core.Net.Common.Messaging.Posting;
using Serilog;

namespace RB.Core.Net;

public abstract class NetClient : NetEngine, INetClient
{
    private readonly MassiveMsgAllocator _massiveMsgAllocator;
    private readonly IMessageHandlerManager<MassiveMsg> _massiveMsgManager;

    private readonly IMassiveMsgPoster _massiveMsgPoster;

    private MassiveMsg? _massiveMsg;
    private readonly IRelayMsgAllocator _relayMsgAllocator;

    private readonly RelayMsgManager _relayMsgManager;

    protected NetClient(string identity)
    {
        Identity = identity;

        //Massive handling
        _massiveMsgAllocator = new MassiveMsgAllocator(Id, _msgAllocator);
        _massiveMsgPoster = new MassiveMsgPoster(_msgPoster);
        _massiveMsgManager = new MessageHandlerManager<MassiveMsg>();

        //Relay
        _relayMsgManager = new RelayMsgManager(_msgAllocator);
        _relayMsgAllocator = new RelayMsgAllocator();

        SetMsgHandler(NetMsgId.FrameworkMassiveReq, OnMassiveMsgReq);
    }

    public string Identity { get; }

    public int ServerId { get; protected set; } = -1;

    protected virtual void OnConnected()
    {
        Connected?.Invoke();
    }

    protected virtual void OnDisconnected()
    {
        Disconnected?.Invoke();
    }

    #region Massive packet handling

    public bool PostMsg(MassiveMsg msg)
    {
        return _massiveMsgPoster.PostMsg(msg);
    }

    public MassiveMsg NewMassiveMsg(string? memberName = null, string? filePath = null, int lineNumber = -1)
    {
        return _massiveMsgAllocator.NewMassiveMsg(memberName, filePath, lineNumber);
    }

    public MassiveMsg NewMassiveMsg(MessageID id, int receiverID = -1, string? memberName = null,
        string? filePath = null,
        int lineNumber = -1)
    {
        return _massiveMsgAllocator.NewMassiveMsg(id, receiverID, memberName, filePath, lineNumber);
    }

    public MassiveMsg NewLocalMassiveMsg(MessageID id, string? memberName = null, string? filePath = null,
        int lineNumber = -1)
    {
        return _massiveMsgAllocator.NewLocalMassiveMsg(id, memberName, filePath, lineNumber);
    }

    private bool OnMassiveMsgReq(Message msg)
    {
        if (!msg.TryRead(out MassiveMsgType type)) return false;

        switch (type)
        {
            //Data
            case MassiveMsgType.Data:
                return OnMassiveMsgData(msg);
            //Header
            case MassiveMsgType.Header:
                return OnMassiveMsgHeader(msg);
            default:
                Log.Warning($"Unexpected massive message type {type}");

                return false;
        }
    }

    private bool OnMassiveMsgHeader(Message msg)
    {
        if (!msg.TryRead(out ushort msgCount))
            return false;

        if (!msg.TryRead(out MessageID realMsgID))
            return false;

        if (_massiveMsg is not null)
        {
            _massiveMsg.Dispose();

            return false;
        }

        _massiveMsg = _massiveMsgAllocator.NewMassiveMsg(realMsgID, msg.ReceiverID);
        _massiveMsg.SenderID = msg.SenderID;
        _massiveMsg.RemainMsgCount = msgCount;

        return true;
    }

    private bool OnMassiveMsgData(Message msg)
    {
        if (_massiveMsg is null) return false;

        if (!_massiveMsg.AppendMessage(msg)) return true;

        try
        {
            OnMessage(_massiveMsg);
        }
        finally
        {
            _massiveMsg.Dispose();
            _massiveMsg = null;
        }

        return true;
    }

    protected virtual bool OnMessage(MassiveMsg massiveMsg)
    {
        return _massiveMsgManager.Handle(massiveMsg);
    }


    public void SetMsgHandler(MessageID id, MsgHandler<MassiveMsg> handler)
    {
        _massiveMsgManager.SetMsgHandler(id, handler);
    }

    #endregion

    #region Relay msg

    public ConfiguredTaskAwaitable<Message?> PostRelayMsg(RelayMsg msg)
    {
        PostMsg(msg.Request);

        _relayMsgManager.Add(msg);

        return msg.WaitAsync();
    }

    protected override bool OnMessage(Message msg)
    {
        _relayMsgManager.Handle(msg);

        if (msg.ID == NetMsgId.LocalNetDisconnected)
            OnDisconnected();

        if (msg.ID == NetMsgId.LocalNetConnected)
            OnConnected();

        return base.OnMessage(msg);
    }

    public RelayMsg NewRelayMsg(Message request, MessageID responseMsgId, string? memberName = null,
        string? filePath = null,
        int lineNumber = -1)
    {
        return _relayMsgAllocator.NewRelayMsg(request, responseMsgId, memberName, filePath, lineNumber);
    }

    public RelayMsg NewRelayMsg(Message request, MessageID responseMsgId, int receiverID = -1,
        string? memberName = null,
        string? filePath = null, int lineNumber = -1)
    {
        return _relayMsgAllocator.NewRelayMsg(request, responseMsgId, receiverID, memberName, filePath, lineNumber);
    }

    #endregion

    #region Module identification

    protected override void SendMessage(Message msg)
    {
        base.SendMessage(msg);

        if (msg.ID == NetMsgId.NetKeyExchangeAck)
            SendIdentity(msg.ReceiverID);
    }

    protected virtual void SendIdentity(int receiverId)
    {
        ServerId = receiverId;

        using var setupCord = NewMsg(NetMsgId.SetupCordNoDir, receiverId);

        if (!setupCord.TryWrite(Identity)) return;
        if (!setupCord.TryWrite(false)) return; //IsLocal = false

        Log.Debug($"Sending identity [{Identity}]");

        PostMsg(setupCord);
    }

    public void Disconnect()
    {
        if (!TryFindSessionById(ServerId, out var session))
            return;

        _disconnector.Disconnect(session, DisconnectReason.EngineShutdown);
    }

    public event INetClient.OnDisconnectedEventHandler? Disconnected;
    public event INetClient.OnConnectedEventHandler? Connected;

    #endregion
}