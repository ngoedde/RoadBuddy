using RB.CLI.Connector;
using RB.Core;

namespace RB.CLI;

public sealed class App : RoadBuddyApp, IRoadBuddyApp
{
    private readonly GatewayConnector _gatewayConnector;
    
    public App(
        GatewayConnector gatewayConnector
    )
    {
        _gatewayConnector = gatewayConnector;
    }

    public new void Run()
    {
        _gatewayConnector.Connect();
        
        base.Run();
    }

    
    protected override void OnUpdate(float deltaTime)
    {
        _gatewayConnector.Update();

        base.OnUpdate(deltaTime);
    }
}