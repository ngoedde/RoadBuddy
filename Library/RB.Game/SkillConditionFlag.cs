namespace RB.Game;

[Flags]
public enum SkillConditionFlag : byte
{
    None = 0,
    Knockdown = 1 << 0,
    MusicPlayArea = 1 << 5
}