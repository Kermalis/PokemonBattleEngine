using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEIllusionPacket : IPBEPacket
    {
        public const ushort Code = 0x25;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFieldPosition Pokemon { get; }
        public PBETeam PokemonTeam { get; }
        public PBEGender ActualGender { get; }
        public bool ActualShiny { get; }
        public string ActualNickname { get; }
        public PBESpecies ActualSpecies { get; }
        public PBEForm ActualForm { get; }
        public PBEType ActualType1 { get; }
        public PBEType ActualType2 { get; }
        public double ActualWeight { get; }

        internal PBEIllusionPacket(PBEPokemon pokemon)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(Pokemon = pokemon.FieldPosition);
                w.Write((PokemonTeam = pokemon.Team).Id);
                w.Write(ActualGender = pokemon.Gender);
                w.Write(ActualNickname = pokemon.Nickname, true);
                w.Write(ActualShiny = pokemon.Shiny);
                w.Write(ActualSpecies = pokemon.Species);
                w.Write(ActualForm = pokemon.Form);
                w.Write(ActualType1 = pokemon.Type1);
                w.Write(ActualType2 = pokemon.Type2);
                w.Write(ActualWeight = pokemon.Weight);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEIllusionPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon = r.ReadEnum<PBEFieldPosition>();
            PokemonTeam = battle.Teams[r.ReadByte()];
            ActualGender = r.ReadEnum<PBEGender>();
            ActualNickname = r.ReadStringNullTerminated();
            ActualShiny = r.ReadBoolean();
            ActualSpecies = r.ReadEnum<PBESpecies>();
            ActualForm = r.ReadEnum<PBEForm>();
            ActualType1 = r.ReadEnum<PBEType>();
            ActualType2 = r.ReadEnum<PBEType>();
            ActualWeight = r.ReadDouble();
        }
    }
}
