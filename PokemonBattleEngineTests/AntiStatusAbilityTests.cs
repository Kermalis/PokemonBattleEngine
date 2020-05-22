using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests
{
    [Collection("Utils")]
    public class AntiStatusAbilityTests
    {
        public AntiStatusAbilityTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Fact]
        public void Immunity_Works_Check()
        {
            PBERandom.SetSeed(0); // Seed prevents Toxic from missing
            PBESettings settings = PBESettings.DefaultSettings;

            var team1Shell = new PBETeamShell(settings, 1, true);
            PBEPokemonShell p = team1Shell[0];
            p.Species = PBESpecies.Seviper;
            p.Item = PBEItem.None;
            p.Moveset[0].Move = PBEMove.Toxic;

            var team2Shell = new PBETeamShell(settings, 1, true);
            p = team2Shell[0];
            p.Species = PBESpecies.Zangoose;
            p.Ability = PBEAbility.Immunity;
            p.Item = PBEItem.None;
            p.Moveset[0].Move = PBEMove.Snore;

            var battle = new PBEBattle(PBEBattleFormat.Single, team1Shell, "Team 1", team2Shell, "Team 2");
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            team1Shell.Dispose();
            team2Shell.Dispose();
            battle.Begin();

            PBETeam t = battle.Teams[0];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Toxic, PBETurnTarget.FoeCenter) }));

            t = battle.Teams[1];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Snore, PBETurnTarget.FoeCenter) }));

            battle.RunTurn();

            Assert.False(battle.Teams[1].Party[0].Status1 == PBEStatus1.BadlyPoisoned);

            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            battle.Dispose();
        }
    }
}
