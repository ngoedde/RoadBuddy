using RB.Core.Net.Common.Messaging;

namespace RB.Core.Net.Common.Protocol.Decoding;

public interface IMessageDecoder
{
    public DecodeResult Decode(IMessageEncodingContext context, Message msg);
}