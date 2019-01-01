using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMovePPChangedPacket : INetPacket
    {
        public const short Code = 0x17;
        public IEnumerable<byte> Buffer { get; }

        public byte Victim { get; }
        public PBEMove Move { get; }
        public short Change { get; }

        public PBEMovePPChangedPacket(PBEPokemon victim, PBEMove move, short change)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(Victim = victim.Id);
            bytes.AddRange(BitConverter.GetBytes((ushort)(Move = move)));
            bytes.AddRange(BitConverter.GetBytes(Change = change));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEMovePPChangedPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Victim = r.ReadByte();
                Move = (PBEMove)r.ReadUInt16();
                Change = r.ReadInt16();
            }
        }

        public void Dispose() { }
    }
}
