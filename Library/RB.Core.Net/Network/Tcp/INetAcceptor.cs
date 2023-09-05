namespace RB.Core.Net.Network.Tcp;

public interface INetAcceptor : INetIOHandler
{
    //NetAcceptedEventHandler? Accepted { get; set; }

    void Listen(string hostOrIP, ushort port);
}