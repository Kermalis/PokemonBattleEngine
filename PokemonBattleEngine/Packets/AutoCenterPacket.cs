using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEAutoCenterPacket : INetPacket
    {
        public const short Code = 0x2A;
        public IEnumerable<byte> Buffer { get; }

        public byte Pokemon1Id { get; }
        public PBEFieldPosition Pokemon1Position { get; }
        public PBETeam Pokemon1Team { get; }
        public byte Pokemon2Id { get; }
        public PBEFieldPosition Pokemon2Position { get; }
        public PBETeam Pokemon2Team { get; }

        public PBEAutoCenterPacket(byte pokemon1Id, PBEFieldPosition pokemon1Position, PBETeam pokemon1Team, byte pokemon2Id, PBEFieldPosition pokemon2Position, PBETeam pokemon2Team)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(Pokemon1Id = pokemon1Id);
            bytes.Add((byte)(Pokemon1Position = pokemon1Position));
            bytes.Add((Pokemon1Team = pokemon1Team).Id);
            bytes.Add(Pokemon2Id = pokemon2Id);
            bytes.Add((byte)(Pokemon2Position = pokemon2Position));
            bytes.Add((Pokemon2Team = pokemon2Team).Id);
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEAutoCenterPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Pokemon1Id = r.ReadByte();
                Pokemon1Position = (PBEFieldPosition)r.ReadByte();
                Pokemon1Team = battle.Teams[r.ReadByte()];
                Pokemon2Id = r.ReadByte();
                Pokemon2Position = (PBEFieldPosition)r.ReadByte();
                Pokemon2Team = battle.Teams[r.ReadByte()];
            }
        }

        public void Dispose() { }
    }
}
