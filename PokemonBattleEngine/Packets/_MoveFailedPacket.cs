using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveFailedPacket : INetPacket
    {
        public const short Code = 0x15;
        public IEnumerable<byte> Buffer { get; }

        public byte Culprit { get; }
        public byte Victim { get; }
        public PBEFailReason FailReason { get; }

        public PBEMoveFailedPacket(PBEPokemon culprit, PBEPokemon victim, PBEFailReason failReason)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(Culprit = culprit.Id);
            bytes.Add(Victim = victim.Id);
            bytes.Add((byte)(FailReason = failReason));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEMoveFailedPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Culprit = r.ReadByte();
                Victim = r.ReadByte();
                FailReason = (PBEFailReason)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
