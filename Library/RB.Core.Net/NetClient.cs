using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Messaging.Allocation;
using RB.Core.Net.Common.Messaging.Handling;
using RB.Core.Net.Common.Messaging.Posting;
using Serilog;

namespace RB.Core.Net;

public abstract class NetClient : NetEngine, INetClient
{
    public string Identity { get; }

    protected readonly IMassiveMsgPoster _massiveMsgPoster;
    protected IMessageHandlerManager<MassiveMsg> _massiveMsgManager;
    
    private MassiveMsg? _massiveMsg;
    private readonly MassiveMsgAllocator _massiveMsgAllocator;

    public int ServerId { get; private set; } = -1;

    protected NetClient(string identity)
    {
        Identity = identity;
        
        //Massive handling
        _massiveMsgAllocator = new MassiveMsgAllocator(this.Id, _msgAllocator);
        _massiveMsgPoster = new MassiveMsgPoster(_msgPoster);
        _massiveMsgManager = new MessageHandlerManager<MassiveMsg>();
        
        SetMsgHandler(NetMsgId.FrameworkMassiveReq, OnMassiveMsgReq);
    }

    #region Massive packet handling
    
    public bool PostMsg(MassiveMsg msg) => _massiveMsgPoster.PostMsg(msg);

    public MassiveMsg NewMassiveMsg(string? memberName = null, string? filePath = null, int lineNumber = -1) => _massiveMsgAllocator.NewMassiveMsg(memberName, filePath, lineNumber);

    public MassiveMsg NewMassiveMsg(MessageID id, int receiverID = -1, string? memberName = null, string? filePath = null,
        int lineNumber = -1) => _massiveMsgAllocator.NewMassiveMsg(id, receiverID, memberName, filePath, lineNumber);

    public MassiveMsg NewLocalMassiveMsg(MessageID id, string? memberName = null, string? filePath = null, int lineNumber = -1) => _massiveMsgAllocator.NewLocalMassiveMsg(id, memberName, filePath, lineNumber);
    
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

    #region Identify

    protected override void SendMessage(Message msg)
    {
        base.SendMessage(msg);
        
        if (msg.ID == NetMsgId.NetKeyExchangeAck)
            SendIdentity(msg.ReceiverID);
    }
    
    private void SendIdentity(int receiverId)
    {
        ServerId = receiverId;
        
        using var setupCord = NewMsg(NetMsgId.SetupCordNoDir, receiverId);

        if (!setupCord.TryWrite(Identity)) return;
        if (!setupCord.TryWrite(false)) return; //IsLocal = false
        
        Log.Debug($"Sending identity [{Identity}]");

        PostMsg(setupCord);
    }
    
    #endregion
}