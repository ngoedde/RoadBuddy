namespace RB.Core.Net.Common.Messaging;

public class MessageDescription
{
    public MessageID ID { get; set; }
    public string? Name { get; set; }
    public MessageDirection Direction { get; set; }
    public MessageOptions Options { get; set; }
}