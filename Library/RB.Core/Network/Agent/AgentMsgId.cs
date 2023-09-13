using RB.Core.Net.Common.Messaging;

namespace RB.Core.Network.Agent;

public static class AgentMsgId
{
    public static readonly MessageID
        LoginReq = MessageID.Create(MessageDirection.Req, MessageType.Framework, 0x103); // 0x6103

    public static readonly MessageID
        LoginAck = MessageID.Create(MessageDirection.Ack, MessageType.Framework, 0x103); // 0xA103
    
    public static readonly MessageID
        CharacterListingJoinReq = MessageID.Create(MessageDirection.Req, MessageType.Game, 0x001); // 0x7001

    public static readonly MessageID
        CharacterListingJoinAck = MessageID.Create(MessageDirection.Ack, MessageType.Game, 0x001); // 0xB001
    
    public static readonly MessageID
        CharacterListingActionReq = MessageID.Create(MessageDirection.Req, MessageType.Game, 0x007); // 0x7007

    public static readonly MessageID
        CharacterListingActionAck = MessageID.Create(MessageDirection.Ack, MessageType.Game, 0x007); // 0xB007

}