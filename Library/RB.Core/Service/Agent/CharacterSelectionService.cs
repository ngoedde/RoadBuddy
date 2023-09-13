using RB.Core.Net.Common.Messaging;
using RB.Core.Network.Agent;
using RB.Game.Objects.CharacterSelection;
using Serilog;

namespace RB.Core.Service.Agent;

public sealed class CharacterSelectionService
{
    public delegate void CharacterListUpdatedEventHandler(CharacterList characterList);
    public event CharacterListUpdatedEventHandler? CharacterListUpdated;

    public delegate void JoinRequestedEventHandler(string characterName);
    public event JoinRequestedEventHandler? JoinRequested;

    public delegate void JoinedEventHandler();
    public event JoinedEventHandler? Joined;
    
    private readonly IAgentClient _agentClient;
    private CharacterList _characterList = new();

    public CharacterSelectionService(IAgentClient agentClient)
    {
        _agentClient = agentClient;
        
        _agentClient.SetMsgHandler(AgentMsgId.CharacterListingActionAck, OnCharacterSelectionActionResponse);
        _agentClient.SetMsgHandler(AgentMsgId.CharacterListingJoinAck, OnJoinResponse);
    }

    public bool RequestJoin(string characterName)
    {
        var joinReq = _agentClient.NewMsg(AgentMsgId.CharacterListingJoinReq, _agentClient.ServerId);
        
        if (!joinReq.TryWrite(characterName)) return false;
        
        OnJoinRequested(characterName);
        
        return _agentClient.PostMsg(joinReq);
    }

    public bool RequestCharacterList()
    {
        var charListReq = _agentClient.NewMsg(AgentMsgId.CharacterListingActionReq, _agentClient.ServerId);

        if (!charListReq.TryWrite(CharacterSelectionAction.List)) return false;

        return _agentClient.PostMsg(charListReq);
    }
    
    private bool OnJoinResponse(Message msg)
    {
        if (!msg.TryRead(out MessageResult result)) return false;
        
        if (result == MessageResult.Success)
            OnJoined();

        return true;
    }
    
    private bool OnCharacterSelectionActionResponse(Message msg)
    {
        if (!msg.TryRead(out CharacterSelectionAction action)) return false;
        if (!msg.TryRead(out MsgResult result)) return false;

        if (result == MsgResult.Success && action == CharacterSelectionAction.List)
        {
            if (!msg.TryDeserialize(out _characterList)) return false;
            
            OnCharacterListUpdated(_characterList);
        }
        
        return true;
    }

    private void OnCharacterListUpdated(CharacterList characterList)
    {
        CharacterListUpdated?.Invoke(characterList);
    }

    private void OnJoinRequested(string characterName)
    {
        JoinRequested?.Invoke(characterName);
    }

    private void OnJoined()
    {
        Log.Information("Successfully entered game!");
        
        Joined?.Invoke();
    }
}