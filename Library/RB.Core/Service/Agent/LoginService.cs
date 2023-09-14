using RB.Core.Net.Common.Messaging;
using RB.Core.Network;
using RB.Core.Network.Agent;
using Serilog;

namespace RB.Core.Service.Agent;

public sealed class LoginService
{
    public delegate void OnLoginErrorEventHandler(byte code);
    public delegate void OnLoginSuccessEventHandler();

    private readonly ServerEngine _agentClient;

    public LoginService(ServerEngine agentClient)
    {
        _agentClient = agentClient;

        _agentClient.SetMsgHandler(AgentMsgId.LoginAck, OnLoginAck);
    }

    public event OnLoginSuccessEventHandler? LoginSuccess;
    public event OnLoginErrorEventHandler? LoginError;

    public bool RequestLogin(uint token, string username, string password, byte contentId)
    {
        var loginReq = _agentClient.NewMsg(AgentMsgId.LoginReq, _agentClient.ServerId);

        var macAddress = new Span<byte>(new byte[6]);

        if (!loginReq.TryWrite(token)) return false;
        if (!loginReq.TryWrite(username)) return false;
        if (!loginReq.TryWrite(password)) return false;
        if (!loginReq.TryWrite(contentId)) return false;
        if (!loginReq.TryWrite(macAddress)) return false; //MacAddress

        return _agentClient.PostMsg(loginReq);
    }

    private bool OnLoginAck(Message msg)
    {
        if (!msg.TryRead(out MsgResult msgResult)) return false;

        if (msgResult == MsgResult.Success)
        {
            Log.Information("Successfully authenticated to agent server!");

            OnLoginSuccess();
        }
        else
        {
            if (!msg.TryRead(out byte errorCode)) ;

            Log.Error($"Authentication to agent server failed: [0x{errorCode:x2}]");
            OnLoginError(errorCode);
        }

        return true;
    }

    private void OnLoginError(byte code)
    {
        LoginError?.Invoke(code);
    }

    private void OnLoginSuccess()
    {
        LoginSuccess?.Invoke();
    }
}