using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PReflectLightScreenPacket : INetPacket
    {
        public const short Code = 0x13;
        public IEnumerable<byte> Buffer => BuildBuffer();
        
        public bool Local;
        public readonly bool Reflect; // False for Light Screen
        public readonly PReflectLightScreenAction Action;

        public PReflectLightScreenPacket(bool local, bool reflect, PReflectLightScreenAction action)
        {
            Local = local;
            Reflect = reflect;
            Action = action;
        }
        public PReflectLightScreenPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Local = r.ReadBoolean();
                Reflect = r.ReadBoolean();
                Action = (PReflectLightScreenAction)r.ReadByte();
            }
        }
        IEnumerable<byte> BuildBuffer()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Local ? 1 : 0));
            bytes.Add((byte)(Reflect ? 1 : 0));
            bytes.Add((byte)Action);
            return BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }

        public void Dispose() { }
    }
}
