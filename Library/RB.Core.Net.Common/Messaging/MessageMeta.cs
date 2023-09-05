
using System;
using System.Runtime.InteropServices;

namespace RB.Core.Net.Common.Messaging;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = sizeof(ushort))]
public struct MessageMeta : IEquatable<MessageMeta>
{
    public static readonly MessageMeta Empty = new MessageMeta(ushort.MinValue);

    #region Reasons to use C++

    // (MSB)                                                                      (LSB)
    // | 15 | 14 | 13 | 12 | 11 | 10 | 09 | 08 | 07 | 06 | 05 | 04 | 03 | 02 | 01 | 00 |
    // | E* |                                   SIZE                                   |
    //
    // E* = EncryptionFlag

    private const int SIZE_SIZE = 15;
    private const int SIZE_OFFSET = 0;
    private const ushort SIZE_MASK = (1 << SIZE_SIZE) - 1 << SIZE_OFFSET;

    private const int ENCRYPTED_SIZE = 1;
    private const int ENCRYPTED_OFFSET = SIZE_OFFSET + SIZE_SIZE;
    private const ushort ENCRYPTED_MASK = (1 << ENCRYPTED_SIZE) - 1 << ENCRYPTED_OFFSET;

    #endregion Reasons to use C++

    private ushort _value;

    #region Properties

    public ushort DataSize
    {
        get => (ushort)((_value & SIZE_MASK) >> SIZE_OFFSET);
        set => _value = (ushort)((_value & ~SIZE_MASK) | ((value << SIZE_OFFSET) & SIZE_MASK));
    }

    public bool Encrypted
    {
        get => ((_value & ENCRYPTED_MASK) >> ENCRYPTED_OFFSET) != 0;
        set => _value = (ushort)((_value & ~ENCRYPTED_MASK) | ((Convert.ToByte(value) << ENCRYPTED_OFFSET) & ENCRYPTED_MASK));
    }

    #endregion Properties

    public MessageMeta(ushort value) => _value = value;

    public MessageMeta(ushort size, bool encrypted)
    {
        _value = (ushort)(size | (encrypted ? Message.HEADER_ENC_MASK : default));
    }

    public override string ToString()
    {
        return this.Encrypted ? $"{this.DataSize} (Encrypted)" : $"{this.DataSize}";
    }

    public int CalcRawSize()
    {
        if (this.Encrypted)
            return Blowfish.GetOutputLength(this.DataSize + Message.HEADER_ENC_SIZE) + Message.HEADER_ENC_OFFSET;
        else
            return this.DataSize + Message.HEADER_SIZE;
    }

    #region IEquatable

    public override bool Equals(object? obj) => obj is MessageMeta size && this.Equals(size);

    public bool Equals(MessageMeta other) => _value == other._value;

    public override int GetHashCode() => HashCode.Combine(_value);

    public static bool operator ==(MessageMeta left, MessageMeta right) => left.Equals(right);

    public static bool operator !=(MessageMeta left, MessageMeta right) => !(left == right);

    #endregion IEquatable

    public static implicit operator ushort(MessageMeta size) => size._value;

    public static explicit operator MessageMeta(ushort size) => new MessageMeta(size);

    //public static explicit operator MessageMeta(short size) => new MessageMeta(unchecked((ushort)size));
}