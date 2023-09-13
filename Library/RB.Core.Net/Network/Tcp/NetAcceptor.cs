using System.Net.Sockets;
using RB.Core.Net.Common;
using RB.Core.Net.Network.Memory;
using RB.Core.Net.Network.Memory.EventArgs;
using Serilog;

namespace RB.Core.Net.Network.Tcp;

internal class NetAcceptor : NetIOHandler, INetAcceptor
{
    private readonly NetAcceptedEventHandler _accepted;

    private readonly INetEventArgsPool<NetEventArgs> _acceptEventArgsPool;
    private readonly Socket _listenerSocket;
    private readonly ISocketPool _socketPool;

    public NetAcceptor(ISocketPool socketPool, NetAcceptedEventHandler accepted)
    {
        _socketPool = socketPool;
        _accepted = accepted;

        _listenerSocket = NetHelper.CreateTcpSocket();
        _acceptEventArgsPool = new NetEventArgsPool<NetEventArgs>(AcceptCompleted);
        _acceptEventArgsPool.Allocate(1024); // TODO: From config
    }

    public void Listen(string hostOrIP, ushort port)
    {
        _listenerSocket.Bind(NetHelper.ToIPEndPoint(hostOrIP, port));
        _listenerSocket.Listen(128); // TODO: From config

        Log.Information($"Listening on {hostOrIP}:{port}");

        Accept();
    }

    private void Accept()
    {
        var args = _acceptEventArgsPool.Rent();
        var socket = _socketPool.Rent();
        try
        {
            args.AcceptSocket = socket;

            //If the I/O operation is pending, the SocketAsyncEventArgs.Completed event will be raised upon completion of the operation.
            if (_listenerSocket.AcceptAsync(args))
                return;

            // The I/O operation completed synchronously, SocketAsyncEventArgs.Completed event will not be raised.
            ReportSyncIO();
            AcceptCompleted(this, args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{nameof(NetAcceptor)}::{nameof(Accept)}: Failed to {nameof(Socket.AcceptAsync)}");

            _acceptEventArgsPool.Return(args);
            _socketPool.Return(socket);
        }
    }

    private void AcceptCompleted(object? sender, SocketAsyncEventArgs e)
    {
        ReportAsyncIO();
        AcceptCompleted(sender, (NetEventArgs)e);
    }

    private void AcceptCompleted(object? sender, NetEventArgs e)
    {
        if (e.AcceptSocket == null)
        {
            Console.WriteLine($"{nameof(this.AcceptCompleted)}: {nameof(e.AcceptSocket)} is NULL.");

            _acceptEventArgsPool.Return(e);
            return;
        }

        if (e.SocketError != SocketError.Success)
        {
            Console.WriteLine($"{nameof(this.AcceptCompleted)}: {e.SocketError}");

            _socketPool.Return(e.AcceptSocket);
            _acceptEventArgsPool.Return(e);
            return;
        }

        Console.WriteLine($"Accepted {e.AcceptSocket.RemoteEndPoint}!");

        try
        {
            OnAccepted(e.AcceptSocket);
        }
        finally
        {
            _acceptEventArgsPool.Return(e);
            Accept();
        }
    }

    protected virtual void OnAccepted(Socket socket)
    {
        _accepted?.Invoke(socket);
    }
}