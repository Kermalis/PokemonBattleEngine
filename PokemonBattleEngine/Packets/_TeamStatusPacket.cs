using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBETeamStatusPacket : INetPacket
    {
        public const short Code = 0x13;
        public IEnumerable<byte> Buffer => BuildBuffer();

        public bool LocalTeam { get; set; }
        public PBETeamStatus TeamStatus { get; }
        public PBETeamStatusAction TeamStatusAction { get; }
        public byte VictimId { get; } // Victim of PBETeamStatusAction.CausedDamage

        public PBETeamStatusPacket(bool localTeam, PBETeamStatus teamStatus, PBETeamStatusAction teamStatusAction, byte victimId) // TODO: Change victimId to a PPokemon and have null checks (byte? VictimId)
        {
            LocalTeam = localTeam;
            TeamStatus = teamStatus;
            TeamStatusAction = teamStatusAction;
            VictimId = victimId;
        }
        public PBETeamStatusPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                LocalTeam = r.ReadBoolean();
                TeamStatus = (PBETeamStatus)r.ReadByte();
                TeamStatusAction = (PBETeamStatusAction)r.ReadByte();
                VictimId = r.ReadByte();
            }
        }
        IEnumerable<byte> BuildBuffer()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(LocalTeam ? 1 : 0));
            bytes.Add((byte)TeamStatus);
            bytes.Add((byte)TeamStatusAction);
            bytes.Add(VictimId);
            return BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }

        public void Dispose() { }
    }
}
