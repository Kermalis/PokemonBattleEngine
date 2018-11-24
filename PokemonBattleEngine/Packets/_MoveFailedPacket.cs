using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PMoveFailedPacket : INetPacket
    {
        public const short Code = 0x15;
        public IEnumerable<byte> Buffer { get; }

        public readonly byte CulpritId;
        public readonly PFailReason Reason;

        public PMoveFailedPacket(PPokemon culprit, PFailReason reason)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(CulpritId = culprit.Id);
            bytes.Add((byte)(Reason = reason));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PMoveFailedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                CulpritId = r.ReadByte();
                Reason = (PFailReason)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
