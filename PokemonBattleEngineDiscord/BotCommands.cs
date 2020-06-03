using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
    public sealed class BotCommands : ModuleBase<SocketCommandContext>
    {
        [Group("ability")]
        public sealed class AbilityCommands : ModuleBase<SocketCommandContext>
        {
            [Command("info")]
            [Alias("data")]
            public async Task Info([Remainder] string abilityName)
            {
                PBEAbility? nAbility = PBELocalizedString.GetAbilityByName(abilityName);
                if (!nAbility.HasValue || nAbility.Value == PBEAbility.None)
                {
                    await Context.Channel.SendMessageAsync($"{Context.User.Mention} Invalid ability!");
                }
                else
                {
                    PBEAbility ability = nAbility.Value;
                    EmbedBuilder embed = new EmbedBuilder()
                        .WithAuthor(Context.User)
                        .WithColor(Utils.RandomColor())
                        .WithTitle(PBELocalizedString.GetAbilityName(ability).English)
                        .WithUrl(Utils.URL)
                        .WithDescription(PBELocalizedString.GetAbilityDescription(ability).English.Replace('\n', ' '));
                    await Context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                }
            }
        }

        [Command("accept", true)]
        public async Task AcceptChallenge()
        {
            await Matchmaking.AcceptChallenge(Context);
        }

        [Command("challenge")]
        [Alias("battle", "duel", "fight")]
        public async Task ChallengeUser(SocketUser battler2)
        {
            await Matchmaking.ChallengeUser(Context, battler2);
        }

        [Command("forfeit", true)]
        [Alias("giveup", "surrender", "throwinthetowel")]
        public async Task Forfeit()
        {
            await Matchmaking.Forfeit(Context);
        }

        [Group("item")]
        [Alias("helditem")]
        public sealed class ItemCommands : ModuleBase<SocketCommandContext>
        {
            [Command("info")]
            [Alias("data")]
            public async Task Info([Remainder] string itemName)
            {
                PBEItem? nItem = PBELocalizedString.GetItemByName(itemName);
                if (!nItem.HasValue || nItem.Value == PBEItem.None)
                {
                    await Context.Channel.SendMessageAsync($"{Context.User.Mention} Invalid item!");
                }
                else
                {
                    PBEItem item = nItem.Value;
                    PBEItemData iData = PBEItemData.Data[item];
                    Color color;
                    if (PBEBerryData.Data.TryGetValue(item, out PBEBerryData bData))
                    {
                        color = Utils.TypeColors[bData.NaturalGiftType];
                    }
                    else
                    {
                        color = Utils.RandomColor();
                    }
                    EmbedBuilder embed = new EmbedBuilder()
                        .WithAuthor(Context.User)
                        .WithColor(color)
                        .WithTitle(PBELocalizedString.GetItemName(item).English)
                        .WithUrl(Utils.URL)
                        .WithDescription(PBELocalizedString.GetItemDescription(item).English.Replace('\n', ' '));
                    if (iData.FlingPower > 0)
                    {
                        embed.AddField("Fling Power", iData.FlingPower, true);
                    }
                    if (bData != null)
                    {
                        embed.AddField("Natural Gift Power", bData.NaturalGiftPower, true);
                        embed.AddField("Natural Gift Type", Utils.TypeEmotes[bData.NaturalGiftType], true);
                        if (bData.Bitterness > 0)
                        {
                            embed.AddField("Bitterness", bData.Bitterness, true);
                        }
                        if (bData.Dryness > 0)
                        {
                            embed.AddField("Dryness", bData.Dryness, true);
                        }
                        if (bData.Sourness > 0)
                        {
                            embed.AddField("Sourness", bData.Sourness, true);
                        }
                        if (bData.Spicyness > 0)
                        {
                            embed.AddField("Spicyness", bData.Spicyness, true);
                        }
                        if (bData.Sweetness > 0)
                        {
                            embed.AddField("Sweetness", bData.Sweetness, true);
                        }
                    }
                    await Context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                }
            }
        }

        [Group("move")]
        [Alias("attack")]
        public sealed class MoveCommands : ModuleBase<SocketCommandContext>
        {
            [Command("info")]
            [Alias("data")]
            public async Task Info([Remainder] string moveName)
            {
                PBEMove? nMove = PBELocalizedString.GetMoveByName(moveName);
                if (!nMove.HasValue || nMove.Value == PBEMove.None)
                {
                    await Context.Channel.SendMessageAsync($"{Context.User.Mention} Invalid move!");
                }
                else
                {
                    PBEMove move = nMove.Value;
                    PBEMoveData mData = PBEMoveData.Data[move];
                    EmbedBuilder embed = new EmbedBuilder()
                        .WithAuthor(Context.User)
                        .WithColor(Utils.TypeColors[mData.Type])
                        .WithTitle(PBELocalizedString.GetMoveName(move).English)
                        .WithUrl(Utils.URL)
                        .WithDescription(PBELocalizedString.GetMoveDescription(move).English.Replace('\n', ' '))
                        .AddField("Type", Utils.TypeEmotes[mData.Type], true)
                        .AddField("Category", mData.Category, true)
                        .AddField("Priority", mData.Priority, true)
                        .AddField("PP", Math.Max(1, mData.PPTier * PBESettings.DefaultPPMultiplier), true)
                        .AddField("Power", mData.Power == 0 ? "--" : mData.Power.ToString(), true)
                        .AddField("Accuracy", mData.Accuracy == 0 ? "--" : mData.Accuracy.ToString(), true);
                    switch (mData.Effect)
                    {
                        case PBEMoveEffect.Recoil: embed.AddField("Recoil", $"1/{mData.EffectParam} damage dealt", true); break;
                        case PBEMoveEffect.Recoil__10PercentBurn: embed.AddField("Recoil", "1/3 damage dealt", true); break; // TODO: Burn chance
                        case PBEMoveEffect.Recoil__10PercentParalyze: embed.AddField("Recoil", "1/3 damage dealt", true); break; // TODO: Paralyze chance
                        case PBEMoveEffect.Struggle: embed.AddField("Recoil", "1/4 user's max HP", true); break;
                    }
                    embed.AddField("Targets", mData.Targets, true)
                        .AddField("Flags", mData.Flags, true);
                    await Context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                }
            }
        }

        [Group("pokemon")]
        [Alias("pokémon", "species", "pkmn", "poke", "poké")]
        public sealed class SpeciesCommands : ModuleBase<SocketCommandContext>
        {
            [Command("info")]
            [Alias("data")]
            public async Task Info([Remainder] string input)
            {
                // Inputs for forms should be like "Giratina (Origin Forme)"
                Match m = Regex.Match(input, @"^(\S+) \((.+)\)$");
                string speciesName;
                string formName;
                if (m.Success)
                {
                    speciesName = m.Groups[1].Value;
                    formName = m.Groups[2].Value;
                }
                else
                {
                    speciesName = input;
                    formName = null;
                }
                PBESpecies? nSpecies = PBELocalizedString.GetSpeciesByName(speciesName);
                if (!nSpecies.HasValue)
                {
                    await Context.Channel.SendMessageAsync($"{Context.User.Mention} Invalid species!");
                }
                else
                {
                    PBESpecies species = nSpecies.Value;
                    speciesName = PBELocalizedString.GetSpeciesName(species).English;
                    PBEForm? nForm = formName == null ? 0 : PBELocalizedString.GetFormByName(species, formName);
                    if (!nForm.HasValue)
                    {
                        await Context.Channel.SendMessageAsync($"{Context.User.Mention} Invalid form for {speciesName}!");
                    }
                    else
                    {
                        PBEForm form = nForm.Value;
                        formName = PBEDataUtils.HasForms(species, false) ? $" ({PBELocalizedString.GetFormName(species, form).English})" : string.Empty;
                        var pData = PBEPokemonData.GetData(species, form);
                        string types = $"{Utils.TypeEmotes[pData.Type1]}";
                        if (pData.Type2 != PBEType.None)
                        {
                            types += $" {Utils.TypeEmotes[pData.Type2]}";
                        }
                        string ratio;
                        switch (pData.GenderRatio)
                        {
                            case PBEGenderRatio.M7_F1: ratio = "87.5% Male, 12.5% Female"; break;
                            case PBEGenderRatio.M3_F1: ratio = "75% Male, 25% Female"; break;
                            case PBEGenderRatio.M1_F1: ratio = "50% Male, 50% Female"; break;
                            case PBEGenderRatio.M1_F3: ratio = "25% Male, 75% Female"; break;
                            case PBEGenderRatio.M0_F1: ratio = "100% Female"; break;
                            case PBEGenderRatio.M1_F0: ratio = "100% Male"; break;
                            case PBEGenderRatio.M0_F0: ratio = "Genderless Species"; break;
                            default: throw new ArgumentOutOfRangeException(nameof(pData.GenderRatio));
                        }
                        string weaknesses = string.Empty,
                        resistances = string.Empty,
                        immunities = string.Empty;
                        for (PBEType atk = PBEType.None + 1; atk < PBEType.MAX; atk++)
                        {
                            double d = PBETypeEffectiveness.GetEffectiveness(atk, pData.Type1, pData.Type2);
                            if (d <= 0)
                            {
                                if (immunities != string.Empty)
                                {
                                    immunities += ' ';
                                }
                                immunities += Utils.TypeEmotes[atk];
                            }
                            else if (d < 1)
                            {
                                if (resistances != string.Empty)
                                {
                                    resistances += ' ';
                                }
                                resistances += Utils.TypeEmotes[atk];
                            }
                            if (d > 1)
                            {
                                if (weaknesses != string.Empty)
                                {
                                    weaknesses += ' ';
                                }
                                weaknesses += Utils.TypeEmotes[atk];
                            }
                        }
                        if (weaknesses == string.Empty)
                        {
                            weaknesses = "No Weaknesses";
                        }
                        if (resistances == string.Empty)
                        {
                            resistances = "No Resistances";
                        }
                        if (immunities == string.Empty)
                        {
                            immunities = "No Immunities";
                        }

                        EmbedBuilder embed = new EmbedBuilder()
                        .WithAuthor(Context.User)
                        .WithColor(Utils.GetColor(pData.Type1, pData.Type2))
                        .WithTitle($"{speciesName}{formName} - {PBELocalizedString.GetSpeciesCategory(species).English}")
                        .WithUrl(Utils.URL)
                        .WithDescription(PBELocalizedString.GetSpeciesEntry(species).English.Replace('\n', ' '))
                        .AddField("Types", types, true)
                        .AddField("Gender Ratio", ratio, true)
                        .AddField("Weight", $"{pData.Weight:N1} kg", true)
                        .AddField("Abilities", string.Join(", ", pData.Abilities.Select(a => PBELocalizedString.GetAbilityName(a).English)), false)
                        .AddField("HP", pData.BaseStats[0], true)
                        .AddField("Attack", pData.BaseStats[1], true)
                        .AddField("Defense", pData.BaseStats[2], true)
                        .AddField("Special Attack", pData.BaseStats[3], true)
                        .AddField("Special Defense", pData.BaseStats[4], true)
                        .AddField("Speed", pData.BaseStats[5], true)
                        .AddField("Type Weaknesses", weaknesses, true)
                        .AddField("Type Resistances", resistances, true)
                        .AddField("Type Immunities", immunities, true)
                        .WithImageUrl(Utils.GetPokemonSprite(species, form, PBERandom.RandomShiny(), PBERandom.RandomGender(pData.GenderRatio), false, false));
                        await Context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                    }
                }
            }
        }

        [Group("type")]
        public sealed class TypeCommands : ModuleBase<SocketCommandContext>
        {
            private const string _tableStart = "⬛";
            private const string _offense = "🗡️";
            private const string _defense = "🛡️";

            private const string _ineffective = "❌";
            private const string _notVeryEffective = "♿";
            private const string _effective = "✅";
            private const string _superEffective = "💥";

            [Command("info")]
            [Alias("data, effectiveness, weaknesses")]
            public async Task Info([Remainder] string typeName)
            {
                PBEType? nType = PBELocalizedString.GetTypeByName(typeName);
                if (!nType.HasValue || nType.Value == PBEType.None)
                {
                    await Context.Channel.SendMessageAsync($"{Context.User.Mention} Invalid type!");
                }
                else
                {
                    PBEType type = nType.Value;
                    string description = $"{_tableStart}{_tableStart}";
                    // Build columns
                    for (PBEType other = PBEType.None + 1; other < PBEType.MAX; other++)
                    {
                        description += $"|{Utils.TypeEmotes[other]}";
                    }
                    // Build rows
                    for (int i = 0; i < 2; i++)
                    {
                        bool doOffense = i == 0;
                        description += $"\n{(doOffense ? _offense : _defense)}{Utils.TypeEmotes[type]}";
                        for (PBEType other = PBEType.None + 1; other < PBEType.MAX; other++)
                        {
                            double d = PBETypeEffectiveness.GetEffectiveness(doOffense ? type : other, doOffense ? other : type);
                            string s;
                            if (d <= 0) // (-infinity, 0]
                            {
                                s = _ineffective;
                            }
                            else if (d < 1) // (0, 1)
                            {
                                s = _notVeryEffective;
                            }
                            else if (d == 1) // [1, 1]
                            {
                                s = _effective;
                            }
                            else // (1, infinity)
                            {
                                s = _superEffective;
                            }
                            description += $"|{s}";
                        }
                    }

                    EmbedBuilder embed = new EmbedBuilder()
                    .WithAuthor(Context.User)
                    .WithColor(Utils.TypeColors[type])
                    .WithTitle(PBELocalizedString.GetTypeName(type).English)
                    .WithUrl(Utils.URL)
                    .WithDescription(description);
                    await Context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                }
            }

            // BROKEN CUZ DISCORD CUTS OFF THE TABLE EVEN THOUGH YOU CAN SEND THE ENTIRE MESSAGE IN A CODE BLOCK
            /*[Command("chart")]
            [Alias("table")]
            public async Task Chart()
            {
                string description = _tableStart;
                // Build columns
                for (PBEType def = PBEType.None + 1; def < PBEType.MAX; def++)
                {
                    description += $"|{Utils.TypeEmotes[def]}";
                }
                // Build rows
                for (PBEType atk = PBEType.None + 1; atk < PBEType.MAX; atk++)
                {
                    description += $"\n{Utils.TypeEmotes[atk]}";
                    for (PBEType def = PBEType.None + 1; def < PBEType.MAX; def++)
                    {
                        double d = PBETypeEffectiveness.GetEffectiveness(atk, def);
                        string s;
                        if (d <= 0) // (-infinity, 0]
                        {
                            s = _ineffective;
                        }
                        else if (d < 1) // (0, 1)
                        {
                            s = _notVeryEffective;
                        }
                        else if (d == 1) // [1, 1]
                        {
                            s = _effective;
                        }
                        else // (1, infinity)
                        {
                            s = _superEffective;
                        }
                        description += $"|{s}";
                    }
                }

                EmbedBuilder embed = new EmbedBuilder()
                    .WithAuthor(Context.User)
                    .WithColor(Utils.RandomColor())
                    .WithTitle("Type Chart")
                    .WithUrl(Utils.URL)
                    .WithDescription(description);
                await Context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
            }*/
        }
    }
}
