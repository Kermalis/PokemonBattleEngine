using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Forms
{
    [Collection("Utils")]
    public class CastformCherrimTests
    {
        public CastformCherrimTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Theory]
        [InlineData(PBESpecies.Castform, PBEAbility.Forecast, PBEForm.Castform_Sunny)]
        [InlineData(PBESpecies.Cherrim, PBEAbility.FlowerGift, PBEForm.Cherrim_Sunshine)]
        public void CastformCherrim_Interacts_With_AirLock(PBESpecies species, PBEAbility ability, PBEForm form)
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(2);
            p0[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);
            p0[1] = new TestPokemon(settings, PBESpecies.Rayquaza, 0, 100, PBEMove.Splash)
            {
                Ability = PBEAbility.AirLock
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, species, 0, 100, PBEMove.Splash)
            {
                Ability = ability
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"),
                weather: PBEWeather.HarshSunlight);
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon magikarp = t0.Party[0];
            PBEBattlePokemon rayquaza = t0.Party[1];
            PBEBattlePokemon castformCherrim = t1.Party[0];
            #endregion

            #region Check Castform/Cherrim for correct form
            Assert.True(battle.Weather == PBEWeather.HarshSunlight && castformCherrim.Form == form);
            #endregion

            #region Swap Magikarp for Rayquaza and check for no form
            Assert.Null(t0.SelectActionsIfValid(new PBETurnAction(magikarp, rayquaza)));
            Assert.Null(t1.SelectActionsIfValid(new PBETurnAction(castformCherrim, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(battle.Weather == PBEWeather.HarshSunlight && castformCherrim.Form == 0);
            #endregion

            #region Swap Rayquaza for Magikarp and check for correct form
            Assert.Null(t0.SelectActionsIfValid(new PBETurnAction(rayquaza, magikarp)));
            Assert.Null(t1.SelectActionsIfValid(new PBETurnAction(castformCherrim, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(battle.Weather == PBEWeather.HarshSunlight && castformCherrim.Form == form);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Theory]
        [InlineData(PBESpecies.Castform, PBEAbility.Forecast, PBEForm.Castform_Sunny)]
        [InlineData(PBESpecies.Cherrim, PBEAbility.FlowerGift, PBEForm.Cherrim_Sunshine)]
        public void CastformCherrim_Loses_Form(PBESpecies species, PBEAbility ability, PBEForm form)
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Shuckle, 0, 100, PBEMove.GastroAcid, PBEMove.Splash);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, species, 0, 100, PBEMove.Splash, PBEMove.SunnyDay)
            {
                Ability = ability
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon shuckle = t0.Party[0];
            PBEBattlePokemon castformCherrim = t1.Party[0];
            #endregion

            #region Use Sunny Day and check for correct form
            Assert.Null(t0.SelectActionsIfValid(new[] { new PBETurnAction(shuckle, PBEMove.Splash, PBETurnTarget.AllyCenter) }));
            Assert.Null(t1.SelectActionsIfValid(new[] { new PBETurnAction(castformCherrim, PBEMove.SunnyDay, PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter) }));

            battle.RunTurn();

            Assert.True(battle.Weather == PBEWeather.HarshSunlight && castformCherrim.Form == form);
            #endregion

            #region Use Gastro Acid and check for no form
            Assert.Null(t0.SelectActionsIfValid(new[] { new PBETurnAction(shuckle, PBEMove.GastroAcid, PBETurnTarget.FoeCenter) }));
            Assert.Null(t1.SelectActionsIfValid(new[] { new PBETurnAction(castformCherrim, PBEMove.Splash, PBETurnTarget.AllyCenter) }));

            battle.RunTurn();

            Assert.True(battle.Weather == PBEWeather.HarshSunlight && castformCherrim.Form == 0);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
