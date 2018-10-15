using Ether.Network.Common;
using Ether.Network.Packets;
using System;

namespace Kermalis.PokemonBattleEngineServer
{
    class Player : NetUser
    {


        public override void HandleMessage(INetPacketStream packet)
        {
            Console.WriteLine("Received '{1}' from {0}", Id, packet);
        }
    }
}
