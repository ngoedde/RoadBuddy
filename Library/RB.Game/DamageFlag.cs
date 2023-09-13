namespace RB.Game;

[Flags]
public enum DamageFlag : byte
{
    None = 0,
    Melee = 1 << 0,
    Ranged = 1 << 1,
    Physical = 1 << 2,
    Magical = 1 << 3,

    All = Melee | Ranged | Physical | Magical
}