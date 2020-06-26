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

        public PBEBattlePokemon AbilityOwner { get; }
        public PBEBattlePokemon Pokemon2 { get; }
        public PBEAbility Ability { get; }
        public PBEAbilityAction AbilityAction { get; }

        internal PBEAbilityPacket(PBEBattlePokemon abilityOwner, PBEBattlePokemon pokemon2, PBEAbility ability, PBEAbilityAction abilityAction)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (AbilityOwner = abilityOwner).ToBytes_Position(w);
                (Pokemon2 = pokemon2).ToBytes_Position(w);
                w.Write(Ability = ability);
                w.Write(AbilityAction = abilityAction);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEAbilityPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            AbilityOwner = battle.GetPokemon_Position(r);
            Pokemon2 = battle.GetPokemon_Position(r);
            Ability = r.ReadEnum<PBEAbility>();
            AbilityAction = r.ReadEnum<PBEAbilityAction>();
        }
    }
}
