using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBETurnBeganPacket : INetPacket
    {
        public const short Code = 0x27;
        public ReadOnlyCollection<byte> Buffer { get; }

        public ushort TurnNumber { get; }

        internal PBETurnBeganPacket(ushort turnNumber)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange(BitConverter.GetBytes(TurnNumber = turnNumber));
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBETurnBeganPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            TurnNumber = r.ReadUInt16();
        }

        public void Dispose() { }
    }
}
