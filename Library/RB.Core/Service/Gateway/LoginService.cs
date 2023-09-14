using RB.Core.Net.Common.Messaging;
using RB.Core.Network;
using RB.Core.Network.Gateway;
using RB.Game.Client.Service;
using Serilog;

namespace RB.Core.Service.Gateway;

public sealed class LoginService
{
    public delegate void OnLoginErrorEventHandler(byte errorCode);

    public delegate void OnLoginSuccessEventHandler(string agentIp, ushort agentPort, uint token);

    private readonly IDivisionInfoService _divisionInfoService;
    private readonly ServerEngine _gatewayClient;

    public LoginService(
        ServerEngine gatewayClient,
        IDivisionInfoService divisionInfoService
    )
    {
        _gatewayClient = gatewayClient;
        _divisionInfoService = divisionInfoService;
        _gatewayClient.SetMsgHandler(GatewayMsgId.LoginAck, OnLoginAck);
    }

    public event OnLoginSuccessEventHandler? LoginSuccess;
    public event OnLoginErrorEventHandler? LoginError;
    
    public bool RequestLogin(string username, string password, ushort shardId)
    {
        var loginReq = _gatewayClient.NewMsg(GatewayMsgId.LoginReq, _gatewayClient.ServerId);

        var contentId = _divisionInfoService.GetDivisionInfo().ContentId;
        loginReq.TryWrite(contentId);
        loginReq.TryWrite(username);
        loginReq.TryWrite(password);
        loginReq.TryWrite(shardId);

        Log.Information($"Sending login request to shard [{shardId}]");

        return _gatewayClient.PostMsg(loginReq);
    }
    
    private bool OnLoginAck(Message msg)
    {
        return ReadLoginResponse(msg);
    }

    private bool ReadLoginResponse(Message msg)
    {
        if (!msg.TryRead(out MsgResult msgResult)) return false;

        if (msgResult == MsgResult.Success)
        {
            if (!msg.TryRead(out uint agentToken)) return false;
            if (!msg.TryRead(out string agentIp)) return false;
            if (!msg.TryRead(out ushort agentPort)) return false;

            OnLoginSuccess(agentIp, agentPort, agentToken);
        }
        else if (msgResult == MsgResult.Error)
        {
            if (!msg.TryRead(out byte errorCode)) return false;

            OnLoginError(errorCode);

            switch (errorCode)
            {
                case 0x01:
                    if (!msg.TryRead(out uint maxAttempts)) return false;
                    if (!msg.TryRead(out uint curAttempts)) return false;

                    Log.Error($"Login failed: Username or password wrong. Failed {curAttempts}/{maxAttempts} times.");
                    break;
                case 0x02:
                    if (!msg.TryRead(out byte blockType)) return false;

                    Log.Error($"Login failed: Account is Blocked [{blockType}]");
                    break;

                case 0x03:
                    Log.Error("Login failed: The account is already logged in.");

                    break;
                case 0x04:
                    Log.Error("Login failed: The server is in check.");
                    break;

                case 0x05:
                    Log.Error("Login failed: The server is full.");
                    break;

                default:
                    Log.Error($"Login failed: Unknown login error [0x{errorCode:x2}]");
                    break;
            }
        }

        return true;
    }

    private void OnLoginSuccess(string agentIp, ushort agentPort, uint token)
    {
        Log.Information("Login success!");

        LoginSuccess?.Invoke(agentIp, agentPort, token);
    }

    private void OnLoginError(byte errorCode)
    {
        LoginError?.Invoke(errorCode);
    }
}