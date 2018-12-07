using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEItemUsedPacket : INetPacket
    {
        public const short Code = 0x16;
        public IEnumerable<byte> Buffer { get; }

        public byte CulpritId { get; }
        public PBEItem Item { get; }

        public PBEItemUsedPacket(PBEPokemon culprit, PBEItem item)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(CulpritId = culprit.Id);
            bytes.AddRange(BitConverter.GetBytes((ushort)(Item = item)));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEItemUsedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                CulpritId = r.ReadByte();
                Item = (PBEItem)r.ReadUInt16();
            }
        }

        public void Dispose() { }
    }
}
