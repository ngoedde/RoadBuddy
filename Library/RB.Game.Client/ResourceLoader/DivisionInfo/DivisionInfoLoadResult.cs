namespace RB.Game.Client.ResourceLoader.DivisionInfo;

public class DivisionInfoLoadResult : LoadResult<Objects.DivisionInfo>
{
    public DivisionInfoLoadResult(bool success, string path, Objects.DivisionInfo? value, string? message = null) :
        base(success, path, value, message)
    {
    }
}