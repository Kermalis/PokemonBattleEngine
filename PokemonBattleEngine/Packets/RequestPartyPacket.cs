using Ether.Network.Packets;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PRequestPartyPacket : INetPacket
    {
        public const short Code = 0x03;
        public IEnumerable<byte> Buffer => new byte[] { 0x02, 0x00, 0x03, 0x00 };

        public PRequestPartyPacket() { }
        public PRequestPartyPacket(byte[] buffer) { }

        public void Dispose() { }
    }
}
