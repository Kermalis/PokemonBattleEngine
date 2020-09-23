using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public interface IPBEPkmnSwitchOutPacket : IPBEPacket
    {
        PBETrainer PokemonTrainer { get; }
        PBEFieldPosition OldPosition { get; }
        bool Forced { get; }
        PBETrainer ForcedByPokemonTrainer { get; }
        PBEFieldPosition ForcedByPokemon { get; }
    }
    public sealed class PBEPkmnSwitchOutPacket : IPBEPkmnSwitchOutPacket
    {
        public const ushort Code = 0x0C;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer PokemonTrainer { get; }
        public byte Pokemon { get; }
        public PBEFieldPosition OldPosition { get; }
        public bool Forced { get; }
        public PBETrainer ForcedByPokemonTrainer { get; }
        public PBEFieldPosition ForcedByPokemon { get; }

        internal PBEPkmnSwitchOutPacket(PBEBattlePokemon pokemon, PBEFieldPosition oldPosition, PBEBattlePokemon forcedByPokemon = null)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((PokemonTrainer = pokemon.Trainer).Id);
                w.Write(Pokemon = pokemon.Id);
                w.Write(OldPosition = oldPosition);
                w.Write(Forced = forcedByPokemon != null);
                if (Forced)
                {
                    w.Write((ForcedByPokemonTrainer = forcedByPokemon.Trainer).Id);
                    w.Write(ForcedByPokemon = forcedByPokemon.FieldPosition);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnSwitchOutPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            PokemonTrainer = battle.Trainers[r.ReadByte()];
            Pokemon = r.ReadByte();
            OldPosition = r.ReadEnum<PBEFieldPosition>();
            Forced = r.ReadBoolean();
            if (Forced)
            {
                ForcedByPokemonTrainer = battle.Trainers[r.ReadByte()];
                ForcedByPokemon = r.ReadEnum<PBEFieldPosition>();
            }
        }
    }
    public sealed class PBEPkmnSwitchOutPacket_Hidden : IPBEPkmnSwitchOutPacket
    {
        public const ushort Code = 0x37;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer PokemonTrainer { get; }
        public PBEFieldPosition OldPosition { get; }
        public bool Forced { get; }
        public PBETrainer ForcedByPokemonTrainer { get; }
        public PBEFieldPosition ForcedByPokemon { get; }

        public PBEPkmnSwitchOutPacket_Hidden(PBEPkmnSwitchOutPacket other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((PokemonTrainer = other.PokemonTrainer).Id);
                w.Write(OldPosition = other.OldPosition);
                w.Write(Forced = other.Forced);
                if (Forced)
                {
                    w.Write((ForcedByPokemonTrainer = other.ForcedByPokemonTrainer).Id);
                    w.Write(ForcedByPokemon = other.ForcedByPokemon);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnSwitchOutPacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            PokemonTrainer = battle.Trainers[r.ReadByte()];
            OldPosition = r.ReadEnum<PBEFieldPosition>();
            Forced = r.ReadBoolean();
            if (Forced)
            {
                ForcedByPokemonTrainer = battle.Trainers[r.ReadByte()];
                ForcedByPokemon = r.ReadEnum<PBEFieldPosition>();
            }
        }
    }
}
