using RB.Game.Client.Objects;

namespace RB.Game.Client.Service;

public interface IDivisionInfoService
{
    public DivisionInfo GetDivisionInfo()
    {
        return Load();
    }

    DivisionInfo Load();
}