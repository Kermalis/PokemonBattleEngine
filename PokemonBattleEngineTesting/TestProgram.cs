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
            const string databasePath = @"../../../../\PokemonBattleEngine\PokemonBattleEngine.db";
            TestType t = TestType.PokemonDataDumper;

            switch (t)
            {
                case TestType.AIBattle: AIBattle.Test(); break;
                case TestType.FontDumper: FontDumper.Dump(); break;
                case TestType.LocalizationDumper: LocalizationDumper.Dump(); break;
                case TestType.PokemonDataDumper: PokemonDataDumper.Dump(databasePath); break;
            }
        }
    }
}
