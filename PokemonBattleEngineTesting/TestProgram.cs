namespace Kermalis.PokemonBattleEngineTesting
{
    class TestProgram
    {
        private enum TestType
        {
            AIBattle,
            MoveGenerator
        }

        static void Main(string[] args)
        {
            TestType t = TestType.MoveGenerator;

            switch (t)
            {
                case TestType.AIBattle: AIBattle.Test(); break;
                case TestType.MoveGenerator: new LocalizationGenerator().GenerateMoves(); break;
            }
        }
    }
}
