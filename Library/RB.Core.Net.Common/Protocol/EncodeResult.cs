namespace RB.Core.Net.Common.Protocol;

public enum EncodeResult : byte
{
    Success,
    InvalidMsgSize,
    InvalidHeader,
}