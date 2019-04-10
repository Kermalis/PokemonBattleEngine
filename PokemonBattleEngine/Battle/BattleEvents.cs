using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBEBattle
    {
        public delegate void BattleEvent(PBEBattle battle, INetPacket packet);
        public event BattleEvent OnNewEvent;

        void BroadcastAbility(PBEPokemon abilityOwner, PBEPokemon pokemon2, PBEAbility ability, PBEAbilityAction abilityAction)
        {
            abilityOwner.Ability = abilityOwner.KnownAbility = ability;
            var p = new PBEAbilityPacket(abilityOwner, pokemon2, ability, abilityAction);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastBattleStatus(PBEBattleStatus battleStatus, PBEBattleStatusAction battleStatusAction)
        {
            var p = new PBEBattleStatusPacket(battleStatus, battleStatusAction);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastIllusion(PBEPokemon pokemon)
        {
            var p = new PBEIllusionPacket(pokemon);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastItem(PBEPokemon itemHolder, PBEPokemon pokemon2, PBEItem item, PBEItemAction itemAction)
        {
            switch (itemAction)
            {
                case PBEItemAction.Consumed:
                    {
                        itemHolder.Item = itemHolder.KnownItem = PBEItem.None;
                        break;
                    }
                default:
                    {
                        itemHolder.Item = itemHolder.KnownItem = item;
                        break;
                    }
            }
            var p = new PBEItemPacket(itemHolder, pokemon2, item, itemAction);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastMoveCrit()
        {
            var p = new PBEMoveCritPacket();
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastEffectiveness(PBEPokemon victim, PBEEffectiveness effectiveness)
        {
            var p = new PBEMoveEffectivenessPacket(victim, effectiveness);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastMoveFailed(PBEPokemon moveUser, PBEPokemon pokemon2, PBEFailReason failReason)
        {
            var p = new PBEMoveFailedPacket(moveUser, pokemon2, failReason);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastMoveLock(PBEPokemon moveUser, PBEMove lockedMove, PBETarget lockedTargets, PBEMoveLockType moveLockType)
        {
            var p = new PBEMoveLockPacket(moveUser, lockedMove, lockedTargets, moveLockType);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastMoveMissed(PBEPokemon moveUser, PBEPokemon pokemon2)
        {
            var p = new PBEMoveMissedPacket(moveUser, pokemon2);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastMovePPChanged(PBEPokemon moveUser, PBEMove move, byte oldValue, byte newValue)
        {
            var p = new PBEMovePPChangedPacket(moveUser.FieldPosition, moveUser.Team, move, oldValue, newValue);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastMoveUsed(PBEPokemon moveUser, PBEMove move)
        {
            if (!calledFromOtherMove && !moveUser.KnownMoves.Contains(move))
            {
                moveUser.KnownMoves[Array.IndexOf(moveUser.KnownMoves, PBEMove.MAX)] = move;
            }
            var p = new PBEMoveUsedPacket(moveUser, move, calledFromOtherMove);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastPkmnFainted(PBEPokemon pokemon, PBEFieldPosition oldPosition)
        {
            var p = new PBEPkmnFaintedPacket(pokemon.Id, oldPosition, pokemon.Team);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastPkmnFormChanged(PBEPokemon pokemon, PBESpecies newSpecies, PBEAbility newAbility, PBEAbility newKnownAbility)
        {
            pokemon.Ability = newAbility;
            pokemon.KnownAbility = newKnownAbility;
            pokemon.Species = pokemon.KnownSpecies = newSpecies;
            ushort oldMaxHP = pokemon.MaxHP; // Store in case someone edits a form's HP base stat
            pokemon.SetStats();
            pokemon.MaxHP = oldMaxHP;
            var pData = PBEPokemonData.GetData(newSpecies);
            pokemon.Type1 = pokemon.KnownType1 = pData.Type1;
            pokemon.Type2 = pokemon.KnownType2 = pData.Type2;
            pokemon.Weight = pokemon.KnownWeight = pData.Weight;
            var p = new PBEPkmnFormChangedPacket(pokemon.FieldPosition, pokemon.Team, pokemon.Attack, pokemon.Defense, pokemon.SpAttack, pokemon.SpDefense, pokemon.Speed, newAbility, newKnownAbility, newSpecies, pokemon.Type1, pokemon.Type2, pokemon.Weight);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastPkmnHPChanged(PBEPokemon pokemon, ushort oldHP, double oldHPPercentage)
        {
            var p = new PBEPkmnHPChangedPacket(pokemon.FieldPosition, pokemon.Team, oldHP, pokemon.HP, oldHPPercentage, pokemon.HPPercentage);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastPkmnStatChanged(PBEPokemon pokemon, PBEStat stat, sbyte oldValue, sbyte newValue)
        {
            var p = new PBEPkmnStatChangedPacket(pokemon, stat, oldValue, newValue);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastPkmnSwitchIn(PBETeam team, IEnumerable<PBEPkmnSwitchInPacket.PBESwitchInInfo> switchIns, bool forced)
        {
            var p = new PBEPkmnSwitchInPacket(team, switchIns, forced);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastPkmnSwitchOut(PBEPokemon pokemon, PBEFieldPosition oldPosition, bool forced)
        {
            var p = new PBEPkmnSwitchOutPacket(pokemon.Id, oldPosition, pokemon.Team, forced);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastPsychUp(PBEPokemon user, PBEPokemon target)
        {
            var p = new PBEPsychUpPacket(user, target);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }

        void BroadcastDraggedOut(PBEPokemon pokemon)
        {
            var p = new PBESpecialMessagePacket(PBESpecialMessage.DraggedOut, pokemon);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastEndure(PBEPokemon pokemon)
        {
            var p = new PBESpecialMessagePacket(PBESpecialMessage.Endure, pokemon);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastHPDrained(PBEPokemon pokemon)
        {
            var p = new PBESpecialMessagePacket(PBESpecialMessage.HPDrained, pokemon);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastMagnitude(byte magnitude)
        {
            var p = new PBESpecialMessagePacket(PBESpecialMessage.Magnitude, magnitude);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastOneHitKnockout()
        {
            var p = new PBESpecialMessagePacket(PBESpecialMessage.OneHitKnockout);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastPainSplit(PBEPokemon user, PBEPokemon target)
        {
            var p = new PBESpecialMessagePacket(PBESpecialMessage.PainSplit, user, target);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastRecoil(PBEPokemon pokemon)
        {
            var p = new PBESpecialMessagePacket(PBESpecialMessage.Recoil, pokemon);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastStruggle(PBEPokemon pokemon)
        {
            var p = new PBESpecialMessagePacket(PBESpecialMessage.Struggle, pokemon);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }

        void BroadcastStatus1(PBEPokemon status1Receiver, PBEPokemon pokemon2, PBEStatus1 status1, PBEStatusAction statusAction)
        {
            var p = new PBEStatus1Packet(status1Receiver, pokemon2, status1, statusAction);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastStatus2(PBEPokemon status2Receiver, PBEPokemon pokemon2, PBEStatus2 status2, PBEStatusAction statusAction)
        {
            var p = new PBEStatus2Packet(status2Receiver, pokemon2, status2, statusAction);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastTeamStatus(PBETeam team, PBETeamStatus teamStatus, PBETeamStatusAction teamStatusAction, PBEPokemon damageVictim = null)
        {
            var p = new PBETeamStatusPacket(team, teamStatus, teamStatusAction, damageVictim);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastTransform(PBEPokemon user, PBEPokemon target)
        {
            var p = new PBETransformPacket(user, target);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastWeather(PBEWeather weather, PBEWeatherAction weatherAction, PBEPokemon damageVictim = null)
        {
            var p = new PBEWeatherPacket(weather, weatherAction, damageVictim);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastActionsRequest(PBETeam team)
        {
            var p = new PBEActionsRequestPacket(team);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastAutoCenter(byte pokemon1Id, PBEFieldPosition pokemon1Position, PBETeam pokemon1Team, byte pokemon2Id, PBEFieldPosition pokemon2Position, PBETeam pokemon2Team)
        {
            var p = new PBEAutoCenterPacket(pokemon1Id, pokemon1Position, pokemon1Team, pokemon2Id, pokemon2Position, pokemon2Team);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastSwitchInRequest(PBETeam team)
        {
            var p = new PBESwitchInRequestPacket(team);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastTurnBegan()
        {
            var p = new PBETurnBeganPacket(TurnNumber);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }
        void BroadcastWinner(PBETeam winningTeam)
        {
            var p = new PBEWinnerPacket(winningTeam);
            Events.Add(p);
            OnNewEvent?.Invoke(this, p);
        }


        /// <summary>
        /// Writes battle events to <see cref="Console.Out"/> in English.
        /// </summary>
        /// <param name="battle">The battle that <paramref name="packet"/> belongs to.</param>
        /// <param name="packet">The battle event packet.</param>
        public static void ConsoleBattleEventHandler(PBEBattle battle, INetPacket packet)
        {
            string NameForTrainer(PBEPokemon pkmn)
            {
                return pkmn == null ? string.Empty : $"{pkmn.Team.TrainerName}'s {pkmn.KnownNickname}";
            }

            switch (packet)
            {
                case PBEAbilityPacket ap:
                    {
                        PBEPokemon abilityOwner = ap.AbilityOwnerTeam.TryGetPokemon(ap.AbilityOwner),
                            pokemon2 = ap.Pokemon2Team.TryGetPokemon(ap.Pokemon2);
                        string message;
                        switch (ap.Ability)
                        {
                            case PBEAbility.Drizzle:
                            case PBEAbility.Drought:
                            case PBEAbility.SandStream:
                            case PBEAbility.SnowWarning:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.Weather: message = "{0}'s {2} activated!"; break; // Message is displayed from a weather packet
                                        default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                                    }
                                    break;
                                }
                            case PBEAbility.EffectSpore:
                            case PBEAbility.FlameBody:
                            case PBEAbility.Healer:
                            case PBEAbility.PoisonPoint:
                            case PBEAbility.Static:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.ChangedStatus: message = "{0}'s {2} activated!"; break; // Message is displayed from a status1 packet
                                        default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                                    }
                                    break;
                                }
                            case PBEAbility.FlowerGift:
                            case PBEAbility.Forecast:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.ChangedAppearance: message = "{0}'s {2} activated!"; break; // Message is displayed from a form changed packet
                                        default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                                    }
                                    break;
                                }
                            case PBEAbility.IceBody:
                            case PBEAbility.RainDish:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.RestoredHP: message = "{0}'s {2} activated!"; break; // Message is displayed from a hp changed packet
                                        default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                                    }
                                    break;
                                }
                            case PBEAbility.Illusion:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.ChangedAppearance: message = "{0}'s illusion wore off!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                                    }
                                    break;
                                }
                            case PBEAbility.Imposter:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.ChangedAppearance: message = "{0}'s {2} activated!"; break; // Message is displayed from a status2 packet
                                        default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                                    }
                                    break;
                                }
                            case PBEAbility.IronBarbs:
                            case PBEAbility.RoughSkin:
                            case PBEAbility.SolarPower:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.Damage: message = "{0}'s {2} activated!"; break; // Message is displayed from a hp changed packet
                                        default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                                    }
                                    break;
                                }
                            case PBEAbility.Justified:
                            case PBEAbility.Rattled:
                            case PBEAbility.WeakArmor:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.Damage: message = "{0}'s {2} activated!"; break; // Message is displayed from a stat changed packet
                                        default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                                    }
                                    break;
                                }
                            case PBEAbility.Levitate:
                            case PBEAbility.WonderGuard:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.Damage: message = "{0}'s {2} activated!"; break; // Message is displayed from an effectiveness packet
                                        default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                                    }
                                    break;
                                }
                            case PBEAbility.Limber:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.ChangedStatus: // Message is displayed from a status1 packet
                                        case PBEAbilityAction.PreventedStatus: message = "{0}'s {2} activated!"; break; // Message is displayed from an effectiveness packet
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
                            case PBEAbility.Mummy:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.Changed: message = "{0}'s Ability became {2}!"; break;
                                        case PBEAbilityAction.Damage: message = "{0}'s {2} activated!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                                    }
                                    break;
                                }
                            case PBEAbility.None:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.Changed: message = "{0}'s Ability was suppressed!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                                    }
                                    break;
                                }
                            case PBEAbility.SpeedBoost:
                            case PBEAbility.Steadfast:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.ChangedStats: message = "{0}'s {2} activated!"; break; // Message is displayed from a stat changed packet
                                        default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                                    }
                                    break;
                                }
                            case PBEAbility.Sturdy:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.Damage: message = "{0}'s {2} activated!"; break; // Message is displayed from a special message packet
                                        default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                                    }
                                    break;
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(ap.Ability));
                        }
                        Console.WriteLine(message, NameForTrainer(abilityOwner), NameForTrainer(pokemon2), PBELocalizedString.GetAbilityName(ap.Ability).English);
                        break;
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
                        Console.WriteLine(message);
                        break;
                    }
                case PBEItemPacket ip:
                    {
                        PBEPokemon itemHolder = ip.ItemHolderTeam.TryGetPokemon(ip.ItemHolder),
                            pokemon2 = ip.Pokemon2Team.TryGetPokemon(ip.Pokemon2);
                        string message;
                        switch (ip.Item)
                        {
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
                                        case PBEItemAction.Consumed: message = "The {2} strengthened {0}'s power!"; break;
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
                                        case PBEItemAction.Damage: message = "{1} was hurt by the {2}!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                                    }
                                    break;
                                }
                            case PBEItem.SitrusBerry:
                                {
                                    switch (ip.ItemAction)
                                    {
                                        case PBEItemAction.Consumed: message = "{0} restored its health using its {2}!"; break;
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
                        Console.WriteLine(message, NameForTrainer(itemHolder), NameForTrainer(pokemon2), PBELocalizedString.GetItemName(ip.Item).English);
                        break;
                    }
                case PBEMoveCritPacket _:
                    {
                        Console.WriteLine("A critical hit!");
                        break;
                    }
                case PBEMoveEffectivenessPacket mep:
                    {
                        PBEPokemon victim = mep.VictimTeam.TryGetPokemon(mep.Victim);
                        string message;
                        switch (mep.Effectiveness)
                        {
                            case PBEEffectiveness.Ineffective: message = "It doesn't affect {0}..."; break;
                            case PBEEffectiveness.NotVeryEffective: message = "It's not very effective..."; break;
                            case PBEEffectiveness.Normal: message = "It's normally effective."; break;
                            case PBEEffectiveness.SuperEffective: message = "It's super effective!"; break;
                            default: throw new ArgumentOutOfRangeException(nameof(mep.Effectiveness));
                        }
                        Console.WriteLine(message, NameForTrainer(victim));
                        break;
                    }
                case PBEMoveFailedPacket mfp:
                    {
                        PBEPokemon moveUser = mfp.MoveUserTeam.TryGetPokemon(mfp.MoveUser),
                            pokemon2 = mfp.Pokemon2Team.TryGetPokemon(mfp.Pokemon2);
                        string message;
                        switch (mfp.FailReason)
                        {
                            case PBEFailReason.AlreadyAsleep: message = "{1} is already asleep!"; break;
                            case PBEFailReason.AlreadyBurned: message = "{1} already has a burn."; break;
                            case PBEFailReason.AlreadyConfused: message = "{1} is already confused!"; break;
                            case PBEFailReason.AlreadyParalyzed: message = "{1} is already paralyzed!"; break;
                            case PBEFailReason.AlreadyPoisoned: message = "{1} is already poisoned."; break;
                            case PBEFailReason.AlreadySubstituted: message = "{1} already has a substitute!"; break;
                            case PBEFailReason.Default: message = "But it failed!"; break;
                            case PBEFailReason.HPFull: message = "{1}'s HP is full!"; break;
                            case PBEFailReason.NoTarget: message = "There was no target..."; break;
                            case PBEFailReason.OneHitKnockoutUnaffected: message = "{1} is unaffected!"; break;
                            default: throw new ArgumentOutOfRangeException(nameof(mfp.FailReason));
                        }
                        Console.WriteLine(message, NameForTrainer(moveUser), NameForTrainer(pokemon2));
                        break;
                    }
                case PBEMoveMissedPacket mmp:
                    {
                        PBEPokemon moveUser = mmp.MoveUserTeam.TryGetPokemon(mmp.MoveUser),
                            pokemon2 = mmp.Pokemon2Team.TryGetPokemon(mmp.Pokemon2);
                        Console.WriteLine("{0}'s attack missed {1}!", NameForTrainer(moveUser), NameForTrainer(pokemon2));
                        break;
                    }
                case PBEMovePPChangedPacket mpcp:
                    {
                        PBEPokemon moveUser = mpcp.MoveUserTeam.TryGetPokemon(mpcp.MoveUser);
                        int change = mpcp.NewValue - mpcp.OldValue;
                        Console.WriteLine("{0}'s {1} {3} {2} PP!", NameForTrainer(moveUser), PBELocalizedString.GetMoveName(mpcp.Move).English, Math.Abs(change), change <= 0 ? "lost" : "gained");
                        break;
                    }
                case PBEMoveUsedPacket mup:
                    {
                        PBEPokemon moveUser = mup.MoveUserTeam.TryGetPokemon(mup.MoveUser);
                        Console.WriteLine("{0} used {1}!", NameForTrainer(moveUser), PBELocalizedString.GetMoveName(mup.Move).English);
                        break;
                    }
                case PBEPkmnFaintedPacket pfap:
                    {
                        PBEPokemon pokemon;
                        if (pfap.PokemonId == byte.MaxValue)
                        {
                            pokemon = pfap.PokemonTeam.TryGetPokemon(pfap.PokemonPosition);
                        }
                        else
                        {
                            pokemon = pfap.PokemonTeam.TryGetPokemon(pfap.PokemonId);
                        }
                        Console.WriteLine("{0} fainted!", NameForTrainer(pokemon));
                        break;
                    }
                case PBEPkmnFormChangedPacket pfcp:
                    {
                        PBEPokemon pokemon = pfcp.PokemonTeam.TryGetPokemon(pfcp.Pokemon);
                        Console.WriteLine("{0} transformed!", NameForTrainer(pokemon));
                        break;
                    }
                case PBEPkmnHPChangedPacket phcp:
                    {
                        PBEPokemon pokemon = phcp.PokemonTeam.TryGetPokemon(phcp.Pokemon);
                        int change = phcp.NewHP - phcp.OldHP;
                        int absChange = Math.Abs(change);
                        double percentageChange = phcp.NewHPPercentage - phcp.OldHPPercentage;
                        double absPercentageChange = Math.Abs(percentageChange);
                        if (pokemon.HP == 0 && pokemon.MaxHP == 0)
                        {
                            Console.WriteLine("{0} {1} {2:P2} of its HP!", NameForTrainer(pokemon), percentageChange <= 0 ? "lost" : "restored", absPercentageChange);
                        }
                        else
                        {
                            Console.WriteLine("{0} {1} {2} ({3:P2}) HP!", NameForTrainer(pokemon), change <= 0 ? "lost" : "restored", absChange, absPercentageChange);
                        }
                        break;
                    }
                case PBEPkmnStatChangedPacket pscp:
                    {
                        PBEPokemon pokemon = pscp.PokemonTeam.TryGetPokemon(pscp.Pokemon);
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
                        Console.WriteLine("{0}'s {1} {2}!", NameForTrainer(pokemon), statName, message);
                        break;
                    }
                case PBEPkmnSwitchInPacket psip:
                    {
                        if (!psip.Forced)
                        {
                            Console.WriteLine("{1} sent out {0}!", psip.SwitchIns.Select(s => s.Nickname).Andify(), psip.Team.TrainerName);
                        }
                        break;
                    }
                case PBEPkmnSwitchOutPacket psop:
                    {
                        if (!psop.Forced)
                        {
                            PBEPokemon pokemon;
                            if (psop.PokemonId == byte.MaxValue)
                            {
                                pokemon = psop.PokemonTeam.TryGetPokemon(psop.PokemonPosition);
                            }
                            else
                            {
                                pokemon = psop.PokemonTeam.TryGetPokemon(psop.PokemonId);
                            }
                            Console.WriteLine("{1} withdrew {0}!", pokemon.KnownNickname, pokemon.Team.TrainerName);
                        }
                        break;
                    }
                case PBEPsychUpPacket pup:
                    {
                        PBEPokemon user = pup.UserTeam.TryGetPokemon(pup.User),
                            target = pup.TargetTeam.TryGetPokemon(pup.Target);
                        Console.WriteLine("{0} copied {1}'s stat changes!", NameForTrainer(user), NameForTrainer(target));
                        break;
                    }
                case PBESpecialMessagePacket smp:
                    {
                        string message;
                        switch (smp.Message)
                        {
                            case PBESpecialMessage.DraggedOut:
                                {
                                    message = string.Format("{0} was dragged out!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0])));
                                    break;
                                }
                            case PBESpecialMessage.Endure:
                                {
                                    message = string.Format("{0} endured the hit!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0])));
                                    break;
                                }
                            case PBESpecialMessage.HPDrained:
                                {
                                    message = string.Format("{0} had its energy drained!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0])));
                                    break;
                                }
                            case PBESpecialMessage.Magnitude:
                                {
                                    message = string.Format("Magnitude {0}!", (byte)smp.Params[0]);
                                    break;
                                }
                            case PBESpecialMessage.OneHitKnockout:
                                {
                                    message = "It's a one-hit KO!";
                                    break;
                                }
                            case PBESpecialMessage.PainSplit:
                                {
                                    message = string.Format("{0} and {1} shared pain!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0])), NameForTrainer(((PBETeam)smp.Params[3]).TryGetPokemon((PBEFieldPosition)smp.Params[2])));
                                    break;
                                }
                            case PBESpecialMessage.Recoil:
                                {
                                    message = string.Format("{0} is damaged by recoil!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0])));
                                    break;
                                }
                            case PBESpecialMessage.Struggle:
                                {
                                    message = string.Format("{0} has no moves left!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0])));
                                    break;
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(smp.Message));
                        }
                        Console.WriteLine(message);
                        break;
                    }
                case PBEStatus1Packet s1p:
                    {
                        PBEPokemon status1Receiver = s1p.Status1ReceiverTeam.TryGetPokemon(s1p.Status1Receiver);
                        string message;
                        switch (s1p.Status1)
                        {
                            case PBEStatus1.Asleep:
                                {
                                    switch (s1p.StatusAction)
                                    {
                                        case PBEStatusAction.Added: message = "{0} fell asleep!"; break;
                                        case PBEStatusAction.CausedImmobility: message = "{0} is fast asleep."; break;
                                        case PBEStatusAction.Cured:
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
                                        case PBEStatusAction.Cured: message = "{0} was cured of its poisoning."; break;
                                        case PBEStatusAction.Damage: message = "{0} was hurt by poison!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                                    }
                                    break;
                                }
                            case PBEStatus1.Poisoned:
                                {
                                    switch (s1p.StatusAction)
                                    {
                                        case PBEStatusAction.Added: message = "{0} was poisoned!"; break;
                                        case PBEStatusAction.Cured: message = "{0} was cured of its poisoning."; break;
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
                                        case PBEStatusAction.Cured: message = "{0}'s burn was healed."; break;
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
                                        case PBEStatusAction.Cured:
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
                                        case PBEStatusAction.Cured: message = "{0} was cured of paralysis."; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                                    }
                                    break;
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(s1p.Status1));
                        }
                        Console.WriteLine(message, NameForTrainer(status1Receiver));
                        break;
                    }
                case PBEStatus2Packet s2p:
                    {
                        PBEPokemon status2Receiver = s2p.Status2ReceiverTeam.TryGetPokemon(s2p.Status2Receiver),
                            pokemon2 = s2p.Pokemon2Team.TryGetPokemon(s2p.Pokemon2);
                        string message;
                        switch (s2p.Status2)
                        {
                            case PBEStatus2.Airborne:
                                {
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.Added: message = "{0} flew up high!"; break;
                                        case PBEStatusAction.Ended: return;
                                        default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                                    }
                                    break;
                                }
                            case PBEStatus2.Confused:
                                {
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.Activated: message = "{0} is confused!"; break;
                                        case PBEStatusAction.Added: message = "{0} became confused!"; break;
                                        case PBEStatusAction.Damage: message = "It hurt itself in its confusion!"; break;
                                        case PBEStatusAction.Ended: message = "{0} snapped out of its confusion."; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                                    }
                                    break;
                                }
                            case PBEStatus2.Cursed:
                                {
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.Added: message = "{1} cut its own HP and laid a curse on {0}!"; break;
                                        case PBEStatusAction.Damage: message = "{0} is afflicted by the curse!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                                    }
                                    break;
                                }
                            case PBEStatus2.Flinching:
                                {
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.CausedImmobility: message = "{0} flinched and couldn't move!"; break;
                                        case PBEStatusAction.Ended: return;
                                        default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                                    }
                                    break;
                                }
                            case PBEStatus2.HelpingHand:
                                {
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.Added: message = "{1} is ready to help {0}!"; break;
                                        case PBEStatusAction.Ended: return;
                                        default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                                    }
                                    break;
                                }
                            case PBEStatus2.Infatuated:
                                {
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.Added: message = "{0} fell in love with {1}!"; break;
                                        case PBEStatusAction.Activated: message = "{0} is in love with {1}!"; break;
                                        case PBEStatusAction.CausedImmobility: message = "{0} is immobilized by love!"; break;
                                        case PBEStatusAction.Ended: return;
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
                            case PBEStatus2.Protected:
                                {
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.Activated:
                                        case PBEStatusAction.Added: message = "{0} protected itself!"; break;
                                        case PBEStatusAction.Ended: return;
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
                            case PBEStatus2.Substitute:
                                {
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.Added: message = "{0} put in a substitute!"; break;
                                        case PBEStatusAction.Damage: message = "The substitute took damage for {0}!"; break;
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
                                        case PBEStatusAction.Ended: return;
                                        default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                                    }
                                    break;
                                }
                            case PBEStatus2.Underwater:
                                {
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.Added: message = "{0} hid underwater!"; break;
                                        case PBEStatusAction.Ended: return;
                                        default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                                    }
                                    break;
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(s2p.Status2));
                        }
                        Console.WriteLine(message, NameForTrainer(status2Receiver), NameForTrainer(pokemon2));
                        break;
                    }
                case PBETeamStatusPacket tsp:
                    {
                        PBEPokemon damageVictim = tsp.Team.TryGetPokemon(tsp.DamageVictim);
                        string message;
                        switch (tsp.TeamStatus)
                        {
                            case PBETeamStatus.LightScreen:
                                {
                                    switch (tsp.TeamStatusAction)
                                    {
                                        case PBETeamStatusAction.Added: message = "Light Screen raised {0}'s team's Special Defense!"; break;
                                        case PBETeamStatusAction.Cleared:
                                        case PBETeamStatusAction.Ended: message = "{0}'s team's Light Screen wore off!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                                    }
                                    break;
                                }
                            case PBETeamStatus.LuckyChant:
                                {
                                    switch (tsp.TeamStatusAction)
                                    {
                                        case PBETeamStatusAction.Added: message = "The Lucky Chant shielded {0}'s team from critical hits!"; break;
                                        case PBETeamStatusAction.Ended: message = "{0}'s team's Lucky Chant wore off!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                                    }
                                    break;
                                }
                            case PBETeamStatus.Reflect:
                                {
                                    switch (tsp.TeamStatusAction)
                                    {
                                        case PBETeamStatusAction.Added: message = "Reflect raised {0}'s team's Defense!"; break;
                                        case PBETeamStatusAction.Cleared:
                                        case PBETeamStatusAction.Ended: message = "{0}'s team's Reflect wore off!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                                    }
                                    break;
                                }
                            case PBETeamStatus.Spikes:
                                {
                                    switch (tsp.TeamStatusAction)
                                    {
                                        case PBETeamStatusAction.Added: message = "Spikes were scattered all around the feet of {0}'s team!"; break;
                                        case PBETeamStatusAction.Cleared: message = "The spikes disappeared from around {0}'s team's feet!"; break;
                                        case PBETeamStatusAction.Damage: message = "{1} is hurt by the spikes!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                                    }
                                    break;
                                }
                            case PBETeamStatus.StealthRock:
                                {
                                    switch (tsp.TeamStatusAction)
                                    {
                                        case PBETeamStatusAction.Added: message = "Pointed stones float in the air around {0}'s team!"; break;
                                        case PBETeamStatusAction.Cleared: message = "The pointed stones disappeared from around {0}'s team!"; break;
                                        case PBETeamStatusAction.Damage: message = "Pointed stones dug into {1}!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                                    }
                                    break;
                                }
                            case PBETeamStatus.ToxicSpikes:
                                {
                                    switch (tsp.TeamStatusAction)
                                    {
                                        case PBETeamStatusAction.Added: message = "Poison spikes were scattered all around {0}'s team's feet!"; break;
                                        case PBETeamStatusAction.Cleared: message = "The poison spikes disappeared from around {0}'s team's feet!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                                    }
                                    break;
                                }
                            case PBETeamStatus.WideGuard:
                                {
                                    switch (tsp.TeamStatusAction)
                                    {
                                        case PBETeamStatusAction.Added: message = "Wide Guard protected {0}'s team!"; break;
                                        case PBETeamStatusAction.Damage: message = "Wide Guard protected {1}!"; break;
                                        case PBETeamStatusAction.Ended: return;
                                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                                    }
                                    break;
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatus));
                        }
                        Console.WriteLine(message, tsp.Team.TrainerName, NameForTrainer(damageVictim));
                        break;
                    }
                case PBEWeatherPacket wp:
                    {
                        PBEPokemon damageVictim = wp.DamageVictimTeam?.TryGetPokemon(wp.DamageVictim);
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
                        Console.WriteLine(message, NameForTrainer(damageVictim));
                        break;
                    }
                case PBEActionsRequestPacket arp:
                    {
                        Console.WriteLine("{0} must submit actions for {1} Pokémon.", arp.Team.TrainerName, arp.Pokemon.Count);
                        break;
                    }
                case PBEAutoCenterPacket _:
                    {
                        Console.WriteLine("The battlers shifted to the center!");
                        break;
                    }
                case PBESwitchInRequestPacket sirp:
                    {
                        Console.WriteLine("{0} must send in {1} Pokémon.", sirp.Team.TrainerName, sirp.Amount);
                        break;
                    }
                case PBETurnBeganPacket tbp:
                    {
                        Console.WriteLine("Turn {0} is starting.", tbp.TurnNumber);
                        break;
                    }
                case PBEWinnerPacket win:
                    {
                        Console.WriteLine("{0} defeated {1}!", win.WinningTeam.TrainerName, (win.WinningTeam == battle.Teams[0] ? battle.Teams[1] : battle.Teams[0]).TrainerName);
                        break;
                    }
            }
        }
    }
}
