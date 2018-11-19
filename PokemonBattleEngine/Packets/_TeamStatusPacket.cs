using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PTeamStatusPacket : INetPacket
    {
        public const short Code = 0x13;
        public IEnumerable<byte> Buffer => BuildBuffer();

        public bool Local;
        public readonly PTeamStatus Status;
        public readonly PTeamStatusAction Action;
        public readonly byte VictimId; // Victim of PTeamStatusAction.CausedDamage

        public PTeamStatusPacket(bool local, PTeamStatus status, PTeamStatusAction action, byte victimId)
        {
            Local = local;
            Status = status;
            Action = action;
            VictimId = victimId;
        }
        public PTeamStatusPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Local = r.ReadBoolean();
                Status = (PTeamStatus)r.ReadByte();
                Action = (PTeamStatusAction)r.ReadByte();
                VictimId = r.ReadByte();
            }
        }
        IEnumerable<byte> BuildBuffer()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Local ? 1 : 0));
            bytes.Add((byte)Status);
            bytes.Add((byte)Action);
            bytes.Add(VictimId);
            return BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }

        public void Dispose() { }
    }
}
