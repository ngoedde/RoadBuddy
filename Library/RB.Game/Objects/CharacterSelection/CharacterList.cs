using System.Collections.ObjectModel;
using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Messaging.Serialization;

namespace RB.Game.Objects.CharacterSelection;

public class CharacterList : Collection<Character>, IMessageDeserializer
{
    public bool TryDeserialize(IMessageReader reader)
    {
        if (!reader.TryRead(out byte characterCount)) return false;

        for (var iChar = 0;  iChar < characterCount ; iChar++)
        {
            if (!reader.TryDeserialize(out Character character)) return false;
        
            Add(character);
        }
        
        return true;
    }
}