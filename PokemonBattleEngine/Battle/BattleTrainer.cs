using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Legality;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public abstract class PBETrainerInfoBase
    {
        public ReadOnlyCollection<IPBEPokemon> Party { get; }
        private readonly PBESettings _requiredSettings;

        protected PBETrainerInfoBase(IPBEPokemonCollection party)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }
            if (party.Count < 1)
            {
                throw new ArgumentException("Party count must be at least 1", nameof(party));
            }
            if (party is PBELegalPokemonCollection lp)
            {
                _requiredSettings = lp.Settings;
            }
            Party = new ReadOnlyCollection<IPBEPokemon>(party.ToArray());
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
            if (_requiredSettings != null)
            {
                return settings.Equals(_requiredSettings);
            }
            return true;
        }
    }
    public sealed class PBETrainerInfo : PBETrainerInfoBase
    {
        public string Name { get; }

        public PBETrainerInfo(IPBEPokemonCollection party, string name)
            : base(party)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentOutOfRangeException(nameof(name));
            }
            Name = name;
        }
    }
    public sealed class PBEWildInfo : PBETrainerInfoBase
    {
        public PBEWildInfo(IPBEPokemonCollection party)
            : base(party) { }
    }
    public sealed class PBETrainer
    {
        public PBEBattle Battle { get; }
        public PBETeam Team { get; }
        public PBEList<PBEBattlePokemon> Party { get; }
        public string Name { get; }
        public byte Id { get; }
        public bool IsWild => Team.IsWild;

        public IEnumerable<PBEBattlePokemon> ActiveBattlers => Battle.ActiveBattlers.Where(p => p.Trainer == this);
        public int NumConsciousPkmn => Party.Count(p => p.HP > 0);
        public int NumPkmnOnField => Party.Count(p => p.FieldPosition != PBEFieldPosition.None);

        public List<PBEBattlePokemon> ActionsRequired { get; } = new List<PBEBattlePokemon>(3); // PBEBattleState.WaitingForActions
        public byte SwitchInsRequired { get; set; } // PBEBattleState.WaitingForSwitchIns
        public List<(PBEBattlePokemon, PBEFieldPosition)> SwitchInQueue { get; } = new List<(PBEBattlePokemon, PBEFieldPosition)>(3); // PBEBattleState.WaitingForSwitchIns

        // Trainer battle / wild battle
        private PBETrainer(PBETeam team, PBETrainerInfoBase ti, string name, List<PBETrainer> trainers)
        {
            Battle = team.Battle;
            Team = team;
            Id = (byte)trainers.Count;
            Name = name;
            ReadOnlyCollection<IPBEPokemon> tiParty = ti.Party;
            Party = new PBEList<PBEBattlePokemon>(tiParty.Count);
            for (byte i = 0; i < tiParty.Count; i++)
            {
                IPBEPokemon pkmn = tiParty[i];
                if (pkmn is IPBEPartyPokemon partyPkmn)
                {
                    new PBEBattlePokemon(this, i, partyPkmn);
                }
                else
                {
                    new PBEBattlePokemon(this, i, pkmn);
                }
            }
            trainers.Add(this);
        }
        // Trainer battle
        internal PBETrainer(PBETeam team, PBETrainerInfo ti, List<PBETrainer> trainers)
            : this(team, ti, ti.Name, trainers) { }
        // Wild battle
        internal PBETrainer(PBETeam team, PBEWildInfo wi, List<PBETrainer> trainers)
            : this(team, wi, "The wild Pokémon", trainers) { }
        // Remote trainer battle
        // TODO: Remote wild battle/replay
        internal PBETrainer(PBETeam team, PBEBattlePacket.PBETeamInfo.PBETrainerInfo info, List<PBETrainer> trainers)
        {
            Battle = team.Battle;
            Team = team;
            Id = info.Id;
            Name = info.Name;
            Party = new PBEList<PBEBattlePokemon>(info.Party.Select(p => new PBEBattlePokemon(this, p)));
            trainers.Add(this);
        }

        public static void Remove(PBEBattlePokemon pokemon)
        {
            pokemon.Trainer.Party.Remove(pokemon);
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
            PBETrainer t = a.Trainer;
            PBEBattlePokemon b = t.Party[t.GetFieldPositionIndex(pos)];
            if (a != b)
            {
                t.Party.Swap(a, b);
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
                PBETrainer t = a.Trainer;
                if (t != b.Trainer)
                {
                    throw new ArgumentException(nameof(a.Trainer));
                }
                t.Party.Swap(a, b);
            }
        }

        public PBEBattlePokemon TryGetPokemon(PBEFieldPosition pos)
        {
            return ActiveBattlers.SingleOrDefault(p => p.FieldPosition == pos);
        }
        public PBEBattlePokemon TryGetPokemon(byte pkmnId)
        {
            return Party.SingleOrDefault(p => p.Id == pkmnId);
        }
    }
}
