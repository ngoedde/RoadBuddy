using System.Net;
using System.Runtime.CompilerServices;
using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Messaging.Allocation;
using RB.Core.Net.Common.Messaging.Handling;

namespace RB.Core.Net;

public interface INetClient : IMessageAllocator, IMassiveMsgAllocator, IRelayMsgAllocator
{
    delegate void OnConnectedEventHandler();

    delegate void OnDisconnectedEventHandler();

    string Identity { get; }

    int ServerId { get; }

    int Id { get; }

    void SetMsgHandler(MessageID id, MsgHandler<Message> handler);

    void SetMsgHandler(MessageID id, MsgHandler<MassiveMsg> handler);

    bool PostMsg(Message msg);

    bool PostMsg(MassiveMsg msg);

    void Connect(EndPoint endpoint);

    public ConfiguredTaskAwaitable<Message?> PostRelayMsg(RelayMsg msg);

    void Update();

    void Disconnect();

    event OnDisconnectedEventHandler? Disconnected;

    event OnConnectedEventHandler? Connected;
}