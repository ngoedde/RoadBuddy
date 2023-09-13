using System.Runtime.InteropServices;

namespace RB.Core.Net.Common.Protocol.KeyExchange;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ServerKeyInfo
{
    public ulong Key;
    public uint Generator;
    public uint Prime;
    public uint Public;
}