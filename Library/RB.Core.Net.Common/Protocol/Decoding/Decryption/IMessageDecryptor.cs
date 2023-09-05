using RB.Core.Net.Common.Messaging;

namespace RB.Core.Net.Common.Protocol.Decoding.Decryption;

public interface IMessageDecryptor
{
    DecodeResult Decrypt(IMessageEncodingContext context, Message msg);
}