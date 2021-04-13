using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests
{
    [Collection("Utils")]
    public class ActionsTests
    {
        public ActionsTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
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

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
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
            Assert.Throws<ArgumentNullException>(() => t0.SelectActionsIfValid((IReadOnlyList<PBETurnAction>)null)); // Throw for null collection
            Assert.Throws<ArgumentNullException>(() => t0.SelectActionsIfValid(new PBETurnAction[] { null })); // Throw for null elements
            Assert.NotNull(t0.SelectActionsIfValid(a, a)); // Too many actions
            Assert.Null(t0.SelectActionsIfValid(a)); // Good actions
            Assert.NotNull(t0.SelectActionsIfValid(a)); // Actions were already submitted
            Assert.NotNull(t0.SelectActionsIfValid(Array.Empty<PBETurnAction>())); // 0 despite us now needing 0 additional actions

            Assert.Null(t1.SelectActionsIfValid(new PBETurnAction(darkrai, PBEMove.Protect, PBETurnTarget.AllyCenter))); // True for good actions

            battle.RunTurn();
            #endregion

            #region More checks
            var s = new PBESwitchIn(magikarp, PBEFieldPosition.Center);
            Assert.Throws<ArgumentNullException>(() => t0.SelectSwitchesIfValid((IReadOnlyList<PBESwitchIn>)null)); // Throw for null collection
            Assert.Throws<ArgumentNullException>(() => t0.SelectSwitchesIfValid(new PBESwitchIn[] { null })); // Throw for null elements
            Assert.NotNull(t0.SelectSwitchesIfValid(s, s)); // Too many
            Assert.Null(t0.SelectSwitchesIfValid(s)); // Good switches

            // Below two wouldn't work because of battle status lol
            //Assert.NotNull(t0.SelectSwitchesIfValid(s)); // Switches were already submitted
            //Assert.NotNull(t0.SelectSwitchesIfValid(Array.Empty<PBESwitchIn>())); // 0 despite us now needing 0 additional switches

            //battle.RunSwitches();
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
