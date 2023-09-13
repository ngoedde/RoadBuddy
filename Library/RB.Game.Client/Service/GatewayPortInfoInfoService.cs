using RB.Game.Client.ResourceLoader;
using RB.Game.Client.ResourceLoader.GatePort;

namespace RB.Game.Client.Service;

public class GatewayPortInfoInfoService : IGatewayPortInfoService
{
    private readonly IGatewayPortLoader _gatewayPortLoader;
    private ushort? _port;

    public GatewayPortInfoInfoService(IGatewayPortLoader gatewayPortLoader)
    {
        _gatewayPortLoader = gatewayPortLoader;
    }

    public ushort Load()
    {
        if (!_gatewayPortLoader.TryLoad(out var gatePortLoaderResult))
            throw new NotLoadedException(gatePortLoaderResult.Path);

        _port = gatePortLoaderResult.Value;

        return _port.Value;
    }

    public ushort GetPort()
    {
        return _port ?? Load();
    }
}