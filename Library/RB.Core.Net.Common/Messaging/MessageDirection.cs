using System;

namespace RB.Core.Net.Common.Messaging;

[Flags]
public enum MessageDirection : byte
{
    NoDir = 0,
    Req = 1,
    Ack = 2,
}