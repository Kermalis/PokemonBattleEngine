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
            PBERandom.SetSeed(0); // Seed prevents Toxic from missing
            PBESettings settings = PBESettings.DefaultSettings;

            var team1Shell = new PBETeamShell(settings, 1, true);
            PBEPokemonShell ps = team1Shell[0];
            ps.Species = PBESpecies.Seviper;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.Toxic;

            var team2Shell = new PBETeamShell(settings, 1, true);
            ps = team2Shell[0];
            ps.Species = PBESpecies.Zangoose;
            ps.Ability = PBEAbility.Immunity;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.Snore;

            var battle = new PBEBattle(PBEBattleTerrain.Plain, PBEBattleFormat.Single, team1Shell, "Team 1", team2Shell, "Team 2");
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            team1Shell.Dispose();
            team2Shell.Dispose();
            battle.Begin();

            PBETeam t = battle.Teams[0];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Toxic, PBETurnTarget.FoeCenter) }));

            t = battle.Teams[1];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Snore, PBETurnTarget.FoeCenter) }));

            battle.RunTurn();

            Assert.True(TestUtils.VerifyMoveResult(battle, battle.Teams[0].Party[0], battle.Teams[1].Party[0], PBEResult.Ineffective_Ability));

            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            battle.Dispose();
        }
    }
}
