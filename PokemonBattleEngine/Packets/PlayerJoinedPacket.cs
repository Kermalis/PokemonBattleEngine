using Kermalis.EndianBinaryIO;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPlayerJoinedPacket : IPBEPacket
    {
        public const ushort Code = 0x01;
        public ReadOnlyCollection<byte> Data { get; }

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
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(IsMe = isMe);
                w.Write(BattleId = battleId);
                w.Write(TrainerName = trainerName, true);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPlayerJoinedPacket(byte[] data, EndianBinaryReader r)
        {
            Data = new ReadOnlyCollection<byte>(data);
            IsMe = r.ReadBoolean();
            BattleId = r.ReadInt32();
            TrainerName = r.ReadStringNullTerminated();
        }
    }
}
