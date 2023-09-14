using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Network;
using RB.Core.Network.Gateway;
using RB.Core.Service.Agent;
using Serilog;

namespace RB.Core.Service;

/// <summary>
///     This class is used to switch the context between Gateway and Agent servers.
/// </summary>
public class ContextSwitcher
{
    private readonly ServerEngine _server;
    private readonly LoginService _agentLoginService;

    private AgentSwitchContext? _agentSwitchContext;
    private GatewaySwitchContext? _gatewaySwitchContext;

    public ContextSwitcher(
        ServerEngine server,
        LoginService agentLoginService,
        Gateway.LoginService gatewayLoginService
    )
    {
        _server = server;
        _server.Disconnected += ServerDisconnected;
        _server.ContextCreated += ServerOnContextCreated;
        
        _agentLoginService = agentLoginService;

        gatewayLoginService.LoginSuccess += OnGatewayLoginSuccess;

        _server.SetMsgHandler(GatewayMsgId.LoginReq, OnGatewayLoginReq);
    }

    private void ServerOnContextCreated(ServerContext oldContext, ServerContext newContext)
    {
        if (newContext == ServerContext.Agent && _agentSwitchContext != null)
        {
            _agentLoginService.RequestLogin(
                _agentSwitchContext.Token,
                _agentSwitchContext.Username,
                _agentSwitchContext.Password,
                _agentSwitchContext.ContentId
            );
        }
    }

    private void ServerDisconnected()
    {
        if (_server.Context == ServerContext.Agent)
            SwitchContextToGateway();
    }

    public void SetAgentContext(string username, string password, byte contentId)
    {
        _agentSwitchContext = new AgentSwitchContext(username, password, contentId);
    }

    public void SetAgentContextEndPoint(string agentIp, ushort agentPort, uint token)
    {
        if (_agentSwitchContext == null)
        {
            Log.Error("Can not set agent context token, the context switch is not initialized.");

            return;
        }

        _agentSwitchContext.HostOrIp = agentIp;
        _agentSwitchContext.Port = agentPort;
        _agentSwitchContext.Token = token;
    }

    private void SwitchContextToAgent()
    {
        if (_agentSwitchContext == null)
        {
            Log.Error("Can not switch to agent: No Context information set.");

            return;
        }
        
        Log.Information("Switching context to agent server...");
        Log.Information(
            $"Connecting to agent server [{_agentSwitchContext.HostOrIp}:{_agentSwitchContext.Port}] (Token: 0x{_agentSwitchContext.Token:X8})");

        if (_server.Context == ServerContext.Gateway) {
            Log.Debug("Disconnecting from gateway...");
            
            _server.Disconnect();
        }
        
        _server.Connect(NetHelper.ToIPEndPoint(_agentSwitchContext.HostOrIp, _agentSwitchContext.Port));
    }

    public void SwitchContextToGateway(string hostOrIp, ushort port)
    {
        _gatewaySwitchContext = new GatewaySwitchContext(hostOrIp, port);
        
        SwitchContextToGateway();
    }
    
    private void SwitchContextToGateway()
    {
        Log.Information("Switching context to gateway server...");

        if (_gatewaySwitchContext == null)
        {
            Log.Error("Can not switch to gateway: No Context information set.");

            return;
        }

        _server.Connect(NetHelper.ToIPEndPoint(_gatewaySwitchContext.HostOrIp, _gatewaySwitchContext.Port));
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
        public string HostOrIp = string.Empty;
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
        public string HostOrIp;
        public ushort Port;

        public GatewaySwitchContext(string hostOrIp, ushort port)
        {
            HostOrIp = hostOrIp;
            Port = port;
        }
    }
}