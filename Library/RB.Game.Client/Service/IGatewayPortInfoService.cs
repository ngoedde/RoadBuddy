namespace RB.Game.Client.Service;

public interface IGatewayPortInfoService
{
    ushort GetPort()
    {
        return Load();
    }

    ushort Load();
}