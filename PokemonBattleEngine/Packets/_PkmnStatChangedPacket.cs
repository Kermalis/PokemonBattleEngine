using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PPkmnStatChangedPacket : INetPacket
    {
        public const short Code = 0x10;
        public IEnumerable<byte> Buffer { get; }

        public readonly byte VictimId;
        public readonly PStat Stat;
        public readonly sbyte Change;
        public readonly bool IsTooMuch;

        public PPkmnStatChangedPacket(PPokemon victim, PStat stat, sbyte change, bool isTooMuch)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(VictimId = victim.Id);
            bytes.Add((byte)(Stat = stat));
            bytes.Add((byte)(Change = change));
            bytes.Add((byte)((IsTooMuch = isTooMuch) ? 1 : 0));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PPkmnStatChangedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                VictimId = r.ReadByte();
                Stat = (PStat)r.ReadByte();
                Change = r.ReadSByte();
                IsTooMuch = r.ReadBoolean();
            }
        }

        public void Dispose() { }
    }
}
