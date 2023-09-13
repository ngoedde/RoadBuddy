namespace RB.Core.Net.Common.Messaging.Allocation;

public interface IRelayMsgAllocator
{
    RelayMsg NewRelayMsg(Message request, MessageID responseMsgId, string? memberName = null,
        string? filePath = null,
        int lineNumber = -1);

    RelayMsg NewRelayMsg(Message request, MessageID responseMsgId, int receiverId = -1,
        string? memberName = null, string? filePath = null,
        int lineNumber = -1);
}