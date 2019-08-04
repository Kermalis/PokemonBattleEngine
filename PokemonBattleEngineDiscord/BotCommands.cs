using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
    public class BotCommands : ModuleBase<SocketCommandContext>
    {
        [Group("ability")]
        public class AbilityCommands : ModuleBase<SocketCommandContext>
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

        [Group("battle")]
        public class BattleCommands : ModuleBase<SocketCommandContext>
        {
            [Command("challenge")]
            public async Task Challenge(SocketUser battler1)
            {
                if (battler1.Id == Context.User.Id)
                {
                    //
                }
                else if (BattleContext.GetBattleContext(Context.User) != null)
                {
                    await Context.Channel.SendMessageAsync($"{Context.User.Username} is already participating in a battle.");
                }
                else if (BattleContext.GetBattleContext(battler1) != null)
                {
                    await Context.Channel.SendMessageAsync($"{battler1.Username} is already participating in a battle.");
                }
                else
                {
                    PBESettings settings = PBESettings.DefaultSettings;
                    PBETeamShell team0Shell, team1Shell;
                    // Completely Randomized Pokémon
                    team0Shell = new PBETeamShell(settings, settings.MaxPartySize, true);
                    team1Shell = new PBETeamShell(settings, settings.MaxPartySize, true);

                    var battle = new PBEBattle(PBEBattleFormat.Single, team0Shell, team1Shell);
                    battle.Teams[0].TrainerName = Context.User.Username;
                    battle.Teams[1].TrainerName = battler1.Username;
                    var battleContext = new BattleContext(battle, Context.User, battler1, Context.Channel);
                }
            }
        }

        [Group("item")]
        public class ItemCommands : ModuleBase<SocketCommandContext>
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
                    EmbedBuilder embed = new EmbedBuilder()
                        .WithAuthor(Context.User)
                        .WithColor(iData.NaturalGiftType == PBEType.None ? Utils.RandomColor() : Utils.TypeToColor[iData.NaturalGiftType])
                        .WithTitle(PBELocalizedString.GetItemName(item).English)
                        .WithUrl(Utils.URL)
                        .WithDescription(PBELocalizedString.GetItemDescription(item).English.Replace('\n', ' '));
                    if (iData.FlingPower > 0)
                    {
                        embed.AddField("Fling Power", iData.FlingPower, true);
                    }
                    if (iData.NaturalGiftPower > 0)
                    {
                        embed.AddField("Natural Gift Power", iData.NaturalGiftPower, true);
                    }
                    if (iData.NaturalGiftType != PBEType.None)
                    {
                        embed.AddField("Natural Gift Type", iData.NaturalGiftType, true);
                    }
                    await Context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                }
            }
        }

        [Group("move")]
        [Alias("attack")]
        public class MoveCommands : ModuleBase<SocketCommandContext>
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
                        .WithColor(Utils.TypeToColor[mData.Type])
                        .WithTitle(PBELocalizedString.GetMoveName(move).English)
                        .WithUrl(Utils.URL)
                        .WithDescription(PBELocalizedString.GetMoveDescription(move).English.Replace('\n', ' '))
                        .AddField("Type", mData.Type, true)
                        .AddField("Category", mData.Category, true)
                        .AddField("Priority", mData.Priority, true)
                        .AddField("PP", Math.Max(1, mData.PPTier * PBESettings.DefaultPPMultiplier), true)
                        .AddField("Power", mData.Power == 0 ? "--" : mData.Power.ToString(), true)
                        .AddField("Accuracy", mData.Accuracy == 0 ? "--" : mData.Accuracy.ToString(), true);
                    switch (mData.Effect)
                    {
                        case PBEMoveEffect.FlareBlitz: embed.AddField("Recoil", "1/3 damage dealt", true); break; // TODO: Burn chance
                        case PBEMoveEffect.Recoil: embed.AddField("Recoil", $"1/{mData.EffectParam} damage dealt", true); break;
                        case PBEMoveEffect.Struggle: embed.AddField("Recoil", "1/4 user's max HP", true); break;
                        case PBEMoveEffect.VoltTackle: embed.AddField("Recoil", "1/3 damage dealt", true); break; // TODO: Paralyze chance
                    }
                    embed.AddField("Targets", mData.Targets, true)
                    .AddField("Flags", mData.Flags, true);
                    await Context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                }
            }
        }

        [Group("pokemon")]
        [Alias("pokémon", "species", "pkmn", "poke", "poké")]
        public class PokemonCommands : ModuleBase<SocketCommandContext>
        {
            [Command("info")]
            [Alias("data")]
            public async Task Info([Remainder] string speciesName)
            {
                PBESpecies? nSpecies = PBELocalizedString.GetSpeciesByName(speciesName);
                if (!nSpecies.HasValue)
                {
                    await Context.Channel.SendMessageAsync($"{Context.User.Mention} Invalid species!");
                }
                else
                {
                    PBESpecies species = nSpecies.Value;
                    var pData = PBEPokemonData.GetData(species);
                    string types = pData.Type1.ToString();
                    if (pData.Type2 != PBEType.None)
                    {
                        types += ", " + pData.Type2.ToString();
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

                    EmbedBuilder embed = new EmbedBuilder()
                        .WithAuthor(Context.User)
                        .WithColor(Utils.GetColor(pData.Type1, pData.Type2))
                        .WithTitle($"{PBELocalizedString.GetSpeciesName(species).English} - {PBELocalizedString.GetSpeciesCategory(species).English}")
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
                        .WithImageUrl(Utils.GetPokemonSprite(species, PBEUtils.RandomShiny(), PBEUtils.RandomGender(pData.GenderRatio), false, false));
                    await Context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                }
            }
        }
    }
}
