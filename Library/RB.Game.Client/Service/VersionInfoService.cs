using RB.Game.Client.ResourceLoader;
using RB.Game.Client.ResourceLoader.VersionInfo;

namespace RB.Game.Client.Service;

public class VersionInfoService : IVersionInfoService
{
    private readonly IVersionInfoLoader _versionInfoLoader;
    private uint? _version;

    public VersionInfoService(IVersionInfoLoader versionInfoLoader)
    {
        _versionInfoLoader = versionInfoLoader;
    }

    public uint Load()
    {
        if (!_versionInfoLoader.TryLoad(out var clientVersionResult))
            throw new NotLoadedException(clientVersionResult.Path);

        _version = clientVersionResult.Value;

        return _version.Value;
    }

    public uint GetVersion()
    {
        return _version ?? Load();
    }
}