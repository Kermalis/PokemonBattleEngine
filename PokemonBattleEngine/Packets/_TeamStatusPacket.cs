using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBETeamStatusPacket : IPBEPacket
    {
        public const ushort Code = 0x13;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETeam Team { get; }
        public PBETeamStatus TeamStatus { get; }
        public PBETeamStatusAction TeamStatusAction { get; }
        public PBEFieldPosition? DamageVictim { get; }

        internal PBETeamStatusPacket(PBETeam team, PBETeamStatus teamStatus, PBETeamStatusAction teamStatusAction, PBEPokemon damageVictim)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((Team = team).Id);
                w.Write(TeamStatus = teamStatus);
                w.Write(TeamStatusAction = teamStatusAction);
                w.Write(damageVictim != null);
                if (damageVictim != null)
                {
                    w.Write((DamageVictim = damageVictim.FieldPosition).Value);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBETeamStatusPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Team = battle.Teams[r.ReadByte()];
            TeamStatus = r.ReadEnum<PBETeamStatus>();
            TeamStatusAction = r.ReadEnum<PBETeamStatusAction>();
            if (r.ReadBoolean())
            {
                DamageVictim = r.ReadEnum<PBEFieldPosition>();
            }
        }
    }
}
