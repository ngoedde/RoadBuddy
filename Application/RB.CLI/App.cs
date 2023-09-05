using System.Net;
using Microsoft.Extensions.Options;
using RB.CLI.Config;
using RB.Core;
using RB.Core.Net.Common;
using RB.Core.Network.Gateway;

namespace RB.CLI;

public class App : RoadBuddyApp, IRoadBuddyApp
{
    private readonly AppConfig _options;
    private readonly IGatewayClient _gatewayClient;

    public App(
        IOptions<AppConfig> options,
        IGatewayClient gatewayClient,
        GatewayHandlerGroup gatewayHandlerGroup
    ) {
        _options = options.Value;
        _gatewayClient = gatewayClient;
    }

    public new virtual void Run()
    {
        var endpoint = new IPEndPoint(IPAddress.Parse(_options.Gateway.Host), _options.Gateway.Port);
        _gatewayClient.Connect(endpoint);
        
        base.Run();
    }

    protected override void OnUpdate(float deltaTime)
    {
        _gatewayClient.Update();
        
        base.OnUpdate(deltaTime);
    }

    public new void Close()
    {
        _gatewayClient.PostDisconnect(0, DisconnectReason.EngineShutdown);
        
        base.Close();
    }
}