namespace RB.Game.Client.ResourceLoader.GatePort;

public class GatewayPortLoadResult : LoadResult<ushort>
{
    public GatewayPortLoadResult(bool success, string path, ushort value, string? message = null) : base(success, path,
        value, message)
    {
    }
}