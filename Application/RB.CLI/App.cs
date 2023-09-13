using RB.Bot.Module;
using RB.CLI.Connector;
using RB.Core;
using RB.Core.Network.Agent;

namespace RB.CLI;

public sealed class App : RoadBuddyApp, IRoadBuddyApp
{
    private readonly IAgentClient _agentClient;
    private readonly GatewayConnector _gatewayConnector;
    private readonly BotKernel _kernel;

    public App(
        GatewayConnector gatewayConnector,
        IAgentClient agentClient,
        BotKernel kernel
    )
    {
        _gatewayConnector = gatewayConnector;
        _agentClient = agentClient;
        _kernel = kernel;
    }

    public new void Run()
    {
        _kernel.LoadGameData();
        _gatewayConnector.Connect();

        // Task.Run(() => );}
        base.Run();
    }

    protected override void OnUpdate(float deltaTime)
    {
        _gatewayConnector.Update();
        _agentClient.Update();

        base.OnUpdate(deltaTime);
    }
}