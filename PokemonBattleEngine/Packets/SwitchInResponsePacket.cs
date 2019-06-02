using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBESwitchInResponsePacket : INetPacket
    {
        public const short Code = 0x24;
        public IEnumerable<byte> Buffer { get; }

        public ReadOnlyCollection<(byte PokemonId, PBEFieldPosition Position)> Switches { get; }

        public PBESwitchInResponsePacket(IEnumerable<(byte PokemonId, PBEFieldPosition Position)> switches)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Switches = switches.ToList().AsReadOnly()).Count);
            bytes.AddRange(Switches.SelectMany(s => new byte[] { s.PokemonId, (byte)s.Position }));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBESwitchInResponsePacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                var switches = new (byte PokemonId, PBEFieldPosition Position)[r.ReadByte()];
                for (int i = 0; i < switches.Length; i++)
                {
                    switches[i] = (r.ReadByte(), (PBEFieldPosition)r.ReadByte());
                }
                Switches = Array.AsReadOnly(switches);
            }
        }

        public void Dispose() { }
    }
}
