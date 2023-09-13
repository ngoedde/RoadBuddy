namespace RB.Game;

[Flags]
public enum AutoInvestFlag : byte
{
    None = 0,

    /// <summary>
    ///     Shows "noob"-icon next to characters name
    /// </summary>
    Beginner = 1,

    /// <summary>
    ///     Shows Helpful-icon above characters name
    /// </summary>
    Helpful = 2
}