using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Abilities
{
    [Collection("Utils")]
    public class AntiStatusAbilityTests
    {
        public AntiStatusAbilityTests(TestUtils _, ITestOutputHelper output)
        {
            TestUtils.SetOutputHelper(output);
        }

        [Fact]
        public void Immunity_Works()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0; // Seed prevents Toxic from missing
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Seviper, 0, 100, PBEMove.Toxic);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Zangoose, 0, 100, PBEMove.Splash)
            {
                Ability = PBEAbility.Immunity
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon seviper = t0.Party[0];
            PBEBattlePokemon zangoose = t1.Party[0];

            battle.Begin();
            #endregion

            #region Badly Poison Zangoose and check
            Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(seviper, PBEMove.Toxic, PBETurnTarget.FoeCenter)));
            Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(zangoose, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(battle.VerifyMoveResultHappened(seviper, zangoose, PBEResult.Ineffective_Ability));
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
