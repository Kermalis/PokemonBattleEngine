using Discord;
using Discord.Net;
using Discord.WebSocket;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
    internal sealed class BattleContext
    {
        private const string Separator = "**--------------------**";
        private static readonly Emoji _switchEmoji = new Emoji("😼");
        private static readonly Emoji _confirmationEmoji = new Emoji("👍");
        private static readonly PBEMove[] _suggestedMoves = Enum.GetValues(typeof(PBEMove)).Cast<PBEMove>().Except(new[] { PBEMove.None, PBEMove.MAX }).ToArray();
        private static readonly Dictionary<PBEType, Emote>[] _moveEmojis = new Dictionary<PBEType, Emote>[4]
        {
            new Dictionary<PBEType, Emote>
            {
                { PBEType.Bug, Emote.Parse("<:A_Bug:589220293814517764>") },
                { PBEType.Dark, Emote.Parse("<:A_Dark:589220294070108172>") },
                { PBEType.Dragon, Emote.Parse("<:A_Dragon:589220295517143048>") },
                { PBEType.Electric, Emote.Parse("<:A_Electric:589220295664074767>") },
                { PBEType.Fighting, Emote.Parse("<:A_Fighting:589220295454359552>") },
                { PBEType.Fire, Emote.Parse("<:A_Fire:589220295286718485>") },
                { PBEType.Flying, Emote.Parse("<:A_Flying:589220295047380994>") },
                { PBEType.Ghost, Emote.Parse("<:A_Ghost:589220295412547599>") },
                { PBEType.Grass, Emote.Parse("<:A_Grass:589220295517143053>") },
                { PBEType.Ground, Emote.Parse("<:A_Ground:589220295525531648>") },
                { PBEType.Ice, Emote.Parse("<:A_Ice:589220295517143105>") },
                { PBEType.Normal, Emote.Parse("<:A_Normal:589220296586690620>") },
                { PBEType.Poison, Emote.Parse("<:A_Poison:589220295202832388>") },
                { PBEType.Psychic, Emote.Parse("<:A_Psychic:589220295395770384>") },
                { PBEType.Rock, Emote.Parse("<:A_Rock:589220295618068549>") },
                { PBEType.Steel, Emote.Parse("<:A_Steel:589220295651622922>") },
                { PBEType.Water, Emote.Parse("<:A_Water:589220295664074770>") }
            },
            new Dictionary<PBEType, Emote>
            {
                { PBEType.Bug, Emote.Parse("<:B_Bug:589220295592902667>") },
                { PBEType.Dark, Emote.Parse("<:B_Dark:589220295286718496>") },
                { PBEType.Dragon, Emote.Parse("<:B_Dragon:589220295517143250>") },
                { PBEType.Electric, Emote.Parse("<:B_Electric:589220295584251944>") },
                { PBEType.Fighting, Emote.Parse("<:B_Fighting:589220295198375948>") },
                { PBEType.Fire, Emote.Parse("<:B_Fire:589220295252901900>") },
                { PBEType.Flying, Emote.Parse("<:B_Flying:589220295622000680>") },
                { PBEType.Ghost, Emote.Parse("<:B_Ghost:589220295605485588>") },
                { PBEType.Grass, Emote.Parse("<:B_Grass:589220295701823506>") },
                { PBEType.Ground, Emote.Parse("<:B_Ground:589220295584514058>") },
                { PBEType.Ice, Emote.Parse("<:B_Ice:589220295739441153>") },
                { PBEType.Normal, Emote.Parse("<:B_Normal:589220295672463485>") },
                { PBEType.Poison, Emote.Parse("<:B_Poison:589220295643234366>") },
                { PBEType.Psychic, Emote.Parse("<:B_Psychic:589220295483719695>") },
                { PBEType.Rock, Emote.Parse("<:B_Rock:589220295676657667>") },
                { PBEType.Steel, Emote.Parse("<:B_Steel:589220295773126672>") },
                { PBEType.Water, Emote.Parse("<:B_Water:589220295710212153>") }
            },
            new Dictionary<PBEType, Emote>
            {
                { PBEType.Bug, Emote.Parse("<:C_Bug:589220295429324812>") },
                { PBEType.Dark, Emote.Parse("<:C_Dark:589220295911538708>") },
                { PBEType.Dragon, Emote.Parse("<:C_Dragon:589220295840104448>") },
                { PBEType.Electric, Emote.Parse("<:C_Electric:589220295517143165>") },
                { PBEType.Fighting, Emote.Parse("<:C_Fighting:589220295852949504>") },
                { PBEType.Fire, Emote.Parse("<:C_Fire:589220295747829801>") },
                { PBEType.Flying, Emote.Parse("<:C_Flying:589220295836041236>") },
                { PBEType.Ghost, Emote.Parse("<:C_Ghost:589220295479525400>") },
                { PBEType.Grass, Emote.Parse("<:C_Grass:589220295706017817>") },
                { PBEType.Ground, Emote.Parse("<:C_Ground:589220296104476702>") },
                { PBEType.Ice, Emote.Parse("<:C_Ice:589220296255602728>") },
                { PBEType.Normal, Emote.Parse("<:C_Normal:589220295865270292>") },
                { PBEType.Poison, Emote.Parse("<:C_Poison:589220295479656450>") },
                { PBEType.Psychic, Emote.Parse("<:C_Psychic:589220295949156352>") },
                { PBEType.Rock, Emote.Parse("<:C_Rock:589220295672463436>") },
                { PBEType.Steel, Emote.Parse("<:C_Steel:589220295936573469>") },
                { PBEType.Water, Emote.Parse("<:C_Water:589220471711465591>") }
            },
            new Dictionary<PBEType, Emote>
            {
                { PBEType.Bug, Emote.Parse("<:D_Bug:589220471065542685>") },
                { PBEType.Dark, Emote.Parse("<:D_Dark:589220471816454173>") },
                { PBEType.Dragon, Emote.Parse("<:D_Dragon:589220471845683271>") },
                { PBEType.Electric, Emote.Parse("<:D_Electric:589220471770447873>") },
                { PBEType.Fighting, Emote.Parse("<:D_Fighting:589220471531372560>") },
                { PBEType.Fire, Emote.Parse("<:D_Fire:589220471678173235>") },
                { PBEType.Flying, Emote.Parse("<:D_Flying:589220471569121300>") },
                { PBEType.Ghost, Emote.Parse("<:D_Ghost:589220471967449123>") },
                { PBEType.Grass, Emote.Parse("<:D_Grass:589220471862460463>") },
                { PBEType.Ground, Emote.Parse("<:D_Ground:589220471900340224>") },
                { PBEType.Ice, Emote.Parse("<:D_Ice:589220471736631328>") },
                { PBEType.Normal, Emote.Parse("<:D_Normal:589220471615258626>") },
                { PBEType.Poison, Emote.Parse("<:D_Poison:589220471816454171>") },
                { PBEType.Psychic, Emote.Parse("<:D_Psychic:589220471615127563>") },
                { PBEType.Rock, Emote.Parse("<:D_Rock:589220471489298445>") },
                { PBEType.Steel, Emote.Parse("<:D_Steel:589220471879237658>") },
                { PBEType.Water, Emote.Parse("<:D_Water:589220471543955481>") }
            }
        };
        private static readonly List<BattleContext> _activeBattles = new List<BattleContext>(); // TODO: Locks for accessing this

        private readonly PBEBattle _battle;
        private readonly SocketUser[] _battlers;
        private readonly ISocketMessageChannel _channel;

        public BattleContext(PBEBattle battle, SocketUser battler0, SocketUser battler1, ISocketMessageChannel channel)
        {
            _activeBattles.Add(this);

            _battle = battle;
            _battlers = new SocketUser[] { battler0, battler1 };
            _channel = channel;

            battle.OnNewEvent += (b, p) => Battle_OnNewEvent(_activeBattles.Single(a => a._battle == b), p).GetAwaiter().GetResult();
            battle.OnStateChanged += (b) => Battle_OnStateChanged(_activeBattles.Single(a => a._battle == b)).GetAwaiter().GetResult();
            battle.Begin();
        }

        public static BattleContext GetBattleContext(SocketUser user)
        {
            return _activeBattles.SingleOrDefault(b => b.IndexOf(user) != -1);
        }
        public int IndexOf(SocketUser user)
        {
            if (_battlers[0].Id == user.Id)
            {
                return 0;
            }
            else if (_battlers[1].Id == user.Id)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        private async Task<IUserMessage> CreateAndSendEmbedAsync(string embedDescription, string messageText = "", PBEPokemon pkmn = null, bool useUpperImage = false, SocketUser userToSendTo = null)
        {
            string title = $"{_battlers[0].Username} vs {_battlers[1].Username}";
            if (_battle.TurnNumber > 0)
            {
                title += $" (Turn {_battle.TurnNumber})";
            }

            EmbedBuilder embed = new EmbedBuilder()
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
                return await _channel.SendMessageAsync(messageText, embed: embed.Build());
            }
        }
        private async void Forfeit(SocketUser user)
        {
            _activeBattles.Remove(this);
            await CreateAndSendEmbedAsync($"{user.Username} has forfeited the match.");
        }

        private static void AddStatChanges(PBEPokemon pkmn, StringBuilder sb)
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
        private static string CustomPokemonToString(PBEPokemon pkmn, bool addReactionChars)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{pkmn.Nickname}/{pkmn.Species} {pkmn.GenderSymbol} Lv.{pkmn.Level}");
            sb.AppendLine($"**HP:** {pkmn.HP}/{pkmn.MaxHP} ({pkmn.HPPercentage:P2})");
            sb.Append($"**Types:** {PBELocalizedString.GetTypeName(pkmn.Type1).English}");
            if (pkmn.Type2 != PBEType.None)
            {
                sb.Append($"/{PBELocalizedString.GetTypeName(pkmn.Type2).English}");
            }
            sb.AppendLine();
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
                sb.AppendLine($"**{PBELocalizedString.GetMoveName(PBEMove.HiddenPower).English}:** {PBELocalizedString.GetTypeName(pkmn.IndividualValues.HiddenPowerType).English}:{pkmn.IndividualValues.HiddenPowerBasePower}");
            }
            sb.Append("**Moves:** ");
            for (int i = 0; i < PBESettings.DefaultNumMoves; i++)
            {
                PBEBattleMoveset.PBEBattleMovesetSlot slot = pkmn.Moves[i];
                PBEMove move = slot.Move;
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(PBELocalizedString.GetMoveName(slot.Move).English);
                if (move != PBEMove.None)
                {
                    sb.Append($" ({slot.PP}/{slot.MaxPP})");
                }
            }
            sb.AppendLine();
            sb.Append("**Usable moves:** ");
            PBEMove[] usableMoves = pkmn.GetUsableMoves();
            for (int i = 0; i < usableMoves.Length; i++)
            {
                PBEMove move = usableMoves[i];
                if (i > 0)
                {
                    sb.Append(", ");
                }
                if (addReactionChars)
                {
                    sb.Append($"{_moveEmojis[i][pkmn.GetMoveType(move)].ToString()} ");
                }
                sb.Append(PBELocalizedString.GetMoveName(move).English);
            }
            return sb.ToString();
        }
        private static string CustomKnownPokemonToString(PBEPokemon pkmn)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{pkmn.Team.TrainerName}'s {pkmn.KnownNickname}/{pkmn.KnownSpecies} {(pkmn.Status2.HasFlag(PBEStatus2.Transformed) ? pkmn.GenderSymbol : pkmn.KnownGenderSymbol)} Lv.{pkmn.Level}");
            sb.AppendLine($"**HP:** {pkmn.HPPercentage:P2}");
            sb.Append($"**Known types:** {PBELocalizedString.GetTypeName(pkmn.KnownType1).English}");
            if (pkmn.KnownType2 != PBEType.None)
            {
                sb.Append($"/{PBELocalizedString.GetTypeName(pkmn.KnownType2).English}");
            }
            sb.AppendLine();
            if (pkmn.Status1 != PBEStatus1.None)
            {
                sb.AppendLine($"**Main status:** {pkmn.Status1}");
            }
            if (pkmn.Status1 == PBEStatus1.Asleep)
            {
                sb.AppendLine($"**Sleep turns:** {pkmn.Status1Counter}");
            }
            else if (pkmn.Status1 == PBEStatus1.BadlyPoisoned)
            {
                sb.AppendLine($"**Toxic Counter:** {pkmn.Status1Counter}");
            }
            PBEStatus2 cleanStatus2 = pkmn.Status2;
            cleanStatus2 &= ~PBEStatus2.Disguised;
            if (cleanStatus2 != PBEStatus2.None)
            {
                sb.AppendLine($"**Volatile status:** {cleanStatus2}");
                if (cleanStatus2.HasFlag(PBEStatus2.Confused))
                {
                    sb.AppendLine($"**Confusion turns:** {pkmn.ConfusionCounter}");
                }
            }
            PBEPokemonData.GetStatRange(PBEStat.HP, pkmn.KnownSpecies, pkmn.Level, pkmn.Team.Battle.Settings, out ushort lowHP, out ushort highHP);
            PBEPokemonData.GetStatRange(PBEStat.Attack, pkmn.KnownSpecies, pkmn.Level, pkmn.Team.Battle.Settings, out ushort lowAttack, out ushort highAttack);
            PBEPokemonData.GetStatRange(PBEStat.Defense, pkmn.KnownSpecies, pkmn.Level, pkmn.Team.Battle.Settings, out ushort lowDefense, out ushort highDefense);
            PBEPokemonData.GetStatRange(PBEStat.SpAttack, pkmn.KnownSpecies, pkmn.Level, pkmn.Team.Battle.Settings, out ushort lowSpAttack, out ushort highSpAttack);
            PBEPokemonData.GetStatRange(PBEStat.SpDefense, pkmn.KnownSpecies, pkmn.Level, pkmn.Team.Battle.Settings, out ushort lowSpDefense, out ushort highSpDefense);
            PBEPokemonData.GetStatRange(PBEStat.Speed, pkmn.KnownSpecies, pkmn.Level, pkmn.Team.Battle.Settings, out ushort lowSpeed, out ushort highSpeed);
            sb.AppendLine($"**Stat range:** [HP] {lowHP}-{highHP}, [A] {lowAttack}-{highAttack}, [D] {lowDefense}-{highDefense}, [SA] {lowSpAttack}-{highSpAttack}, [SD] {lowSpDefense}-{highSpDefense}, [S] {lowSpeed}-{highSpeed}, [W] {pkmn.KnownWeight:0.0}");
            AddStatChanges(pkmn, sb);
            if (pkmn.KnownAbility == PBEAbility.MAX)
            {
                sb.AppendLine($"**Possible abilities:** {string.Join(", ", PBEPokemonData.GetData(pkmn.KnownSpecies).Abilities.Select(a => PBELocalizedString.GetAbilityName(a).English))}");
            }
            else
            {
                sb.AppendLine($"**Known ability:** {PBELocalizedString.GetAbilityName(pkmn.KnownAbility).English}");
            }
            sb.AppendLine($"**Known item:** {(pkmn.KnownItem == (PBEItem)ushort.MaxValue ? "???" : PBELocalizedString.GetItemName(pkmn.KnownItem).English)}");
            sb.Append("**Known moves:** ");
            for (int i = 0; i < pkmn.Team.Battle.Settings.NumMoves; i++)
            {
                PBEBattleMoveset.PBEBattleMovesetSlot slot = pkmn.KnownMoves[i];
                PBEMove move = slot.Move;
                int pp = slot.PP;
                int maxPP = slot.MaxPP;
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(move == PBEMove.MAX ? "???" : PBELocalizedString.GetMoveName(move).English);
                if (move != PBEMove.None && move != PBEMove.MAX)
                {
                    sb.Append($" ({pp}{(maxPP == 0 ? ")" : $"/{maxPP})")}");
                }
            }
            return sb.ToString();
        }

        private static Task Battle_OnStateChanged(BattleContext context)
        {
            switch (context._battle.BattleState)
            {
                case PBEBattleState.ReadyToRunTurn:
                {
                    context._battle.RunTurn();
                    break;
                }
                case PBEBattleState.Ended:
                {
                    _activeBattles.Remove(context);
                    break;
                }
            }
            return Task.CompletedTask;
        }
        private static async Task Battle_OnNewEvent(BattleContext context, INetPacket packet)
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
                                case PBEAbilityAction.ChangedStats: message = "{0}'s {2} activated!"; break;
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
                                case PBEAbilityAction.ChangedAppearance: message = "{0}'s illusion wore off!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
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
                                case PBEAbilityAction.ChangedStats: message = "{0}'s {2} activated!"; break;
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
                    await context.CreateAndSendEmbedAsync(string.Format(message, NameForTrainer(abilityOwner), NameForTrainer(pokemon2), PBELocalizedString.GetAbilityName(ap.Ability).English), pkmn: abilityOwner);
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
                    await context.CreateAndSendEmbedAsync(message);
                    break;
                }
                case PBEHazePacket _:
                {
                    await context.CreateAndSendEmbedAsync("All stat changes were eliminated!");
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
                    await context.CreateAndSendEmbedAsync(string.Format(message, NameForTrainer(itemHolder), NameForTrainer(pokemon2), PBELocalizedString.GetItemName(ip.Item).English), pkmn: itemHolder);
                    break;
                }
                case PBEMoveCritPacket mcp:
                {
                    PBEPokemon victim = mcp.VictimTeam.TryGetPokemon(mcp.Victim);
                    await context.CreateAndSendEmbedAsync("A critical hit on {0}!", NameForTrainer(victim));
                    break;
                }
                case PBEMoveMissedPacket mmp:
                {
                    PBEPokemon moveUser = mmp.MoveUserTeam.TryGetPokemon(mmp.MoveUser),
                            pokemon2 = mmp.Pokemon2Team.TryGetPokemon(mmp.Pokemon2);
                    await context.CreateAndSendEmbedAsync(string.Format("{0}'s attack missed {1}!", NameForTrainer(moveUser), NameForTrainer(pokemon2)), pkmn: moveUser);
                    break;
                }
                case PBEMoveResultPacket mrp:
                {
                    PBEPokemon moveUser = mrp.MoveUserTeam.TryGetPokemon(mrp.MoveUser),
                            pokemon2 = mrp.Pokemon2Team.TryGetPokemon(mrp.Pokemon2);
                    string message;
                    switch (mrp.Result)
                    {
                        case PBEResult.Ineffective_Ability: message = "{1} is protected by its Ability!"; break;
                        case PBEResult.Ineffective_Gender:
                        case PBEResult.Ineffective_Stat:
                        case PBEResult.Ineffective_Status:
                        case PBEResult.InvalidConditions: message = "But it failed!"; break;
                        case PBEResult.Ineffective_Level: message = "{1} is protected by its level!"; break;
                        case PBEResult.Ineffective_MagnetRise: message = $"{{1}} is protected by {PBELocalizedString.GetMoveName(PBEMove.MagnetRise).English}!"; break;
                        case PBEResult.Ineffective_Safeguard: message = $"{{1}} is protected by {PBELocalizedString.GetMoveName(PBEMove.Safeguard).English}!"; break;
                        case PBEResult.Ineffective_Substitute: message = $"{{1}} is protected by {PBELocalizedString.GetMoveName(PBEMove.Substitute).English}!"; break;
                        case PBEResult.Ineffective_Type: message = "{1} is protected by its Type!"; break;
                        case PBEResult.NoTarget: message = "But there was no target..."; break;
                        case PBEResult.NotVeryEffective_Type: message = "It's not very effective on {1}..."; break;
                        case PBEResult.SuperEffective_Type: message = "It's super effective on {1}!"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(mrp.Result));
                    }
                    await context.CreateAndSendEmbedAsync(string.Format(message, NameForTrainer(moveUser), NameForTrainer(pokemon2)), pkmn: pokemon2);
                    break;
                }
                case PBEMoveUsedPacket mup:
                {
                    PBEPokemon moveUser = mup.MoveUserTeam.TryGetPokemon(mup.MoveUser);
                    await context.CreateAndSendEmbedAsync(string.Format("{0} used {1}!", NameForTrainer(moveUser), PBELocalizedString.GetMoveName(mup.Move).English), pkmn: moveUser);
                    break;
                }
                case PBEPkmnFaintedPacket pfap:
                {
                    PBEPokemon pokemon = pfap.PokemonTeam.TryGetPokemon(pfap.PokemonId);
                    await context.CreateAndSendEmbedAsync(string.Format("{0} fainted!", NameForTrainer(pokemon)), pkmn: pokemon);
                    break;
                }
                case PBEPkmnFormChangedPacket pfcp:
                {
                    PBEPokemon pokemon = pfcp.PokemonTeam.TryGetPokemon(pfcp.Pokemon);
                    await context.CreateAndSendEmbedAsync(string.Format("{0} transformed!", NameForTrainer(pokemon)), pkmn: pokemon);
                    break;
                }
                case PBEPkmnHPChangedPacket phcp:
                {
                    PBEPokemon pokemon = phcp.PokemonTeam.TryGetPokemon(phcp.Pokemon);
                    int change = phcp.NewHP - phcp.OldHP;
                    int absChange = Math.Abs(change);
                    double percentageChange = phcp.NewHPPercentage - phcp.OldHPPercentage;
                    double absPercentageChange = Math.Abs(percentageChange);
                    await context.CreateAndSendEmbedAsync(string.Format("{0} {1} {2:P2} of its HP!", NameForTrainer(pokemon), percentageChange <= 0 ? "lost" : "restored", absPercentageChange), pkmn: pokemon);
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
                            if (change == 0 && pscp.NewValue == -context._battle.Settings.MaxStatChange)
                            {
                                message = "won't go lower";
                            }
                            else if (change == 0 && pscp.NewValue == context._battle.Settings.MaxStatChange)
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
                    await context.CreateAndSendEmbedAsync(string.Format("{0}'s {1} {2}!", NameForTrainer(pokemon), statName, message), pkmn: pokemon);
                    break;
                }
                case PBEPkmnSwitchInPacket psip:
                {
                    if (!psip.Forced)
                    {
                        foreach (PBEPkmnSwitchInPacket.PBESwitchInInfo info in psip.SwitchIns)
                        {
                            PBEPokemon pokemon = psip.Team.TryGetPokemon(info.PokemonId);
                            await context.CreateAndSendEmbedAsync(string.Format("{1} sent out {0}!", pokemon.KnownNickname, psip.Team.TrainerName), pkmn: pokemon);
                        }
                    }
                    break;
                }
                case PBEPkmnSwitchOutPacket psop:
                {
                    if (!psop.Forced)
                    {
                        PBEPokemon disguisedAsPokemon = psop.PokemonTeam.TryGetPokemon(psop.DisguisedAsPokemonId);
                        await context.CreateAndSendEmbedAsync(string.Format("{1} withdrew {0}!", disguisedAsPokemon.Nickname, psop.PokemonTeam.TrainerName), pkmn: disguisedAsPokemon);
                    }
                    break;
                }
                case PBEPsychUpPacket pup:
                {
                    PBEPokemon user = pup.UserTeam.TryGetPokemon(pup.User),
                            target = pup.TargetTeam.TryGetPokemon(pup.Target);
                    await context.CreateAndSendEmbedAsync(string.Format("{0} copied {1}'s stat changes!", NameForTrainer(user), NameForTrainer(target)), pkmn: user);
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
                    await context.CreateAndSendEmbedAsync(message, pkmn: pokemon);
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
                    await context.CreateAndSendEmbedAsync(string.Format(message, NameForTrainer(status1Receiver)), pkmn: status1Receiver);
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
                                case PBEStatusAction.Cured:
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
                                case PBEStatusAction.Cured: message = "{0} got over its infatuation."; break;
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
                    await context.CreateAndSendEmbedAsync(string.Format(message, NameForTrainer(status2Receiver), NameForTrainer(pokemon2)), pkmn: status2Receiver);
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
                    await context.CreateAndSendEmbedAsync(string.Format(message, tsp.Team.TrainerName, NameForTrainer(damageVictim)), pkmn: damageVictim);
                    break;
                }
                case PBEWeatherPacket wp:
                {
                    PBEPokemon damageVictim = wp.HasDamageVictim ? wp.DamageVictimTeam.TryGetPokemon(wp.DamageVictim.Value) : null;
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
                    await context.CreateAndSendEmbedAsync(string.Format(message, NameForTrainer(damageVictim)), pkmn: damageVictim);
                    break;
                }
                case PBEActionsRequestPacket arp:
                {
                    SocketUser user = context._battlers[arp.Team.Id];
                    var userArray = new SocketUser[] { user };
                    PBEPokemon mainPkmn = arp.Team.ActionsRequired[0];
                    var allMessages = new List<IUserMessage>(PBESettings.DefaultMaxPartySize);
                    var reactionsToAdd = new List<(IUserMessage Message, IEmote Reaction)>(PBESettings.DefaultMaxPartySize - 1 + PBESettings.DefaultNumMoves); // 5 switch reactions, 4 move reactions

                    if (mainPkmn.CanSwitchOut())
                    {
                        async Task SwitchReactionClicked(IUserMessage switchMsg, PBEPokemon switchPkmn)
                        {
                            await switchMsg.AddReactionAsync(_confirmationEmoji); // Put this here so it happens before RunTurn() takes its time
                            PBEBattle.SelectActionsIfValid(arp.Team, new[] { new PBETurnAction(mainPkmn.Id, switchPkmn.Id) });
                        }

                        PBEPokemon[] switches = arp.Team.Party.Where(p => p != mainPkmn && p.HP > 0).ToArray();
                        for (int i = 0; i < switches.Length; i++)
                        {
                            PBEPokemon switchPkmn = switches[i];
                            IUserMessage switchMsg;
                            try
                            {
                                switchMsg = await context.CreateAndSendEmbedAsync(CustomPokemonToString(switchPkmn, false), messageText: i == 0 ? Separator : string.Empty, pkmn: switchPkmn, useUpperImage: true, userToSendTo: user);
                            }
                            catch (HttpException)
                            {
                                context.Forfeit(user);
                                return;
                            }
                            allMessages.Add(switchMsg);
                            reactionsToAdd.Add((switchMsg, _switchEmoji));
                            ReactionListener.AddListener(switchMsg, allMessages, _switchEmoji, userArray, () => SwitchReactionClicked(switchMsg, switchPkmn));
                        }
                    }

                    IUserMessage mainMsg;
                    try
                    {
                        mainMsg = await context.CreateAndSendEmbedAsync($"{CustomPokemonToString(mainPkmn, true)}\nTo check a move: `!move info {PBELocalizedString.GetMoveName(Utils.RandomElement(_suggestedMoves)).English}`", pkmn: mainPkmn, useUpperImage: false, userToSendTo: user);
                    }
                    catch (HttpException)
                    {
                        context.Forfeit(user);
                        return;
                    }
                    allMessages.Add(mainMsg);

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

                        PBEBattle.SelectActionsIfValid(arp.Team, new[] { new PBETurnAction(mainPkmn.Id, move, targets) });
                    }

                    PBEMove[] usableMoves = mainPkmn.GetUsableMoves();
                    for (int i = 0; i < usableMoves.Length; i++)
                    {
                        PBEMove move = usableMoves[i]; // move must be evaluated before it reaches the lambda
                        Emote emoji = _moveEmojis[i][mainPkmn.GetMoveType(move)];
                        reactionsToAdd.Add((mainMsg, emoji));
                        ReactionListener.AddListener(mainMsg, allMessages, emoji, userArray, () => MoveReactionClicked(move));
                    }

                    // All listeners are added, so now we can send the reactions
                    // Clicking a reaction will not harm reactions that are not sent yet because allMessages is sent by reference
                    foreach ((IUserMessage Message, IEmote Reaction) in reactionsToAdd)
                    {
                        await Message.AddReactionAsync(Reaction);
                    }
                    break;
                }
                case PBEAutoCenterPacket _: // Currently unused
                {
                    await context.CreateAndSendEmbedAsync("The battlers shifted to the center!");
                    break;
                }
                case PBESwitchInRequestPacket sirp:
                {
                    PBEPokemon[] switches = sirp.Team.Party.Where(p => p.HP > 0).ToArray();
                    if (switches.Length == 1)
                    {
                        PBEBattle.SelectSwitchesIfValid(sirp.Team, new[] { new PBESwitchIn(switches[0].Id, PBEFieldPosition.Center) });
                    }
                    else
                    {
                        async Task SwitchReactionClicked(IUserMessage switchMsg, PBEPokemon switchPkmn)
                        {
                            await switchMsg.AddReactionAsync(_confirmationEmoji); // Put this here so it happens before RunTurn() takes its time
                            PBEBattle.SelectSwitchesIfValid(sirp.Team, new[] { new PBESwitchIn(switchPkmn.Id, PBEFieldPosition.Center) });
                        }

                        SocketUser user = context._battlers[sirp.Team.Id];
                        var userArray = new SocketUser[] { user };
                        var allMessages = new IUserMessage[switches.Length];
                        var reactionsToAdd = new (IUserMessage Message, IEmote Reaction)[switches.Length];
                        for (int i = 0; i < switches.Length; i++)
                        {
                            PBEPokemon switchPkmn = switches[i];
                            IUserMessage switchMsg;
                            try
                            {
                                switchMsg = await context.CreateAndSendEmbedAsync(CustomPokemonToString(switchPkmn, false), messageText: i == 0 ? Separator : string.Empty, pkmn: switchPkmn, useUpperImage: true, userToSendTo: user);
                            }
                            catch (HttpException)
                            {
                                context.Forfeit(user);
                                return;
                            }
                            allMessages[i] = switchMsg;
                            reactionsToAdd[i] = (switchMsg, _switchEmoji);
                            ReactionListener.AddListener(switchMsg, allMessages, _switchEmoji, userArray, () => SwitchReactionClicked(switchMsg, switchPkmn));
                        }

                        // All listeners are added, so now we can send the reactions
                        // Clicking a reaction will not harm reactions that are not sent yet because allMessages is sent by reference
                        for (int i = 0; i < reactionsToAdd.Length; i++)
                        {
                            (IUserMessage Message, IEmote Reaction) = reactionsToAdd[i];
                            await Message.AddReactionAsync(Reaction);
                        }
                    }
                    break;
                }
                case PBETurnBeganPacket tbp:
                {
                    string message = Separator;
                    if (context._battle.Weather != PBEWeather.None)
                    {
                        message += $"\n**Weather:** {context._battle.Weather}";
                    }
                    PBEPokemon team0Pkmn = context._battle.Teams[0].ActiveBattlers[0];
                    PBEPokemon team1Pkmn = context._battle.Teams[1].ActiveBattlers[0];
                    await context.CreateAndSendEmbedAsync(CustomKnownPokemonToString(team0Pkmn), messageText: message, pkmn: team0Pkmn);
                    await context.CreateAndSendEmbedAsync(CustomKnownPokemonToString(team1Pkmn), pkmn: team1Pkmn);
                    break;
                }
                case PBEWinnerPacket win:
                {
                    await context.CreateAndSendEmbedAsync(string.Format("{0} defeated {1}!", win.WinningTeam.TrainerName, win.WinningTeam.OpposingTeam.TrainerName));
                    break;
                }
            }
        }
    }
}
