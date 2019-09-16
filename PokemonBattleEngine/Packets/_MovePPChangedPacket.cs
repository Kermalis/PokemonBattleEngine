using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMovePPChangedPacket : INetPacket
    {
        public const short Code = 0x17;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBEFieldPosition MoveUser { get; }
        public PBETeam MoveUserTeam { get; }
        public PBEMove Move { get; }
        public int AmountReduced { get; }

        internal PBEMovePPChangedPacket(PBEFieldPosition moveUser, PBETeam moveUserTeam, PBEMove move, int amountReduced)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(MoveUser = moveUser));
            bytes.Add((MoveUserTeam = moveUserTeam).Id);
            bytes.AddRange(BitConverter.GetBytes((ushort)(Move = move)));
            bytes.AddRange(BitConverter.GetBytes(AmountReduced = amountReduced));
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEMovePPChangedPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            MoveUser = (PBEFieldPosition)r.ReadByte();
            MoveUserTeam = battle.Teams[r.ReadByte()];
            Move = (PBEMove)r.ReadUInt16();
            AmountReduced = r.ReadInt32();
        }

        public void Dispose() { }
    }
}
