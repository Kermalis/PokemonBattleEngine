using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Network;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Diagnostics;
using System.Net;

namespace Kermalis.PokemonBattleEngineClient
{
    internal sealed class NetworkClient : BattleClient
    {
        private readonly PBEClient _client;
        private readonly PBETeamShell _teamShell;

        public NetworkClient(PBEBattleFormat battleFormat, PBETeamShell teamShell)
            : base(new PBEBattle(battleFormat, teamShell.Settings), ClientMode.Online)
        {
            _teamShell = teamShell;
            _client = new PBEClient { Battle = Battle };
            _client.Disconnected += OnDisconnected;
            _client.Error += OnError;
            _client.MessageReceived += HandleMessage;
        }

        public bool Connect(string host, ushort port)
        {
            if (_client.Connect(new IPEndPoint(IPAddress.Parse(host), port), -1))
            {
                OnConnected();
                return true;
            }
            return false;
        }

        private void OnConnected()
        {
            Debug.WriteLine("Connected to {0}", "ur mum");
            //Debug.WriteLine("Connected to {0}", _client.Socket.RemoteEndPoint);
            BattleView.AddMessage("Waiting for players...", messageBox: false);
        }
        private void OnDisconnected(object sender, EventArgs e)
        {
            Debug.WriteLine("Disconnected from server");
            BattleView.AddMessage("Disconnected from server.", messageBox: false);
        }
        private void OnError(object sender, Exception ex)
        {
            Debug.WriteLine("Error: {0}", ex);
        }

        private void HandleMessage(object sender, IPBEPacket packet)
        {
            Debug.WriteLine($"Message received: \"{packet.GetType().Name}\"");
            switch (packet)
            {
                case PBEMatchCancelledPacket _:
                {
                    BattleView.AddMessage("Match cancelled!", messageBox: false);
                    break;
                }
                case PBEPlayerJoinedPacket pjp:
                {
                    int id = pjp.BattleId;
                    if (pjp.IsMe)
                    {
                        BattleId = id;
                        Team = Battle.Teams[id];
                        ShowRawValues0 = id == 0;
                        ShowRawValues1 = id == 1;
                    }
                    else
                    {
                        BattleView.AddMessage(string.Format("{0} joined the game.", pjp.TrainerName), messageBox: false);
                    }
                    if (id < 2)
                    {
                        Battle.Teams[id].TrainerName = pjp.TrainerName;
                    }
                    _client.Send(new PBEResponsePacket());
                    break;
                }
                case PBEPartyRequestPacket _:
                {
                    _client.Send(new PBEPartyResponsePacket(_teamShell));
                    break;
                }
                case PBEActionsRequestPacket _:
                case PBESwitchInRequestPacket _:
                case PBEWinnerPacket _:
                {
                    Battle.Events.Add(packet);
                    StartPacketThread();
                    _client.Send(new PBEResponsePacket());
                    break;
                }
                default:
                {
                    Battle.Events.Add(packet);
                    _client.Send(new PBEResponsePacket());
                    break;
                }
            }
        }

        protected override void OnActionsReady(PBETurnAction[] acts)
        {
            BattleView.AddMessage($"Waiting for {Team.OpposingTeam.TrainerName}...", messageLog: false);
            _client.Send(new PBEActionsResponsePacket(acts));
        }
        protected override void OnSwitchesReady()
        {
            BattleView.AddMessage($"Waiting for {(Team.OpposingTeam.SwitchInsRequired > 0 ? Team.OpposingTeam.TrainerName : "server")}...", messageLog: false);
            _client.Send(new PBESwitchInResponsePacket(Switches));
        }
    }
}
