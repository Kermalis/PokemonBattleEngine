using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBESwitchInRequestPacket : INetPacket
    {
        public const short Code = 0x23;
        public IEnumerable<byte> Buffer { get; }

        public PBETeam Team { get; }
        public byte Amount { get; }

        public PBESwitchInRequestPacket(PBETeam team)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((Team = team).Id);
            bytes.Add(Amount = Team.SwitchInsRequired);
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBESwitchInRequestPacket(byte[] buffer, PBEBattle battle)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Team = battle.Teams[r.ReadByte()];
                Amount = r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
