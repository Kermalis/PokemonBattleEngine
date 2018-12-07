using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBESwitchInResponsePacket : INetPacket
    {
        public const short Code = 0x24;
        public IEnumerable<byte> Buffer { get; }

        public Tuple<byte, PBEFieldPosition>[] Switches { get; } // Pokemon ID, Field Position

        public PBESwitchInResponsePacket(IEnumerable<Tuple<byte, PBEFieldPosition>> switches)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Switches = switches.ToArray()).Length);
            foreach (var t in Switches)
            {
                bytes.Add(t.Item1);
                bytes.Add((byte)t.Item2);
            }
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBESwitchInResponsePacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                byte numActions = r.ReadByte();
                Switches = new Tuple<byte, PBEFieldPosition>[numActions];
                for (int i = 0; i < numActions; i++)
                {
                    Switches[i] = Tuple.Create(r.ReadByte(), (PBEFieldPosition)r.ReadByte());
                }
            }
        }

        public void Dispose() { }
    }
}
