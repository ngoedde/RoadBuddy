using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace RB.Core.Net.Common.Profiling;

public sealed class NetTrafficProfiler
{
    private readonly ConcurrentDictionary<int, NetTrafficProfile> _profiles;

    public NetTrafficProfiler()
    {
        _profiles = new ConcurrentDictionary<int, NetTrafficProfile>();
    }

    public NetTrafficProfile AddProfile(int profileId)
    {
        return _profiles[profileId] = new NetTrafficProfile();
    }

    public bool TryRemoveProfile(int profileId)
    {
        return _profiles.TryRemove(profileId, out _);
    }

    public bool TryGetProfile(int profileId, [MaybeNullWhen(false)] out NetTrafficProfile profile)
    {
        return _profiles.TryGetValue(profileId, out profile);
    }

    public (int ReceivedSegments, int SentSegments, long ReceivedBytes, long SentBytes) GetTotalProfile()
    {
        var receivedSegments = 0;
        var sentSegments = 0;
        var receivedBytes = 0L;
        var sentBytes = 0L;

        foreach (var item in _profiles.Values)
        {
            receivedSegments += Volatile.Read(ref item.ReceivedSegements);
            sentSegments += Volatile.Read(ref item.SentSegments);
            receivedBytes += Volatile.Read(ref item.ReceivedBytes);
            sentBytes += Volatile.Read(ref item.SentBytes);

            item.Reset();
        }

        return (receivedSegments, sentSegments, receivedBytes, sentBytes);
    }
}