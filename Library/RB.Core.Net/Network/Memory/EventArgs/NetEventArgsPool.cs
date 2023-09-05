using RB.Core.Net.Common.Memory;

using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace RB.Core.Net.Network.Memory.EventArgs;

internal class ReceiveNetEventArgsPool : NetEventArgsPool<ReceiveNetEventArgs>
{
    public ReceiveNetEventArgsPool(EventHandler<SocketAsyncEventArgs> completedEventHandler) : base(completedEventHandler)
    {
    }

    public override ReceiveNetEventArgs Create()
    {
        var result = base.Create();

        var pinnedArray = GC.AllocateUninitializedArray<byte>(8192, pinned: true);
        var memory = MemoryMarshal.CreateFromPinnedArray(pinnedArray, 0, pinnedArray.Length);
        result.SetBuffer(memory);

        return result;
    }
}

internal class NetEventArgsPool<TEventArgs> : CustomObjectPool<TEventArgs>, INetEventArgsPool<TEventArgs>
    where TEventArgs : NetEventArgs, new()
{
    private readonly EventHandler<SocketAsyncEventArgs> _completedEventHandler;

    public NetEventArgsPool(EventHandler<SocketAsyncEventArgs> completedEventHandler)
    {
        _completedEventHandler = completedEventHandler;
    }

    public override TEventArgs Create()
    {
        var args = new TEventArgs();
        args.Completed += _completedEventHandler;
        return args;
    }

    public override void Clear(TEventArgs item)
    {
        item.Clear();
    }
}