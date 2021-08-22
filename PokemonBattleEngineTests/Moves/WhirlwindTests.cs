using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Moves
{
    [Collection("Utils")]
    public class WhirlwindTests
    {
        public WhirlwindTests(TestUtils _, ITestOutputHelper output)
        {
            TestUtils.SetOutputHelper(output);
        }

        [Fact]
        public void Whirlwind_FailsLevel_WildSingleBattle()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Tropius, 0, 1, PBEMove.Whirlwind);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash)
            {
                CaughtBall = PBEItem.None
            };

            var battle = PBEBattle.CreateWildBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBEWildInfo(p1));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon tropius = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];

            battle.Begin();
            #endregion

            #region Use Whirlwind and check
            Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(tropius, PBEMove.Whirlwind, PBETurnTarget.FoeCenter)));
            Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(battle.VerifyMoveResultHappened(tropius, magikarp, PBEResult.Ineffective_Level) // Fail
                && battle.BattleResult is null); // Did not flee
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Whirlwind_Fails_WildDoubleBattle()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Tropius, 0, 100, PBEMove.Whirlwind);

            var p1 = new TestPokemonCollection(2);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Splash)
            {
                CaughtBall = PBEItem.None
            };
            p1[1] = new TestPokemon(settings, PBESpecies.Happiny, 0, 1, PBEMove.Splash)
            {
                CaughtBall = PBEItem.None
            };

            var battle = PBEBattle.CreateWildBattle(PBEBattleFormat.Double, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBEWildInfo(p1));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon tropius = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];
            PBEBattlePokemon happiny = t1.Party[1];

            battle.Begin();
            #endregion

            #region Use Whirlwind and check
            Assert.True(t0.SelectActionsIfValid(out _,
                new PBETurnAction(tropius, PBEMove.Whirlwind, PBETurnTarget.FoeLeft)));
            Assert.True(t1.SelectActionsIfValid(out _,
                new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(happiny, PBEMove.Splash, PBETurnTarget.AllyRight)));

            battle.RunTurn();

            Assert.True(battle.VerifyMoveResultHappened(tropius, magikarp, PBEResult.InvalidConditions) // Fail
                && battle.BattleResult is null); // Did not flee
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Whirlwind_Succeeds_WildSingleBattle()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Tropius, 0, 100, PBEMove.Whirlwind);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Splash)
            {
                CaughtBall = PBEItem.None
            };

            var battle = PBEBattle.CreateWildBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBEWildInfo(p1));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon tropius = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];

            battle.Begin();
            #endregion

            #region Use Whirlwind and check
            Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(tropius, PBEMove.Whirlwind, PBETurnTarget.FoeCenter)));
            Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(!battle.VerifyMoveResultHappened(tropius, magikarp, PBEResult.InvalidConditions) // No fail
                && battle.BattleResult == PBEBattleResult.WildFlee); // Fled
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Whirlwind_Succeeds_WildDoubleBattle()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(3);
            p0[0] = new TestPokemon(settings, PBESpecies.Diglett, 0, 1, PBEMove.Splash);
            p0[1] = new TestPokemon(settings, PBESpecies.Geodude, 0, 1, PBEMove.Splash);
            p0[2] = new TestPokemon(settings, PBESpecies.Trubbish, 0, 1, PBEMove.Splash);

            var p1 = new TestPokemonCollection(2);
            p1[0] = new TestPokemon(settings, PBESpecies.Starly, 0, 100, PBEMove.Whirlwind)
            {
                CaughtBall = PBEItem.None
            };
            p1[1] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash)
            {
                CaughtBall = PBEItem.None
            };

            var battle = PBEBattle.CreateWildBattle(PBEBattleFormat.Double, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBEWildInfo(p1));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon diglett = t0.Party[0];
            PBEBattlePokemon geodude = t0.Party[1];
            PBEBattlePokemon trubbish = t0.Party[2];
            PBEBattlePokemon starly = t1.Party[0];
            PBEBattlePokemon magikarp = t1.Party[1];

            battle.Begin();
            #endregion

            #region Use Whirlwind and check
            Assert.True(t0.SelectActionsIfValid(out _,
                new PBETurnAction(diglett, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(geodude, PBEMove.Splash, PBETurnTarget.AllyRight)));
            Assert.True(t1.SelectActionsIfValid(out _,
                new PBETurnAction(starly, PBEMove.Whirlwind, PBETurnTarget.FoeLeft),
                new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyRight)));

            battle.RunTurn();

            Assert.True(!battle.VerifyMoveResultHappened(starly, diglett, PBEResult.InvalidConditions) // No fail
                && diglett.FieldPosition == PBEFieldPosition.None && trubbish.FieldPosition == PBEFieldPosition.Left // Properly swapped
                && battle.BattleResult is null); // Did not flee
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
