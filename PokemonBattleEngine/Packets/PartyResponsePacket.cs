using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPartyResponsePacket : INetPacket
    {
        public const short Code = 0x04;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBETeamShell TeamShell { get; }

        public PBEPartyResponsePacket(PBETeamShell teamShell)
        {
            if (teamShell == null)
            {
                throw new ArgumentNullException(nameof(teamShell));
            }
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            (TeamShell = teamShell).ToBytes(bytes);
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEPartyResponsePacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            TeamShell = new PBETeamShell(battle.Settings, r); // What happens if an exception occurs? Similar question to https://github.com/Kermalis/PokemonBattleEngine/issues/167
        }

        public void Dispose() { }
    }
}
