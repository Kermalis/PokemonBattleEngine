namespace Kermalis.PokemonBattleEngineTesting
{
    class TestProgram
    {
        private enum TestType
        {
            AIBattle,
            LocalizationGenerator
        }

        static void Main(string[] args)
        {
            TestType t = TestType.AIBattle;

            switch (t)
            {
                case TestType.AIBattle: AIBattle.Test(); break;
                case TestType.LocalizationGenerator:
                    {
                        var generator = new LocalizationGenerator();
                        generator.GenerateAbilities();
                        generator.GenerateItems();
                        generator.GenerateMoves();
                        generator.GeneratePokemon();
                        break;
                    }
            }
        }
    }
}
