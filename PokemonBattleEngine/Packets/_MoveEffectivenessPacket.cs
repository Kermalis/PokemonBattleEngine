using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveEffectivenessPacket : INetPacket
    {
        public const short Code = 0x0B;
        public IEnumerable<byte> Buffer { get; }

        public byte Victim { get; }
        public PBEEffectiveness Effectiveness { get; }

        public PBEMoveEffectivenessPacket(PBEPokemon victim, PBEEffectiveness effectiveness)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(Victim = victim.Id);
            bytes.Add((byte)(Effectiveness = effectiveness));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEMoveEffectivenessPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Victim = r.ReadByte();
                Effectiveness = (PBEEffectiveness)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
