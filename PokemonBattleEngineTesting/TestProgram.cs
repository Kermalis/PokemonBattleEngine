namespace Kermalis.PokemonBattleEngineTesting
{
    class TestProgram
    {
        private enum TestType
        {
            AIBattle,
            AbilityGenerator,
            MoveGenerator
        }

        static void Main(string[] args)
        {
            TestType t = TestType.AbilityGenerator;

            switch (t)
            {
                case TestType.AIBattle: AIBattle.Test(); break;
                case TestType.AbilityGenerator: new LocalizationGenerator().GenerateAbilities(); break;
                case TestType.MoveGenerator: new LocalizationGenerator().GenerateMoves(); break;
            }
        }
    }
}
