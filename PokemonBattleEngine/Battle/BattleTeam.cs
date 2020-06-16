using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Legality;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed class PBETeamInfo
    {
        public IPBEPokemonCollection Party { get; }
        public string TrainerName { get; }

        public PBETeamInfo(IPBEPokemonCollection party, string trainerName)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }
            if (string.IsNullOrWhiteSpace(trainerName))
            {
                throw new ArgumentOutOfRangeException(nameof(trainerName));
            }
            Party = party;
            TrainerName = trainerName;
        }

        public bool IsOkayForSettings(PBESettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (!settings.IsReadOnly)
            {
                throw new ArgumentException("Settings must be read-only.", nameof(settings));
            }
            if (Party is PBELegalPokemonCollection lp)
            {
                return settings.Equals(lp.Settings);
            }
            return true;
        }
    }
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

        internal PBETeams(PBEBattle battle, PBETeamInfo party0, PBETeamInfo party1)
        {
            _team0 = new PBETeam(battle, 0, party0);
            _team1 = new PBETeam(battle, 1, party1);
            _team0.OpposingTeam = _team1;
            _team1.OpposingTeam = _team0;
        }
        internal PBETeams(PBEBattle battle)
        {
            _team0 = new PBETeam(battle, 0);
            _team1 = new PBETeam(battle, 1);
            _team0.OpposingTeam = _team1;
            _team1.OpposingTeam = _team0;
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
        public byte Id { get; }
        public string TrainerName { get; set; } // Setter is public because a client cannot submit the opponent's team
        public PBEList<PBEBattlePokemon> Party { get; }
        public PBETeam OpposingTeam { get; internal set; }

        public PBEBattlePokemon[] ActiveBattlers => Battle.ActiveBattlers.Where(p => p.Team == this).OrderBy(p => p.FieldPosition).ToArray();
        public int NumConsciousPkmn => Party.Count(p => p.HP > 0);
        public int NumPkmnOnField => Party.Count(p => p.FieldPosition != PBEFieldPosition.None);

        public List<PBEBattlePokemon> ActionsRequired { get; } = new List<PBEBattlePokemon>(3); // PBEBattleState.WaitingForActions
        public byte SwitchInsRequired { get; set; } // PBEBattleState.WaitingForSwitchIns
        public List<(PBEBattlePokemon, PBEFieldPosition)> SwitchInQueue { get; } = new List<(PBEBattlePokemon, PBEFieldPosition)>(3); // PBEBattleState.WaitingForSwitchIns

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

        internal PBETeam(PBEBattle battle, byte id)
        {
            Battle = battle;
            Id = id;
            Party = new PBEList<PBEBattlePokemon>(Battle.Settings.MaxPartySize);
        }
        internal PBETeam(PBEBattle battle, byte id, PBETeamInfo ti) : this(battle, id)
        {
            CreateParty(ti);
        }
        internal void CreateParty(PBETeamInfo ti)
        {
            if (!ti.IsOkayForSettings(Battle.Settings))
            {
                throw new ArgumentOutOfRangeException(nameof(ti), "Team settings do not comply with battle settings.");
            }
            TrainerName = ti.TrainerName;
            IPBEPokemonCollection party = ti.Party;
            for (int i = 0; i < party.Count; i++)
            {
                IPBEPokemon pkmn = party[i];
                byte id = (byte)((Id * Battle.Settings.MaxPartySize) + i);
                if (pkmn is IPBEPartyPokemon partyPkmn)
                {
                    new PBEBattlePokemon(this, id, partyPkmn);
                }
                else
                {
                    new PBEBattlePokemon(this, id, pkmn);
                }
            }
        }

        /// <summary>Gets a specific active <see cref="PBEBattlePokemon"/> by its <see cref="PBEBattlePokemon.FieldPosition"/>.</summary>
        /// <param name="pos">The <see cref="PBEFieldPosition"/> of the <see cref="PBEBattlePokemon"/>.</param>
        public PBEBattlePokemon TryGetPokemon(PBEFieldPosition pos)
        {
            return ActiveBattlers.SingleOrDefault(p => p.FieldPosition == pos);
        }
        /// <summary>Gets a specific <see cref="PBEBattlePokemon"/> by its ID.</summary>
        /// <param name="pkmnId">The ID of the <see cref="PBEBattlePokemon"/>.</param>
        public PBEBattlePokemon TryGetPokemon(byte pkmnId)
        {
            return Party.SingleOrDefault(p => p.Id == pkmnId);
        }

        public static void Remove(PBEBattlePokemon pokemon)
        {
            PBETeam team = pokemon.Team;
            team.Party.Remove(pokemon);
        }
        public static void SwitchTwoPokemon(PBEBattlePokemon a, PBEFieldPosition pos)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            if (pos == PBEFieldPosition.None || pos >= PBEFieldPosition.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(pos));
            }
            PBETeam team = a.Team;
            PBEBattlePokemon b = team.Party[PBEBattleUtils.GetFieldPositionIndex(team.Battle.BattleFormat, pos)];
            if (a != b)
            {
                team.Party.Swap(a, b);
            }
        }
        public static void SwitchTwoPokemon(PBEBattlePokemon a, PBEBattlePokemon b)
        {
            if (a != b)
            {
                if (a == null)
                {
                    throw new ArgumentNullException(nameof(a));
                }
                if (b == null)
                {
                    throw new ArgumentNullException(nameof(b));
                }
                PBETeam team = a.Team;
                if (team != b.Team)
                {
                    throw new ArgumentException(nameof(a.Team));
                }
                team.Party.Swap(a, b);
            }
        }

        internal void ToBytes(EndianBinaryWriter w)
        {
            w.Write(TrainerName, true);
            sbyte count = (sbyte)Party.Count;
            w.Write(count);
            for (int i = 0; i < count; i++)
            {
                Party[i].ToBytes(w);
            }
        }
        internal void FromBytes(EndianBinaryReader r)
        {
            TrainerName = r.ReadStringNullTerminated();
            sbyte count = r.ReadSByte();
            for (int i = 0; i < count; i++)
            {
                new PBEBattlePokemon(r, this);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{TrainerName}'s team:");
            sb.AppendLine($"TeamStatus: {TeamStatus}");
            sb.AppendLine($"NumPkmn: {Party.Count}");
            sb.AppendLine($"NumConsciousPkmn: {NumConsciousPkmn}");
            sb.AppendLine($"NumPkmnOnField: {NumPkmnOnField}");
            return sb.ToString();
        }
    }
}
