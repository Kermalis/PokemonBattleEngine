using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
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
                PBETeamShell team0 = new PBETeamShell
                {
                    PlayerName = Context.User.Username,
                    Party = { PBECompetitivePokemonShells.Palkia_Uber }
                };
                PBETeamShell team1 = new PBETeamShell
                {
                    PlayerName = battler1.Username,
                    Party = { PBECompetitivePokemonShells.Dialga_Uber }
                };
                PBEBattle battle = new PBEBattle(PBEBattleStyle.Single, team0, team1);
                var battleContext = new BattleContext(battle, Context.User, battler1, Context.Channel);
            }
        }

        [Group("move")]
        public class MoveCommands : ModuleBase<SocketCommandContext>
        {
            [Command("info")]
            public async Task Info(string moveName)
            {
                if (Enum.TryParse(moveName, true, out PBEMove move))
                {
                    if (move == PBEMove.None)
                    {
                        goto invalid;
                    }
                    PBEMoveData mData = PBEMoveData.Data[move];
                    var embed = new EmbedBuilder()
                        .WithColor(Utils.TypeToColor[mData.Type])
                        .WithUrl("https://github.com/Kermalis/PokemonBattleEngine")
                        .WithTitle(move.ToString())
                        .WithAuthor(Context.User)
                        .AddField("Type", mData.Type, true)
                        .AddField("Category", mData.Category, true)
                        .AddField("Priority", mData.Priority, true)
                        .AddField("PP", mData.PPTier * PBESettings.PPMultiplier, true)
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
            public async Task Info(string speciesName)
            {
                if (Enum.TryParse(speciesName, true, out PBESpecies species))
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
                        case PBEGenderRatio.M0_F1: ratio = "0% Male, 100% Female"; break;
                        case PBEGenderRatio.M1_F0: ratio = "100% Male, 0% Female"; break;
                        case PBEGenderRatio.M0_F0: ratio = "Genderless Species"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(pData.GenderRatio), $"Invalid gender ratio: {pData.GenderRatio}");
                    }

                    var embed = new EmbedBuilder()
                        .WithColor(Utils.GetColor(species))
                        .WithUrl("https://github.com/Kermalis/PokemonBattleEngine")
                        .WithTitle(species.ToString())
                        .WithAuthor(Context.User)
                        .AddField("Types", types, true)
                        .AddField("Abilities", pData.Abilities.Print(false), true)
                        .AddField("Gender Ratio", ratio, false)
                        .AddField("Weight", $"{pData.Weight:N1} kg", true)
                        .AddField("Minimum Level", pData.MinLevel, true)
                        .AddField("Shiny Locked", pData.ShinyLocked ? "Yes" : "No", true)
                        .AddField("HP", pData.HP, true)
                        .AddField("Attack", pData.Attack, true)
                        .AddField("Defense", pData.Defense, true)
                        .AddField("Special Attack", pData.SpAttack, true)
                        .AddField("Special Defense", pData.SpDefense, true)
                        .AddField("Speed", pData.Speed, true);
                    await Context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                    return;
                }
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} Invalid species!");
            }
        }
    }
}
