using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data.Legality;
using Kermalis.PokemonBattleEngine.Network;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngineClient.Views;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Kermalis.PokemonBattleEngineClient.Clients
{
    internal sealed class NetworkClientConnection : IDisposable
    {
        private readonly PBEClient _client;
        private readonly PBELegalPokemonCollection _party;
        private readonly Action<object> _action;
        private byte _battleId;

        public NetworkClientConnection(string host, ushort port, PBELegalPokemonCollection party, Action<object> action)
        {
            _party = party;
            _action = action;
            _client = new PBEClient();
            _client.Disconnected += OnDisconnected;
            _client.Error += OnError;
            _client.PacketReceived += OnPacketReceived;
            if (_client.Connect(new IPEndPoint(IPAddress.Parse(host), port), 10 * 1000))
            {
                OnConnected();
            }
            else
            {
                _client.Dispose();
                action.Invoke(null);
            }
        }

        private void OnConnected()
        {
            Debug.WriteLine("Client connected to {0}", _client.RemoteIP);
            _action.Invoke("Waiting for players...");
        }
        private void OnDisconnected(object sender, EventArgs e)
        {
            Debug.WriteLine("Client disconnected from host");
            _action.Invoke(null);
        }
        private void OnError(object sender, Exception ex)
        {
            Debug.WriteLine("Client error: {0}", ex);
        }

        private void OnPacketReceived(object sender, IPBEPacket packet)
        {
            Debug.WriteLine($"Packet received (\"{packet.GetType().Name}\")");
            switch (packet)
            {
                case PBEMatchCancelledPacket _:
                {
                    _action.Invoke(null);
                    break;
                }
                case PBEPartyRequestPacket prp:
                {
                    _battleId = prp.BattleId;
                    if (prp.RequireLegal)
                    {
                        Send(new PBELegalPartyResponsePacket(_party));
                    }
                    else
                    {
                        Send(new PBEPartyResponsePacket(_party));
                    }
                    break;
                }
                case PBEBattlePacket bp:
                {
                    _client.Disconnected -= OnDisconnected;
                    _client.Error -= OnError;
                    _client.PacketReceived -= OnPacketReceived;
                    _action.Invoke(Tuple.Create(_client, bp, _battleId));
                    Send(new PBEResponsePacket());
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(packet));
            }
        }

        private void Send(IPBEPacket packet)
        {
            if (_client.IsConnected)
            {
                _client.Send(packet);
            }
        }

        public void Dispose()
        {
            _client.Dispose(); // Unsubscribe events
        }
    }
    internal sealed class NetworkClient : NonLocalClient
    {
        private readonly PBEClient _client;
        public override PBEBattle Battle { get; }
        public override PBETrainer Trainer { get; }
        public override BattleView BattleView { get; }
        public override bool HideNonOwned => true;

        public NetworkClient(PBEClient client, PBEBattlePacket bp, byte battleId)
        {
            var b = new PBEBattle(bp);
            Battle = b;
            if (battleId != byte.MaxValue)
            {
                Trainer = b.Trainers[battleId];
            }
            BattleView = new BattleView(this);
            client.Battle = b;
            _client = client;
            client.Disconnected += OnDisconnected;
            client.Error += OnError;
            client.PacketReceived += OnPacketReceived;
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            Debug.WriteLine("Client disconnected from host");
            BattleView.AddMessage("Disconnected from host.", messageBox: false);
        }
        private void OnError(object sender, Exception ex)
        {
            Debug.WriteLine("Client error: {0}", ex);
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
                    BattleView.AddMessage(string.Format("{0} joined the game.", pjp.TrainerName), messageBox: false);
                    Send(new PBEResponsePacket());
                    break;
                }
                case PBEActionsRequestPacket _:
                case PBESwitchInRequestPacket _:
                case PBEWinnerPacket _:
                {
                    Battle.Events.Add(packet);
                    Send(new PBEResponsePacket());
                    StartPacketThread();
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
            BattleView.AddMessage($"Waiting for {Trainer.Team.OpposingTeam.CombinedName}...", messageLog: false);
            Send(new PBEActionsResponsePacket(acts));
        }
        protected override void OnSwitchesReady()
        {
            BattleView.AddMessage($"Waiting for {(Trainer.Team.OpposingTeam.Trainers.Sum(t => t.SwitchInsRequired) > 0 ? Trainer.Team.OpposingTeam.CombinedName : "host")}...", messageLog: false);
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
            base.Dispose();
            _client.Dispose(); // Events unsubscribed
        }

        protected override bool ProcessPacket(IPBEPacket packet)
        {
            switch (packet)
            {
                case PBEMovePPChangedPacket mpcp:
                {
                    PBEBattlePokemon moveUser = mpcp.MoveUser;
                    if (moveUser.Trainer == Trainer)
                    {
                        moveUser.Moves[mpcp.Move].PP -= mpcp.AmountReduced;
                    }
                    break;
                }
                case PBEActionsRequestPacket arp:
                {
                    if (arp.Trainer == Trainer)
                    {
                        ActionsLoop(true);
                    }
                    else if (Trainer == null) // Spectators
                    {
                        BattleView.AddMessage("Waiting for players...", messageLog: false);
                    }
                    return true;
                }
                case PBESwitchInRequestPacket sirp:
                {
                    PBETrainer t = sirp.Trainer;
                    t.SwitchInsRequired = sirp.Amount;
                    if (t == Trainer)
                    {
                        _switchesRequired = sirp.Amount;
                        SwitchesLoop(true);
                    }
                    else if (Trainer == null) // Spectators
                    {
                        BattleView.AddMessage("Waiting for players...", messageLog: false);
                    }
                    else if (_switchesRequired == 0) // Don't display this message if we're in switchesloop because it'd overwrite the messages we need to see.
                    {
                        BattleView.AddMessage($"Waiting for {Trainer.Team.OpposingTeam.CombinedName}...", messageLog: false);
                    }
                    return true;
                }
            }
            return base.ProcessPacket(packet);
        }
    }
}
