using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using System;
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

        [Fact]
        public void Basic_Actions()
        {
            #region Setup
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(2);
            p0[0] = new TestPokemon(PBESpecies.Koffing, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Selfdestruct })
            };
            p0[1] = new TestPokemon(PBESpecies.Magikarp, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(PBESpecies.Darkrai, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Protect })
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Team 1"), new PBETrainerInfo(p1, "Team 2"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon koffing = t0.Party[0];
            PBEBattlePokemon magikarp = t0.Party[1];
            PBEBattlePokemon darkrai = t1.Party[0];
            #endregion

            #region Darkrai uses Protect, Koffing uses Selfdestruct and faints
            var a = new PBETurnAction(koffing.Id, PBEMove.Selfdestruct, PBETurnTarget.FoeCenter);
            var a1 = new PBETurnAction[] { a };
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectActionsIfValid(null, a1)); // Throw for null team
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectActionsIfValid(t0, null)); // Throw for null collection
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectActionsIfValid(t0, new PBETurnAction[] { null })); // Throw for null elements
            Assert.False(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction[] { a, a })); // False for too many actions
            Assert.True(PBEBattle.SelectActionsIfValid(t0, a1)); // True for good actions
                                                                 // TODO: bad field position to switch into, bad move, bad targets, bad targets with templockedmove, battle status, bad pkmn id, wrong team pkmn id, duplicate pkmn id, can't switch out but tried, invalid switch mon (null hp pos), duplicate switch mon
            Assert.False(PBEBattle.SelectActionsIfValid(t0, a1)); // False because actions were already submitted
            Assert.False(PBEBattle.SelectActionsIfValid(t0, Array.Empty<PBETurnAction>())); // False for 0 despite us now needing 0 additional actions

            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction[] { new PBETurnAction(darkrai.Id, PBEMove.Protect, PBETurnTarget.AllyCenter) })); // True for good actions

            battle.RunTurn();
            #endregion

            #region More checks
            var s = new PBESwitchIn(magikarp.Id, PBEFieldPosition.Center);
            var s1 = new PBESwitchIn[] { s };
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectSwitchesIfValid(null, s1)); // Throw for null team
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectSwitchesIfValid(t0, null)); // Throw for null collection
            Assert.Throws<ArgumentNullException>(() => PBEBattle.SelectSwitchesIfValid(t0, new PBESwitchIn[] { null })); // Throw for null elements
            Assert.False(PBEBattle.SelectSwitchesIfValid(t0, new PBESwitchIn[] { s, s })); // False for too many
            Assert.True(PBEBattle.SelectSwitchesIfValid(t0, s1)); // True for good switches
                                                                  // Below two wouldn't work because of battle status lol
                                                                  //Assert.False(PBEBattle.SelectSwitchesIfValid(t0, s1)); // False because switches were already submitted
                                                                  //Assert.False(PBEBattle.SelectSwitchesIfValid(t0, Array.Empty<PBESwitchIn>())); // False for 0 despite us now needing 0 additional actions

            //battle.RunSwitches();
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
