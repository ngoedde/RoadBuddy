using System.Net.Sockets;
using RB.Core.Net.Common;
using RB.Core.Net.Common.Extensions;
using RB.Core.Net.Common.Memory;

namespace RB.Core.Net.Network.Memory;

internal class SocketPool : CustomObjectPool<Socket>, ISocketPool
{
    public override Socket Create()
    {
        var socket = NetHelper.CreateTcpSocket();
        socket.Optimize();
        return socket;
    }
}