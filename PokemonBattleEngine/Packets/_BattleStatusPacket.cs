using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEBattleStatusPacket : INetPacket
    {
        public const short Code = 0x21;
        public IEnumerable<byte> Buffer { get; }

        public PBEBattleStatus BattleStatus { get; }
        public PBEBattleStatusAction BattleStatusAction { get; }

        public PBEBattleStatusPacket(PBEBattleStatus battleStatus, PBEBattleStatusAction battleStatusAction)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(BattleStatus = battleStatus));
            bytes.Add((byte)(BattleStatusAction = battleStatusAction));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEBattleStatusPacket(byte[] buffer, PBEBattle battle)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                BattleStatus = (PBEBattleStatus)r.ReadByte();
                BattleStatusAction = (PBEBattleStatusAction)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
