using RB.Core.Network.Agent;
using RB.Core.Network.Gateway;

namespace RB.Core;

public class Kernel
{
    private readonly AgentHandlerGroup _agentHandlerGroup;
    private readonly GatewayHandlerGroup _gatewayHandlerGroup;

    public Kernel(
        AgentHandlerGroup agentHandlerGroup,
        GatewayHandlerGroup gatewayHandlerGroup
    )
    {
        _agentHandlerGroup = agentHandlerGroup;
        _gatewayHandlerGroup = gatewayHandlerGroup;
    }
}