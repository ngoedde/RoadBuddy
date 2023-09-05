using RB.Core.Net.Common.Messaging;

using System;
using System.Diagnostics.CodeAnalysis;

namespace RB.Core.Net;

public interface IMessageBuilder
{
    bool Build(Span<byte> segment);

    bool TryGet([MaybeNullWhen(false)] out Message msg);
}