using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
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
        public ReadOnlyCollection<byte> Buffer { get; }

        public ReadOnlyCollection<PBESwitchIn> Switches { get; }

        public PBESwitchInResponsePacket(IList<PBESwitchIn> switches)
        {
            if (switches == null)
            {
                throw new ArgumentNullException(nameof(switches));
            }
            if (switches.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(switches));
            }
            if (switches.Any(s => s == null))
            {
                throw new ArgumentNullException(nameof(switches));
            }
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Switches = new ReadOnlyCollection<PBESwitchIn>(switches)).Count);
            for (int i = 0; i < (byte)Switches.Count; i++)
            {
                Switches[i].ToBytes(bytes);
            }
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBESwitchInResponsePacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            var switches = new PBESwitchIn[r.ReadByte()];
            for (int i = 0; i < switches.Length; i++)
            {
                switches[i] = new PBESwitchIn(r);
            }
            Switches = new ReadOnlyCollection<PBESwitchIn>(switches);
        }

        public void Dispose() { }
    }
}
