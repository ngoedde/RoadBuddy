// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RB.CLI;
using RB.Core;
using RB.Game.Client.Config;
using AppConfig = RB.CLI.Config.AppConfig;

#region boot

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .AddJsonFile(Path.Combine("Config", "AppConfig.json"))
    .AddJsonFile(Path.Combine("Config", "FileSystem.json"))
    .Build();

ConfigureServices(services, configuration);

var provider = services.BuildServiceProvider(true);

using var app = provider.GetRequiredService<IRoadBuddyApp>();

app.Run();

Console.WriteLine("Exit");

#endregion

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    //Core
    services.AddCoreServices(configuration);

    //Application
    services.Configure<AppConfig>(configuration.Bind);
    
    services.AddSingleton<IRoadBuddyApp, App>();
}