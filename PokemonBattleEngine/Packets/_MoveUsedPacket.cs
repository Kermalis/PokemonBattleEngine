using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveUsedPacket : INetPacket
    {
        public const short Code = 0x09;
        public IEnumerable<byte> Buffer { get; }

        public PBEFieldPosition MoveUser { get; }
        public PBETeam MoveUserTeam { get; }
        public PBEMove Move { get; }
        public bool Reveal { get; }

        public PBEMoveUsedPacket(PBEPokemon moveUser, PBEMove move, bool reveal)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(MoveUser = moveUser.FieldPosition));
            bytes.Add((MoveUserTeam = moveUser.Team).Id);
            bytes.AddRange(BitConverter.GetBytes((ushort)(Move = move)));
            bytes.Add((byte)((Reveal = reveal) ? 1 : 0));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEMoveUsedPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                MoveUser = (PBEFieldPosition)r.ReadByte();
                MoveUserTeam = battle.Teams[r.ReadByte()];
                Move = (PBEMove)r.ReadUInt16();
                Reveal = r.ReadBoolean();
            }
        }

        public void Dispose() { }
    }
}
