using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace RB.Core.Net.Network.Memory;

internal class PinnedMemoryPoolBlock : IMemoryOwner<byte>
{
    public Memory<byte> Memory { get; }
    public PinnedMemoryPool Pool { get; }

    internal PinnedMemoryPoolBlock(PinnedMemoryPool pool, int length)
    {
        this.Pool = pool;

        var pinnedBuffer = GC.AllocateUninitializedArray<byte>(length, pinned: true);
        this.Memory = MemoryMarshal.CreateFromPinnedArray(pinnedBuffer, 0, pinnedBuffer.Length);
    }

    public void Dispose() => this.Pool.Return(this);
}