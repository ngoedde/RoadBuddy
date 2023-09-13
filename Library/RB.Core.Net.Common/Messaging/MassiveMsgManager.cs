using System.Diagnostics.CodeAnalysis;

namespace RB.Core.Net.Common.Messaging;

public class MassiveMsgManager : IMassiveMsgManager
{
    private readonly Dictionary<int, MassiveMsg> _massiveMsg = new(1024);

    public bool TryFindAllocatedMassiveMsg(int sessionId, [MaybeNullWhen(false)] out MassiveMsg massivMsg)
    {
        return _massiveMsg.TryGetValue(sessionId, out massivMsg);
    }

    public bool TryAddMassiveMsg(int sessionId, MassiveMsg massiveMsg)
    {
        return _massiveMsg.TryAdd(sessionId, massiveMsg);
    }

    public bool DeleteAllocatedMassiveMsg(int sessionId)
    {
        return _massiveMsg.Remove(sessionId);
    }

    public bool DeleteAllocatedMassiveMsg(int sessionId, [MaybeNullWhen(false)] out MassiveMsg massiveMsg)
    {
        return _massiveMsg.Remove(sessionId, out massiveMsg);
    }
}