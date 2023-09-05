namespace RB.Core.Net.Common.Memory;

public interface ICustomObjectPool<T>
    where T : notnull
{
    int Count { get; }

    T Rent();

    void Return(T item);

    void Clear(T item);

    T Create();

    void Allocate(int size);
}