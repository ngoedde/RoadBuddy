using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;

using System.Diagnostics;
using System.Net.Sockets;
using RB.Core.Net.Network.Memory;
using RB.Core.Net.Network.Memory.EventArgs;

namespace RB.Core.Net.Network.Tcp;

public class NetSender : NetIOHandler, INetSender
{
    private readonly INetDisconnecter _disconnector;
    private readonly PinnedMemoryPool _memoryPool;
    private readonly INetEventArgsPool<SendNetEventArgs> _sendEventArgsPool;

    private readonly NetSendEventHandler _sent;

    public NetSender(INetDisconnecter disconnector, NetSendEventHandler sent)
    {
        _disconnector = disconnector;
        _sent = sent;

        _memoryPool = new PinnedMemoryPool();
        _sendEventArgsPool = new NetEventArgsPool<SendNetEventArgs>(this.SendCompleted);
        _sendEventArgsPool.Allocate(1024); // TODO: From config
    }

    public bool Send(Session session, Message msg)
    {
        var args = _sendEventArgsPool.Rent();
        Debug.Assert(args != null);

        try
        {
            msg.Retain();

            args.Session = session;
            args.Message = msg;
            args.SetBuffer(msg.GetWrittenMemory());

            //If the I/O operation is pending, the SocketAsyncEventArgs.Completed event will be raised upon completion of the operation.
            if (session.Socket.SendAsync(args))
                return true;

            // The I/O operation completed synchronously, SocketAsyncEventArgs.Completed event will not be raised.
            this.ReportSyncIO();
            this.SendCompleted(session.Socket, args);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            this.OnDisconnect(session, DisconnectReason.SendError);
            _sendEventArgsPool.Return(args);
            return false;
        }
    }

    private void SendCompleted(object? sender, SocketAsyncEventArgs e)
    {
        this.ReportAsyncIO();
        this.SendCompleted(sender, (SendNetEventArgs)e);
    }

    private void SendCompleted(object? sender, SendNetEventArgs args)
    {
        if (args.Session == null)
        {
            Console.WriteLine($"{nameof(this.SendCompleted)}: {nameof(Session)} is NULL!");

            _sendEventArgsPool.Return(args);
            return;
        }

        if (args.BytesTransferred == 0 /*FIN*/ || args.SocketError is SocketError.ConnectionReset
                                                                   or SocketError.TimedOut)
        {
            this.OnDisconnect(args.Session, DisconnectReason.ClosedByPeer);
            _sendEventArgsPool.Return(args);
            return;
        }

        if (args.SocketError is not SocketError.Success)
        {
            Console.WriteLine($"Unhandled SocketError in {nameof(this.SendCompleted)}: {args.SocketError}");

            this.OnDisconnect(args.Session, DisconnectReason.SendError);
            _sendEventArgsPool.Return(args);
            return;
        }

        this.OnSent(args.Session, args.BytesTransferred);
        _sendEventArgsPool.Return(args);
    }

    protected virtual void OnSent(Session session, int bytesTransferred)
        => _sent?.Invoke(session, bytesTransferred);

    protected virtual void OnDisconnect(Session session, DisconnectReason reason)
        => _disconnector.Disconnect(session, reason);
}