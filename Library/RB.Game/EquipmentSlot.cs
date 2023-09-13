namespace RB.Game;

public enum EquipmentSlot : byte
{
    /// <summary>
    ///     %_HA_% or %_CA_%
    /// </summary>
    Helm = 0,

    /// <summary>
    ///     %_BA_%
    /// </summary>
    Mail = 1,

    /// <summary>
    ///     %_SA_%
    /// </summary>
    Shoulder = 2,

    /// <summary>
    ///     %_AA_%
    /// </summary>
    Gauntlet = 3,

    /// <summary>
    ///     %_LA_%
    /// </summary>
    Pants = 4,

    /// <summary>
    ///     %_FA_%
    /// </summary>
    Boots = 5,

    /// <summary>
    ///     Weapon
    /// </summary>
    Primary = 6,

    /// <summary>
    ///     Shield, Ammo
    /// </summary>
    Secondary = 7,

    /// <summary>
    ///     Earring
    /// </summary>
    Earring = 8,

    /// <summary>
    ///     Necklace
    /// </summary>
    Necklace = 9,

    /// <summary>
    ///     Left ring
    /// </summary>
    RingLeft = 10,

    /// <summary>
    ///     Right ring
    /// </summary>
    RingRight = 11,

    /// <summary>
    ///     PVPCape, JobSuit
    /// </summary>
    Extra = 12,

    Gold = 0xFE, //
    BuyBack = 0xFF // NPCs buy back slot
}