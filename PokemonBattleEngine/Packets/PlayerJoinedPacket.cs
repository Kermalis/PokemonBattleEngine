using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            bytes.AddRange(PUtils.StringToBytes(DisplayName = name));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PPlayerJoinedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PlayerId = new Guid(r.ReadBytes(0x10));
                DisplayName = PUtils.StringFromBytes(r);
            }
        }

        public void Dispose() { }
    }
}
