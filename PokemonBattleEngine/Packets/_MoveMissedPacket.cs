using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    // TODO: Include victimID for "victim evaded the attack!"
    public sealed class PMoveMissedPacket : INetPacket
    {
        public const short Code = 0x0D;
        public IEnumerable<byte> Buffer { get; }

        public readonly byte CulpritId;

        public PMoveMissedPacket(PPokemon culprit)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(CulpritId = culprit.Id);
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PMoveMissedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                CulpritId = r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
