using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

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

    public bool TryAdd(T item) => _map.TryAdd(item, null!);

    public void Clear() => _map.Clear();

    public bool Contains(T item) => _map.ContainsKey(item);

    public bool TryRemove(T item) => _map.TryRemove(item, out _);

    public IEnumerator<T> GetEnumerator() => _map.Keys.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _map.Keys.GetEnumerator();
}