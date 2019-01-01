using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMagnitudePacket : INetPacket
    {
        public const short Code = 0x20;
        public IEnumerable<byte> Buffer { get; }

        public byte Magnitude { get; }

        public PBEMagnitudePacket(byte magnitude)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(Magnitude = magnitude);
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEMagnitudePacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Magnitude = r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
