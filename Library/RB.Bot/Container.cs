using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RB.Bot.Config;
using RB.Bot.Module;
using RB.Bot.Module.AutoLogin;

namespace RB.Bot;

public static class Container
{
    public static void AddBotModules(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<BotConfig>(configuration.Bind);
        serviceCollection.AddSingleton<BotKernel>();
        serviceCollection.AddSingleton<AutoLoginModule>();
    }
}