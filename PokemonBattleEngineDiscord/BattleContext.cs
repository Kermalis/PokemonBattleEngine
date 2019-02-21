using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Localization;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
    public class BattleCommandContext : SocketCommandContext
    {
        public BattleContext BattleContext { get; }

        public BattleCommandContext(BattleContext battleContext, DiscordSocketClient client, SocketUserMessage msg)
            : base(client, msg)
        {
            BattleContext = battleContext;
        }
    }

    public class BattleContext
    {
        public static List<BattleContext> ActiveBattles { get; } = new List<BattleContext>();

        public PBEBattle Battle { get; }
        public SocketUser[] Battlers { get; }
        public ISocketMessageChannel Channel { get; }

        public BattleContext(PBEBattle battle, SocketUser battler0, SocketUser battler1, ISocketMessageChannel channel)
        {
            ActiveBattles.Add(this);

            Battle = battle;
            Battlers = new SocketUser[] { battler0, battler1 };
            Channel = channel;

            battle.OnNewEvent += (b, p) => Battle_OnNewEvent(ActiveBattles.Single(a => a.Battle == b), p).GetAwaiter().GetResult();
            battle.OnStateChanged += (b) => Battle_OnStateChanged(ActiveBattles.Single(a => a.Battle == b)).GetAwaiter().GetResult();
            Battle.Begin();
        }

        static async Task Battle_OnStateChanged(BattleContext context)
        {
            switch (context.Battle.BattleState)
            {
                case PBEBattleState.ReadyToRunTurn:
                    {
                        context.Battle.RunTurn();
                        break;
                    }
                case PBEBattleState.Ended:
                    {
                        ActiveBattles.Remove(context);
                        break;
                    }
            }
        }
        static async Task Battle_OnNewEvent(BattleContext context, INetPacket packet)
        {
            string NameForTrainer(PBEPokemon pkmn)
            {
                return pkmn == null ? string.Empty : $"{pkmn.Team.TrainerName}'s {pkmn.VisualNickname}";
            }
            async Task CreateAndSendEmbed(string message, PBEPokemon pkmn = null)
            {
                string title = $"{context.Battlers[0].Username} vs {context.Battlers[1].Username}";
                if (context.Battle.TurnNumber > 0)
                {
                    title += $" (Turn {context.Battle.TurnNumber})";
                }
                var embed = new EmbedBuilder()
                    .WithUrl("https://github.com/Kermalis/PokemonBattleEngine")
                    .WithTitle(title)
                    .WithDescription(message);
                if (pkmn != null)
                {
                    embed = embed.WithColor(Utils.GetColor(pkmn)).WithImageUrl(Utils.GetPokemonSprite(pkmn));
                }
                await context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
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
                            case PBEAbility.Rattled:
                                {
                                    switch (ap.AbilityAction)
                                    {
                                        case PBEAbilityAction.Damage: message = "{0}'s {2} activated!"; break; // Message is displayed from a stat changed packet
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
                        await CreateAndSendEmbed(string.Format(message, NameForTrainer(abilityOwner), NameForTrainer(pokemon2), PBEAbilityLocalization.Names[ap.Ability].English), abilityOwner);
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
                        await CreateAndSendEmbed(message);
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
                        await CreateAndSendEmbed(string.Format(message, NameForTrainer(itemHolder), NameForTrainer(pokemon2), PBEItemLocalization.Names[ip.Item].English), itemHolder);
                        break;
                    }
                case PBEMoveCritPacket _:
                    {
                        await CreateAndSendEmbed("A critical hit!");
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
                            case PBEEffectiveness.Normal: return;
                            case PBEEffectiveness.SuperEffective: message = "It's super effective!"; break;
                            default: throw new ArgumentOutOfRangeException(nameof(mep.Effectiveness));
                        }
                        await CreateAndSendEmbed(string.Format(message, NameForTrainer(victim)), victim);
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
                        await CreateAndSendEmbed(string.Format(message, NameForTrainer(moveUser), NameForTrainer(pokemon2)), pokemon2);
                        break;
                    }
                case PBEMoveMissedPacket mmp:
                    {
                        PBEPokemon moveUser = mmp.MoveUserTeam.TryGetPokemon(mmp.MoveUser),
                            pokemon2 = mmp.Pokemon2Team.TryGetPokemon(mmp.Pokemon2);
                        await CreateAndSendEmbed(string.Format("{0}'s attack missed {1}!", NameForTrainer(moveUser), NameForTrainer(pokemon2)), moveUser);
                        break;
                    }
                case PBEMoveUsedPacket mup:
                    {
                        PBEPokemon moveUser = mup.MoveUserTeam.TryGetPokemon(mup.MoveUser);
                        await CreateAndSendEmbed(string.Format("{0} used {1}!", NameForTrainer(moveUser), PBEMoveLocalization.Names[mup.Move].English), moveUser);
                        break;
                    }
                case PBEPkmnFaintedPacket pfap:
                    {
                        PBEPokemon pokemon = context.Battle.TryGetPokemon(pfap.PokemonId);
                        await CreateAndSendEmbed(string.Format("{0} fainted!", NameForTrainer(pokemon)), pokemon);
                        break;
                    }
                case PBEPkmnHPChangedPacket phcp:
                    {
                        PBEPokemon pokemon = phcp.PokemonTeam.TryGetPokemon(phcp.Pokemon);
                        int change = phcp.NewHP - phcp.OldHP;
                        int absChange = Math.Abs(change);
                        double percentageChange = phcp.NewHPPercentage - phcp.OldHPPercentage;
                        double absPercentageChange = Math.Abs(percentageChange);
                        await CreateAndSendEmbed(string.Format("{0} {1} {2:P2} of its HP!", NameForTrainer(pokemon), percentageChange <= 0 ? "lost" : "restored", absPercentageChange), pokemon);
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
                                    if (change == 0 && pscp.NewValue == -context.Battle.Settings.MaxStatChange)
                                    {
                                        message = "won't go lower";
                                    }
                                    else if (change == 0 && pscp.NewValue == context.Battle.Settings.MaxStatChange)
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
                        await CreateAndSendEmbed(string.Format("{0}'s {1} {2}!", NameForTrainer(pokemon), statName, message), pokemon);
                        break;
                    }
                case PBEPkmnSwitchInPacket psip:
                    {
                        if (!psip.Forced)
                        {
                            foreach (PBEPkmnSwitchInPacket.PBESwitchInInfo info in psip.SwitchIns)
                            {
                                PBEPokemon pokemon = context.Battle.TryGetPokemon(info.PokemonId);
                                await CreateAndSendEmbed(string.Format("{1} sent out {0}!", pokemon.VisualNickname, psip.Team.TrainerName), pokemon);
                            }
                        }
                        break;
                    }
                case PBEPkmnSwitchOutPacket psop:
                    {
                        if (!psop.Forced)
                        {
                            PBEPokemon pokemon = context.Battle.TryGetPokemon(psop.PokemonId);
                            await CreateAndSendEmbed(string.Format("{1} withdrew {0}!", pokemon.VisualNickname, pokemon.Team.TrainerName), pokemon);
                        }
                        break;
                    }
                case PBEPsychUpPacket pup:
                    {
                        PBEPokemon user = pup.UserTeam.TryGetPokemon(pup.User),
                            target = pup.TargetTeam.TryGetPokemon(pup.Target);
                        await CreateAndSendEmbed(string.Format("{0} copied {1}'s stat changes!", NameForTrainer(user), NameForTrainer(target)), user);
                        break;
                    }
                case PBESpecialMessagePacket smp:
                    {
                        PBEPokemon pokemon = null;
                        string message;
                        switch (smp.Message)
                        {
                            case PBESpecialMessage.DraggedOut:
                                {
                                    pokemon = ((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0]);
                                    message = string.Format("{0} was dragged out!", NameForTrainer(pokemon));
                                    break;
                                }
                            case PBESpecialMessage.Endure:
                                {
                                    pokemon = ((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0]);
                                    message = string.Format("{0} endured the hit!", NameForTrainer(pokemon));
                                    break;
                                }
                            case PBESpecialMessage.HPDrained:
                                {
                                    pokemon = ((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0]);
                                    message = string.Format("{0} had its energy drained!", NameForTrainer(pokemon));
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
                                    pokemon = ((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0]);
                                    message = string.Format("{0} is damaged by recoil!", NameForTrainer(pokemon));
                                    break;
                                }
                            case PBESpecialMessage.Struggle:
                                {
                                    pokemon = ((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0]);
                                    message = string.Format("{0} has no moves left!", NameForTrainer(pokemon));
                                    break;
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(smp.Message));
                        }
                        await CreateAndSendEmbed(message, pokemon);
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
                        await CreateAndSendEmbed(string.Format(message, NameForTrainer(status1Receiver)), status1Receiver);
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
                                        case PBEStatusAction.Activated: message = "{0} flinched and couldn't move!"; break;
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
                        await CreateAndSendEmbed(string.Format(message, NameForTrainer(status2Receiver), NameForTrainer(pokemon2)), status2Receiver);
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
                            default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatus));
                        }
                        await CreateAndSendEmbed(string.Format(message, tsp.Team.TrainerName, NameForTrainer(damageVictim)), damageVictim);
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
                        await CreateAndSendEmbed(string.Format(message, NameForTrainer(damageVictim)), damageVictim);
                        break;
                    }
                case PBEWinnerPacket win:
                    {
                        await CreateAndSendEmbed(string.Format("{0} defeated {1}!", win.WinningTeam.TrainerName, (win.WinningTeam == context.Battle.Teams[0] ? context.Battle.Teams[1] : context.Battle.Teams[0]).TrainerName));
                        break;
                    }
                case PBEActionsRequestPacket arp:
                    {
                        SocketUser guy = context.Battlers[Array.IndexOf(context.Battle.Teams, arp.Team)];
                        await guy.SendMessageAsync($"Actions for turn {context.Battle.TurnNumber}");
                        PBEBattle.SelectActionsIfValid(arp.Team, PokemonBattleEngine.AI.AIManager.CreateActions(arp.Team));
                        break;
                    }
                case PBESwitchInRequestPacket sirp:
                    {
                        SocketUser guy = context.Battlers[Array.IndexOf(context.Battle.Teams, sirp.Team)];
                        await guy.SendMessageAsync("Switches");
                        PBEBattle.SelectSwitchesIfValid(sirp.Team, PokemonBattleEngine.AI.AIManager.CreateSwitches(sirp.Team));
                        break;
                    }
            }
        }
    }
}
