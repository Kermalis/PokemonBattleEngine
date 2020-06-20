using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEAutoCenterPacket : IPBEPacket
    {
        public const ushort Code = 0x2A;
        public ReadOnlyCollection<byte> Data { get; }

        public byte Pokemon1Id { get; }
        public PBEFieldPosition Pokemon1Position { get; }
        public PBETeam Pokemon1Team { get; }
        public byte Pokemon2Id { get; }
        public PBEFieldPosition Pokemon2Position { get; }
        public PBETeam Pokemon2Team { get; }

        internal PBEAutoCenterPacket(PBEBattlePokemon pokemon1, PBEFieldPosition pokemon1OldPosition, PBEBattlePokemon pokemon2, PBEFieldPosition pokemon2OldPosition)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(Pokemon1Id = pokemon1.Id);
                w.Write(Pokemon1Position = pokemon1.FieldPosition);
                w.Write((Pokemon1Team = pokemon1.Team).Id);
                w.Write(Pokemon2Id = pokemon2.Id);
                w.Write(Pokemon2Position = pokemon2.FieldPosition);
                w.Write((Pokemon2Team = pokemon2.Team).Id);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEAutoCenterPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon1Id = r.ReadByte();
            Pokemon1Position = r.ReadEnum<PBEFieldPosition>();
            Pokemon1Team = battle.Teams[r.ReadByte()];
            Pokemon2Id = r.ReadByte();
            Pokemon2Position = r.ReadEnum<PBEFieldPosition>();
            Pokemon2Team = battle.Teams[r.ReadByte()];
        }
    }
    public sealed class PBEAutoCenterPacket_Hidden0 : IPBEPacket
    {
        public const ushort Code = 0x30;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFieldPosition Pokemon1Position { get; }
        public PBETeam Pokemon1Team { get; }
        public byte Pokemon2Id { get; }
        public PBEFieldPosition Pokemon2Position { get; }
        public PBETeam Pokemon2Team { get; }

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
                w.Write(Pokemon1Position = other.Pokemon1Position);
                w.Write((Pokemon1Team = other.Pokemon1Team).Id);
                w.Write(Pokemon2Id = other.Pokemon2Id);
                w.Write(Pokemon2Position = other.Pokemon2Position);
                w.Write((Pokemon2Team = other.Pokemon2Team).Id);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEAutoCenterPacket_Hidden0(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon1Position = r.ReadEnum<PBEFieldPosition>();
            Pokemon1Team = battle.Teams[r.ReadByte()];
            Pokemon2Id = r.ReadByte();
            Pokemon2Position = r.ReadEnum<PBEFieldPosition>();
            Pokemon2Team = battle.Teams[r.ReadByte()];
        }
    }
    public sealed class PBEAutoCenterPacket_Hidden1 : IPBEPacket
    {
        public const ushort Code = 0x31;
        public ReadOnlyCollection<byte> Data { get; }

        public byte Pokemon1Id { get; }
        public PBEFieldPosition Pokemon1Position { get; }
        public PBETeam Pokemon1Team { get; }
        public PBEFieldPosition Pokemon2Position { get; }
        public PBETeam Pokemon2Team { get; }

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
                w.Write(Pokemon1Id = other.Pokemon1Id);
                w.Write(Pokemon1Position = other.Pokemon1Position);
                w.Write((Pokemon1Team = other.Pokemon1Team).Id);
                w.Write(Pokemon2Position = other.Pokemon2Position);
                w.Write((Pokemon2Team = other.Pokemon2Team).Id);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEAutoCenterPacket_Hidden1(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon1Id = r.ReadByte();
            Pokemon1Position = r.ReadEnum<PBEFieldPosition>();
            Pokemon1Team = battle.Teams[r.ReadByte()];
            Pokemon2Position = r.ReadEnum<PBEFieldPosition>();
            Pokemon2Team = battle.Teams[r.ReadByte()];
        }
    }
    public sealed class PBEAutoCenterPacket_Hidden01 : IPBEPacket
    {
        public const ushort Code = 0x32;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFieldPosition Pokemon1Position { get; }
        public PBETeam Pokemon1Team { get; }
        public PBEFieldPosition Pokemon2Position { get; }
        public PBETeam Pokemon2Team { get; }

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
                w.Write(Pokemon1Position = other.Pokemon1Position);
                w.Write((Pokemon1Team = other.Pokemon1Team).Id);
                w.Write(Pokemon2Position = other.Pokemon2Position);
                w.Write((Pokemon2Team = other.Pokemon2Team).Id);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEAutoCenterPacket_Hidden01(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon1Position = r.ReadEnum<PBEFieldPosition>();
            Pokemon1Team = battle.Teams[r.ReadByte()];
            Pokemon2Position = r.ReadEnum<PBEFieldPosition>();
            Pokemon2Team = battle.Teams[r.ReadByte()];
        }
    }
}
