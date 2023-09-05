using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Network;
using RB.Core.Network.Gateway;
using RB.Game.Client.Service;

namespace RB.App.Bot.Network.Gateway.Handler;

public class KeyExchangeHandler : IGatewayMsgHandler
{
    private readonly IGatewayClient _gatewayClient;
    private readonly IVersionInfoService _clientVersionInfoService;

    public KeyExchangeHandler(IGatewayClient gatewayClient, IVersionInfoService clientVersionInfoService)
    {
        _gatewayClient = gatewayClient;
        _clientVersionInfoService = clientVersionInfoService;

        _gatewayClient.SetMsgHandler(NetMsgID.LOCAL_NET_KEYEXCHANGED, OnKeyExchanged);
    }

    private bool OnKeyExchanged(Message msg)
    {
        //TESTING!!!!
        var version = _clientVersionInfoService.GetVersion();
        
        
        using var setupCord = _gatewayClient.NewMsg(NetMsgID.SetupCordNoDir, msg.SenderID);
        
        if (!setupCord.TryWrite("SR_Client")) return false;
        if (!setupCord.TryWrite<byte>(0)) return false;
        
        var result = _gatewayClient.PostMsg(setupCord);

        return result;
    }
}