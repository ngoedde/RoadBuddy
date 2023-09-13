using RB.Core.Net.Common.Messaging;
using RB.Core.Network.Gateway;
using RB.Game.Objects.ShardInfo;
using Serilog;

namespace RB.Core.Service.Gateway;

public sealed class ShardInfoService
{
    public delegate void OnUpdateShardInfoEventHandler(ShardInfo info);

    private readonly IGatewayClient _gatewayClient;

    public ShardInfoService(IGatewayClient gatewayClient)
    {
        _gatewayClient = gatewayClient;
        _gatewayClient.SetMsgHandler(GatewayMsgId.ShardInfoAck, OnShardInfoAck);
    }

    public ShardInfo ShardInfo { get; set; } = new();
    
    public event OnUpdateShardInfoEventHandler? UpdateShardInfo;

    private bool OnShardInfoAck(Message msg)
    {
        var result = ReadFromMessage(msg, out var shardInfo);
        if (!result) return false;

        ShardInfo = shardInfo;
        OnUpdateShardInfo();

        return result;
    }

    private bool ReadFromMessage(Message msg, out ShardInfo shardInfo)
    {
        shardInfo = new ShardInfo();

        while (msg.TryRead(out bool hasFarmEntry) && hasFarmEntry)
        {
            if (!msg.TryRead(out byte farmId)) return false;
            if (!msg.TryRead(out string farmName)) return false;

            shardInfo.Farms.Add(new Farm
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

            shardInfo.Shards.Add(new Shard
            {
                Id = shardId,
                Name = shardName,
                Capacity = shardCapacity,
                OnlineCount = shardOnlineCount,
                Operating = shardIsOperating,
                FarmId = shardFarmId
            });
        }

        return true;
    }

    public void RequestShardInfo()
    {
        using var shardListReq = _gatewayClient.NewMsg(GatewayMsgId.ShardInfoReq, _gatewayClient.ServerId);

        _gatewayClient.PostMsg(shardListReq);
    }

    private void OnUpdateShardInfo()
    {
        foreach (var shard in ShardInfo.Shards)
            Log.Information(
                $"Found shard server `{shard.Name}` (Id: {shard.Id}, Name: {shard.Name}, Online: {shard.OnlineCount}/{shard.Capacity}, Operating: {shard.Operating})");

        UpdateShardInfo?.Invoke(ShardInfo);
    }
}