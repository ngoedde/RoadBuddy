using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Protocol.Decoding.Decryption;

namespace RB.Core.Net.Common.Protocol.Decoding;

public class ServerMessageDecoder : IMessageDecoder
{
    private readonly IMessageDecryptor _decrypter;

    public static IMessageDecoder Shared { get; } = new ServerMessageDecoder(MessageDecryptor.Shared);

    public ServerMessageDecoder(IMessageDecryptor decrypter)
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
            if (msg.Sequence != context.Sequencer.Next())
                return DecodeResult.InvalidSequence;

            var msgChecksum = msg.Checksum;
            msg.Checksum = 0;
            if (msgChecksum != context.Checksummer.Compute(msg.GetSpan(), msg.DataSize + Message.HEADER_SIZE))
                return DecodeResult.InvalidChecksum;
        }

        return DecodeResult.Success;
    }
}