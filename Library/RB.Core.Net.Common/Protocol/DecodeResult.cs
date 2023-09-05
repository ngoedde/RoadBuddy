namespace RB.Core.Net.Common.Protocol;

public enum DecodeResult
{
    Success,
    InvalidHeader,
    InvalidSequence,
    InvalidChecksum,
    InvalidMsgSize,
    Unknown,
}