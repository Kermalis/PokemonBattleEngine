using Ether.Network.Packets;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Packets
{
    // TODO: Include IDs
    public sealed class PMoveCritPacket : INetPacket
    {
        public const short Code = 0x0F;
        public IEnumerable<byte> Buffer => new byte[] { 0x02, 0x00, 0x0F, 0x00 };

        public PMoveCritPacket() { }
        public PMoveCritPacket(byte[] buffer) { }

        public void Dispose() { }
    }
}
