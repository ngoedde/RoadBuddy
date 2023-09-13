using RB.Core.Net;

namespace RB.Core.Network.Agent;

public class AgentClient : NetClient, IAgentClient
{
    public AgentClient() : base(NetIdentity.SilkroadClient)
    {
    }
}