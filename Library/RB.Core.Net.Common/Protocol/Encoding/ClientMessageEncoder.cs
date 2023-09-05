using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Protocol.Encoding.Encryption;

namespace RB.Core.Net.Common.Protocol.Encoding;

public class ClientMessageEncoder : IMessageEncoder
{
    private readonly IMessageEncryptor _encrypter;

    public static IMessageEncoder Shared { get; } = new ClientMessageEncoder(MessageEncryptor.Shared);

    public ClientMessageEncoder(IMessageEncryptor encrypter)
    {
        _encrypter = encrypter;
    }

    public EncodeResult Encode(IMessageEncodingContext context, Message msg)
    {
        if (context.IsTrusted || msg.ID == NetMsgID.NET_FILE_IO)
            return EncodeResult.Success;

        if ((context.Options & ProtocolOptions.ErrorDetection) != 0)
        {
            msg.Sequence = context.Sequencer.Next();

            msg.Checksum = 0;
            msg.Checksum = context.Checksummer.Compute(msg.GetSpan(), msg.DataSize + Message.HEADER_SIZE);
        }

        return _encrypter.Encrypt(context, msg);
    }
}