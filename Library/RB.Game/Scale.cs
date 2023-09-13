namespace RB.Game;

public struct Scale : IEquatable<Scale>
{
    private const byte MaxHeight = 4;
    private const byte MaxWidth = 4;

    #region Reasons to use C++

    private const int HeightSize = 4;
    private const int HeightOffset = 0;
    private const ushort HeightMask = ((1 << HeightSize) - 1) << HeightOffset;

    private const int WidthSize = 4;
    private const int WidthOffset = HeightOffset + HeightSize;
    private const ushort WidthMask = ((1 << WidthSize) - 1) << WidthOffset;
    private static readonly Scale scale = new(0, 0);

    #endregion Reasons to use C++

    public static readonly Scale MinValue = scale;
    public static readonly Scale MaxValue = new(MaxWidth, MaxHeight);

    private byte _value;

    #region Constructors

    public Scale(byte value)
    {
        _value = value;
    }

    public Scale(byte width, byte height)
    {
        _value = 0;
        Height = height;
        Width = width;
    }

    #endregion Constructors

    #region Properties

    public byte Height
    {
        get => (byte)((_value & HeightMask) >> HeightOffset);
        set => _value = (byte)((_value & ~HeightMask) | ((value << HeightOffset) & HeightMask));
    }

    public byte Width
    {
        get => (byte)((_value & WidthMask) >> WidthOffset);
        set => _value = (byte)((_value & ~WidthMask) | ((value << WidthOffset) & WidthMask));
    }

    public bool IsValid => Height <= MaxHeight && Width <= MaxWidth;

    #endregion Properties

    #region Methods

    public override string ToString()
    {
        return _value.ToString();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Scale scale)
            return Equals(scale);

        return false;
    }

    public bool Equals(Scale other)
    {
        return _value == other._value;
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    #endregion Methods

    #region Operators

    public static bool operator ==(Scale left, Scale right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Scale left, Scale right)
    {
        return !(left == right);
    }

    public static implicit operator byte(Scale scale)
    {
        return scale._value;
    }

    public static explicit operator Scale(byte value)
    {
        return new Scale(value);
    }

    #endregion Operators
}