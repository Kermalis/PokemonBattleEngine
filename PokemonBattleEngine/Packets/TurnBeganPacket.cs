using Kermalis.EndianBinaryIO;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBETurnBeganPacket : IPBEPacket
    {
        public const ushort Code = 0x27;
        public ReadOnlyCollection<byte> Data { get; }

        public ushort TurnNumber { get; }

        internal PBETurnBeganPacket(ushort turnNumber)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(TurnNumber = turnNumber);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBETurnBeganPacket(byte[] data, EndianBinaryReader r)
        {
            Data = new ReadOnlyCollection<byte>(data);
            TurnNumber = r.ReadUInt16();
        }
    }
}
