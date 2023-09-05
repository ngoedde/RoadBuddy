namespace RB.Core.Net.Common.Protocol.KeyExchange;

public interface IKeyExchangeContext : IMessageEncodingContext
{
    KeyExchangeState KeyState { get; set; }
    ulong InitialKey { get; set; }
    uint Generator { get; set; }
    uint Prime { get; set; }
    uint Private { get; set; }
    uint LocalPublic { get; set; }
    uint RemotePublic { get; set; }
    uint SharedSecret { get; set; }
}