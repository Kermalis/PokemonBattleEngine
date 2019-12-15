using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPartyResponsePacket : IPBEPacket
    {
        public const ushort Code = 0x04;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETeamShell TeamShell { get; }

        public PBEPartyResponsePacket(PBETeamShell teamShell)
        {
            if (teamShell == null)
            {
                throw new ArgumentNullException(nameof(teamShell));
            }
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (TeamShell = teamShell).ToBytes(w);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPartyResponsePacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            TeamShell = new PBETeamShell(battle.Settings, r);
        }
    }
}
