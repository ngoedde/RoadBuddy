using System.Net.Sockets;

namespace RB.Core.Net.Network.Memory.EventArgs;

public class NetEventArgs : SocketAsyncEventArgs
{
    public Session? Session { get; set; }

    internal virtual void Clear()
    {
        AcceptSocket = null;
        UserToken = null;
        Session = null;
    }
}