using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Abilities
{
    [Collection("Utils")]
    public class AntiStatusAbilityTests
    {
        public AntiStatusAbilityTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Fact]
        public void Immunity_Works()
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0; // Seed prevents Toxic from missing
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Seviper, 0, 100, PBEMove.Toxic);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Zangoose, 0, 100, PBEMove.Splash)
            {
                Ability = PBEAbility.Immunity
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon seviper = t0.Party[0];
            PBEBattlePokemon zangoose = t1.Party[0];
            #endregion

            #region Badly Poison Zangoose and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(seviper, PBEMove.Toxic, PBETurnTarget.FoeCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(zangoose, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(battle.VerifyMoveResultHappened(seviper, zangoose, PBEResult.Ineffective_Ability));
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
