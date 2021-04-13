using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Forms
{
    [Collection("Utils")]
    public class ShayminTests
    {
        public ShayminTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Fact]
        public void Shaymin_Reverts_To_Normal_Form_Forever()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 40703; // Seed ensures SecretPower freezes
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Happiny, 0, 100, PBEMove.SecretPower, PBEMove.Splash)
            {
                Ability = PBEAbility.SereneGrace
            };

            var p1 = new TestPokemonCollection(2);
            p1[0] = new TestPokemon(settings, PBESpecies.Shaymin, PBEForm.Shaymin_Sky, 100, PBEMove.Splash);
            p1[1] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false),
                battleTerrain: PBEBattleTerrain.Snow);
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon happiny = t0.Party[0];
            PBEBattlePokemon shaymin = t1.Party[0];
            PBEBattlePokemon magikarp = t1.Party[1];

            battle.Begin();
            #endregion

            #region Freeze Shaymin
            Assert.Null(t0.SelectActionsIfValid(new PBETurnAction(happiny, PBEMove.SecretPower, PBETurnTarget.FoeCenter)));
            Assert.Null(t1.SelectActionsIfValid(new PBETurnAction(shaymin, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(shaymin.Status1 == PBEStatus1.Frozen && shaymin.Form == PBEForm.Shaymin);
            #endregion

            #region Swap Shaymin for Magikarp and check
            Assert.Null(t0.SelectActionsIfValid(new PBETurnAction(happiny, PBEMove.Splash, PBETurnTarget.AllyCenter)));
            Assert.Null(t1.SelectActionsIfValid(new PBETurnAction(shaymin, magikarp)));

            battle.RunTurn();

            Assert.True(shaymin.Form == PBEForm.Shaymin);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
