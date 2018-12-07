using Ether.Network.Packets;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMatchCancelledPacket : INetPacket
    {
        public const short Code = 0x02;
        public IEnumerable<byte> Buffer { get; } = new byte[] { 0x02, 0x00, 0x02, 0x00 };

        public PBEMatchCancelledPacket() { }
        public PBEMatchCancelledPacket(byte[] buffer) { }

        public void Dispose() { }
    }
}
