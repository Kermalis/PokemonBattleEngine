using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBESwitchInRequestPacket : IPBEPacket
    {
        public const ushort Code = 0x23;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETeam Team { get; }
        public byte Amount { get; }

        internal PBESwitchInRequestPacket(PBETeam team)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((Team = team).Id);
                w.Write(Amount = Team.SwitchInsRequired);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBESwitchInRequestPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Team = battle.Teams[r.ReadByte()];
            Amount = r.ReadByte();
        }
    }
}
