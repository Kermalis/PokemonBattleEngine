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
            PBERandom.SetSeed(1); // Seed ensures SecretPower freezes
            PBESettings settings = PBESettings.DefaultSettings;

            var team1Shell = new PBETeamShell(settings, 1, true);
            PBEPokemonShell ps = team1Shell[0];
            ps.Species = PBESpecies.Happiny;
            ps.Ability = PBEAbility.SereneGrace;
            ps.Moveset[0].Move = PBEMove.SecretPower;
            ps.Moveset[1].Move = PBEMove.Snore;

            var team2Shell = new PBETeamShell(settings, 2, true);
            ps = team2Shell[0];
            ps.Species = PBESpecies.Shaymin;
            ps.Form = PBEForm.Shaymin_Sky;
            ps.Moveset[0].Move = PBEMove.Snore;

            var battle = new PBEBattle(PBEBattleTerrain.Snow, PBEBattleFormat.Single, team1Shell, "Team 1", team2Shell, "Team 2");
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            team1Shell.Dispose();
            team2Shell.Dispose();
            battle.Begin();

            PBETeam t = battle.Teams[0];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.SecretPower, PBETurnTarget.FoeCenter) }));

            t = battle.Teams[1];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Snore, PBETurnTarget.FoeCenter) }));

            battle.RunTurn();

            PBEPokemon shaymin = battle.Teams[1].Party[0];
            Assert.True(shaymin.Status1 == PBEStatus1.Frozen && shaymin.Form == PBEForm.Shaymin);

            t = battle.Teams[0];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, PBEMove.Snore, PBETurnTarget.FoeCenter) }));

            t = battle.Teams[1];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new[] { new PBETurnAction(t.Party[0].Id, t.Party[1].Id) }));

            battle.RunTurn();

            Assert.True(shaymin.Form == PBEForm.Shaymin);

            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            battle.Dispose();
        }
    }
}
