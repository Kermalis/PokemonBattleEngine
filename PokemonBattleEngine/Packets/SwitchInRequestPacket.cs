using Ether.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PSwitchInRequestPacket : INetPacket
    {
        public const short Code = 0x23;
        public IEnumerable<byte> Buffer => BuildBuffer();

        public bool Local;
        public readonly byte Amount;

        public PSwitchInRequestPacket(bool local, byte amount)
        {
            Local = local;
            Amount = amount;
        }
        public PSwitchInRequestPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Local = r.ReadBoolean();
                Amount = r.ReadByte();
            }
        }
        IEnumerable<byte> BuildBuffer()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Local ? 1 : 0));
            bytes.Add(Amount);
            return BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }

        public void Dispose() { }
    }
}
