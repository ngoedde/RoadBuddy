using System.Runtime.InteropServices;
using RB.Core.Net.Common.Memory;

namespace RB.Core.Net.Common.Messaging.Memory;

public interface IMessagePool
{
    public Message Rent();

    void Return(Message message);
}

public class MessagePool : CustomObjectPool<Message>, IMessagePool
{
    public override Message Create()
    {
        var pinnedArray = GC.AllocateArray<byte>(Message.BUFFER_SIZE, pinned: true);
        var pinnedMemory = MemoryMarshal.CreateFromPinnedArray(pinnedArray, 0, pinnedArray.Length);
        return new Message(this, pinnedMemory);
        //return new Message(pinnedMemory);
    }

    public override Message Rent()
    {
        var result = base.Rent();
        lock (result._lock)
        {
            if (result.IsRented)
                throw new Exception("Trying to rent already rented.");
            result.IsRented = true;
        }
        return result;
    }

    public override void Return(Message item)
    {
        lock (item._lock)
        {
            if (!item.IsRented)
                throw new Exception("Trying to free already freed.");
            item.IsRented = false;
        }

        item.Reset();
        base.Return(item);
    }
}

//public class MessagePool2 : DefaultObjectPool<Message>
//{
//    public readonly static MessagePool2 Shared = new MessagePool2(new PooledMessageObjectPolicy(), 1024);

//    private class PooledMessageObjectPolicy : IPooledObjectPolicy<Message>
//    {
//        public Message Create()
//        {
//            var pinnedArray = GC.AllocateArray<byte>(Message.BUFFER_SIZE, pinned: true);
//            var pinnedMemory = MemoryMarshal.CreateFromPinnedArray(pinnedArray, 0, pinnedArray.Length);
//            return new Message(null, pinnedMemory);
//        }

//        public bool Return(Message obj)
//        {
//            obj.Reset();
//            return true;
//        }
//    }

//    public MessagePool2(IPooledObjectPolicy<Message> policy, int maximumRetained) : base(policy, maximumRetained)
//    {
//    }
//}