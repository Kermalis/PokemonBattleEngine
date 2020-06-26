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

        public string TrainerName { get; }

        public PBEPlayerJoinedPacket(string trainerName)
        {
            if (string.IsNullOrWhiteSpace(trainerName))
            {
                throw new ArgumentOutOfRangeException(nameof(trainerName));
            }
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(TrainerName = trainerName, true);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPlayerJoinedPacket(byte[] data, EndianBinaryReader r)
        {
            Data = new ReadOnlyCollection<byte>(data);
            TrainerName = r.ReadStringNullTerminated();
        }
    }
}
