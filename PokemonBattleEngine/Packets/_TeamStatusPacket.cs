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
        public PBETrainer DamageVictimTrainer { get; }
        public PBEFieldPosition DamageVictim { get; }

        internal PBETeamStatusPacket(PBETeam team, PBETeamStatus teamStatus, PBETeamStatusAction teamStatusAction, PBEBattlePokemon damageVictim = null)
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
                    w.Write((DamageVictimTrainer = damageVictim.Trainer).Id);
                    w.Write(DamageVictim = damageVictim.FieldPosition);
                }
                Data = new ReadOnlyCollection<byte>(ms.GetBuffer());
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
                DamageVictimTrainer = battle.Trainers[r.ReadByte()];
                DamageVictim = r.ReadEnum<PBEFieldPosition>();
            }
        }
    }
}
