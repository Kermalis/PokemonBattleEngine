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
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(2);
            p0[0] = new TestPokemon(PBESpecies.Groudon, 0, 100)
            {
                Ability = PBEAbility.Drought,
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };
            p0[1] = new TestPokemon(PBESpecies.Rayquaza, 0, 100)
            {
                Ability = PBEAbility.AirLock,
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(species, 0, 100)
            {
                Ability = ability,
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };

            var battle = new PBEBattle(PBEBattleTerrain.Plain, PBEBattleFormat.Single, new PBETeamInfo(p0, "Team 1"), new PBETeamInfo(p1, "Team 2"), settings);
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETeam t0 = battle.Teams[0];
            PBETeam t1 = battle.Teams[1];
            PBEBattlePokemon groudon = t0.Party[0];
            PBEBattlePokemon rayquaza = t0.Party[1];
            PBEBattlePokemon castformCherrim = t1.Party[0];
            #endregion

            #region Check Castform/Cherrim for correct form
            Assert.True(battle.Weather == PBEWeather.HarshSunlight && castformCherrim.Form == form);
            #endregion

            #region Swap Groudon for Rayquaza and check for no form
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(groudon.Id, rayquaza.Id) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(castformCherrim.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));

            battle.RunTurn();

            Assert.True(battle.Weather == PBEWeather.HarshSunlight && castformCherrim.Form == 0);
            #endregion

            #region Swap Rayquaza for Groudon and check for correct form
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(rayquaza.Id, groudon.Id) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(castformCherrim.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));

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
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(PBESpecies.Shuckle, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.GastroAcid, PBEMove.Splash })
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(species, 0, 100)
            {
                Ability = ability,
                Moveset = new TestMoveset(settings, new[] { PBEMove.SunnyDay, PBEMove.Splash })
            };

            var battle = new PBEBattle(PBEBattleTerrain.Plain, PBEBattleFormat.Single, new PBETeamInfo(p0, "Team 1"), new PBETeamInfo(p1, "Team 2"), settings);
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETeam t0 = battle.Teams[0];
            PBETeam t1 = battle.Teams[1];
            PBEBattlePokemon shuckle = t0.Party[0];
            PBEBattlePokemon castformCherrim = t1.Party[0];
            #endregion

            #region Use Sunny Day and check for correct form
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(shuckle.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(castformCherrim.Id, PBEMove.SunnyDay, PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter) }));

            battle.RunTurn();

            Assert.True(battle.Weather == PBEWeather.HarshSunlight && castformCherrim.Form == form);
            #endregion

            #region Use Gastro Acid and check for no form
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(shuckle.Id, PBEMove.GastroAcid, PBETurnTarget.FoeCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(castformCherrim.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));

            battle.RunTurn();

            Assert.True(battle.Weather == PBEWeather.HarshSunlight && castformCherrim.Form == 0);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
