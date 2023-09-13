namespace RB.Core.Net.Common;

public class TickRegulator
{
    private readonly float _updateTime;
    private float _accumulatedTime;

    public TickRegulator(float updateTime)
    {
        IsActive = true;
        _updateTime = updateTime;
    }

    public bool IsActive { get; private set; }

    public void Start()
    {
        IsActive = true;
        _accumulatedTime = 0.0f;
    }

    public void Stop()
    {
        IsActive = false;
    }

    public void Reset()
    {
        _accumulatedTime = 0.0f;
    }

    public void Update(float deltaTime)
    {
        _accumulatedTime += deltaTime;
    }

    public bool IsReady()
    {
        if (!IsActive)
            return false;

        if (_accumulatedTime >= _updateTime)
        {
            _accumulatedTime -= _updateTime;
            return true;
        }

        return false;
    }

    public bool IsReady(float deltaTime, out int dueCount)
    {
        if (!IsActive)
        {
            dueCount = 0;
            return false;
        }

        _accumulatedTime += deltaTime;
        if (_accumulatedTime < _updateTime)
        {
            dueCount = 0;
            return false;
        }

        dueCount = (int)(_accumulatedTime / _updateTime);
        _accumulatedTime %= _updateTime;
        return true;
    }
}