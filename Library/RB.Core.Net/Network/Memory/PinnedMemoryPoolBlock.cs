using System.Buffers;
using System.Runtime.InteropServices;

namespace RB.Core.Net.Network.Memory;

internal class PinnedMemoryPoolBlock : IMemoryOwner<byte>
{
    internal PinnedMemoryPoolBlock(PinnedMemoryPool pool, int length)
    {
        Pool = pool;

        var pinnedBuffer = GC.AllocateUninitializedArray<byte>(length, true);
        Memory = MemoryMarshal.CreateFromPinnedArray(pinnedBuffer, 0, pinnedBuffer.Length);
    }

    public PinnedMemoryPool Pool { get; }
    public Memory<byte> Memory { get; }

    public void Dispose()
    {
        Pool.Return(this);
    }
}