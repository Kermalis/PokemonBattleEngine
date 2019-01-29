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
                return pkmn == null ? string.Empty : $"{pkmn.Team.TrainerName}'s {pkmn.VisualNickname}";
            }

            string embedTitle = $"{context.Battlers[0].Username} vs {context.Battlers[1].Username}"; // TODO: Include turn number

            switch (packet)
            {
                case PBEMoveMissedPacket mmp:
                    {
                        PBEPokemon moveUser = mmp.MoveUserTeam.TryGetPokemon(mmp.MoveUser),
                            pokemon2 = mmp.Pokemon2Team.TryGetPokemon(mmp.Pokemon2);
                        var embed = new EmbedBuilder()
                            .WithColor(Utils.GetColor(moveUser))
                            .WithUrl("https://github.com/Kermalis/PokemonBattleEngine")
                            .WithTitle(embedTitle)
                            .WithDescription(string.Format("{0}'s attack missed {1}!", NameForTrainer(moveUser), NameForTrainer(pokemon2)))
                            .WithImageUrl(Utils.GetPokemonSprite(moveUser));
                        await context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                        break;
                    }
                case PBEMoveUsedPacket mup:
                    {
                        PBEPokemon moveUser = mup.MoveUserTeam.TryGetPokemon(mup.MoveUser);
                        var embed = new EmbedBuilder()
                            .WithColor(Utils.GetColor(moveUser))
                            .WithUrl("https://github.com/Kermalis/PokemonBattleEngine")
                            .WithTitle(embedTitle)
                            .WithDescription(string.Format("{0} used {1}!", NameForTrainer(moveUser), PBEMoveLocalization.Names[mup.Move].English))
                            .WithImageUrl(Utils.GetPokemonSprite(moveUser));
                        await context.Channel.SendMessageAsync(string.Empty, embed: embed.Build());
                        break;
                    }
                case PBEPkmnHPChangedPacket phcp:
                    {
                        PBEPokemon pokemon = phcp.PokemonTeam.TryGetPokemon(phcp.Pokemon);
                        int hp = Math.Abs(phcp.Change);
                        var embed = new EmbedBuilder()
                            .WithColor(Utils.GetColor(pokemon))
                            .WithUrl("https://github.com/Kermalis/PokemonBattleEngine")
                            .WithTitle(embedTitle)
                            .WithDescription(string.Format("{0} {1} {2} ({3:P2}) HP!", NameForTrainer(pokemon), phcp.Change <= 0 ? "lost" : "gained", hp, (double)hp / pokemon.MaxHP))
                            .WithImageUrl(Utils.GetPokemonSprite(pokemon));
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
                                    .WithDescription(string.Format("{1} sent out {0}!", pkmn.VisualNickname, psip.Team.TrainerName))
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
