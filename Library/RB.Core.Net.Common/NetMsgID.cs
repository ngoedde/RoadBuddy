using RB.Core.Net.Common.Messaging;

namespace RB.Core.Net.Common;

public static class NetMsgId
{
    public static readonly MessageID NetFileIo = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 1); // 0x1001

    public static readonly MessageID LocalNetConnect = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 2);
    public static readonly MessageID LocalNetConnected = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 3);

    public static readonly MessageID LocalNetDisconnect = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 4);
    public static readonly MessageID LocalNetDisconnected = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 5);

    public static readonly MessageID LocalNetKeyExchanged = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 6);

    public static readonly MessageID LocalNetFileProgress = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 7);
    public static readonly MessageID LocalNetFileSuccess = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 8);
    public static readonly MessageID LocalNetFileFailed = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 9);

    public static readonly MessageID NetKeyExchangeReq = MessageID.Create(MessageDirection.Req, MessageType.NetEngine, 0);
    public static readonly MessageID NetKeyExchangeAck = MessageID.Create(MessageDirection.Ack, MessageType.NetEngine, 0);
    
    public static readonly MessageID SetupCordNoDir = MessageID.Create(MessageDirection.NoDir, MessageType.Framework, 0x001);
    public static readonly MessageID KeepAliveNoDir = MessageID.Create(MessageDirection.NoDir, MessageType.Framework, 0x002);
    public static readonly MessageID FrameworkMassiveReq = MessageID.Create(MessageDirection.Req, MessageType.Framework, 0x00D);
}