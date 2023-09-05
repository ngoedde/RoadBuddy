using System.Runtime.CompilerServices;

namespace RB.Core.Net.Common.Messaging.Allocation;

public interface IMessageAllocator
{
    Message NewMsg([CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1);

    Message NewMsg(MessageID id, int receiverId = -1, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1);

    Message NewLocalMsg(MessageID id, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1);
}