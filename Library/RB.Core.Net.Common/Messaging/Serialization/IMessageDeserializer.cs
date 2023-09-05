namespace RB.Core.Net.Common.Messaging.Serialization;

public interface IMessageDeserializer
{
    bool TryDeserialize(IMessageReader reader);
}