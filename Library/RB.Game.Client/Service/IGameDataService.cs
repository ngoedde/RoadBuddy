using RB.Game.Objects.RefObject;

namespace RB.Game.Client.Service;

public interface IGameDataService
{
    void LoadGameData();
    
    bool TryGetItem(uint id, out RefObjItem? item);
    bool TryGetCharacter(uint id, out RefObjChar? character);
    bool TryGetSkill(uint id, out RefSkill? skill);
    
    bool TryGetItem(string codeName, out RefObjItem? item);
    bool TryGetCharacter(string codeName, out RefObjChar? character);
}