using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEStatus1Packet : INetPacket
    {
        public const short Code = 0x11;
        public IEnumerable<byte> Buffer { get; }

        public byte Culprit { get; }
        public byte Victim { get; }
        public PBEStatus1 Status1 { get; }
        public PBEStatusAction StatusAction { get; }

        public PBEStatus1Packet(PBEPokemon culprit, PBEPokemon victim, PBEStatus1 status1, PBEStatusAction statusAction)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(Culprit = culprit.Id);
            bytes.Add(Victim = victim.Id);
            bytes.Add((byte)(Status1 = status1));
            bytes.Add((byte)(StatusAction = statusAction));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEStatus1Packet(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Culprit = r.ReadByte();
                Victim = r.ReadByte();
                Status1 = (PBEStatus1)r.ReadByte();
                StatusAction = (PBEStatusAction)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
