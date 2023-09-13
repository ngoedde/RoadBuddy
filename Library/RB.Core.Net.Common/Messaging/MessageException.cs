using System.Runtime.Serialization;

namespace RB.Core.Net.Common.Messaging;

[Serializable]
internal class MessageException : Exception
{
    public MessageException()
    {
    }

    public MessageException(string message) : base(message)
    {
    }

    public MessageException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected MessageException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}