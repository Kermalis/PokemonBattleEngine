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

        public PBEBattlePokemon MoveUser { get; }
        public PBEMoveLockType MoveLockType { get; }
        public PBEMove LockedMove { get; }
        public PBETurnTarget? LockedTargets { get; }

        internal PBEMoveLockPacket(PBEBattlePokemon moveUser, PBEMoveLockType moveLockType, PBEMove lockedMove, PBETurnTarget? lockedTargets = null)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (MoveUser = moveUser).ToBytes_Position(w);
                w.Write(MoveLockType = moveLockType);
                w.Write(LockedMove = lockedMove);
                w.Write(lockedTargets.HasValue);
                if (lockedTargets.HasValue)
                {
                    w.Write((LockedTargets = lockedTargets).Value);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEMoveLockPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            MoveUser = battle.GetPokemon_Position(r);
            MoveLockType = r.ReadEnum<PBEMoveLockType>();
            LockedMove = r.ReadEnum<PBEMove>();
            if (r.ReadBoolean())
            {
                LockedTargets = r.ReadEnum<PBETurnTarget>();
            }
        }
    }
}
