namespace RB.Core.Net.Common.Messaging.Serialization.Collection;

public static class MessageCollectionSerializer
{
    public static readonly IMessageCollectionSerializable VerboseIterator =
        new VerboseIteratorMessageCollectionSerializer();

    public static readonly IMessageCollectionSerializable Iterator = new IteratorMessageCollectionSerializer();
    public static readonly IMessageCollectionSerializable BytePrefix = new BytePrefixMessageCollectionSerializer();
}