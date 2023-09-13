namespace RB.Game.Client.Objects;

public class Division
{
    public Division(string name, string[] gatewayServers)
    {
        Name = name;
        GatewayServers = gatewayServers;
    }

    public string Name { get; }

    public string[] GatewayServers { get; }
}