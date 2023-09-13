using System.Runtime.CompilerServices;

namespace RB.Core.Net.Network.Tcp;

public abstract class NetIOHandler : INetIOHandler
{
    public int SynchronousIOCompletionCount { get; }

    public int AsynchronousIOCompletionCount { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ReportAsyncIO()
    {
        //Interlocked.Increment(ref _asynchronousIOCompletionCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ReportSyncIO()
    {
        //Interlocked.Increment(ref _synchronousIOCompletionCount);
    }
}