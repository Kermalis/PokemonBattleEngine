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
            #region Setup
            PBERandom.SetSeed(0); // Seed prevents Toxic from missing
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(PBESpecies.Seviper, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Toxic })
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(PBESpecies.Zangoose, 0, 100)
            {
                Ability = PBEAbility.Immunity,
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };

            var battle = new PBEBattle(PBEBattleTerrain.Plain, PBEBattleFormat.Single, new PBETeamInfo(p0, "Team 1"), new PBETeamInfo(p1, "Team 2"), settings);
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETeam t0 = battle.Teams[0];
            PBETeam t1 = battle.Teams[1];
            PBEBattlePokemon seviper = t0.Party[0];
            PBEBattlePokemon zangoose = t1.Party[0];
            #endregion

            #region Badly Poison Zangoose and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(seviper.Id, PBEMove.Toxic, PBETurnTarget.FoeCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(zangoose.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));

            battle.RunTurn();

            Assert.True(TestUtils.VerifyMoveResult(battle, seviper, zangoose, PBEResult.Ineffective_Ability));
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
