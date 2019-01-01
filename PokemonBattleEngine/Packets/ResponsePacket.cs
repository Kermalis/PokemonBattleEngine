using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEResponsePacket : INetPacket
    {
        public const short Code = 0x00;
        public IEnumerable<byte> Buffer { get; } = new byte[] { 0x02, 0x00, 0x00, 0x00 };

        public PBEResponsePacket() { }
        public PBEResponsePacket(byte[] buffer, PBEBattle battle) { }

        public void Dispose() { }
    }
}
