using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data.Legality;
using Kermalis.PokemonBattleEngine.Network;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngineClient.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Kermalis.PokemonBattleEngineClient.Clients
{
    internal sealed class NetworkClientConnection : IDisposable
    {
        private readonly PBEClient _client;
        private readonly PBELegalPokemonCollection _party;
        private readonly Action<object?> _action;
        private byte _battleId = byte.MaxValue; // Spectator by default

        public NetworkClientConnection(string host, ushort port, PBELegalPokemonCollection party, Action<object?> action)
        {
            _party = party;
            _action = action;
            _client = new PBEClient();
            _client.Disconnected += OnDisconnected;
            _client.Error += OnError;
            _client.PacketReceived += OnPacketReceived;
            if (_client.Connect(new IPEndPoint(IPAddress.Parse(host), port), 10 * 1000, new PBEPacketProcessor()))
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
        private void OnDisconnected(object? sender, EventArgs e)
        {
            Debug.WriteLine("Connecting... disconnected from host");
            _action.Invoke(null);
        }
        private void OnError(object? sender, Exception ex)
        {
            Debug.WriteLine($"Connecting... error: {ex}");
        }

        private void OnPacketReceived(object? sender, IPBEPacket packet)
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
        public override PBETrainer? Trainer { get; }
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
            ShowAllPokemon();
            Send(new PBEResponsePacket());
        }

        private void OnDisconnected(object? sender, EventArgs e)
        {
            Debug.WriteLine($"{Name} disconnected from host");
            BattleView.AddMessage("Disconnected from host.", messageBox: false);
        }
        private void OnError(object? sender, Exception ex)
        {
            Debug.WriteLine($"{Name} error: {ex}");
        }
        private void OnPacketReceived(object? sender, IPBEPacket packet)
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

        private void OnActionsReady(Queue<PBETurnAction> acts)
        {
            BattleView.AddMessage("Waiting for players...", messageLog: false);
            Send(new PBEActionsResponsePacket(acts.ToArray()));
        }
        private void OnSwitchesReady(Queue<PBESwitchIn> switches)
        {
            BattleView.AddMessage("Waiting for players...", messageLog: false);
            Send(new PBESwitchInResponsePacket(switches.ToArray()));
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
                    PBEBattlePokemon moveUser = mpcp.MoveUserTrainer.GetPokemon(mpcp.MoveUser);
                    if (moveUser.Trainer == Trainer)
                    {
                        moveUser.Moves[mpcp.Move]!.PP -= mpcp.AmountReduced;
                    }
                    break;
                }
                case PBEActionsRequestPacket arp:
                {
                    if (arp.Trainer == Trainer)
                    {
                        _ = new ActionsBuilder(BattleView, Trainer, OnActionsReady);
                    }
                    else if (Trainer is null || Trainer.NumConsciousPkmn == 0) // Spectators/KO'd
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
                        _ = new SwitchesBuilder(BattleView, sirp.Amount, OnSwitchesReady);
                    }
                    else if (BattleView.Actions.SwitchesBuilder?.SwitchesRequired == 0) // No need to switch/Spectators/KO'd
                    {
                        BattleView.AddMessage("Waiting for players...", messageLog: false);
                    }
                    return true;
                }
                case PBEPkmnFaintedPacket_Hidden pfph:
                {
                    bool ret = base.ProcessPacket(packet); // Process before removal
                    PBEBattlePokemon pokemon = pfph.PokemonTrainer.GetPokemon(pfph.OldPosition);
                    Battle.ActiveBattlers.Remove(pokemon);
                    pokemon.FieldPosition = PBEFieldPosition.None;
                    PBETrainer.Remove(pokemon);
                    return ret;
                }
                case PBEPkmnFormChangedPacket_Hidden pfcph:
                {
                    PBEBattlePokemon pokemon = pfcph.PokemonTrainer.GetPokemon(pfcph.Pokemon);
                    pokemon.HPPercentage = pfcph.NewHPPercentage;
                    pokemon.KnownAbility = pfcph.NewKnownAbility;
                    pokemon.KnownForm = pfcph.NewForm;
                    pokemon.KnownType1 = pfcph.NewType1;
                    pokemon.KnownType2 = pfcph.NewType2;
                    pokemon.KnownWeight = pfcph.NewWeight;
                    break;
                }
                case PBEPkmnHPChangedPacket_Hidden phcph:
                {
                    PBEBattlePokemon pokemon = phcph.PokemonTrainer.GetPokemon(phcph.Pokemon);
                    pokemon.HPPercentage = phcph.NewHPPercentage;
                    break;
                }
                case PBEPkmnSwitchInPacket_Hidden psiph:
                {
                    foreach (PBEPkmnSwitchInPacket_Hidden.PBEPkmnSwitchInInfo info in psiph.SwitchIns)
                    {
                        _ = new PBEBattlePokemon(psiph.Trainer, info);
                    }
                    break;
                }
                case PBEPkmnSwitchOutPacket_Hidden psoph:
                {
                    bool ret = base.ProcessPacket(packet); // Process before removal
                    PBEBattlePokemon pokemon = psoph.PokemonTrainer.GetPokemon(psoph.OldPosition);
                    Battle.ActiveBattlers.Remove(pokemon);
                    PBETrainer.Remove(pokemon);
                    return ret;
                }
                case PBEReflectTypePacket_Hidden rtph:
                {
                    PBEBattlePokemon user = rtph.UserTrainer.GetPokemon(rtph.User);
                    PBEBattlePokemon target = rtph.TargetTrainer.GetPokemon(rtph.Target);
                    user.Type1 = user.KnownType1 = target.KnownType1; // Set Type1 and Type2 so Transform works
                    user.Type2 = user.KnownType2 = target.KnownType2;
                    break;
                }
                case PBEWildPkmnAppearedPacket wpap:
                {
                    PBETrainer wildTrainer = Battle.Teams[1].Trainers[0];
                    foreach (PBEPkmnAppearedInfo info in wpap.Pokemon)
                    {
                        PBEBattlePokemon pokemon = wildTrainer.GetPokemon(info.Pokemon);
                        pokemon.FieldPosition = info.FieldPosition;
                        Battle.ActiveBattlers.Add(pokemon);
                    }
                    break;
                }
                case PBEWildPkmnAppearedPacket_Hidden wpaph:
                {
                    foreach (PBEWildPkmnAppearedPacket_Hidden.PBEWildPkmnInfo info in wpaph.Pokemon)
                    {
                        _ = new PBEBattlePokemon(Battle, info);
                    }
                    break;
                }
            }
            return base.ProcessPacket(packet);
        }
    }
}
