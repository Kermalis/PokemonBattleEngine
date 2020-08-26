using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEBattleResultPacket : IPBEPacket
    {
        public const ushort Code = 0x26;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattleResult BattleResult { get; }

        internal PBEBattleResultPacket(PBEBattleResult battleResult)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(BattleResult = battleResult);
                Data = new ReadOnlyCollection<byte>(ms.GetBuffer());
            }
        }
        internal PBEBattleResultPacket(byte[] data, EndianBinaryReader r)
        {
            Data = new ReadOnlyCollection<byte>(data);
            BattleResult = r.ReadEnum<PBEBattleResult>();
        }
    }
}
