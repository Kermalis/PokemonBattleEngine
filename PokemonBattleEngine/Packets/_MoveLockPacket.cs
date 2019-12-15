using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveLockPacket : IPBEPacket
    {
        public const ushort Code = 0x28;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFieldPosition MoveUser { get; }
        public PBETeam MoveUserTeam { get; }
        public PBEMove LockedMove { get; }
        public PBETurnTarget LockedTargets { get; }
        public PBEMoveLockType MoveLockType { get; }

        internal PBEMoveLockPacket(PBEPokemon moveUser, PBEMove lockedMove, PBETurnTarget lockedTargets, PBEMoveLockType moveLockType)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(MoveUser = moveUser.FieldPosition);
                w.Write((MoveUserTeam = moveUser.Team).Id);
                w.Write(LockedMove = lockedMove);
                w.Write(LockedTargets = lockedTargets);
                w.Write(MoveLockType = moveLockType);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEMoveLockPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            MoveUser = r.ReadEnum<PBEFieldPosition>();
            MoveUserTeam = battle.Teams[r.ReadByte()];
            LockedMove = r.ReadEnum<PBEMove>();
            LockedTargets = r.ReadEnum<PBETurnTarget>();
            MoveLockType = r.ReadEnum<PBEMoveLockType>();
        }
    }
}
