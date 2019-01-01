using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBETeamStatusPacket : INetPacket
    {
        public const short Code = 0x13;
        public IEnumerable<byte> Buffer { get; }

        public PBETeam Team { get; }
        public PBETeamStatus TeamStatus { get; }
        public PBETeamStatusAction TeamStatusAction { get; }
        public byte Victim { get; } // Victim of PBETeamStatusAction.CausedDamage (byte.MaxValue means no victim)

        public PBETeamStatusPacket(PBETeam team, PBETeamStatus teamStatus, PBETeamStatusAction teamStatusAction, PBEPokemon victim)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((Team = team).Id);
            bytes.Add((byte)(TeamStatus = teamStatus));
            bytes.Add((byte)(TeamStatusAction = teamStatusAction));
            bytes.Add(victim == null ? byte.MaxValue : victim.Id);
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBETeamStatusPacket(byte[] buffer, PBEBattle battle)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Team = battle.Teams[r.ReadByte()];
                TeamStatus = (PBETeamStatus)r.ReadByte();
                TeamStatusAction = (PBETeamStatusAction)r.ReadByte();
                Victim = r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
