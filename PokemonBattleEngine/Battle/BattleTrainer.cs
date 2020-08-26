using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Legality;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
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
        public ReadOnlyCollection<(PBEItem Item, uint Quantity)> Inventory { get; }

        public PBETrainerInfo(IPBEPokemonCollection party, string name, IList<(PBEItem Item, uint Quantity)> inventory = null)
            : base(party)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentOutOfRangeException(nameof(name));
            }
            Name = name;
            if (inventory is null || inventory.Count == 0)
            {
                Inventory = PBEEmptyReadOnlyCollection<(PBEItem, uint)>.Value;
            }
            else
            {
                Inventory = new ReadOnlyCollection<(PBEItem, uint)>(inventory);
            }
        }
    }
    public sealed class PBEWildInfo : PBETrainerInfoBase
    {
        public PBEWildInfo(IPBEPokemonCollection party)
            : base(party) { }
    }
    public sealed partial class PBETrainer
    {
        public PBEBattle Battle { get; }
        public PBETeam Team { get; }
        public PBEList<PBEBattlePokemon> Party { get; }
        public string Name { get; }
        public PBEBattleInventory Inventory { get; }
        public byte Id { get; }
        public bool IsWild => Team.IsWild;

        public IEnumerable<PBEBattlePokemon> ActiveBattlers => Battle.ActiveBattlers.Where(p => p.Trainer == this);
        public IEnumerable<PBEBattlePokemon> ActiveBattlersOrdered => ActiveBattlers.OrderBy(p => p.FieldPosition);
        public int NumConsciousPkmn => Party.Count(p => p.HP > 0);
        public int NumPkmnOnField => Party.Count(p => p.FieldPosition != PBEFieldPosition.None);

        public bool RequestedFlee { get; set; }
        public List<PBEBattlePokemon> ActionsRequired { get; } = new List<PBEBattlePokemon>(3); // PBEBattleState.WaitingForActions
        public byte SwitchInsRequired { get; set; } // PBEBattleState.WaitingForSwitchIns
        public List<(PBEBattlePokemon, PBEFieldPosition)> SwitchInQueue { get; } = new List<(PBEBattlePokemon, PBEFieldPosition)>(3); // PBEBattleState.WaitingForSwitchIns

        // Trainer battle / wild battle
        private PBETrainer(PBETeam team, PBETrainerInfoBase ti, string name, ReadOnlyCollection<(PBEItem Item, uint Quantity)> inventory, List<PBETrainer> trainers)
        {
            Battle = team.Battle;
            Team = team;
            Id = (byte)trainers.Count;
            Name = name;
            if (inventory is null || inventory.Count == 0) // Wild trainer
            {
                Inventory = PBEBattleInventory.Empty();
            }
            else
            {
                Inventory = new PBEBattleInventory(inventory);
            }
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
            : this(team, ti, ti.Name, ti.Inventory, trainers) { }
        // Wild battle
        internal PBETrainer(PBETeam team, PBEWildInfo wi, List<PBETrainer> trainers)
            : this(team, wi, "The wild Pokémon", null, trainers) { }
        // Remote battle
        internal PBETrainer(PBETeam team, PBEBattlePacket.PBETeamInfo.PBETrainerInfo info, List<PBETrainer> trainers)
        {
            Battle = team.Battle;
            Team = team;
            Id = info.Id;
            Name = team.IsWild ? "The wild Pokémon" : info.Name;
            Inventory = info.Inventory.Count == 0 ? PBEBattleInventory.Empty() : new PBEBattleInventory(info.Inventory);
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
