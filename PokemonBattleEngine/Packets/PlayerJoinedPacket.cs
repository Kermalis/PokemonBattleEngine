using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPlayerJoinedPacket : INetPacket
    {
        public const short Code = 0x01;
        public IEnumerable<byte> Buffer { get; }

        public bool IsMe { get; }
        public int BattleId { get; }
        public string TrainerName { get; }

        public PBEPlayerJoinedPacket(bool isMe, int battleId, string trainerName)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)((IsMe = isMe) ? 1 : 0));
            bytes.AddRange(BitConverter.GetBytes(BattleId = battleId));
            bytes.AddRange(PBEUtils.StringToBytes(TrainerName = trainerName));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEPlayerJoinedPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                IsMe = r.ReadBoolean();
                BattleId = r.ReadInt32();
                TrainerName = PBEUtils.StringFromBytes(r);
            }
        }

        public void Dispose() { }
    }
}
