using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEHazePacket : INetPacket
    {
        public const short Code = 0x0B;
        public ReadOnlyCollection<byte> Buffer { get; }

        internal PBEHazePacket()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEHazePacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
        }

        public void Dispose() { }
    }
}
