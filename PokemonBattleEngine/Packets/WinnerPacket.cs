using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEWinnerPacket : INetPacket
    {
        public const short Code = 0x26;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBETeam WinningTeam { get; }

        internal PBEWinnerPacket(PBETeam winningTeam)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((WinningTeam = winningTeam).Id);
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEWinnerPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            WinningTeam = battle.Teams[r.ReadByte()];
        }

        public void Dispose() { }
    }
}
