
using System;

namespace RB.Core.Net.Common.Extensions;

public static class RandomExtensions
{
    public static sbyte NextSByte(this Random random) => unchecked((sbyte)random.Next());

    public static byte NextByte(this Random random) => unchecked((byte)random.Next());

    public static short NextShort(this Random random) => unchecked((short)random.Next());

    public static ushort NextUShort(this Random random) => unchecked((ushort)random.Next());

    public static uint NextUInt(this Random random) => unchecked((uint)random.Next());

    public static ulong NextULong(this Random random) => (ulong)(random.Next() << 32 | random.Next());

    public static long NextLong(this Random random) => random.Next() << 32 | random.Next();
}