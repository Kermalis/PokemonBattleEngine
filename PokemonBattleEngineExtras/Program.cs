using Kermalis.PokemonBattleEngine;
using Microsoft.Data.Sqlite;

namespace Kermalis.PokemonBattleEngineExtras
{
    internal class Program
    {
        private enum Extra
        {
            AIBattle,
            FontDumper,
            LocalizationDumper,
            NARCTextDumper,
            PokemonDataDumper
        }

        public static void Main()
        {
            SqliteConnection GetConnection()
            {
                SQLitePCL.Batteries_V2.Init();
                const string databasePath = @"../../../../\PokemonBattleEngine\PokemonBattleEngine.db";
                var con = new SqliteConnection($"Filename={databasePath};");
                con.Open();
                return con;
            }

            Extra e = Extra.AIBattle;
            switch (e)
            {
                case Extra.AIBattle:
                {
                    PBEUtils.CreateDatabaseConnection(string.Empty);
                    AIBattle.Test();
                    break;
                }
                case Extra.FontDumper: FontDumper.Dump(); break;
                case Extra.LocalizationDumper:
                {
                    using (SqliteConnection con = GetConnection())
                    {
                        LocalizationDumper.Dump(con);
                        con.Close();
                    }
                    break;
                }
                case Extra.NARCTextDumper: NARCTextDumper.Dump(); break;
                case Extra.PokemonDataDumper:
                {
                    using (SqliteConnection con = GetConnection())
                    {
                        PokemonDataDumper.Dump(con);
                        con.Close();
                    }
                    break;
                }
            }
        }
    }
}
