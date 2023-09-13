using System.Threading.Channels;

namespace RB.Core.Net.Common.Extensions;

public static class ChannelWriterExtensions
{
    public static async ValueTask<bool> TryWriteAsync<T>(this ChannelWriter<T> writer, T item,
        CancellationToken cancellationToken = default)
    {
        while (await writer.WaitToWriteAsync(cancellationToken).ConfigureAwait(false))
            if (writer.TryWrite(item))
                return true;
        return false;
    }

    public static async ValueTask<bool> TryWriteAsync<T>(this ChannelWriter<T> writer, T item, int timeout,
        CancellationToken cancellationToken = default)
    {
        using var timedTokenSource = new CancellationTokenSource(timeout);
        using var combinedTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource(timedTokenSource.Token, cancellationToken);
        while (await writer.WaitToWriteAsync(combinedTokenSource.Token).ConfigureAwait(false))
            if (writer.TryWrite(item))
                return true;
        return false;
    }
}