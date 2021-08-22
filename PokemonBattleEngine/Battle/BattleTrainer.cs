using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed class PBETrainers : IReadOnlyList<PBETrainer>
    {
        private readonly List<PBETrainer> _trainers;

        public int Count => _trainers.Count;
        public PBETrainer this[int index] => _trainers[index];

        internal PBETrainers(List<PBETrainer> trainers)
        {
            _trainers = trainers;
        }

        public IEnumerator<PBETrainer> GetEnumerator()
        {
            return _trainers.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _trainers.GetEnumerator();
        }
    }
    public sealed partial class PBETrainer
    {
        public PBEBattle Battle { get; }
        public PBETeam Team { get; }
        public PBEList<PBEBattlePokemon> Party { get; }
        public string Name { get; }
        public bool GainsEXP { get; }
        public PBEBattleInventory Inventory { get; }
        public byte Id { get; }
        public bool IsWild => Team.IsWild;

        public List<PBEBattlePokemon> ActiveBattlers => Battle.ActiveBattlers.FindAll(p => p.Trainer == this);
        public IEnumerable<PBEBattlePokemon> ActiveBattlersOrdered => ActiveBattlers.OrderBy(p => p.FieldPosition);
        public int NumConsciousPkmn => Party.Count(p => p.CanBattle);
        public int NumPkmnOnField => Party.Count(p => p.FieldPosition != PBEFieldPosition.None);

        public bool RequestedFlee { get; set; }
        public List<PBEBattlePokemon> ActionsRequired { get; } = new(3); // PBEBattleState.WaitingForActions
        public byte SwitchInsRequired { get; set; } // PBEBattleState.WaitingForSwitchIns
        public List<(PBEBattlePokemon Pkmn, PBEFieldPosition Pos)> SwitchInQueue { get; } = new(3); // PBEBattleState.WaitingForSwitchIns

        // Trainer battle / wild battle
        private PBETrainer(PBETeam team, PBETrainerInfoBase ti, string name, ReadOnlyCollection<(PBEItem Item, uint Quantity)>? inventory, List<PBETrainer> trainers)
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
                    _ = new PBEBattlePokemon(this, i, partyPkmn);
                }
                else
                {
                    _ = new PBEBattlePokemon(this, i, pkmn);
                }
            }
            trainers.Add(this);
        }
        // Trainer battle
        internal PBETrainer(PBETeam team, PBETrainerInfo ti, List<PBETrainer> trainers)
            : this(team, ti, ti.Name, ti.Inventory, trainers)
        {
            GainsEXP = ti.GainsEXP;
        }
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
                PBETrainer t = a.Trainer;
                if (t != b.Trainer)
                {
                    throw new ArgumentException(nameof(a.Trainer));
                }
                t.Party.Swap(a, b);
            }
        }

        public bool IsSpotOccupied(PBEFieldPosition pos)
        {
            foreach (PBEBattlePokemon p in ActiveBattlers)
            {
                if (p.FieldPosition == pos)
                {
                    return true;
                }
            }
            return false;
        }
        public bool TryGetPokemon(PBEFieldPosition pos, [NotNullWhen(true)] out PBEBattlePokemon? pkmn)
        {
            foreach (PBEBattlePokemon p in ActiveBattlers)
            {
                if (p.FieldPosition == pos)
                {
                    pkmn = p;
                    return true;
                }
            }
            pkmn = null;
            return false;
        }
        public bool TryGetPokemon(byte pkmnId, [NotNullWhen(true)] out PBEBattlePokemon? pkmn)
        {
            foreach (PBEBattlePokemon p in Party)
            {
                if (p.Id == pkmnId)
                {
                    pkmn = p;
                    return true;
                }
            }
            pkmn = null;
            return false;
        }
        public PBEBattlePokemon GetPokemon(PBEFieldPosition pos)
        {
            foreach (PBEBattlePokemon p in ActiveBattlers)
            {
                if (p.FieldPosition == pos)
                {
                    return p;
                }
            }
            throw new InvalidOperationException();
        }
        public PBEBattlePokemon GetPokemon(byte pkmnId)
        {
            foreach (PBEBattlePokemon p in Party)
            {
                if (p.Id == pkmnId)
                {
                    return p;
                }
            }
            throw new InvalidOperationException();
        }
    }
}
