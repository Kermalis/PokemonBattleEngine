using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests
{
    [Collection("Utils")]
    public class WinnerTests
    {
        public WinnerTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Theory]
        [InlineData(PBEMove.Selfdestruct)]
        [InlineData(PBEMove.Explosion)]
        public void Explosion_User_Loses(PBEMove move)
        {
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var team1Shell = new PBETeamShell(settings, 1, true);
            PBEPokemonShell p = team1Shell[0];
            p.Species = PBESpecies.Golem;
            p.Item = PBEItem.None;
            p.Moveset[0].Move = move;

            var team2Shell = new PBETeamShell(settings, 1, true);
            p = team2Shell[0];
            p.Species = PBESpecies.Happiny;
            p.Level = 1;
            p.Item = PBEItem.None;
            p.Moveset[0].Move = PBEMove.Snore;

            var battle = new PBEBattle(PBEBattleFormat.Single, team1Shell, "Team 1", team2Shell, "Team 2");
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            team1Shell.Dispose();
            team2Shell.Dispose();
            battle.Begin();

            PBETeam t = battle.Teams[0];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, move, PBETurnTarget.FoeCenter) }));

            t = battle.Teams[1];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Snore, PBETurnTarget.FoeCenter) }));

            battle.RunTurn();

            Assert.True(battle.Winner == battle.Teams[1]);

            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            battle.Dispose();
        }
    }
}
