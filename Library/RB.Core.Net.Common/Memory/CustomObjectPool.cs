using System.Collections.Concurrent;

namespace RB.Core.Net.Common.Memory;

public abstract class CustomObjectPool<T> : ICustomObjectPool<T>
    where T : notnull
{
    private readonly ConcurrentQueue<T> _objects;

    protected CustomObjectPool()
    {
        // TODO: Object limitations
        // TODO: Growth and shrinking
        // TODO: Ability to dispose objects with IDisposable if necessary
        _objects = new ConcurrentQueue<T>();
    }

    public int Count => _objects.Count;

    public void Allocate(int size)
    {
        for (var i = 0; i < size; i++)
            _objects.Enqueue(Create());
    }

    public abstract T Create();

    public virtual void Clear(T item)
    {
    }

    public virtual T Rent()
    {
        if (_objects.TryDequeue(out var item))
            return item;

        return Create();
    }

    public virtual void Return(T item)
    {
        Clear(item);
        _objects.Enqueue(item);
    }

    public virtual void Destroy(T item)
    {
    }
}