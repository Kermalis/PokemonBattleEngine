using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnStatChangedPacket : INetPacket
    {
        public const short Code = 0x10;
        public IEnumerable<byte> Buffer { get; }

        public byte Victim { get; }
        public PBEStat Stat { get; }
        public short Change { get; }
        public bool IsTooMuch { get; }

        public PBEPkmnStatChangedPacket(PBEPokemon victim, PBEStat stat, short change, bool isTooMuch)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(Victim = victim.Id);
            bytes.Add((byte)(Stat = stat));
            bytes.AddRange(BitConverter.GetBytes(Change = change));
            bytes.Add((byte)((IsTooMuch = isTooMuch) ? 1 : 0));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEPkmnStatChangedPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Victim = r.ReadByte();
                Stat = (PBEStat)r.ReadByte();
                Change = r.ReadInt16();
                IsTooMuch = r.ReadBoolean();
            }
        }

        public void Dispose() { }
    }
}
