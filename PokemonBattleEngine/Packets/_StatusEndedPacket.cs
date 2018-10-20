using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PStatusEndedPacket : INetPacket
    {
        public const short Code = 0x12;
        public IEnumerable<byte> Buffer { get; }

        public readonly Guid PokemonId;
        public readonly PStatus Status;

        public PStatusEndedPacket(PPokemon pkmn)
        {
            PokemonId = pkmn.Id;
            Status = pkmn.Status;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange(PokemonId.ToByteArray());
            bytes.Add((byte)Status);
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PStatusEndedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = new Guid(r.ReadBytes(0x10));
                Status = (PStatus)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
