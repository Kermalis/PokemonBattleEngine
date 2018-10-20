using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PMoveEffectivenessPacket : INetPacket
    {
        public const short Code = 0x0B;
        public IEnumerable<byte> Buffer { get; }

        public readonly Guid PokemonId; // Defender
        public readonly double Effectiveness;

        public PMoveEffectivenessPacket(PPokemon defender, double effectiveness)
        {
            PokemonId = defender.Id;
            Effectiveness = effectiveness;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange(PokemonId.ToByteArray());
            bytes.AddRange(BitConverter.GetBytes(Effectiveness));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PMoveEffectivenessPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = new Guid(r.ReadBytes(0x10));
                Effectiveness = r.ReadDouble();
            }
        }
        
        public void Dispose() { }
    }
}
