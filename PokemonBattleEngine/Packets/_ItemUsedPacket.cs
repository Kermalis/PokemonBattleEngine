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

        public readonly Guid PokemonId;
        public readonly PItem Item;

        public PItemUsedPacket(PPokemon pkmn)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange((PokemonId = pkmn.Id).ToByteArray());
            bytes.AddRange(BitConverter.GetBytes((ushort)(Item = pkmn.Shell.Item)));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PItemUsedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = new Guid(r.ReadBytes(0x10));
                Item = (PItem)r.ReadUInt16();
            }
        }

        public void Dispose() { }
    }
}
