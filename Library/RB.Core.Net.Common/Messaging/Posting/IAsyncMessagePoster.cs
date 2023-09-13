namespace RB.Core.Net.Common.Messaging.Posting;

public interface IAsyncMessagePoster
{
    public ValueTask<bool> SendMsgAsync(int receiverID, Message msg)
    {
        msg.ReceiverID = receiverID;
        return PostMsgAsync(msg);
    }

    public ValueTask<bool> PostMsgAsync(Message msg, CancellationToken cancellationToken = default);
}