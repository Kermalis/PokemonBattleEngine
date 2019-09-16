using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveLockPacket : INetPacket
    {
        public const short Code = 0x28;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBEFieldPosition MoveUser { get; }
        public PBETeam MoveUserTeam { get; }
        public PBEMove LockedMove { get; }
        public PBETurnTarget LockedTargets { get; }
        public PBEMoveLockType MoveLockType { get; }

        internal PBEMoveLockPacket(PBEPokemon moveUser, PBEMove lockedMove, PBETurnTarget lockedTargets, PBEMoveLockType moveLockType)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(MoveUser = moveUser.FieldPosition));
            bytes.Add((MoveUserTeam = moveUser.Team).Id);
            bytes.AddRange(BitConverter.GetBytes((ushort)(LockedMove = lockedMove)));
            bytes.Add((byte)(LockedTargets = lockedTargets));
            bytes.Add((byte)(MoveLockType = moveLockType));
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEMoveLockPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            MoveUser = (PBEFieldPosition)r.ReadByte();
            MoveUserTeam = battle.Teams[r.ReadByte()];
            LockedMove = (PBEMove)r.ReadUInt16();
            LockedTargets = (PBETurnTarget)r.ReadByte();
            MoveLockType = (PBEMoveLockType)r.ReadByte();
        }

        public void Dispose() { }
    }
}
