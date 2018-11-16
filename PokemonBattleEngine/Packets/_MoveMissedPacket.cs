using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PMoveMissedPacket : INetPacket
    {
        public const short Code = 0x0D;
        public IEnumerable<byte> Buffer { get; }

        public readonly byte PokemonId;

        public PMoveMissedPacket(PPokemon pkmn)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(PokemonId = pkmn.Id);
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PMoveMissedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
