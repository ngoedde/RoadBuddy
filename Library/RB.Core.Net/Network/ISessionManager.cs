using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace RB.Core.Net.Network;

public interface ISessionManager : IReadOnlyCollection<Session>
{
    Session CreateSession(Socket socket, IProtocol protocol);

    bool TryFindSessionById(int id, [MaybeNullWhen(false)] out Session session);

    bool TryRemoveSessionById(int id);
}