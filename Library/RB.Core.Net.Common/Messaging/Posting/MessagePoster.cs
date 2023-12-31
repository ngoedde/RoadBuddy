﻿using RB.Core.Net.Common.Messaging.Pumping;
using Serilog;

namespace RB.Core.Net.Common.Messaging.Posting;

public class MessagePoster : IMessagePoster
{
    private readonly IMessagePump _msgPump;

    public MessagePoster(IMessagePump msgPump)
    {
        _msgPump = msgPump;
    }

    public bool PostMsg(Message msg)
    {
        if (msg.ID == MessageID.Empty)
        {
            Log.Warning($"{nameof(PostMsg)}: Invalid ID");
            return false;
        }

        const int MSG_TARGET_INVALID = -1;
        if (msg.ReceiverID == MSG_TARGET_INVALID)
        {
            Log.Warning($"{nameof(PostMsg)}: Invalid ReceiverID");
            return false;
        }

        if (msg.SenderID == MSG_TARGET_INVALID)
        {
            Log.Warning($"{nameof(PostMsg)}: Invalid SenderID");
            return false;
        }

        msg.Retain();
        _msgPump.Enqueue(msg);
        return true;
    }
}