using RB.Game.Objects.RefObject;

namespace RB.Game.Client.ResourceLoader.ItemData;

public class ItemDataLoadResult : LoadResult<Dictionary<uint, RefObjItem>>
{
    public ItemDataLoadResult(bool success, string path, Dictionary<uint, RefObjItem>? value, string? message = null) :
        base(success, path, value, message)
    {
    }
}