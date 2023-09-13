namespace RB.Core.Net.Common.Threading;

public class ReferenceCounter
{
    private int _state;

    public ReferenceCounter()
    {
        _state = 0;
    }

    public void Reset()
    {
        Interlocked.Exchange(ref _state, 0);
    }

    public void Retain()
    {
        Interlocked.Increment(ref _state);
    }


    public bool Release()
    {
        return Interlocked.Decrement(ref _state) < 0;
    }
}