using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveUsedPacket : INetPacket
    {
        public const short Code = 0x09;
        public IEnumerable<byte> Buffer { get; }

        public byte CulpritId { get; }
        public PBEMove Move { get; }
        public bool OwnsMove { get; }

        public PBEMoveUsedPacket(PBEPokemon culprit, PBEMove move)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(CulpritId = culprit.Id);
            bytes.AddRange(BitConverter.GetBytes((ushort)(Move = move)));
            bytes.Add((byte)((OwnsMove = culprit.Moves.Contains(Move)) ? 1 : 0));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEMoveUsedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                CulpritId = r.ReadByte();
                Move = (PBEMove)r.ReadUInt16();
                OwnsMove = r.ReadBoolean();
            }
        }

        public void Dispose() { }
    }
}
