using RB.Core.Net.Common;
using RB.Core.Net.Common.Protocol;
using RB.Core.Net.Common.Protocol.CRC;
using RB.Core.Net.Common.Protocol.KeyExchange;
using RB.Core.Net.Common.Protocol.Sequence;

namespace RB.Core.Net;

public class ProtocolContext : IProtocolContext
{
    public ProtocolContext()
    {
        Blowfish = new Blowfish();
        Checksummer = new MessageChecksummer();
        Sequencer = new MessageSequencer();
    }

    public KeyExchangeState KeyState { get; set; }
    public ulong InitialKey { get; set; }
    public uint Generator { get; set; }
    public uint Prime { get; set; }
    public uint Private { get; set; }
    public uint LocalPublic { get; set; }
    public uint RemotePublic { get; set; }
    public uint SharedSecret { get; set; }
    public bool IsTrusted { get; set; }
    public IMessageChecksummer Checksummer { get; }
    public IMessageSequencer Sequencer { get; }
    public ProtocolOptions Options { get; set; }
    public Blowfish Blowfish { get; }
}