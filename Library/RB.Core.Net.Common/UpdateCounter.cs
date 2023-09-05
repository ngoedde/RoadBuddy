namespace RB.Core.Net.Common;

public class UpdateCounter
{
    private readonly TimeRegulator _timeRegulator = new TimeRegulator(1.0f);

    private int _updateAccumulator;
    private int _latestUpdateCount;

    public int Update(float deltaTime)
    {
        _updateAccumulator++;
        if (_timeRegulator.IsReady(deltaTime))
        {
            _latestUpdateCount = _updateAccumulator;
            _updateAccumulator = 0;
        }
        return _latestUpdateCount;
    }
}