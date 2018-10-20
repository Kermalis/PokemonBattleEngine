using Ether.Network.Packets;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PMatchCancelledPacket : INetPacket
    {
        public const short Code = 0x02;
        public IEnumerable<byte> Buffer => new byte[] { 0x02, 0x00, 0x02, 0x00 };

        public PMatchCancelledPacket() { }
        public PMatchCancelledPacket(byte[] buffer) { }

        public void Dispose() { }
    }
}
