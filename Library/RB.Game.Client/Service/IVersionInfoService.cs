namespace RB.Game.Client.Service;

public interface IVersionInfoService
{ 
    uint GetVersion() => Load();
    
    uint Load();
}