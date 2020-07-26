using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Moves
{
    [Collection("Utils")]
    public class SecretPowerTests
    {
        public SecretPowerTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SecretPower_SereneGrace__Bug(bool bugFix)
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 473; // Seed ensures SecretPower does not freeze without the bugfix
            var settings = new PBESettings { BugFix = bugFix };
            settings.MakeReadOnly();

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Makuhita, 0, 1, PBEMove.SecretPower)
            {
                Ability = PBEAbility.SereneGrace
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"),
                battleTerrain: PBEBattleTerrain.Snow);
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon makuhita = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];
            #endregion

            #region Use and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(makuhita, PBEMove.SecretPower, PBETurnTarget.FoeCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            if (settings.BugFix)
            {
                Assert.True(battle.VerifyStatus1Happened(magikarp, makuhita, PBEStatus1.Frozen, PBEStatusAction.Added)
                    && magikarp.Status1 == PBEStatus1.Frozen); // Frozen because of Serene Grace
            }
            else
            {
                Assert.True(!battle.VerifyStatus1Happened(magikarp, makuhita, PBEStatus1.Frozen, PBEStatusAction.Added)
                    && magikarp.Status1 == PBEStatus1.None); // Buggy
            }
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
