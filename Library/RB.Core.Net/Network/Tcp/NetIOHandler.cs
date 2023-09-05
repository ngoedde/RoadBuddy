using System.Runtime.CompilerServices;

namespace RB.Core.Net.Network.Tcp;

public abstract class NetIOHandler : INetIOHandler
{
    private int _asynchronousIOCompletionCount;
    private int _synchronousIOCompletionCount;

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

    public int SynchronousIOCompletionCount => _synchronousIOCompletionCount;
    public int AsynchronousIOCompletionCount => _asynchronousIOCompletionCount;
}