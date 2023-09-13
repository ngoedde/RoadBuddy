namespace RB.Core.Net.Common.Messaging.Posting;

public interface IMessagePoster
{
    bool PostMsg(Message msg);

    public bool SendMsg(int receiverID, Message msg)
    {
        msg.ReceiverID = receiverID;
        return PostMsg(msg);
    }
}