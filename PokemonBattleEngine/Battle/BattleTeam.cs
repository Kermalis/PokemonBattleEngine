using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed class PBETeams : IReadOnlyList<PBETeam>
    {
        private readonly PBETeam _team0;
        private readonly PBETeam _team1;

        public int Count => 2;
        public PBETeam this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return _team0;
                    case 1: return _team1;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        // Trainer battle
        internal PBETeams(PBEBattle battle, IReadOnlyList<PBETrainerInfo> ti0, IReadOnlyList<PBETrainerInfo> ti1, out ReadOnlyCollection<PBETrainer> trainers)
        {
            var allTrainers = new List<PBETrainer>();
            _team0 = new PBETeam(battle, 0, ti0, allTrainers);
            _team1 = new PBETeam(battle, 1, ti1, allTrainers);
            _team0.OpposingTeam = _team1;
            _team1.OpposingTeam = _team0;
            trainers = new ReadOnlyCollection<PBETrainer>(allTrainers);
        }
        // Wild battle
        internal PBETeams(PBEBattle battle, IReadOnlyList<PBETrainerInfo> ti, PBEWildInfo wi, out ReadOnlyCollection<PBETrainer> trainers)
        {
            var allTrainers = new List<PBETrainer>();
            _team0 = new PBETeam(battle, 0, ti, allTrainers);
            _team1 = new PBETeam(battle, 1, wi, allTrainers);
            _team0.OpposingTeam = _team1;
            _team1.OpposingTeam = _team0;
            trainers = new ReadOnlyCollection<PBETrainer>(allTrainers);
        }
        // Remote trainer battle
        internal PBETeams(PBEBattle battle, PBEBattlePacket packet, out ReadOnlyCollection<PBETrainer> trainers)
        {
            var allTrainers = new List<PBETrainer>();
            _team0 = new PBETeam(battle, packet.Teams[0], allTrainers);
            _team1 = new PBETeam(battle, packet.Teams[1], allTrainers);
            _team0.OpposingTeam = _team1;
            _team1.OpposingTeam = _team0;
            trainers = new ReadOnlyCollection<PBETrainer>(allTrainers);
        }

        public IEnumerator<PBETeam> GetEnumerator()
        {
            yield return _team0;
            yield return _team1;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    // TODO: INPC
    /// <summary>Represents a team in a specific <see cref="PBEBattle"/>.</summary>
    public sealed class PBETeam
    {
        /// <summary>The battle this team and its party belongs to.</summary>
        public PBEBattle Battle { get; }
        public PBETeam OpposingTeam { get; internal set; }
        public ReadOnlyCollection<PBETrainer> Trainers { get; }
        public byte Id { get; }
        public bool IsWild => Battle.BattleType == PBEBattleType.Wild && Id == 1;

        public string CombinedName => Trainers.Select(t => t.Name).ToArray().Andify();
        public IEnumerable<PBEBattlePokemon> CombinedParty => Trainers.SelectMany(t => t.Party);
        public IEnumerable<PBEBattlePokemon> ActiveBattlers => Battle.ActiveBattlers.Where(p => p.Team == this);
        public int NumConsciousPkmn => Trainers.Sum(t => t.NumConsciousPkmn);
        public int NumPkmnOnField => Trainers.Sum(t => t.NumPkmnOnField);

        public PBETeamStatus TeamStatus { get; set; }
        public byte LightScreenCount { get; set; }
        public byte LuckyChantCount { get; set; }
        public byte ReflectCount { get; set; }
        public byte SafeguardCount { get; set; }
        public byte SpikeCount { get; set; }
        public byte TailwindCount { get; set; }
        public byte ToxicSpikeCount { get; set; }
        public bool MonFaintedLastTurn { get; set; }
        public bool MonFaintedThisTurn { get; set; }

        // Trainer battle
        internal PBETeam(PBEBattle battle, byte id, IReadOnlyList<PBETrainerInfo> ti, List<PBETrainer> allTrainers)
        {
            int count = ti.Count;
            if (!VerifyTrainerCount(battle.BattleFormat, count))
            {
                throw new ArgumentException($"Illegal trainer count (Format: {battle.BattleFormat}, Team: {id}, Count: {count}");
            }
            foreach (PBETrainerInfo t in ti)
            {
                if (!t.IsOkayForSettings(battle.Settings))
                {
                    throw new ArgumentOutOfRangeException(nameof(ti), "Team settings do not comply with battle settings.");
                }
            }
            Battle = battle;
            Id = id;
            var trainers = new PBETrainer[ti.Count];
            for (int i = 0; i < ti.Count; i++)
            {
                trainers[i] = new PBETrainer(this, ti[i], allTrainers);
            }
            Trainers = new ReadOnlyCollection<PBETrainer>(trainers);
        }
        // Wild battle
        internal PBETeam(PBEBattle battle, byte id, PBEWildInfo wi, List<PBETrainer> allTrainers)
        {
            int count = wi.Party.Count;
            if (!VerifyTrainerCount(battle.BattleFormat, count))
            {
                throw new ArgumentException($"Illegal wild Pokémon count (Format: {battle.BattleFormat}, Count: {count}");
            }
            if (!wi.IsOkayForSettings(battle.Settings))
            {
                throw new ArgumentOutOfRangeException(nameof(wi), "Team settings do not comply with battle settings.");
            }
            Battle = battle;
            Id = id;
            Trainers = new ReadOnlyCollection<PBETrainer>(new[] { new PBETrainer(this, wi, allTrainers) });
        }
        // Remote trainer battle
        internal PBETeam(PBEBattle battle, PBEBattlePacket.PBETeamInfo info, List<PBETrainer> allTrainers)
        {
            ReadOnlyCollection<PBEBattlePacket.PBETeamInfo.PBETrainerInfo> ti = info.Trainers;
            int count = ti.Count;
            if (!VerifyTrainerCount(battle.BattleFormat, count))
            {
                throw new InvalidDataException();
            }
            Battle = battle;
            Id = info.Id;
            var trainers = new PBETrainer[ti.Count];
            for (int i = 0; i < trainers.Length; i++)
            {
                trainers[i] = new PBETrainer(this, ti[i], allTrainers);
            }
            Trainers = new ReadOnlyCollection<PBETrainer>(trainers);
        }
        private bool VerifyTrainerCount(PBEBattleFormat format, int count)
        {
            switch (format)
            {
                case PBEBattleFormat.Single:
                case PBEBattleFormat.Rotation: return count == 1;
                case PBEBattleFormat.Double: return count == 1 || count == 2;
                case PBEBattleFormat.Triple: return count == 1 || count == 3;
                default: throw new ArgumentOutOfRangeException(nameof(format));
            }
        }

        /// <summary>Gets a specific active <see cref="PBEBattlePokemon"/> by its <see cref="PBEBattlePokemon.FieldPosition"/>.</summary>
        /// <param name="pos">The <see cref="PBEFieldPosition"/> of the <see cref="PBEBattlePokemon"/>.</param>
        public PBEBattlePokemon TryGetPokemon(PBEFieldPosition pos)
        {
            return ActiveBattlers.SingleOrDefault(p => p.FieldPosition == pos);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Team {Id}:");
            sb.AppendLine($"TeamStatus: {TeamStatus}");
            //sb.AppendLine($"NumPkmn: {Party.Length}");
            sb.AppendLine($"NumConsciousPkmn: {NumConsciousPkmn}");
            sb.AppendLine($"NumPkmnOnField: {NumPkmnOnField}");
            return sb.ToString();
        }
    }
}
