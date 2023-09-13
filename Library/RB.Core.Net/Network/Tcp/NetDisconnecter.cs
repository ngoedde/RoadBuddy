using System.Net.Sockets;
using RB.Core.Net.Common;
using RB.Core.Net.Network.Memory;
using RB.Core.Net.Network.Memory.EventArgs;
using Serilog;

namespace RB.Core.Net.Network.Tcp;

public class NetDisconnecter : NetIOHandler, INetDisconnecter
{
    private readonly NetDisconnectEventHandler _disconnected;
    private readonly INetEventArgsPool<DisconnectNetEventArgs> _disconnectEventArgsPool;
    private readonly ISocketPool _socketPool;

    public NetDisconnecter(ISocketPool socketPool, NetDisconnectEventHandler disconnected)
    {
        _socketPool = socketPool;
        _disconnected = disconnected;

        _disconnectEventArgsPool = new NetEventArgsPool<DisconnectNetEventArgs>(DisconnectCompleted);
        _disconnectEventArgsPool.Allocate(1024); // TODO: From config
    }

    public void Disconnect(Session session, DisconnectReason reason)
    {
        ArgumentNullException.ThrowIfNull(session);

        var args = _disconnectEventArgsPool.Rent();
        try
        {
            args.Session = session;
            args.Reason = reason;
            args.DisconnectReuseSocket = true;

            //If the I/O operation is pending, the SocketAsyncEventArgs.Completed event will be raised upon completion of the operation.
            if (session.Socket.DisconnectAsync(args))
                return;

            // The I/O operation completed synchronously, SocketAsyncEventArgs.Completed event will not be raised.
            ReportSyncIO();
            DisconnectCompleted(session.Socket, args);
        }
        catch (Exception ex)
        {
            Log.Debug($"Socket error: {ex.Message}");
            _disconnectEventArgsPool.Return(args);
        }
    }

    private void DisconnectCompleted(object? sender, SocketAsyncEventArgs e)
    {
        ReportAsyncIO();
        DisconnectCompleted((DisconnectNetEventArgs)e);
    }

    private void DisconnectCompleted(DisconnectNetEventArgs args)
    {
        if (args.Session == null)
        {
            Console.WriteLine($"{nameof(this.DisconnectCompleted)}: {nameof(Session)} is NULL!");

            _disconnectEventArgsPool.Return(args);
            return;
        }

        if (args.SocketError is SocketError.NotConnected)
        {
            _disconnectEventArgsPool.Return(args);
            return;
        }

        if (args.SocketError != SocketError.Success)
        {
            Console.WriteLine($"{nameof(this.DisconnectCompleted)}: {args.SocketError}");

            _disconnectEventArgsPool.Return(args);
            return;
        }

        try
        {
            OnDisconnected(args.Session, args.Reason);
        }
        finally
        {
            var socket = args.Session.Socket;
            _disconnectEventArgsPool.Return(args);
            _socketPool.Return(socket);
        }
    }

    protected virtual void OnDisconnected(Session session, DisconnectReason reason)
    {
        _disconnected.Invoke(session, reason);
    }
}