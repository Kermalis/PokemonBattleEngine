using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Moves
{
    [Collection("Utils")]
    public class CamouflageTests
    {
        public CamouflageTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Fact]
        public void Camouflage_Works__SingleType()
        {
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var team1Shell = new PBETeamShell(settings, 1, true);
            PBEPokemonShell ps = team1Shell[0];
            ps.Species = PBESpecies.Staryu;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.Camouflage;

            var team2Shell = new PBETeamShell(settings, 1, true);
            ps = team2Shell[0];
            ps.Species = PBESpecies.Magikarp;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.Splash;

            var battle = new PBEBattle(PBEBattleTerrain.Plain, PBEBattleFormat.Single, team1Shell, "Team 1", team2Shell, "Team 2");
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            team1Shell.Dispose();
            team2Shell.Dispose();
            battle.Begin();

            PBETeam t = battle.Teams[0];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Camouflage, PBETurnTarget.AllyCenter) }));

            t = battle.Teams[1];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Splash, PBETurnTarget.AllyCenter) })); ;

            battle.RunTurn();

            PBEPokemon pkmn = battle.Teams[0].Party[0];
            Assert.True(pkmn.Type1 == PBEType.Normal && pkmn.Type2 == PBEType.None);

            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            battle.Dispose();
        }

        [Fact]
        public void Camouflage_Fails()
        {
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var team1Shell = new PBETeamShell(settings, 1, true);
            PBEPokemonShell ps = team1Shell[0];
            ps.Species = PBESpecies.Staryu;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.Camouflage;

            var team2Shell = new PBETeamShell(settings, 1, true);
            ps = team2Shell[0];
            ps.Species = PBESpecies.Magikarp;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.Splash;

            var battle = new PBEBattle(PBEBattleTerrain.Water, PBEBattleFormat.Single, team1Shell, "Team 1", team2Shell, "Team 2");
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            team1Shell.Dispose();
            team2Shell.Dispose();
            battle.Begin();

            PBETeam t = battle.Teams[0];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Camouflage, PBETurnTarget.AllyCenter) }));

            t = battle.Teams[1];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Splash, PBETurnTarget.AllyCenter) })); ;

            battle.RunTurn();

            PBEPokemon pkmn = battle.Teams[0].Party[0];
            Assert.True(TestUtils.VerifyMoveResult(battle, pkmn, pkmn, PBEResult.InvalidConditions));

            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            battle.Dispose();
        }

        [Fact]
        public void Camouflage_Works__DualType()
        {
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var team1Shell = new PBETeamShell(settings, 1, true);
            PBEPokemonShell ps = team1Shell[0];
            ps.Species = PBESpecies.Starmie;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.Camouflage;

            var team2Shell = new PBETeamShell(settings, 1, true);
            ps = team2Shell[0];
            ps.Species = PBESpecies.Magikarp;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.Splash;

            var battle = new PBEBattle(PBEBattleTerrain.Water, PBEBattleFormat.Single, team1Shell, "Team 1", team2Shell, "Team 2");
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            team1Shell.Dispose();
            team2Shell.Dispose();
            battle.Begin();

            PBETeam t = battle.Teams[0];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Camouflage, PBETurnTarget.AllyCenter) }));

            t = battle.Teams[1];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Splash, PBETurnTarget.AllyCenter) })); ;

            battle.RunTurn();

            PBEPokemon pkmn = battle.Teams[0].Party[0];
            Assert.True(pkmn.Type1 == PBEType.Water && pkmn.Type2 == PBEType.None);

            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            battle.Dispose();
        }
    }
}
