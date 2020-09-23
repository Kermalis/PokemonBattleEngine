using Kermalis.EndianBinaryIO;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEFleeResponsePacket : IPBEPacket
    {
        public const ushort Code = 0x38;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFleeResponsePacket()
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEFleeResponsePacket(byte[] data)
        {
            Data = new ReadOnlyCollection<byte>(data);
        }
    }
}
