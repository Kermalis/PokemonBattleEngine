using Ether.Network.Packets;
using Ether.Network.Server;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Network;
using System;
using System.Linq;

namespace Kermalis.PokemonBattleEngineServer
{
    sealed class BattleServer : NetServer<Player>
    {
        private enum ServerState
        {
            Startup, // Server is starting up
            Resetting, // Server is currently resetting the game
            WaitingForPlayers, // Server is waiting for 2 players to connect
            WaitingForParties, // Server is waiting for both players to send their party
            StartingMatch, // Server is starting the battle
            WaitingForActions, // Server is waiting for players to select actions
            BattleProcessing, // Battle is running and sending events
        }
        ServerState state = ServerState.Startup;
        Player[] battlers;
        PBattle battle;

        public void Forfeit(Player player)
        {
            // TODO
            CancelMatch(); // Temporary
        }
        void CancelMatch()
        {
            Console.WriteLine("Cancelling match...");
            SendToAll(new PMatchCancelledPacket());
            StopMatchAndReset();
        }
        void StopMatchAndReset()
        {
            if (state == ServerState.Resetting)
                return;
            state = ServerState.Resetting;

            foreach (Player c in Clients)
            {
                c.ResetEvent.Close();
                DisconnectClient(c.Id);
            }
            battlers = null;

            state = ServerState.WaitingForPlayers;
        }
        void PlayersFound()
        {
            Console.WriteLine("Two players connected!");

            battlers = new Player[] { Clients.ElementAt(0), Clients.ElementAt(1) };
            SendTo(battlers, new PRequestPartyPacket());

            Console.WriteLine("Waiting for teams...");
            state = ServerState.WaitingForParties;
        }
        public void PartySubmitted(Player player, PTeamShell team)
        {
            if (state != ServerState.WaitingForParties)
                return;
            player.Team = team;

            if (battlers[0].Team != null && battlers[1].Team != null)
            {
                // Temporary:
                try
                {
                    PPokemonShell.ValidateMany(battlers[0].Team.Party.Concat(battlers[1].Team.Party));
                }
                catch
                {
                    Console.WriteLine("Invalid Pokémon data received!");
                    CancelMatch();
                    return;
                }
                //
                Console.WriteLine("Both players ready!");
                StartMatch();
            }
        }
        void StartMatch()
        {
            if (state == ServerState.StartingMatch)
                return;
            state = ServerState.StartingMatch;

            Console.WriteLine("Battle starting!");

            battle = new PBattle(battlers[0].Team, battlers[1].Team);
            battle.NewEvent += PBattle.ConsoleBattleEventHandler;
            battle.NewEvent += BattleEventHandler;

            // Send opponent names
            battlers[0].Send(new PPlayerJoinedPacket(battlers[1].Id, battlers[1].Team.DisplayName));
            battlers[1].Send(new PPlayerJoinedPacket(battlers[0].Id, battlers[0].Team.DisplayName));
            WaitForBattlersResponses();
            // Send players their parties
            battlers[0].Send(new PSetPartyPacket(PKnownInfo.Instance.LocalParty));
            battlers[1].Send(new PSetPartyPacket(PKnownInfo.Instance.RemoteParty));
            WaitForBattlersResponses();

            state = ServerState.BattleProcessing;
            battle.Start();
            SendTo(battlers, new PRequestActionPacket());
            state = ServerState.WaitingForActions;
        }
        public void ActionsSubmitted(Player player, PSubmitActionsPacket.Action[] actions)
        {
            if (state != ServerState.WaitingForActions)
                return;

            // TODO: Verify info
            player.Actions = actions;

            if (battlers[0].Actions != null && battlers[1].Actions != null)
            {
                state = ServerState.BattleProcessing;
                foreach (var a in battlers[0].Actions.Concat(battlers[1].Actions))
                    battle.SelectAction(a.PokemonId, a.Param);
                // TODO: If battle ended
                //SendTo(battlers, new PRequestActionPacket());
                //state = ServerState.WaitingForActions;
            }
        }

        void WaitForBattlersResponses()
        {
            battlers[0].ResetEvent.WaitOne();
            battlers[1].ResetEvent.WaitOne();
        }
        void BattleEventHandler(INetPacketStream packet)
        {
            switch (packet)
            {
                case PPkmnSwitchInPacket psip:
                    foreach (Player client in Clients)
                    {
                        if (client == battlers[1])
                            psip.LocallyOwned = !psip.LocallyOwned; // Correctly set locally owned for this team
                        client.Send(packet);
                    }
                    WaitForBattlersResponses();
                    break;
                default:
                    SendToAll(packet);
                    WaitForBattlersResponses();
                    break;
            }
        }

        static readonly IPacketProcessor packetProcessor = new PPacketProcessor();
        protected override IPacketProcessor PacketProcessor => packetProcessor;
        public BattleServer(string host)
        {
            Configuration.Backlog = 50;
            Configuration.Host = host;
            Configuration.Port = 8888;
            Configuration.MaximumNumberOfConnections = 2; // Spectators allowed but not yet
            Configuration.BufferSize = 1024;
            Configuration.Blocking = true;
        }
        protected override void Initialize()
        {
            Console.WriteLine("Server online.");
            StopMatchAndReset();
        }
        protected override void OnClientConnected(Player client)
        {
            Console.WriteLine($"Client connected ({client.Id})");
            // TODO: If both players connect while the server is resetting
            if (state == ServerState.WaitingForPlayers && Clients.Count() == 2)
                PlayersFound();
        }
        protected override void OnClientDisconnected(Player client)
        {
            Console.WriteLine($"Client disconnected ({client.Id})");
            // TODO: If spectator disconnects
            switch (state)
            {
                case ServerState.Startup:
                case ServerState.Resetting:
                    break;
                case ServerState.WaitingForPlayers:
                case ServerState.WaitingForParties:
                    CancelMatch();
                    break;
                default:
                    Forfeit(client);
                    break;
            }
        }
        protected override void OnError(Exception e)
        {
            Console.WriteLine($"Server error: {e}");
        }
    }
}
