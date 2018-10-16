using Ether.Network.Common;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Network;
using System;

namespace Kermalis.PokemonBattleEngineServer
{
    class Player : NetUser
    {
        public PTeamShell Team;

        public override void HandleMessage(INetPacketStream packet)
        {
            var ser = (BattleServer)Server;
            Console.WriteLine($"Message received: \"{packet.GetType().Name}\" ({Id})");

            switch (packet)
            {
                case PRequestTeamPacket rtPack:
                    ser.TeamUpdated(this, rtPack.Team);
                    break;
            }
        }
    }
}
