// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RB.CLI;
using RB.CLI.Connector;
using RB.Core;
using Serilog;
using AppConfig = RB.Core.Config.AppConfig;

#region boot

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .AddJsonFile(Path.Combine("Config", "AppConfig.json"))
    .AddJsonFile(Path.Combine("Config", "FileSystem.json"))
    .Build();

ConfigureLogger();
ConfigureServices(services, configuration);

var provider = services.BuildServiceProvider(true);

using var app = provider.GetRequiredService<IRoadBuddyApp>();
app.Run();

Console.WriteLine("Exit");

#endregion

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    //Config
    services.Configure<AppConfig>(configuration.Bind);
    
    //Bot Core
    services.AddCoreServices(configuration);

    //Application
    services.AddSingleton<IRoadBuddyApp, App>();
    services.AddSingleton<GatewayConnector>();
}

static void ConfigureLogger()
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File(Environment.CurrentDirectory + "log.txt")
        .CreateLogger();
}