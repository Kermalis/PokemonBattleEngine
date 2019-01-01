using Ether.Network.Packets;
using Ether.Network.Server;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngineServer
{
    sealed class BattleServer : NetServer<Player>
    {
        private enum ServerState
        {
            Startup, // Server is starting up
            Resetting, // Server is currently resetting the game
            Cancelling, // Server is cancelling the game
            WaitingForPlayers, // Server is waiting for 2 players to connect
            WaitingForParties, // Server is waiting for both players to send their party
            StartingMatch, // Server is starting the battle
            WaitingForActions, // Server is waiting for players to select actions
            BattleProcessing, // Battle is running and sending events
            WaitingForSwitchIns, // Server is waiting for players to switch in new Pokémon
        }
        ServerState state = ServerState.Startup;
        readonly PBEBattle battle;
        Player[] battlers;

        public override IPacketProcessor PacketProcessor { get; }
        public static void Main(string[] args)
        {
            using (var server = new BattleServer("127.0.0.1", 8888))
            {
                server.Start();
            }
        }
        public BattleServer(string host, int port)
        {
            Configuration.Backlog = 50;
            Configuration.Host = host;
            Configuration.Port = port;
            Configuration.MaximumNumberOfConnections = 2; // Spectators allowed but not yet
            Configuration.BufferSize = 1024;
            Configuration.Blocking = true;

            battle = new PBEBattle(PBEBattleFormat.Double, PBESettings.DefaultSettings);
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.OnNewEvent += BattleEventHandler;
            battle.OnStateChanged += BattleStateHandler;
            PacketProcessor = new PBEPacketProcessor(battle);
        }
        protected override void Initialize()
        {
            Console.WriteLine("Server online.");
            StopMatchAndReset();
        }
        protected override void OnClientConnected(Player client)
        {
            // Set new player's info
            client.Index = (byte)(Clients.Count() - 1);
            client.PlayerName = PBEUtils.Sample(new string[] { "Sasha", "Nikki", "Lara", "Violet", "Naomi", "Rose", "Sabrina", "Nicole" });
            // Send all already connected players to this client
            foreach (Player player in Clients)
            {
                player.Send(new PBEPlayerJoinedPacket(player == client, client.Index, client.PlayerName));
                if (player != client)
                {
                    client.Send(new PBEPlayerJoinedPacket(false, player.Index, player.PlayerName));
                }
            }
            Console.WriteLine($"Client connected ({client.Id} {client.Index} {client.PlayerName})");
            if (battlers == null && Clients.Count() == 2)
            {
                Console.WriteLine("Two players connected!");
                state = ServerState.WaitingForParties;
                Console.WriteLine("Waiting for parties...");
                battlers = Clients.OrderBy(c => c.Index).Take(2).ToArray();
                battle.Teams[0].TrainerName = battlers[0].PlayerName;
                battle.Teams[1].TrainerName = battlers[1].PlayerName;
                SendTo(battlers, new PBEPartyRequestPacket());
            }
        }
        protected override void OnClientDisconnected(Player client)
        {
            Console.WriteLine($"Client disconnected ({client.Id})");
            // Temporarily ignore spectators
            if (client.Index >= 2)
            {
                return;
            }
            switch (state)
            {
                case ServerState.Startup:
                case ServerState.Resetting:
                case ServerState.Cancelling:
                    break;
                default:
                    CancelMatch();
                    break;
            }
        }
        protected override void OnError(Exception e)
        {
            Console.WriteLine($"Server error: {e}");
        }

        void CancelMatch()
        {
            Environment.Exit(0); // Temporary
            if (state == ServerState.Cancelling)
            {
                return;
            }
            lock (this)
            {
                if (state == ServerState.Cancelling)
                {
                    return;
                }
                state = ServerState.Cancelling;
                Console.WriteLine("Cancelling match...");
                SendToAll(new PBEMatchCancelledPacket());
                StopMatchAndReset();
            }
        }
        void StopMatchAndReset()
        {
            if (state == ServerState.Resetting)
            {
                return;
            }
            lock (this)
            {
                if (state == ServerState.Resetting)
                {
                    return;
                }
                state = ServerState.Resetting;
                foreach (Player c in Clients)
                {
                    c.ResetEvent.Close();
                    DisconnectClient(c.Id);
                }
                battlers = null;
                state = ServerState.WaitingForPlayers;
            }
        }
        public void PartySubmitted(Player player)
        {
            if (state != ServerState.WaitingForParties)
            {
                return;
            }
            lock (this)
            {
                if (state != ServerState.WaitingForParties)
                {
                    return;
                }
                PBEBattle.CreateTeamParty(battle.Teams[player.Index], player.Party);
            }
        }
        public void ActionsSubmitted(Player player, IEnumerable<PBEAction> actions)
        {
            if (state != ServerState.WaitingForActions)
            {
                return;
            }
            lock (this)
            {
                if (state != ServerState.WaitingForActions)
                {
                    return;
                }
                PBETeam team = battle.Teams[player.Index];
                Console.WriteLine($"Received actions from {team.TrainerName}!");
                if (!PBEBattle.SelectActionsIfValid(team, actions))
                {
                    Console.WriteLine("Actions are invalid!");
                    player.Send(new PBEActionsRequestPacket(team));
                }
            }
        }
        public void SwitchesSubmitted(Player player, IEnumerable<Tuple<byte, PBEFieldPosition>> switches)
        {
            if (state != ServerState.WaitingForSwitchIns)
            {
                return;
            }
            lock (this)
            {
                if (state != ServerState.WaitingForSwitchIns)
                {
                    return;
                }
                PBETeam team = battle.Teams[player.Index];
                Console.WriteLine($"Received switches from {team.TrainerName}!");
                if (!PBEBattle.SelectSwitchesIfValid(team, switches))
                {
                    Console.WriteLine("Switches are invalid!");
                    player.Send(new PBESwitchInRequestPacket(team));
                }
            }
        }

        void BattleStateHandler(PBEBattle battle)
        {
            Console.WriteLine("Battle state changed: {0}", battle.BattleState);
            switch (battle.BattleState)
            {
                case PBEBattleState.ReadyToBegin:
                    try
                    {
                        PBEPokemonShell.ValidateMany(battlers.SelectMany(b => b.Party), battle.Settings);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Invalid Team data received!");
                        Console.WriteLine(e.Message);
                        CancelMatch();
                        return;
                    }
                    state = ServerState.StartingMatch;
                    Console.WriteLine("Battle starting!");
                    battlers[0].Send(new PBESetPartyPacket(battle.Teams[0]));
                    battlers[1].Send(new PBESetPartyPacket(battle.Teams[1]));
                    battlers[0].ResetEvent.WaitOne();
                    battlers[1].ResetEvent.WaitOne();
                    battle.Begin();
                    break;
                case PBEBattleState.Processing:
                    state = ServerState.BattleProcessing;
                    break;
                case PBEBattleState.ReadyToRunTurn:
                    battle.RunTurn();
                    break;
                case PBEBattleState.WaitingForActions:
                    state = ServerState.WaitingForActions;
                    Console.WriteLine("Waiting for actions...");
                    break;
                case PBEBattleState.WaitingForSwitchIns:
                    state = ServerState.WaitingForSwitchIns;
                    Console.WriteLine("Waiting for switches...");
                    break;
            }
        }
        void BattleEventHandler(PBEBattle battle, INetPacket packet)
        {
            switch (packet)
            {
                case PBETransformPacket tp:
                    {
                        PBEPokemon user = battle.TryGetPokemon(tp.User),
                            target = battle.TryGetPokemon(tp.Target);
                        if (user.Team.Id == 0 || target.Team.Id == 0)
                        {
                            battlers[0].Send(tp);
                            battlers[0].ResetEvent.WaitOne();
                        }
                        if (user.Team.Id == 1 || target.Team.Id == 1)
                        {
                            battlers[1].Send(tp);
                            battlers[1].ResetEvent.WaitOne();
                        }
                    }
                    break;
                default:
                    SendToAll(packet);
                    foreach (Player player in Clients)
                    {
                        player.ResetEvent.WaitOne();
                    }
                    break;
            }
        }
    }
}
