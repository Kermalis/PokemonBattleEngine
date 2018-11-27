using Ether.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PMagnitudePacket : INetPacket
    {
        public const short Code = 0x20;
        public IEnumerable<byte> Buffer { get; }

        public readonly byte Magnitude;

        public PMagnitudePacket(byte magnitude)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(Magnitude = magnitude);
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PMagnitudePacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Magnitude = r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
