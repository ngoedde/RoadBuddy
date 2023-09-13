using RB.Game.Client.ResourceLoader.CharacterData;
using RB.Game.Client.ResourceLoader.ItemData;
using RB.Game.Client.ResourceLoader.SkillData;
using RB.Game.Objects.RefObject;
using Serilog;

namespace RB.Game.Client.Service;

public class GameDataService : IGameDataService
{
    private readonly ICharacterDataLoader _characterDataLoader;
    private readonly IItemDataLoader _itemDataLoader;
    private readonly ISkillDataLoader _skillDataLoader;
    
    private Dictionary<uint, RefObjChar> _characterData = new();
    private Dictionary<uint, RefObjItem> _itemData = new();
    private Dictionary<uint, RefSkill> _skillData = new();

    public GameDataService(
        IItemDataLoader itemDataLoader,
        ICharacterDataLoader characterDataLoader,
        ISkillDataLoader skillDataLoader)
    {
        _itemDataLoader = itemDataLoader;
        _characterDataLoader = characterDataLoader;
        _skillDataLoader = skillDataLoader;
    }

    public void LoadGameData()
    {
        if (!_itemDataLoader.TryLoad(out var itemDataLoadResult))
            Log.Error($"Failed to load item data: {itemDataLoadResult.Message}");
        else
            _itemData = itemDataLoadResult.Value!;

        if (!_characterDataLoader.TryLoad(out var characterDataLoadResult))
            Log.Error($"Failed to load character data: {characterDataLoadResult.Message}");
        else
            _characterData = characterDataLoadResult.Value!;

        if (!_skillDataLoader.TryLoad(out var skillDataLoadResult))
            Log.Error($"Failed to load item data: {itemDataLoadResult.Message}");
        else
            _skillData = skillDataLoadResult.Value!;
    }

    public bool TryGetItem(uint id, out RefObjItem? item) => _itemData.TryGetValue(id, out item);
    public bool TryGetCharacter(uint id, out RefObjChar? character) => _characterData.TryGetValue(id, out character);
    public bool TryGetSkill(uint id, out RefSkill? skill) => _skillData.TryGetValue(id, out skill);
    public bool TryGetItem(string codeName, out RefObjItem? item)
    {
        item = _itemData.FirstOrDefault(i => i.Value.CodeName == codeName).Value;

        return item != null;
    }

    public bool TryGetCharacter(string codeName, out RefObjChar? character)
    {
        character = _characterData.FirstOrDefault(c => c.Value.CodeName == codeName).Value;

        return character != null;
    }
}