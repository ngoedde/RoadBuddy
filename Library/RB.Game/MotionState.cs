namespace RB.Game;

/// <summary>
///     OBJ_MOTIONSTATE_
/// </summary>
public enum MotionState : byte
{
    Stand = 0,
    Skill = 1,
    Walk = 2,
    Run = 3,
    Sit = 4,
    Jump = 5,
    Swim = 6,
    Ride = 7,
    Knockdown = 8,
    Stun = 9,
    Frozen = 10,
    Hit = 11,
    ReqHelp = 12,
    Pao = 13,
    Counterattack = 14,
    SkillActionOff = 15,
    SkillKnockBack = 16,
    SkillProtectionWall = 17,
    ChangeMotion = 18
}