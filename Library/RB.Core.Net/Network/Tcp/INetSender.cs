using RB.Core.Net.Common.Messaging;

namespace RB.Core.Net.Network.Tcp;

public interface INetSender : INetIOHandler
{
    //NetSendEventHandler? Sent { get; set; }

    bool Send(Session session, Message msg);
}