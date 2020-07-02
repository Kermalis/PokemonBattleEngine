using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
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
            PBERandom.SetSeed(1); // Seed ensures SecretPower freezes
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Happiny, 0, 100, PBEMove.SecretPower, PBEMove.Splash)
            {
                Ability = PBEAbility.SereneGrace
            };

            var p1 = new TestPokemonCollection(2);
            p1[0] = new TestPokemon(settings, PBESpecies.Shaymin, PBEForm.Shaymin_Sky, 100, PBEMove.Splash);
            p1[1] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"), battleTerrain: PBEBattleTerrain.Snow);
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon happiny = t0.Party[0];
            PBEBattlePokemon shaymin = t1.Party[0];
            PBEBattlePokemon magikarp = t1.Party[1];
            #endregion

            #region Freeze Shaymin
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(happiny, PBEMove.SecretPower, PBETurnTarget.FoeCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(shaymin, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(shaymin.Status1 == PBEStatus1.Frozen && shaymin.Form == PBEForm.Shaymin);
            #endregion

            #region Swap Shaymin for Magikarp and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(happiny, PBEMove.Splash, PBETurnTarget.AllyCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(shaymin, magikarp)));

            battle.RunTurn();

            Assert.True(shaymin.Form == PBEForm.Shaymin);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
