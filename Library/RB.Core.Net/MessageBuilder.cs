using RB.Core.Net.Common;
using RB.Core.Net.Common.Messaging;
using RB.Core.Net.Common.Messaging.Allocation;
using RB.Core.Net.Common.Protocol;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RB.Core.Net;

internal class MessageBuilder : IMessageBuilder
{
    private readonly IMessageAllocator _allocator;
    private readonly IMessageCryptoContext _context;

    private readonly Queue<Message> _completed = new Queue<Message>();
    private readonly Memory<byte> _msgMetaBuffer = new byte[Unsafe.SizeOf<MessageMeta>()];
    private int _msgMetaOffset;

    private Message? _msg;
    private int _msgSize;
    private int _msgOffset;

    public MessageBuilder(IMessageAllocator allocator, IMessageCryptoContext context)
    {
        _allocator = allocator;
        _context = context;
    }

    public bool Build(Span<byte> segment)
    {
        while (segment.Length > 0)
        {
            if (_msg == null)
            {
                // copy meta bytes advance
                var metaBytesToCopy = Math.Min(Unsafe.SizeOf<MessageMeta>() - _msgMetaOffset, segment.Length);
                segment.Slice(0, metaBytesToCopy).CopyTo(_msgMetaBuffer.Span.Slice(_msgMetaOffset));
                _msgMetaOffset += metaBytesToCopy;

                if (_msgMetaOffset < Unsafe.SizeOf<MessageMeta>())
                    break; // we need more data...

                var meta = MemoryMarshal.Read<MessageMeta>(_msgMetaBuffer.Span);
                Debug.Assert(_msgMetaOffset == Unsafe.SizeOf<MessageMeta>());

                // calculate raw message size
                if (meta.Encrypted && (_context.Options & ProtocolOptions.Encryption) != 0)
                    _msgSize = Blowfish.GetOutputLength(meta.DataSize + Message.HEADER_ENC_SIZE) + Message.HEADER_ENC_OFFSET;
                else
                    _msgSize = meta.DataSize + Message.HEADER_SIZE;

                _msg = _allocator.NewMsg();
                _msg.Meta = meta;
                _msg.WritePosition = (ushort)(meta.DataSize + Message.HEADER_SIZE);
                _msg.ReadPosition = 6;

                _msgOffset = 0;
                _msgMetaOffset = 0;
            }

            // copy message bytes and advance
            var numberOfBytesToCopy = Math.Min(_msgSize - _msgOffset, segment.Length);
            segment.Slice(0, numberOfBytesToCopy).CopyTo(_msg.GetSpan(_msgOffset));
            _msgOffset += numberOfBytesToCopy;
            segment = segment.Slice(numberOfBytesToCopy);

            // post message if complete
            if (_msgOffset == _msgSize)
            {
                _completed.Enqueue(_msg);
                _msg = null;
            }
        }
        return true;
    }

    public bool TryGet([MaybeNullWhen(false)] out Message msg) => _completed.TryDequeue(out msg);
}