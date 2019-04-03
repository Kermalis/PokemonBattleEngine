using Ether.Network.Client;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngineMobile.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Timers;

namespace Kermalis.PokemonBattleEngineMobile
{
    public class MobileBattleClient : NetClient
    {
        readonly PBEPacketProcessor packetProcessor;
        public override IPacketProcessor PacketProcessor => packetProcessor;

        public PBEBattle Battle { get; set; }
        public BattleView BattleView { get; set; }
        public int BattleId { get; private set; } = int.MaxValue;

        public MobileBattleClient(string host, int port, PBEBattleFormat battleFormat, PBESettings settings)
        {
            Configuration.Host = host;
            Configuration.Port = port;
            Configuration.BufferSize = 1024;

            Battle = new PBEBattle(battleFormat, settings);
            packetProcessor = new PBEPacketProcessor(Battle);

            packetTimer.Elapsed += PacketTimer_Elapsed;
            packetTimer.Start();
        }

        int currentPacket = -1;
        Timer packetTimer = new Timer(2000);
        public override void HandleMessage(INetPacket packet)
        {
            Debug.WriteLine($"Message received: \"{packet.GetType().Name}\"");
            switch (packet)
            {
                case PBEPlayerJoinedPacket pjp:
                    {
                        if (pjp.IsMe)
                        {
                            BattleId = pjp.BattleId;
                        }
                        else
                        {
                            BattleView.AddMessage(string.Format("{0} joined the game.", pjp.TrainerName), false, true);
                        }
                        if (pjp.BattleId < 2)
                        {
                            Battle.Teams[pjp.BattleId].TrainerName = pjp.TrainerName;
                        }
                        Send(new PBEResponsePacket());
                        break;
                    }
                case PBEPartyRequestPacket _:
                    {
                        //PBEPokemonShell[] team = PBECompetitivePokemonShells.CreateRandomTeam(Battle.Settings.MaxPartySize).ToArray();
                        PBEPokemonShell[] team = PBEUtils.CreateCompletelyRandomTeam(Battle.Settings);
                        Send(new PBEPartyResponsePacket(team));
                        break;
                    }
                case PBESetPartyPacket spp:
                    {
                        // Pokémon are set via internal methods in the packet
                        Send(new PBEResponsePacket());
                        break;
                    }
                default:
                    {
                        Battle.Events.Add(packet);
                        Send(new PBEResponsePacket());
                        break;
                    }
            }
        }
        void PacketTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool runImmediately;
            do
            {
                runImmediately = false;
                if (currentPacket != Battle.Events.Count - 1)
                {
                    currentPacket++;
                    runImmediately = ProcessPacket(Battle.Events[currentPacket]);
                }
            } while (runImmediately);
        }
        // Returns true if the next packet should be run immediately
        bool ProcessPacket(INetPacket packet)
        {
            string NameForTrainer(PBEPokemon pkmn, bool firstLetterCapitalized)
            {
                if (pkmn == null)
                {
                    return string.Empty;
                }
                if (BattleId >= 2)
                {
                    return $"{pkmn.Team.TrainerName}'s {pkmn.KnownNickname}";
                }
                else
                {
                    string prefix;
                    if (firstLetterCapitalized)
                    {
                        if (pkmn.Team.Id == BattleId)
                        {
                            prefix = string.Empty;
                        }
                        else
                        {
                            prefix = "The foe's ";
                        }
                    }
                    else
                    {
                        if (pkmn.Team.Id == BattleId)
                        {
                            prefix = string.Empty;
                        }
                        else
                        {
                            prefix = "the foe's ";
                        }
                    }
                    return prefix + pkmn.KnownNickname;
                }
            }

            switch (packet)
            {
                case PBEAbilityPacket ap:
                    {
                        PBEPokemon abilityOwner = ap.AbilityOwnerTeam.TryGetPokemon(ap.AbilityOwner),
                            pokemon2 = ap.Pokemon2Team.TryGetPokemon(ap.Pokemon2);
                        abilityOwner.Ability = abilityOwner.KnownAbility = ap.Ability;
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
                            case PBEAbility.Healer:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.CuredStatus: message = "{0}'s {2} activated!"; break; // Message is displayed from a status1 packet
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
                                        case PBEAbilityAction.CuredStatus: // Message is displayed from a status1 packet
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
                        BattleView.AddMessage(string.Format(message, NameForTrainer(abilityOwner, true), NameForTrainer(pokemon2, true), PBELocalizedString.GetAbilityName(ap.Ability).FromUICultureInfo()), true, true);
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
                        BattleView.AddMessage(message, true, true);
                        break;
                    }
                case PBEIllusionPacket ilp:
                    {
                        PBEPokemon pokemon = ilp.PokemonTeam.TryGetPokemon(ilp.Pokemon);
                        pokemon.Status2 &= ~PBEStatus2.Disguised;
                        pokemon.DisguisedAsPokemon = null;
                        pokemon.KnownAbility = PBEAbility.Illusion;
                        pokemon.KnownGender = ilp.ActualGender;
                        pokemon.KnownNickname = ilp.ActualNickname;
                        pokemon.KnownShiny = ilp.ActualShiny;
                        pokemon.KnownSpecies = ilp.ActualSpecies;
                        pokemon.KnownType1 = ilp.ActualType1;
                        pokemon.KnownType2 = ilp.ActualType2;
                        pokemon.KnownWeight = ilp.ActualWeight;
                        BattleView.Field.UpdatePokemon(pokemon);
                        break;
                    }
                case PBEItemPacket ip:
                    {
                        PBEPokemon itemHolder = ip.ItemHolderTeam.TryGetPokemon(ip.ItemHolder),
                            pokemon2 = ip.Pokemon2Team.TryGetPokemon(ip.Pokemon2);
                        switch (ip.ItemAction)
                        {
                            case PBEItemAction.Consumed: itemHolder.Item = itemHolder.KnownItem = PBEItem.None; break;
                            default: itemHolder.Item = itemHolder.KnownItem = ip.Item; break;
                        }
                        bool itemHolderCaps = true,
                            pokemon2Caps = false;
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
                        BattleView.AddMessage(string.Format(message, NameForTrainer(itemHolder, itemHolderCaps), NameForTrainer(pokemon2, pokemon2Caps), PBELocalizedString.GetItemName(ip.Item).FromUICultureInfo()), true, true);
                        break;
                    }
                case PBEMoveCritPacket _:
                    {
                        BattleView.AddMessage("A critical hit!", true, true);
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
                            case PBEEffectiveness.Normal: return true;
                            case PBEEffectiveness.SuperEffective: message = "It's super effective!"; break;
                            default: throw new ArgumentOutOfRangeException(nameof(mep.Effectiveness));
                        }
                        BattleView.AddMessage(string.Format(message, NameForTrainer(victim, false)), true, true);
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
                        BattleView.AddMessage(string.Format(message, NameForTrainer(moveUser, true), NameForTrainer(pokemon2, true)), true, true);
                        break;
                    }
                case PBEMoveLockPacket mlp:
                    {
                        PBEPokemon moveUser = mlp.MoveUserTeam.TryGetPokemon(mlp.MoveUser);
                        switch (mlp.MoveLockType)
                        {
                            case PBEMoveLockType.ChoiceItem:
                                {
                                    moveUser.ChoiceLockedMove = mlp.LockedMove;
                                    break;
                                }
                            case PBEMoveLockType.Temporary:
                                {
                                    moveUser.TempLockedMove = mlp.LockedMove;
                                    moveUser.TempLockedTargets = mlp.LockedTargets;
                                    break;
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(mlp.MoveLockType));
                        }
                        return true;
                    }
                case PBEMoveMissedPacket mmp:
                    {
                        PBEPokemon moveUser = mmp.MoveUserTeam.TryGetPokemon(mmp.MoveUser),
                            pokemon2 = mmp.Pokemon2Team.TryGetPokemon(mmp.Pokemon2);
                        BattleView.AddMessage(string.Format("{0}'s attack missed {1}!", NameForTrainer(moveUser, true), NameForTrainer(pokemon2, false)), true, true);
                        break;
                    }
                case PBEMovePPChangedPacket mpcp:
                    {
                        PBEPokemon moveUser = mpcp.MoveUserTeam.TryGetPokemon(mpcp.MoveUser);
                        moveUser.PP[Array.IndexOf(moveUser.Moves, mpcp.Move)] = mpcp.NewValue;
                        return true;
                    }
                case PBEMoveUsedPacket mup:
                    {
                        PBEPokemon moveUser = mup.MoveUserTeam.TryGetPokemon(mup.MoveUser);
                        if (BattleId != int.MaxValue && moveUser.Team.Id != BattleId && !mup.CalledFromOtherMove && !moveUser.KnownMoves.Contains(mup.Move))
                        {
                            moveUser.KnownMoves[Array.IndexOf(moveUser.KnownMoves, PBEMove.MAX)] = mup.Move;
                        }
                        BattleView.AddMessage(string.Format("{0} used {1}!", NameForTrainer(moveUser, true), PBELocalizedString.GetMoveName(mup.Move).FromUICultureInfo()), true, true);
                        break;
                    }
                case PBEPkmnFaintedPacket pfap:
                    {
                        PBEPokemon pokemon = pfap.PokemonTeam.TryGetPokemon(pfap.PokemonPosition);
                        pokemon.FieldPosition = PBEFieldPosition.None;
                        if (pokemon.Id == byte.MaxValue)
                        {
                            pokemon.Team.Party.Remove(pokemon);
                        }
                        Battle.ActiveBattlers.Remove(pokemon);
                        BattleView.Field.UpdatePokemon(pokemon, pfap.PokemonPosition);
                        BattleView.AddMessage(string.Format("{0} fainted!", NameForTrainer(pokemon, true)), true, true);
                        break;
                    }
                case PBEPkmnFormChangedPacket pfcp:
                    {
                        PBEPokemon pokemon = pfcp.PokemonTeam.TryGetPokemon(pfcp.Pokemon);
                        pokemon.Species = pokemon.KnownSpecies = pfcp.NewSpecies;
                        var pData = PBEPokemonData.GetData(pfcp.NewSpecies);
                        pokemon.Type1 = pokemon.KnownType1 = pData.Type1;
                        pokemon.Type2 = pokemon.KnownType2 = pData.Type2;
                        BattleView.Field.UpdatePokemon(pokemon);
                        BattleView.AddMessage(string.Format("{0} transformed!", NameForTrainer(pokemon, true)), true, true);
                        break;
                    }
                case PBEPkmnHPChangedPacket phcp:
                    {
                        PBEPokemon pokemon = phcp.PokemonTeam.TryGetPokemon(phcp.Pokemon);
                        pokemon.HP = phcp.NewHP;
                        pokemon.HPPercentage = phcp.NewHPPercentage;
                        BattleView.Field.UpdatePokemon(pokemon);
                        int change = phcp.NewHP - phcp.OldHP;
                        int absChange = Math.Abs(change);
                        double percentageChange = phcp.NewHPPercentage - phcp.OldHPPercentage;
                        double absPercentageChange = Math.Abs(percentageChange);
                        if (pokemon.Id == byte.MaxValue)
                        {
                            BattleView.AddMessage(string.Format("{0} {1} {2:P2} of its HP!", NameForTrainer(pokemon, true), percentageChange <= 0 ? "lost" : "restored", absPercentageChange), true, true);
                        }
                        else
                        {
                            BattleView.AddMessage(string.Format("{0} {1} {2} ({3:P2}) HP!", NameForTrainer(pokemon, true), change <= 0 ? "lost" : "restored", absChange, absPercentageChange), true, true);
                        }
                        break;
                    }
                case PBEPkmnStatChangedPacket pscp:
                    {
                        PBEPokemon pokemon = pscp.PokemonTeam.TryGetPokemon(pscp.Pokemon);
                        string statName, message;
                        switch (pscp.Stat)
                        {
                            case PBEStat.Accuracy: statName = "Accuracy"; pokemon.AccuracyChange = pscp.NewValue; break;
                            case PBEStat.Attack: statName = "Attack"; pokemon.AttackChange = pscp.NewValue; break;
                            case PBEStat.Defense: statName = "Defense"; pokemon.DefenseChange = pscp.NewValue; break;
                            case PBEStat.Evasion: statName = "Evasion"; pokemon.EvasionChange = pscp.NewValue; break;
                            case PBEStat.SpAttack: statName = "Special Attack"; pokemon.SpAttackChange = pscp.NewValue; break;
                            case PBEStat.SpDefense: statName = "Special Defense"; pokemon.SpDefenseChange = pscp.NewValue; break;
                            case PBEStat.Speed: statName = "Speed"; pokemon.SpeedChange = pscp.NewValue; break;
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
                                    if (change == 0 && pscp.NewValue == -Battle.Settings.MaxStatChange)
                                    {
                                        message = "won't go lower";
                                    }
                                    else if (change == 0 && pscp.NewValue == Battle.Settings.MaxStatChange)
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
                        BattleView.AddMessage(string.Format("{0}'s {1} {2}!", NameForTrainer(pokemon, true), statName, message), true, true);
                        break;
                    }
                case PBEPkmnSwitchInPacket psip:
                    {
                        foreach (PBEPkmnSwitchInPacket.PBESwitchInInfo info in psip.SwitchIns)
                        {
                            PBEPokemon pokemon;
                            if (BattleId != int.MaxValue && psip.Team.Id != BattleId)
                            {
                                pokemon = new PBEPokemon(psip.Team, info);
                            }
                            else
                            {
                                pokemon = psip.Team.TryGetPokemon(info.PokemonId);
                                pokemon.FieldPosition = info.FieldPosition;
                                if (info.DisguisedAsId != info.PokemonId)
                                {
                                    pokemon.Status2 |= PBEStatus2.Disguised;
                                    pokemon.DisguisedAsPokemon = psip.Team.TryGetPokemon(info.DisguisedAsId);
                                    pokemon.KnownGender = pokemon.DisguisedAsPokemon.Gender;
                                    pokemon.KnownNickname = pokemon.DisguisedAsPokemon.Nickname;
                                    pokemon.KnownShiny = pokemon.DisguisedAsPokemon.Shiny;
                                    pokemon.KnownSpecies = pokemon.DisguisedAsPokemon.OriginalSpecies;
                                    var pData = PBEPokemonData.GetData(pokemon.DisguisedAsPokemon.OriginalSpecies);
                                    pokemon.KnownType1 = pData.Type1;
                                    pokemon.KnownType2 = pData.Type2;
                                }
                            }
                            Battle.ActiveBattlers.Add(pokemon);
                            BattleView.Field.UpdatePokemon(pokemon);
                        }
                        if (!psip.Forced)
                        {
                            BattleView.AddMessage(string.Format("{1} sent out {0}!", PBEUtils.Andify(psip.SwitchIns.Select(s => s.Nickname)), psip.Team.TrainerName), true, true);
                        }
                        break;
                    }
                case PBEPkmnSwitchOutPacket psop:
                    {
                        PBEPokemon pokemon = psop.PokemonTeam.TryGetPokemon(psop.PokemonPosition);
                        if (pokemon.Id == byte.MaxValue)
                        {
                            pokemon.FieldPosition = PBEFieldPosition.None;
                            pokemon.Team.Party.Remove(pokemon);
                        }
                        else
                        {
                            pokemon.ClearForSwitch();
                        }
                        Battle.ActiveBattlers.Remove(pokemon);
                        BattleView.Field.UpdatePokemon(pokemon, psop.PokemonPosition);
                        if (!psop.Forced)
                        {
                            BattleView.AddMessage(string.Format("{1} withdrew {0}!", pokemon.KnownNickname, pokemon.Team.TrainerName), true, true);
                        }
                        break;
                    }
                case PBEPsychUpPacket pup:
                    {
                        PBEPokemon user = pup.UserTeam.TryGetPokemon(pup.User),
                            target = pup.TargetTeam.TryGetPokemon(pup.Target);
                        user.AttackChange = target.AttackChange = pup.AttackChange;
                        user.DefenseChange = target.DefenseChange = pup.DefenseChange;
                        user.SpAttackChange = target.SpAttackChange = pup.SpAttackChange;
                        user.SpDefenseChange = target.SpDefenseChange = pup.SpDefenseChange;
                        user.SpeedChange = target.SpeedChange = pup.SpeedChange;
                        user.AccuracyChange = target.AccuracyChange = pup.AccuracyChange;
                        user.EvasionChange = target.EvasionChange = pup.EvasionChange;
                        BattleView.AddMessage(string.Format("{0} copied {1}'s stat changes!", NameForTrainer(user, true), NameForTrainer(target, false)), true, true);
                        break;
                    }
                case PBESpecialMessagePacket smp:
                    {
                        string message;
                        switch (smp.Message)
                        {
                            case PBESpecialMessage.DraggedOut:
                                {
                                    message = string.Format("{0} was dragged out!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0]), true));
                                    break;
                                }
                            case PBESpecialMessage.Endure:
                                {
                                    message = string.Format("{0} endured the hit!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0]), true));
                                    break;
                                }
                            case PBESpecialMessage.HPDrained:
                                {
                                    message = string.Format("{0} had its energy drained!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0]), true));
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
                                    message = "The battlers shared their pain!";
                                    break;
                                }
                            case PBESpecialMessage.Recoil:
                                {
                                    message = string.Format("{0} is damaged by recoil!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0]), true));
                                    break;
                                }
                            case PBESpecialMessage.Struggle:
                                {
                                    message = string.Format("{0} has no moves left!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0]), true));
                                    break;
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(smp.Message));
                        }
                        BattleView.AddMessage(message, true, true);
                        break;
                    }
                case PBEStatus1Packet s1p:
                    {
                        PBEPokemon status1Receiver = s1p.Status1ReceiverTeam.TryGetPokemon(s1p.Status1Receiver);
                        switch (s1p.StatusAction)
                        {
                            case PBEStatusAction.Added: status1Receiver.Status1 = s1p.Status1; break;
                            case PBEStatusAction.Cured:
                            case PBEStatusAction.Ended: status1Receiver.Status1 = PBEStatus1.None; break;
                        }
                        BattleView.Field.UpdatePokemon(status1Receiver);
                        string message;
                        switch (s1p.Status1)
                        {
                            case PBEStatus1.Asleep:
                                {
                                    switch (s1p.StatusAction)
                                    {
                                        case PBEStatusAction.Activated: message = "{0} is fast asleep."; break;
                                        case PBEStatusAction.Added: message = "{0} fell asleep!"; break;
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
                                        case PBEStatusAction.Activated: message = "{0} is frozen solid!"; break;
                                        case PBEStatusAction.Added: message = "{0} was frozen solid!"; break;
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
                                        case PBEStatusAction.Activated: message = "{0} is paralyzed! It can't move!"; break;
                                        case PBEStatusAction.Added: message = "{0} is paralyzed! It may be unable to move!"; break;
                                        case PBEStatusAction.Cured: message = "{0} was cured of paralysis."; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                                    }
                                    break;
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(s1p.Status1));
                        }
                        BattleView.AddMessage(string.Format(message, NameForTrainer(status1Receiver, true)), true, true);
                        break;
                    }
                case PBEStatus2Packet s2p:
                    {
                        PBEPokemon status2Receiver = s2p.Status2ReceiverTeam.TryGetPokemon(s2p.Status2Receiver),
                            pokemon2 = s2p.Pokemon2Team.TryGetPokemon(s2p.Pokemon2);
                        switch (s2p.StatusAction)
                        {
                            case PBEStatusAction.Added: status2Receiver.Status2 |= s2p.Status2; break;
                            case PBEStatusAction.Ended: status2Receiver.Status2 &= ~s2p.Status2; break;
                        }
                        string message;
                        bool status2ReceiverCaps = true,
                            pokemon2Caps = false;
                        switch (s2p.Status2)
                        {
                            case PBEStatus2.Airborne:
                                {
                                    BattleView.Field.UpdatePokemon(status2Receiver);
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.Added: message = "{0} flew up high!"; break;
                                        case PBEStatusAction.Ended: return true;
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
                                        case PBEStatusAction.Added: message = "{1} cut its own HP and laid a curse on {0}!"; status2ReceiverCaps = false; pokemon2Caps = true; break;
                                        case PBEStatusAction.Damage: message = "{0} is afflicted by the curse!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                                    }
                                    break;
                                }
                            case PBEStatus2.Flinching:
                                {
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.Activated: message = "{0} flinched and couldn't move!"; break;
                                        case PBEStatusAction.Ended: return true;
                                        default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                                    }
                                    break;
                                }
                            case PBEStatus2.HelpingHand:
                                {
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.Added: message = "{1} is ready to help {0}!"; status2ReceiverCaps = false; pokemon2Caps = true; break;
                                        case PBEStatusAction.Ended: return true;
                                        default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                                    }
                                    break;
                                }
                            case PBEStatus2.LeechSeed:
                                {
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.Added:
                                            {
                                                status2Receiver.SeededPosition = pokemon2.FieldPosition;
                                                message = "{0} was seeded!";
                                                break;
                                            }
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
                                        case PBEStatusAction.Ended: return true;
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
                                    BattleView.Field.UpdatePokemon(status2Receiver);
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
                                    status2Receiver.Transform(pokemon2);
                                    BattleView.Field.UpdatePokemon(status2Receiver);
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.Added: message = "{0} transformed into {1}!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                                    }
                                    break;
                                }
                            case PBEStatus2.Underground:
                                {
                                    BattleView.Field.UpdatePokemon(status2Receiver);
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.Added: message = "{0} burrowed its way under the ground!"; break;
                                        case PBEStatusAction.Ended: return true;
                                        default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                                    }
                                    break;
                                }
                            case PBEStatus2.Underwater:
                                {
                                    BattleView.Field.UpdatePokemon(status2Receiver);
                                    switch (s2p.StatusAction)
                                    {
                                        case PBEStatusAction.Added: message = "{0} hid underwater!"; break;
                                        case PBEStatusAction.Ended: return true;
                                        default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                                    }
                                    break;
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(s2p.Status2));
                        }
                        BattleView.AddMessage(string.Format(message, NameForTrainer(status2Receiver, status2ReceiverCaps), NameForTrainer(pokemon2, pokemon2Caps)), true, true);
                        break;
                    }
                case PBETeamStatusPacket tsp:
                    {
                        switch (tsp.TeamStatusAction)
                        {
                            case PBETeamStatusAction.Added: tsp.Team.TeamStatus |= tsp.TeamStatus; break;
                            case PBETeamStatusAction.Cleared:
                            case PBETeamStatusAction.Ended: tsp.Team.TeamStatus &= ~tsp.TeamStatus; break;
                        }
                        PBEPokemon damageVictim = tsp.Team.TryGetPokemon(tsp.DamageVictim);
                        string message;
                        bool damageVictimCaps = false;
                        switch (tsp.TeamStatus)
                        {
                            case PBETeamStatus.LightScreen:
                                {
                                    switch (tsp.TeamStatusAction)
                                    {
                                        case PBETeamStatusAction.Added: message = "Light Screen raised {0} team's Special Defense!"; break;
                                        case PBETeamStatusAction.Cleared:
                                        case PBETeamStatusAction.Ended: message = "{1} team's Light Screen wore off!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                                    }
                                    break;
                                }
                            case PBETeamStatus.LuckyChant:
                                {
                                    switch (tsp.TeamStatusAction)
                                    {
                                        case PBETeamStatusAction.Added: message = "The Lucky Chant shielded {0} team from critical hits!"; break;
                                        case PBETeamStatusAction.Ended: message = "{1} team's Lucky Chant wore off!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                                    }
                                    break;
                                }
                            case PBETeamStatus.Reflect:
                                {
                                    switch (tsp.TeamStatusAction)
                                    {
                                        case PBETeamStatusAction.Added: message = "Reflect raised {0} team's Defense!"; break;
                                        case PBETeamStatusAction.Cleared:
                                        case PBETeamStatusAction.Ended: message = "{1} team's Reflect wore off!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                                    }
                                    break;
                                }
                            case PBETeamStatus.Spikes:
                                {
                                    switch (tsp.TeamStatusAction)
                                    {
                                        case PBETeamStatusAction.Added:
                                            {
                                                tsp.Team.SpikeCount++;
                                                message = "Spikes were scattered all around the feet of {2} team!";
                                                break;
                                            }
                                        case PBETeamStatusAction.Cleared:
                                            {
                                                tsp.Team.SpikeCount = 0;
                                                message = "The spikes disappeared from around {2} team's feet!";
                                                break;
                                            }
                                        case PBETeamStatusAction.Damage: message = "{4} is hurt by the spikes!"; damageVictimCaps = true; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                                    }
                                    break;
                                }
                            case PBETeamStatus.StealthRock:
                                {
                                    switch (tsp.TeamStatusAction)
                                    {
                                        case PBETeamStatusAction.Added: message = "Pointed stones float in the air around {3} team!"; break;
                                        case PBETeamStatusAction.Cleared: message = "The pointed stones disappeared from around {2} team!"; break;
                                        case PBETeamStatusAction.Damage: message = "Pointed stones dug into {4}!"; break;
                                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                                    }
                                    break;
                                }
                            case PBETeamStatus.ToxicSpikes:
                                {
                                    switch (tsp.TeamStatusAction)
                                    {
                                        case PBETeamStatusAction.Added:
                                            {
                                                tsp.Team.ToxicSpikeCount++;
                                                message = "Poison spikes were scattered all around {2} team's feet!";
                                                break;
                                            }
                                        case PBETeamStatusAction.Cleared:
                                            {
                                                tsp.Team.ToxicSpikeCount = 0;
                                                message = "The poison spikes disappeared from around {2} team's feet!";
                                                break;
                                            }
                                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                                    }
                                    break;
                                }
                            case PBETeamStatus.WideGuard:
                                {
                                    switch (tsp.TeamStatusAction)
                                    {
                                        case PBETeamStatusAction.Added: message = "Wide Guard protected {2} team!"; break;
                                        case PBETeamStatusAction.Damage: message = "Wide Guard protected {4}!"; break;
                                        case PBETeamStatusAction.Ended: return true;
                                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                                    }
                                    break;
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatus));
                        }
                        BattleView.AddMessage(string.Format(message,
                            BattleId >= 2 ? $"{tsp.Team.TrainerName}'s" : tsp.Team.Id == BattleId ? "your" : "the opposing",
                            BattleId >= 2 ? $"{tsp.Team.TrainerName}'s" : tsp.Team.Id == BattleId ? "Your" : "The opposing",
                            BattleId >= 2 ? $"{tsp.Team.TrainerName}'s" : tsp.Team.Id == BattleId ? "your" : "the foe's",
                            BattleId >= 2 ? $"{tsp.Team.TrainerName}'s" : tsp.Team.Id == BattleId ? "your" : "your foe's",
                            NameForTrainer(damageVictim, damageVictimCaps)
                            ), true, true);
                        break;
                    }
                case PBETransformPacket tp:
                    {
                        PBEPokemon user = tp.UserTeam.TryGetPokemon(tp.User),
                            target = tp.TargetTeam.TryGetPokemon(tp.Target);
                        target.Attack = tp.TargetAttack;
                        target.Defense = tp.TargetDefense;
                        target.SpAttack = tp.TargetSpAttack;
                        target.SpDefense = tp.TargetSpDefense;
                        target.Speed = tp.TargetSpeed;
                        target.AttackChange = tp.TargetAttackChange;
                        target.DefenseChange = tp.TargetDefenseChange;
                        target.SpAttackChange = tp.TargetSpAttackChange;
                        target.SpDefenseChange = tp.TargetSpDefenseChange;
                        target.SpeedChange = tp.TargetSpeedChange;
                        target.AccuracyChange = tp.TargetAccuracyChange;
                        target.EvasionChange = tp.TargetEvasionChange;
                        target.Ability = target.KnownAbility = tp.TargetAbility;
                        target.Moves = tp.TargetMoves.ToArray();
                        target.Species = target.KnownSpecies = tp.TargetSpecies;
                        target.Type1 = target.KnownType1 = tp.TargetType1;
                        target.Type2 = target.KnownType2 = tp.TargetType2;
                        target.Weight = target.KnownWeight = tp.TargetWeight;
                        return true;
                    }
                case PBEWeatherPacket wp:
                    {
                        switch (wp.WeatherAction)
                        {
                            case PBEWeatherAction.Added:
                                {
                                    Battle.Weather = wp.Weather;
                                    BattleView.Field.UpdateWeather();
                                    break;
                                }
                            case PBEWeatherAction.Ended:
                                {
                                    Battle.Weather = PBEWeather.None;
                                    BattleView.Field.UpdateWeather();
                                    break;
                                }
                        }
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
                        BattleView.AddMessage(string.Format(message, NameForTrainer(damageVictim, true)), true, true);
                        break;
                    }
                case PBEWinnerPacket win:
                    {
                        Battle.Winner = win.WinningTeam;
                        BattleView.AddMessage(string.Format("{0} defeated {1}!", win.WinningTeam.TrainerName, (win.WinningTeam == Battle.Teams[0] ? Battle.Teams[1] : Battle.Teams[0]).TrainerName), true, true);
                        break;
                    }
                case PBEActionsRequestPacket arp:
                    {
                        if (arp.Team.Id == BattleId)
                        {
                            ActionsLoop(true);
                        }
                        else if (BattleId != int.MaxValue && BattleId >= 2)
                        {
                            BattleView.AddMessage("Waiting for players...", true, false);
                        }
                        return true;
                    }
                case PBESwitchInRequestPacket sirp:
                    {
                        sirp.Team.SwitchInsRequired = sirp.Amount;
                        if (sirp.Team.Id == BattleId)
                        {
                            SwitchesLoop(true);
                        }
                        else if (BattleId == int.MaxValue)
                        {

                        }
                        else if (BattleId >= 2)
                        {
                            BattleView.AddMessage("Waiting for players...", true, false);
                        }
                        else if (Battle.Teams[BattleId].SwitchInsRequired == 0) // Don't display this message if we're in switchesloop because it'd overwrite the messages we need to see.
                        {
                            BattleView.AddMessage($"Waiting for {Battle.Teams[BattleId == 0 ? 1 : 0].TrainerName}...", true, false);
                        }
                        return true;
                    }
                case PBETurnBeganPacket tbp:
                    {
                        BattleView.AddMessage($"Turn {Battle.TurnNumber = tbp.TurnNumber}", false, true);
                        return true;
                    }
            }
            return false;
        }

        readonly List<PBEPokemon> actions = new List<PBEPokemon>(3);
        public List<PBEPokemon> StandBy { get; } = new List<PBEPokemon>(3);
        public void ActionsLoop(bool begin)
        {
            if (begin)
            {
                foreach (PBEPokemon pkmn in Battle.Teams[BattleId].Party)
                {
                    pkmn.SelectedAction.Decision = PBEDecision.None;
                }
                actions.Clear();
                actions.AddRange(Battle.Teams[BattleId].ActiveBattlers);
                StandBy.Clear();
            }
            int i = actions.FindIndex(p => p.SelectedAction.Decision == PBEDecision.None);
            if (i == -1)
            {
                BattleView.AddMessage($"Waiting for {Battle.Teams[BattleId == 0 ? 1 : 0].TrainerName}...", true, false);
                Send(new PBEActionsResponsePacket(actions.Select(p => p.SelectedAction)));
            }
            else
            {
                BattleView.AddMessage($"What will {actions[i].Nickname} do?", true, false);
                BattleView.Actions.DisplayActions(actions[i]);
            }
        }
        public List<Tuple<byte, PBEFieldPosition>> Switches { get; } = new List<Tuple<byte, PBEFieldPosition>>(3);
        public List<PBEFieldPosition> PositionStandBy { get; } = new List<PBEFieldPosition>(3);
        public void SwitchesLoop(bool begin)
        {
            if (begin)
            {
                Switches.Clear();
                StandBy.Clear();
                PositionStandBy.Clear();
            }
            else
            {
                Battle.Teams[BattleId].SwitchInsRequired--;
            }
            if (Battle.Teams[BattleId].SwitchInsRequired == 0)
            {
                BattleView.AddMessage($"Waiting for {(Battle.Teams[BattleId == 0 ? 1 : 0].SwitchInsRequired > 0 ? Battle.Teams[BattleId == 0 ? 1 : 0].TrainerName : "server")}...", true, false);
                Send(new PBESwitchInResponsePacket(Switches));
            }
            else
            {
                BattleView.AddMessage($"You must send in {Battle.Teams[BattleId].SwitchInsRequired} Pokémon.", true, false);
                BattleView.Actions.DisplaySwitches();
            }
        }

        protected override void OnConnected()
        {
            Debug.WriteLine("Connected to {0}", Socket.RemoteEndPoint);
            BattleView.AddMessage("Waiting for players...", false, true);
        }
        protected override void OnDisconnected()
        {
            Debug.WriteLine("Disconnected from server");
            //Environment.Exit(0);
        }
        protected override void OnSocketError(SocketError socketError)
        {
            Debug.WriteLine("Socket Error: {0}", socketError);
        }
    }
}
