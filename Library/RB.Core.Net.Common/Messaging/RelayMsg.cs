using System.Runtime.CompilerServices;

namespace RB.Core.Net.Common.Messaging;

public class RelayMsg : IDisposable
{
    private readonly TaskCompletionSource<Message?> _completionSource;
    public readonly int ReceiverId;

    public MessageID ResponseMsgId;

    internal RelayMsg(Message request, MessageID responseMsgId, int receiverId = -1)
    {
        ResponseMsgId = responseMsgId;
        ReceiverId = receiverId;
        Request = request;

        Request.Retain();

        _completionSource = new TaskCompletionSource<Message?>();
    }

    public Message Request { get; }

    public void Dispose()
    {
        Request.Dispose();
    }

    public ConfiguredTaskAwaitable<Message?> WaitAsync(CancellationToken token = default)
    {
        token.Register(OnCanceled);

        return _completionSource.Task.ConfigureAwait(true);
    }

    private void OnCanceled()
    {
        _completionSource.TrySetResult(null);
    }

    internal bool TrySetResult(Message response)
    {
        return _completionSource.TrySetResult(response);
    }
}