﻿namespace RB.Core.Net.Common.Messaging.Posting;

public interface IMassiveMsgPoster
{
    public bool SendMsg(int receiverID, MassiveMsg msg)
    {
        msg.ReceiverID = receiverID;
        return PostMsg(msg);
    }

    bool PostMsg(MassiveMsg msg);
}