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

        public PBEFieldPosition AbilityOwner { get; }
        public PBETeam AbilityOwnerTeam { get; }
        public PBEFieldPosition Pokemon2 { get; }
        public PBETeam Pokemon2Team { get; }
        public PBEAbility Ability { get; }
        public PBEAbilityAction AbilityAction { get; }

        public PBEAbilityPacket(PBEPokemon abilityOwner, PBEPokemon pokemon2, PBEAbility ability, PBEAbilityAction abilityAction)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(AbilityOwner = abilityOwner.FieldPosition));
            bytes.Add((AbilityOwnerTeam = abilityOwner.Team).Id);
            bytes.Add((byte)(Pokemon2 = pokemon2.FieldPosition));
            bytes.Add((Pokemon2Team = pokemon2.Team).Id);
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
                AbilityOwner = (PBEFieldPosition)r.ReadByte();
                AbilityOwnerTeam = battle.Teams[r.ReadByte()];
                Pokemon2 = (PBEFieldPosition)r.ReadByte();
                Pokemon2Team = battle.Teams[r.ReadByte()];
                Ability = (PBEAbility)r.ReadByte();
                AbilityAction = (PBEAbilityAction)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
