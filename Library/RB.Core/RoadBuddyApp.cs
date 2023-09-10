using RB.Core.Net.Common;

namespace RB.Core;

public abstract class RoadBuddyApp : IRoadBuddyApp
{
    private const float UsToMs = 0.0001f;
    private const int MsPerSecond = 1000;
    private const int TicksPerMs = 10000;

    private const int TARGET_FIXED = 32;
    private const int TARGET_VARIABLE = 20;

    private const long TARGET_FIXED_TIME = (long)((MsPerSecond * TimeSpan.TicksPerMillisecond) / (float)TARGET_FIXED);
    private const long TARGET_VARIABLE_TIME = (long)((MsPerSecond * TimeSpan.TicksPerMillisecond) / (float)TARGET_VARIABLE);

    private bool _disposed;
    private bool _exit;

    //private long _lastTime;

    private float _timeSinceStartup;

    private int _variableTicks;
    private int _fixedTicks;

    private string? _title;
    private UpdateCounter _fixedCounter = new UpdateCounter();
    private UpdateCounter _variableCounter = new UpdateCounter();
    private TickRegulator _fixedRegulator = new TickRegulator(1.0f / TARGET_FIXED);
    private int _lastSpinCount;
    private float _lastDeltaTime;
    
    protected RoadBuddyApp()
    {
    }

    public void Run()
    {
        var prevTime = TimerHelper.GetTimestamp();
        var spinner = new SpinWait();

        this.OnStart();
        while (!_exit)
        {
            var curTime = TimerHelper.GetTimestamp();
            var elaspedTime = TimerHelper.GetElaspedTime(prevTime, curTime);

            // frame indipendent physics update loop
            _fixedRegulator.Update(elaspedTime * 0.0000001f);
            while (_fixedRegulator.IsReady())
            {
                this.OnFixedUpdate(TARGET_FIXED_TIME * 0.0000001f);
                _timeSinceStartup += TARGET_FIXED_TIME;
            }
            this.OnUpdate(elaspedTime * 0.0000001f); // fraction of a second
            prevTime = curTime;

            // Calculate how many milliseconds until the next update is due.
            const int sleepAccuracyCompensation = 0; // 0 = slightly inaccurate, 1 = hightly accurate
            var updateDueTime = (int)((prevTime + TARGET_VARIABLE_TIME - TimerHelper.GetTimestamp()) * UsToMs);
            if (updateDueTime > 0 && updateDueTime <= (int)(TARGET_VARIABLE_TIME * UsToMs))
                Thread.Sleep(updateDueTime - sleepAccuracyCompensation);

            // Spin the CPU idle for the remaining microseconds
            // SpinWait will try to mix in Sleep(0) and Yield so we remaing responsive
            // while avoiding stavation or cache contention.

            // while (TimerHelper.GetElaspedTime(prevTime) < TARGET_VARIABLE_TIME)
            //     spinner.SpinOnce(-1); // -1 to disable Sleep(1+) because we already slept

            _lastSpinCount = spinner.Count;
            spinner.Reset();
        }
        this.OnExit();
    }


    public void Close()
    {
        _exit = true;
    }

    protected virtual void OnStart()
    {
        if (OperatingSystem.IsWindows())
            _ = TimerHelper.timeBeginPeriod(1);
    }

    protected virtual void OnFixedUpdate(float deltaTime)
    {
        _fixedTicks = _fixedCounter.Update(deltaTime);
    }

    protected virtual void OnUpdate(float deltaTime)
    {
        _lastDeltaTime = deltaTime;
        _variableTicks = _variableCounter.Update(deltaTime);

        Console.Title = $"{_title} [{_fixedTicks} UPS/s :: {TARGET_FIXED_TIME * UsToMs:0.000}ms] [FPS:{_variableTicks} :: {deltaTime * MsPerSecond:0.000}ms] (SpinCount = {_lastSpinCount})";

        if (!Console.IsInputRedirected && Console.KeyAvailable)
        {
            var key = Console.ReadKey(true);
            if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.C)
                this.Close();
        }
    }

    public string GetMetrics()
    {
        return
            $"[{_fixedTicks} UPS/s :: {TARGET_FIXED_TIME * UsToMs:0.000}ms] [FPS:{_variableTicks} :: {_lastDeltaTime * MsPerSecond:0.000}ms] (SpinCount = {_lastSpinCount})";
    }

    protected virtual void OnExit()
    {
        if (OperatingSystem.IsWindows())
            _ = TimerHelper.timeEndPeriod(1);

        Console.WriteLine($"{nameof(this.OnExit)} called.");
        Console.Beep();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
            }
            
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}