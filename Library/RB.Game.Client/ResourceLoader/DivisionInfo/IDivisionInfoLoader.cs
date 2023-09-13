namespace RB.Game.Client.ResourceLoader.DivisionInfo;

public interface IDivisionInfoLoader
{
    const string Path = "divisioninfo.txt";

    /// <summary>
    ///     Loads the division info.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    bool TryLoad(out DivisionInfoLoadResult result);
}