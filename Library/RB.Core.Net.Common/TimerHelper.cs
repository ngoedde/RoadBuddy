using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace RB.Core.Net.Common;

public static class TimerHelper
{
    private static double s_tickFrequency = TimeSpan.TicksPerSecond / Stopwatch.Frequency;

    [DllImport("winmm.dll")]
    [SupportedOSPlatform("windows")]
    public static extern uint timeBeginPeriod(uint period);

    [DllImport("winmm.dll")]
    [SupportedOSPlatform("windows")]
    public static extern uint timeEndPeriod(uint period);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long GetTimestamp() => Stopwatch.GetTimestamp();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long GetElaspedTime(long startingTimestamp) => (long)((GetTimestamp() - startingTimestamp) * s_tickFrequency);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long GetElaspedTime(long startingTimestamp, long endingTimestamp) => (long)((endingTimestamp - startingTimestamp) * s_tickFrequency);

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static TimeSpan GetElaspedTimeSpan(long startingTimestamp) => Stopwatch.GetElapsedTime(startingTimestamp);

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static TimeSpan GetElaspedTimeSpan(long startingTimestamp, long endingTimestamp) => Stopwatch.GetElapsedTime(startingTimestamp, endingTimestamp);
}