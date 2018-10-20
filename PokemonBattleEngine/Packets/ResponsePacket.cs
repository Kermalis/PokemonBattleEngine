using Ether.Network.Packets;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PResponsePacket : INetPacket
    {
        public const short Code = 0x00;
        public IEnumerable<byte> Buffer => new byte[] { 0x02, 0x00, 0x00, 0x00 };

        public PResponsePacket() { }
        public PResponsePacket(byte[] buffer) { }
        
        public void Dispose() { }
    }
}
