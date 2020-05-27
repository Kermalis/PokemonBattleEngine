using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Moves
{
    [Collection("Utils")]
    public class ProtectionTests
    {
        public ProtectionTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Theory]
        [InlineData(PBEMove.Detect)]
        [InlineData(PBEMove.Protect)]
        //[InlineData(PBEMove.QuickGuard)]
        [InlineData(PBEMove.WideGuard)]
        public void Protection_Counter_Does_Not_Reset(PBEMove move)
        {
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var team1Shell = new PBETeamShell(settings, 1, true);
            PBEPokemonShell ps = team1Shell[0];
            ps.Species = PBESpecies.Mienshao;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = move;

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
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, move, PBETurnTarget.AllyCenter) }));

            t = battle.Teams[1];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Splash, PBETurnTarget.AllyCenter) })); ;

            battle.RunTurn();

            PBEPokemon pkmn = battle.Teams[0].Party[0];
            Assert.True(pkmn.Protection_Counter == 1);

            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            battle.Dispose();
        }

        // https://github.com/Kermalis/PokemonBattleEngine/issues/261
        [Theory]
        [InlineData(PBEMove.Detect)]
        [InlineData(PBEMove.Protect)]
        //[InlineData(PBEMove.QuickGuard)]
        [InlineData(PBEMove.WideGuard)]
        public void Protection_Counter_Does_Reset(PBEMove move)
        {
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var team1Shell = new PBETeamShell(settings, 1, true);
            PBEPokemonShell ps = team1Shell[0];
            ps.Species = PBESpecies.Mienshao;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = move;
            ps.Moveset[1].Move = PBEMove.CalmMind;

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
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, move, PBETurnTarget.AllyCenter) }));

            t = battle.Teams[1];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Splash, PBETurnTarget.AllyCenter) })); ;

            battle.RunTurn();

            PBEPokemon pkmn = battle.Teams[0].Party[0];
            Assert.True(pkmn.Protection_Counter == 1);

            t = battle.Teams[0];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.CalmMind, PBETurnTarget.AllyCenter) }));

            t = battle.Teams[1];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Splash, PBETurnTarget.AllyCenter) })); ;

            battle.RunTurn();

            Assert.True(pkmn.Protection_Counter == 0);

            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            battle.Dispose();
        }
    }
}
