using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Abilities
{
    [Collection("Utils")]
    public class IllusionTests
    {
        public IllusionTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Fact]
        public void Illusion_Does_Not_Copy_Same_Species()
        {
            #region Setup
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var team1Shell = new PBETeamShell(settings, 1, true);

            var team2Shell = new PBETeamShell(settings, 2, true);
            PBEPokemonShell ps = team2Shell[0];
            ps.Species = PBESpecies.Zoroark;
            ps.Ability = PBEAbility.Illusion;
            ps = team2Shell[1];
            ps.Species = PBESpecies.Zoroark;

            var battle = new PBEBattle(PBEBattleTerrain.Plain, PBEBattleFormat.Single, team1Shell, "Team 1", team2Shell, "Team 2");
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            team1Shell.Dispose();
            team2Shell.Dispose();
            battle.Begin();

            PBETeam t1 = battle.Teams[1];
            PBEPokemon zoroark1 = t1.Party[0];
            #endregion

            #region Check
            Assert.True(zoroark1.DisguisedAsPokemon == null);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            battle.Dispose();
            #endregion
        }

        [Fact]
        public void Illusion_Copies_Shaymin_Reversion()
        {
            #region Setup
            PBERandom.SetSeed(1); // Seed ensures SecretPower freezes
            PBESettings settings = PBESettings.DefaultSettings;

            var team1Shell = new PBETeamShell(settings, 1, true);
            PBEPokemonShell ps = team1Shell[0];
            ps.Species = PBESpecies.Happiny;
            ps.Ability = PBEAbility.SereneGrace;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.SecretPower;
            ps.Moveset[1].Move = PBEMove.Snore;

            var team2Shell = new PBETeamShell(settings, 3, true);
            ps = team2Shell[0];
            ps.Species = PBESpecies.Shaymin;
            ps.Form = PBEForm.Shaymin_Sky;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.Snore;
            ps = team2Shell[1];
            ps.Species = PBESpecies.Zoroark;
            ps.Ability = PBEAbility.Illusion;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.Snore;
            ps = team2Shell[2];
            ps.Species = PBESpecies.Deoxys;
            ps.Item = PBEItem.None;
            ps.Moveset[0].Move = PBEMove.Snore;

            var battle = new PBEBattle(PBEBattleTerrain.Snow, PBEBattleFormat.Single, team1Shell, "Team 1", team2Shell, "Team 2");
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            team1Shell.Dispose();
            team2Shell.Dispose();
            battle.Begin();

            PBETeam t0 = battle.Teams[0];
            PBETeam t1 = battle.Teams[1];
            PBEPokemon happiny = t0.Party[0];
            PBEPokemon shaymin = t1.Party[0];
            PBEPokemon zoroark = t1.Party[1];
            PBEPokemon deoxys = t1.Party[2];
            #endregion

            #region Freeze Shaymin
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(happiny.Id, PBEMove.SecretPower, PBETurnTarget.FoeCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(shaymin.Id, PBEMove.Snore, PBETurnTarget.FoeCenter) }));

            battle.RunTurn();

            Assert.True(shaymin.Status1 == PBEStatus1.Frozen && shaymin.Form == PBEForm.Shaymin);
            #endregion

            #region Swap Shaymin for Deoxys
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(happiny.Id, PBEMove.Snore, PBETurnTarget.FoeCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(shaymin.Id, deoxys.Id) }));

            battle.RunTurn();

            Assert.True(t1.Party[2] == shaymin);
            #endregion

            #region Swap Deoxys for Zoroark and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(happiny.Id, PBEMove.Snore, PBETurnTarget.FoeCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(deoxys.Id, zoroark.Id) }));

            battle.RunTurn();

            Assert.True(zoroark.DisguisedAsPokemon == shaymin && zoroark.KnownForm == PBEForm.Shaymin);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            battle.Dispose();
            #endregion
        }
    }
}
