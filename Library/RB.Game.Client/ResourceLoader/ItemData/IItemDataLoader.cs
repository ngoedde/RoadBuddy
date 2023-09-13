namespace RB.Game.Client.ResourceLoader.ItemData;

public interface IItemDataLoader
{
    const string Path = "server_dep\\silkroad\\textdata\\itemdata.txt";

    bool TryLoad(string path, out ItemDataLoadResult result);

    bool TryLoad(out ItemDataLoadResult result)
    {
        return TryLoad(Path, out result);
    }
}