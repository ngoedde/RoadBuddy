namespace RB.Core.Net.Common;

public class TickRegulator
{
    private bool _isActive;
    private float _accumulatedTime;
    private readonly float _updateTime;

    public bool IsActive => _isActive;

    public TickRegulator(float updateTime)
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

    public void Update(float deltaTime) => _accumulatedTime += deltaTime;

    public bool IsReady()
    {
        if (!_isActive)
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
        if (!_isActive)
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