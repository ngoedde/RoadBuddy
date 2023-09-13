namespace RB.Game.Client.ResourceLoader.VersionInfo;

public class VersionInfoLoadResult : LoadResult<uint>
{
    public VersionInfoLoadResult(bool success, string path, uint value, string? message = null) : base(success, path,
        value, message)
    {
    }
}