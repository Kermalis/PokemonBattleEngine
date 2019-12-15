using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBETeamPacket : IPBEPacket
    {
        public const ushort Code = 0x05;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETeam Team { get; }

        internal PBETeamPacket(PBETeam team)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((Team = team).Id);
                Team.ToBytes(w);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBETeamPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Team = battle.Teams[r.ReadByte()];
            Team.FromBytes(r);
        }
    }
}
