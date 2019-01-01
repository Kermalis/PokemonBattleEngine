using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
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

        public byte Culprit { get; } // Ability owner
        public byte Victim { get; }
        public PBEAbility Ability { get; }
        public PBEAbilityAction AbilityAction { get; }

        public PBEAbilityPacket(PBEPokemon culprit, PBEPokemon victim, PBEAbility ability, PBEAbilityAction abilityAction)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(Culprit = culprit.Id);
            bytes.Add(Victim = victim.Id);
            bytes.Add((byte)(Ability = ability));
            bytes.Add((byte)(AbilityAction = abilityAction));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEAbilityPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Culprit = r.ReadByte();
                Victim = r.ReadByte();
                Ability = (PBEAbility)r.ReadByte();
                AbilityAction = (PBEAbilityAction)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
