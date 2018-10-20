using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PPkmnStatChangePacket : INetPacket
    {
        public const short Code = 0x10;
        public IEnumerable<byte> Buffer { get; }

        public readonly Guid PokemonId;
        public readonly PStat Stat;
        public readonly sbyte Change;
        public readonly bool IsTooMuch;

        public PPkmnStatChangePacket(PPokemon pkmn, PStat stat, sbyte change, bool isTooMuch)
        {
            PokemonId = pkmn.Id;
            Stat = stat;
            Change = change;
            IsTooMuch = isTooMuch;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange(PokemonId.ToByteArray());
            bytes.Add((byte)Stat);
            bytes.Add((byte)Change);
            bytes.Add((byte)(IsTooMuch ? 1 : 0));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PPkmnStatChangePacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = new Guid(r.ReadBytes(0x10));
                Stat = (PStat)r.ReadByte();
                Change = r.ReadSByte();
                IsTooMuch = r.ReadByte() != 0;
            }
        }
        
        public void Dispose() { }
    }
}
