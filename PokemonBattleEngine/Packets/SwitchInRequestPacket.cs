using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBESwitchInRequestPacket : INetPacket
    {
        public const short Code = 0x23;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBETeam Team { get; }
        public byte Amount { get; }

        internal PBESwitchInRequestPacket(PBETeam team)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((Team = team).Id);
            bytes.Add(Amount = Team.SwitchInsRequired);
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBESwitchInRequestPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            Team = battle.Teams[r.ReadByte()];
            Amount = r.ReadByte();
        }

        public void Dispose() { }
    }
}
