namespace RB.Game.Client.ResourceLoader.SkillData;

public interface ISkillDataLoader
{
    const string Path = "server_dep\\silkroad\\textdata\\skilldataenc.txt";

    bool TryLoad(string path, out SkillDataLoadResult result);

    bool TryLoad(out SkillDataLoadResult result)
    {
        return TryLoad(Path, out result);
    }
}