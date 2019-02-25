using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.AI
{
    /// <summary>
    /// Creates valid decisions for a team in a battle. Decisions may not be valid for custom settings and/or move changes.
    /// </summary>
    public static partial class PBEAIManager
    {
        /// <summary>
        /// Creates valid actions for a battle turn for a specific team.
        /// </summary>
        /// <param name="team">The team to create actions for.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="team"/> has no active battlers or <paramref name="team"/>'s <see cref="PBETeam.Battle"/>'s <see cref="PBEBattle.BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a Pokémon has no moves, the AI tries to use a move with invalid targets, or <paramref name="team"/>'s <see cref="PBETeam.Battle"/>'s <see cref="PBEBattle.BattleFormat"/> is invalid.</exception>
        public static IEnumerable<PBEAction> CreateActions(PBETeam team)
        {
            if (team.Battle.BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(team.Battle.BattleState)} must be {PBEBattleState.WaitingForActions} to create actions.");
            }
            PBEPokemon[] active = team.ActiveBattlers.ToArray();
            if (active.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(team)} must have at least one active battler.");
            }

            PBETeam opposingTeam = team == team.Battle.Teams[0] ? team.Battle.Teams[1] : team.Battle.Teams[0];
            var actions = new PBEAction[team.ActionsRequired.Count];
            var standBy = new List<PBEPokemon>();
            for (int i = 0; i < actions.Length; i++)
            {
                PBEPokemon pkmn = team.ActionsRequired[i];
                if (pkmn.Moves.All(m => m == PBEMove.None))
                {
                    throw new ArgumentOutOfRangeException(nameof(pkmn.Moves), $"{pkmn.Shell.Nickname} has no moves.");
                }

                // If a Pokémon is forced to struggle, it is best that it just stays in until it faints
                if (pkmn.IsForcedToStruggle())
                {
                    actions[i].Decision = PBEDecision.Fight;
                    actions[i].FightMove = PBEMove.Struggle;
                    actions[i].FightTargets = GetSpreadMoveTargets(pkmn, pkmn.GetMoveTargets(PBEMove.Struggle));
                }
                // If a Pokémon has a temp locked move (Dig, Dive, Shadow Force) it must be used
                else if (pkmn.TempLockedMove != PBEMove.None)
                {
                    actions[i].Decision = PBEDecision.Fight;
                    actions[i].FightMove = pkmn.TempLockedMove;
                    actions[i].FightTargets = pkmn.TempLockedTargets;
                }
                // The Pokémon is free to switch or fight (unless it cannot switch due to Magnet Pull etc)
                else
                {
                    // Gather all options of switching and moves
                    PBEPokemon[] availableForSwitch = team.Party.Except(standBy).Except(active).Where(p => p.HP > 0).ToArray();
                    var movesToChooseFrom = new List<PBEMove>();
                    if (pkmn.ChoiceLockedMove != PBEMove.None)
                    {
                        movesToChooseFrom.Add(pkmn.ChoiceLockedMove);
                    }
                    else
                    {
                        for (int m = 0; m < pkmn.Moves.Length; m++)
                        {
                            if (pkmn.PP[m] > 0)
                            {
                                movesToChooseFrom.Add(pkmn.Moves[m]);
                            }
                        }
                    }

                    var possibleActions = new List<Tuple<PBEAction, double>>(); // Associate specific actions with a score
                    for (int m = 0; m < movesToChooseFrom.Count; m++) // Score moves
                    {
                        PBEMove move = movesToChooseFrom[m];
                        PBEType moveType = pkmn.GetMoveType(move);
                        PBEMoveTarget moveTargets = pkmn.GetMoveTargets(move);
                        PBETarget[] possibleTargets;
                        if (PBEMoveData.IsSpreadMove(moveTargets))
                        {
                            possibleTargets = new PBETarget[] { GetSpreadMoveTargets(pkmn, moveTargets) };
                        }
                        else
                        {
                            possibleTargets = GetPossibleTargets(pkmn, moveTargets);
                        }
                        foreach (PBETarget possibleTarget in possibleTargets)
                        {
                            // TODO: RandomFoeSurrounding (probably just account for the specific effects that use this target type)
                            var targets = new List<PBEPokemon>();
                            if (possibleTarget.HasFlag(PBETarget.AllyLeft))
                            {
                                targets.Add(team.TryGetPokemon(PBEFieldPosition.Left));
                            }
                            if (possibleTarget.HasFlag(PBETarget.AllyCenter))
                            {
                                targets.Add(team.TryGetPokemon(PBEFieldPosition.Center));
                            }
                            if (possibleTarget.HasFlag(PBETarget.AllyRight))
                            {
                                targets.Add(team.TryGetPokemon(PBEFieldPosition.Right));
                            }
                            if (possibleTarget.HasFlag(PBETarget.FoeLeft))
                            {
                                targets.Add(opposingTeam.TryGetPokemon(PBEFieldPosition.Left));
                            }
                            if (possibleTarget.HasFlag(PBETarget.FoeCenter))
                            {
                                targets.Add(opposingTeam.TryGetPokemon(PBEFieldPosition.Center));
                            }
                            if (possibleTarget.HasFlag(PBETarget.FoeRight))
                            {
                                targets.Add(opposingTeam.TryGetPokemon(PBEFieldPosition.Right));
                            }

                            double score = 0.0;
                            switch (PBEMoveData.Data[move].Effect)
                            {
                                case PBEMoveEffect.Hail:
                                    {
                                        if (team.Battle.Weather == PBEWeather.Hailstorm)
                                        {
                                            score -= 100;
                                        }
                                        break;
                                    }
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
                                                // TODO: Put "KnownType1" etc so the AI doesn't cheat (using species types for now)
                                                // TODO: Check items
                                                // TODO: Stat changes and accuracy
                                                // TODO: Check base power specifically against hp remaining (include spread move damage reduction)
                                                PBEPokemonData pData = PBEPokemonData.Data[target.VisualSpecies];
                                                double typeEffectiveness = PBEPokemonData.TypeEffectiveness[(int)moveType][(int)pData.Type1];
                                                typeEffectiveness *= PBEPokemonData.TypeEffectiveness[(int)moveType][(int)pData.Type2];
                                                if (typeEffectiveness <= 0.0) // (-infinity, 0.0] Ineffective
                                                {
                                                    score += target.Team == opposingTeam ? -60 : -1;
                                                }
                                                else if (typeEffectiveness <= 0.25) // (0.0, 0.25] NotVeryEffective
                                                {
                                                    score += target.Team == opposingTeam ? -30 : -5;
                                                }
                                                else if (typeEffectiveness < 1.0) // (0.25, 1.0) NotVeryEffective
                                                {
                                                    score += target.Team == opposingTeam ? -10 : -10;
                                                }
                                                else if (typeEffectiveness == 1.0) // [1.0, 1.0] Normal
                                                {
                                                    score += target.Team == opposingTeam ? +5 : -15;
                                                }
                                                else if (typeEffectiveness < 4.0) // (1.0, 4.0) SuperEffective
                                                {
                                                    score += target.Team == opposingTeam ? +20 : -20;
                                                }
                                                else // [4.0, infinity) SuperEffective
                                                {
                                                    score += target.Team == opposingTeam ? +40 : -30;
                                                }
                                                if (pkmn.HasType(moveType) && typeEffectiveness > 0.0) // STAB
                                                {
                                                    score += (pkmn.Ability == PBEAbility.Adaptability ? 20 : 15) * (target.Team == opposingTeam ? +1 : -1);
                                                }
                                            }
                                        }

                                        break;
                                    }
                                case PBEMoveEffect.RestoreTargetHP:
                                    {
                                        PBEPokemon target = targets[0];
                                        if (target == null || target.Team == opposingTeam)
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
                                case PBEMoveEffect.BrickBreak:
                                case PBEMoveEffect.Burn:
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
                                case PBEMoveEffect.Dig:
                                case PBEMoveEffect.Dive:
                                case PBEMoveEffect.Endeavor:
                                case PBEMoveEffect.Fail:
                                case PBEMoveEffect.FinalGambit:
                                case PBEMoveEffect.FlareBlitz:
                                case PBEMoveEffect.Flatter:
                                case PBEMoveEffect.Fly:
                                case PBEMoveEffect.FocusEnergy:
                                case PBEMoveEffect.GastroAcid:
                                case PBEMoveEffect.Growth:
                                case PBEMoveEffect.HelpingHand:
                                case PBEMoveEffect.HPDrain:
                                case PBEMoveEffect.LeechSeed:
                                case PBEMoveEffect.LightScreen:
                                case PBEMoveEffect.LowerTarget_ATK_DEF_By1:
                                case PBEMoveEffect.LowerUser_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2:
                                case PBEMoveEffect.LuckyChant:
                                case PBEMoveEffect.Metronome:
                                case PBEMoveEffect.Moonlight:
                                case PBEMoveEffect.OneHitKnockout:
                                case PBEMoveEffect.PainSplit:
                                case PBEMoveEffect.Paralyze:
                                case PBEMoveEffect.Poison:
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
                                case PBEMoveEffect.Recoil:
                                case PBEMoveEffect.Reflect:
                                case PBEMoveEffect.Rest:
                                case PBEMoveEffect.RestoreUserHP:
                                case PBEMoveEffect.SeismicToss:
                                case PBEMoveEffect.Selfdestruct:
                                case PBEMoveEffect.SetDamage:
                                case PBEMoveEffect.Sleep:
                                case PBEMoveEffect.Snore:
                                case PBEMoveEffect.Spikes:
                                case PBEMoveEffect.StealthRock:
                                case PBEMoveEffect.Struggle:
                                case PBEMoveEffect.Substitute:
                                case PBEMoveEffect.SuckerPunch:
                                case PBEMoveEffect.SuperFang:
                                case PBEMoveEffect.Swagger:
                                case PBEMoveEffect.Toxic:
                                case PBEMoveEffect.ToxicSpikes:
                                case PBEMoveEffect.Transform:
                                case PBEMoveEffect.TrickRoom:
                                case PBEMoveEffect.VoltTackle:
                                case PBEMoveEffect.Whirlwind:
                                case PBEMoveEffect.WideGuard:
                                    {
                                        // TODO Moves
                                        score -= 200;
                                        break;
                                    }
                            }
                            var mAction = new PBEAction
                            {
                                Decision = PBEDecision.Fight,
                                FightMove = move,
                                FightTargets = possibleTarget
                            };
                            possibleActions.Add(Tuple.Create(mAction, score));
                        }
                    }
                    for (int s = 0; s < availableForSwitch.Length; s++) // Score switches
                    {
                        PBEPokemon switchPkmn = availableForSwitch[s];
                        // TODO: Entry hazards
                        // TODO: Known moves of active battlers
                        // TODO: Type effectiveness
                        double score = 0.0;
                        var sAction = new PBEAction
                        {
                            Decision = PBEDecision.SwitchOut,
                            SwitchPokemonId = switchPkmn.Id
                        };
                        possibleActions.Add(Tuple.Create(sAction, score));
                    }

                    string ToDebugString(Tuple<PBEAction, double> a)
                    {
                        string str = "{";
                        if (a.Item1.Decision == PBEDecision.Fight)
                        {
                            str += string.Format("Fight {0} {1}", a.Item1.FightMove, a.Item1.FightTargets);
                        }
                        else
                        {
                            str += string.Format("Switch {0}", team.TryGetPokemon(a.Item1.SwitchPokemonId).Shell.Nickname);
                        }
                        str += " [" + a.Item2 + "]}";
                        return str;
                    }
                    IOrderedEnumerable<Tuple<PBEAction, double>> byScore = possibleActions.OrderByDescending(a => a.Item2);
                    Debug.WriteLine("{0}'s possible actions: {1}", pkmn.Shell.Nickname, byScore.Select(a => ToDebugString(a)).Print());
                    double bestScore = byScore.First().Item2;
                    actions[i] = byScore.Where(a => a.Item2 == bestScore).Sample().Item1; // Pick random action of the ones that tied for best score
                }

                // Action was chosen, finish up for this Pokémon
                actions[i].PokemonId = pkmn.Id;
                if (actions[i].Decision == PBEDecision.SwitchOut)
                {
                    standBy.Add(team.TryGetPokemon(actions[i].SwitchPokemonId));
                }
            }
            return actions;
        }

        /// <summary>
        /// Creates valid switches for a battle for a specific team.
        /// </summary>
        /// <param name="team">The team to create switches for.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="team"/> does not require switch-ins or <paramref name="team"/>'s <see cref="PBETeam.Battle"/>'s <see cref="PBEBattle.BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="team"/>'s <see cref="PBETeam.Battle"/>'s <see cref="PBEBattle.BattleFormat"/> is invalid.</exception>
        public static IEnumerable<Tuple<byte, PBEFieldPosition>> CreateSwitches(PBETeam team)
        {
            if (team.Battle.BattleState != PBEBattleState.WaitingForSwitchIns)
            {
                throw new InvalidOperationException($"{nameof(team.Battle.BattleState)} must be {PBEBattleState.WaitingForSwitchIns} to create switch-ins.");
            }
            if (team.SwitchInsRequired == 0)
            {
                throw new InvalidOperationException($"{nameof(team)} must require switch-ins.");
            }
            var switches = new List<Tuple<byte, PBEFieldPosition>>(team.SwitchInsRequired);
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
            for (int i = 0; i < team.SwitchInsRequired; i++)
            {
                switches.Add(Tuple.Create(available[i].Id, availablePositions[i]));
            }
            return switches;
        }
    }
}
