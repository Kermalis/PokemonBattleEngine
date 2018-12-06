using Discord;
using Discord.Commands;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
    public class BotCommands : ModuleBase<BattleCommandContext>
    {
        static readonly Dictionary<PType, Color> typeToColor = new Dictionary<PType, Color>
        {
            { PType.Bug, new Color(173, 189, 31) },
            { PType.Dark, new Color(115, 90, 74) },
            { PType.Dragon, new Color(123, 99, 231) },
            { PType.Electric, new Color(255, 198, 49) },
            { PType.Fighting, new Color(165, 82, 57) },
            { PType.Fire, new Color(247, 82, 49) },
            { PType.Flying, new Color(156, 173, 247) },
            { PType.Ghost, new Color(99, 99, 181) },
            { PType.Grass, new Color(123, 206, 82) },
            { PType.Ground, new Color(214, 181, 90) },
            { PType.Ice, new Color(90, 206, 231) },
            { PType.Normal, new Color(173, 165, 148) },
            { PType.Poison, new Color(181, 90, 165) },
            { PType.Psychic, new Color(255, 115, 165) },
            { PType.Rock, new Color(189, 165, 90) },
            { PType.Steel, new Color(173, 173, 198) },
            { PType.Water, new Color(57, 156, 255) }
        };

        [Command("player")]
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
            if (pIndex != -1 && i < PSettings.NumMoves)
            {
                await Context.Channel.SendMessageAsync(Context.BattleContext.Battle.Teams[pIndex].Party[0].Moves[i].ToString());
            }
        }

        [Group("move")]
        public class MoveCommands : ModuleBase<BattleCommandContext>
        {
            [Command("info")]
            public async Task Info(string moveName)
            {
                if (Enum.TryParse(moveName, true, out PMove move))
                {
                    if (move == PMove.None)
                    {
                        goto invalid;
                    }
                    PMoveData mData = PMoveData.Data[move];
                    var embed = new EmbedBuilder()
                        .WithColor(typeToColor[mData.Type])
                        .WithUrl("https://github.com/Kermalis/PokemonBattleEngine")
                        .WithCurrentTimestamp()
                        .WithTitle(move.ToString())
                        .WithAuthor(Context.User)
                        .AddField("Type", mData.Type, true)
                        .AddField("Category", mData.Category, true)
                        .AddField("Priority", mData.Priority, true)
                        .AddField("PP", mData.PPTier * PSettings.PPMultiplier, true)
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
        public class PokemonCommands : ModuleBase<BattleCommandContext>
        {
            [Command("info")]
            public async Task Info(string speciesName)
            {
                if (Enum.TryParse(speciesName, true, out PSpecies species))
                {
                    PPokemonData pData = PPokemonData.Data[species];
                    Color color = typeToColor[pData.Type1];
                    string types = pData.Type1.ToString();
                    if (pData.Type2 != PType.None)
                    {
                        types += ", " + pData.Type2.ToString();
                        color = color.Blend(typeToColor[pData.Type2]);
                    }
                    string ratio;
                    switch (pData.GenderRatio)
                    {
                        case PGenderRatio.M7_F1: ratio = "87.5% Male, 12.5% Female"; break;
                        case PGenderRatio.M3_F1: ratio = "75% Male, 25% Female"; break;
                        case PGenderRatio.M1_F1: ratio = "50% Male, 50% Female"; break;
                        case PGenderRatio.M1_F3: ratio = "25% Male, 75% Female"; break;
                        case PGenderRatio.M1_F7: ratio = "12.5% Male, 87.5% Female"; break;
                        case PGenderRatio.M0_F1: ratio = "0% Male, 100% Female"; break;
                        case PGenderRatio.M1_F0: ratio = "100% Male, 0% Female"; break;
                        case PGenderRatio.M0_F0: ratio = "Genderless Species"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(pData.GenderRatio), $"Invalid gender ratio: {pData.GenderRatio}");
                    }

                    var embed = new EmbedBuilder()
                        .WithColor(color)
                        .WithUrl("https://github.com/Kermalis/PokemonBattleEngine")
                        .WithCurrentTimestamp()
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
