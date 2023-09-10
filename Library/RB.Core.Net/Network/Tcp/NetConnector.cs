using RB.Core.Net.Common;

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using RB.Core.Net.Network.Memory;
using RB.Core.Net.Network.Memory.EventArgs;
using Serilog;

namespace RB.Core.Net.Network.Tcp;

internal class NetConnector : NetIOHandler, INetConnector
{
    private readonly NetConnectedEventHandler _connected;

    private readonly ISocketPool _socketPool;
    private readonly INetEventArgsPool<NetEventArgs> _connectEventArgsPool;

    public NetConnector(ISocketPool socketPool, NetConnectedEventHandler connected)
    {
        _socketPool = socketPool;
        _connected = connected;

        _connectEventArgsPool = new NetEventArgsPool<NetEventArgs>(this.ConnectCompleted);
        _connectEventArgsPool.Allocate(1024);
    }

    public void Connect(string hostOrIP, ushort port) => this.Connect(NetHelper.ToIPEndPoint(hostOrIP, port));

    public void Connect(EndPoint remoteEndPoint)
    {
        var connectArgs = _connectEventArgsPool.Rent();
        var socket = _socketPool.Rent();

        try
        {
            connectArgs.RemoteEndPoint = remoteEndPoint;

            Log.Debug($"Connecting to {remoteEndPoint}...");

            // If the I/O operation is pending, the SocketAsyncEventArgs.Completed event will be raised upon completion of the operation.
            if (socket.ConnectAsync(connectArgs))
                return;

            // The I/O operation completed synchronously, SocketAsyncEventArgs.Completed event will not be raised.
            this.ReportSyncIO();
            this.ConnectCompleted(socket, connectArgs);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);

            _socketPool.Return(socket);
            _connectEventArgsPool.Return(connectArgs);
        }
    }

    private void ConnectCompleted(object? sender, SocketAsyncEventArgs e)
    {
        this.ReportAsyncIO();
        this.ConnectCompleted((NetEventArgs)e);
    }

    private void ConnectCompleted(NetEventArgs e)
    {
        if (e.ConnectSocket == null)
        {
            Log.Error($"{nameof(this.ConnectCompleted)}: ConnectSocket is NULL.");

            _connectEventArgsPool.Return(e);
            return;
        }

        if (e.SocketError != SocketError.Success)
        {
            Log.Error($"{nameof(this.ConnectCompleted)}: {e.SocketError}");

            _socketPool.Return(e.ConnectSocket);
            _connectEventArgsPool.Return(e);
            return;
        }

        this.OnConnected(e.ConnectSocket);
        _connectEventArgsPool.Return(e);
    }

    protected virtual void OnConnected(Socket socket)
    {
        Log.Debug($"Connected to {socket.RemoteEndPoint}!");

        _connected?.Invoke(socket);
    }
}