using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBESetPartyPacket : INetPacket
    {
        public const short Code = 0x05;
        public IEnumerable<byte> Buffer { get; }

        public PBEPokemon[] Party { get; }

        public PBESetPartyPacket(IEnumerable<PBEPokemon> party, PBESettings settings)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            var numPkmn = Math.Min(settings.MaxPartySize, (Party = party.ToArray()).Length);
            bytes.Add((byte)numPkmn);
            for (int i = 0; i < numPkmn; i++)
            {
                bytes.AddRange(Party[i].ToBytes(settings));
            }
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBESetPartyPacket(byte[] buffer, PBESettings settings)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                var numPkmn = Math.Min(settings.MaxPartySize, r.ReadByte());
                Party = new PBEPokemon[numPkmn];
                for (int i = 0; i < numPkmn; i++)
                {
                    Party[i] = PBEPokemon.FromBytes(r, settings);
                }
            }
        }

        public void Dispose() { }
    }
}
