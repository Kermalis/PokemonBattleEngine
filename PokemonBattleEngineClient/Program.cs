using System;
using System.Threading;

namespace Kermalis.PokemonBattleEngineClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var client = new BattleClient("127.0.0.1");
            client.Connect();

            if (!client.IsConnected)
            {
                Console.WriteLine("Can't connect to server!");
                return;
            }

            Thread.Sleep(-1);
        }
    }
}
