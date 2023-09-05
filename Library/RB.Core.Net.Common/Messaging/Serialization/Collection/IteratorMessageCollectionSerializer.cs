
using System.Collections.Generic;
using System.Diagnostics;

namespace RB.Core.Net.Common.Messaging.Serialization.Collection;

internal class IteratorMessageCollectionSerializer : IMessageCollectionSerializable
{
    public bool Serialize<T>(IMessageWriter writer, IReadOnlyCollection<T> collection)
        where T : IMessageSerializer
    {
        const byte LIST_BEGIN = 0;
        const byte LIST_NEXT = 1;
        const byte LIST_END = 2;

        if (!writer.TryWrite(LIST_BEGIN)) return false;
        foreach (var item in collection)
        {
            if (!writer.TryWrite(LIST_NEXT)) return false;
            if (!writer.TrySerialize(item)) return false;
        }
        return writer.TryWrite(LIST_END);
    }

    public bool Deserialize<T>(IMessageReader reader, ICollection<T> collection)
        where T : IMessageDeserializer, new()
    {
        const byte ITERATOR_BEGIN = 0;
        const byte ITERATOR_NEXT = 1;
        const byte ITERATOR_END = 2;

        if (!reader.TryRead(out byte iterator)) return false;
        if (iterator != ITERATOR_BEGIN) return false;
        while (true)
        {
            if (!reader.TryRead(out iterator)) return false;
            if (iterator == ITERATOR_END)
                break;

            Debug.Assert(iterator == ITERATOR_NEXT, $"Unexpected ITERATOR_NEXT value. (Expected: {ITERATOR_NEXT}; Actual: {iterator}");

            if (!reader.TryDeserialize(out T item)) return false;
            collection.Add(item);
        }

        return true;
    }
}