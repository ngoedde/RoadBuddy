using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RB.Core.Net.Common.Extensions;

public static class ProcessExtensions
{
    /// <summary>
    /// Waits asynchronously for the process to exit.
    /// </summary>
    /// <param name="process">The process to wait for cancellation.</param>
    /// <param name="cancellationToken">A cancellation token. If invoked, the task will return
    /// immediately as canceled.</param>
    /// <returns>A Task representing waiting for the process to end.</returns>
    public static Task<bool> WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<bool>();
        process.EnableRaisingEvents = true;
        cancellationToken.Register(() => tcs.TrySetResult(false));
        process.Exited += (sender, args) => tcs.TrySetResult(true);
        return tcs.Task;
    }
}