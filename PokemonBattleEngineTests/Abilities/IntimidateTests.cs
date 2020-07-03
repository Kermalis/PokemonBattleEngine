using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Abilities
{
    [Collection("Utils")]
    public class IntimidateTests
    {
        public IntimidateTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Fact]
        public void Intimidate_Works()
        {
            #region Setup
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(3);
            p0[0] = new TestPokemon(settings, PBESpecies.Shuckle, 0, 100, PBEMove.Splash);
            p0[1] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);
            p0[2] = new TestPokemon(settings, PBESpecies.Happiny, 0, 100, PBEMove.Splash);

            var p1 = new TestPokemonCollection(2);
            p1[0] = new TestPokemon(settings, PBESpecies.Luxray, 0, 100, PBEMove.Splash)
            {
                Ability = PBEAbility.Intimidate
            };
            p1[1] = new TestPokemon(settings, PBESpecies.Skitty, 0, 100, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Triple, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon shuckle = t0.Party[0];
            PBEBattlePokemon magikarp = t0.Party[1];
            PBEBattlePokemon happiny = t0.Party[2];
            PBEBattlePokemon luxray = t1.Party[0];
            PBEBattlePokemon skitty = t1.Party[1];
            #endregion

            #region Check
            Assert.True(battle.VerifyAbilityHappened(luxray, luxray, PBEAbility.Intimidate, PBEAbilityAction.Stats) // Activated
                && happiny.AttackChange < 0 && magikarp.AttackChange < 0 && shuckle.AttackChange == 0 && skitty.AttackChange == 0); // Hit only surrounding foes
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Intimidate_Does_Not_Announce_If_No_Foes()
        {
            #region Setup
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Shuckle, 0, 100, PBEMove.Splash);

            var p1 = new TestPokemonCollection(2);
            p1[0] = new TestPokemon(settings, PBESpecies.Luxray, 0, 100, PBEMove.Splash)
            {
                Ability = PBEAbility.Intimidate
            };
            p1[1] = new TestPokemon(settings, PBESpecies.Skitty, 0, 100, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Triple, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon shuckle = t0.Party[0];
            PBEBattlePokemon luxray = t1.Party[0];
            PBEBattlePokemon skitty = t1.Party[1];
            #endregion

            #region Check
            Assert.True(!battle.VerifyAbilityHappened(luxray, luxray, PBEAbility.Intimidate, PBEAbilityAction.Stats)); // Did not activate
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Intimidate_Does_Not_Hit_Through_Substitute()
        {
            #region Setup
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Shuckle, 0, 100, PBEMove.Substitute, PBEMove.Splash);

            var p1 = new TestPokemonCollection(2);
            p1[0] = new TestPokemon(settings, PBESpecies.Skitty, 0, 100, PBEMove.Splash);
            p1[1] = new TestPokemon(settings, PBESpecies.Luxray, 0, 100, PBEMove.Splash)
            {
                Ability = PBEAbility.Intimidate
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon shuckle = t0.Party[0];
            PBEBattlePokemon skitty = t1.Party[0];
            PBEBattlePokemon luxray = t1.Party[1];
            #endregion

            #region Use Substitute
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(shuckle, PBEMove.Substitute, PBETurnTarget.AllyCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(skitty, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(shuckle.Status2.HasFlag(PBEStatus2.Substitute));
            #endregion

            #region Switch in Luxray and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(shuckle, PBEMove.Splash, PBETurnTarget.AllyCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(skitty, luxray)));

            battle.RunTurn();

            Assert.True(battle.VerifyAbilityHappened(luxray, luxray, PBEAbility.Intimidate, PBEAbilityAction.Stats) // Activated
                && battle.VerifyMoveResult(luxray, shuckle, PBEResult.Ineffective_Substitute) && shuckle.AttackChange == 0); // Did not affect
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
