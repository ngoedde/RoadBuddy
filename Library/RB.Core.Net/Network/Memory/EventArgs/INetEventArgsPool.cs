using RB.Core.Net.Common.Memory;

namespace RB.Core.Net.Network.Memory.EventArgs;

public interface INetEventArgsPool<TEventArgs> : ICustomObjectPool<TEventArgs>
    where TEventArgs : NetEventArgs
{
}