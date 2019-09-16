using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEAbilityPacket : INetPacket
    {
        public const short Code = 0x19;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBEFieldPosition AbilityOwner { get; }
        public PBETeam AbilityOwnerTeam { get; }
        public PBEFieldPosition Pokemon2 { get; }
        public PBETeam Pokemon2Team { get; }
        public PBEAbility Ability { get; }
        public PBEAbilityAction AbilityAction { get; }

        internal PBEAbilityPacket(PBEPokemon abilityOwner, PBEPokemon pokemon2, PBEAbility ability, PBEAbilityAction abilityAction)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(AbilityOwner = abilityOwner.FieldPosition));
            bytes.Add((AbilityOwnerTeam = abilityOwner.Team).Id);
            bytes.Add((byte)(Pokemon2 = pokemon2.FieldPosition));
            bytes.Add((Pokemon2Team = pokemon2.Team).Id);
            bytes.Add((byte)(Ability = ability));
            bytes.Add((byte)(AbilityAction = abilityAction));
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEAbilityPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            AbilityOwner = (PBEFieldPosition)r.ReadByte();
            AbilityOwnerTeam = battle.Teams[r.ReadByte()];
            Pokemon2 = (PBEFieldPosition)r.ReadByte();
            Pokemon2Team = battle.Teams[r.ReadByte()];
            Ability = (PBEAbility)r.ReadByte();
            AbilityAction = (PBEAbilityAction)r.ReadByte();
        }

        public void Dispose() { }
    }
}
