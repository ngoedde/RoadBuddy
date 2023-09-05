namespace RB.Core.Net.Network.Tcp;

public interface INetIOHandler
{
    int SynchronousIOCompletionCount { get; }
    int AsynchronousIOCompletionCount { get; }
}