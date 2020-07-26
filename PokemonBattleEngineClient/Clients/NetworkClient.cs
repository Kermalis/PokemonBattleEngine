using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data.Legality;
using Kermalis.PokemonBattleEngine.Network;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngineClient.Views;
using System;
using System.Diagnostics;
using System.Net;

namespace Kermalis.PokemonBattleEngineClient.Clients
{
    internal sealed class NetworkClientConnection : IDisposable
    {
        private readonly PBEClient _client;
        private readonly PBELegalPokemonCollection _party;
        private readonly Action<object> _action;
        private byte _battleId = byte.MaxValue; // Spectator by default

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
            Debug.WriteLine($"Connecting... connected to {_client.RemoteIP}");
            _action.Invoke("Waiting for players...");
        }
        private void OnDisconnected(object sender, EventArgs e)
        {
            Debug.WriteLine("Connecting... disconnected from host");
            _action.Invoke(null);
        }
        private void OnError(object sender, Exception ex)
        {
            Debug.WriteLine($"Connecting... error: {ex}");
        }

        private void OnPacketReceived(object sender, IPBEPacket packet)
        {
            Debug.WriteLine($"Connecting... received \"{packet.GetType().Name}\"");
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
                    // Response will be sent in NetworkClient constructor so the server doesn't send packets between threads
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

        public NetworkClient(PBEClient client, PBEBattlePacket bp, byte battleId, string name) : base(name)
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
            Send(new PBEResponsePacket());
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            Debug.WriteLine($"{Name} disconnected from host");
            BattleView.AddMessage("Disconnected from host.", messageBox: false);
        }
        private void OnError(object sender, Exception ex)
        {
            Debug.WriteLine($"{Name} error: {ex}");
        }
        private void OnPacketReceived(object sender, IPBEPacket packet)
        {
            Debug.WriteLine($"{Name} received \"{packet.GetType().Name}\"");
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
                case PBEBattleResultPacket _:
                {
                    Battle.Events.Add(packet);
                    Send(new PBEResponsePacket());
                    StartPacketThread();
                    break;
                }
                default:
                {
                    BattleView.AddMessage("Communicating...", messageLog: false);
                    Battle.Events.Add(packet);
                    Send(new PBEResponsePacket());
                    break;
                }
            }
        }

        protected override void OnActionsReady(PBETurnAction[] acts)
        {
            BattleView.AddMessage("Waiting for players...", messageLog: false);
            Send(new PBEActionsResponsePacket(acts));
        }
        protected override void OnSwitchesReady()
        {
            BattleView.AddMessage("Waiting for players...", messageLog: false);
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
                    PBEBattlePokemon moveUser = mpcp.MoveUserTrainer.TryGetPokemon(mpcp.MoveUser);
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
                    else if (Trainer == null || Trainer.NumConsciousPkmn == 0) // Spectators/KO'd
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
                    else if (_switchesRequired == 0) // No need to switch/Spectators/KO'd
                    {
                        BattleView.AddMessage("Waiting for players...", messageLog: false);
                    }
                    return true;
                }
            }
            return base.ProcessPacket(packet);
        }
    }
}
