using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEAbilityReplacedPacket : IPBEPacket
    {
        public const ushort Code = 0x2C;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon AbilityOwner { get; }
        public PBEAbility? OldAbility { get; }
        public PBEAbility NewAbility { get; }

        internal PBEAbilityReplacedPacket(PBEBattlePokemon abilityOwner, PBEAbility? oldAbility, PBEAbility newAbility)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (AbilityOwner = abilityOwner).ToBytes_Position(w);
                w.Write(oldAbility.HasValue);
                if (oldAbility.HasValue)
                {
                    w.Write((OldAbility = oldAbility).Value);
                }
                w.Write(NewAbility = newAbility);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEAbilityReplacedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            AbilityOwner = battle.GetPokemon_Position(r);
            if (r.ReadBoolean())
            {
                OldAbility = r.ReadEnum<PBEAbility>();
            }
            NewAbility = r.ReadEnum<PBEAbility>();
        }
    }
}
