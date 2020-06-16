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
            #region Setup
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(PBESpecies.Golem, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { move })
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(PBESpecies.Magikarp, 0, 1)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };

            var battle = new PBEBattle(PBEBattleTerrain.Plain, PBEBattleFormat.Single, new PBETeamInfo(p0, "Team 1"), new PBETeamInfo(p1, "Team 2"), settings);
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETeam t0 = battle.Teams[0];
            PBETeam t1 = battle.Teams[1];
            PBEBattlePokemon golem = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];
            #endregion

            #region Use move and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(golem.Id, move, PBETurnTarget.FoeCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(magikarp.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));

            battle.RunTurn();

            Assert.True(battle.Winner == battle.Teams[1]);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        // TODO: Who wins if you use explosion and faint everyone on the field in a double battle?
        // TODO: Who wins if you use Perish Song and everyone faints at the same time? Is it based on who's slowest?
        // TODO: Final Gambit
    }
}
