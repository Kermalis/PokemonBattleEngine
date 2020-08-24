using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBEBattle
    {
        public delegate void BattleEvent(PBEBattle battle, IPBEPacket packet);
        public event BattleEvent OnNewEvent;

        private void Broadcast(IPBEPacket packet)
        {
            Events.Add(packet);
            OnNewEvent?.Invoke(this, packet);
        }

        private void BroadcastAbility(PBEBattlePokemon abilityOwner, PBEBattlePokemon pokemon2, PBEAbility ability, PBEAbilityAction abilityAction)
        {
            abilityOwner.Ability = ability;
            abilityOwner.KnownAbility = ability;
            Broadcast(new PBEAbilityPacket(abilityOwner, pokemon2, ability, abilityAction));
        }
        private void BroadcastAbilityReplaced(PBEBattlePokemon abilityOwner, PBEAbility newAbility)
        {
            PBEAbility? oldAbility = newAbility == PBEAbility.None ? (PBEAbility?)null : abilityOwner.Ability; // Gastro Acid does not reveal previous ability
            abilityOwner.Ability = newAbility;
            abilityOwner.KnownAbility = newAbility;
            Broadcast(new PBEAbilityReplacedPacket(abilityOwner, oldAbility, newAbility));
        }
        private void BroadcastBattleStatus(PBEBattleStatus battleStatus, PBEBattleStatusAction battleStatusAction)
        {
            switch (battleStatusAction)
            {
                case PBEBattleStatusAction.Added: BattleStatus |= battleStatus; break;
                case PBEBattleStatusAction.Cleared:
                case PBEBattleStatusAction.Ended: BattleStatus &= ~battleStatus; break;
                default: throw new ArgumentOutOfRangeException(nameof(battleStatusAction));
            }
            Broadcast(new PBEBattleStatusPacket(battleStatus, battleStatusAction));
        }
        private void BroadcastCapture(PBEBattlePokemon pokemon, PBEItem ball, byte numShakes, bool success, bool critical)
        {
            Broadcast(new PBECapturePacket(pokemon, ball, numShakes, success, critical));
        }
        private void BroadcastFleeFailed(PBEBattlePokemon pokemon)
        {
            Broadcast(new PBEFleeFailedPacket(pokemon));
        }
        private void BroadcastHaze()
        {
            Broadcast(new PBEHazePacket());
        }
        private void BroadcastIllusion(PBEBattlePokemon pokemon)
        {
            Broadcast(new PBEIllusionPacket(pokemon));
        }
        private void BroadcastItem(PBEBattlePokemon itemHolder, PBEBattlePokemon pokemon2, PBEItem item, PBEItemAction itemAction)
        {
            switch (itemAction)
            {
                case PBEItemAction.Consumed:
                {
                    itemHolder.Item = PBEItem.None;
                    itemHolder.KnownItem = PBEItem.None;
                    break;
                }
                default:
                {
                    itemHolder.Item = item;
                    itemHolder.KnownItem = item;
                    break;
                }
            }
            Broadcast(new PBEItemPacket(itemHolder, pokemon2, item, itemAction));
        }
        private void BroadcastItemTurn(PBEBattlePokemon itemUser, PBEItem item, PBEItemTurnAction itemAction)
        {
            Broadcast(new PBEItemTurnPacket(itemUser, item, itemAction));
        }
        private void BroadcastMoveCrit(PBEBattlePokemon victim)
        {
            Broadcast(new PBEMoveCritPacket(victim));
        }
        private void BroadcastMoveLock_ChoiceItem(PBEBattlePokemon moveUser, PBEMove lockedMove)
        {
            moveUser.ChoiceLockedMove = lockedMove;
            Broadcast(new PBEMoveLockPacket(moveUser, PBEMoveLockType.ChoiceItem, lockedMove));
        }
        private void BroadcastMoveLock_Temporary(PBEBattlePokemon moveUser, PBEMove lockedMove, PBETurnTarget lockedTargets)
        {
            moveUser.TempLockedMove = lockedMove;
            moveUser.TempLockedTargets = lockedTargets;
            Broadcast(new PBEMoveLockPacket(moveUser, PBEMoveLockType.Temporary, lockedMove, lockedTargets));
        }
        private void BroadcastMovePPChanged(PBEBattlePokemon moveUser, PBEMove move, int amountReduced)
        {
            Broadcast(new PBEMovePPChangedPacket(moveUser, move, amountReduced));
        }
        private void BroadcastMoveResult(PBEBattlePokemon moveUser, PBEBattlePokemon pokemon2, PBEResult result)
        {
            Broadcast(new PBEMoveResultPacket(moveUser, pokemon2, result));
        }
        private void BroadcastMoveUsed(PBEBattlePokemon moveUser, PBEMove move)
        {
            bool owned;
            if (!_calledFromOtherMove && moveUser.Moves.Contains(move))
            {
                if (!moveUser.KnownMoves.Contains(move))
                {
                    moveUser.KnownMoves[PBEMove.MAX].Move = move;
                }
                owned = true;
            }
            else
            {
                owned = false;
            }
            Broadcast(new PBEMoveUsedPacket(moveUser, move, owned));
        }
        private void BroadcastPkmnFainted(PBEBattlePokemon pokemon, PBEBattlePokemon disguisedAsPokemon, PBEFieldPosition oldPosition)
        {
            Broadcast(new PBEPkmnFaintedPacket(pokemon, disguisedAsPokemon, oldPosition));
        }
        private void BroadcastPkmnFormChanged(PBEBattlePokemon pokemon, PBEForm newForm, PBEAbility newAbility, PBEAbility newKnownAbility, bool isRevertForm)
        {
            pokemon.Ability = newAbility;
            pokemon.KnownAbility = newKnownAbility;
            pokemon.Form = newForm;
            pokemon.KnownForm = newForm;
            if (isRevertForm)
            {
                pokemon.RevertForm = newForm;
                pokemon.RevertAbility = newAbility;
            }
            pokemon.SetStats(false);
            var pData = PBEPokemonData.GetData(pokemon.Species, newForm);
            PBEType type1 = pData.Type1;
            pokemon.Type1 = type1;
            pokemon.KnownType1 = type1;
            PBEType type2 = pData.Type2;
            pokemon.Type2 = type2;
            pokemon.KnownType2 = type2;
            double weight = pData.Weight; // TODO: Is weight updated here? Bulbapedia claims in Autotomize's page that it is not
            pokemon.Weight = weight;
            pokemon.KnownWeight = weight;
            Broadcast(new PBEPkmnFormChangedPacket(pokemon, isRevertForm));
            // BUG: PBEStatus2.PowerTrick is not cleared when changing form
            if (Settings.BugFix && pokemon.Status2.HasFlag(PBEStatus2.PowerTrick))
            {
                BroadcastStatus2(pokemon, pokemon, PBEStatus2.PowerTrick, PBEStatusAction.Ended);
            }
        }
        private void BroadcastPkmnHPChanged(PBEBattlePokemon pokemon, ushort oldHP, double oldHPPercentage)
        {
            Broadcast(new PBEPkmnHPChangedPacket(pokemon, oldHP, oldHPPercentage));
        }
        private void BroadcastPkmnStatChanged(PBEBattlePokemon pokemon, PBEStat stat, sbyte oldValue, sbyte newValue)
        {
            Broadcast(new PBEPkmnStatChangedPacket(pokemon, stat, oldValue, newValue));
        }
        private void BroadcastPkmnSwitchIn(PBETrainer trainer, PBEPkmnSwitchInPacket.PBEPkmnSwitchInInfo[] switchIns, PBEBattlePokemon forcedByPokemon = null)
        {
            Broadcast(new PBEPkmnSwitchInPacket(trainer, switchIns, forcedByPokemon));
        }
        private void BroadcastPkmnSwitchOut(PBEBattlePokemon pokemon, PBEBattlePokemon disguisedAsPokemon, PBEFieldPosition oldPosition, PBEBattlePokemon forcedByPokemon = null)
        {
            Broadcast(new PBEPkmnSwitchOutPacket(pokemon, disguisedAsPokemon, oldPosition, forcedByPokemon));
        }
        private void BroadcastPsychUp(PBEBattlePokemon user, PBEBattlePokemon target)
        {
            user.AttackChange = target.AttackChange;
            user.DefenseChange = target.DefenseChange;
            user.SpAttackChange = target.SpAttackChange;
            user.SpDefenseChange = target.SpDefenseChange;
            user.SpeedChange = target.SpeedChange;
            user.AccuracyChange = target.AccuracyChange;
            user.EvasionChange = target.EvasionChange;
            Broadcast(new PBEPsychUpPacket(user, target));
        }
        private void BroadcastReflectType(PBEBattlePokemon user, PBEBattlePokemon target)
        {
            user.Type1 = user.KnownType1 = target.KnownType1 = target.Type1;
            user.Type2 = user.KnownType2 = target.KnownType2 = target.Type2;
            Broadcast(new PBEReflectTypePacket(user, target));
        }

        private void BroadcastDraggedOut(PBEBattlePokemon pokemon)
        {
            Broadcast(new PBESpecialMessagePacket(PBESpecialMessage.DraggedOut, pokemon));
        }
        private void BroadcastEndure(PBEBattlePokemon pokemon)
        {
            Broadcast(new PBESpecialMessagePacket(PBESpecialMessage.Endure, pokemon));
        }
        private void BroadcastHPDrained(PBEBattlePokemon pokemon)
        {
            Broadcast(new PBESpecialMessagePacket(PBESpecialMessage.HPDrained, pokemon));
        }
        private void BroadcastMagnitude(byte magnitude)
        {
            Broadcast(new PBESpecialMessagePacket(PBESpecialMessage.Magnitude, magnitude));
        }
        private void BroadcastMultiHit(byte numHits)
        {
            Broadcast(new PBESpecialMessagePacket(PBESpecialMessage.MultiHit, numHits));
        }
        private void BroadcastNothingHappened()
        {
            Broadcast(new PBESpecialMessagePacket(PBESpecialMessage.NothingHappened));
        }
        private void BroadcastOneHitKnockout()
        {
            Broadcast(new PBESpecialMessagePacket(PBESpecialMessage.OneHitKnockout));
        }
        private void BroadcastPainSplit(PBEBattlePokemon user, PBEBattlePokemon target)
        {
            Broadcast(new PBESpecialMessagePacket(PBESpecialMessage.PainSplit, user, target));
        }
        private void BroadcastPayDay()
        {
            Broadcast(new PBESpecialMessagePacket(PBESpecialMessage.PayDay));
        }
        private void BroadcastRecoil(PBEBattlePokemon pokemon)
        {
            Broadcast(new PBESpecialMessagePacket(PBESpecialMessage.Recoil, pokemon));
        }
        private void BroadcastStruggle(PBEBattlePokemon pokemon)
        {
            Broadcast(new PBESpecialMessagePacket(PBESpecialMessage.Struggle, pokemon));
        }

        private void BroadcastStatus1(PBEBattlePokemon status1Receiver, PBEBattlePokemon pokemon2, PBEStatus1 status1, PBEStatusAction statusAction)
        {
            Broadcast(new PBEStatus1Packet(status1Receiver, pokemon2, status1, statusAction));
        }
        private void BroadcastStatus2(PBEBattlePokemon status2Receiver, PBEBattlePokemon pokemon2, PBEStatus2 status2, PBEStatusAction statusAction)
        {
            switch (statusAction)
            {
                case PBEStatusAction.Added:
                case PBEStatusAction.Announced:
                case PBEStatusAction.CausedImmobility:
                case PBEStatusAction.Damage: status2Receiver.Status2 |= status2; status2Receiver.KnownStatus2 |= status2; break;
                case PBEStatusAction.Cleared:
                case PBEStatusAction.Ended: status2Receiver.Status2 &= ~status2; status2Receiver.KnownStatus2 &= ~status2; break;
                default: throw new ArgumentOutOfRangeException(nameof(statusAction));
            }
            Broadcast(new PBEStatus2Packet(status2Receiver, pokemon2, status2, statusAction));
        }
        private void BroadcastTeamStatus(PBETeam team, PBETeamStatus teamStatus, PBETeamStatusAction teamStatusAction, PBEBattlePokemon damageVictim = null)
        {
            switch (teamStatusAction)
            {
                case PBETeamStatusAction.Added:
                case PBETeamStatusAction.Damage: team.TeamStatus |= teamStatus; break;
                case PBETeamStatusAction.Cleared:
                case PBETeamStatusAction.Ended: team.TeamStatus &= ~teamStatus; break;
                default: throw new ArgumentOutOfRangeException(nameof(teamStatusAction));
            }
            Broadcast(new PBETeamStatusPacket(team, teamStatus, teamStatusAction, damageVictim: damageVictim));
        }
        private void BroadcastTransform(PBEBattlePokemon user, PBEBattlePokemon target)
        {
            Broadcast(new PBETransformPacket(user, target));
        }
        private void BroadcastTypeChanged(PBEBattlePokemon pokemon, PBEType type1, PBEType type2)
        {
            pokemon.Type1 = type1;
            pokemon.KnownType1 = type1;
            pokemon.Type2 = type2;
            pokemon.KnownType2 = type2;
            Broadcast(new PBETypeChangedPacket(pokemon, type1, type2));
        }
        private void BroadcastWeather(PBEWeather weather, PBEWeatherAction weatherAction, PBEBattlePokemon damageVictim = null)
        {
            Broadcast(new PBEWeatherPacket(weather, weatherAction, damageVictim));
        }
        private void BroadcastWildPkmnAppeared(PBEWildPkmnAppearedPacket.PBEWildPkmnInfo[] appearances)
        {
            Broadcast(new PBEWildPkmnAppearedPacket(appearances));
        }
        private void BroadcastActionsRequest(PBETrainer trainer)
        {
            Broadcast(new PBEActionsRequestPacket(trainer));
        }
        private void BroadcastAutoCenter(PBEBattlePokemon pokemon0, PBEFieldPosition pokemon0OldPosition, PBEBattlePokemon pokemon1, PBEFieldPosition pokemon1OldPosition)
        {
            Broadcast(new PBEAutoCenterPacket(pokemon0, pokemon0OldPosition, pokemon1, pokemon1OldPosition));
        }
        private void BroadcastBattle()
        {
            Broadcast(new PBEBattlePacket(this));
        }
        private void BroadcastBattleResult()
        {
            Broadcast(new PBEBattleResultPacket(BattleResult.Value));
        }
        private void BroadcastSwitchInRequest(PBETrainer trainer)
        {
            Broadcast(new PBESwitchInRequestPacket(trainer));
        }
        private void BroadcastTurnBegan()
        {
            Broadcast(new PBETurnBeganPacket(TurnNumber));
        }

        public static string GetDefaultMessage(PBEBattle battle, IPBEPacket packet, bool showRawHP = false, PBETrainer userTrainer = null,
            Func<PBEBattlePokemon, bool, string> pkmnNameFunc = null, Func<PBETrainer, string> trainerNameFunc = null, Func<PBETeam, bool, string> teamNameFunc = null)
        {
            if (battle is null)
            {
                throw new ArgumentNullException(nameof(battle));
            }
            if (packet is null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            // This is not used by switching in or out or wild Pokémon appearing; those always use the known nickname
            string GetPkmnName(PBEBattlePokemon pkmn, bool firstLetterCapitalized)
            {
                if (pkmnNameFunc != null)
                {
                    return pkmnNameFunc(pkmn, firstLetterCapitalized);
                }
                if (pkmn is null)
                {
                    return string.Empty;
                }
                if (pkmn.IsWild)
                {
                    string wildPrefix = firstLetterCapitalized ? "The wild " : "the wild ";
                    return wildPrefix + pkmn.KnownNickname;
                }
                // Replay/spectator always see prefix, but if you're battling a multi-battle, your Pokémon should still have no prefix
                if (userTrainer is null || (pkmn.Trainer != userTrainer && pkmn.Team.Trainers.Count > 1))
                {
                    return $"{GetTrainerName(pkmn.Trainer)}'s {pkmn.KnownNickname}";
                }
                string ownerPrefix = string.Empty;
                string foePrefix = firstLetterCapitalized ? "The foe's " : "the foe's ";
                string prefix = pkmn.Trainer == userTrainer ? ownerPrefix : foePrefix;
                return prefix + pkmn.KnownNickname;
            }
            string GetTrainerName(PBETrainer trainer)
            {
                if (trainerNameFunc != null)
                {
                    return trainerNameFunc(trainer);
                }
                return trainer.Name;
            }
            // This is not used by PBEBattleResultPacket; those always use the combined name
            string GetTeamName(PBETeam team, bool firstLetterCapitalized)
            {
                if (teamNameFunc != null)
                {
                    return teamNameFunc(team, firstLetterCapitalized);
                }
                if (userTrainer is null)
                {
                    return $"{team.CombinedName}'s";
                }
                string ownerPrefix = firstLetterCapitalized ? "Your" : "your";
                string foePrefix = firstLetterCapitalized ? "The opposing" : "the opposing";
                return team == userTrainer.Team ? ownerPrefix : foePrefix;
            }
            string DoHiddenHP(PBEBattlePokemon pokemon, double percentageChange, double absPercentageChange)
            {
                return string.Format("{0} {1} {2:P2} of its HP!", GetPkmnName(pokemon, true), percentageChange <= 0 ? "lost" : "restored", absPercentageChange);
            }

            switch (packet)
            {
                case PBEAbilityPacket ap:
                {
                    PBEBattlePokemon abilityOwner = ap.AbilityOwnerTrainer.TryGetPokemon(ap.AbilityOwner);
                    PBEBattlePokemon pokemon2 = ap.AbilityOwnerTrainer.TryGetPokemon(ap.Pokemon2);
                    bool abilityOwnerCaps = true,
                            pokemon2Caps = true;
                    string message;
                    switch (ap.Ability)
                    {
                        case PBEAbility.AirLock:
                        case PBEAbility.CloudNine:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Weather: message = "{0}'s {2} causes the effects of weather to disappear!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.Anticipation:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Announced: message = "{0}'s {2} made it shudder!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.BadDreams:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Damage: message = "{1} is tormented by {0}'s {2}!"; abilityOwnerCaps = false; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.BigPecks:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Stats: message = $"{{0}}'s {PBELocalizedString.GetStatName(PBEStat.Defense).English} was not lowered!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.ClearBody:
                        case PBEAbility.WhiteSmoke:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Stats: message = "{0}'s {2} prevents stat reduction!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.ColorChange:
                        case PBEAbility.FlowerGift:
                        case PBEAbility.Forecast:
                        case PBEAbility.Imposter:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.ChangedAppearance: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.CuteCharm:
                        case PBEAbility.EffectSpore:
                        case PBEAbility.FlameBody:
                        case PBEAbility.Healer:
                        case PBEAbility.PoisonPoint:
                        case PBEAbility.ShedSkin:
                        case PBEAbility.Static:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.ChangedStatus: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.Download:
                        case PBEAbility.Intimidate:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Stats: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.Drizzle:
                        case PBEAbility.Drought:
                        case PBEAbility.SandStream:
                        case PBEAbility.SnowWarning:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Weather: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.HyperCutter:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Stats: message = $"{{0}}'s {PBELocalizedString.GetStatName(PBEStat.Attack).English} was not lowered!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.IceBody:
                        case PBEAbility.PoisonHeal:
                        case PBEAbility.RainDish:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.RestoredHP: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.Illusion:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.ChangedAppearance: goto bottom;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                        }
                        case PBEAbility.Immunity:
                        case PBEAbility.Insomnia:
                        case PBEAbility.Limber:
                        case PBEAbility.MagmaArmor:
                        case PBEAbility.Oblivious:
                        case PBEAbility.OwnTempo:
                        case PBEAbility.VitalSpirit:
                        case PBEAbility.WaterVeil:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.ChangedStatus:
                                case PBEAbilityAction.PreventedStatus: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.IronBarbs:
                        case PBEAbility.Justified:
                        case PBEAbility.Levitate:
                        case PBEAbility.Mummy:
                        case PBEAbility.Rattled:
                        case PBEAbility.RoughSkin:
                        case PBEAbility.SolarPower:
                        case PBEAbility.Sturdy:
                        case PBEAbility.WeakArmor:
                        case PBEAbility.WonderGuard:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Damage: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.KeenEye:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Stats: message = $"{{0}}'s {PBELocalizedString.GetStatName(PBEStat.Accuracy).English} was not lowered!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.LeafGuard:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.PreventedStatus: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.LiquidOoze:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Damage: message = "{1} sucked up the liquid ooze!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.MoldBreaker:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Announced: message = "{0} breaks the mold!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.Moody:
                        case PBEAbility.SpeedBoost:
                        case PBEAbility.Steadfast:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Stats: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.SlowStart:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Announced: message = "{0} can't get it going!"; break;
                                case PBEAbilityAction.SlowStart_Ended: message = "{0} finally got its act together!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.Teravolt:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Announced: message = "{0} is radiating a bursting aura!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.Turboblaze:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Announced: message = "{0} is radiating a blazing aura!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(ap.Ability));
                    }
                    return string.Format(message, GetPkmnName(abilityOwner, abilityOwnerCaps), GetPkmnName(pokemon2, pokemon2Caps), PBELocalizedString.GetAbilityName(ap.Ability).English);
                }
                case PBEAbilityReplacedPacket arp:
                {
                    PBEBattlePokemon abilityOwner = arp.AbilityOwnerTrainer.TryGetPokemon(arp.AbilityOwner);
                    string message;
                    switch (arp.NewAbility)
                    {
                        case PBEAbility.None: message = "{0}'s {1} was suppressed!"; break;
                        default: message = "{0}'s {1} was changed to {2}!"; break;
                    }
                    return string.Format(message,
                        GetPkmnName(abilityOwner, true),
                        arp.OldAbility.HasValue ? PBELocalizedString.GetAbilityName(arp.OldAbility.Value).English : "Ability",
                        PBELocalizedString.GetAbilityName(arp.NewAbility).English);
                }
                case PBEBattleStatusPacket bsp:
                {
                    string message;
                    switch (bsp.BattleStatus)
                    {
                        case PBEBattleStatus.TrickRoom:
                        {
                            switch (bsp.BattleStatusAction)
                            {
                                case PBEBattleStatusAction.Added: message = "The dimensions were twisted!"; break;
                                case PBEBattleStatusAction.Cleared:
                                case PBEBattleStatusAction.Ended: message = "The twisted dimensions returned to normal!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(bsp.BattleStatusAction));
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(bsp.BattleStatus));
                    }
                    return message;
                }
                case PBECapturePacket cp:
                {
                    PBEBattlePokemon pokemon = cp.PokemonTrainer.TryGetPokemon(cp.Pokemon);
                    string ballEnglish = PBELocalizedString.GetItemName(cp.Ball).English;
                    if (cp.Success)
                    {
                        return string.Format("Gotcha! {0} was caught with the {1} after {2} shake{3}!", pokemon.Nickname, ballEnglish, cp.NumShakes, cp.NumShakes == 1 ? string.Empty : "s");
                    }
                    if (cp.NumShakes == 0)
                    {
                        return "The Pokémon broke free without shaking!";
                    }
                    return string.Format("The Pokémon broke free after {0} shake{1}!", cp.NumShakes, cp.NumShakes == 1 ? string.Empty : "s");
                }
                case PBEFleeFailedPacket ffp:
                {
                    string name;
                    if (ffp.Pokemon == PBEFieldPosition.None)
                    {
                        name = GetTrainerName(ffp.PokemonTrainer);
                    }
                    else
                    {
                        PBEBattlePokemon pokemon = ffp.PokemonTrainer.TryGetPokemon(ffp.Pokemon);
                        name = GetPkmnName(pokemon, true);
                    }
                    return string.Format("{0} could not get away!", name);
                }
                case PBEHazePacket _:
                {
                    return "All stat changes were eliminated!";
                }
                case PBEItemPacket ip:
                {
                    PBEBattlePokemon itemHolder = ip.ItemHolderTrainer.TryGetPokemon(ip.ItemHolder);
                    PBEBattlePokemon pokemon2 = ip.Pokemon2Trainer.TryGetPokemon(ip.Pokemon2);
                    bool itemHolderCaps = true,
                            pokemon2Caps = false;
                    string message;
                    switch (ip.Item)
                    {
                        case PBEItem.AguavBerry:
                        case PBEItem.BerryJuice:
                        case PBEItem.FigyBerry:
                        case PBEItem.IapapaBerry:
                        case PBEItem.MagoBerry:
                        case PBEItem.OranBerry:
                        case PBEItem.SitrusBerry:
                        case PBEItem.WikiBerry:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Consumed: message = "{0} restored its health using its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.ApicotBerry:
                        case PBEItem.GanlonBerry:
                        case PBEItem.LiechiBerry:
                        case PBEItem.PetayaBerry:
                        case PBEItem.SalacBerry:
                        case PBEItem.StarfBerry:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Consumed: message = "{0} used its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.BugGem:
                        case PBEItem.DarkGem:
                        case PBEItem.DragonGem:
                        case PBEItem.ElectricGem:
                        case PBEItem.FightingGem:
                        case PBEItem.FireGem:
                        case PBEItem.FlyingGem:
                        case PBEItem.GhostGem:
                        case PBEItem.GrassGem:
                        case PBEItem.GroundGem:
                        case PBEItem.IceGem:
                        case PBEItem.NormalGem:
                        case PBEItem.PoisonGem:
                        case PBEItem.PsychicGem:
                        case PBEItem.RockGem:
                        case PBEItem.SteelGem:
                        case PBEItem.WaterGem:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Consumed: message = "The {2} strengthened {0}'s power!"; itemHolderCaps = false; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.BlackSludge:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Damage: message = "{0} is hurt by its {2}!"; break;
                                case PBEItemAction.RestoredHP: message = "{0} restored a little HP using its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.DestinyKnot:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.ChangedStatus: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.FlameOrb:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.ChangedStatus: message = "{0} was burned by its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.FocusBand:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Damage: message = "{0} hung on using its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.FocusSash:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Consumed: message = "{0} hung on using its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.Leftovers:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.RestoredHP: message = "{0} restored a little HP using its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.LifeOrb:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Damage: message = "{0} is hurt by its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.PowerHerb:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Consumed: message = "{0} became fully charged due to its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.RockyHelmet:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Damage: message = "{1} was hurt by the {2}!"; pokemon2Caps = true; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.ToxicOrb:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.ChangedStatus: message = "{0} was badly poisoned by its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(ip.Item));
                    }
                    return string.Format(message, GetPkmnName(itemHolder, itemHolderCaps), GetPkmnName(pokemon2, pokemon2Caps), PBELocalizedString.GetItemName(ip.Item).English);
                }
                case PBEItemTurnPacket itp:
                {
                    PBEBattlePokemon itemUser = itp.ItemUserTrainer.TryGetPokemon(itp.ItemUser);
                    string itemEnglish = PBELocalizedString.GetItemName(itp.Item).English;
                    switch (itp.ItemAction)
                    {
                        case PBEItemTurnAction.Attempt:
                        {
                            string word;
                            if (PBEDataUtils.AllBalls.Contains(itp.Item))
                            {
                                word = "threw";
                            }
                            else
                            {
                                word = "used";
                            }
                            return string.Format("{0} {1} the {2}.", GetTrainerName(itemUser.Trainer), word, itemEnglish);
                        }
                        case PBEItemTurnAction.NoEffect:
                        {
                            if (PBEDataUtils.AllBalls.Contains(itp.Item))
                            {
                                return "The trainer blocked the ball! Don't be a thief!";
                            }
                            return string.Format("The {0} had no effect.", itemEnglish);
                        }
                        case PBEItemTurnAction.Success:
                        {
                            string message;
                            switch (itp.Item)
                            {
                                // No "success" items yet
                                default: throw new ArgumentOutOfRangeException(nameof(itp.Item));
                            }
                            return string.Format(message, GetPkmnName(itemUser, true), itemEnglish);
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(itp.ItemAction));
                    }
                }
                case PBEMoveCritPacket mcp:
                {
                    PBEBattlePokemon victim = mcp.VictimTrainer.TryGetPokemon(mcp.Victim);
                    return string.Format("A critical hit on {0}!", GetPkmnName(victim, false));
                }
                case PBEMovePPChangedPacket mpcp:
                {
                    PBEBattlePokemon moveUser = mpcp.MoveUserTrainer.TryGetPokemon(mpcp.MoveUser);
                    return string.Format("{0}'s {1} {3} {2} PP!",
                        GetPkmnName(moveUser, true),
                        PBELocalizedString.GetMoveName(mpcp.Move).English,
                        Math.Abs(mpcp.AmountReduced),
                        mpcp.AmountReduced >= 0 ? "lost" : "gained");
                }
                case PBEMoveResultPacket mrp:
                {
                    PBEBattlePokemon moveUser = mrp.MoveUserTrainer.TryGetPokemon(mrp.MoveUser);
                    PBEBattlePokemon pokemon2 = mrp.Pokemon2Trainer.TryGetPokemon(mrp.Pokemon2);
                    bool pokemon2Caps = true;
                    string message;
                    switch (mrp.Result)
                    {
                        case PBEResult.Ineffective_Ability: message = "{1} is protected by its Ability!"; break;
                        case PBEResult.Ineffective_Gender: message = "It doesn't affect {1}..."; pokemon2Caps = false; break;
                        case PBEResult.Ineffective_Level: message = "{1} is protected by its level!"; break;
                        case PBEResult.Ineffective_MagnetRise: message = $"{{1}} is protected by {PBELocalizedString.GetMoveName(PBEMove.MagnetRise).English}!"; break;
                        case PBEResult.Ineffective_Safeguard: message = $"{{1}} is protected by {PBELocalizedString.GetMoveName(PBEMove.Safeguard).English}!"; break;
                        case PBEResult.Ineffective_Stat:
                        case PBEResult.Ineffective_Status:
                        case PBEResult.InvalidConditions: message = "But it failed!"; break;
                        case PBEResult.Ineffective_Substitute: message = $"{{1}} is protected by {PBELocalizedString.GetMoveName(PBEMove.Substitute).English}!"; break;
                        case PBEResult.Ineffective_Type: message = "{1} is protected by its Type!"; break;
                        case PBEResult.Missed: message = "{0}'s attack missed {1}!"; pokemon2Caps = false; break;
                        case PBEResult.NoTarget: message = "But there was no target..."; break;
                        case PBEResult.NotVeryEffective_Type: message = "It's not very effective on {1}..."; pokemon2Caps = false; break;
                        case PBEResult.SuperEffective_Type: message = "It's super effective on {1}!"; pokemon2Caps = false; break;
                        default: throw new ArgumentOutOfRangeException(nameof(mrp.Result));
                    }
                    return string.Format(message, GetPkmnName(moveUser, true), GetPkmnName(pokemon2, pokemon2Caps));
                }
                case PBEMoveUsedPacket mup:
                {
                    PBEBattlePokemon moveUser = mup.MoveUserTrainer.TryGetPokemon(mup.MoveUser);
                    return string.Format("{0} used {1}!", GetPkmnName(moveUser, true), PBELocalizedString.GetMoveName(mup.Move).English);
                }
                case PBEPkmnFaintedPacket pfp:
                {
                    PBEBattlePokemon disguisedAsPokemon = pfp.PokemonTrainer.TryGetPokemon(pfp.DisguisedAsPokemon);
                    return string.Format("{0} fainted!", GetPkmnName(disguisedAsPokemon, true));
                }
                case PBEPkmnFaintedPacket_Hidden pfph:
                {
                    PBEBattlePokemon pokemon = pfph.PokemonTrainer.TryGetPokemon(pfph.OldPosition);
                    return string.Format("{0} fainted!", GetPkmnName(pokemon, true));
                }
                case IPBEPkmnFormChangedPacket pfcp:
                {
                    PBEBattlePokemon pokemon = pfcp.PokemonTrainer.TryGetPokemon(pfcp.Pokemon);
                    return string.Format("{0}'s new form is {1}!", GetPkmnName(pokemon, true), PBELocalizedString.GetFormName(pokemon.Species, pfcp.NewForm).English);
                }
                case PBEPkmnHPChangedPacket phcp:
                {
                    PBEBattlePokemon pokemon = phcp.PokemonTrainer.TryGetPokemon(phcp.Pokemon);
                    double percentageChange = phcp.NewHPPercentage - phcp.OldHPPercentage;
                    double absPercentageChange = Math.Abs(percentageChange);
                    if (showRawHP || userTrainer == pokemon.Trainer) // Owner should see raw values
                    {
                        int change = phcp.NewHP - phcp.OldHP;
                        int absChange = Math.Abs(change);
                        return string.Format("{0} {1} {2} ({3:P2}) HP!", GetPkmnName(pokemon, true), change <= 0 ? "lost" : "restored", absChange, absPercentageChange);
                    }
                    return DoHiddenHP(pokemon, percentageChange, absPercentageChange);
                }
                case PBEPkmnHPChangedPacket_Hidden phcph:
                {
                    PBEBattlePokemon pokemon = phcph.PokemonTrainer.TryGetPokemon(phcph.Pokemon);
                    double percentageChange = phcph.NewHPPercentage - phcph.OldHPPercentage;
                    double absPercentageChange = Math.Abs(percentageChange);
                    return DoHiddenHP(pokemon, percentageChange, absPercentageChange);
                }
                case PBEPkmnStatChangedPacket pscp:
                {
                    PBEBattlePokemon pokemon = pscp.PokemonTrainer.TryGetPokemon(pscp.Pokemon);
                    string statName, message;
                    switch (pscp.Stat)
                    {
                        case PBEStat.Accuracy: statName = "Accuracy"; break;
                        case PBEStat.Attack: statName = "Attack"; break;
                        case PBEStat.Defense: statName = "Defense"; break;
                        case PBEStat.Evasion: statName = "Evasion"; break;
                        case PBEStat.SpAttack: statName = "Special Attack"; break;
                        case PBEStat.SpDefense: statName = "Special Defense"; break;
                        case PBEStat.Speed: statName = "Speed"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(pscp.Stat));
                    }
                    int change = pscp.NewValue - pscp.OldValue;
                    switch (change)
                    {
                        case -2: message = "harshly fell"; break;
                        case -1: message = "fell"; break;
                        case +1: message = "rose"; break;
                        case +2: message = "rose sharply"; break;
                        default:
                        {
                            if (change == 0 && pscp.NewValue == -battle.Settings.MaxStatChange)
                            {
                                message = "won't go lower";
                            }
                            else if (change == 0 && pscp.NewValue == battle.Settings.MaxStatChange)
                            {
                                message = "won't go higher";
                            }
                            else if (change <= -3)
                            {
                                message = "severely fell";
                            }
                            else if (change >= +3)
                            {
                                message = "rose drastically";
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException();
                            }
                            break;
                        }
                    }
                    return string.Format("{0}'s {1} {2}!", GetPkmnName(pokemon, true), statName, message);
                }
                case IPBEPkmnSwitchInPacket psip:
                {
                    if (!psip.Forced)
                    {
                        return string.Format("{1} sent out {0}!", psip.SwitchIns.Select(s => s.Nickname).ToArray().Andify(), GetTrainerName(psip.Trainer));
                    }
                    goto bottom;
                }
                case PBEPkmnSwitchOutPacket psop:
                {
                    if (!psop.Forced)
                    {
                        PBEBattlePokemon disguisedAsPokemon = psop.PokemonTrainer.TryGetPokemon(psop.DisguisedAsPokemon);
                        return string.Format("{1} withdrew {0}!", disguisedAsPokemon.KnownNickname, GetTrainerName(psop.PokemonTrainer));
                    }
                    goto bottom;
                }
                case PBEPkmnSwitchOutPacket_Hidden psoph:
                {
                    if (!psoph.Forced)
                    {
                        PBEBattlePokemon pokemon = psoph.PokemonTrainer.TryGetPokemon(psoph.OldPosition);
                        return string.Format("{1} withdrew {0}!", pokemon.KnownNickname, GetTrainerName(psoph.PokemonTrainer));
                    }
                    goto bottom;
                }
                case PBEPsychUpPacket pup:
                {
                    PBEBattlePokemon user = pup.UserTrainer.TryGetPokemon(pup.User);
                    PBEBattlePokemon target = pup.TargetTrainer.TryGetPokemon(pup.Target);
                    return string.Format("{0} copied {1}'s stat changes!", GetPkmnName(user, true), GetPkmnName(target, false));
                }
                case PBEReflectTypePacket rtp:
                {
                    PBEBattlePokemon user = rtp.UserTrainer.TryGetPokemon(rtp.User);
                    PBEBattlePokemon target = rtp.TargetTrainer.TryGetPokemon(rtp.Target);
                    string type1Str = PBELocalizedString.GetTypeName(rtp.Type1).English;
                    return string.Format("{0} copied {1}'s {2}",
                        GetPkmnName(user, true),
                        GetPkmnName(target, false),
                        rtp.Type2 == PBEType.None ? $"{type1Str} type!" : $"{type1Str} and {PBELocalizedString.GetTypeName(rtp.Type2).English} types!");
                }
                case PBEReflectTypePacket_Hidden rtph:
                {
                    PBEBattlePokemon user = rtph.UserTrainer.TryGetPokemon(rtph.User);
                    PBEBattlePokemon target = rtph.TargetTrainer.TryGetPokemon(rtph.Target);
                    return string.Format("{0} copied {1}'s types!", GetPkmnName(user, true), GetPkmnName(target, false));
                }
                case PBESpecialMessagePacket smp:
                {
                    string message;
                    switch (smp.Message)
                    {
                        case PBESpecialMessage.DraggedOut: message = string.Format("{0} was dragged out!", GetPkmnName(((PBETrainer)smp.Params[0]).TryGetPokemon((PBEFieldPosition)smp.Params[1]), true)); break;
                        case PBESpecialMessage.Endure: message = string.Format("{0} endured the hit!", GetPkmnName(((PBETrainer)smp.Params[0]).TryGetPokemon((PBEFieldPosition)smp.Params[1]), true)); break;
                        case PBESpecialMessage.HPDrained: message = string.Format("{0} had its energy drained!", GetPkmnName(((PBETrainer)smp.Params[0]).TryGetPokemon((PBEFieldPosition)smp.Params[1]), true)); break;
                        case PBESpecialMessage.Magnitude: message = string.Format("Magnitude {0}!", (byte)smp.Params[0]); break;
                        case PBESpecialMessage.MultiHit: message = string.Format("Hit {0} time(s)!", (byte)smp.Params[0]); break;
                        case PBESpecialMessage.NothingHappened: message = "But nothing happened!"; break;
                        case PBESpecialMessage.OneHitKnockout: message = "It's a one-hit KO!"; break;
                        case PBESpecialMessage.PainSplit: message = "The battlers shared their pain!"; break;
                        case PBESpecialMessage.PayDay: message = "Coins were scattered everywhere!"; break;
                        case PBESpecialMessage.Recoil: message = string.Format("{0} is damaged by recoil!", GetPkmnName(((PBETrainer)smp.Params[0]).TryGetPokemon((PBEFieldPosition)smp.Params[1]), true)); break;
                        case PBESpecialMessage.Struggle: message = string.Format("{0} has no moves left!", GetPkmnName(((PBETrainer)smp.Params[0]).TryGetPokemon((PBEFieldPosition)smp.Params[1]), true)); break;
                        default: throw new ArgumentOutOfRangeException(nameof(smp.Message));
                    }
                    return message;
                }
                case PBEStatus1Packet s1p:
                {
                    PBEBattlePokemon status1Receiver = s1p.Status1ReceiverTrainer.TryGetPokemon(s1p.Status1Receiver);
                    string message;
                    switch (s1p.Status1)
                    {
                        case PBEStatus1.Asleep:
                        {
                            switch (s1p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} fell asleep!"; break;
                                case PBEStatusAction.CausedImmobility: message = "{0} is fast asleep."; break;
                                case PBEStatusAction.Cleared:
                                case PBEStatusAction.Ended: message = "{0} woke up!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus1.BadlyPoisoned:
                        {
                            switch (s1p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} was badly poisoned!"; break;
                                case PBEStatusAction.Cleared: message = "{0} was cured of its poisoning."; break;
                                case PBEStatusAction.Damage: message = "{0} was hurt by poison!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus1.Burned:
                        {
                            switch (s1p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} was burned!"; break;
                                case PBEStatusAction.Cleared: message = "{0}'s burn was healed."; break;
                                case PBEStatusAction.Damage: message = "{0} was hurt by its burn!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus1.Frozen:
                        {
                            switch (s1p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} was frozen solid!"; break;
                                case PBEStatusAction.CausedImmobility: message = "{0} is frozen solid!"; break;
                                case PBEStatusAction.Cleared:
                                case PBEStatusAction.Ended: message = "{0} thawed out!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus1.Paralyzed:
                        {
                            switch (s1p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} is paralyzed! It may be unable to move!"; break;
                                case PBEStatusAction.CausedImmobility: message = "{0} is paralyzed! It can't move!"; break;
                                case PBEStatusAction.Cleared: message = "{0} was cured of paralysis."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus1.Poisoned:
                        {
                            switch (s1p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} was poisoned!"; break;
                                case PBEStatusAction.Cleared: message = "{0} was cured of its poisoning."; break;
                                case PBEStatusAction.Damage: message = "{0} was hurt by poison!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(s1p.Status1));
                    }
                    return string.Format(message, GetPkmnName(status1Receiver, true));
                }
                case PBEStatus2Packet s2p:
                {
                    PBEBattlePokemon status2Receiver = s2p.Status2ReceiverTrainer.TryGetPokemon(s2p.Status2Receiver);
                    PBEBattlePokemon pokemon2 = s2p.Pokemon2Trainer.TryGetPokemon(s2p.Pokemon2);
                    string message;
                    bool status2ReceiverCaps = true,
                            pokemon2Caps = false;
                    switch (s2p.Status2)
                    {
                        case PBEStatus2.Airborne:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} flew up high!"; break;
                                case PBEStatusAction.Ended: goto bottom;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Confused:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} became confused!"; break;
                                case PBEStatusAction.Announced: message = "{0} is confused!"; break;
                                case PBEStatusAction.Cleared:
                                case PBEStatusAction.Ended: message = "{0} snapped out of its confusion."; break;
                                case PBEStatusAction.Damage: message = "It hurt itself in its confusion!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Cursed:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{1} cut its own HP and laid a curse on {0}!"; status2ReceiverCaps = false; pokemon2Caps = true; break;
                                case PBEStatusAction.Damage: message = "{0} is afflicted by the curse!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Disguised:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Ended: message = "{0}'s illusion wore off!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Flinching:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.CausedImmobility: message = "{0} flinched and couldn't move!"; break;
                                case PBEStatusAction.Ended: goto bottom;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Identified:
                        case PBEStatus2.MiracleEye:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} was identified!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.HelpingHand:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{1} is ready to help {0}!"; status2ReceiverCaps = false; pokemon2Caps = true; break;
                                case PBEStatusAction.Ended: goto bottom;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Infatuated:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} fell in love with {1}!"; break;
                                case PBEStatusAction.Announced: message = "{0} is in love with {1}!"; break;
                                case PBEStatusAction.CausedImmobility: message = "{0} is immobilized by love!"; break;
                                case PBEStatusAction.Cleared:
                                case PBEStatusAction.Ended: message = "{0} got over its infatuation."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.LeechSeed:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} was seeded!"; break;
                                case PBEStatusAction.Damage: message = "{0}'s health is sapped by Leech Seed!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.LockOn:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} took aim at {1}!"; break;
                                case PBEStatusAction.Ended: goto bottom;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.MagnetRise:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} levitated with electromagnetism!"; break;
                                case PBEStatusAction.Ended: message = "{0}'s electromagnetism wore off!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Nightmare:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} began having a nightmare!"; break;
                                case PBEStatusAction.Damage: message = "{0} is locked in a nightmare!"; break;
                                case PBEStatusAction.Ended: goto bottom;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.PowerTrick:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} switched its Attack and Defense!"; break;
                                case PBEStatusAction.Ended: goto bottom;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Protected:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added:
                                case PBEStatusAction.Damage: message = "{0} protected itself!"; break;
                                case PBEStatusAction.Cleared: message = "{1} broke through {0}'s protection!"; status2ReceiverCaps = false; pokemon2Caps = true; break;
                                case PBEStatusAction.Ended: goto bottom;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Pumped:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} is getting pumped!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Roost:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added:
                                case PBEStatusAction.Ended: goto bottom;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                        }
                        case PBEStatus2.ShadowForce:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} vanished instantly!"; break;
                                case PBEStatusAction.Ended: goto bottom;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Substitute:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} put in a substitute!"; break;
                                case PBEStatusAction.Damage: message = "The substitute took damage for {0}!"; status2ReceiverCaps = false; break;
                                case PBEStatusAction.Ended: message = "{0}'s substitute faded!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Transformed:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} transformed into {1}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Underground:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} burrowed its way under the ground!"; break;
                                case PBEStatusAction.Ended: goto bottom;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Underwater:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} hid underwater!"; break;
                                case PBEStatusAction.Ended: goto bottom;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(s2p.Status2));
                    }
                    return string.Format(message, GetPkmnName(status2Receiver, status2ReceiverCaps), GetPkmnName(pokemon2, pokemon2Caps));
                }
                case PBETeamStatusPacket tsp:
                {
                    PBEBattlePokemon damageVictim = tsp.DamageVictimTrainer?.TryGetPokemon(tsp.DamageVictim);
                    string message;
                    bool teamCaps = true,
                        damageVictimCaps = false;
                    switch (tsp.TeamStatus)
                    {
                        case PBETeamStatus.LightScreen:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "Light Screen raised {0} team's Special Defense!"; teamCaps = false; break;
                                case PBETeamStatusAction.Cleared:
                                case PBETeamStatusAction.Ended: message = "{0} team's Light Screen wore off!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.LuckyChant:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "The Lucky Chant shielded {0} team from critical hits!"; teamCaps = false; break;
                                case PBETeamStatusAction.Ended: message = "{0} team's Lucky Chant wore off!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.QuickGuard:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "Quick Guard protected {0} team!"; teamCaps = false; break;
                                case PBETeamStatusAction.Cleared: message = "{0} team's Quick Guard was destroyed!"; break;
                                case PBETeamStatusAction.Damage: message = "Quick Guard protected {1}!"; break;
                                case PBETeamStatusAction.Ended: goto bottom;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.Reflect:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "Reflect raised {0} team's Defense!"; teamCaps = false; break;
                                case PBETeamStatusAction.Cleared:
                                case PBETeamStatusAction.Ended: message = "{0} team's Reflect wore off!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.Safeguard:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "{0} team became cloaked in a mystical veil!"; break;
                                case PBETeamStatusAction.Ended: message = "{0} team is no longer protected by Safeguard!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.Spikes:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "Spikes were scattered all around the feet of {0} team!"; teamCaps = false; break;
                                //case PBETeamStatusAction.Cleared: message = "The spikes disappeared from around {0} team's feet!"; teamCaps = false; break;
                                case PBETeamStatusAction.Damage: message = "{1} is hurt by the spikes!"; damageVictimCaps = true; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.StealthRock:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "Pointed stones float in the air around {0} team!"; teamCaps = false; break;
                                //case PBETeamStatusAction.Cleared: message = "The pointed stones disappeared from around {0} team!"; teamCaps = false; break;
                                case PBETeamStatusAction.Damage: message = "Pointed stones dug into {1}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.Tailwind:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "The tailwind blew from behind {0} team!"; teamCaps = false; break;
                                case PBETeamStatusAction.Ended: message = "{0} team's tailwind petered out!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.ToxicSpikes:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "Poison spikes were scattered all around {0} team's feet!"; break;
                                case PBETeamStatusAction.Cleared: message = "The poison spikes disappeared from around {0} team's feet!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.WideGuard:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "Wide Guard protected {0} team!"; break;
                                case PBETeamStatusAction.Cleared: message = "{0} team's Wide Guard was destroyed!"; break;
                                case PBETeamStatusAction.Damage: message = "Wide Guard protected {1}!"; break;
                                case PBETeamStatusAction.Ended: goto bottom;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatus));
                    }
                    return string.Format(message, GetTeamName(tsp.Team, teamCaps), GetPkmnName(damageVictim, damageVictimCaps));
                }
                case PBETypeChangedPacket tcp:
                {
                    PBEBattlePokemon pokemon = tcp.PokemonTrainer.TryGetPokemon(tcp.Pokemon);
                    string type1Str = PBELocalizedString.GetTypeName(tcp.Type1).English;
                    return string.Format("{0} transformed into the {1}",
                        GetPkmnName(pokemon, true),
                        tcp.Type2 == PBEType.None ? $"{type1Str} type!" : $"{type1Str} and {PBELocalizedString.GetTypeName(tcp.Type2).English} types!");
                }
                case PBEWeatherPacket wp:
                {
                    PBEBattlePokemon damageVictim = wp.DamageVictimTrainer?.TryGetPokemon(wp.DamageVictim);
                    string message;
                    switch (wp.Weather)
                    {
                        case PBEWeather.Hailstorm:
                        {
                            switch (wp.WeatherAction)
                            {
                                case PBEWeatherAction.Added: message = "It started to hail!"; break;
                                case PBEWeatherAction.CausedDamage: message = "{0} is buffeted by the hail!"; break;
                                case PBEWeatherAction.Ended: message = "The hail stopped."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction));
                            }
                            break;
                        }
                        case PBEWeather.HarshSunlight:
                        {
                            switch (wp.WeatherAction)
                            {
                                case PBEWeatherAction.Added: message = "The sunlight turned harsh!"; break;
                                case PBEWeatherAction.Ended: message = "The sunlight faded."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction));
                            }
                            break;
                        }
                        case PBEWeather.Rain:
                        {
                            switch (wp.WeatherAction)
                            {
                                case PBEWeatherAction.Added: message = "It started to rain!"; break;
                                case PBEWeatherAction.Ended: message = "The rain stopped."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction));
                            }
                            break;
                        }
                        case PBEWeather.Sandstorm:
                        {
                            switch (wp.WeatherAction)
                            {
                                case PBEWeatherAction.Added: message = "A sandstorm kicked up!"; break;
                                case PBEWeatherAction.CausedDamage: message = "{0} is buffeted by the sandstorm!"; break;
                                case PBEWeatherAction.Ended: message = "The sandstorm subsided."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction));
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(wp.Weather));
                    }
                    return string.Format(message, GetPkmnName(damageVictim, true));
                }
                case PBEWildPkmnAppearedPacket wpap:
                {
                    return string.Format("{0}{1} appeared!", wpap.Pokemon.Count == 1 ? "A wild " : "Oh! A wild ", wpap.Pokemon.Select(s => s.Nickname).ToArray().Andify());
                }
                case PBEActionsRequestPacket arp:
                {
                    return string.Format("{0} must submit actions for {1} Pokémon.", GetTrainerName(arp.Trainer), arp.Pokemon.Count);
                }
                case IPBEAutoCenterPacket _:
                {
                    return "The battlers shifted to the center!";
                }
                case PBEBattleResultPacket brp:
                {
                    string message;
                    switch (brp.BattleResult)
                    {
                        case PBEBattleResult.Team0Forfeit: message = "{0} forfeited."; break;
                        case PBEBattleResult.Team0Win: message = "{0} defeated {1}!"; break; // TODO: X defeated The wild Pokémon! (capital T in the)
                        case PBEBattleResult.Team1Forfeit: message = "{1} forfeited."; break;
                        case PBEBattleResult.Team1Win: message = "{1} defeated {0}!"; break;
                        case PBEBattleResult.WildCapture: goto bottom;
                        case PBEBattleResult.WildEscape: message = "{0} got away!"; break;
                        case PBEBattleResult.WildFlee: message = "{1} got away!"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(brp.BattleResult));
                    }
                    return string.Format(message, battle.Teams[0].CombinedName, battle.Teams[1].CombinedName);
                }
                case PBESwitchInRequestPacket sirp:
                {
                    return string.Format("{0} must send in {1} Pokémon.", GetTrainerName(sirp.Trainer), sirp.Amount);
                }
                case PBETurnBeganPacket tbp:
                {
                    return string.Format("Turn {0} is starting.", tbp.TurnNumber);
                }
            }
        bottom:
            return string.Empty;
        }

        /// <summary>Writes battle events to <see cref="Console.Out"/> in English.</summary>
        /// <param name="battle">The battle that <paramref name="packet"/> belongs to.</param>
        /// <param name="packet">The battle event packet.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="battle"/> or <paramref name="packet"/> are null.</exception>
        public static void ConsoleBattleEventHandler(PBEBattle battle, IPBEPacket packet)
        {
            if (battle is null)
            {
                throw new ArgumentNullException(nameof(battle));
            }
            if (packet is null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            string message = GetDefaultMessage(battle, packet, showRawHP: true);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            Console.WriteLine(message);
        }
    }
}
