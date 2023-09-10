namespace RB.Game.Client.ResourceLoader.GatePort;

public class GatewayPortLoaderResult : LoaderResult<ushort>
{
    public GatewayPortLoaderResult(bool success, string path, ushort value, string? message = null) : base(success, path, value, message)
    {
    }
}