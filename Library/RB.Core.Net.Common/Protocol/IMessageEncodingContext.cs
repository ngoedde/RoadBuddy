using RB.Core.Net.Common.Protocol.CRC;
using RB.Core.Net.Common.Protocol.Sequence;

namespace RB.Core.Net.Common.Protocol;

public interface IMessageEncodingContext : IMessageCryptoContext
{
    bool IsTrusted { get; set; }
    public IMessageChecksummer Checksummer { get; }
    public IMessageSequencer Sequencer { get; }
}