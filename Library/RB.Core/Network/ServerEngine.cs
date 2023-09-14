using RB.Core.Net;
using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Network.Exception;

namespace RB.Core.Network;

public sealed class ServerEngine : NetClient
{
    public delegate void ContextCreatedEventHandler(ServerContext oldContext, ServerContext newContext);
    public event ContextCreatedEventHandler? ContextCreated;

    public delegate void OnServerMessageEventHandler(Message msg);
    public event OnServerMessageEventHandler? ServerMessage;
    
    public ServerContext Context { get; private set; }

    public ServerEngine(string identity = NetIdentity.SilkroadClient) : base(identity)
    {
        SetMsgHandler(NetMsgId.SetupCordNoDir, OnSetupCord);
    }
    
    private bool OnSetupCord(Message msg)
    {
        var oldContext = Context;

        if (msg.SenderID == msg.ReceiverID)
            return true;

        if (!msg.TryRead(out string identityName))
            return false;
        
        switch (identityName)
        {
            case NetIdentity.AgentServer:
                Context = ServerContext.Agent;
                ServerId = msg.SenderID;
                
                OnContextCreated(oldContext, Context);
                
                break;
            case NetIdentity.GatewayServer:
                ServerId = msg.SenderID;
                Context = ServerContext.Gateway;
                
                OnContextCreated(oldContext, Context);
                
                break;
            default:
                throw new InvalidIdentityException($"{NetIdentity.AgentServer} or {NetIdentity.GatewayServer}", identityName);
        }

        return true;
    }
    
    private void OnContextCreated(ServerContext oldContext, ServerContext newContext)
    {
        ContextCreated?.Invoke(oldContext, newContext);
    }

    protected override bool OnMessage(Message msg)
    {
        OnServerMessage(msg);

        return true;
    }

    private void OnServerMessage(Message msg)
    {
        ServerMessage?.Invoke(msg);
    }
}