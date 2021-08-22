using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Moves
{
    [Collection("Utils")]
    public class TeleportTests
    {
        public TeleportTests(TestUtils _, ITestOutputHelper output)
        {
            TestUtils.SetOutputHelper(output);
        }

        [Fact]
        public void Teleport_Works()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Abra, 0, 100, PBEMove.Teleport);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Splash)
            {
                CaughtBall = PBEItem.None
            };

            var battle = PBEBattle.CreateWildBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBEWildInfo(p1));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon abra = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];

            battle.Begin();
            #endregion

            #region Use Teleport and check
            Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(abra, PBEMove.Teleport, PBETurnTarget.AllyCenter)));
            Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(!battle.VerifyMoveResultHappened(abra, abra, PBEResult.InvalidConditions) // No fail
                && battle.BattleResult == PBEBattleResult.WildEscape); // Escaped
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
