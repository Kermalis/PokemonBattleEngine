namespace Kermalis.PokemonBattleEngineTesting
{
    class TestProgram
    {
        private enum TestType
        {
            AIBattle,
            AbilityGenerator,
            ItemGenerator,
            MoveGenerator
        }

        static void Main(string[] args)
        {
            TestType t = TestType.MoveGenerator;

            switch (t)
            {
                case TestType.AIBattle: AIBattle.Test(); break;
                case TestType.AbilityGenerator: new LocalizationGenerator().GenerateAbilities(); break;
                case TestType.ItemGenerator: new LocalizationGenerator().GenerateItems(); break;
                case TestType.MoveGenerator: new LocalizationGenerator().GenerateMoves(); break;
            }
        }
    }
}
