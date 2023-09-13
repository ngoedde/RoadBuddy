namespace RB.Core.Net.Common.Extensions;

public static class InterlockedExtensions
{
    public static bool ExchangeIfGreaterThan(ref float location, float comparison, float newValue)
    {
        float initialValue;
        do
        {
            initialValue = location;
            if (initialValue >= comparison)
                return false;
        } while (Interlocked.CompareExchange(ref location, newValue, initialValue) != initialValue);

        return true;
    }

    public static bool ExchangeIfGreaterThan(ref double location, double comparison, double newValue)
    {
        double initialValue;
        do
        {
            initialValue = location;
            if (initialValue >= comparison)
                return false;
        } while (Interlocked.CompareExchange(ref location, newValue, initialValue) != initialValue);

        return true;
    }

    public static bool ExchangeIfGreaterThan(ref uint location, uint comparison, uint newValue)
    {
        uint initialValue;
        do
        {
            initialValue = location;
            if (initialValue >= comparison)
                return false;
        } while (Interlocked.CompareExchange(ref location, newValue, initialValue) != initialValue);

        return true;
    }

    public static bool ExchangeIfGreaterThan(ref int location, int comparison, int newValue)
    {
        int initialValue;
        do
        {
            initialValue = location;
            if (initialValue >= comparison)
                return false;
        } while (Interlocked.CompareExchange(ref location, newValue, initialValue) != initialValue);

        return true;
    }

    public static bool ExchangeIfGreaterThan(ref ulong location, ulong comparison, ulong newValue)
    {
        ulong initialValue;
        do
        {
            initialValue = location;
            if (initialValue >= comparison)
                return false;
        } while (Interlocked.CompareExchange(ref location, newValue, initialValue) != initialValue);

        return true;
    }

    public static bool ExchangeIfGreaterThan(ref long location, long comparison, long newValue)
    {
        long initialValue;
        do
        {
            initialValue = location;
            if (initialValue >= comparison)
                return false;
        } while (Interlocked.CompareExchange(ref location, newValue, initialValue) != initialValue);

        return true;
    }

    public static bool ExchangeIfLessThan(ref int location, int comparison, int newValue)
    {
        int initialValue;
        do
        {
            initialValue = location;
            if (initialValue <= comparison)
                return false;
        } while (Interlocked.CompareExchange(ref location, newValue, initialValue) != initialValue);

        return true;
    }

    public static bool ExchangeIfLessThan(ref long location, long comparison, long newValue)
    {
        long initialValue;
        do
        {
            initialValue = location;
            if (initialValue <= comparison)
                return false;
        } while (Interlocked.CompareExchange(ref location, newValue, initialValue) != initialValue);

        return true;
    }
}