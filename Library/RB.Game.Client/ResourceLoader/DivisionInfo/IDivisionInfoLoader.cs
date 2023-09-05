namespace RB.Game.Client.ResourceLoader.DivisionInfo;

public interface IDivisionInfoLoader
{
    const string Path = "divisioninfo.txt";
    
    bool TryLoad(out DivisionInfoLoaderResult result);
}