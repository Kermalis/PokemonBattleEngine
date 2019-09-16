using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEBattleStatusPacket : INetPacket
    {
        public const short Code = 0x21;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBEBattleStatus BattleStatus { get; }
        public PBEBattleStatusAction BattleStatusAction { get; }

        internal PBEBattleStatusPacket(PBEBattleStatus battleStatus, PBEBattleStatusAction battleStatusAction)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(BattleStatus = battleStatus));
            bytes.Add((byte)(BattleStatusAction = battleStatusAction));
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEBattleStatusPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            BattleStatus = (PBEBattleStatus)r.ReadByte();
            BattleStatusAction = (PBEBattleStatusAction)r.ReadByte();
        }

        public void Dispose() { }
    }
}
