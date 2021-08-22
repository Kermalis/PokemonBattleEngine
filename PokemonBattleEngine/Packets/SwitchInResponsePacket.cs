using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBESwitchInResponsePacket : IPBEPacket
    {
        public const ushort Code = 0x24;
        public ReadOnlyCollection<byte> Data { get; }

        public ReadOnlyCollection<PBESwitchIn> Switches { get; }

        public PBESwitchInResponsePacket(IList<PBESwitchIn> switches)
        {
            if (switches.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(switches));
            }
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                byte count = (byte)(Switches = new ReadOnlyCollection<PBESwitchIn>(switches)).Count;
                w.Write(count);
                for (int i = 0; i < count; i++)
                {
                    Switches[i].ToBytes(w);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBESwitchInResponsePacket(byte[] data, EndianBinaryReader r)
        {
            Data = new ReadOnlyCollection<byte>(data);
            var switches = new PBESwitchIn[r.ReadByte()];
            for (int i = 0; i < switches.Length; i++)
            {
                switches[i] = new PBESwitchIn(r);
            }
            Switches = new ReadOnlyCollection<PBESwitchIn>(switches);
        }
    }
}
