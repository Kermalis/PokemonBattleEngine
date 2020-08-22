using Discord;
using Discord.WebSocket;
using Kermalis.PokemonBattleEngine.AI;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
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
            await Forfeit(GetTrainer(user));
        }
        private async Task Forfeit(PBETrainer trainer)
        {
            await CloseNormalWithMessage(string.Format("{0} has forfeited the match.", GetTrainerName(trainer)), ReplaySaver.ShouldSaveForfeits, true);
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
            string s = $"**[#{BattleId}] ― {GetTrainerName(_battle.Trainers[0])} vs {GetTrainerName(_battle.Trainers[1])}";
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
            sb.AppendLine($"{pkmn.Nickname}/{pkmn.Species}{formStr} {pkmn.Gender.ToSymbol()} Lv.{pkmn.Level}{(pkmn.Shiny ? $" {_shinyEmoji}" : string.Empty)}");
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
            if (pkmn.Moves.Contains(PBEMoveEffect.Frustration) || pkmn.Moves.Contains(PBEMoveEffect.Return))
            {
                sb.AppendLine($"**Friendship:** {pkmn.Friendship} ({pkmn.Friendship / (double)byte.MaxValue:P2})");
            }
            if (pkmn.Moves.Contains(PBEMoveEffect.HiddenPower))
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
        private string CreateKnownPokemonEmbed(PBEBattlePokemon pkmn)
        {
            var pData = PBEPokemonData.GetData(pkmn.KnownSpecies, pkmn.KnownForm);
            var sb = new StringBuilder();
            string formStr = PBEDataUtils.HasForms(pkmn.KnownSpecies, false) ? $" ({PBELocalizedString.GetFormName(pkmn.KnownSpecies, pkmn.KnownForm)})" : string.Empty;
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
        private PBETrainer GetTrainer(SocketUser battler)
        {
            // Assumes "battler" is not null (AI)
            if (_battler0 != null && battler.Id == _battler0.Id)
            {
                return _battle.Trainers[0];
            }
            if (_battler1 != null && battler.Id == _battler1.Id)
            {
                return _battle.Trainers[1];
            }
            throw new ArgumentOutOfRangeException(nameof(battler));
        }
        private SocketUser GetBattler(PBETrainer trainer)
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
        // The second parameter in the following is necessary for the packet handler since funcs these are passed in as arguments
        private string GetTrainerName(PBETrainer trainer)
        {
            SocketUser user = GetBattler(trainer);
            return user == null ? trainer.Name : user.Username;
        }
        private string GetTeamName(PBETeam team, bool firstLetterCapitalized = true)
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
                    SocketUser user = GetBattler(trainer);
                    if (user == null) // PBEAI
                    {
                        trainer.CreateAIActions();
                    }
                    else
                    {
                        PBEBattlePokemon mainPkmn = trainer.ActionsRequired[0];
                        var reactionsToAdd = new List<(IUserMessage Message, IEmote Reaction)>(PBESettings.DefaultMaxPartySize - 1 + PBESettings.DefaultNumMoves); // 5 switch reactions, 4 move reactions
                        string description;
                        EmbedFieldBuilder[] fields;

                        if (mainPkmn.CanSwitchOut())
                        {
                            async Task SwitchReactionClicked(IUserMessage switchMsg, PBEBattlePokemon switchPkmn)
                            {
                                await switchMsg.AddReactionAsync(_confirmationEmoji); // Put this here so it happens before RunTurn() takes its time
                                trainer.SelectActionsIfValid(new PBETurnAction(mainPkmn, switchPkmn));
                            }

                            PBEBattlePokemon[] switches = trainer.Party.Where(p => p != mainPkmn && p.HP > 0).ToArray();
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

                            trainer.SelectActionsIfValid(new PBETurnAction(mainPkmn, move, targets));
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
                    return;
                }
                case PBESwitchInRequestPacket sirp:
                {
                    PBETrainer trainer = sirp.Trainer;
                    PBEBattlePokemon[] switches = trainer.Party.Where(p => p.HP > 0).ToArray();
                    if (switches.Length == 1)
                    {
                        trainer.SelectSwitchesIfValid(new PBESwitchIn(switches[0], PBEFieldPosition.Center));
                    }
                    else
                    {
                        SocketUser user = GetBattler(trainer);
                        if (user == null) // PBEAI
                        {
                            trainer.CreateAISwitches();
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
                                trainer.SelectSwitchesIfValid(new PBESwitchIn(switchPkmn, PBEFieldPosition.Center));
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
                        default: throw new ArgumentOutOfRangeException(nameof(brp.BattleResult));
                    }
                    _queuedMessages.AppendLine(string.Format(m, GetTeamName(_battle.Teams[0]), GetTeamName(_battle.Teams[1])));
                    await SendQueuedMessages();
                    return;
                }
            }
            // Get default message
            string message = PBEBattle.GetDefaultMessage(_battle, packet, trainerNameFunc: GetTrainerName, teamNameFunc: GetTeamName);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            _queuedMessages.AppendLine(message);
        }
    }
}
