using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PPkmnMovePacket : INetPacketStream
    {
        public const int Code = 9;
        byte[] buf;
        public byte[] Buffer => (byte[])buf.Clone();

        public readonly Guid PokemonId;
        public readonly PMove Move;
        public readonly bool OwnsMove;

        public PPkmnMovePacket(Guid id, PMove move, bool ownsMove)
        {
            PokemonId = id;
            Move = move;
            OwnsMove = ownsMove;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange(PokemonId.ToByteArray());
            bytes.AddRange(BitConverter.GetBytes((ushort)Move));
            bytes.Add((byte)(OwnsMove ? 1 : 0));
            buf = BitConverter.GetBytes(bytes.Count).Concat(bytes).ToArray();
        }
        public PPkmnMovePacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buf = buffer)))
            {
                r.ReadInt32(); // Skip Code
                PokemonId = new Guid(r.ReadBytes(0x10));
                Move = (PMove)r.ReadUInt16();
                OwnsMove = r.ReadByte() != 0;
            }
        }

        public int Size => throw new NotImplementedException();
        public long Position => throw new NotImplementedException();
        public T Read<T>() => throw new NotImplementedException();
        public T[] ReadArray<T>(int amount) => throw new NotImplementedException();
        public void Write<T>(T value) => throw new NotImplementedException();
        public void Dispose() { }
    }
}
