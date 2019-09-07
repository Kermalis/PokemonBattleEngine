using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPartyResponsePacket : INetPacket
    {
        public const short Code = 0x04;
        public IEnumerable<byte> Buffer { get; }

        public PBETeamShell TeamShell { get; }

        public PBEPartyResponsePacket(PBETeamShell teamShell)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange((TeamShell = teamShell).ToBytes());
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEPartyResponsePacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code                               
                TeamShell = new PBETeamShell(r); // What happens if an exception occurs? Similar question to https://github.com/Kermalis/PokemonBattleEngine/issues/167
            }
        }

        public void Dispose() { }
    }
}
