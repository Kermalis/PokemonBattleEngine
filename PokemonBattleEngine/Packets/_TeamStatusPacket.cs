using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBETeamStatusPacket : INetPacket
    {
        public const short Code = 0x13;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBETeam Team { get; }
        public PBETeamStatus TeamStatus { get; }
        public PBETeamStatusAction TeamStatusAction { get; }
        public PBEFieldPosition DamageVictim { get; } // PBEFieldPosition.None means no victim

        internal PBETeamStatusPacket(PBETeam team, PBETeamStatus teamStatus, PBETeamStatusAction teamStatusAction, PBEPokemon damageVictim)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((Team = team).Id);
            bytes.AddRange(BitConverter.GetBytes((ushort)(TeamStatus = teamStatus)));
            bytes.Add((byte)(TeamStatusAction = teamStatusAction));
            bytes.Add((byte)(DamageVictim = damageVictim == null ? PBEFieldPosition.None : damageVictim.FieldPosition));
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBETeamStatusPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            Team = battle.Teams[r.ReadByte()];
            TeamStatus = (PBETeamStatus)r.ReadUInt16();
            TeamStatusAction = (PBETeamStatusAction)r.ReadByte();
            DamageVictim = (PBEFieldPosition)r.ReadByte();
        }

        public void Dispose() { }
    }
}
