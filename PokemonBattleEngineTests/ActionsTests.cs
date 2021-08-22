using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests
{
    [Collection("Utils")]
    public class ActionsTests
    {
        public ActionsTests(TestUtils _, ITestOutputHelper output)
        {
            TestUtils.SetOutputHelper(output);
        }

        // TODO: bad field position to switch into, bad move, bad targets, bad targets with templockedmove, battle status, bad pkmn id,
        // TODO: wrong team pkmn id, duplicate pkmn id, can't switch out but tried, invalid switch mon (null hp pos), duplicate switch mon
        // TODO: Too many items, items we do not have, items when in templockedmove
        // TODO: Flee
        [Fact]
        public void Basic_Actions()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(2);
            p0[0] = new TestPokemon(settings, PBESpecies.Koffing, 0, 100, PBEMove.Selfdestruct);
            p0[1] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Darkrai, 0, 100, PBEMove.Protect);

            var battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon koffing = t0.Party[0];
            PBEBattlePokemon magikarp = t0.Party[1];
            PBEBattlePokemon darkrai = t1.Party[0];

            battle.Begin();
            #endregion

            #region Darkrai uses Protect, Koffing uses Selfdestruct and faints
            var a = new PBETurnAction(koffing, PBEMove.Selfdestruct, PBETurnTarget.FoeCenter);
            Assert.False(t0.SelectActionsIfValid(out _, a, a)); // Too many actions
            Assert.True(t0.SelectActionsIfValid(out _, a)); // Good actions
            Assert.False(t0.SelectActionsIfValid(out _, a)); // Actions were already submitted
            Assert.False(t0.SelectActionsIfValid(out _, Array.Empty<PBETurnAction>())); // 0 despite us now needing 0 additional actions

            Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(darkrai, PBEMove.Protect, PBETurnTarget.AllyCenter))); // True for good actions

            battle.RunTurn();
            #endregion

            #region More checks
            var s = new PBESwitchIn(magikarp, PBEFieldPosition.Center);
            Assert.False(t0.SelectSwitchesIfValid(out _, s, s)); // Too many
            Assert.True(t0.SelectSwitchesIfValid(out _, s)); // Good switches

            // Below two wouldn't work because of battle status lol
            //Assert.False(t0.SelectSwitchesIfValid(out _, s)); // Switches were already submitted
            //Assert.False(t0.SelectSwitchesIfValid(out _, Array.Empty<PBESwitchIn>())); // 0 despite us now needing 0 additional switches

            //battle.RunSwitches();
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Cannot_Send_Egg()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(3);
            p0[0] = new TestPokemon(settings, PBESpecies.Koffing, 0, 100, PBEMove.Selfdestruct);
            p0[1] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Splash)
            {
                PBEIgnore = true
            };
            p0[2] = new TestPokemon(settings, PBESpecies.Happiny, 0, 100, PBEMove.Splash);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Darkrai, 0, 100, PBEMove.Protect);

            var battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon koffing = t0.Party[0];
            PBEBattlePokemon magikarp = t0.Party[1];
            PBEBattlePokemon darkrai = t1.Party[0];

            battle.Begin();
            #endregion

            #region Darkrai uses Protect, Koffing uses Selfdestruct and faints
            Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(koffing, PBEMove.Selfdestruct, PBETurnTarget.FoeCenter)));
            Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(darkrai, PBEMove.Protect, PBETurnTarget.AllyCenter)));

            battle.RunTurn();
            #endregion

            #region Check
            Assert.False(t0.SelectSwitchesIfValid(out _, new PBESwitchIn(magikarp, PBEFieldPosition.Center)));
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Cannot_Switch_In_Egg()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(2);
            p0[0] = new TestPokemon(settings, PBESpecies.Koffing, 0, 100, PBEMove.Selfdestruct);
            p0[1] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Splash)
            {
                PBEIgnore = true
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Darkrai, 0, 100, PBEMove.Protect);

            var battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon koffing = t0.Party[0];
            PBEBattlePokemon magikarp = t0.Party[1];
            PBEBattlePokemon darkrai = t1.Party[0];

            battle.Begin();
            #endregion

            #region Check
            Assert.False(t0.SelectActionsIfValid(out _, new PBETurnAction(koffing, magikarp)));
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
