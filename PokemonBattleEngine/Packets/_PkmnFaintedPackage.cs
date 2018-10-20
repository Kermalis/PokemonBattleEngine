using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PPkmnFaintedPacket : INetPacket
    {
        public const short Code = 0x0E;
        public IEnumerable<byte> Buffer { get; }

        public readonly Guid PokemonId;

        public PPkmnFaintedPacket(PPokemon pkmn)
        {
            PokemonId = pkmn.Id;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange(PokemonId.ToByteArray());
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PPkmnFaintedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = new Guid(r.ReadBytes(0x10));
            }
        }
        
        public void Dispose() { }
    }
}
