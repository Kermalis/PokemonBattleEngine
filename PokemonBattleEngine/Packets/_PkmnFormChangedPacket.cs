using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnFormChangedPacket : IPBEPacket
    {
        public const ushort Code = 0x29;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFieldPosition Pokemon { get; }
        public PBETeam PokemonTeam { get; }
        public ushort NewAttack { get; }
        public ushort NewDefense { get; }
        public ushort NewSpAttack { get; }
        public ushort NewSpDefense { get; }
        public ushort NewSpeed { get; }
        public PBEAbility NewAbility { get; }
        public PBEAbility NewKnownAbility { get; }
        public PBEForm NewForm { get; }
        public PBEType NewType1 { get; }
        public PBEType NewType2 { get; }
        public double NewWeight { get; }
        public bool IsRevertForm { get; }

        private PBEPkmnFormChangedPacket(PBEFieldPosition pokemonPosition, PBETeam pokemonTeam, ushort newAttack, ushort newDefense, ushort newSpAttack, ushort newSpDefense, ushort newSpeed,
            PBEAbility newAbility, PBEAbility newKnownAbility, PBEForm newForm, PBEType newType1, PBEType newType2, double newWeight, bool isRevertForm)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(Pokemon = pokemonPosition);
                w.Write((PokemonTeam = pokemonTeam).Id);
                w.Write(NewAttack = newAttack);
                w.Write(NewDefense = newDefense);
                w.Write(NewSpAttack = newSpAttack);
                w.Write(NewSpDefense = newSpDefense);
                w.Write(NewSpeed = newSpeed);
                w.Write(NewAbility = newAbility);
                w.Write(NewKnownAbility = newKnownAbility);
                w.Write(NewForm = newForm);
                w.Write(NewType1 = newType1);
                w.Write(NewType2 = newType2);
                w.Write(NewWeight = newWeight);
                w.Write(IsRevertForm = isRevertForm);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnFormChangedPacket(PBEBattlePokemon pokemon, bool isRevertForm)
            : this(pokemon.FieldPosition, pokemon.Team, pokemon.Attack, pokemon.Defense, pokemon.SpAttack, pokemon.SpDefense, pokemon.Speed, pokemon.Ability, pokemon.KnownAbility, pokemon.Form, pokemon.Type1, pokemon.Type2, pokemon.Weight, isRevertForm) { }
        internal PBEPkmnFormChangedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon = r.ReadEnum<PBEFieldPosition>();
            PokemonTeam = battle.Teams[r.ReadByte()];
            NewAttack = r.ReadUInt16();
            NewDefense = r.ReadUInt16();
            NewSpAttack = r.ReadUInt16();
            NewSpDefense = r.ReadUInt16();
            NewSpeed = r.ReadUInt16();
            NewAbility = r.ReadEnum<PBEAbility>();
            NewKnownAbility = r.ReadEnum<PBEAbility>();
            NewForm = r.ReadEnum<PBEForm>();
            NewType1 = r.ReadEnum<PBEType>();
            NewType2 = r.ReadEnum<PBEType>();
            NewWeight = r.ReadDouble();
        }

        public PBEPkmnFormChangedPacket MakeHidden()
        {
            return new PBEPkmnFormChangedPacket(Pokemon, PokemonTeam, ushort.MinValue, ushort.MinValue, ushort.MinValue, ushort.MinValue, ushort.MinValue, NewKnownAbility != PBEAbility.MAX ? NewAbility : PBEAbility.MAX, NewKnownAbility, NewForm, NewType1, NewType2, NewWeight, IsRevertForm);
        }
    }
}
