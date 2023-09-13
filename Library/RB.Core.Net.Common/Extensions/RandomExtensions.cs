namespace RB.Core.Net.Common.Extensions;

public static class RandomExtensions
{
    public static sbyte NextSByte(this Random random)
    {
        return unchecked((sbyte)random.Next());
    }

    public static byte NextByte(this Random random)
    {
        return unchecked((byte)random.Next());
    }

    public static short NextShort(this Random random)
    {
        return unchecked((short)random.Next());
    }

    public static ushort NextUShort(this Random random)
    {
        return unchecked((ushort)random.Next());
    }

    public static uint NextUInt(this Random random)
    {
        return unchecked((uint)random.Next());
    }

    public static ulong NextULong(this Random random)
    {
        return (ulong)((random.Next() << 32) | random.Next());
    }

    public static long NextLong(this Random random)
    {
        return (random.Next() << 32) | random.Next();
    }
}