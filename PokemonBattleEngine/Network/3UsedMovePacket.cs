using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PUsedMovePacket : INetPacketStream
    {
        public const int Code = 0x3;
        byte[] buf;
        public byte[] Buffer => (byte[])buf.Clone();

        public readonly PPokemon Pokemon;
        public readonly PMove Move;

        public PUsedMovePacket(PPokemon pkmn, PMove move)
        {
            Pokemon = pkmn ?? throw new ArgumentNullException(nameof(pkmn));
            Move = move;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange(Pokemon.ToBytes());
            bytes.AddRange(BitConverter.GetBytes((ushort)Move));
            buf = BitConverter.GetBytes(bytes.Count).Concat(bytes).ToArray();
        }
        public PUsedMovePacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buf = buffer)))
            {
                r.ReadInt32(); // Skip Code
                Pokemon = PPokemon.FromBytes(r);
                Move = (PMove)r.ReadUInt16();
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
