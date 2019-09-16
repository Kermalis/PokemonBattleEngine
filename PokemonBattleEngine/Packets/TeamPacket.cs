using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBETeamPacket : INetPacket
    {
        public const short Code = 0x05;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBETeam Team { get; }

        internal PBETeamPacket(PBETeam team)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((Team = team).Id);
            Team.ToBytes(bytes);
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBETeamPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            Team = battle.Teams[r.ReadByte()];
            Team.FromBytes(r);
        }

        public void Dispose() { }
    }
}
