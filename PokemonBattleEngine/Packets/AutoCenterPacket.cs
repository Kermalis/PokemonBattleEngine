using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public interface IPBEAutoCenterPacket : IPBEPacket
    {
        PBEBattlePokemon Pokemon0 { get; }
        PBEFieldPosition Pokemon0OldPosition { get; }
        PBEBattlePokemon Pokemon1 { get; }
        PBEFieldPosition Pokemon1OldPosition { get; }
    }
    public sealed class PBEAutoCenterPacket : IPBEAutoCenterPacket
    {
        public const ushort Code = 0x2A;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon Pokemon0 { get; }
        public PBEFieldPosition Pokemon0OldPosition { get; }
        public PBEBattlePokemon Pokemon1 { get; }
        public PBEFieldPosition Pokemon1OldPosition { get; }

        internal PBEAutoCenterPacket(PBEBattlePokemon pokemon0, PBEFieldPosition pokemon0OldPosition, PBEBattlePokemon pokemon1, PBEFieldPosition pokemon1OldPosition)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (Pokemon0 = pokemon0).ToBytes_Id(w);
                w.Write(Pokemon0OldPosition = pokemon0OldPosition);
                (Pokemon1 = pokemon1).ToBytes_Id(w);
                w.Write(Pokemon1OldPosition = pokemon1OldPosition);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEAutoCenterPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon0 = battle.GetPokemon_Id(r);
            Pokemon0OldPosition = r.ReadEnum<PBEFieldPosition>();
            Pokemon1 = battle.GetPokemon_Id(r);
            Pokemon1OldPosition = r.ReadEnum<PBEFieldPosition>();
        }
    }
    public sealed class PBEAutoCenterPacket_Hidden0 : IPBEAutoCenterPacket
    {
        public const ushort Code = 0x30;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon Pokemon0 { get; }
        public PBEFieldPosition Pokemon0OldPosition { get; }
        public PBEBattlePokemon Pokemon1 { get; }
        public PBEFieldPosition Pokemon1OldPosition { get; }

        public PBEAutoCenterPacket_Hidden0(PBEAutoCenterPacket other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (Pokemon0 = other.Pokemon0).ToBytes_Position(w);
                w.Write(Pokemon0OldPosition = other.Pokemon0OldPosition);
                (Pokemon1 = other.Pokemon1).ToBytes_Id(w);
                w.Write(Pokemon1OldPosition = other.Pokemon1OldPosition);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEAutoCenterPacket_Hidden0(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon0 = battle.GetPokemon_Position(r);
            Pokemon0OldPosition = r.ReadEnum<PBEFieldPosition>();
            Pokemon1 = battle.GetPokemon_Id(r);
            Pokemon1OldPosition = r.ReadEnum<PBEFieldPosition>();
        }
    }
    public sealed class PBEAutoCenterPacket_Hidden1 : IPBEAutoCenterPacket
    {
        public const ushort Code = 0x31;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon Pokemon0 { get; }
        public PBEFieldPosition Pokemon0OldPosition { get; }
        public PBEBattlePokemon Pokemon1 { get; }
        public PBEFieldPosition Pokemon1OldPosition { get; }

        public PBEAutoCenterPacket_Hidden1(PBEAutoCenterPacket other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (Pokemon0 = other.Pokemon0).ToBytes_Id(w);
                w.Write(Pokemon0OldPosition = other.Pokemon0OldPosition);
                (Pokemon1 = other.Pokemon1).ToBytes_Position(w);
                w.Write(Pokemon1OldPosition = other.Pokemon1OldPosition);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEAutoCenterPacket_Hidden1(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon0 = battle.GetPokemon_Id(r);
            Pokemon0OldPosition = r.ReadEnum<PBEFieldPosition>();
            Pokemon1 = battle.GetPokemon_Position(r);
            Pokemon1OldPosition = r.ReadEnum<PBEFieldPosition>();
        }
    }
    public sealed class PBEAutoCenterPacket_Hidden01 : IPBEAutoCenterPacket
    {
        public const ushort Code = 0x32;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon Pokemon0 { get; }
        public PBEFieldPosition Pokemon0OldPosition { get; }
        public PBEBattlePokemon Pokemon1 { get; }
        public PBEFieldPosition Pokemon1OldPosition { get; }

        public PBEAutoCenterPacket_Hidden01(PBEAutoCenterPacket other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (Pokemon0 = other.Pokemon0).ToBytes_Position(w);
                w.Write(Pokemon0OldPosition = other.Pokemon0OldPosition);
                (Pokemon1 = other.Pokemon1).ToBytes_Position(w);
                w.Write(Pokemon1OldPosition = other.Pokemon1OldPosition);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEAutoCenterPacket_Hidden01(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon0 = battle.GetPokemon_Position(r);
            Pokemon0OldPosition = r.ReadEnum<PBEFieldPosition>();
            Pokemon1 = battle.GetPokemon_Position(r);
            Pokemon1OldPosition = r.ReadEnum<PBEFieldPosition>();
        }
    }
}
