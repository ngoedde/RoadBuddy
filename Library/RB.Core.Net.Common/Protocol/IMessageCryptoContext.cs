namespace RB.Core.Net.Common.Protocol;

public interface IMessageCryptoContext
{
    ProtocolOptions Options { get; set; }
    Blowfish Blowfish { get; }
}