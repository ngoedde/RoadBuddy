using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Protocol.Decoding.Decryption;

namespace RB.Core.Net.Common.Protocol.Decoding;

public class ClientMessageDecoder : IMessageDecoder
{
    private readonly IMessageDecryptor _decrypter;

    public static IMessageDecoder Shared { get; } = new ClientMessageDecoder(MessageDecryptor.Shared);

    public ClientMessageDecoder(IMessageDecryptor decrypter)
    {
        _decrypter = decrypter;
    }

    public DecodeResult Decode(IMessageEncodingContext context, Message msg)
    {
        if (context.IsTrusted || msg.ID == NetMsgID.NET_FILE_IO)
            return DecodeResult.Success;

        var result = _decrypter.Decrypt(context, msg);
        if (result != DecodeResult.Success)
            return result;

        if ((context.Options & ProtocolOptions.ErrorDetection) != 0)
        {
            if (msg.Sequence != 0) return DecodeResult.InvalidSequence;
            if (msg.Checksum != 0) return DecodeResult.InvalidChecksum;
        }

        return DecodeResult.Success;
    }
}