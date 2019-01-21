using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Localization;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static List<BattleContext> ActiveBattles { get; } = new List<BattleContext>();

        public PBEBattle Battle { get; }
        public SocketUser[] Battlers { get; }
        public ISocketMessageChannel Channel { get; }

        public BattleContext(PBEBattle battle, SocketUser battler0, SocketUser battler1, ISocketMessageChannel channel)
        {
            ActiveBattles.Add(this);

            Battle = battle;
            Battlers = new SocketUser[] { battler0, battler1 };
            Channel = channel;

            battle.OnNewEvent += (b, p) => Battle_OnNewEvent(ActiveBattles.Single(a => a.Battle == b), p).GetAwaiter().GetResult();
            battle.OnStateChanged += (b) => Battle_OnStateChanged(ActiveBattles.Single(a => a.Battle == b)).GetAwaiter().GetResult();
            Battle.Begin();
        }

        static async Task Battle_OnStateChanged(BattleContext context)
        {
            switch (context.Battle.BattleState)
            {
                case PBEBattleState.ReadyToRunTurn:
                    context.Battle.RunTurn();
                    break;
                case PBEBattleState.Ended:
                    ActiveBattles.Remove(context);
                    break;
            }
        }
        static async Task Battle_OnNewEvent(BattleContext context, INetPacket packet)
        {
            string NameForTrainer(PBEPokemon pkmn)
            {
                return pkmn == null ? string.Empty : $"{pkmn.Team.TrainerName}'s {pkmn.Shell.Nickname}";
            }

            string embedTitle = $"{context.Battlers[0].Username} vs {context.Battlers[1].Username}"; // TODO: Include turn number

            switch (packet)
            {
                case PBEMoveMissedPacket mmp:
                    {
                        PBEPokemon culprit = context.Battle.TryGetPokemon(mmp.Culprit),
                            victim = context.Battle.TryGetPokemon(mmp.Victim);
                        var embed = new EmbedBuilder()
                            .WithColor(Utils.GetColor(culprit))
                            .WithUrl("https://github.com/Kermalis/PokemonBattleEngine")
                            .WithTitle(embedTitle)
                            .WithDescription(string.Format("{0}'s attack missed {1}!", NameForTrainer(culprit), NameForTrainer(victim)))
                            .WithImageUrl(Utils.GetPokemonSprite(culprit));
                        await context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                        break;
                    }
                case PBEMoveUsedPacket mup:
                    {
                        PBEPokemon culprit = context.Battle.TryGetPokemon(mup.Culprit);
                        var embed = new EmbedBuilder()
                            .WithColor(Utils.GetColor(culprit))
                            .WithUrl("https://github.com/Kermalis/PokemonBattleEngine")
                            .WithTitle(embedTitle)
                            .WithDescription(string.Format("{0} used {1}!", NameForTrainer(culprit), PBEMoveLocalization.Names[mup.Move].English))
                            .WithImageUrl(Utils.GetPokemonSprite(culprit));
                        await context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                        break;
                    }
                case PBEPkmnHPChangedPacket phcp:
                    {
                        PBEPokemon victim = context.Battle.TryGetPokemon(phcp.Victim);
                        int hp = Math.Abs(phcp.Change);
                        var embed = new EmbedBuilder()
                            .WithColor(Utils.GetColor(victim))
                            .WithUrl("https://github.com/Kermalis/PokemonBattleEngine")
                            .WithTitle(embedTitle)
                            .WithDescription(string.Format("{0} {1} {2} ({3:P2}) HP!", NameForTrainer(victim), phcp.Change <= 0 ? "lost" : "gained", hp, (double)hp / victim.MaxHP))
                            .WithImageUrl(Utils.GetPokemonSprite(victim));
                        await context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                        break;
                    }
                case PBEPkmnSwitchInPacket psip:
                    {
                        if (!psip.Forced)
                        {
                            foreach (PBEPkmnSwitchInPacket.PBESwitchInInfo info in psip.SwitchIns)
                            {
                                PBEPokemon pkmn = context.Battle.TryGetPokemon(info.PokemonId);
                                var embed = new EmbedBuilder()
                                    .WithColor(Utils.GetColor(pkmn))
                                    .WithUrl("https://github.com/Kermalis/PokemonBattleEngine")
                                    .WithTitle(embedTitle)
                                    .WithDescription(string.Format("{1} sent out {0}!", pkmn.Shell.Nickname, psip.Team.TrainerName))
                                    .WithImageUrl(Utils.GetPokemonSprite(pkmn));
                                await context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                            }
                        }
                        break;
                    }
                case PBEActionsRequestPacket arp:
                    {
                        SocketUser guy = context.Battlers[Array.IndexOf(context.Battle.Teams, arp.Team)];
                        await guy.SendMessageAsync("Actions");
                        PBEBattle.SelectActionsIfValid(arp.Team, PokemonBattleEngine.AI.AIManager.CreateActions(arp.Team));
                        break;
                    }
                case PBESwitchInRequestPacket sirp:
                    {
                        SocketUser guy = context.Battlers[Array.IndexOf(context.Battle.Teams, sirp.Team)];
                        await guy.SendMessageAsync("Switches");
                        PBEBattle.SelectSwitchesIfValid(sirp.Team, PokemonBattleEngine.AI.AIManager.CreateSwitches(sirp.Team));
                        break;
                    }
            }
        }
    }
}
