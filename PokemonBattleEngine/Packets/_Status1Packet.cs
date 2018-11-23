using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PStatus1Packet : INetPacket
    {
        public const short Code = 0x11;
        public IEnumerable<byte> Buffer { get; }

        public readonly byte PokemonId;
        public readonly PStatus1 Status;
        public readonly PStatusAction Action;

        public PStatus1Packet(PPokemon pkmn, PStatus1 status, PStatusAction action)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(PokemonId = pkmn.Id);
            bytes.Add((byte)(Status = status));
            bytes.Add((byte)(Action = action));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PStatus1Packet(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = r.ReadByte();
                Status = (PStatus1)r.ReadByte();
                Action = (PStatusAction)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
