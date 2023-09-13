namespace RB.Game.Client.Objects;

public class DivisionInfo
{
    public DivisionInfo(byte contentId, Division[] divisions)
    {
        ContentId = contentId;
        Divisions = divisions;
    }

    public byte ContentId { get; }

    public Division[] Divisions { get; }
}