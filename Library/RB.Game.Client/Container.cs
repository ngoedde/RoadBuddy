using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RB.Game.Client.Config;
using RB.Game.Client.ResourceLoader.CharacterData;
using RB.Game.Client.ResourceLoader.DivisionInfo;
using RB.Game.Client.ResourceLoader.GatePort;
using RB.Game.Client.ResourceLoader.ItemData;
using RB.Game.Client.ResourceLoader.SkillData;
using RB.Game.Client.ResourceLoader.SkillData.Cryptography;
using RB.Game.Client.ResourceLoader.VersionInfo;
using RB.Game.Client.Service;

namespace RB.Game.Client;

public static class Container
{
    public static void AddClientFileSystem(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        //Config
        serviceCollection.Configure<FileSystemConfig>(configuration.Bind);

        //ResLoader
        serviceCollection.AddSingleton<IClientFileSystem, ClientFileSystem>();
        serviceCollection.AddSingleton<IVersionInfoLoader, VersionInfoLoader>();
        serviceCollection.AddSingleton<IDivisionInfoLoader, DivisionInfoLoader>();
        serviceCollection.AddSingleton<IGatewayPortLoader, GatewayPortLoader>();
        serviceCollection.AddSingleton<IItemDataLoader, ItemDataLoader>();
        serviceCollection.AddSingleton<ISkillDataLoader, SkillDataLoader>();
        serviceCollection.AddSingleton<ICharacterDataLoader, CharacterDataLoader>();

        //Services
        serviceCollection.AddSingleton<IVersionInfoService, VersionInfoService>();
        serviceCollection.AddSingleton<IDivisionInfoService, DivisionInfoService>();
        serviceCollection.AddSingleton<IGatewayPortInfoService, GatewayPortInfoInfoService>();
        serviceCollection.AddSingleton<IGameDataService, GameDataService>();
        
        serviceCollection.AddSingleton<SkillCryptographyProvider>();
    }
}