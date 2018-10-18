using Ether.Network.Common;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Network;
using System;
using System.Threading;

namespace Kermalis.PokemonBattleEngineServer
{
    sealed class Player : NetUser
    {
        public readonly ManualResetEvent ResetEvent = new ManualResetEvent(true);
        public PTeamShell Team;
        
        public override void Send(INetPacketStream packet)
        {
            base.Send(packet);
            ResetEvent.Reset();
        }
        public override void HandleMessage(INetPacketStream packet)
        {
            Console.WriteLine($"Message received: \"{packet.GetType().Name}\" ({Id})");
            ResetEvent.Set();

            var ser = (BattleServer)Server;
            switch (packet)
            {
                case PRequestTeamPacket rtPack:
                    ser.TeamUpdated(this, rtPack.Team);
                    break;
            }
        }
    }
}
