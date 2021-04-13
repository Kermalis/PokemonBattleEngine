using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
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
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Lucario, 0, 50, PBEMove.Earthquake, PBEMove.VacuumWave);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, species, 0, 100, PBEMove.Roost, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

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

            battle.Begin();
            #endregion

            #region Use VacuumWave to lower HP
            Assert.Null(t0.SelectActionsIfValid(new PBETurnAction(lucario, PBEMove.VacuumWave, PBETurnTarget.FoeCenter)));
            Assert.Null(t1.SelectActionsIfValid(new PBETurnAction(rooster, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();
            #endregion

            #region Use Roost and check
            Assert.Null(t0.SelectActionsIfValid(new PBETurnAction(lucario, PBEMove.Earthquake, PBETurnTarget.FoeCenter)));
            Assert.Null(t1.SelectActionsIfValid(new PBETurnAction(rooster, PBEMove.Roost, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(!battle.VerifyMoveResultHappened(lucario, rooster, PBEResult.Ineffective_Type) // Earthquake hit
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
