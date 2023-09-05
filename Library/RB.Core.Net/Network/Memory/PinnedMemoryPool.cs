using System;
using System.Buffers;
using System.Collections.Concurrent;

namespace RB.Core.Net.Network.Memory;

public class PinnedMemoryPool : MemoryPool<byte>
{
    private readonly ConcurrentQueue<PinnedMemoryPoolBlock> _blocks = new ConcurrentQueue<PinnedMemoryPoolBlock>();

    public bool Disposed { get; private set; }

    public override int MaxBufferSize { get; } = 8192;

    public override IMemoryOwner<byte> Rent(int minBufferSize = -1)
    {
        if (this.Disposed)
            throw new ObjectDisposedException(nameof(PinnedMemoryPool));

        if (minBufferSize > this.MaxBufferSize)
            throw new ArgumentOutOfRangeException(nameof(minBufferSize));

        if (minBufferSize < 0)
            minBufferSize = this.MaxBufferSize;

        if (!_blocks.TryDequeue(out PinnedMemoryPoolBlock? block))
            block = new PinnedMemoryPoolBlock(this, minBufferSize);

        return block;
    }

    internal void Return(PinnedMemoryPoolBlock block)
    {
        if (this.Disposed)
            return;

        _blocks.Enqueue(block);
    }

    protected override void Dispose(bool disposing)
    {
        if (this.Disposed)
            return;

        // Free your own state (unmanaged objects).
        this.Disposed = true;

        if (!disposing)
            return;

        // Free other state (managed objects).
        _blocks.Clear();
    }
}