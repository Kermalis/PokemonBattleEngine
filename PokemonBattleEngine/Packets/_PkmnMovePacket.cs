using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PPkmnMovePacket : INetPacket
    {
        public const short Code = 0x09;
        public IEnumerable<byte> Buffer { get; }

        public readonly Guid PokemonId;
        public readonly PMove Move;
        public readonly bool OwnsMove;

        public PPkmnMovePacket(PPokemon pkmn, PMove move)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange((PokemonId = pkmn.Id).ToByteArray());
            bytes.AddRange(BitConverter.GetBytes((ushort)(Move = move)));
            bytes.Add((byte)((OwnsMove = pkmn.Shell.Moves.Contains(Move)) ? 1 : 0));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PPkmnMovePacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = new Guid(r.ReadBytes(0x10));
                Move = (PMove)r.ReadUInt16();
                OwnsMove = r.ReadByte() != 0;
            }
        }

        public void Dispose() { }
    }
}
