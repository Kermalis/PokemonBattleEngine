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

            [Command("info")]
            public async Task Info(string moveName)
            {
                if (Enum.TryParse(moveName, out PMove move))
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
                        .WithTitle(moveName)
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
    }
}
