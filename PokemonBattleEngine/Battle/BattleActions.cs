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
        /// <param name="localTeam">Which team the inputted actions belong to.</param>
        /// <param name="actions">The actions the team wishes to execute.</param>
        /// <returns>False if the team already chose actions or the actions are illegal, True otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the battle state is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        public bool AreActionsValid(bool localTeam, IEnumerable<PBEAction> actions)
        {
            if (BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {nameof(PBEBattleState.WaitingForActions)} to validate actions.");
            }
            PBETeam team = Teams[localTeam ? 0 : 1];
            if (actions.Count() == 0 || actions.Count() != team.ActionsRequired.Count)
            {
                return false;
            }
            var standBy = new List<PBEPokemon>();
            foreach (PBEAction action in actions)
            {
                PBEPokemon pkmn = GetPokemon(action.PokemonId);
                // Validate Pokémon
                if (!team.ActionsRequired.Contains(pkmn))
                {
                    return false;
                }
                switch (action.Decision)
                {
                    case PBEDecision.Fight:
                        // Invalid move
                        if (!pkmn.Moves.Contains(action.FightMove) || action.FightMove == PBEMove.None)
                        {
                            return false;
                        }
                        // TODO: Struggle
                        // Out of PP
                        if (pkmn.PP[Array.IndexOf(pkmn.Moves, action.FightMove)] < 1)
                        {
                            return false;
                        }
                        // If the mon has a locked move, it must be used
                        if (pkmn.LockedAction.Decision == PBEDecision.Fight)
                        {
                            if ((pkmn.LockedAction.FightMove != PBEMove.None && pkmn.LockedAction.FightMove != action.FightMove)
                                || (pkmn.LockedAction.FightTargets != PBETarget.None && pkmn.LockedAction.FightTargets != action.FightTargets))
                            {
                                return false;
                            }
                        }
                        // Verify targets
                        if (!AreTargetsValid(pkmn, action.FightMove, action.FightTargets))
                        {
                            return false;
                        }
                        break;
                    case PBEDecision.SwitchOut:
                        // Cannot switch while airborne, underground or underwater
                        if (pkmn.Status2.HasFlag(PBEStatus2.Airborne) || pkmn.Status2.HasFlag(PBEStatus2.Underground) || pkmn.Status2.HasFlag(PBEStatus2.Underwater))
                        {
                            return false;
                        }
                        PBEPokemon switchPkmn = GetPokemon(action.SwitchPokemonId);
                        // Validate the new battler's ID
                        if (switchPkmn == null || switchPkmn.LocalTeam != localTeam || switchPkmn.Id == pkmn.Id)
                        {
                            return false;
                        }
                        // Cannot switch into a fainted Pokémon
                        if (switchPkmn.HP < 1)
                        {
                            return false;
                        }
                        // Cannot switch into a Pokémon already on the field
                        if (switchPkmn.FieldPosition != PBEFieldPosition.None)
                        {
                            return false;
                        }
                        // Cannot switch into a Pokémon already selected to switch in this turn
                        if (standBy.Contains(switchPkmn))
                        {
                            return false;
                        }
                        standBy.Add(switchPkmn);
                        break;
                }
            }
            return true;
        }
        /// <summary>
        /// Selects actions if they are valid. Changes the battle state if both teams have selected valid actions.
        /// </summary>
        /// <param name="localTeam">Which team the inputted actions belong to.</param>
        /// <param name="actions">The actions the team wishes to execute.</param>
        /// <returns>True if the actions are valid and were selected.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the battle state is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        public bool SelectActionsIfValid(bool localTeam, IEnumerable<PBEAction> actions)
        {
            if (BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {nameof(PBEBattleState.WaitingForActions)} to select actions.");
            }
            if (AreActionsValid(localTeam, actions))
            {
                Teams[localTeam ? 0 : 1].ActionsRequired.Clear();
                foreach (PBEAction action in actions)
                {
                    PBEPokemon pkmn = GetPokemon(action.PokemonId);
                    pkmn.SelectedAction = action;
                    switch (pkmn.SelectedAction.Decision)
                    {
                        case PBEDecision.Fight:
                            switch (GetMoveTargetsForPokemon(pkmn, pkmn.SelectedAction.FightMove))
                            {
                                case PBEMoveTarget.RandomFoeSurrounding:
                                    switch (BattleFormat)
                                    {
                                        case PBEBattleFormat.Single:
                                        case PBEBattleFormat.Rotation:
                                            pkmn.SelectedAction.FightTargets = PBETarget.FoeCenter;
                                            break;
                                        case PBEBattleFormat.Double:
                                            pkmn.SelectedAction.FightTargets = PBEUtils.RNG.Next(2) == 0 ? PBETarget.FoeLeft : PBETarget.FoeRight;
                                            break;
                                        case PBEBattleFormat.Triple:
                                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                            {
                                                pkmn.SelectedAction.FightTargets = PBEUtils.RNG.Next(2) == 0 ? PBETarget.FoeCenter : PBETarget.FoeRight;
                                            }
                                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                            {
                                                PBETeam opposingTeam = Teams[pkmn.LocalTeam ? 1 : 0]; // Other team                                                                                                  
                                                int r; // Keep randomly picking until a non-fainted foe is selected
                                            roll:
                                                r = PBEUtils.RNG.Next(3);
                                                if (r == 0)
                                                {
                                                    if (opposingTeam.PokemonAtPosition(PBEFieldPosition.Left) != null)
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
                                                    if (opposingTeam.PokemonAtPosition(PBEFieldPosition.Center) != null)
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
                                                    if (opposingTeam.PokemonAtPosition(PBEFieldPosition.Right) != null)
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
                                                pkmn.SelectedAction.FightTargets = PBEUtils.RNG.Next(2) == 0 ? PBETarget.FoeLeft : PBETarget.FoeCenter;
                                            }
                                            break;
                                    }
                                    break;
                                case PBEMoveTarget.SingleAllySurrounding:
                                    if (BattleFormat == PBEBattleFormat.Single || BattleFormat == PBEBattleFormat.Rotation)
                                    {
                                        pkmn.SelectedAction.FightTargets = PBETarget.AllyCenter;
                                    }
                                    break;
                            }
                            break;
                    }
                }
                if (Teams[0].ActionsRequired.Count == 0 && Teams[1].ActionsRequired.Count == 0)
                {
                    BattleState = PBEBattleState.ReadyToRunTurn;
                    OnStateChanged?.Invoke(this);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether chosen switches are valid.
        /// </summary>
        /// <param name="localTeam">Which team the inputted switches belong to.</param>
        /// <param name="switches">The switches the team wishes to execute.</param>
        /// <returns>False if the team already chose switches or the switches are illegal, True otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the battle state is not <see cref="PBEBattleState.WaitingForSwitchIns"/>.</exception>
        public bool AreSwitchesValid(bool localTeam, IEnumerable<Tuple<byte, PBEFieldPosition>> switches)
        {
            if (BattleState != PBEBattleState.WaitingForSwitchIns)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {nameof(PBEBattleState.WaitingForSwitchIns)} to validate switches.");
            }
            if (switches.Count() == 0 || switches.Count() != Teams[localTeam ? 0 : 1].SwitchInsRequired)
            {
                return false;
            }
            foreach (Tuple<byte, PBEFieldPosition> s in switches)
            {
                PBEPokemon pkmn = GetPokemon(s.Item1);
                if (pkmn == null || pkmn.LocalTeam != localTeam || pkmn.HP < 1 || pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Selects switches if they are valid. Changes the battle state if both teams have selected valid switches.
        /// </summary>
        /// <param name="localTeam">Which team the inputted switches belong to.</param>
        /// <param name="switches">The switches the team wishes to execute.</param>
        /// <returns>True if the switches are valid and were selected.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the battle state is not <see cref="PBEBattleState.WaitingForSwitchIns"/>.</exception>
        public bool SelectSwitchesIfValid(bool localTeam, IEnumerable<Tuple<byte, PBEFieldPosition>> switches)
        {
            if (BattleState != PBEBattleState.WaitingForSwitchIns)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {nameof(PBEBattleState.WaitingForSwitchIns)} to select switches.");
            }
            if (AreSwitchesValid(localTeam, switches))
            {
                PBETeam team = Teams[localTeam ? 0 : 1];
                team.SwitchInsRequired = 0;
                foreach (Tuple<byte, PBEFieldPosition> s in switches)
                {
                    PBEPokemon pkmn = GetPokemon(s.Item1);
                    pkmn.FieldPosition = s.Item2;
                    team.SwitchInQueue.Add(pkmn);
                }
                if (Teams[0].SwitchInsRequired == 0 && Teams[1].SwitchInsRequired == 0)
                {
                    SwitchInQueuedPokemon();
                    RequestActions();
                }
                return true;
            }
            return false;
        }
    }
}
