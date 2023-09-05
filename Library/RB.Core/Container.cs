using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RB.Core.Network;
using RB.Core.Network.Gateway;
using RB.Core.Network.Gateway.Handler;
using RB.Game.Client;

namespace RB.Core;

public static class Container
{
    public static void AddCoreServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        //Client
        serviceCollection.AddClientFileSystem(configuration);
        
        //NetEngine
        //  Gateway
        serviceCollection.AddSingleton<IGatewayClient, GatewayClient>();
        serviceCollection.AddSingleton<GatewayHandlerGroup>();
        serviceCollection.AddSingleton<IGatewayMsgHandler, IdentificationHandler>();
    }
}