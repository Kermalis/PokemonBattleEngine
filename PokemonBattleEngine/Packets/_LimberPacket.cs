using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    // TODO: Transform this into an ability action packet
    public sealed class PBELimberPacket : INetPacket
    {
        public const short Code = 0x19;
        public IEnumerable<byte> Buffer { get; }

        public byte PokemonId { get; }
        public bool Prevented { get; } // TODO: Remember why I put this

        public PBELimberPacket(PBEPokemon pkmn, bool prevented)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(PokemonId = pkmn.Id);
            bytes.Add((byte)((Prevented = prevented) ? 1 : 0));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBELimberPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = r.ReadByte();
                Prevented = r.ReadBoolean();
            }
        }

        public void Dispose() { }
    }
}
