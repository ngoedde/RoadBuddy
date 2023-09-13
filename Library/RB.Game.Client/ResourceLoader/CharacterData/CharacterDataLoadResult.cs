using RB.Game.Objects.RefObject;

namespace RB.Game.Client.ResourceLoader.CharacterData;

public class CharacterDataLoadResult : LoadResult<Dictionary<uint, RefObjChar>>
{
    public CharacterDataLoadResult(bool success, string path, Dictionary<uint, RefObjChar>? value,
        string? message = null) : base(success, path, value, message)
    {
    }
}