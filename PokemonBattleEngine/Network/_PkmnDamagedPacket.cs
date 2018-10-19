using Ether.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PPkmnDamagedPacket : INetPacketStream
    {
        public const int Code = 10;
        byte[] buf;
        public byte[] Buffer => (byte[])buf.Clone();

        public readonly Guid PokemonId;
        public readonly ushort Damage;

        public PPkmnDamagedPacket(Guid id, ushort dmg)
        {
            PokemonId = id;
            Damage = dmg;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange(PokemonId.ToByteArray());
            bytes.AddRange(BitConverter.GetBytes(Damage));
            buf = BitConverter.GetBytes(bytes.Count).Concat(bytes).ToArray();
        }
        public PPkmnDamagedPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buf = buffer)))
            {
                r.ReadInt32(); // Skip Code
                PokemonId = new Guid(r.ReadBytes(0x10));
                Damage = r.ReadUInt16();
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
