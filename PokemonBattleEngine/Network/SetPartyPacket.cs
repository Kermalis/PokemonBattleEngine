using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PSetPartyPacket : INetPacketStream
    {
        public const int Code = 0x5;
        byte[] buf;
        public byte[] Buffer => (byte[])buf.Clone();

        public readonly PPokemon[] Pokemon;

        public PSetPartyPacket(PPokemon[] pokemon)
        {
            Pokemon = pokemon;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            var numPkmn = Math.Min(PConstants.MaxPokemon, Pokemon.Length);
            bytes.Add((byte)numPkmn);
            for (int i = 0; i < numPkmn; i++)
                bytes.AddRange(Pokemon[i].ToBytes());
            buf = BitConverter.GetBytes(bytes.Count).Concat(bytes).ToArray();
        }
        public PSetPartyPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buf = buffer)))
            {
                r.ReadInt32(); // Skip Code
                var numPkmn = Math.Min(PConstants.MaxPokemon, r.ReadByte());
                Pokemon = new PPokemon[numPkmn];
                for (int i = 0; i < numPkmn; i++)
                    Pokemon[i] = PPokemon.FromBytes(r);
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
