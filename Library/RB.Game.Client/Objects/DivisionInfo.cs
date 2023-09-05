using System.Collections.ObjectModel;

namespace RB.Game.Client.Objects;

public class DivisionInfo
{
    public byte Locale { get; }
    
    public Division[] Divisions { get; }

    public DivisionInfo(byte locale, Division[] divisions)
    {
        Locale = locale;
        Divisions = divisions;
    }
}