using RB.Game.Client.Objects;

namespace RB.Game.Client.Service;

public interface IDivisionInfoService
{
    public DivisionInfo GetDivisionInfo() => Load();

    DivisionInfo Load();
}