using RB.Core.Net.Common.Messaging;

namespace RB.Core.Network.Gateway;

public static class GatewayMsgId
{
    public static readonly MessageID
        PatchInfoReq = MessageID.Create(MessageDirection.Req, MessageType.Framework, 0x100); // 0x6100

    public static readonly MessageID
        PatchInfoAck = MessageID.Create(MessageDirection.Ack, MessageType.Framework, 0x100); // 0xA100

    public static readonly MessageID
        ShardInfoReq = MessageID.Create(MessageDirection.Req, MessageType.Framework, 0x101); // 0x6101

    public static readonly MessageID
        ShardInfoAck = MessageID.Create(MessageDirection.Ack, MessageType.Framework, 0x101); // 0xA101

    public static readonly MessageID
        LoginReq = MessageID.Create(MessageDirection.Req, MessageType.Framework, 0x102); // 0x6102

    public static readonly MessageID
        LoginAck = MessageID.Create(MessageDirection.Ack, MessageType.Framework, 0x102); // 0xA102

    public static readonly MessageID ArticleInfoReq =
        MessageID.Create(MessageDirection.Req, MessageType.Framework, 0x104); // 0x6104

    public static readonly MessageID ArticleInfoAck =
        MessageID.Create(MessageDirection.Ack, MessageType.Framework, 0x104); // 0xA104

    public static readonly MessageID
        FarmInfoReq = MessageID.Create(MessageDirection.Req, MessageType.Framework, 0x106); // 0x6106

    public static readonly MessageID
        FarmInfoAck = MessageID.Create(MessageDirection.Ack, MessageType.Framework, 0x106); // 0xA106
}