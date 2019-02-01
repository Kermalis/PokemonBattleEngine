using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMovePPChangedPacket : INetPacket
    {
        public const short Code = 0x17;
        public IEnumerable<byte> Buffer { get; }

        public PBEFieldPosition MoveUser { get; }
        public PBETeam MoveUserTeam { get; }
        public PBEMove Move { get; }
        public byte OldValue { get; }
        public byte NewValue { get; }

        public PBEMovePPChangedPacket(PBEFieldPosition moveUser, PBETeam moveUserTeam, PBEMove move, byte oldValue, byte newValue)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(MoveUser = moveUser));
            bytes.Add((MoveUserTeam = moveUserTeam).Id);
            bytes.AddRange(BitConverter.GetBytes((ushort)(Move = move)));
            bytes.Add(OldValue = oldValue);
            bytes.Add(NewValue = newValue);
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEMovePPChangedPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                MoveUser = (PBEFieldPosition)r.ReadByte();
                MoveUserTeam = battle.Teams[r.ReadByte()];
                Move = (PBEMove)r.ReadUInt16();
                OldValue = r.ReadByte();
                NewValue = r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
