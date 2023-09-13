namespace RB.Core.Net.Common.Protocol;

[Flags]
public enum ProtocolOptions : byte
{
    None = 0,

    Disabled = 1,

    /// <summary>
    ///     Blowfish
    /// </summary>
    Encryption = 2,

    /// <summary>
    ///     Error detection code with Sequence and Cyclic redundancy check
    /// </summary>
    ErrorDetection = 4,

    KeyExchange = 8,

    KeyChallenge = 16
}