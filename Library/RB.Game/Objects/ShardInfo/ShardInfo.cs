namespace RB.Game.Objects.ShardInfo;

public class ShardInfo
{
    public ShardInfo()
    {
        Farms = new List<Farm>(4);
        Shards = new List<Shard>(8);
    }

    public List<Farm> Farms { get; }
    public List<Shard> Shards { get; }

    public bool TryGetShardId(string shardName, out ushort shardId)
    {
        try
        {
            shardId = Shards.First(s => s.Name == shardName).Id;

            return true;
        }
        catch
        {
            shardId = 0;

            return false;
        }
    }
}