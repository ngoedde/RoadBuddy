namespace RB.Core.Net.Network.Tcp;

public delegate void NetReceiveEventHandler(Session session, Memory<byte> buffer, int bytesTransferred);