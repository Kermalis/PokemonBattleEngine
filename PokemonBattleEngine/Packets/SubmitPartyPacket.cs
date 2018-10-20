using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PSubmitPartyPacket : INetPacket
    {
        public const short Code = 0x04;
        public IEnumerable<byte> Buffer { get; }

        public readonly PTeamShell Team;

        public PSubmitPartyPacket(PTeamShell team)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange((Team = team).ToBytes());
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PSubmitPartyPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Team = PTeamShell.FromBytes(r);
            }
        }

        public void Dispose() { }
    }
}
