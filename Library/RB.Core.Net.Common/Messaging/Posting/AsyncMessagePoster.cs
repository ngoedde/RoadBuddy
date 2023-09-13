using System.Diagnostics;
using System.Threading.Channels;
using RB.Core.Net.Common.Extensions;
using RB.Core.Net.Common.Messaging.Handling;
using Serilog;

namespace RB.Core.Net.Common.Messaging.Posting;

public class AsyncMessagePoster : IAsyncMessagePoster
{
    private readonly AsyncMsgHandler<Message> _handleMsg;
    private readonly int _id;
    private readonly ChannelWriter<Message> _localWriter;
    private readonly AsyncMsgHandler<Message> _sendMsg;

    public AsyncMessagePoster(int id, ChannelWriter<Message> localWriter, AsyncMsgHandler<Message> handleMsg,
        AsyncMsgHandler<Message> sendMsg)
    {
        _id = id;
        _localWriter = localWriter;
        _handleMsg = handleMsg;
        _sendMsg = sendMsg;
    }

    public ValueTask<bool> PostMsgAsync(Message msg, CancellationToken cancellationToken = default)
    {
        if (msg.ID == MessageID.Empty)
        {
            Log.Warning($"{nameof(PostMsgAsync)}: Invalid ID");
            return ValueTask.FromResult(false);
        }

        const int MSG_TARGET_INVALID = -1;
        if (msg.ReceiverID == MSG_TARGET_INVALID)
        {
            Log.Warning($"{nameof(PostMsgAsync)}: Invalid ReceiverID");
            return ValueTask.FromResult(false);
        }

        if (msg.SenderID == MSG_TARGET_INVALID)
        {
            Log.Warning($"{nameof(PostMsgAsync)}: Invalid SenderID");
            return ValueTask.FromResult(false);
        }

        // Local
        if (msg.ReceiverID == _id && msg.SenderID == _id)
        {
            msg.Retain();
            return _localWriter.TryWriteAsync(msg, cancellationToken);
        }

        if (msg.ReceiverID == _id && msg.SenderID != _id)
            return _handleMsg(msg, cancellationToken);
        if (msg.ReceiverID != _id && msg.SenderID == _id)
            return _sendMsg(msg, cancellationToken);

        return ValueTask.FromException<bool>(new UnreachableException());
    }
}