using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Packets
{
    // TODO: Include IDs
    public sealed class PBEMoveCritPacket : INetPacket
    {
        public const short Code = 0x0F;
        public IEnumerable<byte> Buffer { get; } = new byte[] { 0x02, 0x00, 0x0F, 0x00 };

        public PBEMoveCritPacket() { }
        public PBEMoveCritPacket(byte[] buffer, PBEBattle battle) { }

        public void Dispose() { }
    }
}
