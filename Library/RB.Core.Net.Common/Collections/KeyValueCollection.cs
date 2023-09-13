using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace RB.Core.Net.Common.Collections;

public class KeyValueCollection<TKey, TValue> : ICollection<TValue>, IReadOnlyCollection<TValue>
    where TKey : notnull
    where TValue : IKeyValue<TKey>
{
    private readonly IDictionary<TKey, TValue> _inner;

    public KeyValueCollection() : this(0, null)
    {
    }

    public KeyValueCollection(int capcacity) : this(capcacity, null)
    {
    }

    public KeyValueCollection(int capacity, IEqualityComparer<TKey>? comparer)
    {
        _inner = new Dictionary<TKey, TValue>(capacity, comparer);
    }

    protected KeyValueCollection(IDictionary<TKey, TValue> dictionary)
    {
        _inner = dictionary;
    }

    public TValue this[TKey key]
    {
        get => _inner[key];
        set => _inner[key] = value;
    }

    public int Count => _inner.Count;

    public bool IsReadOnly => false;

    public void Add(TValue value)
    {
        _inner.Add(value.Key, value);
    }

    public void Clear()
    {
        _inner.Clear();
    }

    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
    {
        return _inner.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _inner.Values.GetEnumerator();
    }

    public bool Contains(TValue item)
    {
        return _inner.ContainsKey(item.Key);
    }

    public void CopyTo(TValue[] array, int arrayIndex)
    {
        _inner.Values.CopyTo(array, arrayIndex);
    }

    public bool Remove(TValue item)
    {
        return _inner.Remove(item.Key);
    }

    public void Remove(TKey key)
    {
        _inner.Remove(key);
    }

    public bool ContainsKey(TKey key)
    {
        return _inner.ContainsKey(key);
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return _inner.TryGetValue(key, out value);
    }

    public void AddRange(IEnumerable<TValue> values)
    {
        foreach (var item in values)
            _inner.Add(item.Key, item);
    }
}