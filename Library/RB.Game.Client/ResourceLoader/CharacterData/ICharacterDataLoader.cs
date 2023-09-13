namespace RB.Game.Client.ResourceLoader.CharacterData;

public interface ICharacterDataLoader
{
    const string Path = "server_dep\\silkroad\\textdata\\characterdata.txt";

    bool TryLoad(string path, out CharacterDataLoadResult result);

    bool TryLoad(out CharacterDataLoadResult result)
    {
        return TryLoad(Path, out result);
    }
}