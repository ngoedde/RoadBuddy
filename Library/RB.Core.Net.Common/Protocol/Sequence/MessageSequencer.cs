namespace RB.Core.Net.Common.Protocol.Sequence;

public sealed class MessageSequencer : IMessageSequencer
{
    private const uint DEFAULT_SEED = 0x9ABFB3B6;
    private byte _byte0;
    private byte _byte1;
    private byte _byte2;

    public void Initialize(uint value)
    {
        var mut0 = value != 0 ? value : DEFAULT_SEED;
        var mut1 = GenerateValue(ref mut0);
        var mut2 = GenerateValue(ref mut0);
        var mut3 = GenerateValue(ref mut0);
        _ = GenerateValue(ref mut0);

        _byte1 = (byte)((mut1 & byte.MaxValue) ^ (mut2 & byte.MaxValue));
        if (_byte1 == 0)
            _byte1 = 1;

        _byte2 = (byte)((mut0 & byte.MaxValue) ^ (mut3 & byte.MaxValue));
        if (_byte2 == 0)
            _byte2 = 1;

        _byte0 = (byte)(_byte2 ^ _byte1);
    }

    public byte Next()
    {
        var value = (byte)(_byte2 * (~_byte0 + _byte1));
        return _byte0 = (byte)(value ^ (value >> 4));
    }

    private static uint GenerateValue(ref uint value)
    {
        for (var i = 0; i < 32; i++)
        {
            var v = value;
            v = (v >> 2) ^ value;
            v = (v >> 2) ^ value;
            v = (v >> 1) ^ value;
            v = (v >> 1) ^ value;
            v = (v >> 1) ^ value;
            value = (((value >> 1) | (value << 31)) & ~1u) | (v & 1);
        }

        return value;
    }
}