using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests
{
    [Collection("Utils")]
    public class AutoCenterTests
    {
        public AutoCenterTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        [Fact]
        public void AutoCenter_Works()
        {
            #region Setup
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var p = new TestPokemonCollection(3);
            p[0] = new TestPokemon(PBESpecies.Magikarp, 0, 1)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };
            p[1] = new TestPokemon(PBESpecies.Golem, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Explosion })
            };
            p[2] = new TestPokemon(PBESpecies.Cradily, 0, 1)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Protect })
            };

            var battle = new PBEBattle(PBEBattleFormat.Triple, settings, new PBETrainerInfo(p, "Team 1"), new PBETrainerInfo(p, "Team 2"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon magikarp0 = t0.Party[0];
            PBEBattlePokemon golem0 = t0.Party[1];
            PBEBattlePokemon cradily0 = t0.Party[2];
            PBEBattlePokemon magikarp1 = t1.Party[0];
            PBEBattlePokemon golem1 = t1.Party[1];
            PBEBattlePokemon cradily1 = t1.Party[2];
            #endregion

            #region Force auto-center and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[]
            {
                new PBETurnAction(magikarp0.Id, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(golem0.Id, PBEMove.Explosion, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight),
                new PBETurnAction(cradily0.Id, PBEMove.Protect, PBETurnTarget.AllyRight),
            }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[]
            {
                new PBETurnAction(magikarp1.Id, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(golem1.Id, PBEMove.Explosion, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight),
                new PBETurnAction(cradily1.Id, PBEMove.Protect, PBETurnTarget.AllyRight),
            }));

            battle.RunTurn();

            Assert.True(cradily0.FieldPosition == PBEFieldPosition.Center && cradily1.FieldPosition == PBEFieldPosition.Center);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
