using RB.Game.Parser;

namespace RB.Game.Objects.RefObject;

public class RefObjItem : RefObjCommon
{
    public string Desc1;
    public string Desc2;
    public string Desc3;
    public string Desc4;
    public byte ItemClass;
    public int MaxStack;
    public int Param1;
    public int Param2;
    public int Param3;
    public int Param4;
    public byte Quivered; //Consumes ammo
    public short Range;
    public byte ReqGender;
    public int ReqInt;
    public int ReqStr;
    public byte SpeedClass;
    public byte TwoHanded;

    //public float Dur_L;
    //public float Dur_U;

    //public float PD_L;
    //public float PD_U;
    //public float PDInc;

    //public float ER_L;
    //public float ER_U;
    //public float ERInc;

    //public float PAR_L;
    //public float PAR_U;
    //public float PARInc;

    //public float BR_L;
    //public float BR_U;

    //public float MD_L;
    //public float MD_U;
    //public float MDInc;

    //public float MAR_L;
    //public float MAR_U;
    //public float MARInc;

    //public float PDStr_L;
    //public float PDStr_U;

    //public float MDInt_L;
    //public float MDInt_U;

    //public float PAttackMin_L;
    //public float PAttackMin_U;
    //public float PAttackMax_L;
    //public float PAttackMax_U;
    //public float PAttackInc;
    //public float MAttackMin_L;
    //public float MAttackMin_U;
    //public float MAttackMax_L;
    //public float MAttackMax_U;
    //public float MAttackInc;
    //public float PAStrMin_L;
    //public float PAStrMin_U;
    //public float PAStrMax_L;
    //public float PAStrMax_U;
    //public float MAInt_Min_L;
    //public float MAInt_Min_U;
    //public float MAInt_Max_L;
    //public float MAInt_Max_U;
    //public float HR_L;
    //public float HR_U;
    //public float HRInc;
    //public float CHR_L;
    //public float CHR_U;

    /// <summary>
    ///     Gets the degree of the item
    /// </summary>
    public int Degree => (ItemClass - 1) / 3 + 1;

    /// <summary>
    ///     Gets the degree offset.
    /// </summary>
    public int DegreeOffset => ItemClass - 3 * ((ItemClass - 1) / 3) - 1; //sro_client.sub_8BA6E0

    public override bool Load(RefObjectParser parser)
    {
        if (!base.Load(parser))
            return false;

        //parser.TryParse(63, out Dur_L);
        //parser.TryParse(64, out Dur_U);
        //parser.TryParse(65, out PD_L);
        //parser.TryParse(66, out PD_U);
        //parser.TryParse(67, out PDInc);
        //parser.TryParse(68, out ER_L);
        //parser.TryParse(69, out ER_U);
        //parser.TryParse(70, out ERInc);
        //parser.TryParse(71, out PAR_L);
        //parser.TryParse(72, out PAR_U);
        //parser.TryParse(73, out PARInc);
        //parser.TryParse(74, out BR_L);
        //parser.TryParse(75, out BR_U);
        //parser.TryParse(76, out MD_L);
        //parser.TryParse(77, out MD_U);
        //parser.TryParse(78, out MDInc);
        //parser.TryParse(79, out MAR_L);
        //parser.TryParse(80, out MAR_U);
        //parser.TryParse(81, out MARInc);
        //parser.TryParse(82, out PDStr_L);
        //parser.TryParse(83, out PDStr_U);
        //parser.TryParse(84, out MDInt_L);
        //parser.TryParse(85, out MDInt_U);

        //parser.TryParse(95, out PAttackMin_L);
        //parser.TryParse(96, out PAttackMin_U);
        //parser.TryParse(97, out PAttackMax_L);
        //parser.TryParse(98, out PAttackMax_U);
        //parser.TryParse(99, out PAttackInc);
        //parser.TryParse(100, out MAttackMin_L);
        //parser.TryParse(101, out MAttackMin_U);
        //parser.TryParse(102, out MAttackMax_L);
        //parser.TryParse(103, out MAttackMax_U);
        //parser.TryParse(104, out MAttackInc);
        //parser.TryParse(105, out PAStrMin_L);
        //parser.TryParse(106, out PAStrMin_U);
        //parser.TryParse(107, out PAStrMax_L);
        //parser.TryParse(108, out PAStrMax_U);
        //parser.TryParse(109, out MAInt_Min_L);
        //parser.TryParse(110, out MAInt_Min_U);
        //parser.TryParse(111, out MAInt_Max_L);
        //parser.TryParse(112, out MAInt_Max_U);
        //parser.TryParse(113, out HR_L);
        //parser.TryParse(114, out HR_U);
        //parser.TryParse(115, out HRInc);
        //parser.TryParse(116, out CHR_L); //critical hit rate
        //parser.TryParse(117, out CHR_U);

        parser.TryParse(57, out MaxStack);
        parser.TryParse(58, out ReqGender);
        parser.TryParse(59, out ReqStr);
        parser.TryParse(60, out ReqInt);
        parser.TryParse(61, out ItemClass);
        parser.TryParse(86, out Quivered);
        parser.TryParse(92, out SpeedClass);
        parser.TryParse(93, out TwoHanded);
        parser.TryParse(94, out Range);
        parser.TryParse(118, out Param1);
        parser.TryParse(119, out Desc1);
        parser.TryParse(120, out Param2);
        parser.TryParse(121, out Desc2);
        parser.TryParse(122, out Param3);
        parser.TryParse(123, out Desc3);
        parser.TryParse(124, out Param4);
        parser.TryParse(125, out Desc4);

        return true;
    }

    public override string ToString()
    {
        return $"{CodeName} TID1:{TypeID1} TID2:{TypeID2} TID3:{TypeID3} TID4:{TypeID4}";
    }
}