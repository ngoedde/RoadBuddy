using Microsoft.Extensions.Options;
using RB.Bot.Config;
using RB.Core.Service;
using RB.Core.Service.Agent;
using RB.Core.Service.Gateway;
using RB.Game.Client.Service;
using RB.Game.Objects;
using RB.Game.Objects.CharacterSelection;
using RB.Game.Objects.ShardInfo;
using Serilog;
using LoginService = RB.Core.Service.Gateway.LoginService;

namespace RB.Bot.Module.AutoLogin;

/// <summary>
///     This module represents the logic behind the auto login.
/// </summary>
public class AutoLoginModule
{
    private readonly BotConfig _config;
    private readonly ContextSwitcher _contextSwitcher;
    private readonly CharacterSelectionService _characterSelectionService;
    private readonly IDivisionInfoService _divisionInfo;
    private readonly LoginService _loginService;
    private readonly ShardInfoService _shardInfoService;

    public AutoLoginModule(
        LoginService loginService,
        ShardInfoService shardInfoService,
        PatchInfoService patchInfoService,
        IOptions<BotConfig> options,
        IDivisionInfoService divisionInfo,
        ContextSwitcher contextSwitcher,
        CharacterSelectionService characterSelectionService,
        RB.Core.Service.Agent.LoginService agentLoginService
    )
    {
        _loginService = loginService;
        _shardInfoService = shardInfoService;
        _divisionInfo = divisionInfo;
        _contextSwitcher = contextSwitcher;
        _characterSelectionService = characterSelectionService;
        _config = options.Value;

        patchInfoService.UpdatePatchInfo += PatchInfoServiceOnUpdatePatchInfo;
        shardInfoService.UpdateShardInfo += OnUpdateShardInfo;
        agentLoginService.LoginSuccess += OnAgentLoginSuccess;
        characterSelectionService.CharacterListUpdated += OnUpdateCharacterList;
    }
    
    private void OnUpdateCharacterList(CharacterList characterList)
    {
        foreach (var character in characterList)
            Log.Debug($"Found character `{character.Name}` (lv. {character.Level})");

        if (!_config.AutoLogin.Enabled)
            return;
        
        if (characterList.Count == 0)
        {
            Log.Error("Auto login failed: No characters found on this account/server.");

            return;
        }

        var charToSelect = string.IsNullOrEmpty(_config.AutoLogin.Character)
            ? characterList.FirstOrDefault()
            : characterList.FirstOrDefault(c => c.Name == _config.AutoLogin.Character);

        if (charToSelect == null)
        {
            Log.Error($"Auto login failed: The character `{_config.AutoLogin.Character}` does not exist!");
            
            return;
        }

        Log.Information($"Auto login: Entering game as `{charToSelect.Name}` (lv. {charToSelect.Level})");
        
        _characterSelectionService.RequestJoin(charToSelect.Name);
    }

    private void OnAgentLoginSuccess()
    {
        if (!_config.AutoLogin.Enabled)
            return;
        
        _characterSelectionService.RequestCharacterList();
    }

    private void OnUpdateShardInfo(ShardInfo info)
    {
        if (!_config.AutoLogin.Enabled)
            return;

        DoAutologin(info);
    }

    private void PatchInfoServiceOnUpdatePatchInfo(PatchInfo patchInfo)
    {
        if (!_config.AutoLogin.Enabled)
            return;

        _shardInfoService.RequestShardInfo();
    }

    private void DoAutologin(ShardInfo shardInfo)
    {
        if (!shardInfo.TryGetShardId(_config.AutoLogin.Shard, out var shardId))
        {
            Log.Error($"Auto login failed: The shard `{_config.AutoLogin.Shard}` doesn't exist!");

            return;
        }

        _contextSwitcher.SetAgentContext(_config.AutoLogin.Username, _config.AutoLogin.Password,
        _divisionInfo.GetDivisionInfo().ContentId);
        _loginService.RequestLogin(_config.AutoLogin.Username, _config.AutoLogin.Password, shardId);
    }
}