using System.Collections.Generic;

namespace RB.Core.Net.Common.Messaging.Serialization.Collection;

public interface IMessageCollectionSerializer
{
    bool Serialize<T>(IMessageWriter writer, IReadOnlyCollection<T> collection)
        where T : IMessageSerializer;
}