using RB.Game.Client.Objects;
using RB.Game.Client.ResourceLoader;
using RB.Game.Client.ResourceLoader.DivisionInfo;

namespace RB.Game.Client.Service;

public class DivisionInfoService : IDivisionInfoService
{
    private readonly IDivisionInfoLoader _divisionInfoLoader;
    private DivisionInfo? _divisionInfo;

    public DivisionInfoService(IDivisionInfoLoader divisionInfoLoader)
    {
        _divisionInfoLoader = divisionInfoLoader;
    }

    public DivisionInfo Load()
    {
        if (!_divisionInfoLoader.TryLoad(out var divisionInfoResult))
            throw new NotLoadedException(divisionInfoResult.Path);

        _divisionInfo = divisionInfoResult.Value!;

        return _divisionInfo;
    }

    public DivisionInfo GetDivisionInfo()
    {
        return _divisionInfo ?? Load();
    }
}