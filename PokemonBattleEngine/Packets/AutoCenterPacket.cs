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

        private PBEAutoCenterPacket(byte pokemon1Id, PBEFieldPosition pokemon1Position, PBETeam pokemon1Team, byte pokemon2Id, PBEFieldPosition pokemon2Position, PBETeam pokemon2Team)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(Pokemon1Id = pokemon1Id);
                w.Write(Pokemon1Position = pokemon1Position);
                w.Write((Pokemon1Team = pokemon1Team).Id);
                w.Write(Pokemon2Id = pokemon2Id);
                w.Write(Pokemon2Position = pokemon2Position);
                w.Write((Pokemon2Team = pokemon2Team).Id);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEAutoCenterPacket(PBEBattlePokemon pokemon1, PBEFieldPosition pokemon1OldPosition, PBEBattlePokemon pokemon2, PBEFieldPosition pokemon2OldPosition)
            : this(pokemon1.Id, pokemon1OldPosition, pokemon1.Team, pokemon2.Id, pokemon2OldPosition, pokemon2.Team) { }
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

        public PBEAutoCenterPacket MakeHidden(bool team0Hidden, bool team1Hidden)
        {
            if (!team0Hidden && !team1Hidden)
            {
                throw new ArgumentException();
            }
            return new PBEAutoCenterPacket(team0Hidden ? byte.MaxValue : Pokemon1Id, Pokemon1Position, Pokemon1Team, team1Hidden ? byte.MaxValue : Pokemon2Id, Pokemon2Position, Pokemon2Team);
        }
    }
}
