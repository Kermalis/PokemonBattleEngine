using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed class PBETurnAction
    {
        public byte PokemonId { get; }
        public PBETurnDecision Decision { get; }
        public PBEMove FightMove { get; }
        public PBETurnTarget FightTargets { get; internal set; } // Internal set because of PBEMoveTarget.RandomFoeSurrounding (Shouldn't this happen at runtime?)
        public PBEItem UseItem { get; }
        public byte SwitchPokemonId { get; }

        internal PBETurnAction(EndianBinaryReader r)
        {
            PokemonId = r.ReadByte();
            Decision = r.ReadEnum<PBETurnDecision>();
            switch (Decision)
            {
                case PBETurnDecision.Fight:
                {
                    FightMove = r.ReadEnum<PBEMove>();
                    FightTargets = r.ReadEnum<PBETurnTarget>();
                    break;
                }
                case PBETurnDecision.Item:
                {
                    UseItem = r.ReadEnum<PBEItem>();
                    break;
                }
                case PBETurnDecision.SwitchOut:
                {
                    SwitchPokemonId = r.ReadByte();
                    break;
                }
                case PBETurnDecision.WildFlee: break;
                default: throw new ArgumentOutOfRangeException(nameof(Decision));
            }
        }
        // Fight
        public PBETurnAction(PBEBattlePokemon pokemon, PBEMove fightMove, PBETurnTarget fightTargets)
            : this(pokemon.Id, fightMove, fightTargets) { }
        public PBETurnAction(byte pokemonId, PBEMove fightMove, PBETurnTarget fightTargets)
        {
            PokemonId = pokemonId;
            Decision = PBETurnDecision.Fight;
            FightMove = fightMove;
            FightTargets = fightTargets;
        }
        // Item
        public PBETurnAction(PBEBattlePokemon pokemon, PBEItem item)
            : this(pokemon.Id, item) { }
        public PBETurnAction(byte pokemonId, PBEItem item)
        {
            PokemonId = pokemonId;
            Decision = PBETurnDecision.Item;
            UseItem = item;
        }
        // Switch
        public PBETurnAction(PBEBattlePokemon pokemon, PBEBattlePokemon switchPokemon)
            : this(pokemon.Id, switchPokemon.Id) { }
        public PBETurnAction(byte pokemonId, byte switchPokemonId)
        {
            PokemonId = pokemonId;
            Decision = PBETurnDecision.SwitchOut;
            SwitchPokemonId = switchPokemonId;
        }
        // Internal wild flee
        internal PBETurnAction(PBEBattlePokemon pokemon)
            : this(pokemon.Id) { }
        internal PBETurnAction(byte pokemonId)
        {
            PokemonId = pokemonId;
            Decision = PBETurnDecision.WildFlee;
        }

        internal void ToBytes(EndianBinaryWriter w)
        {
            w.Write(PokemonId);
            w.Write(Decision);
            switch (Decision)
            {
                case PBETurnDecision.Fight:
                {
                    w.Write(FightMove);
                    w.Write(FightTargets);
                    break;
                }
                case PBETurnDecision.Item:
                {
                    w.Write(UseItem);
                    break;
                }
                case PBETurnDecision.SwitchOut:
                {
                    w.Write(SwitchPokemonId);
                    break;
                }
                case PBETurnDecision.WildFlee: break;
                default: throw new ArgumentOutOfRangeException(nameof(Decision));
            }
        }
    }
    public sealed class PBESwitchIn
    {
        public byte PokemonId { get; }
        public PBEFieldPosition Position { get; }

        internal PBESwitchIn(EndianBinaryReader r)
        {
            PokemonId = r.ReadByte();
            Position = r.ReadEnum<PBEFieldPosition>();
        }
        public PBESwitchIn(PBEBattlePokemon pokemon, PBEFieldPosition position)
            : this(pokemon.Id, position) { }
        public PBESwitchIn(byte pokemonId, PBEFieldPosition position)
        {
            PokemonId = pokemonId;
            Position = position;
        }

        internal void ToBytes(EndianBinaryWriter w)
        {
            w.Write(PokemonId);
            w.Write(Position);
        }
    }
    public sealed partial class PBEBattle
    {
        internal static string AreActionsValid(PBETrainer trainer, IReadOnlyCollection<PBETurnAction> actions)
        {
            if (actions == null || actions.Any(a => a == null))
            {
                throw new ArgumentNullException(nameof(actions));
            }
            if (trainer.Battle.BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForActions} to validate actions.");
            }
            if (trainer.ActionsRequired.Count == 0)
            {
                return "Actions were already submitted";
            }
            if (actions.Count != trainer.ActionsRequired.Count)
            {
                return $"Invalid amount of actions submitted; required amount is {trainer.ActionsRequired.Count}";
            }
            var verified = new List<PBEBattlePokemon>(trainer.ActionsRequired.Count);
            var standBy = new List<PBEBattlePokemon>(trainer.ActionsRequired.Count);
            var items = new Dictionary<PBEItem, int>(trainer.ActionsRequired.Count);
            foreach (PBETurnAction action in actions)
            {
                PBEBattlePokemon pkmn = trainer.TryGetPokemon(action.PokemonId);
                if (pkmn is null)
                {
                    return $"Invalid Pokémon ID ({action.PokemonId})";
                }
                if (!trainer.ActionsRequired.Contains(pkmn))
                {
                    return $"Pokémon {action.PokemonId} not looking for actions";
                }
                if (verified.Contains(pkmn))
                {
                    return $"Pokémon {action.PokemonId} was multiple actions";
                }
                switch (action.Decision)
                {
                    case PBETurnDecision.Fight:
                    {
                        if (Array.IndexOf(pkmn.GetUsableMoves(), action.FightMove) == -1)
                        {
                            return $"{action.FightMove} is not usable by Pokémon {action.PokemonId}";
                        }
                        if (action.FightMove == pkmn.TempLockedMove && action.FightTargets != pkmn.TempLockedTargets)
                        {
                            return $"Pokémon {action.PokemonId} must target {pkmn.TempLockedTargets}";
                        }
                        if (!AreTargetsValid(pkmn, action.FightMove, action.FightTargets))
                        {
                            return $"Invalid move targets for Pokémon {action.PokemonId}'s {action.FightMove}";
                        }
                        break;
                    }
                    case PBETurnDecision.Item:
                    {
                        if (pkmn.TempLockedMove != PBEMove.None)
                        {
                            return $"Pokémon {action.PokemonId} must use {pkmn.TempLockedMove}";
                        }
                        if (!trainer.Inventory.TryGetValue(action.UseItem, out PBEBattleInventory.PBEBattleInventorySlot slot))
                        {
                            return $"Trainer \"{trainer.Name}\" does not have any {action.UseItem}";
                        }
                        bool used = items.TryGetValue(action.UseItem, out int amtUsed);
                        if (!used)
                        {
                            amtUsed = 0;
                        }
                        long newAmt = slot.Quantity - amtUsed;
                        if (newAmt <= 0)
                        {
                            return $"Tried to use too many {action.UseItem}";
                        }
                        amtUsed++;
                        if (used)
                        {
                            items[action.UseItem] = amtUsed;
                        }
                        else
                        {
                            items.Add(action.UseItem, amtUsed);
                        }
                        break;
                    }
                    case PBETurnDecision.SwitchOut:
                    {
                        if (!pkmn.CanSwitchOut())
                        {
                            return $"Pokémon {action.PokemonId} cannot switch out";
                        }
                        PBEBattlePokemon switchPkmn = trainer.TryGetPokemon(action.SwitchPokemonId);
                        if (switchPkmn is null)
                        {
                            return $"Invalid switch Pokémon ID ({action.PokemonId})";
                        }
                        if (switchPkmn.HP == 0)
                        {
                            return $"Switch Pokémon {action.PokemonId} is fainted";
                        }
                        if (switchPkmn.FieldPosition != PBEFieldPosition.None)
                        {
                            return $"Switch Pokémon {action.PokemonId} is already on the field";
                        }
                        if (standBy.Contains(switchPkmn))
                        {
                            return $"Switch Pokémon {action.PokemonId} was asked to be switched in multiple times";
                        }
                        standBy.Add(switchPkmn);
                        break;
                    }
                    default: return $"Invalid turn decision ({action.Decision})";
                }
                verified.Add(pkmn);
            }
            return null;
        }
        internal static string SelectActionsIfValid(PBETrainer trainer, IReadOnlyCollection<PBETurnAction> actions)
        {
            string valid = AreActionsValid(trainer, actions);
            if (valid is null)
            {
                trainer.ActionsRequired.Clear();
                foreach (PBETurnAction action in actions)
                {
                    PBEBattlePokemon pkmn = trainer.TryGetPokemon(action.PokemonId);
                    if (action.Decision == PBETurnDecision.Fight && pkmn.GetMoveTargets(action.FightMove) == PBEMoveTarget.RandomFoeSurrounding)
                    {
                        switch (trainer.Battle.BattleFormat)
                        {
                            case PBEBattleFormat.Single:
                            case PBEBattleFormat.Rotation:
                            {
                                action.FightTargets = PBETurnTarget.FoeCenter;
                                break;
                            }
                            case PBEBattleFormat.Double:
                            {
                                action.FightTargets = trainer.Battle._rand.RandomBool() ? PBETurnTarget.FoeLeft : PBETurnTarget.FoeRight;
                                break;
                            }
                            case PBEBattleFormat.Triple:
                            {
                                if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                {
                                    action.FightTargets = trainer.Battle._rand.RandomBool() ? PBETurnTarget.FoeCenter : PBETurnTarget.FoeRight;
                                }
                                else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                {
                                    PBETeam oppTeam = trainer.Team.OpposingTeam;
                                    int r; // Keep randomly picking until a non-fainted foe is selected
                                roll:
                                    r = trainer.Battle._rand.RandomInt(0, 2);
                                    if (r == 0)
                                    {
                                        if (oppTeam.TryGetPokemon(PBEFieldPosition.Left) != null)
                                        {
                                            action.FightTargets = PBETurnTarget.FoeLeft;
                                        }
                                        else
                                        {
                                            goto roll;
                                        }
                                    }
                                    else if (r == 1)
                                    {
                                        if (oppTeam.TryGetPokemon(PBEFieldPosition.Center) != null)
                                        {
                                            action.FightTargets = PBETurnTarget.FoeCenter;
                                        }
                                        else
                                        {
                                            goto roll;
                                        }
                                    }
                                    else
                                    {
                                        if (oppTeam.TryGetPokemon(PBEFieldPosition.Right) != null)
                                        {
                                            action.FightTargets = PBETurnTarget.FoeRight;
                                        }
                                        else
                                        {
                                            goto roll;
                                        }
                                    }
                                }
                                else
                                {
                                    action.FightTargets = trainer.Battle._rand.RandomBool() ? PBETurnTarget.FoeLeft : PBETurnTarget.FoeCenter;
                                }
                                break;
                            }
                            default: throw new ArgumentOutOfRangeException(nameof(trainer.Battle.BattleFormat));
                        }
                    }
                    pkmn.TurnAction = action;
                }
                if (trainer.Battle.Trainers.All(t => t.ActionsRequired.Count == 0))
                {
                    trainer.Battle.BattleState = PBEBattleState.ReadyToRunTurn;
                    trainer.Battle.OnStateChanged?.Invoke(trainer.Battle);
                }
            }
            return valid;
        }

        internal static string AreSwitchesValid(PBETrainer trainer, IReadOnlyCollection<PBESwitchIn> switches)
        {
            if (switches == null || switches.Any(s => s == null))
            {
                throw new ArgumentNullException(nameof(switches));
            }
            if (trainer.Battle.BattleState != PBEBattleState.WaitingForSwitchIns)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForSwitchIns} to validate switches.");
            }
            if (trainer.SwitchInsRequired == 0)
            {
                return "Switches were already submitted";
            }
            if (switches.Count != trainer.SwitchInsRequired)
            {
                return $"Invalid amount of switches submitted; required amount is {trainer.SwitchInsRequired}";
            }
            var verified = new List<PBEBattlePokemon>(trainer.SwitchInsRequired);
            foreach (PBESwitchIn s in switches)
            {
                if (s.Position == PBEFieldPosition.None || s.Position >= PBEFieldPosition.MAX || !trainer.OwnsSpot(s.Position))
                {
                    return $"Invalid position ({s.PokemonId})";
                }
                PBEBattlePokemon pkmn = trainer.TryGetPokemon(s.PokemonId);
                if (pkmn is null)
                {
                    return $"Invalid Pokémon ID ({s.PokemonId})";
                }
                if (pkmn.HP == 0)
                {
                    return $"Pokémon {s.PokemonId} is fainted";
                }
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    return $"Pokémon {s.PokemonId} is already on the field";
                }
                if (verified.Contains(pkmn))
                {
                    return $"Pokémon {s.PokemonId} was asked to be switched in multiple times";
                }
                verified.Add(pkmn);
            }
            return null;
        }
        internal static string SelectSwitchesIfValid(PBETrainer trainer, IReadOnlyCollection<PBESwitchIn> switches)
        {
            string valid = AreSwitchesValid(trainer, switches);
            if (valid is null)
            {
                trainer.SwitchInsRequired = 0;
                foreach (PBESwitchIn s in switches)
                {
                    PBEBattlePokemon pkmn = trainer.TryGetPokemon(s.PokemonId);
                    trainer.SwitchInQueue.Add((pkmn, s.Position));
                }
                if (trainer.Battle.Trainers.All(t => t.SwitchInsRequired == 0))
                {
                    trainer.Battle.BattleState = PBEBattleState.ReadyToRunSwitches;
                    trainer.Battle.OnStateChanged?.Invoke(trainer.Battle);
                }
            }
            return valid;
        }

        internal static string IsFleeValid(PBETrainer trainer)
        {
            if (trainer.Battle.BattleType != PBEBattleType.Wild)
            {
                throw new InvalidOperationException($"{nameof(BattleType)} must be {PBEBattleType.Wild} to flee.");
            }
            if (trainer.Battle.BattleState == PBEBattleState.WaitingForActions)
            {
                if (trainer.ActionsRequired.Count == 0)
                {
                    return "Actions were already submitted";
                }
                PBEBattlePokemon pkmn = trainer.ActiveBattlersOrdered.First();
                if (pkmn.TempLockedMove != PBEMove.None)
                {
                    return $"Pokémon {pkmn.Id} must use {pkmn.TempLockedMove}";
                }
            }
            else if (trainer.Battle.BattleState != PBEBattleState.WaitingForSwitchIns)
            {
                if (trainer.SwitchInsRequired == 0)
                {
                    return "Switches were already submitted";
                }
            }
            else
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForActions} or {PBEBattleState.WaitingForSwitchIns} to flee.");
            }
            return null;
        }
        internal static string SelectFleeIfValid(PBETrainer trainer)
        {
            string valid = IsFleeValid(trainer);
            if (valid is null)
            {
                trainer.RequestedFlee = true;
                if (trainer.Battle.BattleState == PBEBattleState.WaitingForActions)
                {
                    trainer.ActionsRequired.Clear();
                    if (trainer.Battle.Trainers.All(t => t.ActionsRequired.Count == 0))
                    {
                        trainer.Battle.BattleState = PBEBattleState.ReadyToRunTurn;
                        trainer.Battle.OnStateChanged?.Invoke(trainer.Battle);
                    }
                }
                else
                {
                    trainer.SwitchInsRequired = 0;
                    if (trainer.Battle.Trainers.All(t => t.SwitchInsRequired == 0))
                    {
                        trainer.Battle.BattleState = PBEBattleState.ReadyToRunSwitches;
                        trainer.Battle.OnStateChanged?.Invoke(trainer.Battle);
                    }
                }
            }
            return valid;
        }
    }
    public sealed partial class PBETrainer
    {
        public string AreActionsValid(params PBETurnAction[] actions)
        {
            return PBEBattle.AreActionsValid(this, actions);
        }
        public string AreActionsValid(IReadOnlyCollection<PBETurnAction> actions)
        {
            return PBEBattle.AreActionsValid(this, actions);
        }
        public string SelectActionsIfValid(params PBETurnAction[] actions)
        {
            return PBEBattle.SelectActionsIfValid(this, actions);
        }
        public string SelectActionsIfValid(IReadOnlyCollection<PBETurnAction> actions)
        {
            return PBEBattle.SelectActionsIfValid(this, actions);
        }

        public string AreSwitchesValid(params PBESwitchIn[] switches)
        {
            return PBEBattle.AreSwitchesValid(this, switches);
        }
        public string AreSwitchesValid(IReadOnlyCollection<PBESwitchIn> switches)
        {
            return PBEBattle.AreSwitchesValid(this, switches);
        }
        public string SelectSwitchesIfValid(params PBESwitchIn[] switches)
        {
            return PBEBattle.SelectSwitchesIfValid(this, switches);
        }
        public string SelectSwitchesIfValid(IReadOnlyCollection<PBESwitchIn> switches)
        {
            return PBEBattle.SelectSwitchesIfValid(this, switches);
        }

        public string IsFleeValid()
        {
            return PBEBattle.IsFleeValid(this);
        }
        public string SelectFleeIfValid()
        {
            return PBEBattle.SelectFleeIfValid(this);
        }
    }
}
