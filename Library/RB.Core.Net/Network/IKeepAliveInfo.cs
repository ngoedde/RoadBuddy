namespace RB.Core.Net.Network;

public interface IKeepAliveInfo
{
    bool IsAlive(int timeOut = 30000);

    void ReportAlive();
}