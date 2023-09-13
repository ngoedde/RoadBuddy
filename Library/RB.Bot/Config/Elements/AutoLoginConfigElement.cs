namespace RB.Bot.Config.Elements;

public class AutoLoginConfigElement
{
    public bool Enabled { get; set; } = false;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Captcha { get; set; } = null;
    public string Shard { get; set; } = string.Empty;
    public string Character { get; set; } = string.Empty;
}