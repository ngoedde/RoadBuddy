using RB.Core.Net.Common.Extensions;

using System.Net.Sockets;

namespace RB.Core.Net.Network;

public class Session
{
    public int Id { get; init; }
    internal Socket Socket { get; init; }
    public string RemoteAddress { get; init; }
    public SessionType Type { get; internal set; } = SessionType.Invalid;
    public IKeepAliveInfo KeepAliveInfo { get; }

    public IProtocol Protocol { get; }

    public Session(int id, Socket socket, IProtocol protocol)
    {
        this.Id = id;
        this.Socket = socket;
        this.RemoteAddress = socket.GetRemoteAddress();

        this.KeepAliveInfo = new KeepAliveInfo();
        this.Protocol = protocol;
    }

    public override string ToString() => $"#{this.Id} ({this.RemoteAddress})";

}