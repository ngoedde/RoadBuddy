using RB.Core.Net.Common;

namespace RB.Core.Net.Network.Tcp;

public delegate void NetDisconnectEventHandler(Session session, DisconnectReason reason);