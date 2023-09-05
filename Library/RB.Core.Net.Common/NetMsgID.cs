using RB.Core.Net.Common.Messaging;

namespace RB.Core.Net.Common;

public static class NetMsgID
{
    public static readonly MessageID NET_FILE_IO = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 1); // 0x1001

    public static readonly MessageID LOCAL_NET_CONNECT = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 2);
    public static readonly MessageID LOCAL_NET_CONNECTED = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 3);

    public static readonly MessageID LOCAL_NET_DISCONNECT = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 4);
    public static readonly MessageID LOCAL_NET_DISCONNECTED = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 5);

    public static readonly MessageID LOCAL_NET_KEYEXCHANGED = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 6);

    public static readonly MessageID LOCAL_NET_FILE_PROGRESS = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 7);
    public static readonly MessageID LOCAL_NET_FILE_SUCCESS = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 8);
    public static readonly MessageID LOCAL_NET_FILE_FAILED = MessageID.Create(MessageDirection.NoDir, MessageType.NetEngine, 9);

    public static readonly MessageID NET_KEYEXCHANGE_REQ = MessageID.Create(MessageDirection.Req, MessageType.NetEngine, 0);
    public static readonly MessageID NET_KEYEXCHANGE_ACK = MessageID.Create(MessageDirection.Ack, MessageType.NetEngine, 0);
    
    public static readonly MessageID SetupCordNoDir = MessageID.Create(MessageDirection.NoDir, MessageType.Framework, 0x001);
    public static readonly MessageID KeepAliveNoDir = MessageID.Create(MessageDirection.NoDir, MessageType.Framework, 0x002);
    public static readonly MessageID FrameworkMassiveReq = MessageID.Create(MessageDirection.Req, MessageType.Framework, 0x00D);

}