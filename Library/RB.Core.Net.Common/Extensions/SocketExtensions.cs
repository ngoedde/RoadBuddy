using System.Net;
using System.Net.Sockets;

namespace RB.Core.Net.Common.Extensions;

public static class SocketExtensions
{
    public static string GetRemoteAddress(this Socket socket)
    {
        return socket.RemoteEndPoint is not IPEndPoint ipEndPoint ? string.Empty : ipEndPoint.Address.ToString();
    }

    public static int GetRemotePort(this Socket socket)
    {
        return socket.RemoteEndPoint is not IPEndPoint ipEndPoint ? -1 : ipEndPoint.Port;
    }

    public static void Optimize(this Socket socket)
    {
        socket.ExclusiveAddressUse = true;

        socket.SendBufferSize = 8192;
        socket.ReceiveBufferSize = 8192;

        socket.SendTimeout = 1000;
        socket.ReceiveTimeout = 1000;

        //socket.Blocking = false;

        // According to http://ithare.com/tcp-and-websockets-for-games/
        // Disable "Nagle"-Algorithm
        socket.NoDelay = true;

        // Linger
        ArgumentNullException.ThrowIfNull(socket?.LingerState);
        socket.LingerState.Enabled = true;
        socket.LingerState.LingerTime = 0;
    }
}