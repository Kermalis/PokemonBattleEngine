using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PPkmnDamagedPacket : INetPacket
    {
        public const short Code = 0x0A;
        public IEnumerable<byte> Buffer { get; }

        public readonly Guid PokemonId;
        public readonly ushort Damage;

        public PPkmnDamagedPacket(PPokemon pkmn, ushort dmg)
        {
            PokemonId = pkmn.Id;
            Damage = dmg;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange(PokemonId.ToByteArray());
            bytes.AddRange(BitConverter.GetBytes(Damage));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PPkmnDamagedPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = new Guid(r.ReadBytes(0x10));
                Damage = r.ReadUInt16();
            }
        }
        
        public void Dispose() { }
    }
}
