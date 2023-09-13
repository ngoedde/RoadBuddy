using System.Diagnostics;

namespace RB.Core.Net.Common.Messaging.Serialization.Collection;

internal class VerboseIteratorMessageCollectionSerializer : IMessageCollectionSerializable
{
    public bool Serialize<T>(IMessageWriter writer, IReadOnlyCollection<T> collection)
        where T : IMessageSerializer
    {
        const byte ITERATOR_NEXT = 1;
        const byte ITERATOR_END = 0;

        foreach (var item in collection)
        {
            if (!writer.TryWrite(ITERATOR_NEXT)) return false;
            if (!writer.TrySerialize(item)) return false;
        }

        return writer.TryWrite(ITERATOR_END);
    }

    public bool Deserialize<T>(IMessageReader reader, ICollection<T> collection)
        where T : IMessageDeserializer, new()
    {
        const byte ITERATOR_NEXT = 1;
        const byte ITERATOR_END = 0;

        while (true)
        {
            if (!reader.TryRead(out byte iterator)) return false;
            if (iterator == ITERATOR_END)
                break;

            Debug.Assert(iterator == ITERATOR_NEXT,
                $"Unexpected ITERATOR_NEXT value. (Expected: {ITERATOR_NEXT}; Actual: {iterator}");

            if (!reader.TryDeserialize(out T item)) return false;
            collection.Add(item);
        }

        return true;
    }
}