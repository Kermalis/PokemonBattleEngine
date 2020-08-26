using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests
{
    [Collection("Utils")]
    public class BehaviorTests
    {
        public BehaviorTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Fact]
        public void Wild_Pkmn_Positions_Set_Before_Begin()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Darkrai, 0, 100, PBEMove.Splash)
            {
                CaughtBall = PBEItem.None
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBEWildInfo(p1));

            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon darkrai = t1.Party[0];
            #endregion

            #region Check
            Assert.True(darkrai.FieldPosition == PBEFieldPosition.Center
                && battle.ActiveBattlers.Single() == darkrai);
            #endregion
        }
    }
}
