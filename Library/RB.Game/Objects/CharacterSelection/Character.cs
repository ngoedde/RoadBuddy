using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Messaging.Serialization;
using static System.String;

namespace RB.Game.Objects.CharacterSelection;

public class Character : IMessageDeserializer
{
    public uint RefObjID;
    public string Name;
    public byte Scale;
    public byte Level;
    public ulong ExpOffset;
    public ushort Strength;
    public ushort Intelligence;
    public ushort StatPoints;
    public uint Health;
    public uint Mana;
    public bool IsDeleting;
    public uint DeleteTime;
    public CharacterSelectionMemberClass GuildMemberClass;
    public bool IsGuildRenameRequired;
    public string CurrentGuildName = Empty;
    public CharacterSelectionMemberClass AcademyMemberClass;

    public Dictionary<uint, byte> Inventory = new(16);
    public Dictionary<uint, byte> AvatarInventory = new(8);
    
    public bool TryDeserialize(IMessageReader reader)
    {
        if (!reader.TryRead(out RefObjID)) return false;
        if (!reader.TryRead(out Name)) return false;
        if (!reader.TryRead(out Scale)) return false;
        if (!reader.TryRead(out Level)) return false;
        if (!reader.TryRead(out ExpOffset)) return false;
        if (!reader.TryRead(out Strength)) return false;
        if (!reader.TryRead(out Intelligence)) return false;
        if (!reader.TryRead(out StatPoints)) return false;
        if (!reader.TryRead(out Health)) return false;
        if (!reader.TryRead(out Mana)) return false;
        if (!reader.TryRead(out IsDeleting)) return false;
        if (IsDeleting && !reader.TryRead(out DeleteTime)) return false;
        if (!reader.TryRead(out GuildMemberClass)) return false;
        if (!reader.TryRead(out IsGuildRenameRequired)) return false;
        if (IsGuildRenameRequired && !reader.TryRead(out CurrentGuildName)) return false;
        if (!reader.TryRead(out AcademyMemberClass)) return false;
        
        //Inventory
        if (!reader.TryRead(out byte itemCount)) return false;
        for (var iItem = 0; iItem < itemCount; iItem++)
        {
            if (!reader.TryRead(out uint refObjId)) return false;
            if (!reader.TryRead(out byte optLevel)) return false;
            if (!Inventory.TryAdd(refObjId, optLevel)) return false;
        }

        //Avatars
        if (!reader.TryRead(out byte avatarCount)) return false;
        for (var iAvatar = 0; iAvatar < avatarCount; iAvatar++)
        {
            if (!reader.TryRead(out uint refObjId)) return false;
            if (!reader.TryRead(out byte optLevel)) return false;
            if (!AvatarInventory.TryAdd(refObjId, optLevel)) return false;
        }

        return true;
    }
}