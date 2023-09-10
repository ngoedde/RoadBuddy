using System.Collections.ObjectModel;

namespace RB.Game.Client.Objects;

public class DivisionInfo
{
    public byte ContentId { get; }
    
    public Division[] Divisions { get; }

    public DivisionInfo(byte contentId, Division[] divisions)
    {
        ContentId = contentId;
        Divisions = divisions;
    }
}