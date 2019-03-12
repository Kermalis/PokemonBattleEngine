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
            TestType t = TestType.MovesetDumper;

            switch (t)
            {
                case TestType.AIBattle: AIBattle.Test(); break;
                case TestType.FontDumper: FontDumper.Dump(); break;
                case TestType.LocalizationDumper: LocalizationDumper.Dump(); break;
                case TestType.MovesetDumper: MovesetDumper.Dump(); break;
            }
        }
    }
}
