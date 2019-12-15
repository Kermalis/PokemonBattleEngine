using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEBattleStatusPacket : IPBEPacket
    {
        public const ushort Code = 0x21;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattleStatus BattleStatus { get; }
        public PBEBattleStatusAction BattleStatusAction { get; }

        internal PBEBattleStatusPacket(PBEBattleStatus battleStatus, PBEBattleStatusAction battleStatusAction)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(BattleStatus = battleStatus);
                w.Write(BattleStatusAction = battleStatusAction);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEBattleStatusPacket(byte[] data, EndianBinaryReader r)
        {
            Data = new ReadOnlyCollection<byte>(data);
            BattleStatus = r.ReadEnum<PBEBattleStatus>();
            BattleStatusAction = r.ReadEnum<PBEBattleStatusAction>();
        }
    }
}
