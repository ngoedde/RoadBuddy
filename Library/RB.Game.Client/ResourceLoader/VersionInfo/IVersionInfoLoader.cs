namespace RB.Game.Client.ResourceLoader.VersionInfo;

public interface IVersionInfoLoader
{
    const string Path = "SV.T";
    
    bool TryLoad(out VersionInfoLoaderResult result);
}