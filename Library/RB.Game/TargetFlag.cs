namespace RB.Game;

[Flags]
public enum TargetFlag : byte
{
    None = 0,
    Self = 1,
    Hide = 2,
    Party = 4,
    EnemyPlayer = 8,
    EnemyMonster = 16,
    Guild = 32,
    PET2 = 64
}