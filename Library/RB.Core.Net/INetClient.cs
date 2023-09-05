using System.Net;
using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Messaging.Allocation;
using RB.Core.Net.Handling;

namespace RB.Core.Net;

public interface INetClient : IMessageAllocator
{
    void SetMsgHandler(MessageID id, MsgHandler<Message> handler);
    bool PostMsg(Message msg);
    
    void Connect(EndPoint endpoint); 
    bool PostConnect(string? hostOrIP, ushort port);
    
    bool PostDisconnect(int id, DisconnectReason reason = DisconnectReason.Intentional);
    void Update();
}