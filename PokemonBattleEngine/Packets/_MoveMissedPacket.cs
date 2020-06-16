using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveMissedPacket : IPBEPacket
    {
        public const ushort Code = 0x0D;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFieldPosition MoveUser { get; }
        public PBETeam MoveUserTeam { get; }
        public PBEFieldPosition Pokemon2 { get; }
        public PBETeam Pokemon2Team { get; }

        internal PBEMoveMissedPacket(PBEBattlePokemon moveUser, PBEBattlePokemon pokemon2)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(MoveUser = moveUser.FieldPosition);
                w.Write((MoveUserTeam = moveUser.Team).Id);
                w.Write(Pokemon2 = pokemon2.FieldPosition);
                w.Write((Pokemon2Team = pokemon2.Team).Id);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEMoveMissedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            MoveUser = r.ReadEnum<PBEFieldPosition>();
            MoveUserTeam = battle.Teams[r.ReadByte()];
            Pokemon2 = r.ReadEnum<PBEFieldPosition>();
            Pokemon2Team = battle.Teams[r.ReadByte()];
        }
    }
}
