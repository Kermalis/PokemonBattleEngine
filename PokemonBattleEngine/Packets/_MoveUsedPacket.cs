using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveUsedPacket : INetPacket
    {
        public const short Code = 0x09;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBEFieldPosition MoveUser { get; }
        public PBETeam MoveUserTeam { get; }
        public PBEMove Move { get; }
        public bool Reveal { get; }

        internal PBEMoveUsedPacket(PBEPokemon moveUser, PBEMove move, bool reveal)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(MoveUser = moveUser.FieldPosition));
            bytes.Add((MoveUserTeam = moveUser.Team).Id);
            bytes.AddRange(BitConverter.GetBytes((ushort)(Move = move)));
            bytes.Add((byte)((Reveal = reveal) ? 1 : 0));
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEMoveUsedPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            MoveUser = (PBEFieldPosition)r.ReadByte();
            MoveUserTeam = battle.Teams[r.ReadByte()];
            Move = (PBEMove)r.ReadUInt16();
            Reveal = r.ReadBoolean();
        }

        public void Dispose() { }
    }
}
