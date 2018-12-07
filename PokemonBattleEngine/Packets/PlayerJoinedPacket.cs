using Ether.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPlayerJoinedPacket : INetPacket
    {
        public const short Code = 0x01;
        public IEnumerable<byte> Buffer { get; }

        public Guid PlayerId { get; }
        public string TrainerName { get; }

        public PBEPlayerJoinedPacket(Guid playerId, string trainerName)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange((PlayerId = playerId).ToByteArray());
            bytes.AddRange(PBEUtils.StringToBytes(TrainerName = trainerName));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEPlayerJoinedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PlayerId = new Guid(r.ReadBytes(0x10));
                TrainerName = PBEUtils.StringFromBytes(r);
            }
        }

        public void Dispose() { }
    }
}
