using System;

namespace RB.Core.Net.Network.Tcp;

public delegate void NetSendEventHandler(Session session, int bytesTransferred);