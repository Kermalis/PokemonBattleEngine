namespace Kermalis.PokemonBattleEngineServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var server = new BattleServer("127.0.0.1"))
                server.Start();
        }
    }
}