using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PLimberPacket : INetPacket
    {
        public const short Code = 0x19;
        public IEnumerable<byte> Buffer { get; }

        public readonly Guid PokemonId;
        public readonly bool Prevented; // TODO: Remember why I put this

        public PLimberPacket(PPokemon pkmn, bool prevented)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange((PokemonId = pkmn.Id).ToByteArray());
            bytes.Add((byte)((Prevented = prevented) ? 1 : 0));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PLimberPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = new Guid(r.ReadBytes(0x10));
                Prevented = r.ReadBoolean();
            }
        }

        public void Dispose() { }
    }
}
