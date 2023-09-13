namespace RB.Core.Net.Common.Messaging.Allocation;

public class RelayMsgAllocator : IRelayMsgAllocator
{
    public RelayMsg NewRelayMsg(Message request, MessageID responseMsgId, string? memberName = null,
        string? filePath = null,
        int lineNumber = -1)
    {
        return new RelayMsg(request, responseMsgId);
    }

    public RelayMsg NewRelayMsg(Message request, MessageID responseMsgId, int receiverId = -1,
        string? memberName = null,
        string? filePath = null, int lineNumber = -1)
    {
        return new RelayMsg(request, responseMsgId, receiverId);
    }
}