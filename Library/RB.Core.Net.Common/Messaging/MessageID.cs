using System;

namespace RB.Core.Net.Common.Messaging;

public struct MessageID : IEquatable<MessageID>
{
    #region Reasons to use C++

    // (MSB)                                                                      (LSB)
    // | 15 | 14 | 13 | 12 | 11 | 10 | 09 | 08 | 07 | 06 | 05 | 04 | 03 | 02 | 01 | 00 |
    // |    DIR  |  TYPE   |                       OPERATION                           |

    private const int OPERATION_SIZE = 12;
    private const int OPERATION_OFFSET = 0;
    private const ushort OPERATION_MASK = ((1 << OPERATION_SIZE) - 1) << OPERATION_OFFSET;

    private const int TYPE_SIZE = 2;
    private const int TYPE_OFFSET = OPERATION_OFFSET + OPERATION_SIZE;
    private const ushort TYPE_MASK = ((1 << TYPE_SIZE) - 1) << TYPE_OFFSET;

    private const int DIRECTION_SIZE = 2;
    private const int DIRECTION_OFFSET = TYPE_OFFSET + TYPE_SIZE;
    private const ushort DIRECTION_MASK = ((1 << DIRECTION_SIZE) - 1) << DIRECTION_OFFSET;

    #endregion Reasons to use C++

    public static readonly MessageID Empty = new MessageID(ushort.MinValue);
    public static readonly MessageID MaxValue = new MessageID(ushort.MaxValue);

    private ushort _value;

    #region Properties

    public MessageDirection Direction
    {
        get => (MessageDirection)((_value & DIRECTION_MASK) >> DIRECTION_OFFSET);
        set => _value = (ushort)((_value & ~DIRECTION_MASK) | (((byte)value << DIRECTION_OFFSET) & DIRECTION_MASK));
    }

    public MessageType Type
    {
        get => (MessageType)((_value & TYPE_MASK) >> TYPE_OFFSET);
        set => _value = (ushort)((_value & ~TYPE_MASK) | (((byte)value << TYPE_OFFSET) & TYPE_MASK));
    }

    public ushort Operation
    {
        get => (ushort)((_value & OPERATION_MASK) >> OPERATION_OFFSET);
        set => _value = (ushort)((_value & ~OPERATION_MASK) | ((value << OPERATION_OFFSET) & OPERATION_MASK));
    }

    #endregion Properties

    public MessageID(ushort value) => _value = value;

    private MessageID(MessageDirection direction, MessageType type, ushort operation)
    {
        _value = default;
        this.Direction = direction;
        this.Type = type;
        this.Operation = operation;
    }

    public static MessageID Create(ushort value) => new MessageID(value);

    public static MessageID Create(MessageDirection dir, MessageType type, ushort operation) => new MessageID(dir, type, operation);

    public override string ToString() => $"0x{_value:X4} [{this.Direction}; {this.Type}; 0x{this.Operation:X3}]";

    //public static implicit operator ushort(MessageID id) => id._value;

    public static explicit operator MessageID(ushort id) => new MessageID(id);

    public static implicit operator ushort(MessageID id) => id._value;

    #region IEquatable

    public static bool operator ==(MessageID left, MessageID right) => left.Equals(right);

    public static bool operator !=(MessageID left, MessageID right) => !(left == right);

    public override bool Equals(object? obj) => obj is MessageID id && this.Equals(id);

    public bool Equals(MessageID other) => _value == other._value;

    public override int GetHashCode() => HashCode.Combine(_value);

    #endregion IEquatable
}