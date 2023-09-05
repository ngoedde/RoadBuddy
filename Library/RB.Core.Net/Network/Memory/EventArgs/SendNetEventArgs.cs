using RB.Core.Net.Common.Messaging;

namespace RB.Core.Net.Network.Memory.EventArgs;

public class SendNetEventArgs : NetEventArgs
{
    public Message? Message { get; set; }

    internal override void Clear()
    {
        base.Clear();
        this.Message?.Dispose();
        this.Message = null;
    }
}