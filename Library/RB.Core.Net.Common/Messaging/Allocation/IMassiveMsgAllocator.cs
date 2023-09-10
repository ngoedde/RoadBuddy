using System.Runtime.CompilerServices;

namespace RB.Core.Net.Common.Messaging.Allocation;

public interface IMassiveMsgAllocator
{
    MassiveMsg NewMassiveMsg([CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1);

    MassiveMsg NewMassiveMsg(MessageID id, int receiverID = -1, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1);

    MassiveMsg NewLocalMassiveMsg(MessageID id, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1);
}