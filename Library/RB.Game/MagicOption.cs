namespace RB.Game;

public struct MagicOption : IEquatable<MagicOption>
{
    #region Reasons to use C++

    private const int TYPE_SIZE = 32;
    private const int TYPE_OFFSET = 0;
    private const ulong TYPE_MASK = ((1ul << TYPE_SIZE) - 1ul) << TYPE_OFFSET;

    private const int VALUE_SIZE = 32;
    private const int VALUE_OFFSET = TYPE_OFFSET + TYPE_SIZE;
    private const ulong VALUE_MASK = ((1ul << VALUE_SIZE) - 1ul) << VALUE_OFFSET;

    #endregion Reasons to use C++

    private ulong _value;

    /// <summary>
    ///     MOptID
    /// </summary>
    public int ID
    {
        get => Convert.ToInt32((_value & TYPE_MASK) >> TYPE_OFFSET);
        set => _value = (_value & ~TYPE_MASK) | ((Convert.ToUInt64(value) << TYPE_OFFSET) & TYPE_MASK);
    }

    /// <summary>
    ///     MOptValue
    /// </summary>
    public int Value
    {
        get => Convert.ToInt32((_value & VALUE_MASK) >> VALUE_OFFSET);
        set => _value = (_value & ~VALUE_MASK) | ((Convert.ToUInt64(value) << VALUE_OFFSET) & VALUE_MASK);
    }

    public MagicOption(ulong value)
    {
        _value = value;
    }

    public MagicOption(int id, int value)
    {
        _value = 0;
        ID = id;
        Value = value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is MagicOption option)
            return Equals(option);

        return false;
    }

    public bool Equals(MagicOption other)
    {
        return _value == other._value;
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public static explicit operator MagicOption(ulong value)
    {
        return new MagicOption(value);
    }

    public static implicit operator ulong(MagicOption option)
    {
        return option._value;
    }

    public static bool operator ==(MagicOption left, MagicOption right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(MagicOption left, MagicOption right)
    {
        return !(left == right);
    }
}