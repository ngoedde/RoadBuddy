namespace RB.Game;

public struct HitDamage : IEquatable<HitDamage>
{
    #region Reasons to use C++

    private const int FLAG_SIZE = 8;
    private const int FLAG_OFFSET = 0;
    private const uint FLAG_MASK = ((1u << FLAG_SIZE) - 1u) << FLAG_OFFSET;

    private const int VALUE_SIZE = 24;
    private const int VALUE_OFFSET = FLAG_OFFSET + FLAG_SIZE;
    private const uint VALUE_MASK = ((1u << VALUE_SIZE) - 1u) << VALUE_OFFSET;

    #endregion Reasons to use C++

    private uint _value;

    public HitDamage(uint value)
    {
        _value = value;
    }

    public HitDamageFlag Flag
    {
        get => (HitDamageFlag)((_value & FLAG_MASK) >> FLAG_OFFSET);
        set => _value = (uint)((_value & ~FLAG_MASK) | (((byte)value << FLAG_OFFSET) & FLAG_MASK));
    }

    public int Value
    {
        get => (int)((_value & VALUE_MASK) >> VALUE_OFFSET);
        set => _value = (uint)((_value & ~VALUE_MASK) | (((byte)value << VALUE_OFFSET) & VALUE_MASK));
    }

    public override bool Equals(object? obj)
    {
        if (obj is HitDamage damage)
            return Equals(damage);

        return false;
    }

    public bool Equals(HitDamage other)
    {
        return _value == other._value;
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public static implicit operator uint(HitDamage region)
    {
        return region._value;
    }

    public static explicit operator HitDamage(uint value)
    {
        return new HitDamage(value);
    }

    public static bool operator ==(HitDamage left, HitDamage right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(HitDamage left, HitDamage right)
    {
        return !(left == right);
    }
}