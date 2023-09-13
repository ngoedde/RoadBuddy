using System.Diagnostics;
using RB.Bot.Module.AutoLogin;
using RB.Core;
using RB.Game.Client.Service;
using Serilog;

namespace RB.Bot.Module;

public class BotKernel
{
    private readonly IGameDataService _clientDataService;

    public BotKernel(
        Kernel rbKernel,
        AutoLoginModule autoLoginModule,
        IGameDataService clientDataService
    )
    {
        _clientDataService = clientDataService;
    }

    public void LoadGameData()
    {
        var sw = Stopwatch.StartNew();
        _clientDataService.LoadGameData();

        Log.Information($"Game data loaded in [{sw.ElapsedMilliseconds}ms]");
    }

    public void Tick()
    {
    }
}