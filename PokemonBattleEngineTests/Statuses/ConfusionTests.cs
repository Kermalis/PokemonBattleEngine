using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Statuses
{
    [Collection("Utils")]
    public class ConfusionTests
    {
        public ConfusionTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Confusion_Heal__Bug(bool bugFix)
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 40703; // Seed ensures Swagger does not miss and Deoxys hurts itself
            var settings = new PBESettings { BugFix = bugFix };
            settings.MakeReadOnly();

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Deoxys, PBEForm.Deoxys, 50, PBEMove.Splash)
            {
                Item = PBEItem.SitrusBerry
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Accelgor, 0, 100, PBEMove.Swagger);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon deoxys = t0.Party[0];
            PBEBattlePokemon accelgor = t1.Party[0];

            battle.Begin();
            #endregion

            #region Use and check
            Assert.Null(t0.SelectActionsIfValid(new PBETurnAction(deoxys, PBEMove.Splash, PBETurnTarget.AllyCenter)));
            Assert.Null(t1.SelectActionsIfValid(new PBETurnAction(accelgor, PBEMove.Swagger, PBETurnTarget.FoeCenter)));

            battle.RunTurn();

            Assert.True(battle.VerifyStatus2Happened(deoxys, accelgor, PBEStatus2.Confused, PBEStatusAction.Added)
                && battle.VerifyStatus2Happened(deoxys, deoxys, PBEStatus2.Confused, PBEStatusAction.Damage));
            if (settings.BugFix)
            {
                Assert.True(battle.VerifyItemHappened(deoxys, deoxys, PBEItem.SitrusBerry, PBEItemAction.Consumed)
                    && deoxys.Item == PBEItem.None); // Healed
            }
            else
            {
                Assert.True(!battle.VerifyItemHappened(deoxys, deoxys, PBEItem.SitrusBerry, PBEItemAction.Consumed)
                    && deoxys.Item == PBEItem.SitrusBerry); // Buggy
            }
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Confusion_Does_Not_Ignore_Sturdy()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 40703; // Seed ensures Swagger does not miss and Deoxys hurts itself
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Deoxys, PBEForm.Deoxys_Attack, 50, PBEMove.Splash)
            {
                Ability = PBEAbility.Sturdy
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Accelgor, 0, 100, PBEMove.Swagger);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon deoxys = t0.Party[0];
            PBEBattlePokemon accelgor = t1.Party[0];

            battle.Begin();
            #endregion

            #region Use and check
            Assert.Null(t0.SelectActionsIfValid(new PBETurnAction(deoxys, PBEMove.Splash, PBETurnTarget.AllyCenter)));
            Assert.Null(t1.SelectActionsIfValid(new PBETurnAction(accelgor, PBEMove.Swagger, PBETurnTarget.FoeCenter)));

            battle.RunTurn();

            Assert.True(battle.VerifyStatus2Happened(deoxys, accelgor, PBEStatus2.Confused, PBEStatusAction.Added)
                && battle.VerifyStatus2Happened(deoxys, deoxys, PBEStatus2.Confused, PBEStatusAction.Damage)
                && battle.VerifyAbilityHappened(deoxys, deoxys, PBEAbility.Sturdy, PBEAbilityAction.Damage)
                && deoxys.HP == 1);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
