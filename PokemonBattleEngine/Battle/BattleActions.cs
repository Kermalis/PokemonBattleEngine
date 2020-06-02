using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
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
        public PBETurnTarget FightTargets { get; internal set; } // Internal set because of PBEMoveTarget.RandomFoeSurrounding
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
        public PBETurnAction(byte pokemonId, PBEMove fightMove, PBETurnTarget fightTargets)
        {
            PokemonId = pokemonId;
            Decision = PBETurnDecision.Fight;
            FightMove = fightMove;
            FightTargets = fightTargets;
        }
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
        /// <summary>Determines whether chosen actions are valid.</summary>
        /// <param name="team">The team the inputted actions belong to.</param>
        /// <param name="actions">The actions the team wishes to execute.</param>
        /// <returns>False if the team already chose actions or the actions are illegal, True otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        public static bool AreActionsValid(PBETeam team, IList<PBETurnAction> actions)
        {
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }
            if (actions == null || actions.Any(a => a == null))
            {
                throw new ArgumentNullException(nameof(actions));
            }
            if (team.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(team));
            }
            if (team.Battle.BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForActions} to validate actions.");
            }
            if (team.ActionsRequired.Count == 0 || actions.Count != team.ActionsRequired.Count)
            {
                return false;
            }
            var verified = new List<PBEPokemon>(team.ActionsRequired.Count);
            var standBy = new List<PBEPokemon>(team.ActionsRequired.Count);
            foreach (PBETurnAction action in actions)
            {
                PBEPokemon pkmn = team.TryGetPokemon(action.PokemonId);
                if (pkmn == null || !team.ActionsRequired.Contains(pkmn) || verified.Contains(pkmn))
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
                        PBEPokemon switchPkmn = team.TryGetPokemon(action.SwitchPokemonId);
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
        /// <summary>Selects actions if they are valid. Changes the battle state if both teams have selected valid actions.</summary>
        /// <param name="team">The team the inputted actions belong to.</param>
        /// <param name="actions">The actions the team wishes to execute.</param>
        /// <returns>True if the actions are valid and were selected.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        public static bool SelectActionsIfValid(PBETeam team, IList<PBETurnAction> actions)
        {
            if (AreActionsValid(team, actions))
            {
                lock (team.Battle._disposeLockObj)
                {
                    if (!team.Battle.IsDisposed)
                    {
                        team.ActionsRequired.Clear();
                        foreach (PBETurnAction action in actions)
                        {
                            PBEPokemon pkmn = team.TryGetPokemon(action.PokemonId);
                            if (action.Decision == PBETurnDecision.Fight && pkmn.GetMoveTargets(action.FightMove) == PBEMoveTarget.RandomFoeSurrounding)
                            {
                                switch (team.Battle.BattleFormat)
                                {
                                    case PBEBattleFormat.Single:
                                    case PBEBattleFormat.Rotation:
                                    {
                                        action.FightTargets = PBETurnTarget.FoeCenter;
                                        break;
                                    }
                                    case PBEBattleFormat.Double:
                                    {
                                        action.FightTargets = PBERandom.RandomBool() ? PBETurnTarget.FoeLeft : PBETurnTarget.FoeRight;
                                        break;
                                    }
                                    case PBEBattleFormat.Triple:
                                    {
                                        if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                        {
                                            action.FightTargets = PBERandom.RandomBool() ? PBETurnTarget.FoeCenter : PBETurnTarget.FoeRight;
                                        }
                                        else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                        {
                                            int r; // Keep randomly picking until a non-fainted foe is selected
                                        roll:
                                            r = PBERandom.RandomInt(0, 2);
                                            if (r == 0)
                                            {
                                                if (team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Left) != null)
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
                                                if (team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Center) != null)
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
                                                if (team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Right) != null)
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
                                            action.FightTargets = PBERandom.RandomBool() ? PBETurnTarget.FoeLeft : PBETurnTarget.FoeCenter;
                                        }
                                        break;
                                    }
                                    default: throw new ArgumentOutOfRangeException(nameof(team.Battle.BattleFormat));
                                }
                            }
                            pkmn.TurnAction = action;
                        }
                        if (team.Battle.Teams.All(t => t.ActionsRequired.Count == 0))
                        {
                            team.Battle.BattleState = PBEBattleState.ReadyToRunTurn;
                            team.Battle.OnStateChanged?.Invoke(team.Battle);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>Determines whether chosen switches are valid.</summary>
        /// <param name="team">The team the inputted switches belong to.</param>
        /// <param name="switches">The switches the team wishes to execute.</param>
        /// <returns>False if the team already chose switches or the switches are illegal, True otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.WaitingForSwitchIns"/>.</exception>
        public static bool AreSwitchesValid(PBETeam team, IList<PBESwitchIn> switches)
        {
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }
            if (switches == null || switches.Any(s => s == null))
            {
                throw new ArgumentNullException(nameof(switches));
            }
            if (team.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(team));
            }
            if (team.Battle.BattleState != PBEBattleState.WaitingForSwitchIns)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForSwitchIns} to validate switches.");
            }
            if (team.SwitchInsRequired == 0 || switches.Count != team.SwitchInsRequired)
            {
                return false;
            }
            var verified = new List<PBEPokemon>(team.SwitchInsRequired);
            foreach (PBESwitchIn s in switches)
            {
                if (s.Position == PBEFieldPosition.None || s.Position >= PBEFieldPosition.MAX)
                {
                    return false;
                }
                PBEPokemon pkmn = team.TryGetPokemon(s.PokemonId);
                if (pkmn == null || pkmn.HP == 0 || pkmn.FieldPosition != PBEFieldPosition.None || verified.Contains(pkmn))
                {
                    return false;
                }
                verified.Add(pkmn);
            }
            return true;
        }
        /// <summary>Selects switches if they are valid. Changes the battle state if both teams have selected valid switches.</summary>
        /// <param name="team">The team the inputted switches belong to.</param>
        /// <param name="switches">The switches the team wishes to execute.</param>
        /// <returns>True if the switches are valid and were selected.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.WaitingForSwitchIns"/>.</exception>
        public static bool SelectSwitchesIfValid(PBETeam team, IList<PBESwitchIn> switches)
        {
            if (AreSwitchesValid(team, switches))
            {
                lock (team.Battle._disposeLockObj)
                {
                    if (!team.Battle.IsDisposed)
                    {
                        team.SwitchInsRequired = 0;
                        foreach (PBESwitchIn s in switches)
                        {
                            PBEPokemon pkmn = team.TryGetPokemon(s.PokemonId);
                            team.SwitchInQueue.Add((pkmn, s.Position));
                        }
                        if (team.Battle.Teams.All(t => t.SwitchInsRequired == 0))
                        {
                            team.Battle.SwitchesOrActions();
                        }
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
