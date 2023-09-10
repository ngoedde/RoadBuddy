namespace RB.Game.Client.ResourceLoader.VersionInfo;

public interface IVersionInfoLoader
{
    const string Path = "SV.T";
    
    /// <summary>
    /// Loads the version.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    bool TryLoad(out VersionInfoLoaderResult result);
}