using RB.Game.Objects.RefObject;

namespace RB.Game.Client.ResourceLoader.SkillData;

public class SkillDataLoadResult : LoadResult<Dictionary<uint, RefSkill>>
{
    public SkillDataLoadResult(bool success, string path, Dictionary<uint, RefSkill>? value, string? message = null) :
        base(success, path, value, message)
    {
    }
}