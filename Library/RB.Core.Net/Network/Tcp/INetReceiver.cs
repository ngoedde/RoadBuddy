namespace RB.Core.Net.Network.Tcp;

public interface INetReceiver : INetIOHandler
{
    //NetReceiveEventHandler? Received { get; set; }

    void Receive(Session session);
}