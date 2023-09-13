namespace RB.Game;

[Flags]
public enum HitDamageFlag : byte
{
    None = 0,
    Normal = 1 << 0,
    Critical = 1 << 1,
    Berserk = 1 << 2,
    Bit3 = 1 << 3,
    Effect = 1 << 4,
    ArmorIgnored = 1 << 5,
    Bit6 = 1 << 6,
    Bit7 = 1 << 7
}