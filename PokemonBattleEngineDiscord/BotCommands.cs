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
        /*[Command("player")]
        public async Task Player(byte i)
        {
            if (i < 2)
            {
                await Context.Channel.SendMessageAsync(Context.BattleContext.Battlers[i].Mention);
            }
        }

        [Command("what")]
        public async Task What(byte i)
        {
            int pIndex = Context.BattleContext.GetBattlerIndex(Context.User.Id);
            if (pIndex != -1 && i < PBESettings.NumMoves)
            {
                await Context.Channel.SendMessageAsync(Context.BattleContext.Battle.Teams[pIndex].Party[0].Moves[i].ToString());
            }
        }*/

        [Group("battle")]
        public class BattleCommands : ModuleBase<SocketCommandContext>
        {
            [Command("challenge")]
            public async Task Challenge(SocketUser battler1)
            {
                if (BattleContext.ActiveBattles.Any(b => b.Battlers.Contains(battler1)))
                {
                    await Context.Channel.SendMessageAsync($"{battler1.Username} is already participating in a battle.");
                }
                else if (BattleContext.ActiveBattles.Any(b => b.Battlers.Contains(Context.User)))
                {
                    await Context.Channel.SendMessageAsync($"{Context.User.Username} is already participating in a battle.");
                }
                else
                {
                    PBEPokemonShell[] team0Party = PBECompetitivePokemonShells.CreateRandomTeam(PBESettings.DefaultSettings.MaxPartySize).ToArray();
                    PBEPokemonShell[] team1Party = PBECompetitivePokemonShells.CreateRandomTeam(PBESettings.DefaultSettings.MaxPartySize).ToArray();
                    var battle = new PBEBattle(PBEBattleFormat.Single, PBESettings.DefaultSettings, team0Party, team1Party);
                    battle.Teams[0].TrainerName = Context.User.Username;
                    battle.Teams[1].TrainerName = battler1.Username;
                    var battleContext = new BattleContext(battle, Context.User, battler1, Context.Channel);
                }
            }
        }

        [Group("move")]
        public class MoveCommands : ModuleBase<SocketCommandContext>
        {
            [Command("info")]
            public async Task Info([Remainder] string moveName)
            {
                PBEMove move = 0;
                PBELocalizedString localized = PBEMoveLocalization.Names.Values.FirstOrDefault(l => l.Contains(moveName));
                if (localized != null)
                {
                    move = PBEMoveLocalization.Names.First(p => p.Value == localized).Key;
                }
                else
                {
                    Enum.TryParse(moveName, true, out move);
                }
                if (move != 0)
                {
                    if (move == PBEMove.None)
                    {
                        goto invalid;
                    }
                    PBEMoveData mData = PBEMoveData.Data[move];
                    var embed = new EmbedBuilder()
                        .WithColor(Utils.TypeToColor[mData.Type])
                        .WithUrl("https://github.com/Kermalis/PokemonBattleEngine")
                        .WithTitle(PBEMoveLocalization.Names[move].English)
                        .WithAuthor(Context.User)
                        .AddField("Type", mData.Type, true)
                        .AddField("Category", mData.Category, true)
                        .AddField("Priority", mData.Priority, true)
                        .AddField("PP", mData.PPTier * PBESettings.DefaultSettings.PPMultiplier, true)
                        .AddField("Power", mData.Power == 0 ? "--" : mData.Power.ToString(), true)
                        .AddField("Accuracy", mData.Accuracy == 0 ? "--" : mData.Accuracy.ToString(), true)
                        .AddField("Effect", mData.Effect, true)
                        .AddField("Effect Param", mData.EffectParam, true)
                        .AddField("Targets", mData.Targets, true)
                        .AddField("Flags", mData.Flags, true);
                    await Context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                    return;
                }
            invalid:
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} Invalid move!");
            }
        }

        [Group("pokemon")]
        public class PokemonCommands : ModuleBase<SocketCommandContext>
        {
            [Command("info")]
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
                if (species != 0)
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
                        case PBEGenderRatio.M1_F7: ratio = "12.5% Male, 87.5% Female"; break;
                        case PBEGenderRatio.M0_F1: ratio = "100% Female"; break;
                        case PBEGenderRatio.M1_F0: ratio = "100% Male"; break;
                        case PBEGenderRatio.M0_F0: ratio = "Genderless Species"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(pData.GenderRatio));
                    }

                    var embed = new EmbedBuilder()
                        .WithColor(Utils.GetColor(species))
                        .WithUrl("https://github.com/Kermalis/PokemonBattleEngine")
                        .WithTitle(PBEPokemonLocalization.Names[species].English)
                        .WithAuthor(Context.User)
                        .WithImageUrl(Utils.GetPokemonSprite(species, PBEUtils.RNG.NextShiny(), PBEUtils.RNG.NextGender(species), false, false))
                        .AddField("Types", types, true)
                        .AddField("Gender Ratio", ratio, true)
                        .AddField("Weight", $"{pData.Weight:N1} kg", true)
                        .AddField("Abilities", string.Join(", ", pData.Abilities.Select(a => PBEAbilityLocalization.Names[a].English)), false)
                        .AddField("HP", pData.BaseStats[0], true)
                        .AddField("Attack", pData.BaseStats[1], true)
                        .AddField("Defense", pData.BaseStats[2], true)
                        .AddField("Special Attack", pData.BaseStats[3], true)
                        .AddField("Special Defense", pData.BaseStats[4], true)
                        .AddField("Speed", pData.BaseStats[5], true);
                    await Context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                    return;
                }
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} Invalid species!");
            }
        }
    }
}
