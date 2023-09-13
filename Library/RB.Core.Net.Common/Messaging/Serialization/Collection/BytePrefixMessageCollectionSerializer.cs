namespace RB.Core.Net.Common.Messaging.Serialization.Collection;

internal class BytePrefixMessageCollectionSerializer : IMessageCollectionSerializable
{
    public bool Deserialize<T>(IMessageReader reader, ICollection<T> collection) where T : IMessageDeserializer, new()
    {
        if (!reader.TryRead(out byte value)) return false;
        for (byte i = 0; i < value; i++)
        {
            if (!reader.TryDeserialize(out T item)) return false;
            collection.Add(item);
        }

        return true;
    }

    public bool Serialize<T>(IMessageWriter writer, IReadOnlyCollection<T> collection) where T : IMessageSerializer
    {
        if (!writer.TryWrite((byte)collection.Count)) return false;
        foreach (var item in collection)
            if (!writer.TrySerialize(item))
                return false;
        return true;
    }
}