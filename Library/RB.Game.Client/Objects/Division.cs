namespace RB.Game.Client.Objects;

public class Division
{
    public string Name { get; }

    public string[] GatewayServers { get; }

    public Division(string name, string[] gatewayServers)
    {
        Name = name;
        GatewayServers = gatewayServers;
    }
}