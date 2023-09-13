namespace RB.Game;

[Flags]
public enum HideFlag : byte
{
    None = 0,
    Stealth = 1 << 0,
    Invisible = 1 << 1,
    Trap = 1 << 2
}