using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
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

        internal PBEPkmnFormChangedPacket(PBEBattlePokemon pokemon, bool isRevertForm)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(Pokemon = pokemon.FieldPosition);
                w.Write((PokemonTeam = pokemon.Team).Id);
                w.Write(NewAttack = pokemon.Attack);
                w.Write(NewDefense = pokemon.Defense);
                w.Write(NewSpAttack = pokemon.SpAttack);
                w.Write(NewSpDefense = pokemon.SpDefense);
                w.Write(NewSpeed = pokemon.Speed);
                w.Write(NewAbility = pokemon.Ability);
                w.Write(NewKnownAbility = pokemon.KnownAbility);
                w.Write(NewForm = pokemon.Form);
                w.Write(NewType1 = pokemon.Type1);
                w.Write(NewType2 = pokemon.Type2);
                w.Write(NewWeight = pokemon.Weight);
                w.Write(IsRevertForm = isRevertForm);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
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
    }
    public sealed class PBEPkmnFormChangedPacket_Hidden : IPBEPacket
    {
        public const ushort Code = 0x34;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFieldPosition Pokemon { get; }
        public PBETeam PokemonTeam { get; }
        public PBEAbility NewAbility { get; }
        public PBEAbility NewKnownAbility { get; }
        public PBEForm NewForm { get; }
        public PBEType NewType1 { get; }
        public PBEType NewType2 { get; }
        public double NewWeight { get; }
        public bool IsRevertForm { get; }

        public PBEPkmnFormChangedPacket_Hidden(PBEPkmnFormChangedPacket other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(Pokemon = other.Pokemon);
                w.Write((PokemonTeam = other.PokemonTeam).Id);
                w.Write(NewAbility = other.NewKnownAbility != PBEAbility.MAX ? other.NewAbility : PBEAbility.MAX);
                w.Write(NewKnownAbility = other.NewKnownAbility);
                w.Write(NewForm = other.NewForm);
                w.Write(NewType1 = other.NewType1);
                w.Write(NewType2 = other.NewType2);
                w.Write(NewWeight = other.NewWeight);
                w.Write(IsRevertForm = other.IsRevertForm);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnFormChangedPacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon = r.ReadEnum<PBEFieldPosition>();
            PokemonTeam = battle.Teams[r.ReadByte()];
            NewAbility = r.ReadEnum<PBEAbility>();
            NewKnownAbility = r.ReadEnum<PBEAbility>();
            NewForm = r.ReadEnum<PBEForm>();
            NewType1 = r.ReadEnum<PBEType>();
            NewType2 = r.ReadEnum<PBEType>();
            NewWeight = r.ReadDouble();
        }
    }
}
