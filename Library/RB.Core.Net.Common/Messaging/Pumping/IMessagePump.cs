using System.Diagnostics.CodeAnalysis;

namespace RB.Core.Net.Common.Messaging.Pumping;

public interface IMessagePump
{
    void Enqueue(Message message);

    bool TryGetMessage([MaybeNullWhen(false)] out Message message);
}