using RB.Core.Net.Common.Messaging;
using RB.Game.Objects.Gateway;
using Serilog;

namespace RB.Core.Network.Gateway.Service;

public sealed class ShardInfoService
{
    public ShardInfo ShardInfo { get; set; } = new();
    
    public delegate void OnUpdateShardInfoEventHandler(ShardInfo info);
    public event OnUpdateShardInfoEventHandler? UpdateShardInfo;
    
    private readonly IGatewayClient _gatewayClient;

    public ShardInfoService(IGatewayClient gatewayClient)
    {
        _gatewayClient = gatewayClient;
        _gatewayClient.SetMsgHandler(GatewayMsgId.ShardInfoAck, OnShardInfoAck);
    }

    private bool OnShardInfoAck(Message msg)
    {
        ShardInfo = new ShardInfo();
        
        while (msg.TryRead(out bool hasFarmEntry) && hasFarmEntry)
        {
            if (!msg.TryRead(out byte farmId)) return false;
            if (!msg.TryRead(out string farmName)) return false;
            
            ShardInfo.Farms.Add(new Farm
            {
                Id = farmId,
                Name = farmName
            });
        }

        while (msg.TryRead(out bool hasShardEntry) && hasShardEntry)
        {
            if (!msg.TryRead(out ushort shardId)) return false;
            if (!msg.TryRead(out string shardName)) return false;
            if (!msg.TryRead(out ushort shardOnlineCount)) return false;
            if (!msg.TryRead(out ushort shardCapacity)) return false;
            if (!msg.TryRead(out bool shardIsOperating)) return false;
            if (!msg.TryRead(out byte shardFarmId)) return false;
            
            ShardInfo.Shards.Add(new Shard
            {
                Id = shardId,
                Name = shardName,
                Capacity = shardCapacity,
                OnlineCount = shardOnlineCount,
                Operating = shardIsOperating,
                FarmId = shardFarmId
            });
        }
        
        OnUpdateShardInfo(ShardInfo);
        
        return true;
    }

    public void RequestShardInfo()
    {
        using var shardListReq = _gatewayClient.NewMsg(GatewayMsgId.ShardInfoReq, _gatewayClient.ServerId);
    
        _gatewayClient.PostMsg(shardListReq);
    }

    private void OnUpdateShardInfo(ShardInfo info)
    {
        foreach (var shard in info.Shards)
            Log.Information($"Found shard server `{shard.Name}` (Name:  {shard.Name}, Online: {shard.OnlineCount}/{shard.Capacity}, Operating: {shard.Operating})");
        
        UpdateShardInfo?.Invoke(info);
    }
}