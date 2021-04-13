using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Moves
{
    [Collection("Utils")]
    public class MultiStrikeTests
    {
        public MultiStrikeTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Theory]
        [InlineData(PBEAbility.Technician, 2)]
        [InlineData(PBEAbility.SkillLink, 5)]
        public void SkillLink_Works__2To5(PBEAbility ability, byte numHits)
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 1230; // Seed ensures hits would normally not be 5
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Cinccino, 0, 1, PBEMove.TailSlap)
            {
                Ability = ability
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon cinccino = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];

            battle.Begin();
            #endregion

            #region Use and check
            Assert.Null(t0.SelectActionsIfValid(new PBETurnAction(cinccino, PBEMove.TailSlap, PBETurnTarget.FoeCenter)));
            Assert.Null(t1.SelectActionsIfValid(new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(battle.VerifySpecialMessageHappened(PBESpecialMessage.MultiHit, numHits)); // Correct number of hits
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
