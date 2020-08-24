using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Abilities
{
    [Collection("Utils")]
    public class PoisonHealTests
    {
        public PoisonHealTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Fact]
        public void PoisonHeal_BadlyPoisoned_Counter_Works()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0; // Seed prevents Toxic from missing
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Seviper, 0, 100, PBEMove.Toxic);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Gliscor, 0, 100, PBEMove.Splash)
            {
                Ability = PBEAbility.PoisonHeal
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon seviper = t0.Party[0];
            PBEBattlePokemon gliscor = t1.Party[0];
            #endregion

            #region Badly Poison Gliscor and check
            Assert.Null(t0.SelectActionsIfValid(new PBETurnAction(seviper, PBEMove.Toxic, PBETurnTarget.FoeCenter)));
            Assert.Null(t1.SelectActionsIfValid(new PBETurnAction(gliscor, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(gliscor.Status1 == PBEStatus1.BadlyPoisoned // Was afflicted
                && !battle.VerifyAbilityHappened(gliscor, gliscor, PBEAbility.PoisonHeal, PBEAbilityAction.RestoredHP) // Did not activate
                && !battle.VerifyStatus1Happened(gliscor, gliscor, PBEStatus1.BadlyPoisoned, PBEStatusAction.Damage) // Did not activate
                && gliscor.Status1Counter == 2 // Counter still increments
                && gliscor.HPPercentage == 1); // Did not take damage
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
