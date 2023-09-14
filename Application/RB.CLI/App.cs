using RB.Bot.Module;
using RB.CLI.Connector;
using RB.Core;
using RB.Core.Network;
using RB.Core.Network.Agent;

namespace RB.CLI;

public sealed class App : RoadBuddyApp, IRoadBuddyApp
{
    private readonly GatewayConnector _gatewayConnector;
    private readonly BotKernel _kernel;
    private readonly ServerEngine _networkEngine;

    public App(
        GatewayConnector gatewayConnector,
        BotKernel kernel,
        ServerEngine networkEngine
    )
    {
        _gatewayConnector = gatewayConnector;
        _kernel = kernel;
        _networkEngine = networkEngine;
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
        _networkEngine.Update();
   
        base.OnUpdate(deltaTime);
    }
}