using RB.Game.Client.Objects;
using RB.Game.Client.ResourceLoader;
using RB.Game.Client.ResourceLoader.DivisionInfo;
using RB.Game.Client.ResourceLoader.VersionInfo;

namespace RB.Game.Client.Service;

public class DivisionInfoService : IDivisionInfoService
{
    private readonly IDivisionInfoLoader _divisionInfoLoader;
    private DivisionInfo? _divisionInfo;

    public DivisionInfoService(IDivisionInfoLoader divisionInfoLoader)
    {
        _divisionInfoLoader = divisionInfoLoader;
    }

    private bool Load()
    {
        if (!_divisionInfoLoader.TryLoad(out var divisionInfoResult))
            throw new NotLoadedException(divisionInfoResult.Path);

        _divisionInfo = divisionInfoResult.Value!;

        return true;
    }

    public DivisionInfo GetDivisionInfo()
    {
        if (_divisionInfo == null && !Load())
            throw new NotLoadedException(IVersionInfoLoader.Path);

        return _divisionInfo!;
    }
}