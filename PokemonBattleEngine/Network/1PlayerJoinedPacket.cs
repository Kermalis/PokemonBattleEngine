using Ether.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PPlayerJoinedPacket : INetPacketStream
    {
        public const int Code = 0x1;
        byte[] buf;
        public byte[] Buffer => (byte[])buf.Clone();

        public readonly Guid PlayerId;
        public readonly string DisplayName;

        public PPlayerJoinedPacket(Guid id, string name)
        {
            PlayerId = id;
            DisplayName = name;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange(id.ToByteArray());
            byte[] nameBytes = Encoding.ASCII.GetBytes(DisplayName);
            bytes.Add((byte)nameBytes.Length);
            bytes.AddRange(nameBytes);
            buf = BitConverter.GetBytes(bytes.Count).Concat(bytes).ToArray();
        }
        public PPlayerJoinedPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buf = buffer)))
            {
                r.ReadInt32(); // Skip Code
                PlayerId = new Guid(r.ReadBytes(16));
                DisplayName = Encoding.ASCII.GetString(r.ReadBytes(r.ReadByte()));
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
