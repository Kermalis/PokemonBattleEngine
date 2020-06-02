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

            var team1Shell = new PBETeamShell(settings, 2, true);
            PBEPokemonShell ps = team1Shell[0];
            ps.Species = PBESpecies.Groudon;
            ps.Ability = PBEAbility.Drought;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.Snore;
            ps = team1Shell[1];
            ps.Species = PBESpecies.Rayquaza;
            ps.Moveset[0].Move = PBEMove.Snore;

            var team2Shell = new PBETeamShell(settings, 1, true);
            ps = team2Shell[0];
            ps.Species = species;
            ps.Ability = ability;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.Snore;

            var battle = new PBEBattle(PBEBattleTerrain.Plain, PBEBattleFormat.Single, team1Shell, "Team 1", team2Shell, "Team 2");
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            team1Shell.Dispose();
            team2Shell.Dispose();
            battle.Begin();

            PBETeam t0 = battle.Teams[0];
            PBETeam t1 = battle.Teams[1];
            PBEPokemon groudon = t0.Party[0];
            PBEPokemon rayquaza = t0.Party[1];
            PBEPokemon castformCherrim = t1.Party[0];
            #endregion

            #region Check Castform/Cherrim for correct form
            Assert.True(battle.Weather == PBEWeather.HarshSunlight && castformCherrim.Form == form);
            #endregion

            #region Swap Groudon for Rayquaza and check for no form
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(groudon.Id, rayquaza.Id) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(castformCherrim.Id, PBEMove.Snore, PBETurnTarget.FoeCenter) }));

            battle.RunTurn();

            Assert.True(battle.Weather == PBEWeather.HarshSunlight && castformCherrim.Form == 0);
            #endregion

            #region Swap Rayquaza for Groudon and check for correct form
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(rayquaza.Id, groudon.Id) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(castformCherrim.Id, PBEMove.Snore, PBETurnTarget.FoeCenter) }));

            battle.RunTurn();

            Assert.True(battle.Weather == PBEWeather.HarshSunlight && castformCherrim.Form == form);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            battle.Dispose();
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

            var team1Shell = new PBETeamShell(settings, 1, true);
            PBEPokemonShell ps = team1Shell[0];
            ps.Species = PBESpecies.Shuckle;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.Snore;
            ps.Moveset[1].Move = PBEMove.GastroAcid;

            var team2Shell = new PBETeamShell(settings, 1, true);
            ps = team2Shell[0];
            ps.Species = species;
            ps.Ability = ability;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.Snore;
            ps.Moveset[1].Move = PBEMove.SunnyDay;

            var battle = new PBEBattle(PBEBattleTerrain.Plain, PBEBattleFormat.Single, team1Shell, "Team 1", team2Shell, "Team 2");
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            team1Shell.Dispose();
            team2Shell.Dispose();
            battle.Begin();

            PBETeam t0 = battle.Teams[0];
            PBETeam t1 = battle.Teams[1];
            PBEPokemon shuckle = t0.Party[0];
            PBEPokemon castformCherrim = t1.Party[0];
            #endregion

            #region Use Sunny Day and check for correct form
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(shuckle.Id, PBEMove.Snore, PBETurnTarget.FoeCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(castformCherrim.Id, PBEMove.SunnyDay, PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter) }));

            battle.RunTurn();

            Assert.True(battle.Weather == PBEWeather.HarshSunlight && castformCherrim.Form == form);
            #endregion

            #region Use Gastro Acid and check for no form
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(shuckle.Id, PBEMove.GastroAcid, PBETurnTarget.FoeCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(castformCherrim.Id, PBEMove.Snore, PBETurnTarget.FoeCenter) }));

            battle.RunTurn();

            Assert.True(battle.Weather == PBEWeather.HarshSunlight && castformCherrim.Form == 0);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            battle.Dispose();
            #endregion
        }
    }
}
