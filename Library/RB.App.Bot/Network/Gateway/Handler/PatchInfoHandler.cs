using RB.Core.Net.Common.Messaging;
using RB.Core.Network;
using RB.Core.Network.Gateway;

namespace RB.App.Bot.Network.Gateway.Handler;

public class PatchInfoHandler : IGatewayMsgHandler
{
    private readonly IGatewayClient _gatewayClient;

    public PatchInfoHandler(IGatewayClient gatewayClient)
    {
        _gatewayClient = gatewayClient;
        _gatewayClient.SetMsgHandler(GatewayMsgId.PatchInfoAck, OnPatchInfo);
    }

    private bool OnPatchInfo(Message msg)
    {
        if (!msg.TryRead(out byte result))
            return false;

        return true;
    }
}