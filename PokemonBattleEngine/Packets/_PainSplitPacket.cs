using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Packets
{
    // TODO: Include IDs
    public sealed class PBEPainSplitPacket : INetPacket
    {
        public const short Code = 0x21;
        public IEnumerable<byte> Buffer { get; } = new byte[] { 0x02, 0x00, 0x21, 0x00 };

        public PBEPainSplitPacket() { }
        public PBEPainSplitPacket(byte[] buffer, PBEBattle battle) { }

        public void Dispose() { }
    }
}
