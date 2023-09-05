using RB.Core.Net.Common.Messaging;

namespace RB.Core.Net.Common.Protocol.Encoding.Encryption;

public interface IMessageEncryptor
{
    EncodeResult Encrypt(IMessageEncodingContext session, Message msg);
}