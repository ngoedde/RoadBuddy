
using System.Runtime.InteropServices;

namespace RB.Core.Net.Common.Protocol.KeyExchange;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
public struct ClientKeyInfo
{
    public uint PublicKey;
    public ulong Signature;
}