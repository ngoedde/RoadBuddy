using System.Buffers;
using System.Collections.Concurrent;

namespace RB.Core.Net.Network.Memory;

public class PinnedMemoryPool : MemoryPool<byte>
{
    private readonly ConcurrentQueue<PinnedMemoryPoolBlock> _blocks = new();

    public bool Disposed { get; private set; }

    public override int MaxBufferSize { get; } = 8192;

    public override IMemoryOwner<byte> Rent(int minBufferSize = -1)
    {
        if (Disposed)
            throw new ObjectDisposedException(nameof(PinnedMemoryPool));

        if (minBufferSize > MaxBufferSize)
            throw new ArgumentOutOfRangeException(nameof(minBufferSize));

        if (minBufferSize < 0)
            minBufferSize = MaxBufferSize;

        if (!_blocks.TryDequeue(out var block))
            block = new PinnedMemoryPoolBlock(this, minBufferSize);

        return block;
    }

    internal void Return(PinnedMemoryPoolBlock block)
    {
        if (Disposed)
            return;

        _blocks.Enqueue(block);
    }

    protected override void Dispose(bool disposing)
    {
        if (Disposed)
            return;

        // Free your own state (unmanaged objects).
        Disposed = true;

        if (!disposing)
            return;

        // Free other state (managed objects).
        _blocks.Clear();
    }
}