using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Localization;
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
                PBEAbility ability = PBEAbility.None;
                PBELocalizedString localized = PBEAbilityLocalization.Names.Values.FirstOrDefault(l => l.Contains(abilityName));
                if (localized != null)
                {
                    ability = PBEAbilityLocalization.Names.First(p => p.Value == localized).Key;
                }
                else
                {
                    Enum.TryParse(abilityName, true, out ability);
                }
                if (ability == PBEAbility.None)
                {
                    await Context.Channel.SendMessageAsync($"{Context.User.Mention} Invalid ability!");
                }
                else
                {
                    var embed = new EmbedBuilder()
                        .WithAuthor(Context.User)
                        .WithColor(PBEUtils.Sample(Utils.TypeToColor).Value)
                        .WithTitle(PBEAbilityLocalization.Names[ability].English)
                        .WithUrl(Utils.URL)
                        .WithDescription(PBEAbilityLocalization.Descriptions[ability].English.Replace('\n', ' '));
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
                    PBEPokemonShell[] team0Party, team1Party;
                    // Completely Randomized Pokémon
                    team0Party = PBEUtils.CreateCompletelyRandomTeam(settings);
                    team1Party = PBEUtils.CreateCompletelyRandomTeam(settings);

                    // Randomized Competitive Pokémon
                    /*team0Party = PBECompetitivePokemonShells.CreateRandomTeam(settings.MaxPartySize).ToArray();
                    team1Party = PBECompetitivePokemonShells.CreateRandomTeam(settings.MaxPartySize).ToArray();*/

                    var battle = new PBEBattle(PBEBattleFormat.Single, settings, team0Party, team1Party);
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
                PBEItem item = PBEItem.None;
                PBELocalizedString localized = PBEItemLocalization.Names.Values.FirstOrDefault(l => l.Contains(itemName));
                if (localized != null)
                {
                    item = PBEItemLocalization.Names.First(p => p.Value == localized).Key;
                }
                else
                {
                    Enum.TryParse(itemName, true, out item);
                }
                if (item == PBEItem.None)
                {
                    await Context.Channel.SendMessageAsync($"{Context.User.Mention} Invalid item!");
                }
                else
                {
                    PBEItemData iData = PBEItemData.Data[item];
                    var embed = new EmbedBuilder()
                        .WithAuthor(Context.User)
                        .WithColor(iData.NaturalGiftType == PBEType.None ? Utils.RandomColor() : Utils.TypeToColor[iData.NaturalGiftType])
                        .WithTitle(PBEItemLocalization.Names[item].English)
                        .WithUrl(Utils.URL)
                        .WithDescription(PBEItemLocalization.Descriptions[item].English.Replace('\n', ' '));
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
                PBEMove move = PBEMove.None;
                PBELocalizedString localized = PBEMoveLocalization.Names.Values.FirstOrDefault(l => l.Contains(moveName));
                if (localized != null)
                {
                    move = PBEMoveLocalization.Names.First(p => p.Value == localized).Key;
                }
                else
                {
                    Enum.TryParse(moveName, true, out move);
                }
                if (move == PBEMove.None)
                {
                    await Context.Channel.SendMessageAsync($"{Context.User.Mention} Invalid move!");
                }
                else
                {
                    PBEMoveData mData = PBEMoveData.Data[move];
                    var embed = new EmbedBuilder()
                        .WithAuthor(Context.User)
                        .WithColor(Utils.TypeToColor[mData.Type])
                        .WithTitle(PBEMoveLocalization.Names[move].English)
                        .WithUrl(Utils.URL)
                        .WithDescription(PBEMoveLocalization.Descriptions[move].English.Replace('\n', ' '))
                        .AddField("Type", mData.Type, true)
                        .AddField("Category", mData.Category, true)
                        .AddField("Priority", mData.Priority, true)
                        .AddField("PP", mData.PPTier * PBESettings.DefaultSettings.PPMultiplier, true)
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
                PBESpecies species = 0;
                PBELocalizedString localized = PBEPokemonLocalization.Names.Values.FirstOrDefault(l => l.Contains(speciesName));
                if (localized != null)
                {
                    species = PBEPokemonLocalization.Names.First(p => p.Value == localized).Key;
                }
                else
                {
                    Enum.TryParse(speciesName, true, out species);
                }
                if (species == 0)
                {
                    await Context.Channel.SendMessageAsync($"{Context.User.Mention} Invalid species!");
                }
                else
                {
                    PBEPokemonData pData = PBEPokemonData.Data[species];
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

                    var noForme = (PBESpecies)((int)species & 0xFFFF);
                    var embed = new EmbedBuilder()
                        .WithAuthor(Context.User)
                        .WithColor(Utils.GetColor(species))
                        .WithTitle($"{PBEPokemonLocalization.Names[noForme].English} - {PBEPokemonLocalization.Categories[noForme].English}")
                        .WithUrl(Utils.URL)
                        .WithDescription(PBEPokemonLocalization.Entries[noForme].English.Replace('\n', ' '))
                        .AddField("Types", types, true)
                        .AddField("Gender Ratio", ratio, true)
                        .AddField("Weight", $"{pData.Weight:N1} kg", true)
                        .AddField("Abilities", string.Join(", ", pData.Abilities.Select(a => PBEAbilityLocalization.Names[a].English)), false)
                        .AddField("HP", pData.BaseStats[0], true)
                        .AddField("Attack", pData.BaseStats[1], true)
                        .AddField("Defense", pData.BaseStats[2], true)
                        .AddField("Special Attack", pData.BaseStats[3], true)
                        .AddField("Special Defense", pData.BaseStats[4], true)
                        .AddField("Speed", pData.BaseStats[5], true)
                        .WithImageUrl(Utils.GetPokemonSprite(species, PBEUtils.RNG.NextShiny(), PBEUtils.RNG.NextGender(species), false, false));
                    await Context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                }
            }
        }
    }
}
