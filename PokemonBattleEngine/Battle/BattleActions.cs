using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Kermalis.PokemonBattleEngine.Battle
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PBEAction
    {
        [FieldOffset(0)]
        public byte PokemonId;
        [FieldOffset(1)]
        public PBEDecision Decision;
        [FieldOffset(2)]
        public PBEMove FightMove;
        [FieldOffset(4)]
        public PBETarget FightTargets;
        [FieldOffset(4)]
        public byte SwitchPokemonId;

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.Add(PokemonId);
            bytes.Add((byte)Decision);
            bytes.AddRange(BitConverter.GetBytes((ushort)FightMove));
            bytes.Add(SwitchPokemonId);
            return bytes.ToArray();
        }
        internal static PBEAction FromBytes(BinaryReader r)
        {
            return new PBEAction
            {
                PokemonId = r.ReadByte(),
                Decision = (PBEDecision)r.ReadByte(),
                FightMove = (PBEMove)r.ReadUInt16(),
                SwitchPokemonId = r.ReadByte()
            };
        }
    }
    public sealed partial class PBEBattle
    {
        /// <summary>
        /// Determines whether chosen actions are valid.
        /// </summary>
        /// <param name="team">The team the inputted actions belong to.</param>
        /// <param name="actions">The actions the team wishes to execute.</param>
        /// <returns>False if the team already chose actions or the actions are illegal, True otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        public static bool AreActionsValid(PBETeam team, IEnumerable<PBEAction> actions)
        {
            if (team.Battle.BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForActions} to validate actions.");
            }
            if (team.ActionsRequired.Count == 0 || actions.Count() != team.ActionsRequired.Count)
            {
                return false;
            }
            var standBy = new List<PBEPokemon>();
            foreach (PBEAction action in actions)
            {
                PBEPokemon pkmn = team.Battle.TryGetPokemon(action.PokemonId);
                if (!team.ActionsRequired.Contains(pkmn))
                {
                    return false;
                }
                switch (action.Decision)
                {
                    case PBEDecision.Fight:
                        {
                            if (action.FightMove == PBEMove.Struggle && pkmn.IsForcedToStruggle())
                            {
                                continue;
                            }
                            else
                            {
                                if (action.FightMove == PBEMove.None
                                    || !pkmn.Moves.Contains(action.FightMove)
                                    || pkmn.PP[Array.IndexOf(pkmn.Moves, action.FightMove)] == 0
                                    || (pkmn.ChoiceLockedMove != PBEMove.None && pkmn.ChoiceLockedMove != action.FightMove)
                                    || (pkmn.TempLockedMove != PBEMove.None && pkmn.TempLockedMove != action.FightMove)
                                    || (pkmn.TempLockedTargets != PBETarget.None && pkmn.TempLockedTargets != action.FightTargets)
                                    || !AreTargetsValid(pkmn, action.FightMove, action.FightTargets)
                                    )
                                {
                                    return false;
                                }
                            }
                            break;
                        }
                    case PBEDecision.SwitchOut:
                        {
                            if (pkmn.Status2.HasFlag(PBEStatus2.Airborne)
                                || pkmn.Status2.HasFlag(PBEStatus2.Underground)
                                || pkmn.Status2.HasFlag(PBEStatus2.Underwater)
                                )
                            {
                                return false;
                            }
                            PBEPokemon switchPkmn = team.Battle.TryGetPokemon(action.SwitchPokemonId);
                            if (switchPkmn == null
                                || switchPkmn.Team != team
                                || switchPkmn.Id == pkmn.Id
                                || switchPkmn.HP == 0
                                || switchPkmn.FieldPosition != PBEFieldPosition.None
                                || standBy.Contains(switchPkmn)
                                )
                            {
                                return false;
                            }
                            else
                            {
                                standBy.Add(switchPkmn);
                            }
                            break;
                        }
                }
            }
            return true;
        }
        /// <summary>
        /// Selects actions if they are valid. Changes the battle state if both teams have selected valid actions.
        /// </summary>
        /// <param name="team">The team the inputted actions belong to.</param>
        /// <param name="actions">The actions the team wishes to execute.</param>
        /// <returns>True if the actions are valid and were selected.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        public static bool SelectActionsIfValid(PBETeam team, IEnumerable<PBEAction> actions)
        {
            if (team.Battle.BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForActions} to select actions.");
            }
            if (AreActionsValid(team, actions))
            {
                team.ActionsRequired.Clear();
                foreach (PBEAction action in actions)
                {
                    PBEPokemon pkmn = team.Battle.TryGetPokemon(action.PokemonId);
                    pkmn.SelectedAction = action;
                    switch (pkmn.SelectedAction.Decision)
                    {
                        case PBEDecision.Fight:
                            {
                                if (pkmn.Item == PBEItem.ChoiceBand || pkmn.Item == PBEItem.ChoiceScarf || pkmn.Item == PBEItem.ChoiceSpecs)
                                {
                                    pkmn.ChoiceLockedMove = pkmn.SelectedAction.FightMove;
                                }
                                switch (pkmn.GetMoveTargets(pkmn.SelectedAction.FightMove))
                                {
                                    case PBEMoveTarget.RandomFoeSurrounding:
                                        {
                                            switch (team.Battle.BattleFormat)
                                            {
                                                case PBEBattleFormat.Single:
                                                case PBEBattleFormat.Rotation:
                                                    {
                                                        pkmn.SelectedAction.FightTargets = PBETarget.FoeCenter;
                                                        break;
                                                    }
                                                case PBEBattleFormat.Double:
                                                    {
                                                        pkmn.SelectedAction.FightTargets = PBEUtils.RNG.NextBoolean() ? PBETarget.FoeLeft : PBETarget.FoeRight;
                                                        break;
                                                    }
                                                case PBEBattleFormat.Triple:
                                                    {
                                                        if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                                        {
                                                            pkmn.SelectedAction.FightTargets = PBEUtils.RNG.NextBoolean() ? PBETarget.FoeCenter : PBETarget.FoeRight;
                                                        }
                                                        else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                                        {
                                                            PBETeam opposingTeam = team == team.Battle.Teams[0] ? team.Battle.Teams[1] : team.Battle.Teams[0];
                                                            int r; // Keep randomly picking until a non-fainted foe is selected
                                                        roll:
                                                            r = PBEUtils.RNG.Next(3);
                                                            if (r == 0)
                                                            {
                                                                if (opposingTeam.TryGetPokemon(PBEFieldPosition.Left) != null)
                                                                {
                                                                    pkmn.SelectedAction.FightTargets = PBETarget.FoeLeft;
                                                                }
                                                                else
                                                                {
                                                                    goto roll;
                                                                }
                                                            }
                                                            else if (r == 1)
                                                            {
                                                                if (opposingTeam.TryGetPokemon(PBEFieldPosition.Center) != null)
                                                                {
                                                                    pkmn.SelectedAction.FightTargets = PBETarget.FoeCenter;
                                                                }
                                                                else
                                                                {
                                                                    goto roll;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (opposingTeam.TryGetPokemon(PBEFieldPosition.Right) != null)
                                                                {
                                                                    pkmn.SelectedAction.FightTargets = PBETarget.FoeRight;
                                                                }
                                                                else
                                                                {
                                                                    goto roll;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            pkmn.SelectedAction.FightTargets = PBEUtils.RNG.NextBoolean() ? PBETarget.FoeLeft : PBETarget.FoeCenter;
                                                        }
                                                        break;
                                                    }
                                            }
                                            break;
                                        }
                                    case PBEMoveTarget.SingleAllySurrounding:
                                        {
                                            if (team.Battle.BattleFormat == PBEBattleFormat.Single || team.Battle.BattleFormat == PBEBattleFormat.Rotation)
                                            {
                                                pkmn.SelectedAction.FightTargets = PBETarget.AllyCenter;
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                }
                if (team.Battle.Teams.All(t => t.ActionsRequired.Count == 0))
                {
                    team.Battle.BattleState = PBEBattleState.ReadyToRunTurn;
                    team.Battle.OnStateChanged?.Invoke(team.Battle);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether chosen switches are valid.
        /// </summary>
        /// <param name="team">The team the inputted switches belong to.</param>
        /// <param name="switches">The switches the team wishes to execute.</param>
        /// <returns>False if the team already chose switches or the switches are illegal, True otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.WaitingForSwitchIns"/>.</exception>
        public static bool AreSwitchesValid(PBETeam team, IEnumerable<Tuple<byte, PBEFieldPosition>> switches)
        {
            if (team.Battle.BattleState != PBEBattleState.WaitingForSwitchIns)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForSwitchIns} to validate switches.");
            }
            if (team.SwitchInsRequired == 0 || switches.Count() != team.SwitchInsRequired)
            {
                return false;
            }
            foreach (Tuple<byte, PBEFieldPosition> s in switches)
            {
                PBEPokemon pkmn = team.Battle.TryGetPokemon(s.Item1);
                if (pkmn == null || pkmn.Team != team || pkmn.HP == 0 || pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Selects switches if they are valid. Changes the battle state if both teams have selected valid switches.
        /// </summary>
        /// <param name="team">The team the inputted switches belong to.</param>
        /// <param name="switches">The switches the team wishes to execute.</param>
        /// <returns>True if the switches are valid and were selected.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.WaitingForSwitchIns"/>.</exception>
        public static bool SelectSwitchesIfValid(PBETeam team, IEnumerable<Tuple<byte, PBEFieldPosition>> switches)
        {
            if (team.Battle.BattleState != PBEBattleState.WaitingForSwitchIns)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForSwitchIns} to select switches.");
            }
            if (AreSwitchesValid(team, switches))
            {
                team.SwitchInsRequired = 0;
                foreach (Tuple<byte, PBEFieldPosition> s in switches)
                {
                    PBEPokemon pkmn = team.Battle.TryGetPokemon(s.Item1);
                    pkmn.FieldPosition = s.Item2;
                    team.SwitchInQueue.Add(pkmn);
                }
                if (team.Battle.Teams.All(t => t.SwitchInsRequired == 0))
                {
                    team.Battle.SwitchInQueuedPokemon();
                    team.Battle.RequestActions();
                }
                return true;
            }
            return false;
        }
    }
}
