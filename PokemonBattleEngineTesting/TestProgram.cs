using Microsoft.Data.Sqlite;

namespace Kermalis.PokemonBattleEngineTesting
{
    class TestProgram
    {
        private enum TestType
        {
            AIBattle,
            FontDumper,
            LocalizationDumper,
            PokemonDataDumper
        }

        static void Main(string[] args)
        {
            SqliteConnection GetConnection()
            {
                SQLitePCL.Batteries_V2.Init();
                const string databasePath = @"../../../../\PokemonBattleEngine\PokemonBattleEngine.db";
                var con = new SqliteConnection($"Filename={databasePath};");
                con.Open();
                return con;
            }

            TestType t = TestType.LocalizationDumper;
            switch (t)
            {
                case TestType.AIBattle: AIBattle.Test(); break;
                case TestType.FontDumper: FontDumper.Dump(); break;
                case TestType.LocalizationDumper:
                    {
                        using (SqliteConnection con = GetConnection())
                        {
                            LocalizationDumper.Dump(con);
                            con.Close();
                        }
                        break;
                    }
                case TestType.PokemonDataDumper:
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
