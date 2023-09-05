using RB.Core.Net.Common;


using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace RB.Core.Net.Network;

public class SessionManager : ISessionManager
{
    private readonly IDGenerator32 _generator;
    private readonly ConcurrentDictionary<int, Session> _sessions;

    public SessionManager(IDGenerator32 generator)
    {
        _generator = generator;
        _sessions = new ConcurrentDictionary<int, Session>();
    }

    public Session CreateSession(Socket socket, IProtocol protocol)
    {
        var id = _generator.Next();
        var session = new Session(id, socket, protocol);
        _sessions[id] = session;
        return session;
    }

    public bool TryFindSessionById(int id, [MaybeNullWhen(false)] out Session session) => _sessions.TryGetValue(id, out session);

    public bool TryRemoveSessionById(int id) => _sessions.TryRemove(id, out _);

    public IEnumerator<Session> GetEnumerator() => _sessions.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _sessions.Values.GetEnumerator();

    public int Count => _sessions.Count;
}