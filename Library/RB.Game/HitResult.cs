namespace RB.Game;

public enum HitResult : byte
{
    None = 0,

    //Miss = 1, // maybe?
    Blocked = 2,

    Knockdown = 4,
    KnockBack = 5,
    Unknown = 7,
    Repeat = 8,
    Death = 1 << 7 // only actual flag
}