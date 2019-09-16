using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPlayerJoinedPacket : INetPacket
    {
        public const short Code = 0x01;
        public ReadOnlyCollection<byte> Buffer { get; }

        public bool IsMe { get; }
        public int BattleId { get; }
        public string TrainerName { get; }

        public PBEPlayerJoinedPacket(bool isMe, int battleId, string trainerName)
        {
            if (trainerName == null)
            {
                throw new ArgumentNullException(nameof(trainerName));
            }
            if (string.IsNullOrWhiteSpace(trainerName))
            {
                throw new ArgumentOutOfRangeException(nameof(trainerName));
            }
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)((IsMe = isMe) ? 1 : 0));
            bytes.AddRange(BitConverter.GetBytes(BattleId = battleId));
            PBEUtils.StringToBytes(bytes, TrainerName = trainerName);
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEPlayerJoinedPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            IsMe = r.ReadBoolean();
            BattleId = r.ReadInt32();
            TrainerName = PBEUtils.StringFromBytes(r);
        }

        public void Dispose() { }
    }
}
