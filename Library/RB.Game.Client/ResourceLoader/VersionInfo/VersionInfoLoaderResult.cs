namespace RB.Game.Client.ResourceLoader.VersionInfo;

public class VersionInfoLoaderResult : LoaderResult<uint>
{
    public VersionInfoLoaderResult(bool success, string path, uint value, string? message = null) : base(success, path, value, message)
    {
    }
}