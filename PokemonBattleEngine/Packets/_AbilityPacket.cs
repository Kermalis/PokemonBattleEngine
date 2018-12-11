using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEAbilityPacket : INetPacket
    {
        public const short Code = 0x19;
        public IEnumerable<byte> Buffer { get; }

        public byte CulpritId { get; } // Ability owner
        public byte VictimId { get; }
        public PBEAbility Ability { get; }
        public PBEAbilityAction AbilityAction { get; }

        public PBEAbilityPacket(PBEPokemon culprit, PBEPokemon victim, PBEAbility ability, PBEAbilityAction abilityAction)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(CulpritId = culprit.Id);
            bytes.Add(VictimId = victim.Id);
            bytes.Add((byte)(Ability = ability));
            bytes.Add((byte)(AbilityAction = abilityAction));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEAbilityPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                CulpritId = r.ReadByte();
                VictimId = r.ReadByte();
                Ability = (PBEAbility)r.ReadByte();
                AbilityAction = (PBEAbilityAction)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
