using System.Net.Sockets;
using RB.Core.Net.Common.Extensions;

namespace RB.Core.Net.Network;

public class Session
{
    public Session(int id, Socket socket, IProtocol protocol)
    {
        Id = id;
        Socket = socket;
        RemoteAddress = socket.GetRemoteAddress();

        KeepAliveInfo = new KeepAliveInfo();
        Protocol = protocol;
    }

    public int Id { get; init; }
    internal Socket Socket { get; init; }
    public string RemoteAddress { get; init; }
    public SessionType Type { get; internal set; } = SessionType.Invalid;
    public IKeepAliveInfo KeepAliveInfo { get; }

    public IProtocol Protocol { get; }

    public override string ToString()
    {
        return $"#{Id} ({RemoteAddress})";
    }
}