using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Protocol.Encoding.Encryption;

namespace RB.Core.Net.Common.Protocol.Encoding;

public class ServerMessageEncoder : IMessageEncoder
{
    private readonly IMessageEncryptor _encrypter;

    public ServerMessageEncoder(IMessageEncryptor encrypter)
    {
        _encrypter = encrypter;
    }

    public static IMessageEncoder Shared { get; } = new ServerMessageEncoder(MessageEncryptor.Shared);

    public EncodeResult Encode(IMessageEncodingContext context, Message msg)
    {
        if (context.IsTrusted || msg.ID == NetMsgId.NetFileIo)
            return EncodeResult.Success;

        msg.Checksum = 0;
        msg.Sequence = 0;

        return _encrypter.Encrypt(context, msg);
    }
}