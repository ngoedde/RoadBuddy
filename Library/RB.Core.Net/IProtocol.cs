using System.Diagnostics.CodeAnalysis;
using RB.Core.Net.Common.Messaging;

namespace RB.Core.Net;

public interface IProtocol
{
    bool Encode(Message msg);
    bool Decode(Message msg);
    bool TryGetMessage([MaybeNullWhen(false)] out Message message);
    bool Receive(Span<byte> segment);
    bool SetTrusted(bool trusted);

    bool ProcessReq(Message msg);
    bool ProcessAck(Message msg);
    bool Initialize(int receiverId);
}