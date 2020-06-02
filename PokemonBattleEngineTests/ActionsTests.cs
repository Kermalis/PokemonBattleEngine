using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests
{
    [Collection("Utils")]
    public class ActionsTests
    {
        public ActionsTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Fact]
        public void Basic_Actions()
        {
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var team1Shell = new PBETeamShell(settings, 2, true);
            PBEPokemonShell p = team1Shell[0];
            p.Species = PBESpecies.Koffing;
            p.Item = PBEItem.None;
            p.Moveset[0].Move = PBEMove.Selfdestruct;

            var team2Shell = new PBETeamShell(settings, 1, true);
            p = team2Shell[0];
            p.Species = PBESpecies.Darkrai;
            p.Item = PBEItem.None;
            p.Moveset[0].Move = PBEMove.Protect;

            var battle = new PBEBattle(PBEBattleTerrain.Plain, PBEBattleFormat.Single, team1Shell, "Team 1", team2Shell, "Team 2");
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            team1Shell.Dispose();
            team2Shell.Dispose();
            battle.Begin();

            PBETeam t = battle.Teams[0];
            var a = new PBETurnAction(t.Party[0].Id, PBEMove.Selfdestruct, PBETurnTarget.FoeCenter);
            var a1 = new PBETurnAction[] { a };
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectActionsIfValid(null, a1)); // Throw for null team
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectActionsIfValid(t, null)); // Throw for null collection
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectActionsIfValid(t, new PBETurnAction[] { null })); // Throw for null elements
            Assert.False(PBEBattle.SelectActionsIfValid(t, new PBETurnAction[] { a, a })); // False for too many actions
            Assert.True(PBEBattle.SelectActionsIfValid(t, a1)); // True for good actions
            // TODO: bad field position to switch into, bad move, bad targets, bad targets with templockedmove, battle status, bad pkmn id, wrong team pkmn id, duplicate pkmn id, can't switch out but tried, invalid switch mon (null hp pos), duplicate switch mon
            Assert.False(PBEBattle.SelectActionsIfValid(t, a1)); // False because actions were already submitted
            Assert.False(PBEBattle.SelectActionsIfValid(t, Array.Empty<PBETurnAction>())); // False for 0 despite us now needing 0 additional actions

            t = battle.Teams[1];
            Assert.True(PBEBattle.SelectActionsIfValid(t, new PBETurnAction[] { new PBETurnAction(t.Party[0].Id, PBEMove.Protect, PBETurnTarget.AllyCenter) })); // True for good actions

            battle.RunTurn(); // Darkrai uses Protect, Koffing uses Selfdestruct, Koffing faints

            t = battle.Teams[0];
            var s = new PBESwitchIn(t.Party[1].Id, PBEFieldPosition.Center);
            var s1 = new PBESwitchIn[] { s };
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectSwitchesIfValid(null, s1)); // Throw for null team
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectSwitchesIfValid(t, null)); // Throw for null collection
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectSwitchesIfValid(t, new PBESwitchIn[] { null })); // Throw for null elements
            Assert.False(PBEBattle.SelectSwitchesIfValid(t, new PBESwitchIn[] { s, s })); // False for too many
            Assert.True(PBEBattle.SelectSwitchesIfValid(t, s1)); // True for good switches
            // Below two wouldn't work because of battle status lol
            //Assert.False(PBEBattle.SelectSwitchesIfValid(t, s1)); // False because switches were already submitted
            //Assert.False(PBEBattle.SelectSwitchesIfValid(t, Array.Empty<PBESwitchIn>())); // False for 0 despite us now needing 0 additional actions
            
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            battle.Dispose();

            Assert.Throws<ObjectDisposedException>(() => PBEBattle.SelectActionsIfValid(t, a1)); // Throw for disposed battle
            Assert.Throws<ObjectDisposedException>(() => PBEBattle.SelectSwitchesIfValid(t, s1)); // Throw for disposed battle
        }
    }
}
