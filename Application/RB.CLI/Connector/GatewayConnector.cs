using RB.Core.Network.Gateway;
using RB.Core.Service;
using RB.Game.Client.Service;
using Serilog;

namespace RB.CLI.Connector;

public class GatewayConnector
{
    private readonly ContextSwitcher _contextSwitcher;
    private readonly IDivisionInfoService _divisionInfoService;
    private readonly IGatewayClient _gatewayClient;
    private readonly IGatewayPortInfoService _gatewayPortInfoService;

    public GatewayConnector(
        IGatewayClient gatewayClient,
        IDivisionInfoService divisionInfoService,
        IGatewayPortInfoService gatewayPortInfoService,
        ContextSwitcher contextSwitcher
    )
    {
        _gatewayClient = gatewayClient;
        _divisionInfoService = divisionInfoService;
        _gatewayPortInfoService = gatewayPortInfoService;
        _contextSwitcher = contextSwitcher;
    }

    public void Connect()
    {
        string? gatewayHost = null;
        var gatewayPort = (ushort)0;

        var divisionInfo = _divisionInfoService.GetDivisionInfo();

        if (divisionInfo.Divisions.Length > 0 && divisionInfo.Divisions[0].GatewayServers.Length > 0)
        {
            gatewayHost = divisionInfo.Divisions[0].GatewayServers[0];
            gatewayPort = _gatewayPortInfoService.GetPort();
        }

        if (string.IsNullOrEmpty(gatewayHost) || gatewayPort == 0)
        {
            Log.Error("Can not detect valid gateway server IP/port.");

            throw new Exception("Can not detect valid gateway server IP/port.");
        }

        Log.Information("Connecting to gateway {host}:{port} ...", gatewayHost, gatewayPort);

        _contextSwitcher.SwitchContextToGateway(gatewayHost, gatewayPort);
    }

    public void Update()
    {
        _gatewayClient.Update();
    }
}