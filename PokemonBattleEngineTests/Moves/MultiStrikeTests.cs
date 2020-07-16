using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
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
            PBEUtils.GlobalRandom.Seed = 1230; // Seed ensures hits would normally not be 5
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Cinccino, 0, 1, PBEMove.TailSlap)
            {
                Ability = ability
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon cinccino = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];
            #endregion

            #region Use and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(cinccino, PBEMove.TailSlap, PBETurnTarget.FoeCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(battle.VerifySpecialMessageHappened(PBESpecialMessage.MultiHit, numHits)); // Correct number of hits
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
