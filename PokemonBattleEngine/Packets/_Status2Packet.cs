using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PStatus2Packet : INetPacket
    {
        public const short Code = 0x12;
        public IEnumerable<byte> Buffer { get; }

        public readonly byte CulpritId, VictimId;
        public readonly PStatus2 Status;
        public readonly PStatusAction Action;

        public PStatus2Packet(PPokemon culprit, PPokemon victim, PStatus2 status, PStatusAction action)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(CulpritId = culprit.Id);
            bytes.Add(VictimId = victim.Id);
            bytes.AddRange(BitConverter.GetBytes((uint)(Status = status)));
            bytes.Add((byte)(Action = action));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PStatus2Packet(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                CulpritId = r.ReadByte();
                VictimId = r.ReadByte();
                Status = (PStatus2)r.ReadUInt32();
                Action = (PStatusAction)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
