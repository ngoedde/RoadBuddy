using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using RB.Core.Net.Common.Messaging.Serialization;
using RB.Core.Net.Common.Messaging.Serialization.Collection;

namespace RB.Core.Net.Common.Messaging;

public interface IMessageReader
{
    bool TryRead<T>(out T value, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1) where T : unmanaged;

    bool TryRead<T>(Span<T> values, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1) where T : unmanaged;

    bool TryRead(out DateTime value, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1);

    bool TryRead([NotNullWhen(true)] out string value, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1);

    bool TryRead([NotNullWhen(true)] out string value, Encoding encoding, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1);

    bool TryRead([NotNullWhen(true)] out string value, int length, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1);

    bool TryRead([NotNullWhen(true)] out string value, int length, Encoding encoding, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1);

    bool TryDeserialize<T>([NotNullWhen(true)] out T deserializeable, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1) where T : IMessageDeserializer, new();

    bool TryDeserialize<T>(ICollection<T> collection, IMessageCollectionDeserializer collectionDeserializer, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1) where T : IMessageDeserializer, new();
}