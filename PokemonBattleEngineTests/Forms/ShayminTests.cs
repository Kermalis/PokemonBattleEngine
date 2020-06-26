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
            p0[0] = new TestPokemon(PBESpecies.Happiny, 0, 100)
            {
                Ability = PBEAbility.SereneGrace,
                Moveset = new TestMoveset(settings, new[] { PBEMove.SecretPower, PBEMove.Splash })
            };

            var p1 = new TestPokemonCollection(2);
            p1[0] = new TestPokemon(PBESpecies.Shaymin, PBEForm.Shaymin_Sky, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };
            p1[1] = new TestPokemon(PBESpecies.Magikarp, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Team 1"), new PBETrainerInfo(p1, "Team 2"), battleTerrain: PBEBattleTerrain.Snow);
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon happiny = t0.Party[0];
            PBEBattlePokemon shaymin = t1.Party[0];
            PBEBattlePokemon magikarp = t1.Party[1];
            #endregion

            #region Freeze Shaymin
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(happiny.Id, PBEMove.SecretPower, PBETurnTarget.FoeCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(shaymin.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));

            battle.RunTurn();

            Assert.True(shaymin.Status1 == PBEStatus1.Frozen && shaymin.Form == PBEForm.Shaymin);
            #endregion

            #region Swap Shaymin for Magikarp and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(happiny.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(shaymin.Id, magikarp.Id) }));

            battle.RunTurn();

            Assert.True(shaymin.Form == PBEForm.Shaymin);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
