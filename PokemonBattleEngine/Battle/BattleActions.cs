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
                case PBETurnDecision.SwitchOut:
                {
                    SwitchPokemonId = r.ReadByte();
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(Decision));
            }
        }
        public PBETurnAction(PBEBattlePokemon pokemon, PBEMove fightMove, PBETurnTarget fightTargets)
            : this(pokemon.Id, fightMove, fightTargets) { }
        public PBETurnAction(byte pokemonId, PBEMove fightMove, PBETurnTarget fightTargets)
        {
            PokemonId = pokemonId;
            Decision = PBETurnDecision.Fight;
            FightMove = fightMove;
            FightTargets = fightTargets;
        }
        public PBETurnAction(PBEBattlePokemon pokemon, PBEBattlePokemon switchPokemon)
            : this(pokemon.Id, switchPokemon.Id) { }
        public PBETurnAction(byte pokemonId, byte switchPokemonId)
        {
            PokemonId = pokemonId;
            Decision = PBETurnDecision.SwitchOut;
            SwitchPokemonId = switchPokemonId;
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
                case PBETurnDecision.SwitchOut:
                {
                    w.Write(SwitchPokemonId);
                    break;
                }
                throw new ArgumentOutOfRangeException(nameof(Decision));
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
        public static bool AreActionsValid(PBETrainer trainer, params PBETurnAction[] actions)
        {
            return AreActionsValid(trainer, (IReadOnlyList<PBETurnAction>)actions);
        }
        /// <summary>Determines whether chosen actions are valid.</summary>
        /// <param name="trainer">The trainer the inputted actions belong to.</param>
        /// <param name="actions">The actions the team wishes to execute.</param>
        /// <returns>False if the team already chose actions or the actions are illegal, True otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        public static bool AreActionsValid(PBETrainer trainer, IReadOnlyList<PBETurnAction> actions)
        {
            if (trainer == null)
            {
                throw new ArgumentNullException(nameof(trainer));
            }
            if (actions == null || actions.Any(a => a == null))
            {
                throw new ArgumentNullException(nameof(actions));
            }
            if (trainer.Battle.BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForActions} to validate actions.");
            }
            if (trainer.ActionsRequired.Count == 0 || actions.Count != trainer.ActionsRequired.Count)
            {
                return false;
            }
            var verified = new List<PBEBattlePokemon>(trainer.ActionsRequired.Count);
            var standBy = new List<PBEBattlePokemon>(trainer.ActionsRequired.Count);
            foreach (PBETurnAction action in actions)
            {
                PBEBattlePokemon pkmn = trainer.TryGetPokemon(action.PokemonId);
                if (pkmn == null || !trainer.ActionsRequired.Contains(pkmn) || verified.Contains(pkmn))
                {
                    return false;
                }
                switch (action.Decision)
                {
                    case PBETurnDecision.Fight:
                    {
                        if (Array.IndexOf(pkmn.GetUsableMoves(), action.FightMove) == -1
                            || (action.FightMove == pkmn.TempLockedMove && action.FightTargets != pkmn.TempLockedTargets)
                            || !AreTargetsValid(pkmn, action.FightMove, action.FightTargets)
                            )
                        {
                            return false;
                        }
                        break;
                    }
                    case PBETurnDecision.SwitchOut:
                    {
                        if (!pkmn.CanSwitchOut())
                        {
                            return false;
                        }
                        PBEBattlePokemon switchPkmn = trainer.TryGetPokemon(action.SwitchPokemonId);
                        if (switchPkmn == null
                            || switchPkmn.HP == 0
                            || switchPkmn.FieldPosition != PBEFieldPosition.None // Also takes care of trying to switch into yourself
                            || standBy.Contains(switchPkmn)
                            )
                        {
                            return false;
                        }
                        standBy.Add(switchPkmn);
                        break;
                    }
                    default: return false;
                }
                verified.Add(pkmn);
            }
            return true;
        }
        public static bool SelectActionsIfValid(PBETrainer trainer, params PBETurnAction[] actions)
        {
            return SelectActionsIfValid(trainer, (IReadOnlyList<PBETurnAction>)actions);
        }
        /// <summary>Selects actions if they are valid. Changes the battle state if both teams have selected valid actions.</summary>
        /// <param name="trainer">The trainer the inputted actions belong to.</param>
        /// <param name="actions">The actions the team wishes to execute.</param>
        /// <returns>True if the actions are valid and were selected.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        public static bool SelectActionsIfValid(PBETrainer trainer, IReadOnlyList<PBETurnAction> actions)
        {
            if (AreActionsValid(trainer, actions))
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
                return true;
            }
            return false;
        }

        public static bool AreSwitchesValid(PBETrainer trainer, params PBESwitchIn[] switches)
        {
            return AreSwitchesValid(trainer, (IReadOnlyList<PBESwitchIn>)switches);
        }
        /// <summary>Determines whether chosen switches are valid.</summary>
        /// <param name="trainer">The trainer the inputted switches belong to.</param>
        /// <param name="switches">The switches the team wishes to execute.</param>
        /// <returns>False if the team already chose switches or the switches are illegal, True otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.WaitingForSwitchIns"/>.</exception>
        public static bool AreSwitchesValid(PBETrainer trainer, IReadOnlyList<PBESwitchIn> switches)
        {
            if (trainer == null)
            {
                throw new ArgumentNullException(nameof(trainer));
            }
            if (switches == null || switches.Any(s => s == null))
            {
                throw new ArgumentNullException(nameof(switches));
            }
            if (trainer.Battle.BattleState != PBEBattleState.WaitingForSwitchIns)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForSwitchIns} to validate switches.");
            }
            if (trainer.SwitchInsRequired == 0 || switches.Count != trainer.SwitchInsRequired)
            {
                return false;
            }
            var verified = new List<PBEBattlePokemon>(trainer.SwitchInsRequired);
            foreach (PBESwitchIn s in switches)
            {
                if (s.Position == PBEFieldPosition.None || s.Position >= PBEFieldPosition.MAX || !trainer.OwnsSpot(s.Position))
                {
                    return false;
                }
                PBEBattlePokemon pkmn = trainer.TryGetPokemon(s.PokemonId);
                if (pkmn == null || pkmn.HP == 0 || pkmn.FieldPosition != PBEFieldPosition.None || verified.Contains(pkmn))
                {
                    return false;
                }
                verified.Add(pkmn);
            }
            return true;
        }
        public static bool SelectSwitchesIfValid(PBETrainer trainer, params PBESwitchIn[] switches)
        {
            return SelectSwitchesIfValid(trainer, (IReadOnlyList<PBESwitchIn>)switches);
        }
        /// <summary>Selects switches if they are valid. Changes the battle state if both teams have selected valid switches.</summary>
        /// <param name="trainer">The trainer the inputted switches belong to.</param>
        /// <param name="switches">The switches the team wishes to execute.</param>
        /// <returns>True if the switches are valid and were selected.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.WaitingForSwitchIns"/>.</exception>
        public static bool SelectSwitchesIfValid(PBETrainer trainer, IReadOnlyList<PBESwitchIn> switches)
        {
            if (AreSwitchesValid(trainer, switches))
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
                return true;
            }
            return false;
        }
    }
}
