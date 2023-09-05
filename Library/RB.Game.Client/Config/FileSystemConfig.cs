namespace RB.Game.Client.Config;

public class FileSystemConfig
{
    public PackFileConfigElement Media { get; set; }
    public PackFileConfigElement Data { get; set; }
    
    public PackFileConfigElement? Music { get; set; }
    public PackFileConfigElement? Particles { get; set; }
}