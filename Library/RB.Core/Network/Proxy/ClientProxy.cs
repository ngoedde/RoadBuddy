using RB.Core.Net;
using RB.Core.Net.Common.Messaging;

namespace RB.Core.Network.Proxy;

public class ClientProxy : NetServer
{
    private int ClientId = -1;
    
    private readonly ServerEngine _server;

    public ClientProxy(ServerEngine server)
    {
        _server = server;
        _server.ServerMessage += OnServerMessage;
        
    }

    private void OnServerMessage(Message msg)
    {
        //TODO: reroute packet
        msg.ReceiverID = Id;
        
        PostMsg(msg);
    }
    
    protected override bool OnMessage(Message msg)
    {
        // return base.OnMessage(msg);

        msg.ReceiverID = _server.Id;
        
        return _server.PostMsg(msg);
    }
}