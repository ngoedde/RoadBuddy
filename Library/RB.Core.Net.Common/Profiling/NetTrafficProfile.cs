namespace RB.Core.Net.Common.Profiling;

public class NetTrafficProfile
{
    public long ReceivedBytes;
    public int ReceivedSegements;
    public long SentBytes;
    public int SentSegments;

    public void ReportReceive(int value)
    {
        Volatile.Write(ref ReceivedSegements, ReceivedSegements + 1);
        Volatile.Write(ref ReceivedBytes, ReceivedBytes + value);
    }

    public void ReportSent(int value)
    {
        Volatile.Write(ref SentSegments, SentSegments + 1);
        Volatile.Write(ref SentBytes, SentBytes + value);
    }

    public void Reset()
    {
        Volatile.Write(ref ReceivedSegements, 0);
        Volatile.Write(ref ReceivedBytes, 0);

        Volatile.Write(ref SentSegments, 0);
        Volatile.Write(ref SentBytes, 0);
    }
}