using RB.Bot.Config.Elements;

namespace RB.Bot.Config;

public class BotConfig
{
    public AutoLoginConfigElement AutoLogin { get; set; } = new();
}