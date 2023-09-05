namespace RB.Game.Client.ResourceLoader.DivisionInfo;

public class DivisionInfoLoaderResult : LoaderResult<Objects.DivisionInfo>
{
    public DivisionInfoLoaderResult(bool success, string path, Objects.DivisionInfo? value, string? message = null) : base(success, path, value, message)
    {
    }
}