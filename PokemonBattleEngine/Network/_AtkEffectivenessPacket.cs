using Ether.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PAtkEffectivenessPacket : INetPacketStream
    {
        public const int Code = 11;
        byte[] buf;
        public byte[] Buffer => (byte[])buf.Clone();

        public readonly Guid PokemonId; // Defender
        public readonly double Effectiveness;

        public PAtkEffectivenessPacket(Guid pkmnId, double effectiveness)
        {
            PokemonId = pkmnId;
            Effectiveness = effectiveness;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange(PokemonId.ToByteArray());
            bytes.AddRange(BitConverter.GetBytes(Effectiveness));
            buf = BitConverter.GetBytes(bytes.Count).Concat(bytes).ToArray();
        }
        public PAtkEffectivenessPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buf = buffer)))
            {
                r.ReadInt32(); // Skip Code
                PokemonId = new Guid(r.ReadBytes(0x10));
                Effectiveness = r.ReadDouble();
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
