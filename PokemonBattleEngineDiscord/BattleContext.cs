using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
    public class BattleCommandContext : SocketCommandContext
    {
        public BattleContext BattleContext { get; }

        public BattleCommandContext(BattleContext battleContext, DiscordSocketClient client, SocketUserMessage msg)
            : base(client, msg)
        {
            BattleContext = battleContext;
        }
    }

    public class BattleContext
    {
        public PBEBattle Battle { get; }
        public SocketUser[] Battlers { get; }
        public ISocketMessageChannel Channel { get; }

        // Returns -1 if not a battler
        public int GetBattlerIndex(ulong userId)
        {
            if (Battlers == null)
            {
                return -1;
            }
            else if (Battlers[0].Id == userId)
            {
                return 0;
            }
            else if (Battlers[1].Id == userId)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public BattleContext(PBEBattle battle, SocketUser battler0, SocketUser battler1, ISocketMessageChannel channel)
        {
            Battle = battle;
            Battlers = new SocketUser[] { battler0, battler1 };
            Channel = channel;

            battle.OnNewEvent += (b, p) => Battle_OnNewEvent(b, p).GetAwaiter().GetResult();
            battle.OnStateChanged += (b) => Battle_OnStateChanged(b).GetAwaiter().GetResult();
            Battle.Begin();
        }

        async Task Battle_OnStateChanged(PBEBattle battle)
        {
            await Channel.SendMessageAsync($"Battle State Change: \"{battle.BattleState}\"");

            switch (battle.BattleState)
            {
                case PBEBattleState.ReadyToRunTurn:
                    battle.RunTurn();
                    break;
                case PBEBattleState.WaitingForActions:
                    await Channel.SendMessageAsync($"Send actions {Battlers[0].Mention} {Battlers[1].Mention}");
                    break;
                case PBEBattleState.WaitingForSwitchIns:
                    await Channel.SendMessageAsync($"Send switches {Battlers[0].Mention} {Battlers[1].Mention}");
                    break;
            }
        }
        async Task Battle_OnNewEvent(PBEBattle battle, INetPacket packet)
        {
            PBEPokemon culprit, victim;
            EmbedBuilder embed;
            string embedTitle = $"{Battlers[0].Username} vs {Battlers[1].Username}"; // TODO: Include turn number
            string message;

            switch (packet)
            {
                /*case PBEPkmnSwitchInPacket psip:
                    culprit = battle.GetPokemon(psip.PokemonId);
                    message = string.Format("{1} sent out {0}!", culprit.Shell.Nickname, battle.Teams[culprit.LocalTeam ? 0 : 1].TrainerName);
                    embed = new EmbedBuilder()
                        .WithColor(Utils.GetColor(culprit))
                        .WithUrl("https://github.com/Kermalis/PokemonBattleEngine")
                        .WithTitle(embedTitle)
                        .WithDescription(message)
                        .WithImageUrl("http://sprites.pokecheck.org/i/445.gif");
                    await Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                    break;*/
                default:
                    await Channel.SendMessageAsync($"Battle Event: \"{packet.GetType().Name}\"");
                    break;
            }
        }
    }
}
