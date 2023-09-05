namespace RB.Game.Client.ResourceLoader.VersionInfo;

public class VersionInfoLoaderResult : LoaderResult<int>
{
    public VersionInfoLoaderResult(bool success, string path, int value, string? message = null) : base(success, path, value, message)
    {
    }
}