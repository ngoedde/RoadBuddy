namespace RB.Game;

[Flags]
public enum AbnormalStateFlag : uint
{
    None = 0,

    Frozen = 1u << 0,

    /// <summary>
    ///     Frostbitten (Reduced Move & Attack speed)
    /// </summary>
    Frostbite = 1u << 1,

    /// <summary>
    ///     Shocked (Reduced Parry Rate)
    /// </summary>
    EShock = 1u << 2,

    /// <summary>
    ///     Burnt (DoT)
    /// </summary>
    Burn = 1u << 3,

    /// <summary>
    ///     Poisoned (DoT)
    /// </summary>
    Poison = 1u << 4,

    /// <summary>
    ///     Inverts HP recovery effect
    /// </summary>
    Zombie = 1u << 5,

    //All effects below carry extra byte for level
    Sleep = 1u << 6,

    /// <summary>
    ///     Bind
    /// </summary>
    Root = 1u << 7,

    /// <summary>
    ///     Dull (Reduced Move & Attack speed)
    /// </summary>
    Slow = 1u << 8,

    /// <summary>
    ///     Unable to select or attack the target for fixed time)
    /// </summary>
    Fear = 1u << 9,

    /// <summary>
    ///     ShortSighted (Reduced Range)
    /// </summary>
    Myopia = 1u << 10,

    /// <summary>
    ///     Bleed (DoT; Reduced Phy. & Mag. defense)
    /// </summary>
    Blood = 1u << 11,

    /// <summary>
    ///     Petrify (DoT)
    /// </summary>
    Stone = 1u << 12,

    /// <summary>
    ///     Darkness (Reduced Hit Rate)
    /// </summary>
    Dark = 1u << 13,

    /// <summary>
    ///     Stunned
    /// </summary>
    Stun = 1u << 14,

    /// <summary>
    ///     Disease (Increased AbnormalState probability rate)
    /// </summary>
    Disease = 1u << 15,

    /// <summary>
    ///     Confusion
    /// </summary>
    Chaos = 1u << 16,

    /// <summary>
    ///     Decay (Reduced Phy. defense)
    /// </summary>
    CursePD = 1u << 17,

    /// <summary>
    ///     Weak (Reduced Mag. defense)
    /// </summary>
    CurseMD = 1u << 18,

    /// <summary>
    ///     Impotent (Reduced Phy. & Mag. attack)
    /// </summary>
    CurseStr = 1u << 19,

    /// <summary>
    ///     Division (Reduced Phy. & Mag. defense)
    /// </summary>
    CurseINT = 1u << 20,

    /// <summary>
    ///     Panic (Increased HP Recovery delay [4sec])
    /// </summary>
    CurseHP = 1u << 21,

    /// <summary>
    ///     Combustion (Increased MP Recovery delay by [4sec])
    /// </summary>
    CurseMP = 1u << 22,

    Bit23 = 1u << 23,

    /// <summary>
    ///     Hidden
    /// </summary>
    Bomb = 1u << 24,

    Bit25 = 1u << 25,
    Bit26 = 1u << 26,
    Bit27 = 1u << 27,
    Bit28 = 1u << 28,
    Bit29 = 1u << 29,
    Bit30 = 1u << 30,
    Bit31 = 1u << 31
}