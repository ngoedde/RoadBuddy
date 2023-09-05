using Microsoft.Extensions.DependencyInjection;
using RB.App.Bot.Network.Gateway.Handler;
using RB.Core.Network.Gateway;

namespace RB.App.Bot;

public static class Container
{
    public static void AddBotServices(this IServiceCollection serviceCollection)
    {
            
        //Gateway connection
        IServiceCollection.AddGatewayServices();
    
        //We need the file system of the client as well
        IServiceCollection.AddClientFileSystem(configuration);

        serviceCollection.AddSingleton<IGatewayMsgHandler, KeyExchangeHandler>();
        serviceCollection.AddSingleton<IGatewayMsgHandler, PatchInfoHandler>();
    }
}