using System;
using System.IO;
using System.Runtime.InteropServices;

namespace RB.Core.Net.Common.Collections;

public class CompressedLookupTable32
{
    private const uint VALUE_MASK = 0b11;
    private const int VALUE_BIT_SIZE = 2;
    private const int ELEMENT_BIT_SIZE = 32;
    private const int VALUE_COUNT_PER_ELEMENT = ELEMENT_BIT_SIZE / VALUE_BIT_SIZE;

    private int _x;
    private int _y;
    private uint[] _bits;

    public int Length => _x * _y;

    public int MemoryUsage => _bits.Length * sizeof(uint);

    public int GetLength(int dimension)
    {
        if (dimension == 0)
            return _x;
        else if (dimension == 1)
            return _y;

        throw new ArgumentOutOfRangeException(nameof(dimension));
    }

    public CompressedLookupTable32(int x, int y)
    {
        if (x <= 0)
            throw new ArgumentOutOfRangeException(nameof(x), $"{nameof(x)} must be greater than 0");

        if (y <= 0)
            throw new ArgumentOutOfRangeException(nameof(y), $"{nameof(y)} must be greater than 0");

        _x = x;
        _y = y;

        _bits = new uint[(int)MathF.Ceiling(this.Length / (float)VALUE_COUNT_PER_ELEMENT)];
        for (int i = 0; i < _bits.Length; i++)
            _bits[i] = uint.MaxValue;
    }

    public int this[int index]
    {
        get
        {
            if (index < 0 || index >= this.Length)
                return -1;

            var value = (int)(_bits[index >> 4] >> (index % VALUE_COUNT_PER_ELEMENT * VALUE_BIT_SIZE) & VALUE_MASK);
            return value - 1;
        }
        set
        {
            var offset = (index % VALUE_COUNT_PER_ELEMENT * VALUE_BIT_SIZE);
            var mask = VALUE_MASK << offset;
            _bits[index >> 4] = (_bits[index >> 4] & ~mask) | ((((uint)(value + 1) & VALUE_MASK) << offset) & mask);
        }
    }

    public int this[int x, int y]
    {
        get => this[(y * _x) + x];
        set => this[(y * _x) + x] = value;
    }

    public void Read(BinaryReader reader)
    {
        _x = reader.ReadInt32();
        _y = reader.ReadInt32();

        if (_x <= 0)
            throw new ArgumentOutOfRangeException(nameof(_x), $"{nameof(_x)} must be greater than 0");

        if (_y <= 0)
            throw new ArgumentOutOfRangeException(nameof(_y), $"{nameof(_y)} must be greater than 0");

        _bits = new uint[(int)MathF.Ceiling(this.Length / (float)VALUE_COUNT_PER_ELEMENT)];
        _ = reader.BaseStream.Read(MemoryMarshal.Cast<uint, byte>(_bits));
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(_x);
        writer.Write(_y);
        writer.Write(MemoryMarshal.Cast<uint, byte>(_bits));
    }
}