using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnFaintedPacket : INetPacket
    {
        public const short Code = 0x0E;
        public IEnumerable<byte> Buffer { get; }

        public byte VictimId { get; }

        public PBEPkmnFaintedPacket(PBEPokemon victim)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(VictimId = victim.Id);
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEPkmnFaintedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                VictimId = r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
