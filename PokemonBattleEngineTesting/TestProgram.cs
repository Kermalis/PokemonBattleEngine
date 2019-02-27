namespace Kermalis.PokemonBattleEngineTesting
{
    class TestProgram
    {
        private enum TestType
        {
            AIBattle,
            AbilityGenerator,
            ItemGenerator,
            MoveGenerator,
            PokemonGenerator
        }

        static void Main(string[] args)
        {
            TestType t = TestType.AIBattle;

            switch (t)
            {
                case TestType.AIBattle: AIBattle.Test(); break;
                case TestType.AbilityGenerator: new LocalizationGenerator().GenerateAbilities(); break;
                case TestType.ItemGenerator: new LocalizationGenerator().GenerateItems(); break;
                case TestType.MoveGenerator: new LocalizationGenerator().GenerateMoves(); break;
                case TestType.PokemonGenerator: new LocalizationGenerator().GeneratePokemon(); break;
            }
        }
    }
}
