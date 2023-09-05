using RB.Core.Net.Common;

using System;

namespace RB.Core.Net.Network.Tcp;

public delegate void NetDisconnectEventHandler(Session session, DisconnectReason reason);