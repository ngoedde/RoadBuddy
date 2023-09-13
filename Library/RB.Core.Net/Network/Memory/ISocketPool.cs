using System.Net.Sockets;
using RB.Core.Net.Common.Memory;

namespace RB.Core.Net.Network.Memory;

public interface ISocketPool : ICustomObjectPool<Socket>
{
}