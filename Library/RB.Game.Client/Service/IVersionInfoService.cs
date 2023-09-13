namespace RB.Game.Client.Service;

public interface IVersionInfoService
{
    uint GetVersion()
    {
        return Load();
    }

    uint Load();
}