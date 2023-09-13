namespace RB.Core.Net.Common;

public sealed class IDGenerator32
{
    private readonly int _initialValue;
    private int _value = -1;

    public IDGenerator32()
    {
    }

    public IDGenerator32(int initialValue = -1)
    {
        _value = _initialValue = initialValue;
    }

    public int Next()
    {
        return Interlocked.Increment(ref _value);
    }

    public void Reset()
    {
        Interlocked.Exchange(ref _value, _initialValue);
    }
}