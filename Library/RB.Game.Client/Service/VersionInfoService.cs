using RB.Game.Client.ResourceLoader;
using RB.Game.Client.ResourceLoader.VersionInfo;

namespace RB.Game.Client.Service;

public class VersionInfoService : IVersionInfoService
{
    private readonly IVersionInfoLoader _versionInfoLoader;
    private int? _version;

    public VersionInfoService(IVersionInfoLoader versionInfoLoader)
    {
        _versionInfoLoader = versionInfoLoader;
    }

    private bool Load()
    {
        if (!_versionInfoLoader.TryLoad(out var clientVersionResult))
            throw new NotLoadedException(clientVersionResult.Path);

        _version = clientVersionResult.Value;

        return true;
    }

    public int GetVersion()
    {
        if (!_version.HasValue && !Load())
            throw new NotLoadedException(IVersionInfoLoader.Path);

        return _version!.Value;
    }
}