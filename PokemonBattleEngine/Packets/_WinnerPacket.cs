using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEWinnerPacket : INetPacket
    {
        public const short Code = 0x26;
        public IEnumerable<byte> Buffer { get; }

        public PBETeam WinningTeam { get; }

        public PBEWinnerPacket(PBETeam winningTeam)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((WinningTeam = winningTeam).Id);
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEWinnerPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                WinningTeam = battle.Teams[r.ReadByte()];
            }
        }

        public void Dispose() { }
    }
}
