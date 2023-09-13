namespace RB.Core.Net.Common;

public enum DisconnectReason
{
    Invalid,
    EngineError,
    EngineShutdown,
    Intentional,
    TimeOut,
    ClosedByPeer,
    ReceiveError,
    SendError
}