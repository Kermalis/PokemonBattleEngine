namespace Kermalis.PokemonBattleEngineTesting
{
    class TestProgram
    {
        private enum TestType
        {
            AIBattle,
            FontDumper,
            LocalizationDumper,
            MovesetDumper
        }

        static void Main(string[] args)
        {
            TestType t = TestType.LocalizationDumper;

            switch (t)
            {
                case TestType.AIBattle: AIBattle.Test(); break;
                case TestType.FontDumper: FontDumper.Dump(); break;
                case TestType.LocalizationDumper: LocalizationDumper.Dump(); break;
            }
        }
    }
}
