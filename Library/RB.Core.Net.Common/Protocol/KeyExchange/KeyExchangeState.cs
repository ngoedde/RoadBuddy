namespace RB.Core.Net.Common.Protocol.KeyExchange;

public enum KeyExchangeState : byte
{
    Uninitialized,
    Initialized,
    Challenged,
    Accepted
}