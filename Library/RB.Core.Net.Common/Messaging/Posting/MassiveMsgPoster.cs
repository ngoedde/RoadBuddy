namespace RB.Core.Net.Common.Messaging.Posting;

public class MassiveMsgPoster : IMassiveMsgPoster
{
    private readonly IMessagePoster _msgPoster;

    public MassiveMsgPoster(IMessagePoster msgPoster)
    {
        _msgPoster = msgPoster;
    }

    public bool PostMsg(MassiveMsg msg)
    {
        using (var headerMsg = msg.AllocateHeaderMsg())
        {
            if (!_msgPoster.PostMsg(headerMsg))
                return false;
        }

        foreach (var dataMsg in msg)
        {
            dataMsg.SenderID = msg.SenderID;
            dataMsg.ReceiverID = msg.ReceiverID;
            if (!_msgPoster.PostMsg(dataMsg))
                return false;
        }
        return true;
    }
}