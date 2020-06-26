using Kermalis.EndianBinaryIO;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPartyRequestPacket : IPBEPacket
    {
        public const ushort Code = 0x03;
        public ReadOnlyCollection<byte> Data { get; }

        public byte BattleId { get; }
        public bool RequireLegal { get; }

        public PBEPartyRequestPacket(byte battleId, bool requireLegal)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(BattleId = battleId);
                w.Write(RequireLegal = requireLegal);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPartyRequestPacket(byte[] data, EndianBinaryReader r)
        {
            Data = new ReadOnlyCollection<byte>(data);
            BattleId = r.ReadByte();
            RequireLegal = r.ReadBoolean();
        }
    }
}
