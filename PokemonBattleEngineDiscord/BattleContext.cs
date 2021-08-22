using Discord;
using Discord.WebSocket;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Utils;
using Kermalis.PokemonBattleEngine.DefaultData.AI;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
    internal sealed partial class BattleContext
    {
        private sealed class Battler
        {
            public readonly PBETeam Team;
            public readonly PBETrainer Trainer;
            public readonly SocketUser? User; // Null means PBEAI is battling
            public readonly PBEDDAI? AI;

            public Battler(PBETeam t, SocketUser? u)
            {
                Team = t;
                Trainer = t.Trainers[0];
                User = u;
                if (u is null)
                {
                    AI = new PBEDDAI(Trainer);
                }
            }

            public string GetName()
            {
                return User is null ? Trainer.Name : User.Username;
            }
            public bool Is(SocketUser u)
            {
                return User is not null && User.Id == u.Id;
            }
        }

        private static readonly object _activeBattlesLockObj = new();
        private static readonly List<BattleContext> _activeBattles = new();
        private static readonly Dictionary<SocketUser, BattleContext> _activeBattlers = new(DiscordComparers.UserComparer);
        private static readonly Dictionary<ITextChannel, BattleContext> _activeChannels = new(DiscordComparers.ChannelComparer);
        private static readonly Dictionary<IGuild, List<BattleContext>> _activeGuilds = new(DiscordComparers.GuildComparer);
        private static ulong _battleCounter = 1;

        public readonly ulong BattleId;
        private readonly PBEBattle _battle;
        private readonly Battler _battler0;
        private readonly Battler _battler1;
        private ITextChannel _channel;
        private readonly StringBuilder _queuedMessages;

        public BattleContext(PBEBattle battle, SocketUser? battler0, SocketUser? battler1)
        {
            lock (_activeBattlesLockObj)
            {
                _activeBattles.Add(this);
                if (battler0 is not null)
                {
                    _activeBattlers.Add(battler0, this);
                }
                if (battler1 is not null)
                {
                    _activeBattlers.Add(battler1, this);
                }

                _queuedMessages = new StringBuilder();
                BattleId = _battleCounter++;
                _battle = battle;
                _battler0 = new Battler(battle.Teams[0], battler0);
                _battler1 = new Battler(battle.Teams[1], battler1);
                SetEmbedTitle();

                battle.OnNewEvent += Battle_OnNewEvent;
                battle.OnStateChanged += Battle_OnStateChanged;
            }
            _channel = null!; // _channel will be set in Begin()
        }
        public async Task Begin(ITextChannel channel)
        {
            lock (_activeBattlesLockObj)
            {
                _activeChannels.Add(channel, this);
                _channel = channel;
                IGuild guild = channel.Guild;
                if (!_activeGuilds.TryGetValue(guild, out List<BattleContext>? list))
                {
                    list = new List<BattleContext>();
                    _activeGuilds.Add(guild, list);
                }
                list.Add(this);
            }
            try
            {
                new Thread(() => _battle.Begin()).Start();
            }
            catch (Exception ex)
            {
                await CloseNormalWithException(ex);
            }
        }

        private void Battle_OnStateChanged(PBEBattle battle)
        {
            Battle_OnStateChanged().GetAwaiter().GetResult();
        }
        private void Battle_OnNewEvent(PBEBattle battle, IPBEPacket packet)
        {
            Battle_OnNewEvent(packet).GetAwaiter().GetResult();
        }

        public static void OnChannelDeleted(SocketChannel channel)
        {
            if (channel is ITextChannel c)
            {
                lock (_activeBattlesLockObj)
                {
                    if (_activeChannels.TryGetValue(c, out BattleContext? bc))
                    {
                        bc.CloseSilent(false);
                    }
                }
            }
        }
        public static void OnLeftGuild(SocketGuild guild)
        {
            lock (_activeBattlesLockObj)
            {
                if (_activeGuilds.TryGetValue(guild, out List<BattleContext>? list))
                {
                    foreach (BattleContext bc in list.ToArray()) // Prevent collection being modified in loop
                    {
                        bc.CloseSilent(false);
                    }
                    _activeGuilds.Remove(guild);
                }
            }
        }
        public static Task OnUserLeft(SocketGuildUser user)
        {
            lock (_activeBattlesLockObj)
            {
                if (_activeBattlers.TryGetValue(user, out BattleContext? bc))
                {
                    return bc.Forfeit(user);
                }
            }
            return Task.CompletedTask;
        }
        public static void OnGuildMemberUpdated(SocketGuildUser user)
        {
            lock (_activeBattlesLockObj)
            {
                if (_activeBattlers.TryGetValue(user, out BattleContext? bc))
                {
                    bc.SetEmbedTitle();
                }
            }
        }
        public static bool GetBattleContext(SocketUser user, [NotNullWhen(true)] out BattleContext? bc)
        {
            lock (_activeBattlesLockObj)
            {
                return _activeBattlers.TryGetValue(user, out bc);
            }
        }

        public async Task Forfeit(SocketUser user)
        {
            // Assumes "user" is not null (AI)
            await Forfeit(GetBattler(user));
        }
        private async Task Forfeit(Battler battler)
        {
            await CloseNormalWithMessage(string.Format("{0} has forfeited the match.", battler.GetName()), ReplaySaver.ShouldSaveForfeits, true);
        }
        private async Task CloseNormalWithException(Exception ex)
        {
            Console.WriteLine("Battle #{0} exception:{1}{2}", BattleId, Environment.NewLine, ex);
            await CloseNormalWithMessage(string.Format("Encountered an error, battle resulted in a draw. Error:\n{0}", ex.Message), false, false);
        }
        private async Task CloseNormalWithMessage(string message, bool saveReplay, bool deleteChannel)
        {
            await CreateAndSendEmbedAsync(message);
            if (deleteChannel)
            {
                await CloseNormalAndDelete(saveReplay);
            }
            else
            {
                await CloseNormal(saveReplay);
            }
        }
        private async Task CloseNormalAndDelete(bool saveReplay)
        {
            await CloseNormal(saveReplay);
            await ChannelHandler.MarkChannelForDeletion(_channel);
        }
        private async Task CloseNormal(bool saveReplay)
        {
            CloseSilent(saveReplay);
            await ChannelHandler.ChangeCategory(_channel);
        }
        private void CloseSilent(bool saveReplay)
        {
            SocketUser? u0 = _battler0.User;
            SocketUser? u1 = _battler1.User;
            lock (_activeBattlesLockObj)
            {
                _activeBattles.Remove(this);
                if (u0 is not null)
                {
                    _activeBattlers.Remove(u0);
                }
                if (u1 is not null)
                {
                    _activeBattlers.Remove(u1);
                }
                _activeChannels.Remove(_channel);
                _activeGuilds[_channel.Guild].Remove(this);
                // Only save replay if ((saveReplay is true) and (not battling an AI or (battling an AI and should save AI replays)))
                if (saveReplay && ((u0 is not null && u1 is not null) || ReplaySaver.ShouldSaveAIBattles))
                {
                    ReplaySaver.SaveReplay(_battle, BattleId); // Save battle in the lock so they don't conflict while directory checking
                }
            }
            _battle.OnNewEvent -= Battle_OnNewEvent;
            _battle.OnStateChanged -= Battle_OnStateChanged;
            ReactionHandler.RemoveListeners(u0, u1);
        }

        private string? _embedTitle; // Mini performance saver
        private void SetEmbedTitle()
        {
            string s = $"**[#{BattleId}] ― {_battler0.GetName()} vs {_battler1.GetName()}";
            if (_battle.TurnNumber > 0)
            {
                s += $" (Turn {_battle.TurnNumber})";
            }
            if (_battle.Weather != PBEWeather.None)
            {
                s += $" {Utils.WeatherEmotes[_battle.Weather]}";
            }
            _embedTitle = s + "**";
        }
        private ushort _lastSwitchinsTurn;
        private async Task SendQueuedMessages()
        {
            string str = _queuedMessages.ToString();
            _queuedMessages.Clear();
            int i = 0;
            do
            {
                int count = Math.Min(str.Length - i, EmbedBuilder.MaxDescriptionLength);
                string sub = str.Substring(i, count);
                await CreateAndSendEmbedAsync(sub);
                i += count;
            } while (i < str.Length);
        }
        private async Task SendActiveBattlerEmbeds()
        {
            bool b = true;
            foreach (PBEBattlePokemon pkmn in _battle.ActiveBattlers)
            {
                await CreateAndSendEmbedAsync(CreateKnownPokemonEmbed(pkmn), messageText: b ? Separator : string.Empty, pkmn: pkmn);
            }
        }
        private async Task<IUserMessage> CreateAndSendEmbedAsync(string embedDescription, string messageText = "",
            PBEBattlePokemon? pkmn = null, bool useUpperImage = false, EmbedFieldBuilder[]? fields = null, SocketUser? userToSendTo = null)
        {
            EmbedBuilder embed = new EmbedBuilder()
                .WithUrl(Utils.URL)
                .WithTitle(_embedTitle)
                .WithDescription(embedDescription);
            if (pkmn is null)
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
            if (fields is not null)
            {
                foreach (EmbedFieldBuilder f in fields)
                {
                    embed.AddField(f);
                }
            }
            if (userToSendTo is not null)
            {
                return await userToSendTo.SendMessageAsync(messageText, embed: embed.Build());
            }
            return await _channel.SendMessageAsync(messageText, embed: embed.Build());
        }
        private static void AddStatChanges(PBEBattlePokemon pkmn, StringBuilder sb)
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
        private static void CreatePokemonEmbed(PBEBattlePokemon pkmn, bool addReactionChars, out string outStr, out EmbedFieldBuilder[] outFields)
        {
            var sb = new StringBuilder();
            string formStr = PBEDataUtils.HasForms(pkmn.Species, false) ? $" ({PBEDataProvider.Instance.GetFormName(pkmn).English})" : string.Empty;
            sb.AppendLine($"{pkmn.Nickname}/{PBEDataProvider.Instance.GetSpeciesName(pkmn.Species).English}{formStr} {pkmn.Gender.ToSymbol()} Lv.{pkmn.Level}{(pkmn.Shiny ? $" {_shinyEmoji}" : string.Empty)}");
            sb.AppendLine($"**HP:** {pkmn.HP}/{pkmn.MaxHP} ({pkmn.HPPercentage:P2})");
            sb.Append($"**Types:** {Utils.TypeEmotes[pkmn.Type1]}");
            if (pkmn.Type2 != PBEType.None)
            {
                sb.Append($" {Utils.TypeEmotes[pkmn.Type2]}");
            }
            sb.AppendLine();
            if (pkmn.Status1 != PBEStatus1.None)
            {
                sb.AppendLine($"**Main status:** {Utils.Status1Emotes[pkmn.Status1]}");
            }
            if (pkmn.FieldPosition != PBEFieldPosition.None)
            {
                if (pkmn.Status1 == PBEStatus1.Asleep)
                {
                    sb.AppendLine($"**{Utils.Status1Emotes[PBEStatus1.Asleep]} turns:** {pkmn.Status1Counter}");
                }
                else if (pkmn.Status1 == PBEStatus1.BadlyPoisoned)
                {
                    sb.AppendLine($"**{Utils.Status1Emotes[PBEStatus1.BadlyPoisoned]} counter:** {pkmn.Status1Counter}");
                }
                if (pkmn.Status2 != PBEStatus2.None)
                {
                    sb.AppendLine($"**Volatile status:** {pkmn.Status2}");
                    if (pkmn.Status2.HasFlag(PBEStatus2.Confused))
                    {
                        sb.AppendLine($"**Confusion turns:** {pkmn.ConfusionCounter}");
                    }
                    if (pkmn.Status2.HasFlag(PBEStatus2.Disguised))
                    {
                        formStr = PBEDataUtils.HasForms(pkmn.KnownSpecies, false) ? $" ({PBEDataProvider.Instance.GetFormName(pkmn.KnownSpecies, pkmn.KnownForm).English})" : string.Empty;
                        sb.AppendLine($"**Disguised as:** {pkmn.KnownNickname}/{PBEDataProvider.Instance.GetSpeciesName(pkmn.KnownSpecies).English}{formStr} {pkmn.KnownGender.ToSymbol()}");
                    }
                    if (pkmn.Status2.HasFlag(PBEStatus2.Substitute))
                    {
                        sb.AppendLine($"**Substitute HP:** {pkmn.SubstituteHP}");
                    }
                }
            }
            sb.AppendLine($"**Stats:** [A] {pkmn.Attack}, [D] {pkmn.Defense}, [SA] {pkmn.SpAttack}, [SD] {pkmn.SpDefense}, [S] {pkmn.Speed}, [W] {pkmn.Weight:0.0}");
            if (pkmn.FieldPosition != PBEFieldPosition.None)
            {
                AddStatChanges(pkmn, sb);
            }
            sb.AppendLine($"**Ability:** {PBEDataProvider.Instance.GetAbilityName(pkmn.Ability).English}");
            sb.AppendLine($"**Item:** {PBEDataProvider.Instance.GetItemName(pkmn.Item).English}");
            if (pkmn.Moves.Contains(PBEMoveEffect.Frustration) || pkmn.Moves.Contains(PBEMoveEffect.Return))
            {
                sb.AppendLine($"**Friendship:** {pkmn.Friendship} ({pkmn.Friendship / (float)byte.MaxValue:P2})");
            }
            outStr = sb.ToString();
            sb.Clear();
            outFields = new EmbedFieldBuilder[2];
            EmbedFieldBuilder field = new EmbedFieldBuilder()
                .WithName("**Moves:**")
                .WithIsInline(true);
            outFields[0] = field;
            for (int i = 0; i < PBESettings.DefaultNumMoves; i++)
            {
                PBEBattleMoveset.PBEBattleMovesetSlot slot = pkmn.Moves[i];
                PBEMove move = slot.Move;
                if (move != PBEMove.None)
                {
                    PBEType moveType = pkmn.GetMoveType(move);
                    sb.Append($"{Utils.TypeEmotes[moveType]} {PBEDataProvider.Instance.GetMoveName(move).English} (**{slot.PP}**/**{slot.MaxPP}** **PP**)");
                    if (i < PBESettings.DefaultNumMoves - 1)
                    {
                        sb.AppendLine();
                    }
                }
            }
            field.WithValue(sb.ToString());
            sb.Clear();
            field = new EmbedFieldBuilder()
                .WithName("**Usable moves:**")
                .WithIsInline(true);
            outFields[1] = field;
            PBEMove[] usableMoves = pkmn.GetUsableMoves();
            for (int i = 0; i < usableMoves.Length; i++)
            {
                PBEMove move = usableMoves[i];
                PBEType moveType = pkmn.GetMoveType(move);
                if (addReactionChars)
                {
                    sb.Append($"{_moveEmotes[i][moveType]} ");
                }
                else
                {
                    sb.Append($"{Utils.TypeEmotes[moveType]} ");
                }
                sb.Append(PBEDataProvider.Instance.GetMoveName(move).English);
                IPBEMoveData mData = PBEDataProvider.Instance.GetMoveData(move);
                string powerStr;
                if (mData.Effect == PBEMoveEffect.HiddenPower)
                {
                    powerStr = pkmn.IndividualValues!.GetHiddenPowerBasePower(PBESettings.DefaultSettings).ToString();
                }
                else
                {
                    powerStr = mData.Power == 0 ? "―" : mData.Power.ToString();
                }
                sb.Append($" (**{powerStr}** | **{(mData.Accuracy == 0 ? "―" : mData.Accuracy.ToString() + '%')}** | **{mData.Category}**)");
                if (i < usableMoves.Length - 1)
                {
                    sb.AppendLine();
                }
            }
            field.WithValue(sb.ToString());
        }
        private string CreateKnownPokemonEmbed(PBEBattlePokemon pkmn)
        {
            IPBEPokemonData pData = PBEDataProvider.Instance.GetPokemonData(pkmn.KnownSpecies, pkmn.KnownForm);
            var sb = new StringBuilder();
            string formStr = PBEDataUtils.HasForms(pkmn.KnownSpecies, false) ? $" ({PBEDataProvider.Instance.GetFormName(pkmn.KnownSpecies, pkmn.KnownForm).English})" : string.Empty;
            sb.AppendLine($"{GetTrainerName(pkmn.Trainer)}'s {pkmn.KnownNickname}/{pkmn.KnownSpecies}{formStr} {(pkmn.KnownStatus2.HasFlag(PBEStatus2.Transformed) ? pkmn.Gender.ToSymbol() : pkmn.KnownGender.ToSymbol())} Lv.{pkmn.Level}{(pkmn.KnownShiny ? $" {_shinyEmoji}" : string.Empty)}");
            sb.AppendLine($"**HP:** {pkmn.HPPercentage:P2}");
            sb.Append($"**Known types:** {Utils.TypeEmotes[pkmn.KnownType1]}");
            if (pkmn.KnownType2 != PBEType.None)
            {
                sb.Append($" {Utils.TypeEmotes[pkmn.KnownType2]}");
            }
            sb.AppendLine();
            if (pkmn.Status1 != PBEStatus1.None)
            {
                sb.AppendLine($"**Main status:** {Utils.Status1Emotes[pkmn.Status1]}");
                if (pkmn.Status1 == PBEStatus1.Asleep)
                {
                    sb.AppendLine($"**{Utils.Status1Emotes[PBEStatus1.Asleep]} turns:** {pkmn.Status1Counter}");
                }
                else if (pkmn.Status1 == PBEStatus1.BadlyPoisoned)
                {
                    sb.AppendLine($"**{Utils.Status1Emotes[PBEStatus1.BadlyPoisoned]} counter:** {pkmn.Status1Counter}");
                }
            }
            if (pkmn.KnownStatus2 != PBEStatus2.None)
            {
                sb.AppendLine($"**Volatile status:** {pkmn.KnownStatus2}");
                if (pkmn.KnownStatus2.HasFlag(PBEStatus2.Confused))
                {
                    sb.AppendLine($"**Confusion turns:** {pkmn.ConfusionCounter}");
                }
            }
            PBEDataUtils.GetStatRange(pData, PBEStat.HP, pkmn.Level, PBESettings.DefaultSettings, out ushort lowHP, out ushort highHP);
            PBEDataUtils.GetStatRange(pData, PBEStat.Attack, pkmn.Level, PBESettings.DefaultSettings, out ushort lowAttack, out ushort highAttack);
            PBEDataUtils.GetStatRange(pData, PBEStat.Defense, pkmn.Level, PBESettings.DefaultSettings, out ushort lowDefense, out ushort highDefense);
            PBEDataUtils.GetStatRange(pData, PBEStat.SpAttack, pkmn.Level, PBESettings.DefaultSettings, out ushort lowSpAttack, out ushort highSpAttack);
            PBEDataUtils.GetStatRange(pData, PBEStat.SpDefense, pkmn.Level, PBESettings.DefaultSettings, out ushort lowSpDefense, out ushort highSpDefense);
            PBEDataUtils.GetStatRange(pData, PBEStat.Speed, pkmn.Level, PBESettings.DefaultSettings, out ushort lowSpeed, out ushort highSpeed);
            sb.AppendLine($"**Stat range:** [HP] {lowHP}-{highHP}, [A] {lowAttack}-{highAttack}, [D] {lowDefense}-{highDefense}, [SA] {lowSpAttack}-{highSpAttack}, [SD] {lowSpDefense}-{highSpDefense}, [S] {lowSpeed}-{highSpeed}, [W] {pkmn.KnownWeight:0.0}");
            AddStatChanges(pkmn, sb);
            if (pkmn.KnownAbility == PBEAbility.MAX)
            {
                sb.AppendLine($"**Possible abilities:** {string.Join(", ", pData.Abilities.Select(a => PBEDataProvider.Instance.GetAbilityName(a).English))}");
            }
            else
            {
                sb.AppendLine($"**Known ability:** {PBEDataProvider.Instance.GetAbilityName(pkmn.KnownAbility).English}");
            }
            sb.AppendLine($"**Known item:** {(pkmn.KnownItem == (PBEItem)ushort.MaxValue ? "???" : PBEDataProvider.Instance.GetItemName(pkmn.KnownItem).English)}");
            sb.Append("**Known moves:** ");
            for (int i = 0; i < PBESettings.DefaultNumMoves; i++)
            {
                PBEBattleMoveset.PBEBattleMovesetSlot slot = pkmn.KnownMoves[i];
                PBEMove move = slot.Move;
                if (move != PBEMove.None)
                {
                    int pp = slot.PP;
                    int maxPP = slot.MaxPP;
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    if (move == PBEMove.MAX)
                    {
                        sb.Append("???");
                    }
                    else
                    {
                        sb.Append($"{Utils.TypeEmotes[pkmn.GetMoveType(move, useKnownInfo: true)]} {PBEDataProvider.Instance.GetMoveName(move).English} ({pp}{(maxPP == 0 ? ")" : $"/{maxPP})")}");
                    }
                }
            }
            return sb.ToString();
        }
        private Battler GetBattler(SocketUser battler)
        {
            if (_battler0.Is(battler))
            {
                return _battler0;
            }
            if (_battler1.Is(battler))
            {
                return _battler1;
            }
            throw new ArgumentOutOfRangeException(nameof(battler));
        }
        private Battler GetBattler(PBETrainer trainer)
        {
            if (trainer.Id == 0)
            {
                return _battler0;
            }
            if (trainer.Id == 1)
            {
                return _battler1;
            }
            throw new ArgumentOutOfRangeException(nameof(trainer));
        }
        // These two are passed to the message handler
        private string GetTrainerName(PBETrainer trainer)
        {
            return GetBattler(trainer).GetName();
        }
        private string GetTeamName(PBETeam team, bool _)
        {
            return GetTrainerName(team.Trainers[0]);
        }

        private async Task Battle_OnStateChanged()
        {
            switch (_battle.BattleState)
            {
                case PBEBattleState.Ended:
                {
                    await CloseNormalAndDelete(true);
                    break;
                }
                case PBEBattleState.ReadyToRunSwitches:
                {
                    try
                    {
                        new Thread(() => _battle.RunSwitches()).Start();
                    }
                    catch (Exception ex)
                    {
                        await CloseNormalWithException(ex);
                    }
                    break;
                }
                case PBEBattleState.ReadyToRunTurn:
                {
                    try
                    {
                        new Thread(() => _battle.RunTurn()).Start();
                    }
                    catch (Exception ex)
                    {
                        await CloseNormalWithException(ex);
                    }
                    break;
                }
            }
        }
        private async Task Battle_OnNewEvent(IPBEPacket packet)
        {
            // Packets that need extra logic and/or should not have a default message
            switch (packet)
            {
                case PBEMovePPChangedPacket _: return;
                case PBEActionsRequestPacket arp:
                {
                    PBETrainer trainer = arp.Trainer;
                    Battler battler = GetBattler(trainer);
                    SocketUser? user = battler.User;
                    if (user is null) // PBEAI
                    {
                        battler.AI!.CreateActions();
                        return;
                    }
                    PBEBattlePokemon mainPkmn = trainer.ActionsRequired[0];
                    var reactionsToAdd = new List<(IUserMessage Message, IEmote Reaction)>(PBESettings.DefaultMaxPartySize - 1 + PBESettings.DefaultNumMoves); // 5 switch reactions, 4 move reactions
                    string description;
                    EmbedFieldBuilder[] fields;

                    if (mainPkmn.CanSwitchOut())
                    {
                        async Task SwitchReactionClicked(IUserMessage switchMsg, PBEBattlePokemon switchPkmn)
                        {
                            await switchMsg.AddReactionAsync(_confirmationEmoji); // Put this here so it happens before RunTurn() takes its time
                            trainer.SelectActionsIfValid(out _, new PBETurnAction(mainPkmn, switchPkmn));
                        }

                        List<PBEBattlePokemon> switches = trainer.Party.FindAll(p => p != mainPkmn && p.CanBattle);
                        for (int i = 0; i < switches.Count; i++)
                        {
                            PBEBattlePokemon switchPkmn = switches[i];
                            CreatePokemonEmbed(switchPkmn, false, out description, out fields);
                            IUserMessage switchMsg = await CreateAndSendEmbedAsync(description, messageText: i == 0 ? Separator : string.Empty, pkmn: switchPkmn, useUpperImage: true, fields: fields, userToSendTo: user);
                            reactionsToAdd.Add((switchMsg, _switchEmoji));
                            ReactionHandler.AddListener(user, switchMsg, _switchEmoji, () => SwitchReactionClicked(switchMsg, switchPkmn));
                        }
                    }

                    CreatePokemonEmbed(mainPkmn, true, out description, out fields);
                    IUserMessage mainMsg = await CreateAndSendEmbedAsync($"{description}\nTo check a move: `!move info {PBEDataProvider.Instance.GetMoveName(Utils.RandomElement(PBEDataUtils.AllMoves)).English}`", pkmn: mainPkmn, useUpperImage: false, fields: fields, userToSendTo: user);

                    async Task MoveReactionClicked(PBEMove move)
                    {
                        await mainMsg.AddReactionAsync(_confirmationEmoji); // Put this here so it happens before RunTurn() takes its time
                        PBEMoveTarget possibleTargets = mainPkmn.GetMoveTargets(move);
                        PBETurnTarget targets;
                        switch (possibleTargets)
                        {
                            case PBEMoveTarget.All:
                            {
                                targets = PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter;
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
                                targets = PBETurnTarget.FoeCenter;
                                break;
                            }
                            case PBEMoveTarget.AllTeam:
                            case PBEMoveTarget.Self:
                            case PBEMoveTarget.SelfOrAllySurrounding:
                            case PBEMoveTarget.SingleAllySurrounding:
                            {
                                targets = PBETurnTarget.AllyCenter;
                                break;
                            }
                            default: throw new InvalidDataException(nameof(possibleTargets));
                        }

                        trainer.SelectActionsIfValid(out _, new PBETurnAction(mainPkmn, move, targets));
                    }

                    PBEMove[] usableMoves = mainPkmn.GetUsableMoves();
                    for (int i = 0; i < usableMoves.Length; i++)
                    {
                        PBEMove move = usableMoves[i]; // move must be evaluated before it reaches the lambda
                        Emote emoji = _moveEmotes[i][mainPkmn.GetMoveType(move)];
                        reactionsToAdd.Add((mainMsg, emoji));
                        ReactionHandler.AddListener(user, mainMsg, emoji, () => MoveReactionClicked(move));
                    }

                    // All listeners are added, so now we can send the reactions
                    foreach ((IUserMessage Message, IEmote Reaction) in reactionsToAdd)
                    {
                        await Message.AddReactionAsync(Reaction);
                    }
                    return;
                }
                case PBESwitchInRequestPacket sirp:
                {
                    PBETrainer trainer = sirp.Trainer;
                    List<PBEBattlePokemon> switches = trainer.Party.FindAll(p => p.CanBattle);
                    if (switches.Count == 1)
                    {
                        trainer.SelectSwitchesIfValid(out _, new PBESwitchIn(switches[0], PBEFieldPosition.Center));
                        return;
                    }
                    Battler battler = GetBattler(trainer);
                    SocketUser? user = battler.User;
                    if (user is null) // PBEAI
                    {
                        battler.AI!.CreateAISwitches();
                        return;
                    }
                    ushort curTurn = _battle.TurnNumber;
                    if (_lastSwitchinsTurn != curTurn)
                    {
                        await SendQueuedMessages();
                        await SendActiveBattlerEmbeds();
                        _lastSwitchinsTurn = curTurn;
                    }

                    async Task SwitchReactionClicked(IUserMessage switchMsg, PBEBattlePokemon switchPkmn)
                    {
                        await switchMsg.AddReactionAsync(_confirmationEmoji); // Put this here so it happens before RunTurn() takes its time
                        trainer.SelectSwitchesIfValid(out _, new PBESwitchIn(switchPkmn, PBEFieldPosition.Center));
                    }

                    var reactionsToAdd = new (IUserMessage Message, IEmote Reaction)[switches.Count];
                    for (int i = 0; i < switches.Count; i++)
                    {
                        PBEBattlePokemon switchPkmn = switches[i];
                        CreatePokemonEmbed(switchPkmn, false, out string description, out EmbedFieldBuilder[] fields);
                        IUserMessage switchMsg = await CreateAndSendEmbedAsync(description, messageText: i == 0 ? Separator : string.Empty, pkmn: switchPkmn, useUpperImage: true, fields: fields, userToSendTo: user);
                        reactionsToAdd[i] = (switchMsg, _switchEmoji);
                        ReactionHandler.AddListener(user, switchMsg, _switchEmoji, () => SwitchReactionClicked(switchMsg, switchPkmn));
                    }

                    // All listeners are added, so now we can send the reactions
                    for (int i = 0; i < reactionsToAdd.Length; i++)
                    {
                        (IUserMessage Message, IEmote Reaction) = reactionsToAdd[i];
                        await Message.AddReactionAsync(Reaction);
                    }
                    return;
                }
                case PBEWeatherPacket wp:
                {
                    if (wp.WeatherAction == PBEWeatherAction.Added || wp.WeatherAction == PBEWeatherAction.Ended)
                    {
                        SetEmbedTitle();
                    }
                    break; // Continue to the default message
                }
                case PBETurnBeganPacket tbp:
                {
                    await SendQueuedMessages();
                    SetEmbedTitle();
                    await SendActiveBattlerEmbeds();
                    return;
                }
                case PBEBattleResultPacket brp: // We do not want the default message since it uses the combined name
                {
                    string m;
                    switch (brp.BattleResult)
                    {
                        case PBEBattleResult.Team0Win: m = "{0} defeated {1}!"; break;
                        case PBEBattleResult.Team1Win: m = "{1} defeated {0}!"; break;
                        default: throw new InvalidDataException(nameof(brp.BattleResult));
                    }
                    _queuedMessages.AppendLine(string.Format(m, _battler0.GetName(), _battler1.GetName()));
                    await SendQueuedMessages();
                    return;
                }
            }
            // Get default message
            string? message = PBEBattle.GetDefaultMessage(_battle, packet, trainerNameFunc: GetTrainerName, teamNameFunc: GetTeamName);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            _queuedMessages.AppendLine(message);
        }
    }
}
