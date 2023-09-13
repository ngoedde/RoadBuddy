namespace RB.Core.Net.Network;

internal class KeepAliveInfo : IKeepAliveInfo
{
    protected long _lastReportTime;

    public void ReportAlive()
    {
        _lastReportTime = Environment.TickCount64;
    }

    public bool IsAlive(int timeOut = 30000)
    {
        return Environment.TickCount64 - _lastReportTime < timeOut;
    }
}