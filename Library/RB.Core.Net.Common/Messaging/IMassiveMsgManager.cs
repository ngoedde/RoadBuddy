
using System.Diagnostics.CodeAnalysis;

namespace RB.Core.Net.Common.Messaging;

public interface IMassiveMsgManager
{
    public bool TryAddMassiveMsg(int sessionId, MassiveMsg massiveMsg);

    bool TryFindAllocatedMassiveMsg(int sessionId, [MaybeNullWhen(false)] out MassiveMsg massivMsg);

    bool DeleteAllocatedMassiveMsg(int sessionId);

    bool DeleteAllocatedMassiveMsg(int sessionId, [MaybeNullWhen(false)] out MassiveMsg massiveMsg);
}