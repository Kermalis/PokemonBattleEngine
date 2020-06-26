using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Moves
{
    [Collection("Utils")]
    public class RoostTests
    {
        public RoostTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Theory]
        [InlineData(PBESpecies.Mew, false)] // Non-Flying single-type
        [InlineData(PBESpecies.Tornadus, false)] // Flying single-type
        [InlineData(PBESpecies.Volcarona, false)] // Non-Flying dual-type
        [InlineData(PBESpecies.Gyarados, true)] // Flying dual-type-primary
        [InlineData(PBESpecies.Gyarados, false)] // Flying dual-type-secondary
        public void Roost_Works(PBESpecies species, bool swapTypes) // Swap types around since there's no primary flying-type
        {
            #region Setup
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(PBESpecies.Lucario, 0, 50)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.VacuumWave, PBEMove.Earthquake })
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(species, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash, PBEMove.Roost })
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Team 1"), new PBETrainerInfo(p1, "Team 2"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon lucario = t0.Party[0];
            PBEBattlePokemon rooster = t1.Party[0];
            PBEType type1 = rooster.Type1;
            PBEType type2 = rooster.Type2;
            if (swapTypes)
            {
                rooster.Type1 = type2;
                rooster.Type2 = type1;
                rooster.KnownType1 = type2;
                rooster.KnownType2 = type1;
            }
            #endregion

            #region Use VacuumWave to lower HP
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(lucario.Id, PBEMove.VacuumWave, PBETurnTarget.FoeCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(rooster.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));

            battle.RunTurn();
            #endregion

            #region Use Roost and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(lucario.Id, PBEMove.Earthquake, PBETurnTarget.FoeCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(rooster.Id, PBEMove.Roost, PBETurnTarget.AllyCenter) }));

            battle.RunTurn();

            Assert.True(!TestUtils.VerifyMoveResult(battle, lucario, rooster, PBEResult.Ineffective_Type) // Earthquake hit
                && !rooster.Status2.HasFlag(PBEStatus2.Roost) // Roost ended properly
                && rooster.Type1 == type1 && rooster.Type2 == type2 // Types restored properly
                && rooster.KnownType1 == type1 && rooster.KnownType2 == type2);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
