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
            : base(new PBEBattle(PBEBattleTerrain.Plain, battleFormat, teamShell.Settings), ClientMode.Online)
        {
            _teamShell = teamShell;
            _client = new PBEClient { Battle = Battle };
            _client.Disconnected += OnDisconnected;
            _client.Error += OnError;
            _client.PacketReceived += OnPacketReceived;
        }

        public bool Connect(string host, ushort port)
        {
            if (_client.Connect(new IPEndPoint(IPAddress.Parse(host), port), 10 * 1000))
            {
                OnConnected();
                return true;
            }
            return false;
        }

        private void OnConnected()
        {
            Debug.WriteLine("Connected to {0}", _client.RemoteIP);
            BattleView.AddMessage("Connected to host.", messageBox: false);
        }
        private void OnDisconnected(object sender, EventArgs e)
        {
            Debug.WriteLine("Disconnected from host");
            BattleView.AddMessage("Disconnected from host.", messageBox: false);
        }
        private void OnError(object sender, Exception ex)
        {
            Debug.WriteLine("Error: {0}", ex);
        }

        private void OnPacketReceived(object sender, IPBEPacket packet)
        {
            Debug.WriteLine($"Packet received (\"{packet.GetType().Name}\")");
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
                        if (id < 2)
                        {
                            Team = Battle.Teams[id];
                            ShowRawValues0 = id == 0;
                            ShowRawValues1 = id == 1;
                        }
                    }
                    else
                    {
                        BattleView.AddMessage(string.Format("{0} joined the game.", pjp.TrainerName), messageBox: false);
                    }
                    if (id < 2)
                    {
                        Battle.Teams[id].TrainerName = pjp.TrainerName;
                    }
                    Send(new PBEResponsePacket());
                    break;
                }
                case PBEPartyRequestPacket _:
                {
                    Send(new PBEPartyResponsePacket(_teamShell));
                    break;
                }
                case PBEActionsRequestPacket _:
                case PBESwitchInRequestPacket _:
                case PBEWinnerPacket _:
                {
                    Battle.Events.Add(packet);
                    StartPacketThread();
                    Send(new PBEResponsePacket());
                    break;
                }
                default:
                {
                    Battle.Events.Add(packet);
                    Send(new PBEResponsePacket());
                    break;
                }
            }
        }

        protected override void OnActionsReady(PBETurnAction[] acts)
        {
            BattleView.AddMessage($"Waiting for {Team.OpposingTeam.TrainerName}...", messageLog: false);
            Send(new PBEActionsResponsePacket(acts));
        }
        protected override void OnSwitchesReady()
        {
            BattleView.AddMessage($"Waiting for {(Team.OpposingTeam.SwitchInsRequired > 0 ? Team.OpposingTeam.TrainerName : "host")}...", messageLog: false);
            Send(new PBESwitchInResponsePacket(Switches));
        }

        private void Send(IPBEPacket packet)
        {
            if (_client.IsConnected)
            {
                _client.Send(packet);
            }
        }

        public override void Dispose()
        {
            _client.Dispose();
            _client.Disconnected -= OnDisconnected;
            _client.Error -= OnError;
            _client.PacketReceived -= OnPacketReceived;
        }
    }
}
