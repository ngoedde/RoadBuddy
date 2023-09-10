using Microsoft.Extensions.Options;
using RB.Core.Config;
using RB.Core.Net.Common.Messaging;
using RB.Game.Client.Service;
using RB.Game.Objects.Gateway;
using Serilog;

namespace RB.Core.Network.Gateway.Service;

public sealed class PatchInfoService
{
    public delegate void UpdatePatchInfoEventHandler(PatchInfo patchInfo);
    public event UpdatePatchInfoEventHandler? UpdatePatchInfo;

    public delegate void PatchInfoRequestedEventHandler();
    public event PatchInfoRequestedEventHandler PatchInfoRequested;
    
    public PatchInfo PatchInfo { get; set; }
    
    private readonly IGatewayClient _gatewayClient;
    private readonly AppConfig _config;
    private readonly IVersionInfoService _versionInfoService;
    private readonly IDivisionInfoService _divisionInfoService;
    
    public PatchInfoService(
        IGatewayClient gatewayClient,
        IOptions<AppConfig> config,
        IVersionInfoService versionInfoService,
        IDivisionInfoService divisionInfoService
    ) {
        PatchInfo = new PatchInfo { LatestVersion = 0, PatchRequired = false };
        
        _gatewayClient = gatewayClient;
        _versionInfoService = versionInfoService;
        _divisionInfoService = divisionInfoService;
        _config = config.Value;
        
        _gatewayClient.SetMsgHandler(GatewayMsgId.PatchInfoAck, OnPatchInfoAck);
    }
    
    private bool OnPatchInfoAck(MassiveMsg msg)
    {
        var clientVersion = _versionInfoService.GetVersion();
        var patchRequired = false;
        var currentVersion = clientVersion;
        
        if (!msg.TryRead(out MsgResult result)) return false;

        if (result == MsgResult.Success)
            Log.Information($"{_gatewayClient.Identity} version [{clientVersion}] is up to date!");

        if (result == MsgResult.Error)
        {
            patchRequired = true;
            
            if (!msg.TryRead(out byte errorCode)) return false;

            //Unknown error code. Ignore for now
            if (errorCode != 2) 
            {
                Log.Error($"Unknown {_gatewayClient.Identity} patch info error [{errorCode}]");
                
                return false;
            }
            
            if (!msg.TryRead(out string downloadServerIp)) return false;
            if (!msg.TryRead(out ushort downloadServerPort)) return false;
            if (!msg.TryRead(out currentVersion)) return false;

            Log.Error($"SR_Client version [{clientVersion}] is out of date. Latest version is [{currentVersion}]");
            Log.Debug($"DownloadServer IP: [{downloadServerIp}:{downloadServerPort}]");
        }
        
        PatchInfo = new PatchInfo
        {
            LatestVersion = currentVersion,
            PatchRequired = patchRequired
        };
        
        OnUpdatePatchInfo(PatchInfo);
        
        return true;
    }
    
    public void RequestPatchInfo()
    {
        using var patchReq = _gatewayClient.NewMsg(GatewayMsgId.PatchInfoReq, _gatewayClient.ServerId);

        var contentId = _config.Overrides.ContentId ?? _divisionInfoService.GetDivisionInfo().ContentId; 
        var version = _config.Overrides.Version ?? _versionInfoService.GetVersion();
        
        Log.Information($"Detected content ID [{contentId}] and version [{version}]");
        
        if (!patchReq.TryWrite(contentId)) return;
        if (!patchReq.TryWrite(NetIdentity.SilkroadClient)) return;
        if (!patchReq.TryWrite(version)) return;

        _gatewayClient.PostMsg(patchReq);
        
        OnPatchInfoRequested();
    }

    private void OnUpdatePatchInfo(PatchInfo patchInfo)
    {
        UpdatePatchInfo?.Invoke(patchInfo);
    }

    private void OnPatchInfoRequested()
    {
        PatchInfoRequested?.Invoke();
    }
}