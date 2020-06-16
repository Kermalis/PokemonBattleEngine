using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnHPChangedPacket : IPBEPacket
    {
        public const ushort Code = 0x0A;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFieldPosition Pokemon { get; }
        public PBETeam PokemonTeam { get; }
        public ushort OldHP { get; }
        public ushort NewHP { get; }
        public double OldHPPercentage { get; }
        public double NewHPPercentage { get; }

        private PBEPkmnHPChangedPacket(PBEFieldPosition pokemon, PBETeam pokemonTeam, ushort oldHP, ushort newHP, double oldHPPercentage, double newHPPercentage)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(Pokemon = pokemon);
                w.Write((PokemonTeam = pokemonTeam).Id);
                w.Write(OldHP = oldHP);
                w.Write(NewHP = newHP);
                w.Write(OldHPPercentage = oldHPPercentage);
                w.Write(NewHPPercentage = newHPPercentage);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnHPChangedPacket(PBEBattlePokemon pokemon, ushort oldHP, double oldHPPercentage)
            : this(pokemon.FieldPosition, pokemon.Team, oldHP, pokemon.HP, oldHPPercentage, pokemon.HPPercentage) { }
        internal PBEPkmnHPChangedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon = r.ReadEnum<PBEFieldPosition>();
            PokemonTeam = battle.Teams[r.ReadByte()];
            OldHP = r.ReadUInt16();
            NewHP = r.ReadUInt16();
            OldHPPercentage = r.ReadDouble();
            NewHPPercentage = r.ReadDouble();
        }

        public PBEPkmnHPChangedPacket MakeHidden()
        {
            return new PBEPkmnHPChangedPacket(Pokemon, PokemonTeam, ushort.MinValue, ushort.MinValue, OldHPPercentage, NewHPPercentage);
        }
    }
}
