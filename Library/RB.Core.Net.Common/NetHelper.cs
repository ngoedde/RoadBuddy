using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace RB.Core.Net.Common;

public static class NetHelper
{
    internal const int BufferSize = 4096;

    public static Socket CreateTcpSocket()
    {
        return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public static Socket CreateUdpSocket()
    {
        return new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    public static IPEndPoint ToIPEndPoint(string? hostOrIP, ushort port)
    {
        ArgumentNullException.ThrowIfNull(hostOrIP);

        if (!IPAddress.TryParse(hostOrIP, out var address))
            address = Array.Find(Dns.GetHostEntry(hostOrIP).AddressList,
                p => p.AddressFamily == AddressFamily.InterNetwork);

        return new IPEndPoint(address!, port);
    }

    public static string? AddressFromEndPoint(EndPoint endPoint)
    {
        return (endPoint as IPEndPoint)?.ToString();
    }

    public static IPAddress? IPAddressFromEndPoint(EndPoint endPoint)
    {
        return (endPoint as IPEndPoint)?.Address;
    }

    public static int PortFromEndPoint(EndPoint endPoint)
    {
        return (endPoint as IPEndPoint)?.Port ?? -1;
    }

    public static string IntToIP(uint ip)
    {
        var b0 = (byte)((ip >> 24) & byte.MaxValue);
        var b1 = (byte)((ip >> 16) & byte.MaxValue);
        var b2 = (byte)((ip >> 8) & byte.MaxValue);
        var b3 = (byte)((ip >> 0) & byte.MaxValue);
        return $"{b0}.{b1}.{b2}.{b3}";
        //return $"{b3}.{b2}.{b1}.{b0}";
    }

    public static int IPToInt(ReadOnlySpan<char> ip)
    {
        Span<byte> span = stackalloc byte[4];

        for (var i = 0; i < 4; i++)
        {
            var index = ip.IndexOf('.');
            if (!byte.TryParse(ip[..(index > 0 ? index : ip.Length)], out span[i]))
                return -1;

            ip = ip[(index + 1)..];
        }

        return (span[0] << 24) | (span[1] << 16) | (span[2] << 08) | (span[3] << 00);
        //return (span[3] << 24) | (span[2] << 16) | (span[1] << 08) | (span[0] << 00);
    }

    public static IPAddress? IdentifyIPAddress()
    {
        if (!NetworkInterface.GetIsNetworkAvailable())
            return IPAddress.Loopback;

        var hostName = Dns.GetHostName();
        var hostEntry = Dns.GetHostEntry(hostName);
        return Array.Find(hostEntry.AddressList, p => p.AddressFamily == AddressFamily.InterNetwork);
    }
}