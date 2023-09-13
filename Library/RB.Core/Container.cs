using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RB.Core.Network.Agent;
using RB.Core.Network.Gateway;
using RB.Core.Network.Gateway.Handler;
using RB.Core.Service;
using RB.Core.Service.Agent;
using RB.Core.Service.Gateway;
using RB.Game.Client;
using LoginService = RB.Core.Service.Gateway.LoginService;

namespace RB.Core;

public static class Container
{
    public static void AddRoadBuddyKernel(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton<Kernel>();

        //SR_Client
        serviceCollection.AddClientFileSystem(configuration);

        //NetEngine
        //  GatewayServer
        serviceCollection.AddSingleton<IGatewayClient, GatewayClient>();
        serviceCollection.AddSingleton<PatchInfoService>();
        serviceCollection.AddSingleton<GatewayHandlerGroup>();
        serviceCollection.AddSingleton<IGatewayMsgHandler, IdentificationHandler>();
        serviceCollection.AddSingleton<ShardInfoService>();
        serviceCollection.AddSingleton<LoginService>();
        serviceCollection.AddSingleton<CharacterSelectionService>();

        //NetEngine
        //  AgentServer
        serviceCollection.AddSingleton<IAgentClient, AgentClient>();
        serviceCollection.AddSingleton<AgentHandlerGroup>();
        serviceCollection.AddSingleton<Service.Agent.LoginService>();
        serviceCollection.AddSingleton<ContextSwitcher>();
    }
}