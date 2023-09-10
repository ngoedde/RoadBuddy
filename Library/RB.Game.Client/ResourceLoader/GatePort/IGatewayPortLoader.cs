namespace RB.Game.Client.ResourceLoader.GatePort;

public interface IGatewayPortLoader
{
    const string Path = "GATEPORT.TXT";
    
    /// <summary>
    /// Loads the version.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    bool TryLoad(out GatewayPortLoaderResult result);
}