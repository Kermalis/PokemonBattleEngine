using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBETurnBeganPacket : INetPacket
    {
        public const short Code = 0x27;
        public IEnumerable<byte> Buffer { get; }

        public ushort TurnNumber { get; }

        public PBETurnBeganPacket(ushort turnNumber)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange(BitConverter.GetBytes(TurnNumber = turnNumber));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBETurnBeganPacket(byte[] buffer, PBEBattle battle)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                TurnNumber = r.ReadUInt16();
            }
        }

        public void Dispose() { }
    }
}
