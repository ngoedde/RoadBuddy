using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Network.Exception;
using RB.Game.Client.Service;

namespace RB.Core.Network.Gateway.Handler;

public class IdentificationHandler : IGatewayMsgHandler
{
    private readonly IGatewayClient _gatewayClient;
    private readonly IVersionInfoService _versionInfoService;
    private readonly IDivisionInfoService _divisionInfoService;

    public IdentificationHandler(
        IGatewayClient gatewayClient,
        IVersionInfoService versionInfoService,
        IDivisionInfoService divisionInfoService
    ) {
        _gatewayClient = gatewayClient;
        _versionInfoService = versionInfoService;
        _divisionInfoService = divisionInfoService;
        _gatewayClient.SetMsgHandler(NetMsgID.SetupCordNoDir, OnSetupCord);
    }

    private bool OnSetupCord(Message msg)
    {
        if (msg.SenderID == msg.ReceiverID)
            return true;
        
        if (!msg.TryRead(out string identityName)) 
            return false;

        if (identityName != GatewayClient.ExpectedIdentity)
            throw new InvalidIdentityException();
        
        using var patchReq = _gatewayClient.NewMsg(GatewayMsgId.PatchInfoReq, msg.SenderID);
        
        patchReq.TryWrite(_divisionInfoService.GetDivisionInfo().Locale);
        patchReq.TryWrite("SR_Client");
        patchReq.TryWrite(_versionInfoService.GetVersion());
            
        // _gatewayClient.PostMsg(patchReq);
        
        return true;
    }
}