using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
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
        [Fact]
        public void Basic_Actions()
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(2);
            p0[0] = new TestPokemon(settings, PBESpecies.Koffing, 0, 100, PBEMove.Selfdestruct);
            p0[1] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Darkrai, 0, 100, PBEMove.Protect);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon koffing = t0.Party[0];
            PBEBattlePokemon magikarp = t0.Party[1];
            PBEBattlePokemon darkrai = t1.Party[0];
            #endregion

            #region Darkrai uses Protect, Koffing uses Selfdestruct and faints
            var a = new PBETurnAction(koffing, PBEMove.Selfdestruct, PBETurnTarget.FoeCenter);
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectActionsIfValid(null, a)); // Throw for null team
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectActionsIfValid(t0, (IReadOnlyList<PBETurnAction>)null)); // Throw for null collection
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectActionsIfValid(t0, new PBETurnAction[] { null })); // Throw for null elements
            Assert.False(PBEBattle.SelectActionsIfValid(t0, a, a)); // False for too many actions
            Assert.True(PBEBattle.SelectActionsIfValid(t0, a)); // True for good actions
            Assert.False(PBEBattle.SelectActionsIfValid(t0, a)); // False because actions were already submitted
            Assert.False(PBEBattle.SelectActionsIfValid(t0, Array.Empty<PBETurnAction>())); // False for 0 despite us now needing 0 additional actions

            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(darkrai, PBEMove.Protect, PBETurnTarget.AllyCenter))); // True for good actions

            battle.RunTurn();
            #endregion

            #region More checks
            var s = new PBESwitchIn(magikarp, PBEFieldPosition.Center);
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectSwitchesIfValid(null, s)); // Throw for null team
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectSwitchesIfValid(t0, (IReadOnlyList<PBESwitchIn>)null)); // Throw for null collection
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectSwitchesIfValid(t0, new PBESwitchIn[] { null })); // Throw for null elements
            Assert.False(PBEBattle.SelectSwitchesIfValid(t0, s, s)); // False for too many
            Assert.True(PBEBattle.SelectSwitchesIfValid(t0, s)); // True for good switches

            // Below two wouldn't work because of battle status lol
            //Assert.False(PBEBattle.SelectSwitchesIfValid(t0, s)); // False because switches were already submitted
            //Assert.False(PBEBattle.SelectSwitchesIfValid(t0, Array.Empty<PBESwitchIn>())); // False for 0 despite us now needing 0 additional actions

            //battle.RunSwitches();
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
