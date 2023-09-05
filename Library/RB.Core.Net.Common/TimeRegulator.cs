namespace RB.Core.Net.Common;

public class TimeRegulator
{
    private bool _isActive;
    private float _accumulatedTime;
    private readonly float _updateTime;

    public bool IsActive => _isActive;

    public TimeRegulator(float updateTime)
    {
        _isActive = true;
        _updateTime = updateTime;
    }

    public void Start()
    {
        _isActive = true;
        _accumulatedTime = 0.0f;
    }

    public void Stop()
    {
        _isActive = false;
    }

    public void Reset()
    {
        _accumulatedTime = 0.0f;
    }

    public bool IsReady(float deltaTime)
    {
        if (!_isActive)
            return false;

        _accumulatedTime += deltaTime;
        if (_accumulatedTime < _updateTime)
            return false;

        _accumulatedTime = 0.0f;
        return true;
    }
}