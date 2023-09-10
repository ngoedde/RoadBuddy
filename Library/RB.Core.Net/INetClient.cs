using System.Net;
using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Messaging.Allocation;
using RB.Core.Net.Common.Messaging.Handling;

namespace RB.Core.Net;

public interface INetClient : IMessageAllocator, IMassiveMsgAllocator
{
    string Identity { get; }
    
    int ServerId { get; }
    
    void SetMsgHandler(MessageID id, MsgHandler<Message> handler);
    
    void SetMsgHandler(MessageID id, MsgHandler<MassiveMsg> handler);
    
    bool PostMsg(Message msg);
    
    bool PostMsg(MassiveMsg msg);
    
    void Connect(EndPoint endpoint);

    void Update();
}