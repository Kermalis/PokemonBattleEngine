using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Moves
{
    [Collection("Utils")]
    public class HelpingHandTests
    {
        public HelpingHandTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        // https://github.com/Kermalis/PokemonBattleEngine/issues/308
        [Theory]
        [InlineData(PBEMove.Bounce, PBEStatus2.Airborne)]
        [InlineData(PBEMove.Dig, PBEStatus2.Underground)]
        [InlineData(PBEMove.Dive, PBEStatus2.Underwater)]
        [InlineData(PBEMove.Fly, PBEStatus2.Airborne)]
        [InlineData(PBEMove.ShadowForce, PBEStatus2.ShadowForce)]
        //[InlineData(PBEMove.SkyDrop, PBEStatus2.Airborne)]
        public void HelpingHand_HitsSemiInvulnerable(PBEMove move, PBEStatus2 status2)
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(2);
            p0[0] = new TestPokemon(settings, PBESpecies.Minun, 0, 100, PBEMove.HelpingHand, PBEMove.Splash);
            p0[1] = new TestPokemon(settings, PBESpecies.Giratina, 0, 1, move);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Double, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon minun = t0.Party[0];
            PBEBattlePokemon giratina = t0.Party[1];
            PBEBattlePokemon magikarp = t1.Party[0];
            #endregion

            #region Use Shadow Force
            Assert.Null(t0.SelectActionsIfValid(
                new PBETurnAction(minun, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(giratina, move, PBETurnTarget.FoeLeft)));
            Assert.Null(t1.SelectActionsIfValid(
                new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyLeft)));

            battle.RunTurn();

            Assert.True(giratina.Status2.HasFlag(status2));
            #endregion

            #region Use Helping Hand and check
            Assert.Null(t0.SelectActionsIfValid(
                new PBETurnAction(minun, PBEMove.HelpingHand, PBETurnTarget.AllyRight),
                new PBETurnAction(giratina, move, PBETurnTarget.FoeLeft)));
            Assert.Null(t1.SelectActionsIfValid(
                new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyLeft)));

            battle.RunTurn();

            Assert.True(battle.VerifyStatus2Happened(giratina, minun, PBEStatus2.HelpingHand, PBEStatusAction.Added));
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void HelpingHand_Fails_If_Self()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Minun, 0, 100, PBEMove.HelpingHand);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon minun = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];
            #endregion

            #region Use Helping Hand and check
            Assert.Null(t0.SelectActionsIfValid(new PBETurnAction(minun, PBEMove.HelpingHand, PBETurnTarget.AllyCenter)));
            Assert.Null(t1.SelectActionsIfValid(new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(battle.VerifyMoveResultHappened(minun, minun, PBEResult.NoTarget) // Fail
                && !minun.Status2.HasFlag(PBEStatus2.HelpingHand)); // No status
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
