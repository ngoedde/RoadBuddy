using Microsoft.Extensions.Options;
using RB.Core.Config;
using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Network.Exception;
using RB.Core.Network.Gateway.Service;
using RB.Game.Client.Service;
using Serilog;

namespace RB.Core.Network.Gateway.Handler;

public class IdentificationHandler : IGatewayMsgHandler
{
    private readonly IGatewayClient _gatewayClient;
    private readonly PatchInfoService _patchInfoService;

    public IdentificationHandler(
        IGatewayClient gatewayClient, 
        PatchInfoService patchInfoService
    ) {
        _gatewayClient = gatewayClient;
        _patchInfoService = patchInfoService;
        _gatewayClient.SetMsgHandler(NetMsgId.SetupCordNoDir, OnSetupCordSendPatchReq);
    }

    private bool OnSetupCordSendPatchReq(Message msg)
    {
        if (msg.SenderID == msg.ReceiverID)
            return true;
        
        if (!msg.TryRead(out string identityName)) 
            return false;

        if (identityName != NetIdentity.GatewayServer)
            throw new InvalidIdentityException(NetIdentity.GatewayServer, identityName);
        
        _patchInfoService.RequestPatchInfo();
        
        return true;
    }
}