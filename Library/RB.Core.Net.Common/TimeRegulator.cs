namespace RB.Core.Net.Common;

public class TimeRegulator
{
    private readonly float _updateTime;
    private float _accumulatedTime;

    public TimeRegulator(float updateTime)
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

    public bool IsReady(float deltaTime)
    {
        if (!IsActive)
            return false;

        _accumulatedTime += deltaTime;
        if (_accumulatedTime < _updateTime)
            return false;

        _accumulatedTime = 0.0f;
        return true;
    }
}