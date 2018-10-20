using Ether.Network.Packets;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PMoveFailPacket : INetPacket
    {
        public const short Code = 0x15;
        public IEnumerable<byte> Buffer => new byte[] { 0x02, 0x00, 0x15, 0x00 };

        public PMoveFailPacket() { }
        public PMoveFailPacket(byte[] buffer) { }

        public void Dispose() { }
    }
}
