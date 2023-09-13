using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Network.Exception;
using RB.Core.Service.Gateway;

namespace RB.Core.Network.Gateway.Handler;

public class IdentificationHandler : IGatewayMsgHandler
{
    private readonly PatchInfoService _patchInfoService;

    public IdentificationHandler(
        IGatewayClient gatewayClient,
        PatchInfoService patchInfoService
    )
    {
        _patchInfoService = patchInfoService;
        gatewayClient.SetMsgHandler(NetMsgId.SetupCordNoDir, OnSetupCord);
    }

    private bool OnSetupCord(Message msg)
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