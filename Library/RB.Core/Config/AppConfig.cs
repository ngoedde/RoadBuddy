namespace RB.Core.Config;

public class AppConfig
{
    public ClientOverridesConfigElement Overrides { get; set; } = new();
}

public class ClientOverridesConfigElement
{
    public ushort? Version { get; set; }
    public byte? ContentId { get; set; }
    
    public EndPointConfigElement? Gateway { get; set; }
    public EndPointConfigElement? Agent { get; set; }
}