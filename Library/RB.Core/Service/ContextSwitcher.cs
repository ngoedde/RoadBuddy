using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Network;
using RB.Core.Network.Agent;
using RB.Core.Network.Exception;
using RB.Core.Network.Gateway;
using RB.Core.Service.Agent;
using Serilog;

namespace RB.Core.Service;

/// <summary>
///     This class is used to switch the context between Gateway and Agent servers.
/// </summary>
public class ContextSwitcher
{
    private readonly IAgentClient _agentClient;
    private readonly LoginService _agentLoginService;
    private readonly IGatewayClient _gatewayClient;

    private AgentSwitchContext? _agentSwitchContext;
    private GatewaySwitchContext? _gatewaySwitchContext;

    public ContextSwitcher(
        IGatewayClient gatewayClient,
        IAgentClient agentClient,
        LoginService agentLoginService,
        Gateway.LoginService gatewayLoginService
    )
    {
        _gatewayClient = gatewayClient;
        _agentClient = agentClient;
        _agentLoginService = agentLoginService;

        gatewayLoginService.LoginSuccess += OnGatewayLoginSuccess;
        agentClient.Disconnected += AgentClientDisconnected;
        agentClient.SetMsgHandler(NetMsgId.SetupCordNoDir, OnSetupAgentServerCord);
        gatewayClient.SetMsgHandler(GatewayMsgId.LoginReq, OnGatewayLoginReq);
    }

    private void AgentClientDisconnected()
    {
        if (_gatewaySwitchContext != null)
            SwitchContextToGateway();
    }

    public void SetAgentContext(string username, string password, byte contentId)
    {
        _agentSwitchContext = new AgentSwitchContext(username, password, contentId);
    }

    public void SetGatewayContext(string gatewayIp, ushort gatewayPort)
    {
        _gatewaySwitchContext = new GatewaySwitchContext(gatewayIp, gatewayPort);
    }

    public void SetAgentContextEndPoint(string agentIp, ushort agentPort, uint token)
    {
        if (_agentSwitchContext == null)
        {
            Log.Error("Can not set agent context token, the context switch is not initialized.");

            return;
        }

        _agentSwitchContext.Ip = agentIp;
        _agentSwitchContext.Port = agentPort;
        _agentSwitchContext.Token = token;
    }

    private void SwitchContextToAgent()
    {
        Log.Information("Switching context to agent server...");

        if (_agentSwitchContext == null)
        {
            Log.Error("Can not switch to agent: No Context information set.");

            return;
        }

        Log.Information(
            $"Connecting to agent server [{_agentSwitchContext.Ip}:{_agentSwitchContext.Port}] (Token: 0x{_agentSwitchContext.Token:X8})");

        _gatewayClient.Disconnect();
        _agentClient.Connect(NetHelper.ToIPEndPoint(_agentSwitchContext.Ip, _agentSwitchContext.Port));
    }

    public void SwitchContextToAgent(string agentIp, ushort agentPort, uint token, string username, string password,
        byte contentId)
    {
        _agentSwitchContext = new AgentSwitchContext(username, password, contentId)
        {
            Ip = agentIp,
            Port = agentPort,
            Token = token
        };

        SwitchContextToAgent();
    }

    private void SwitchContextToGateway()
    {
        Log.Information("Switching context to gateway server...");

        if (_gatewaySwitchContext == null)
        {
            Log.Error("Can not switch to gateway: No Context information set.");

            return;
        }

        _agentClient.Disconnect();
        _gatewayClient.Connect(NetHelper.ToIPEndPoint(_gatewaySwitchContext.Ip, _gatewaySwitchContext.Port));
    }

    public void SwitchContextToGateway(string gatewayIp, ushort gatewayPort)
    {
        SetGatewayContext(gatewayIp, gatewayPort);
        SwitchContextToGateway();
    }

    private bool OnSetupAgentServerCord(Message msg)
    {
        if (msg.SenderID == msg.ReceiverID)
            return true;

        if (!msg.TryRead(out string identityName))
            return false;

        if (identityName != NetIdentity.AgentServer)
            throw new InvalidIdentityException(NetIdentity.AgentServer, identityName);

        if (_agentSwitchContext == null)
            return false;

        _agentLoginService.RequestLogin(
            _agentSwitchContext.Token,
            _agentSwitchContext.Username,
            _agentSwitchContext.Password,
            _agentSwitchContext.ContentId
        );

        return true;
    }

    private bool OnGatewayLoginReq(Message msg)
    {
        if (!msg.TryRead(out byte contentId)) return false;
        if (!msg.TryRead(out string username)) return false;
        if (!msg.TryRead(out string password)) return false;

        SetAgentContext(username, password, contentId);

        return true;
    }

    private void OnGatewayLoginSuccess(string agentIp, ushort agentPort, uint token)
    {
        SetAgentContextEndPoint(agentIp, agentPort, token);

        SwitchContextToAgent();
    }

    internal class AgentSwitchContext
    {
        public byte ContentId;
        public string Ip = string.Empty;
        public string Password;
        public ushort Port;
        public uint Token = uint.MinValue;
        public string Username;

        public AgentSwitchContext(string username, string password, byte contentId)
        {
            Username = username;
            Password = password;
            ContentId = contentId;
        }
    }

    internal class GatewaySwitchContext
    {
        public string Ip;
        public ushort Port;

        public GatewaySwitchContext(string ip, ushort port)
        {
            Ip = ip;
            Port = port;
        }
    }
}