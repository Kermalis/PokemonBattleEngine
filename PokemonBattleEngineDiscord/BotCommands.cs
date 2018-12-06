using Discord.Commands;
using Kermalis.PokemonBattleEngine.Data;
using System;
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
            [Command("info")]
            public async Task Info(string moveName)
            {
                if (Enum.TryParse(moveName, out PMove move))
                {
                    await Context.Channel.SendMessageAsync(PMoveData.Data[move].ToString());
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"{Context.User.Mention} Invalid move!");
                }
            }
        }
    }
}
