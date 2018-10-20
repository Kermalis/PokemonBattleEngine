using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PSetPartyPacket : INetPacket
    {
        public const short Code = 0x05;
        public IEnumerable<byte> Buffer { get; }

        public readonly PPokemon[] Party;

        public PSetPartyPacket(PPokemon[] party)
        {
            Party = party;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            var numPkmn = Math.Min(PConstants.MaxPartySize, Party.Length);
            bytes.Add((byte)numPkmn);
            for (int i = 0; i < numPkmn; i++)
                bytes.AddRange(Party[i].ToBytes());
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PSetPartyPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                var numPkmn = Math.Min(PConstants.MaxPartySize, r.ReadByte());
                Party = new PPokemon[numPkmn];
                for (int i = 0; i < numPkmn; i++)
                    Party[i] = PPokemon.FromBytes(r);
            }
        }
        
        public void Dispose() { }
    }
}
