namespace Kermalis.PokemonBattleEngineTesting
{
    class TestProgram
    {
        private enum TestType
        {
            AIBattle
        }

        static void Main(string[] args)
        {
            TestType t = TestType.AIBattle;

            switch (t)
            {
                case TestType.AIBattle: AIBattle.Test(); break;
            }
        }
    }
}
