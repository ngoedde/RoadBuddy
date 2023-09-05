using System;
using System.Text;

namespace RB.Core.Net.Common.Messaging;

public interface IMessage : IDisposable
{
    public MessageID ID { get; }

    // -----------------------------------

    bool TryRead<T>(out T value) where T : unmanaged;

    bool TryRead<T>(Span<T> values) where T : unmanaged;

    bool TryRead(out string value, int length, Encoding encoding);

    // -----------------------------------

    bool TryWrite<T>(ref T value) where T : unmanaged;

    bool TryWrite<T>(ReadOnlySpan<T> values) where T : unmanaged;

    bool TryWrite(string value, int length, Encoding encoding);
}