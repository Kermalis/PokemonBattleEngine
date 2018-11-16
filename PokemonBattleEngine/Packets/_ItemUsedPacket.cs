using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PItemUsedPacket : INetPacket
    {
        public const short Code = 0x16;
        public IEnumerable<byte> Buffer { get; }

        public readonly byte PokemonId;
        public readonly PItem Item;

        public PItemUsedPacket(PPokemon pkmn, PItem item)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(PokemonId = pkmn.Id);
            bytes.AddRange(BitConverter.GetBytes((ushort)(Item = item)));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PItemUsedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = r.ReadByte();
                Item = (PItem)r.ReadUInt16();
            }
        }

        public void Dispose() { }
    }
}
