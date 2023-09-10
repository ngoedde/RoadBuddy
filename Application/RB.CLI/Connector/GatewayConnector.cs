using System.Net;
using Microsoft.Extensions.Options;
using RB.Core.Config;
using RB.Core.Net.Common;
using RB.Core.Network.Gateway;
using RB.Game.Client.Service;
using Serilog;

namespace RB.CLI.Connector;

public class GatewayConnector
{
    private readonly AppConfig _options;
    private readonly IGatewayClient _gatewayClient;
    private readonly IDivisionInfoService _divisionInfoService;
    private readonly IGatewayPortInfoService _gatewayPortInfoService;
    
    public GatewayConnector(
        IOptions<AppConfig> options,
        IGatewayClient gatewayClient,
        IDivisionInfoService divisionInfoService,
        IGatewayPortInfoService gatewayPortInfoService,
        GatewayHandlerGroup gatewayHandlerGroup
    ) 
    {
        _options = options.Value;
        _gatewayClient = gatewayClient;
        _divisionInfoService = divisionInfoService;
        _gatewayPortInfoService = gatewayPortInfoService;
    }

    public void Connect()
    {
        string? gatewayHost = null;
        var gatewayPort = (ushort) 0;
        
        if (_options.Overrides.Gateway != null)
        {
            gatewayHost = _options.Overrides.Gateway.Host;
            gatewayPort = _options.Overrides.Gateway.Port;
        }
        else
        {
            var divisionInfo = _divisionInfoService.GetDivisionInfo();
            
            if (divisionInfo.Divisions.Length > 0 && divisionInfo.Divisions[0].GatewayServers.Length > 0)
            {
                gatewayHost = divisionInfo.Divisions[0].GatewayServers[0];
                gatewayPort = _gatewayPortInfoService.GetPort();
            }
        }
    
        if (string.IsNullOrEmpty(gatewayHost) || gatewayPort == 0) {
            Log.Error("Can not detect valid gateway server IP/port.");
            
            throw new Exception("Can not detect valid gateway server IP/port.");
        }
        
        Log.Information("Connecting to gateway {host}:{port} ...", gatewayHost, gatewayPort);
        
        var endpoint = NetHelper.ToIPEndPoint(gatewayHost, gatewayPort);
        _gatewayClient.Connect(endpoint);
    }

    public void Update()
    {
        _gatewayClient.Update();
    }
}