using System.Buffers;
using System.Diagnostics;
using System.Net.Sockets;
using RB.Core.Net.Common;
using RB.Core.Net.Network.Memory;
using RB.Core.Net.Network.Memory.EventArgs;

namespace RB.Core.Net.Network.Tcp;

internal class NetReceiver : NetIOHandler, INetReceiver
{
    private readonly INetDisconnecter _disconnector;
    private readonly MemoryPool<byte> _memoryPool;

    private readonly NetReceiveEventHandler _received;
    private readonly INetEventArgsPool<ReceiveNetEventArgs> _receiveEventArgsPool;

    public NetReceiver(INetDisconnecter disconnector, NetReceiveEventHandler received)
    {
        _disconnector = disconnector;
        _received = received;

        _memoryPool = new PinnedMemoryPool();
        //_receiveEventArgsPool = new NetEventArgsPool<ReceiveNetEventArgs>(this.ReceiveCompleted);
        _receiveEventArgsPool = new ReceiveNetEventArgsPool(ReceiveCompleted);
        _receiveEventArgsPool.Allocate(1024); // TODO: From config
    }

    public void Receive(Session session)
    {
        var args = _receiveEventArgsPool.Rent();
        Debug.Assert(args != null);

        try
        {
            args.Session = session;

            // If the I/O operation is pending, the SocketAsyncEventArgs.Completed event will be raised upon completion of the operation.
            if (session.Socket.ReceiveAsync(args))
                return;

            // The I/O operation completed synchronously, SocketAsyncEventArgs.Completed event will not be raised.
            ReportSyncIO();
            ReceiveCompleted(session.Socket, args);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            OnDisconnect(session, DisconnectReason.ReceiveError);
            _receiveEventArgsPool.Return(args);
        }
    }

    private void ReceiveCompleted(object? sender, SocketAsyncEventArgs args)
    {
        ReportAsyncIO();
        ReceiveCompleted(sender, (ReceiveNetEventArgs)args);
    }

    private void ReceiveCompleted(object? sender, ReceiveNetEventArgs args)
    {
        if (args.Session == null)
        {
            Console.WriteLine($"{nameof(this.ReceiveCompleted)}: {nameof(Session)} is NULL!");

            _receiveEventArgsPool.Return(args);
            return;
        }

        // Closed by peer...
        if (args.BytesTransferred == 0 /*FIN*/ || args.SocketError is SocketError.ConnectionReset
                or SocketError.TimedOut)
        {
            OnDisconnect(args.Session, DisconnectReason.ClosedByPeer);
            _receiveEventArgsPool.Return(args);
            return;
        }

        if (args.SocketError != SocketError.Success)
        {
            Console.WriteLine($"Unhandled SocketError in {nameof(this.ReceiveCompleted)}: {args.SocketError}");

            OnDisconnect(args.Session, DisconnectReason.ReceiveError);
            _receiveEventArgsPool.Return(args);
            return;
        }

        var session = args.Session;
        try
        {
            OnReceived(session, args.MemoryBuffer, args.BytesTransferred);
        }
        finally
        {
            _receiveEventArgsPool.Return(args);
            Receive(session);
        }
    }

    protected virtual void OnReceived(Session session, Memory<byte> buffer, int bytesTransferred)
    {
        _received?.Invoke(session, buffer, bytesTransferred);
    }

    protected virtual void OnDisconnect(Session session, DisconnectReason reason)
    {
        _disconnector.Disconnect(session, reason);
    }
}