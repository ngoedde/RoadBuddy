namespace RB.Core.Net.Common.Router;

public interface INetMsgRouter
{
    bool PostLocalNetConnected(int id);
    bool PostLocalNetKeyExchanged(int id);
    bool PostLocalNetDisconnected(int id, DisconnectReason reason);
}