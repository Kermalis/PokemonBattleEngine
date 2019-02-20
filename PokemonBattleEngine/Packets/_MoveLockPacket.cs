using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveLockPacket : INetPacket
    {
        public const short Code = 0x28;
        public IEnumerable<byte> Buffer { get; }

        public PBEFieldPosition MoveUser { get; }
        public PBETeam MoveUserTeam { get; }
        public PBEMove LockedMove { get; }
        public PBETarget LockedTargets { get; }
        public PBEMoveLockType MoveLockType { get; }

        public PBEMoveLockPacket(PBEPokemon moveUser, PBEMove lockedMove, PBETarget lockedTargets, PBEMoveLockType moveLockType)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(MoveUser = moveUser.FieldPosition));
            bytes.Add((MoveUserTeam = moveUser.Team).Id);
            bytes.Add((byte)(LockedMove = lockedMove));
            bytes.Add((byte)(LockedTargets = lockedTargets));
            bytes.Add((byte)(MoveLockType = moveLockType));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEMoveLockPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                MoveUser = (PBEFieldPosition)r.ReadByte();
                MoveUserTeam = battle.Teams[r.ReadByte()];
                LockedMove = (PBEMove)r.ReadByte();
                LockedTargets = (PBETarget)r.ReadByte();
                MoveLockType = (PBEMoveLockType)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
