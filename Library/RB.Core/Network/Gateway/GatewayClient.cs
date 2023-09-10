using RB.Core.Net;

namespace RB.Core.Network.Gateway;

public class GatewayClient : NetClient, IGatewayClient
{
    public GatewayClient() : base(NetIdentity.SilkroadClient)
    {
    }
}