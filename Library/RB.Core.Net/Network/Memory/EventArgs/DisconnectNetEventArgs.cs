using RB.Core.Net.Common;

namespace RB.Core.Net.Network.Memory.EventArgs;

public class DisconnectNetEventArgs : NetEventArgs
{
    public DisconnectReason Reason { get; set; }
}