using Ether.Network.Common;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Packets;
using System;

namespace Kermalis.PokemonBattleEngineServer
{
    class Player : NetUser
    {
        public TeamShell Team;

        public override void HandleMessage(INetPacketStream packet)
        {
            var ser = (BattleServer)Server;
            Console.WriteLine($"Message received: \"{packet.GetType().Name}\" ({Id})");


            if (packet is RequestTeamPacket rtPack)
            {
                Team = rtPack.Team;
                ser.TeamUpdated(this);
            }
        }
    }
}
