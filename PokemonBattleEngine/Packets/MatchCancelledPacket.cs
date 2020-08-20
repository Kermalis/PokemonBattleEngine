using Kermalis.EndianBinaryIO;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMatchCancelledPacket : IPBEPacket
    {
        public const ushort Code = 0x02;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEMatchCancelledPacket()
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                Data = new ReadOnlyCollection<byte>(ms.GetBuffer());
            }
        }
        internal PBEMatchCancelledPacket(byte[] data)
        {
            Data = new ReadOnlyCollection<byte>(data);
        }
    }
}
