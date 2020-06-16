using Discord;
using Discord.WebSocket;
using Kermalis.PokemonBattleEngine.AI;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
    internal sealed class BattleContext
    {
        #region Constants
        private const string Separator = "**--------------------**";
        private static readonly Emoji _shinyEmoji = new Emoji("✨");
        private static readonly Emoji _switchEmoji = new Emoji("😼");
        private static readonly Emoji _confirmationEmoji = new Emoji("👍");
        private static readonly Dictionary<PBEType, Emote>[] _moveEmotes = new Dictionary<PBEType, Emote>[4]
        {
            new Dictionary<PBEType, Emote>
            {
                { PBEType.None, Emote.Parse("<:Normal1:708768399538520095>") },
                { PBEType.Bug, Emote.Parse("<:Bug1:708768297889300503>") },
                { PBEType.Dark, Emote.Parse("<:Dark1:708768298732355667>") },
                { PBEType.Dragon, Emote.Parse("<:Dragon1:708768299235672084>") },
                { PBEType.Electric, Emote.Parse("<:Electric1:708768298103341073>") },
                { PBEType.Fighting, Emote.Parse("<:Fighting1:708768297457549472>") },
                { PBEType.Fire, Emote.Parse("<:Fire1:708768298782818394>") },
                { PBEType.Flying, Emote.Parse("<:Flying1:708768298841669713>") },
                { PBEType.Ghost, Emote.Parse("<:Ghost1:708768298598400081>") },
                { PBEType.Grass, Emote.Parse("<:Grass1:708768299219025950>") },
                { PBEType.Ground, Emote.Parse("<:Ground1:708768298317119548>") },
                { PBEType.Ice, Emote.Parse("<:Ice1:708768397214744627>") },
                { PBEType.Normal, Emote.Parse("<:Normal1:708768399538520095>") },
                { PBEType.Poison, Emote.Parse("<:Poison1:708768399450308680>") },
                { PBEType.Psychic, Emote.Parse("<:Psychic1:708768399915876462>") },
                { PBEType.Rock, Emote.Parse("<:Rock1:708768399836315670>") },
                { PBEType.Steel, Emote.Parse("<:Steel1:708768399400108083>") },
                { PBEType.Water, Emote.Parse("<:Water1:708768400389963877>") }
            },
            new Dictionary<PBEType, Emote>
            {
                { PBEType.None, Emote.Parse("<:Normal2:708768399496314880>") },
                { PBEType.Bug, Emote.Parse("<:Bug2:708768298665246791>") },
                { PBEType.Dark, Emote.Parse("<:Dark2:708768298992533586>") },
                { PBEType.Dragon, Emote.Parse("<:Dragon2:708768298883350570>") },
                { PBEType.Electric, Emote.Parse("<:Electric2:708768297960603708>") },
                { PBEType.Fighting, Emote.Parse("<:Fighting2:708768297654681620>") },
                { PBEType.Fire, Emote.Parse("<:Fire2:708768298870767616>") },
                { PBEType.Flying, Emote.Parse("<:Flying2:708768298921099325>") },
                { PBEType.Ghost, Emote.Parse("<:Ghost2:708768299072356402>") },
                { PBEType.Grass, Emote.Parse("<:Grass2:708768298900127845>") },
                { PBEType.Ground, Emote.Parse("<:Ground2:708768298338353232>") },
                { PBEType.Ice, Emote.Parse("<:Ice2:708768397336510534>") },
                { PBEType.Normal, Emote.Parse("<:Normal2:708768399496314880>") },
                { PBEType.Poison, Emote.Parse("<:Poison2:708768399647440907>") },
                { PBEType.Psychic, Emote.Parse("<:Psychic2:708768399441788938>") },
                { PBEType.Rock, Emote.Parse("<:Rock2:708768399442051114>") },
                { PBEType.Steel, Emote.Parse("<:Steel2:708768399873933325>") },
                { PBEType.Water, Emote.Parse("<:Water2:708768398829682759>") }
            },
            new Dictionary<PBEType, Emote>
            {
                { PBEType.None, Emote.Parse("<:Normal3:708768399404302426>") },
                { PBEType.Bug, Emote.Parse("<:Bug3:708768298958979233>") },
                { PBEType.Dark, Emote.Parse("<:Dark3:708768298690674739>") },
                { PBEType.Dragon, Emote.Parse("<:Dragon3:708768300045434911>") },
                { PBEType.Electric, Emote.Parse("<:Electric3:708768298057203823>") },
                { PBEType.Fighting, Emote.Parse("<:Fighting3:708768297679847474>") },
                { PBEType.Fire, Emote.Parse("<:Fire3:708768298744938546>") },
                { PBEType.Flying, Emote.Parse("<:Flying3:708768298480828557>") },
                { PBEType.Ghost, Emote.Parse("<:Ghost3:708768298845601822>") },
                { PBEType.Grass, Emote.Parse("<:Grass3:708768298858184785>") },
                { PBEType.Ground, Emote.Parse("<:Ground3:708768298312925244>") },
                { PBEType.Ice, Emote.Parse("<:Ice3:708768396644450355>") },
                { PBEType.Normal, Emote.Parse("<:Normal3:708768399404302426>") },
                { PBEType.Poison, Emote.Parse("<:Poison3:708768399681126420>") },
                { PBEType.Psychic, Emote.Parse("<:Psychic3:708768401123836015>") },
                { PBEType.Rock, Emote.Parse("<:Rock3:708768399274016838>") },
                { PBEType.Steel, Emote.Parse("<:Steel3:708768399383330836>") },
                { PBEType.Water, Emote.Parse("<:Water3:708768399936978977>") }
            },
            new Dictionary<PBEType, Emote>
            {
                { PBEType.None, Emote.Parse("<:Normal4:708768399332999240>") },
                { PBEType.Bug, Emote.Parse("<:Bug4:708768298883612792>") },
                { PBEType.Dark, Emote.Parse("<:Dark4:708768298665508906>") },
                { PBEType.Dragon, Emote.Parse("<:Dragon4:708768298627498066>") },
                { PBEType.Electric, Emote.Parse("<:Electric4:708768297918660698>") },
                { PBEType.Fighting, Emote.Parse("<:Fighting4:708768297532915785>") },
                { PBEType.Fire, Emote.Parse("<:Fire4:708768298380034152>") },
                { PBEType.Flying, Emote.Parse("<:Flying4:708768298795270245>") },
                { PBEType.Ghost, Emote.Parse("<:Ghost4:708768298841669672>") },
                { PBEType.Grass, Emote.Parse("<:Grass4:708768298875093084>") },
                { PBEType.Ground, Emote.Parse("<:Ground4:708768298468114512>") },
                { PBEType.Ice, Emote.Parse("<:Ice4:708768398821163009>") },
                { PBEType.Normal, Emote.Parse("<:Normal4:708768399332999240>") },
                { PBEType.Poison, Emote.Parse("<:Poison4:708768400020602910>") },
                { PBEType.Psychic, Emote.Parse("<:Psychic4:708768399328673803>") },
                { PBEType.Rock, Emote.Parse("<:Rock4:708768399345451009>") },
                { PBEType.Steel, Emote.Parse("<:Steel4:708768399161032735>") },
                { PBEType.Water, Emote.Parse("<:Water4:708768398691139656>") }
            }
        };
        #endregion
        private static readonly object _activeBattlesLockObj = new object();
        private static readonly List<BattleContext> _activeBattles = new List<BattleContext>();
        private static readonly Dictionary<SocketUser, BattleContext> _activeBattlers = new Dictionary<SocketUser, BattleContext>(DiscordComparers.UserComparer);
        private static readonly Dictionary<ITextChannel, BattleContext> _activeChannels = new Dictionary<ITextChannel, BattleContext>(DiscordComparers.ChannelComparer);
        private static readonly Dictionary<IGuild, List<BattleContext>> _activeGuilds = new Dictionary<IGuild, List<BattleContext>>(DiscordComparers.GuildComparer);
        private static ulong _battleCounter = 1;

        public readonly ulong BattleId;
        private readonly PBEBattle _battle;
        private readonly SocketUser _battler0; // Null means PBEAI is battling
        private readonly SocketUser _battler1; // Null means PBEAI is battling
        private ITextChannel _channel;
        private readonly StringBuilder _queuedMessages;

        public BattleContext(PBEBattle battle, SocketUser battler0, SocketUser battler1)
        {
            lock (_activeBattlesLockObj)
            {
                _activeBattles.Add(this);
                if (battler0 != null)
                {
                    _activeBattlers.Add(battler0, this);
                }
                if (battler1 != null)
                {
                    _activeBattlers.Add(battler1, this);
                }

                _queuedMessages = new StringBuilder();
                BattleId = _battleCounter++;
                _battle = battle;
                _battler0 = battler0;
                _battler1 = battler1;
                SetEmbedTitle();

                battle.OnNewEvent += Battle_OnNewEvent;
                battle.OnStateChanged += Battle_OnStateChanged;
            }
        }
        public async Task Begin(ITextChannel channel)
        {
            lock (_activeBattlesLockObj)
            {
                _activeChannels.Add(channel, this);
                _channel = channel;
                IGuild guild = channel.Guild;
                if (!_activeGuilds.TryGetValue(guild, out List<BattleContext> list))
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
                    if (_activeChannels.TryGetValue(c, out BattleContext bc))
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
                if (_activeGuilds.TryGetValue(guild, out List<BattleContext> list))
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
                if (_activeBattlers.TryGetValue(user, out BattleContext bc))
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
                if (_activeBattlers.TryGetValue(user, out BattleContext bc))
                {
                    SocketUser b0 = bc._battler0;
                    bc._battle.Teams[b0 != null && b0.Id == user.Id ? 0 : 1].TrainerName = user.Username;
                    bc.SetEmbedTitle();
                }
            }
        }
        public static BattleContext GetBattleContext(SocketUser user)
        {
            lock (_activeBattlesLockObj)
            {
                return _activeBattlers.TryGetValue(user, out BattleContext bc) ? bc : null;
            }
        }

        public async Task Forfeit(SocketUser user)
        {
            // Assumes "user" is not null (AI)
            await Forfeit(user.Id == _battler0.Id ? 0 : 1);
        }
        private async Task Forfeit(int teamId)
        {
            await CloseNormalWithMessage(string.Format("{0} has forfeited the match.", _battle.Teams[teamId].TrainerName), ReplaySaver.ShouldSaveForfeits, true);
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
            lock (_activeBattlesLockObj)
            {
                _activeBattles.Remove(this);
                if (_battler0 != null)
                {
                    _activeBattlers.Remove(_battler0);
                }
                if (_battler1 != null)
                {
                    _activeBattlers.Remove(_battler1);
                }
                _activeChannels.Remove(_channel);
                _activeGuilds[_channel.Guild].Remove(this);
                // Only save replay if ((saveReplay is true) and (not battling an AI or (battling an AI and should save AI replays)))
                if (saveReplay && ((_battler0 != null && _battler1 != null) || ReplaySaver.ShouldSaveAIBattles))
                {
                    ReplaySaver.SaveReplay(_battle, BattleId); // Save battle in the lock so they don't conflict while directory checking
                }
            }
            _battle.OnNewEvent -= Battle_OnNewEvent;
            _battle.OnStateChanged -= Battle_OnStateChanged;
            ReactionHandler.RemoveListeners(_battler0, _battler1);
        }

        private string _embedTitle; // Mini performance saver
        private void SetEmbedTitle()
        {
            _embedTitle = $"[#{BattleId}] ― {_battle.Teams[0].TrainerName} vs {_battle.Teams[1].TrainerName}";
            if (_battle.TurnNumber > 0)
            {
                _embedTitle += $" (Turn {_battle.TurnNumber})";
            }
            if (_battle.Weather != PBEWeather.None)
            {
                _embedTitle += $" {Utils.WeatherEmotes[_battle.Weather]}";
            }
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
        private async Task<IUserMessage> CreateAndSendEmbedAsync(string embedDescription, string messageText = "", PBEBattlePokemon pkmn = null, bool useUpperImage = false, EmbedFieldBuilder[] fields = null, SocketUser userToSendTo = null)
        {
            EmbedBuilder embed = new EmbedBuilder()
                .WithUrl(Utils.URL)
                .WithTitle(_embedTitle)
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
            if (fields != null)
            {
                foreach (EmbedFieldBuilder f in fields)
                {
                    embed.AddField(f);
                }
            }
            if (userToSendTo != null)
            {
                return await userToSendTo.SendMessageAsync(messageText, embed: embed.Build());
            }
            else
            {
                return await _channel.SendMessageAsync(messageText, embed: embed.Build());
            }
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
            string formStr = PBEDataUtils.HasForms(pkmn.Species, false) ? $" ({PBELocalizedString.GetFormName(pkmn.Species, pkmn.Form)})" : string.Empty;
            sb.AppendLine($"{pkmn.Nickname}/{pkmn.Species}{formStr} {pkmn.GenderSymbol} Lv.{pkmn.Level}{(pkmn.Shiny ? $" {_shinyEmoji}" : string.Empty)}");
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
                        sb.AppendLine($"**Disguised as:** {pkmn.DisguisedAsPokemon.Nickname}");
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
            sb.AppendLine($"**Ability:** {PBELocalizedString.GetAbilityName(pkmn.Ability).English}");
            sb.AppendLine($"**Item:** {PBELocalizedString.GetItemName(pkmn.Item).English}");
            if (pkmn.Moves.Contains(PBEMove.Frustration) || pkmn.Moves.Contains(PBEMove.Return))
            {
                sb.AppendLine($"**Friendship:** {pkmn.Friendship} ({pkmn.Friendship / (double)byte.MaxValue:P2})");
            }
            if (pkmn.Moves.Contains(PBEMove.HiddenPower))
            {
                sb.AppendLine($"**{PBELocalizedString.GetMoveName(PBEMove.HiddenPower).English}:** {Utils.TypeEmotes[pkmn.IndividualValues.GetHiddenPowerType()]}|{pkmn.IndividualValues.GetHiddenPowerBasePower(PBESettings.DefaultSettings)}");
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
                    sb.Append($"{Utils.TypeEmotes[moveType]} {PBELocalizedString.GetMoveName(move).English} ({slot.PP}/{slot.MaxPP})");
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
                sb.Append(PBELocalizedString.GetMoveName(move).English);
                if (i < usableMoves.Length - 1)
                {
                    sb.AppendLine();
                }
            }
            field.WithValue(sb.ToString());
        }
        private static string CreateKnownPokemonEmbed(PBEBattlePokemon pkmn)
        {
            var pData = PBEPokemonData.GetData(pkmn.KnownSpecies, pkmn.KnownForm);
            var sb = new StringBuilder();
            string formStr = PBEDataUtils.HasForms(pkmn.KnownSpecies, false) ? $" ({PBELocalizedString.GetFormName(pkmn.KnownSpecies, pkmn.KnownForm)})" : string.Empty;
            sb.AppendLine($"{pkmn.Team.TrainerName}'s {pkmn.KnownNickname}/{pkmn.KnownSpecies}{formStr} {(pkmn.KnownStatus2.HasFlag(PBEStatus2.Transformed) ? pkmn.GenderSymbol : pkmn.KnownGenderSymbol)} Lv.{pkmn.Level}{(pkmn.KnownShiny ? $" {_shinyEmoji}" : string.Empty)}");
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
                sb.AppendLine($"**Possible abilities:** {string.Join(", ", pData.Abilities.Select(a => PBELocalizedString.GetAbilityName(a).English))}");
            }
            else
            {
                sb.AppendLine($"**Known ability:** {PBELocalizedString.GetAbilityName(pkmn.KnownAbility).English}");
            }
            sb.AppendLine($"**Known item:** {(pkmn.KnownItem == (PBEItem)ushort.MaxValue ? "???" : PBELocalizedString.GetItemName(pkmn.KnownItem).English)}");
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
                        sb.Append($"{Utils.TypeEmotes[pkmn.GetMoveType(move, useKnownInfo: true)]} {PBELocalizedString.GetMoveName(move).English} ({pp}{(maxPP == 0 ? ")" : $"/{maxPP})")}");
                    }
                }
            }
            return sb.ToString();
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
            static string NameForTrainer(PBEBattlePokemon pkmn)
            {
                return pkmn == null ? string.Empty : $"{pkmn.Team.TrainerName}'s {pkmn.KnownNickname}";
            }

            switch (packet)
            {
                case PBEAbilityPacket ap:
                {
                    PBEBattlePokemon abilityOwner = ap.AbilityOwnerTeam.TryGetPokemon(ap.AbilityOwner),
                            pokemon2 = ap.Pokemon2Team.TryGetPokemon(ap.Pokemon2);
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
                                case PBEAbilityAction.Damage: message = "{1} is tormented by {0}'s {2}!"; break;
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
                        case PBEAbility.IceBody:
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
                                case PBEAbilityAction.ChangedAppearance: return;
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
                    _queuedMessages.AppendLine(string.Format(message, NameForTrainer(abilityOwner), NameForTrainer(pokemon2), PBELocalizedString.GetAbilityName(ap.Ability).English));
                    break;
                }
                case PBEAbilityReplacedPacket arp:
                {
                    PBEBattlePokemon abilityOwner = arp.AbilityOwnerTeam.TryGetPokemon(arp.AbilityOwner);
                    string message;
                    switch (arp.NewAbility)
                    {
                        case PBEAbility.None: message = "{0}'s {1} was suppressed!"; break;
                        default: message = "{0}'s {1} was changed to {2}!"; break;
                    }
                    _queuedMessages.AppendLine(string.Format(message, NameForTrainer(abilityOwner), arp.OldAbility.HasValue ? PBELocalizedString.GetAbilityName(arp.OldAbility.Value).English : "Ability", PBELocalizedString.GetAbilityName(arp.NewAbility).English));
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
                    _queuedMessages.AppendLine(message);
                    break;
                }
                case PBEHazePacket _:
                {
                    _queuedMessages.AppendLine("All stat changes were eliminated!");
                    break;
                }
                case PBEItemPacket ip:
                {
                    PBEBattlePokemon itemHolder = ip.ItemHolderTeam.TryGetPokemon(ip.ItemHolder),
                            pokemon2 = ip.Pokemon2Team.TryGetPokemon(ip.Pokemon2);
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
                    _queuedMessages.AppendLine(string.Format(message, NameForTrainer(itemHolder), NameForTrainer(pokemon2), PBELocalizedString.GetItemName(ip.Item).English));
                    break;
                }
                case PBEMoveCritPacket mcp:
                {
                    PBEBattlePokemon victim = mcp.VictimTeam.TryGetPokemon(mcp.Victim);
                    _queuedMessages.AppendLine(string.Format("A critical hit on {0}!", NameForTrainer(victim)));
                    break;
                }
                case PBEMoveMissedPacket mmp:
                {
                    PBEBattlePokemon moveUser = mmp.MoveUserTeam.TryGetPokemon(mmp.MoveUser),
                            pokemon2 = mmp.Pokemon2Team.TryGetPokemon(mmp.Pokemon2);
                    _queuedMessages.AppendLine(string.Format("{0}'s attack missed {1}!", NameForTrainer(moveUser), NameForTrainer(pokemon2)));
                    break;
                }
                case PBEMoveResultPacket mrp:
                {
                    PBEBattlePokemon moveUser = mrp.MoveUserTeam.TryGetPokemon(mrp.MoveUser),
                            pokemon2 = mrp.Pokemon2Team.TryGetPokemon(mrp.Pokemon2);
                    string message;
                    switch (mrp.Result)
                    {
                        case PBEResult.Ineffective_Ability: message = "{1} is protected by its Ability!"; break;
                        case PBEResult.Ineffective_Gender: message = "It doesn't affect {1}..."; break;
                        case PBEResult.Ineffective_Level: message = "{1} is protected by its level!"; break;
                        case PBEResult.Ineffective_MagnetRise: message = $"{{1}} is protected by {PBELocalizedString.GetMoveName(PBEMove.MagnetRise).English}!"; break;
                        case PBEResult.Ineffective_Safeguard: message = $"{{1}} is protected by {PBELocalizedString.GetMoveName(PBEMove.Safeguard).English}!"; break;
                        case PBEResult.Ineffective_Stat:
                        case PBEResult.Ineffective_Status:
                        case PBEResult.InvalidConditions: message = "But it failed!"; break;
                        case PBEResult.Ineffective_Substitute: message = $"{{1}} is protected by {PBELocalizedString.GetMoveName(PBEMove.Substitute).English}!"; break;
                        case PBEResult.Ineffective_Type: message = "{1} is protected by its Type!"; break;
                        case PBEResult.NoTarget: message = "But there was no target..."; break;
                        case PBEResult.NotVeryEffective_Type: message = "It's not very effective on {1}..."; break;
                        case PBEResult.SuperEffective_Type: message = "It's super effective on {1}!"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(mrp.Result));
                    }
                    _queuedMessages.AppendLine(string.Format(message, NameForTrainer(moveUser), NameForTrainer(pokemon2)));
                    break;
                }
                case PBEMoveUsedPacket mup:
                {
                    PBEBattlePokemon moveUser = mup.MoveUserTeam.TryGetPokemon(mup.MoveUser);
                    _queuedMessages.AppendLine(string.Format("{0} used {1}!", NameForTrainer(moveUser), PBELocalizedString.GetMoveName(mup.Move).English));
                    break;
                }
                case PBEPkmnFaintedPacket pfap:
                {
                    PBEBattlePokemon pokemon = pfap.PokemonTeam.TryGetPokemon(pfap.PokemonId);
                    _queuedMessages.AppendLine(string.Format("{0} fainted!", NameForTrainer(pokemon)));
                    break;
                }
                case PBEPkmnFormChangedPacket pfcp:
                {
                    PBEBattlePokemon pokemon = pfcp.PokemonTeam.TryGetPokemon(pfcp.Pokemon);
                    _queuedMessages.AppendLine(string.Format("{0}'s new form is {1}!", NameForTrainer(pokemon), PBELocalizedString.GetFormName(pokemon.Species, pfcp.NewForm).English));
                    break;
                }
                case PBEPkmnHPChangedPacket phcp:
                {
                    PBEBattlePokemon pokemon = phcp.PokemonTeam.TryGetPokemon(phcp.Pokemon);
                    int change = phcp.NewHP - phcp.OldHP;
                    int absChange = Math.Abs(change);
                    double percentageChange = phcp.NewHPPercentage - phcp.OldHPPercentage;
                    double absPercentageChange = Math.Abs(percentageChange);
                    _queuedMessages.AppendLine(string.Format("{0} {1} {2:P2} of its HP!", NameForTrainer(pokemon), percentageChange <= 0 ? "lost" : "restored", absPercentageChange));
                    break;
                }
                case PBEPkmnStatChangedPacket pscp:
                {
                    PBEBattlePokemon pokemon = pscp.PokemonTeam.TryGetPokemon(pscp.Pokemon);
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
                            if (change == 0 && pscp.NewValue == -PBESettings.DefaultMaxStatChange)
                            {
                                message = "won't go lower";
                            }
                            else if (change == 0 && pscp.NewValue == PBESettings.DefaultMaxStatChange)
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
                    _queuedMessages.AppendLine(string.Format("{0}'s {1} {2}!", NameForTrainer(pokemon), statName, message));
                    break;
                }
                case PBEPkmnSwitchInPacket psip:
                {
                    if (!psip.Forced)
                    {
                        foreach (PBEPkmnSwitchInPacket.PBESwitchInInfo info in psip.SwitchIns)
                        {
                            PBEBattlePokemon pokemon = psip.Team.TryGetPokemon(info.PokemonId);
                            _queuedMessages.AppendLine(string.Format("{1} sent out {0}!", pokemon.KnownNickname, psip.Team.TrainerName));
                        }
                    }
                    break;
                }
                case PBEPkmnSwitchOutPacket psop:
                {
                    if (!psop.Forced)
                    {
                        PBEBattlePokemon disguisedAsPokemon = psop.PokemonTeam.TryGetPokemon(psop.DisguisedAsPokemonId);
                        _queuedMessages.AppendLine(string.Format("{1} withdrew {0}!", disguisedAsPokemon.Nickname, psop.PokemonTeam.TrainerName));
                    }
                    break;
                }
                case PBEPsychUpPacket pup:
                {
                    PBEBattlePokemon user = pup.UserTeam.TryGetPokemon(pup.User),
                            target = pup.TargetTeam.TryGetPokemon(pup.Target);
                    _queuedMessages.AppendLine(string.Format("{0} copied {1}'s stat changes!", NameForTrainer(user), NameForTrainer(target)));
                    break;
                }
                case PBESpecialMessagePacket smp:
                {
                    PBEBattlePokemon pokemon = null;
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
                        case PBESpecialMessage.MultiHit:
                        {
                            message = string.Format("Hit {0} time(s)!", (byte)smp.Params[0]);
                            break;
                        }
                        case PBESpecialMessage.NothingHappened:
                        {
                            message = "But nothing happened!";
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
                    _queuedMessages.AppendLine(message);
                    break;
                }
                case PBEStatus1Packet s1p:
                {
                    PBEBattlePokemon status1Receiver = s1p.Status1ReceiverTeam.TryGetPokemon(s1p.Status1Receiver);
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
                    _queuedMessages.AppendLine(string.Format(message, NameForTrainer(status1Receiver)));
                    break;
                }
                case PBEStatus2Packet s2p:
                {
                    PBEBattlePokemon status2Receiver = s2p.Status2ReceiverTeam.TryGetPokemon(s2p.Status2Receiver),
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
                                case PBEStatusAction.Added: message = "{1} cut its own HP and laid a curse on {0}!"; break;
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
                                case PBEStatusAction.Ended: return;
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
                                case PBEStatusAction.Ended: return;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.PowerTrick:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} switched its Attack and Defense!"; break;
                                case PBEStatusAction.Ended: return;
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
                                case PBEStatusAction.Cleared: message = "{1} broke through {0}'s protection!"; break;
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
                        case PBEStatus2.ShadowForce:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} vanished instantly!"; break;
                                case PBEStatusAction.Ended: return;
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
                    _queuedMessages.AppendLine(string.Format(message, NameForTrainer(status2Receiver), NameForTrainer(pokemon2)));
                    break;
                }
                case PBETeamStatusPacket tsp:
                {
                    PBEBattlePokemon damageVictim = tsp.DamageVictim.HasValue ? tsp.Team.TryGetPokemon(tsp.DamageVictim.Value) : null;
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
                        case PBETeamStatus.Safeguard:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "{0}'s team became cloaked in a mystical veil!"; break;
                                case PBETeamStatusAction.Ended: message = "{0}'s team is no longer protected by Safeguard!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.Spikes:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "Spikes were scattered all around the feet of {0}'s team!"; break;
                                //case PBETeamStatusAction.Cleared: message = "The spikes disappeared from around {0}'s team's feet!"; break;
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
                                //case PBETeamStatusAction.Cleared: message = "The pointed stones disappeared from around {0}'s team!"; break;
                                case PBETeamStatusAction.Damage: message = "Pointed stones dug into {1}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.Tailwind:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "The tailwind blew from behind {0}'s team!"; break;
                                case PBETeamStatusAction.Ended: message = "{0}'s team's tailwind petered out!"; break;
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
                                case PBETeamStatusAction.Cleared: message = "{0}'s team's Wide Guard was destroyed!"; break;
                                case PBETeamStatusAction.Damage: message = "Wide Guard protected {1}!"; break;
                                case PBETeamStatusAction.Ended: return;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatus));
                    }
                    _queuedMessages.AppendLine(string.Format(message, tsp.Team.TrainerName, NameForTrainer(damageVictim)));
                    break;
                }
                case PBETypeChangedPacket tcp:
                {
                    PBEBattlePokemon pokemon = tcp.PokemonTeam.TryGetPokemon(tcp.Pokemon);
                    PBEType type1 = tcp.Type1;
                    PBEType type2 = tcp.Type2;
                    string type1Str = PBELocalizedString.GetTypeName(type1).English;
                    _queuedMessages.AppendLine(string.Format("{0} transformed into the {1}", NameForTrainer(pokemon), type2 == PBEType.None ? $"{type1Str} type!" : $"{type1Str} and {PBELocalizedString.GetTypeName(type2).English} types!"));
                    break;
                }
                case PBEWeatherPacket wp:
                {
                    PBEBattlePokemon damageVictim = wp.DamageVictim.HasValue ? wp.DamageVictimTeam.TryGetPokemon(wp.DamageVictim.Value) : null;
                    if (wp.WeatherAction == PBEWeatherAction.Added || wp.WeatherAction == PBEWeatherAction.Ended)
                    {
                        SetEmbedTitle();
                    }
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
                    _queuedMessages.AppendLine(string.Format(message, NameForTrainer(damageVictim)));
                    break;
                }
                case PBEActionsRequestPacket arp:
                {
                    PBETeam team = arp.Team;
                    SocketUser user = team.Id == 0 ? _battler0 : _battler1;
                    if (user == null) // PBEAI
                    {
                        PBEBattle.SelectActionsIfValid(team, PBEAI.CreateActions(team));
                    }
                    else
                    {
                        PBEBattlePokemon mainPkmn = team.ActionsRequired[0];
                        var reactionsToAdd = new List<(IUserMessage Message, IEmote Reaction)>(PBESettings.DefaultMaxPartySize - 1 + PBESettings.DefaultNumMoves); // 5 switch reactions, 4 move reactions
                        string description;
                        EmbedFieldBuilder[] fields;

                        if (mainPkmn.CanSwitchOut())
                        {
                            async Task SwitchReactionClicked(IUserMessage switchMsg, PBEBattlePokemon switchPkmn)
                            {
                                await switchMsg.AddReactionAsync(_confirmationEmoji); // Put this here so it happens before RunTurn() takes its time
                                PBEBattle.SelectActionsIfValid(team, new[] { new PBETurnAction(mainPkmn.Id, switchPkmn.Id) });
                            }

                            PBEBattlePokemon[] switches = team.Party.Where(p => p != mainPkmn && p.HP > 0).ToArray();
                            for (int i = 0; i < switches.Length; i++)
                            {
                                PBEBattlePokemon switchPkmn = switches[i];
                                CreatePokemonEmbed(switchPkmn, false, out description, out fields);
                                IUserMessage switchMsg = await CreateAndSendEmbedAsync(description, messageText: i == 0 ? Separator : string.Empty, pkmn: switchPkmn, useUpperImage: true, fields: fields, userToSendTo: user);
                                reactionsToAdd.Add((switchMsg, _switchEmoji));
                                ReactionHandler.AddListener(user, switchMsg, _switchEmoji, () => SwitchReactionClicked(switchMsg, switchPkmn));
                            }
                        }

                        CreatePokemonEmbed(mainPkmn, true, out description, out fields);
                        IUserMessage mainMsg = await CreateAndSendEmbedAsync($"{description}\nTo check a move: `!move info {PBELocalizedString.GetMoveName(Utils.RandomElement(PBEDataUtils.AllMoves)).English}`", pkmn: mainPkmn, useUpperImage: false, fields: fields, userToSendTo: user);

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
                                default: throw new ArgumentOutOfRangeException(nameof(possibleTargets));
                            }

                            PBEBattle.SelectActionsIfValid(team, new[] { new PBETurnAction(mainPkmn.Id, move, targets) });
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
                    }
                    break;
                }
                case PBESwitchInRequestPacket sirp:
                {
                    PBETeam team = sirp.Team;
                    PBEBattlePokemon[] switches = team.Party.Where(p => p.HP > 0).ToArray();
                    if (switches.Length == 1)
                    {
                        PBEBattle.SelectSwitchesIfValid(team, new[] { new PBESwitchIn(switches[0].Id, PBEFieldPosition.Center) });
                    }
                    else
                    {
                        SocketUser user = team.Id == 0 ? _battler0 : _battler1;
                        if (user == null) // PBEAI
                        {
                            PBEBattle.SelectSwitchesIfValid(team, PBEAI.CreateSwitches(team));
                        }
                        else
                        {
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
                                PBEBattle.SelectSwitchesIfValid(team, new[] { new PBESwitchIn(switchPkmn.Id, PBEFieldPosition.Center) });
                            }

                            var reactionsToAdd = new (IUserMessage Message, IEmote Reaction)[switches.Length];
                            for (int i = 0; i < switches.Length; i++)
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
                        }
                    }
                    break;
                }
                case PBETurnBeganPacket tbp:
                {
                    await SendQueuedMessages();
                    SetEmbedTitle();
                    await SendActiveBattlerEmbeds();
                    break;
                }
                case PBEWinnerPacket win:
                {
                    _queuedMessages.AppendLine(string.Format("{0} defeated {1}!", win.WinningTeam.TrainerName, win.WinningTeam.OpposingTeam.TrainerName));
                    await SendQueuedMessages();
                    break;
                }
            }
        }
    }
}
