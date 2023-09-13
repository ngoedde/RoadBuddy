namespace RB.Core.Net.Common.Messaging;

[Flags]
public enum MessageOptions : byte
{
    None = 0,
    Local = 1,
    Encrypted = 2,
    Massive = 4
}