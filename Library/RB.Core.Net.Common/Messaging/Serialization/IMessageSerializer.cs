namespace RB.Core.Net.Common.Messaging.Serialization;

public interface IMessageSerializer
{
    bool TrySerialize(IMessageWriter writer);
}