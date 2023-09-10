
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace RB.Core.Net.Common.Messaging.Pumping;

public class MessagePump : IMessagePump
{
    private readonly ConcurrentQueue<Message> _queue = new();

    public void Enqueue(Message message) => _queue.Enqueue(message);

    public bool TryGetMessage([MaybeNullWhen(false)] out Message message) => _queue.TryDequeue(out message);
}