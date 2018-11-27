using Ether.Network.Packets;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Packets
{
    // TODO: Include IDs
    public sealed class PPainSplitPacket : INetPacket
    {
        public const short Code = 0x21;
        public IEnumerable<byte> Buffer => new byte[] { 0x02, 0x00, 0x21, 0x00 };

        public PPainSplitPacket() { }
        public PPainSplitPacket(byte[] buffer) { }

        public void Dispose() { }
    }
}
