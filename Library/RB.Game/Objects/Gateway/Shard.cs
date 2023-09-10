namespace RB.Game.Objects.Gateway;

public class Shard
{
    public ushort Id { get; init; }
    public string Name { get; init; }
    public ushort OnlineCount { get; init; }
    public ushort Capacity { get; init; }
    public bool Operating { get; init; }
    
    public byte FarmId { get; init; }
}