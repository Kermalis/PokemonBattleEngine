using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEWinnerPacket : IPBEPacket
    {
        public const ushort Code = 0x26;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETeam WinningTeam { get; }

        internal PBEWinnerPacket(PBETeam winningTeam)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((WinningTeam = winningTeam).Id);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEWinnerPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            WinningTeam = battle.Teams[r.ReadByte()];
        }
    }
}
