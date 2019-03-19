using Discord;
using Discord.WebSocket;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Localization;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
    public class BattleContext
    {
        const string separator = "**--------------------**";
        static readonly Emoji switchEmoji = new Emoji("😼");
        static readonly Emoji confirmationEmoji = new Emoji("👍");
        static readonly List<BattleContext> activeBattles = new List<BattleContext>(); // TODO: Locks for accessing this

        readonly PBEBattle battle;
        readonly SocketUser[] battlers;
        readonly ISocketMessageChannel channel;

        public BattleContext(PBEBattle battle, SocketUser battler0, SocketUser battler1, ISocketMessageChannel channel)
        {
            activeBattles.Add(this);

            this.battle = battle;
            battlers = new SocketUser[] { battler0, battler1 };
            this.channel = channel;

            battle.OnNewEvent += (b, p) => Battle_OnNewEvent(activeBattles.Single(a => a.battle == b), p).GetAwaiter().GetResult();
            battle.OnStateChanged += (b) => Battle_OnStateChanged(activeBattles.Single(a => a.battle == b)).GetAwaiter().GetResult();
            battle.Begin();
        }

        public static BattleContext GetBattleContext(SocketUser user)
        {
            return activeBattles.SingleOrDefault(b => b.IndexOf(user) != -1);
        }
        public int IndexOf(SocketUser user)
        {
            if (battlers[0].Id == user.Id)
            {
                return 0;
            }
            else if (battlers[1].Id == user.Id)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        static string CustomPokemonToString(PBEPokemon pkmn, bool addReactionChars)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{pkmn.Nickname}/{pkmn.OriginalSpecies} {pkmn.GenderSymbol} Lv.{pkmn.Level}");
            sb.AppendLine($"**HP:** {pkmn.HP}/{pkmn.MaxHP} ({pkmn.HPPercentage:P2})");
            if (pkmn.FieldPosition != PBEFieldPosition.None)
            {
                sb.AppendLine($"**Types:** {pkmn.Type1}/{pkmn.Type2}");
            }
            if (pkmn.Status1 != PBEStatus1.None)
            {
                sb.AppendLine($"**Main status:** {pkmn.Status1}");
            }
            if (pkmn.FieldPosition != PBEFieldPosition.None)
            {
                if (pkmn.Status1 == PBEStatus1.Asleep)
                {
                    sb.AppendLine($"**Sleep turns:** {pkmn.Status1Counter}");
                }
                else if (pkmn.Status1 == PBEStatus1.BadlyPoisoned)
                {
                    sb.AppendLine($"**Toxic Counter:** {pkmn.Status1Counter}");
                }

                if (pkmn.Status2 != PBEStatus2.None)
                {
                    sb.AppendLine($"**Volatile status:** {pkmn.Status2}");
                }
                if (pkmn.Status2.HasFlag(PBEStatus2.Confused))
                {
                    sb.AppendLine($"**Confusion turns:** {pkmn.ConfusionCounter}");
                }
                if (pkmn.Status2.HasFlag(PBEStatus2.Disguised))
                {
                    sb.AppendLine($"**Disguised as:** {pkmn.DisguisedAsPokemon.Nickname}");
                }
                if (pkmn.Status2.HasFlag(PBEStatus2.Substitute))
                {
                    sb.AppendLine($"**Substitute HP:** {pkmn.SubstituteHP}");
                }
            }
            sb.AppendLine($"**Stats:** [A] {pkmn.Attack}, [D] {pkmn.Defense}, [SA] {pkmn.SpAttack}, [SD] {pkmn.SpDefense}, [S] {pkmn.Speed}, [W] {pkmn.Weight:0.0}");
            if (pkmn.FieldPosition != PBEFieldPosition.None)
            {
                PBEStat[] statChanges = pkmn.GetChangedStats();
                if (statChanges.Length > 0)
                {
                    var statStrs = new List<string>(7);
                    if (Array.IndexOf(statChanges, PBEStat.Attack) != -1)
                    {
                        statStrs.Add($"[A] x{PBEBattle.GetStatChangeModifier(pkmn.AttackChange, false):0.00}");
                    }
                    if (Array.IndexOf(statChanges, PBEStat.Defense) != -1)
                    {
                        statStrs.Add($"[D] x{PBEBattle.GetStatChangeModifier(pkmn.DefenseChange, false):0.00}");
                    }
                    if (Array.IndexOf(statChanges, PBEStat.SpAttack) != -1)
                    {
                        statStrs.Add($"[SA] x{PBEBattle.GetStatChangeModifier(pkmn.SpAttackChange, false):0.00}");
                    }
                    if (Array.IndexOf(statChanges, PBEStat.SpDefense) != -1)
                    {
                        statStrs.Add($"[SD] x{PBEBattle.GetStatChangeModifier(pkmn.SpDefenseChange, false):0.00}");
                    }
                    if (Array.IndexOf(statChanges, PBEStat.Speed) != -1)
                    {
                        statStrs.Add($"[S] x{PBEBattle.GetStatChangeModifier(pkmn.SpeedChange, false):0.00}");
                    }
                    if (Array.IndexOf(statChanges, PBEStat.Accuracy) != -1)
                    {
                        statStrs.Add($"[AC] x{PBEBattle.GetStatChangeModifier(pkmn.AccuracyChange, true):0.00}");
                    }
                    if (Array.IndexOf(statChanges, PBEStat.Evasion) != -1)
                    {
                        statStrs.Add($"[E] x{PBEBattle.GetStatChangeModifier(pkmn.EvasionChange, true):0.00}");
                    }
                    sb.AppendLine($"**Stat changes:** {string.Join(", ", statStrs)}");
                }
            }
            sb.AppendLine($"**Item:** {PBEItemLocalization.Names[pkmn.Item].English}");
            sb.AppendLine($"**Ability:** {PBEAbilityLocalization.Names[pkmn.Ability].English}");
            if (Array.IndexOf(pkmn.Moves, PBEMove.HiddenPower) != -1)
            {
                sb.AppendLine($"**Hidden Power:** {pkmn.GetHiddenPowerType()}/{pkmn.GetHiddenPowerBasePower()}");
            }
            string[] moveStrs = new string[PBESettings.DefaultSettings.NumMoves];
            for (int i = 0; i < PBESettings.DefaultSettings.NumMoves; i++)
            {
                PBEMove move = pkmn.Moves[i];
                string str = PBEMoveLocalization.Names[move].English;
                int moveIndex = Array.IndexOf(pkmn.Moves, move);
                if (moveIndex != -1)
                {
                    str += $" {pkmn.PP[moveIndex]}/{pkmn.MaxPP[moveIndex]}";
                }
                moveStrs[i] = str;
            }
            sb.AppendLine($"**Moves:** {string.Join(", ", moveStrs)}");
            PBEMove[] usableMoves = pkmn.GetUsableMoves();
            moveStrs = new string[usableMoves.Length];
            for (int i = 0; i < usableMoves.Length; i++)
            {
                PBEMove move = usableMoves[i];
                string str = string.Empty;
                if (addReactionChars)
                {
                    str += $"{char.ConvertFromUtf32(0x1F1E6 + i)} ";
                }
                str += PBEMoveLocalization.Names[move].English;
                moveStrs[i] = str;
            }
            sb.Append($"**Usable moves:** {string.Join(", ", moveStrs)}");
            return sb.ToString();
        }

        static Task Battle_OnStateChanged(BattleContext context)
        {
            switch (context.battle.BattleState)
            {
                case PBEBattleState.ReadyToRunTurn:
                    {
                        context.battle.RunTurn();
                        break;
                    }
                case PBEBattleState.Ended:
                    {
                        activeBattles.Remove(context);
                        break;
                    }
            }
            return Task.CompletedTask;
        }
        static async Task Battle_OnNewEvent(BattleContext context, INetPacket packet)
        {
            string NameForTrainer(PBEPokemon pkmn)
            {
                return pkmn == null ? string.Empty : $"{pkmn.Team.TrainerName}'s {pkmn.KnownNickname}";
            }
            async Task<IUserMessage> CreateAndSendEmbedAsync(string embedDescription, string messageText = "", PBEPokemon pkmn = null, bool useUpperImage = false, SocketUser userToSendTo = null)
            {
                string title = $"{context.battlers[0].Username} vs {context.battlers[1].Username}";
                if (context.battle.TurnNumber > 0)
                {
                    title += $" (Turn {context.battle.TurnNumber})";
                }

                var embed = new EmbedBuilder()
                    .WithUrl(Utils.URL)
                    .WithTitle(title)
                    .WithDescription(embedDescription);
                if (pkmn == null)
                {
                    embed.WithColor(Utils.RandomColor());
                }
                else
                {
                    embed.WithColor(Utils.GetColor(pkmn));
                    string sprite = Utils.GetPokemonSprite(pkmn);
                    if (useUpperImage)
                    {
                        embed.WithThumbnailUrl(sprite);
                    }
                    else
                    {
                        embed.WithImageUrl(sprite);
                    }
                }

                if (userToSendTo != null)
                {
                    return await userToSendTo.SendMessageAsync(messageText, embed: embed.Build());
                }
                else
                {
                    return await context.channel.SendMessageAsync(messageText, embed: embed.Build());
                }
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
                        await CreateAndSendEmbedAsync(string.Format(message, NameForTrainer(abilityOwner), NameForTrainer(pokemon2), PBEAbilityLocalization.Names[ap.Ability].English), pkmn: abilityOwner);
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
                        await CreateAndSendEmbedAsync(message);
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
                        await CreateAndSendEmbedAsync(string.Format(message, NameForTrainer(itemHolder), NameForTrainer(pokemon2), PBEItemLocalization.Names[ip.Item].English), pkmn: itemHolder);
                        break;
                    }
                case PBEMoveCritPacket _:
                    {
                        await CreateAndSendEmbedAsync("A critical hit!");
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
                        await CreateAndSendEmbedAsync(string.Format(message, NameForTrainer(victim)), pkmn: victim);
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
                        await CreateAndSendEmbedAsync(string.Format(message, NameForTrainer(moveUser), NameForTrainer(pokemon2)), pkmn: pokemon2);
                        break;
                    }
                case PBEMoveMissedPacket mmp:
                    {
                        PBEPokemon moveUser = mmp.MoveUserTeam.TryGetPokemon(mmp.MoveUser),
                            pokemon2 = mmp.Pokemon2Team.TryGetPokemon(mmp.Pokemon2);
                        await CreateAndSendEmbedAsync(string.Format("{0}'s attack missed {1}!", NameForTrainer(moveUser), NameForTrainer(pokemon2)), pkmn: moveUser);
                        break;
                    }
                case PBEMoveUsedPacket mup:
                    {
                        PBEPokemon moveUser = mup.MoveUserTeam.TryGetPokemon(mup.MoveUser);
                        await CreateAndSendEmbedAsync(string.Format("{0} used {1}!", NameForTrainer(moveUser), PBEMoveLocalization.Names[mup.Move].English), pkmn: moveUser);
                        break;
                    }
                case PBEPkmnFaintedPacket pfap:
                    {
                        PBEPokemon pokemon = context.battle.TryGetPokemon(pfap.PokemonId);
                        await CreateAndSendEmbedAsync(string.Format("{0} fainted!", NameForTrainer(pokemon)), pkmn: pokemon);
                        break;
                    }
                case PBEPkmnHPChangedPacket phcp:
                    {
                        PBEPokemon pokemon = phcp.PokemonTeam.TryGetPokemon(phcp.Pokemon);
                        int change = phcp.NewHP - phcp.OldHP;
                        int absChange = Math.Abs(change);
                        double percentageChange = phcp.NewHPPercentage - phcp.OldHPPercentage;
                        double absPercentageChange = Math.Abs(percentageChange);
                        await CreateAndSendEmbedAsync(string.Format("{0} {1} {2:P2} of its HP!", NameForTrainer(pokemon), percentageChange <= 0 ? "lost" : "restored", absPercentageChange), pkmn: pokemon);
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
                                    if (change == 0 && pscp.NewValue == -context.battle.Settings.MaxStatChange)
                                    {
                                        message = "won't go lower";
                                    }
                                    else if (change == 0 && pscp.NewValue == context.battle.Settings.MaxStatChange)
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
                        await CreateAndSendEmbedAsync(string.Format("{0}'s {1} {2}!", NameForTrainer(pokemon), statName, message), pkmn: pokemon);
                        break;
                    }
                case PBEPkmnSwitchInPacket psip:
                    {
                        if (!psip.Forced)
                        {
                            foreach (PBEPkmnSwitchInPacket.PBESwitchInInfo info in psip.SwitchIns)
                            {
                                PBEPokemon pokemon = context.battle.TryGetPokemon(info.PokemonId);
                                await CreateAndSendEmbedAsync(string.Format("{1} sent out {0}!", pokemon.KnownNickname, psip.Team.TrainerName), pkmn: pokemon);
                            }
                        }
                        break;
                    }
                case PBEPkmnSwitchOutPacket psop:
                    {
                        if (!psop.Forced)
                        {
                            PBEPokemon pokemon = context.battle.TryGetPokemon(psop.PokemonId);
                            await CreateAndSendEmbedAsync(string.Format("{1} withdrew {0}!", pokemon.KnownNickname, pokemon.Team.TrainerName), pkmn: pokemon);
                        }
                        break;
                    }
                case PBEPsychUpPacket pup:
                    {
                        PBEPokemon user = pup.UserTeam.TryGetPokemon(pup.User),
                            target = pup.TargetTeam.TryGetPokemon(pup.Target);
                        await CreateAndSendEmbedAsync(string.Format("{0} copied {1}'s stat changes!", NameForTrainer(user), NameForTrainer(target)), pkmn: user);
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
                        await CreateAndSendEmbedAsync(message, pkmn: pokemon);
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
                        await CreateAndSendEmbedAsync(string.Format(message, NameForTrainer(status1Receiver)), pkmn: status1Receiver);
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
                        await CreateAndSendEmbedAsync(string.Format(message, NameForTrainer(status2Receiver), NameForTrainer(pokemon2)), pkmn: status2Receiver);
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
                        await CreateAndSendEmbedAsync(string.Format(message, tsp.Team.TrainerName, NameForTrainer(damageVictim)), pkmn: damageVictim);
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
                        await CreateAndSendEmbedAsync(string.Format(message, NameForTrainer(damageVictim)), pkmn: damageVictim);
                        break;
                    }
                case PBEWinnerPacket win:
                    {
                        await CreateAndSendEmbedAsync(string.Format("{0} defeated {1}!", win.WinningTeam.TrainerName, (win.WinningTeam == context.battle.Teams[0] ? context.battle.Teams[1] : context.battle.Teams[0]).TrainerName));
                        break;
                    }
                case PBEActionsRequestPacket arp:
                    {
                        SocketUser user = context.battlers[Array.IndexOf(context.battle.Teams, arp.Team)];
                        var userArray = new SocketUser[] { user };
                        PBEPokemon mainPkmn = arp.Team.ActionsRequired[0];
                        var allMessages = new List<IUserMessage>(PBESettings.DefaultSettings.MaxPartySize);
                        var reactionsToAdd = new List<Tuple<IUserMessage, IEmote>>(PBESettings.DefaultSettings.MaxPartySize - 1 + PBESettings.DefaultSettings.NumMoves); // 5 switch reactions, 4 move reactions

                        if (mainPkmn.CanSwitchOut())
                        {
                            async Task SwitchReactionClicked(IUserMessage switchMsg, PBEPokemon switchPkmn)
                            {
                                await switchMsg.AddReactionAsync(confirmationEmoji); // Put this here so it happens before RunTurn() takes its time
                                var action = new PBEAction
                                {
                                    Decision = PBEDecision.SwitchOut,
                                    PokemonId = mainPkmn.Id,
                                    SwitchPokemonId = switchPkmn.Id
                                };
                                PBEBattle.SelectActionsIfValid(arp.Team, new[] { action });
                            }

                            PBEPokemon[] switches = arp.Team.Party.Where(p => p != mainPkmn && p.HP > 0).ToArray();
                            for (int i = 0; i < switches.Length; i++)
                            {
                                PBEPokemon switchPkmn = switches[i];
                                IUserMessage switchMsg = await CreateAndSendEmbedAsync(CustomPokemonToString(switchPkmn, false), messageText: i == 0 ? separator : string.Empty, pkmn: switchPkmn, useUpperImage: true, userToSendTo: user);
                                allMessages.Add(switchMsg);
                                reactionsToAdd.Add(Tuple.Create(switchMsg, (IEmote)switchEmoji));
                                ReactionListener.AddListener(switchMsg, allMessages, switchEmoji, userArray, () => SwitchReactionClicked(switchMsg, switchPkmn));
                            }
                        }

                        IUserMessage mainMsg = await CreateAndSendEmbedAsync($"{CustomPokemonToString(mainPkmn, true)}\nTo check a move: `!move info {PBEUtils.Sample(PBEMoveLocalization.Names).Value.English}`", pkmn: mainPkmn, useUpperImage: false, userToSendTo: user);
                        allMessages.Add(mainMsg);

                        async Task MoveReactionClicked(PBEMove move)
                        {
                            await mainMsg.AddReactionAsync(confirmationEmoji); // Put this here so it happens before RunTurn() takes its time
                            var action = new PBEAction
                            {
                                Decision = PBEDecision.Fight,
                                FightMove = move,
                                PokemonId = mainPkmn.Id
                            };
                            PBEMoveTarget possibleTargets = mainPkmn.GetMoveTargets(move);
                            switch (possibleTargets)
                            {
                                case PBEMoveTarget.All:
                                    {
                                        action.FightTargets = PBETarget.AllyCenter | PBETarget.FoeCenter;
                                        break;
                                    }
                                case PBEMoveTarget.AllFoes:
                                case PBEMoveTarget.AllFoesSurrounding:
                                case PBEMoveTarget.AllSurrounding:
                                case PBEMoveTarget.RandomFoeSurrounding:
                                case PBEMoveTarget.SingleFoeSurrounding:
                                case PBEMoveTarget.SingleNotSelf:
                                case PBEMoveTarget.SingleSurrounding:
                                    {
                                        action.FightTargets = PBETarget.FoeCenter;
                                        break;
                                    }
                                case PBEMoveTarget.AllTeam:
                                case PBEMoveTarget.Self:
                                case PBEMoveTarget.SelfOrAllySurrounding:
                                case PBEMoveTarget.SingleAllySurrounding:
                                    {
                                        action.FightTargets = PBETarget.AllyCenter;
                                        break;
                                    }
                                default: throw new ArgumentOutOfRangeException(nameof(possibleTargets));
                            }

                            PBEBattle.SelectActionsIfValid(arp.Team, new[] { action });
                        }

                        PBEMove[] usableMoves = mainPkmn.GetUsableMoves();
                        for (int i = 0; i < usableMoves.Length; i++)
                        {
                            PBEMove move = usableMoves[i]; // move must be evaluated before it reaches the lambda
                            var emoji = new Emoji(char.ConvertFromUtf32(0x1F1E6 + i).ToString());
                            reactionsToAdd.Add(Tuple.Create(mainMsg, (IEmote)emoji));
                            ReactionListener.AddListener(mainMsg, allMessages, emoji, userArray, () => MoveReactionClicked(move));
                        }

                        // All listeners are added, so now we can send the reactions
                        // Clicking a reaction will not harm reactions that are not sent yet because allMessages is sent by reference
                        foreach (Tuple<IUserMessage, IEmote> tup in reactionsToAdd)
                        {
                            await tup.Item1.AddReactionAsync(tup.Item2);
                        }
                        break;
                    }
                case PBESwitchInRequestPacket sirp:
                    {
                        PBEPokemon[] switches = sirp.Team.Party.Where(p => p.HP > 0).ToArray();
                        if (switches.Length == 1)
                        {
                            PBEBattle.SelectSwitchesIfValid(sirp.Team, new Tuple<byte, PBEFieldPosition>[] { Tuple.Create(switches[0].Id, PBEFieldPosition.Center) });
                        }
                        else
                        {
                            async Task SwitchReactionClicked(IUserMessage switchMsg, PBEPokemon switchPkmn)
                            {
                                await switchMsg.AddReactionAsync(confirmationEmoji); // Put this here so it happens before RunTurn() takes its time
                                PBEBattle.SelectSwitchesIfValid(sirp.Team, new Tuple<byte, PBEFieldPosition>[] { Tuple.Create(switchPkmn.Id, PBEFieldPosition.Center) });
                            }

                            SocketUser user = context.battlers[Array.IndexOf(context.battle.Teams, sirp.Team)];
                            var userArray = new SocketUser[] { user };
                            var allMessages = new IUserMessage[switches.Length];
                            var reactionsToAdd = new Tuple<IUserMessage, IEmote>[switches.Length];
                            for (int i = 0; i < switches.Length; i++)
                            {
                                PBEPokemon switchPkmn = switches[i];
                                IUserMessage switchMsg = await CreateAndSendEmbedAsync(CustomPokemonToString(switchPkmn, false), messageText: i == 0 ? separator : string.Empty, pkmn: switchPkmn, useUpperImage: true, userToSendTo: user);
                                allMessages[i] = switchMsg;
                                reactionsToAdd[i] = Tuple.Create(switchMsg, (IEmote)switchEmoji);
                                ReactionListener.AddListener(switchMsg, allMessages, switchEmoji, userArray, () => SwitchReactionClicked(switchMsg, switchPkmn));
                            }

                            // All listeners are added, so now we can send the reactions
                            // Clicking a reaction will not harm reactions that are not sent yet because allMessages is sent by reference
                            for (int i = 0; i < reactionsToAdd.Length; i++)
                            {
                                Tuple<IUserMessage, IEmote> tup = reactionsToAdd[i];
                                await tup.Item1.AddReactionAsync(tup.Item2);
                            }
                        }
                        break;
                    }
                case PBETurnBeganPacket tbp:
                    {
                        await context.channel.SendMessageAsync(separator);
                        break;
                    }
            }
        }
    }
}
