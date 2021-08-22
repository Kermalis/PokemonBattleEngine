using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Statuses
{
    [Collection("Utils")]
    public class SubstituteTests
    {
        public SubstituteTests(TestUtils _, ITestOutputHelper output)
        {
            TestUtils.SetOutputHelper(output);
        }

        [Fact]
        public void ColorChange_Does_Not_Activate()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Conkeldurr, 0, 50, PBEMove.CloseCombat);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Kecleon, 0, 100, PBEMove.Substitute)
            {
                Ability = PBEAbility.ColorChange
            };

            var battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon conkeldurr = t0.Party[0];
            PBEBattlePokemon kecleon = t1.Party[0];

            battle.Begin();
            #endregion

            #region Use and check
            Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(conkeldurr, PBEMove.CloseCombat, PBETurnTarget.FoeCenter)));
            Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(kecleon, PBEMove.Substitute, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(battle.VerifyStatus2Happened(kecleon, kecleon, PBEStatus2.Substitute, PBEStatusAction.Added) // Substitute added
                && !kecleon.Status2.HasFlag(PBEStatus2.Substitute) // Substitute broke
                && kecleon.Type1 == PBEType.Normal && kecleon.Type2 == PBEType.None); // ColorChange not activated
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
