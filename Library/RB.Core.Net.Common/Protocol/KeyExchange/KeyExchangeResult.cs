namespace RB.Core.Net.Common.Protocol.KeyExchange;

public enum KeyExchangeResult : byte
{
    InvalidState,
    Success,
    InvalidMsg,
    InvalidSignature
}