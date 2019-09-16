using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEAutoCenterPacket : INetPacket
    {
        public const short Code = 0x2A;
        public ReadOnlyCollection<byte> Buffer { get; }

        public byte Pokemon1Id { get; }
        public PBEFieldPosition Pokemon1Position { get; }
        public PBETeam Pokemon1Team { get; }
        public byte Pokemon2Id { get; }
        public PBEFieldPosition Pokemon2Position { get; }
        public PBETeam Pokemon2Team { get; }

        internal PBEAutoCenterPacket(byte pokemon1Id, PBEFieldPosition pokemon1Position, PBETeam pokemon1Team, byte pokemon2Id, PBEFieldPosition pokemon2Position, PBETeam pokemon2Team)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(Pokemon1Id = pokemon1Id);
            bytes.Add((byte)(Pokemon1Position = pokemon1Position));
            bytes.Add((Pokemon1Team = pokemon1Team).Id);
            bytes.Add(Pokemon2Id = pokemon2Id);
            bytes.Add((byte)(Pokemon2Position = pokemon2Position));
            bytes.Add((Pokemon2Team = pokemon2Team).Id);
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEAutoCenterPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            Pokemon1Id = r.ReadByte();
            Pokemon1Position = (PBEFieldPosition)r.ReadByte();
            Pokemon1Team = battle.Teams[r.ReadByte()];
            Pokemon2Id = r.ReadByte();
            Pokemon2Position = (PBEFieldPosition)r.ReadByte();
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

        public void Dispose() { }
    }
}
