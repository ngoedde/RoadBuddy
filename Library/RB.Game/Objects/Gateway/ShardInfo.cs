namespace RB.Game.Objects.Gateway;

public class ShardInfo
{
    public List<Farm> Farms { get; }
    public List<Shard> Shards { get; }

    public ShardInfo()
    {
        Farms = new List<Farm>(4);
        Shards = new List<Shard>(8);
    }
}