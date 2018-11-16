using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PMoveUsedPacket : INetPacket
    {
        public const short Code = 0x09;
        public IEnumerable<byte> Buffer { get; }

        public readonly byte PokemonId;
        public readonly PMove Move;
        public readonly bool OwnsMove;

        public PMoveUsedPacket(PPokemon pkmn, PMove move)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(PokemonId = pkmn.Id);
            bytes.AddRange(BitConverter.GetBytes((ushort)(Move = move)));
            bytes.Add((byte)((OwnsMove = pkmn.Moves.Contains(Move)) ? 1 : 0));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PMoveUsedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = r.ReadByte();
                Move = (PMove)r.ReadUInt16();
                OwnsMove = r.ReadBoolean();
            }
        }

        public void Dispose() { }
    }
}
