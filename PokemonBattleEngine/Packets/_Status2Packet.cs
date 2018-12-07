using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEStatus2Packet : INetPacket
    {
        public const short Code = 0x12;
        public IEnumerable<byte> Buffer { get; }

        public byte CulpritId { get; }
        public byte VictimId { get; }
        public PBEStatus2 Status2 { get; }
        public PBEStatusAction StatusAction { get; }

        public PBEStatus2Packet(PBEPokemon culprit, PBEPokemon victim, PBEStatus2 status2, PBEStatusAction statusAction)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(CulpritId = culprit.Id);
            bytes.Add(VictimId = victim.Id);
            bytes.AddRange(BitConverter.GetBytes((uint)(Status2 = status2)));
            bytes.Add((byte)(StatusAction = statusAction));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEStatus2Packet(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                CulpritId = r.ReadByte();
                VictimId = r.ReadByte();
                Status2 = (PBEStatus2)r.ReadUInt32();
                StatusAction = (PBEStatusAction)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
