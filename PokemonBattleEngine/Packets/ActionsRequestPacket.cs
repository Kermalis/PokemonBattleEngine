using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEActionsRequestPacket : INetPacket
    {
        public const short Code = 0x07;
        public IEnumerable<byte> Buffer { get; }

        public PBETeam Team { get; }
        public ReadOnlyCollection<byte> Pokemon { get; }

        public PBEActionsRequestPacket(PBETeam team)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((Team = team).Id);
            bytes.Add((byte)(Pokemon = Team.ActionsRequired.Select(p => p.Id).ToList().AsReadOnly()).Count);
            bytes.AddRange(Pokemon);
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEActionsRequestPacket(byte[] buffer, PBEBattle battle)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Team = battle.Teams[r.ReadByte()];
                Pokemon = new ReadOnlyCollection<byte>(r.ReadBytes(r.ReadByte()));
            }
        }

        public void Dispose() { }
    }
}
