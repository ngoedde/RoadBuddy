using System.Collections;
using System.Collections.Concurrent;

namespace RB.Core.Net.Common.Collections;

public class ConcurrentHashSet<T> : IEnumerable<T>
    where T : notnull
{
    private readonly ConcurrentDictionary<T, object> _map;

    public ConcurrentHashSet()
    {
        _map = new ConcurrentDictionary<T, object>();
    }

    public ConcurrentHashSet(int concurrencyLevel, int capacity)
    {
        _map = new ConcurrentDictionary<T, object>(concurrencyLevel, capacity);
    }

    public int Count => _map.Count;

    public bool IsEmpty => _map.IsEmpty;

    public IEnumerator<T> GetEnumerator()
    {
        return _map.Keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _map.Keys.GetEnumerator();
    }

    public bool TryAdd(T item)
    {
        return _map.TryAdd(item, null!);
    }

    public void Clear()
    {
        _map.Clear();
    }

    public bool Contains(T item)
    {
        return _map.ContainsKey(item);
    }

    public bool TryRemove(T item)
    {
        return _map.TryRemove(item, out _);
    }
}