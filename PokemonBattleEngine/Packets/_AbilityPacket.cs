using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEAbilityPacket : IPBEPacket
    {
        public const ushort Code = 0x19;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFieldPosition AbilityOwner { get; }
        public PBETeam AbilityOwnerTeam { get; }
        public PBEFieldPosition Pokemon2 { get; }
        public PBETeam Pokemon2Team { get; }
        public PBEAbility Ability { get; }
        public PBEAbilityAction AbilityAction { get; }

        internal PBEAbilityPacket(PBEBattlePokemon abilityOwner, PBEBattlePokemon pokemon2, PBEAbility ability, PBEAbilityAction abilityAction)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(AbilityOwner = abilityOwner.FieldPosition);
                w.Write((AbilityOwnerTeam = abilityOwner.Team).Id);
                w.Write(Pokemon2 = pokemon2.FieldPosition);
                w.Write((Pokemon2Team = pokemon2.Team).Id);
                w.Write(Ability = ability);
                w.Write(AbilityAction = abilityAction);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEAbilityPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            AbilityOwner = r.ReadEnum<PBEFieldPosition>();
            AbilityOwnerTeam = battle.Teams[r.ReadByte()];
            Pokemon2 = r.ReadEnum<PBEFieldPosition>();
            Pokemon2Team = battle.Teams[r.ReadByte()];
            Ability = r.ReadEnum<PBEAbility>();
            AbilityAction = r.ReadEnum<PBEAbilityAction>();
        }
    }
}
