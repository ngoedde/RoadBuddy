namespace RB.Game.Client.Service;

public interface IGatewayPortInfoService
{ 
    ushort GetPort() => Load();
    
    ushort Load();
}