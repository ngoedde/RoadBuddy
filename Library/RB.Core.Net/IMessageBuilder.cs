using System.Diagnostics.CodeAnalysis;
using RB.Core.Net.Common.Messaging;

namespace RB.Core.Net;

public interface IMessageBuilder
{
    bool Build(Span<byte> segment);

    bool TryGet([MaybeNullWhen(false)] out Message msg);
}