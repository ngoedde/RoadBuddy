namespace RB.Game;

public enum RangeType : byte
{
    Surrounding = 1, //Stikes enemies in a 360° circle.
    Front = 2, //Stikes enemies in a 180° arc.
    Piercing = 3, //Stikes enemies behind target.
    Projectile = 4, //Strikes enemies between caster and target.

    //Unknown5 = 5,
    Transfer = 6 //Stikes nearest target and repeat. (Back and forth?)
}