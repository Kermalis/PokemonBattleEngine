using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveFailedPacket : INetPacket
    {
        public const short Code = 0x15;
        public IEnumerable<byte> Buffer { get; }

        public byte CulpritId { get; }
        public byte VictimId { get; }
        public PBEFailReason FailReason { get; }

        public PBEMoveFailedPacket(PBEPokemon culprit, PBEPokemon victim, PBEFailReason failReason)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(CulpritId = culprit.Id);
            bytes.Add(VictimId = victim.Id);
            bytes.Add((byte)(FailReason = failReason));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEMoveFailedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                CulpritId = r.ReadByte();
                VictimId = r.ReadByte();
                FailReason = (PBEFailReason)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
