using System.Net;

namespace RB.Core.Net.Network.Tcp;

public interface INetConnector : INetIOHandler
{
    //NetConnectedEventHandler? _connected { get; set; }

    void Connect(EndPoint remoteEndPoint);

    void Connect(string hostOrIP, ushort port);
}