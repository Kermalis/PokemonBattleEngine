using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.AI
{
    /// <summary>Creates valid decisions for a team in a battle. Decisions may not be valid for custom settings and/or move changes.</summary>
    public static partial class PBEAI
    {
        /// <summary>Creates valid actions for a battle turn for a specific team.</summary>
        /// <param name="team">The team to create actions for.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="team"/> has no active battlers or <paramref name="team"/>'s <see cref="PBETeam.Battle"/>'s <see cref="PBEBattle.BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a Pokémon has no moves, the AI tries to use a move with invalid targets, or <paramref name="team"/>'s <see cref="PBETeam.Battle"/>'s <see cref="PBEBattle.BattleFormat"/> is invalid.</exception>
        public static PBETurnAction[] CreateActions(PBETeam team)
        {
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }
            if (team.Battle.BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(team.Battle.BattleState)} must be {PBEBattleState.WaitingForActions} to create actions.");
            }

            var actions = new PBETurnAction[team.ActionsRequired.Count];
            var standBy = new List<PBEPokemon>();
            for (int i = 0; i < actions.Length; i++)
            {
                PBEPokemon pkmn = team.ActionsRequired[i];

                // If a Pokémon is forced to struggle, it is best that it just stays in until it faints
                if (pkmn.IsForcedToStruggle())
                {
                    actions[i] = new PBETurnAction(pkmn.Id, PBEMove.Struggle, GetPossibleTargets(pkmn, pkmn.GetMoveTargets(PBEMove.Struggle))[0]);
                }
                // If a Pokémon has a temp locked move (Dig, Dive, Shadow Force) it must be used
                else if (pkmn.TempLockedMove != PBEMove.None)
                {
                    actions[i] = new PBETurnAction(pkmn.Id, pkmn.TempLockedMove, pkmn.TempLockedTargets);
                }
                // The Pokémon is free to switch or fight (unless it cannot switch due to Magnet Pull etc)
                else
                {
                    // Gather all options of switching and moves
                    PBEPokemon[] availableForSwitch = team.Party.Except(standBy).Where(p => p.FieldPosition == PBEFieldPosition.None && p.HP > 0).ToArray();
                    PBEMove[] usableMoves = pkmn.GetUsableMoves();

                    var possibleActions = new List<(PBETurnAction Action, double Score)>();
                    for (int m = 0; m < usableMoves.Length; m++) // Score moves
                    {
                        PBEMove move = usableMoves[m];
                        PBEType moveType = pkmn.GetMoveType(move);
                        PBEMoveTarget moveTargets = pkmn.GetMoveTargets(move);
                        PBETurnTarget[] possibleTargets = PBEMoveData.IsSpreadMove(moveTargets)
                            ? new PBETurnTarget[] { GetSpreadMoveTargets(pkmn, moveTargets) }
                            : GetPossibleTargets(pkmn, moveTargets);
                        foreach (PBETurnTarget possibleTarget in possibleTargets)
                        {
                            // TODO: RandomFoeSurrounding (probably just account for the specific effects that use this target type)
                            var targets = new List<PBEPokemon>();
                            if (possibleTarget.HasFlag(PBETurnTarget.AllyLeft))
                            {
                                targets.Add(team.TryGetPokemon(PBEFieldPosition.Left));
                            }
                            if (possibleTarget.HasFlag(PBETurnTarget.AllyCenter))
                            {
                                targets.Add(team.TryGetPokemon(PBEFieldPosition.Center));
                            }
                            if (possibleTarget.HasFlag(PBETurnTarget.AllyRight))
                            {
                                targets.Add(team.TryGetPokemon(PBEFieldPosition.Right));
                            }
                            if (possibleTarget.HasFlag(PBETurnTarget.FoeLeft))
                            {
                                targets.Add(team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Left));
                            }
                            if (possibleTarget.HasFlag(PBETurnTarget.FoeCenter))
                            {
                                targets.Add(team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Center));
                            }
                            if (possibleTarget.HasFlag(PBETurnTarget.FoeRight))
                            {
                                targets.Add(team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Right));
                            }

                            double score = 0.0;
                            switch (PBEMoveData.Data[move].Effect)
                            {
                                case PBEMoveEffect.Burn:
                                case PBEMoveEffect.Paralyze:
                                case PBEMoveEffect.Poison:
                                case PBEMoveEffect.Sleep:
                                case PBEMoveEffect.Toxic:
                                {
                                    foreach (PBEPokemon target in targets)
                                    {
                                        if (target == null)
                                        {
                                            // TODO: If all targets are null, this should give a bad score
                                        }
                                        else
                                        {
                                            // TODO: Effectiveness check
                                            // TODO: Favor sleep with Bad Dreams (unless ally)
                                            if (target.Status1 != PBEStatus1.None)
                                            {
                                                score += target.Team == team.OpposingTeam ? -60 : 0;
                                            }
                                            else
                                            {
                                                score += target.Team == team.OpposingTeam ? +40 : -20;
                                            }
                                        }
                                    }
                                    break;
                                }
                                case PBEMoveEffect.Hail:
                                {
                                    if (team.Battle.Weather == PBEWeather.Hailstorm)
                                    {
                                        score -= 100;
                                    }
                                    break;
                                }
                                case PBEMoveEffect.BrickBreak:
                                case PBEMoveEffect.Dig:
                                case PBEMoveEffect.Dive:
                                case PBEMoveEffect.Fly:
                                case PBEMoveEffect.Hit:
                                case PBEMoveEffect.Hit__MaybeBurn:
                                case PBEMoveEffect.Hit__MaybeConfuse:
                                case PBEMoveEffect.Hit__MaybeFlinch:
                                case PBEMoveEffect.Hit__MaybeFreeze:
                                case PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1:
                                case PBEMoveEffect.Hit__MaybeLowerTarget_ATK_By1:
                                case PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1:
                                case PBEMoveEffect.Hit__MaybeLowerTarget_SPATK_By1:
                                case PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1:
                                case PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2:
                                case PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1:
                                case PBEMoveEffect.Hit__MaybeLowerUser_ATK_DEF_By1:
                                case PBEMoveEffect.Hit__MaybeLowerUser_DEF_SPDEF_By1:
                                case PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2:
                                case PBEMoveEffect.Hit__MaybeLowerUser_SPE_By1:
                                case PBEMoveEffect.Hit__MaybeLowerUser_SPE_DEF_SPDEF_By1:
                                case PBEMoveEffect.Hit__MaybeParalyze:
                                case PBEMoveEffect.Hit__MaybePoison:
                                case PBEMoveEffect.Hit__MaybeRaiseUser_ATK_By1:
                                case PBEMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1:
                                case PBEMoveEffect.Hit__MaybeRaiseUser_DEF_By1:
                                case PBEMoveEffect.Hit__MaybeRaiseUser_SPATK_By1:
                                case PBEMoveEffect.Hit__MaybeRaiseUser_SPE_By1:
                                case PBEMoveEffect.Hit__MaybeToxic:
                                case PBEMoveEffect.HPDrain:
                                case PBEMoveEffect.Recoil:
                                case PBEMoveEffect.FlareBlitz:
                                case PBEMoveEffect.SuckerPunch:
                                case PBEMoveEffect.VoltTackle:
                                {
                                    foreach (PBEPokemon target in targets)
                                    {
                                        if (target == null)
                                        {
                                            // TODO: If all targets are null, this should give a bad score
                                        }
                                        else
                                        {
                                            // TODO: Put type checking somewhere in PBEPokemon (levitate, wonder guard, etc)
                                            // TODO: Favor hitting ally with move if it absorbs it
                                            // TODO: Check items
                                            // TODO: Stat changes and accuracy
                                            // TODO: Check base power specifically against hp remaining (include spread move damage reduction)
                                            double typeEffectiveness = PBEPokemonData.TypeEffectiveness[moveType][target.KnownType1];
                                            typeEffectiveness *= PBEPokemonData.TypeEffectiveness[moveType][target.KnownType2];
                                            if (typeEffectiveness <= 0.0) // (-infinity, 0.0] Ineffective
                                            {
                                                score += target.Team == team.OpposingTeam ? -60 : -1;
                                            }
                                            else if (typeEffectiveness <= 0.25) // (0.0, 0.25] NotVeryEffective
                                            {
                                                score += target.Team == team.OpposingTeam ? -30 : -5;
                                            }
                                            else if (typeEffectiveness < 1.0) // (0.25, 1.0) NotVeryEffective
                                            {
                                                score += target.Team == team.OpposingTeam ? -10 : -10;
                                            }
                                            else if (typeEffectiveness == 1.0) // [1.0, 1.0] Normal
                                            {
                                                score += target.Team == team.OpposingTeam ? +10 : -15;
                                            }
                                            else if (typeEffectiveness < 4.0) // (1.0, 4.0) SuperEffective
                                            {
                                                score += target.Team == team.OpposingTeam ? +25 : -20;
                                            }
                                            else // [4.0, infinity) SuperEffective
                                            {
                                                score += target.Team == team.OpposingTeam ? +40 : -30;
                                            }
                                            if (pkmn.HasType(moveType) && typeEffectiveness > 0.0) // STAB
                                            {
                                                score += (pkmn.Ability == PBEAbility.Adaptability ? 15 : 10) * (target.Team == team.OpposingTeam ? +1 : -1);
                                            }
                                        }
                                    }

                                    break;
                                }
                                case PBEMoveEffect.Moonlight:
                                case PBEMoveEffect.Rest:
                                case PBEMoveEffect.RestoreTargetHP:
                                case PBEMoveEffect.RestoreUserHP:
                                {
                                    PBEPokemon target = targets[0];
                                    if (target == null || target.Team == team.OpposingTeam)
                                    {
                                        score -= 100;
                                    }
                                    else // Ally
                                    {
                                        // 0% = +45, 25% = +30, 50% = +15, 75% = 0, 100% = -15
                                        score -= (60 * target.HPPercentage) - 45;
                                    }
                                    break;
                                }
                                case PBEMoveEffect.RainDance:
                                {
                                    if (team.Battle.Weather == PBEWeather.Rain)
                                    {
                                        score -= 100;
                                    }
                                    break;
                                }
                                case PBEMoveEffect.Sandstorm:
                                {
                                    if (team.Battle.Weather == PBEWeather.Sandstorm)
                                    {
                                        score -= 100;
                                    }
                                    break;
                                }
                                case PBEMoveEffect.SunnyDay:
                                {
                                    if (team.Battle.Weather == PBEWeather.HarshSunlight)
                                    {
                                        score -= 100;
                                    }
                                    break;
                                }
                                case PBEMoveEffect.Attract:
                                case PBEMoveEffect.ChangeTarget_ACC:
                                case PBEMoveEffect.ChangeTarget_ATK:
                                case PBEMoveEffect.ChangeTarget_DEF:
                                case PBEMoveEffect.ChangeTarget_EVA:
                                case PBEMoveEffect.ChangeTarget_SPDEF:
                                case PBEMoveEffect.ChangeTarget_SPE:
                                case PBEMoveEffect.ChangeUser_ATK:
                                case PBEMoveEffect.ChangeUser_DEF:
                                case PBEMoveEffect.ChangeUser_EVA:
                                case PBEMoveEffect.ChangeUser_SPATK:
                                case PBEMoveEffect.ChangeUser_SPDEF:
                                case PBEMoveEffect.ChangeUser_SPE:
                                case PBEMoveEffect.Confuse:
                                case PBEMoveEffect.Curse:
                                case PBEMoveEffect.Endeavor:
                                case PBEMoveEffect.Fail:
                                case PBEMoveEffect.FinalGambit:
                                case PBEMoveEffect.Flatter:
                                case PBEMoveEffect.FocusEnergy:
                                case PBEMoveEffect.GastroAcid:
                                case PBEMoveEffect.Growth:
                                case PBEMoveEffect.HelpingHand:
                                case PBEMoveEffect.LeechSeed:
                                case PBEMoveEffect.LightScreen:
                                case PBEMoveEffect.LowerTarget_ATK_DEF_By1:
                                case PBEMoveEffect.LowerUser_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2:
                                case PBEMoveEffect.LuckyChant:
                                case PBEMoveEffect.Metronome:
                                case PBEMoveEffect.OneHitKnockout:
                                case PBEMoveEffect.PainSplit:
                                case PBEMoveEffect.Protect:
                                case PBEMoveEffect.PsychUp:
                                case PBEMoveEffect.Psywave:
                                case PBEMoveEffect.RaiseUser_ATK_ACC_By1:
                                case PBEMoveEffect.RaiseUser_ATK_DEF_By1:
                                case PBEMoveEffect.RaiseUser_ATK_DEF_ACC_By1:
                                case PBEMoveEffect.RaiseUser_ATK_SPATK_By1:
                                case PBEMoveEffect.RaiseUser_ATK_SPE_By1:
                                case PBEMoveEffect.RaiseUser_DEF_SPDEF_By1:
                                case PBEMoveEffect.RaiseUser_SPATK_SPDEF_By1:
                                case PBEMoveEffect.RaiseUser_SPATK_SPDEF_SPE_By1:
                                case PBEMoveEffect.RaiseUser_SPE_By2_ATK_By1:
                                case PBEMoveEffect.Reflect:
                                case PBEMoveEffect.SeismicToss:
                                case PBEMoveEffect.Selfdestruct:
                                case PBEMoveEffect.SetDamage:
                                case PBEMoveEffect.Snore:
                                case PBEMoveEffect.Spikes:
                                case PBEMoveEffect.StealthRock:
                                case PBEMoveEffect.Substitute:
                                case PBEMoveEffect.SuperFang:
                                case PBEMoveEffect.Swagger:
                                case PBEMoveEffect.ToxicSpikes:
                                case PBEMoveEffect.Transform:
                                case PBEMoveEffect.TrickRoom:
                                case PBEMoveEffect.Whirlwind:
                                case PBEMoveEffect.WideGuard:
                                {
                                    // TODO Moves
                                    break;
                                }
                                default: throw new ArgumentOutOfRangeException(nameof(PBEMoveData.Effect));
                            }
                            possibleActions.Add((new PBETurnAction(pkmn.Id, move, possibleTarget), score));
                        }
                    }
                    if (pkmn.CanSwitchOut())
                    {
                        for (int s = 0; s < availableForSwitch.Length; s++) // Score switches
                        {
                            PBEPokemon switchPkmn = availableForSwitch[s];
                            // TODO: Entry hazards
                            // TODO: Known moves of active battlers
                            // TODO: Type effectiveness
                            double score = 0.0;
                            possibleActions.Add((new PBETurnAction(pkmn.Id, switchPkmn.Id), score));
                        }
                    }

                    string ToDebugString((PBETurnAction Action, double Score) t)
                    {
                        string str = "{";
                        if (t.Action.Decision == PBETurnDecision.Fight)
                        {
                            str += string.Format("Fight {0} {1}", t.Action.FightMove, t.Action.FightTargets);
                        }
                        else
                        {
                            str += string.Format("Switch {0}", team.TryGetPokemon(t.Action.SwitchPokemonId).Nickname);
                        }
                        str += " [" + t.Score + "]}";
                        return str;
                    }
                    IOrderedEnumerable<(PBETurnAction Action, double Score)> byScore = possibleActions.OrderByDescending(t => t.Score);
                    Debug.WriteLine("{0}'s possible actions: {1}", pkmn.Nickname, byScore.Select(t => ToDebugString(t)).Print());
                    double bestScore = byScore.First().Score;
                    actions[i] = byScore.Where(t => t.Score == bestScore).ToArray().RandomElement().Action; // Pick random action of the ones that tied for best score
                }

                // Action was chosen, finish up for this Pokémon
                if (actions[i].Decision == PBETurnDecision.SwitchOut)
                {
                    standBy.Add(team.TryGetPokemon(actions[i].SwitchPokemonId));
                }
            }
            return actions;
        }

        /// <summary>Creates valid switches for a battle for a specific team.</summary>
        /// <param name="team">The team to create switches for.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="team"/> does not require switch-ins or <paramref name="team"/>'s <see cref="PBETeam.Battle"/>'s <see cref="PBEBattle.BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="team"/>'s <see cref="PBETeam.Battle"/>'s <see cref="PBEBattle.BattleFormat"/> is invalid.</exception>
        public static PBESwitchIn[] CreateSwitches(PBETeam team)
        {
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }
            if (team.Battle.BattleState != PBEBattleState.WaitingForSwitchIns)
            {
                throw new InvalidOperationException($"{nameof(team.Battle.BattleState)} must be {PBEBattleState.WaitingForSwitchIns} to create switch-ins.");
            }
            if (team.SwitchInsRequired == 0)
            {
                throw new InvalidOperationException($"{nameof(team)} must require switch-ins.");
            }
            PBEPokemon[] available = team.Party.Where(p => p.FieldPosition == PBEFieldPosition.None && p.HP > 0).ToArray();
            available.Shuffle();
            var availablePositions = new List<PBEFieldPosition>();
            switch (team.Battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                {
                    availablePositions.Add(PBEFieldPosition.Center);
                    break;
                }
                case PBEBattleFormat.Double:
                {
                    if (team.TryGetPokemon(PBEFieldPosition.Left) == null)
                    {
                        availablePositions.Add(PBEFieldPosition.Left);
                    }
                    if (team.TryGetPokemon(PBEFieldPosition.Right) == null)
                    {
                        availablePositions.Add(PBEFieldPosition.Right);
                    }
                    break;
                }
                case PBEBattleFormat.Triple:
                case PBEBattleFormat.Rotation:
                {
                    if (team.TryGetPokemon(PBEFieldPosition.Left) == null)
                    {
                        availablePositions.Add(PBEFieldPosition.Left);
                    }
                    if (team.TryGetPokemon(PBEFieldPosition.Center) == null)
                    {
                        availablePositions.Add(PBEFieldPosition.Center);
                    }
                    if (team.TryGetPokemon(PBEFieldPosition.Right) == null)
                    {
                        availablePositions.Add(PBEFieldPosition.Right);
                    }
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(team.Battle.BattleFormat));
            }
            var switches = new PBESwitchIn[team.SwitchInsRequired];
            for (int i = 0; i < team.SwitchInsRequired; i++)
            {
                switches[i] = new PBESwitchIn(available[i].Id, availablePositions[i]);
            }
            return switches;
        }
    }
}
