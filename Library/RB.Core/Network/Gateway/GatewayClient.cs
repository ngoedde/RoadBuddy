using RB.Core.Net;
using RB.Core.Net.Common.Messaging;

namespace RB.Core.Network.Gateway;

public class GatewayClient : NetClient, IGatewayClient
{
    public const string ExpectedIdentity = "GatewayServer";

    protected override bool OnMessage(Message msg)
    {
        Console.WriteLine($"Message {msg.ID}");
        
        return base.OnMessage(msg);
    }
}