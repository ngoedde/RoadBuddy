using System.Diagnostics;
using RB.Core.Net.Common.Messaging;

namespace RB.Core.Net.Common.Protocol.Decoding.Decryption;

public class MessageDecryptor : IMessageDecryptor
{
    public static IMessageDecryptor Shared { get; } = new MessageDecryptor();

    public DecodeResult Decrypt(IMessageEncodingContext context, Message msg)
    {
        if (!msg.Encrypted)
            return DecodeResult.Success;

        if ((context.Options & ProtocolOptions.Encryption) == 0)
            return DecodeResult.InvalidHeader;

        var inputLength = Message.HEADER_ENC_SIZE + msg.DataSize;
        var outputLength = Blowfish.GetOutputLength(inputLength);
        if (outputLength + Message.HEADER_ENC_OFFSET > Message.BUFFER_SIZE)
        {
            Console.WriteLine($"{nameof(this.Decrypt)}: Message exceeded buffer size. [{msg.ID}]");
            return DecodeResult.InvalidMsgSize;
        }

        var result = context.Blowfish.Decode(msg.GetSpan(Message.HEADER_ENC_OFFSET, outputLength));
        Debug.Assert(result == outputLength);
        return DecodeResult.Success;
    }
}