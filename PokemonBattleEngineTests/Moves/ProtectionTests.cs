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

        // https://github.com/Kermalis/PokemonBattleEngine/issues/261
        [Theory]
        [InlineData(PBEMove.Detect)]
        [InlineData(PBEMove.Protect)]
        //[InlineData(PBEMove.QuickGuard)]
        [InlineData(PBEMove.WideGuard)]
        public void Protection_Counter_Resets(PBEMove move)
        {
            #region Setup
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(PBESpecies.Mienshao, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { move, PBEMove.CalmMind })
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(PBESpecies.Magikarp, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };

            var battle = new PBEBattle(PBEBattleTerrain.Plain, PBEBattleFormat.Single, new PBETeamInfo(p0, "Team 1"), new PBETeamInfo(p1, "Team 2"), settings);
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETeam t0 = battle.Teams[0];
            PBETeam t1 = battle.Teams[1];
            PBEBattlePokemon mienshao = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];
            #endregion

            #region Use move
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(mienshao.Id, move, PBETurnTarget.AllyCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(magikarp.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));

            battle.RunTurn();

            PBEBattlePokemon pkmn = battle.Teams[0].Party[0];
            Assert.True(pkmn.Protection_Counter == 1);
            #endregion

            #region Use Calm Mind and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(mienshao.Id, move, PBETurnTarget.AllyCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(magikarp.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));

            battle.RunTurn();

            Assert.True(pkmn.Protection_Counter == 0);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
