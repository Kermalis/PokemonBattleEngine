using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public interface IPBEPkmnHPChangedPacket : IPBEPacket
    {
        PBETrainer PokemonTrainer { get; }
        PBEFieldPosition Pokemon { get; }
        float OldHPPercentage { get; }
        float NewHPPercentage { get; }
    }
    public sealed class PBEPkmnHPChangedPacket : IPBEPkmnHPChangedPacket
    {
        public const ushort Code = 0x0A;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer PokemonTrainer { get; }
        public PBEFieldPosition Pokemon { get; }
        public ushort OldHP { get; }
        public ushort NewHP { get; }
        public float OldHPPercentage { get; }
        public float NewHPPercentage { get; }

        internal PBEPkmnHPChangedPacket(PBEBattlePokemon pokemon, ushort oldHP, float oldHPPercentage)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((PokemonTrainer = pokemon.Trainer).Id);
                w.Write(Pokemon = pokemon.FieldPosition);
                w.Write(OldHP = oldHP);
                w.Write(NewHP = pokemon.HP);
                w.Write(OldHPPercentage = oldHPPercentage);
                w.Write(NewHPPercentage = pokemon.HPPercentage);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnHPChangedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            PokemonTrainer = battle.Trainers[r.ReadByte()];
            Pokemon = r.ReadEnum<PBEFieldPosition>();
            OldHP = r.ReadUInt16();
            NewHP = r.ReadUInt16();
            OldHPPercentage = r.ReadSingle();
            NewHPPercentage = r.ReadSingle();
        }
    }
    public sealed class PBEPkmnHPChangedPacket_Hidden : IPBEPkmnHPChangedPacket
    {
        public const ushort Code = 0x35;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer PokemonTrainer { get; }
        public PBEFieldPosition Pokemon { get; }
        public float OldHPPercentage { get; }
        public float NewHPPercentage { get; }

        public PBEPkmnHPChangedPacket_Hidden(PBEPkmnHPChangedPacket other)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((PokemonTrainer = other.PokemonTrainer).Id);
                w.Write(Pokemon = other.Pokemon);
                w.Write(OldHPPercentage = other.OldHPPercentage);
                w.Write(NewHPPercentage = other.NewHPPercentage);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnHPChangedPacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            PokemonTrainer = battle.Trainers[r.ReadByte()];
            Pokemon = r.ReadEnum<PBEFieldPosition>();
            OldHPPercentage = r.ReadSingle();
            NewHPPercentage = r.ReadSingle();
        }
    }
}
