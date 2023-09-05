using RB.Core.Net.Common;

namespace RB.Core.Net.Network.Tcp;

public interface INetDisconnecter : INetIOHandler
{
    //NetDisconnectEventHandler? Disconnected { get; set; }

    void Disconnect(Session session, DisconnectReason reason);
}