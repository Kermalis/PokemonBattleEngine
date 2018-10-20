using Ether.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PPlayerJoinedPacket : INetPacket
    {
        public const short Code = 0x01;
        public IEnumerable<byte> Buffer { get; }

        public readonly Guid PlayerId;
        public readonly string DisplayName;

        public PPlayerJoinedPacket(Guid playerId, string name)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange((PlayerId = playerId).ToByteArray());
            byte[] nameBytes = Encoding.ASCII.GetBytes(DisplayName = name);
            bytes.Add((byte)nameBytes.Length);
            bytes.AddRange(nameBytes);
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PPlayerJoinedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PlayerId = new Guid(r.ReadBytes(0x10));
                DisplayName = Encoding.ASCII.GetString(r.ReadBytes(r.ReadByte()));
            }
        }

        public void Dispose() { }
    }
}
