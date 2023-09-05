using RB.Core.Net.Common.Messaging;

namespace RB.Core.Net.Common.Protocol.Encoding;

public interface IMessageEncoder
{
    public EncodeResult Encode(IMessageEncodingContext context, Message msg);
}