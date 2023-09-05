using RB.Core.Net.Common.Memory;

using System.Net.Sockets;

namespace RB.Core.Net.Network.Memory;

public interface ISocketPool : ICustomObjectPool<Socket>
{
}