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

        public readonly byte PokemonId; // Defender
        public readonly PEffectiveness Effectiveness;

        public PMoveEffectivenessPacket(PPokemon pkmn, PEffectiveness effectiveness)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(PokemonId = pkmn.Id);
            bytes.Add((byte)(Effectiveness = effectiveness));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PMoveEffectivenessPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = r.ReadByte();
                Effectiveness = (PEffectiveness)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
