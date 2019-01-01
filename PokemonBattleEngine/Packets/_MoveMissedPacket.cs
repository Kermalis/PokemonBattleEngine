using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveMissedPacket : INetPacket
    {
        public const short Code = 0x0D;
        public IEnumerable<byte> Buffer { get; }

        public byte Culprit { get; }
        public byte Victim { get; }

        public PBEMoveMissedPacket(PBEPokemon culprit, PBEPokemon victim)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(Culprit = culprit.Id);
            bytes.Add(Victim = victim.Id);
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEMoveMissedPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Culprit = r.ReadByte();
                Victim = r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
