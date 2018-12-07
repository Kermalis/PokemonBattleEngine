using Ether.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBESwitchInRequestPacket : INetPacket
    {
        public const short Code = 0x23;
        public IEnumerable<byte> Buffer => BuildBuffer();

        public bool LocalTeam { get; set; }
        public byte Amount { get; }

        public PBESwitchInRequestPacket(bool localTeam, byte amount)
        {
            LocalTeam = localTeam;
            Amount = amount;
        }
        public PBESwitchInRequestPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                LocalTeam = r.ReadBoolean();
                Amount = r.ReadByte();
            }
        }
        IEnumerable<byte> BuildBuffer()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(LocalTeam ? 1 : 0));
            bytes.Add(Amount);
            return BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }

        public void Dispose() { }
    }
}
