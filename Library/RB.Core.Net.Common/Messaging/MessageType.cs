namespace RB.Core.Net.Common.Messaging;

public enum MessageType : byte
{
    P2P = 0, // NODIR=0x0000, REQ=0x4000, ACK=0x8000
    NetEngine = 1, // NODIR=0x1000, REQ=0x5000, ACK=0x9000
    Framework = 2, // NODIR=0x2000, REQ=0x6000, ACK=0xA000
    Game = 3, // NODIR=0x3000, REQ=0x7000, ACK=0xB000
}