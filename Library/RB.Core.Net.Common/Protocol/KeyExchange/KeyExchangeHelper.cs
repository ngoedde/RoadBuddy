using System.Runtime.InteropServices;

namespace RB.Core.Net.Common.Protocol.KeyExchange;

public static class KeyExchangeHelper
{
    public static uint G_pow_X_mod_P(uint G, uint X, uint P)
    {
        long result = 1;
        long mult = G;
        if (X == 0)
            return 1;

        while (X != 0)
        {
            if ((X & 1) > 0)
                result = mult * result % P;

            X >>= 1;
            mult = mult * mult % P;
        }

        return (uint)result;
    }

    public static void KeyTransformValue(Span<byte> value, uint key, byte keyByte)
    {
        var span = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref key, 1));

        value[0] ^= (byte)(value[0] + span[0] + keyByte);
        value[1] ^= (byte)(value[1] + span[1] + keyByte);
        value[2] ^= (byte)(value[2] + span[2] + keyByte);
        value[3] ^= (byte)(value[3] + span[3] + keyByte);

        value[4] ^= (byte)(value[4] + span[0] + keyByte);
        value[5] ^= (byte)(value[5] + span[1] + keyByte);
        value[6] ^= (byte)(value[6] + span[2] + keyByte);
        value[7] ^= (byte)(value[7] + span[3] + keyByte);
    }

    public static void CalculateKey(Span<byte> key, uint sharedSecret, uint secret1, uint secret2)
    {
        MemoryMarshal.Write(key.Slice(0), ref secret1);
        MemoryMarshal.Write(key.Slice(4), ref secret2);
        KeyTransformValue(key, sharedSecret, (byte)(sharedSecret & 3));
    }

    public static void CalculateFinalKey(Span<byte> key, uint sharedSecret, ulong initialKey)
    {
        MemoryMarshal.Write(key, ref initialKey);
        KeyTransformValue(key, sharedSecret, 3);
    }

    public static void CalculateSignature(Span<byte> key, uint sharedSecret, uint secret1, uint secret2)
    {
        MemoryMarshal.Write(key.Slice(0), ref secret1);
        MemoryMarshal.Write(key.Slice(4), ref secret2);
        KeyTransformValue(key, sharedSecret, (byte)(secret1 & 7));
    }
}