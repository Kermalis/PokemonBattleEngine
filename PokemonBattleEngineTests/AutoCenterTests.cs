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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AutoCenter_Works(bool left)
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p = new TestPokemonCollection(3);
            p[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Protect, PBEMove.Splash);
            p[1] = new TestPokemon(settings, PBESpecies.Golem, 0, 100, PBEMove.Explosion);
            p[2] = new TestPokemon(settings, PBESpecies.Happiny, 0, 1, PBEMove.Protect, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Triple, settings, new PBETrainerInfo(p, "Trainer 0"), new PBETrainerInfo(p, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon magikarp0 = t0.Party[0];
            PBEBattlePokemon golem0 = t0.Party[1];
            PBEBattlePokemon happiny0 = t0.Party[2];
            PBEBattlePokemon magikarp1 = t1.Party[0];
            PBEBattlePokemon golem1 = t1.Party[1];
            PBEBattlePokemon happiny1 = t1.Party[2];
            #endregion

            #region Force auto-center and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0,
                new PBETurnAction(magikarp0, left ? PBEMove.Splash : PBEMove.Protect, PBETurnTarget.AllyLeft),
                new PBETurnAction(golem0, PBEMove.Explosion, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight),
                new PBETurnAction(happiny0, left ? PBEMove.Protect : PBEMove.Splash, PBETurnTarget.AllyRight)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1,
                new PBETurnAction(magikarp1, left ? PBEMove.Splash : PBEMove.Protect, PBETurnTarget.AllyLeft),
                new PBETurnAction(golem1, PBEMove.Explosion, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight),
                new PBETurnAction(happiny1, left ? PBEMove.Protect : PBEMove.Splash, PBETurnTarget.AllyRight)));

            battle.RunTurn();

            Assert.True((left ? happiny0 : magikarp0).FieldPosition == PBEFieldPosition.Center && (left ? happiny1 : magikarp1).FieldPosition == PBEFieldPosition.Center);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void AutoCenter_ActivatesFromHazard()
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(5);
            p0[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Splash)
            {
                Item = PBEItem.FocusSash
            };
            p0[1] = new TestPokemon(settings, PBESpecies.Munchlax, 0, 1, PBEMove.Splash)
            {
                Item = PBEItem.FocusSash
            };
            p0[2] = new TestPokemon(settings, PBESpecies.Happiny, 0, 1, PBEMove.Splash);
            p0[3] = new TestPokemon(settings, PBESpecies.Regigigas, 0, 10, PBEMove.Explosion)
            {
                Ability = PBEAbility.SlowStart
            };
            p0[4] = new TestPokemon(settings, PBESpecies.Mudkip, 0, 1, PBEMove.Splash);

            var p1 = new TestPokemonCollection(3);
            p1[0] = new TestPokemon(settings, PBESpecies.Budew, 0, 1, PBEMove.Splash);
            p1[1] = new TestPokemon(settings, PBESpecies.Butterfree, 0, 10, PBEMove.RockSlide, PBEMove.Splash)
            {
                Ability = PBEAbility.Compoundeyes
            };
            p1[2] = new TestPokemon(settings, PBESpecies.Sunkern, 0, 1, PBEMove.Splash, PBEMove.StealthRock);

            var battle = new PBEBattle(PBEBattleFormat.Triple, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon magikarp = t0.Party[0];
            PBEBattlePokemon munchlax = t0.Party[1];
            PBEBattlePokemon happiny = t0.Party[2];
            PBEBattlePokemon regigigas = t0.Party[3];
            PBEBattlePokemon mudkip = t0.Party[4];
            PBEBattlePokemon budew = t1.Party[0];
            PBEBattlePokemon butterfree = t1.Party[1];
            PBEBattlePokemon sunkern = t1.Party[2];
            happiny.HP = 1; // Set Happiny HP to 1 so it faints on switch
            happiny.UpdateHPPercentage();
            #endregion

            #region Set up Stealth Rock while lowering HP, swap Happiny for Regigigas
            Assert.True(PBEBattle.SelectActionsIfValid(t0,
                new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(munchlax, PBEMove.Splash, PBETurnTarget.AllyCenter),
                new PBETurnAction(happiny, regigigas)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1,
                new PBETurnAction(budew, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(butterfree, PBEMove.RockSlide, PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight),
                new PBETurnAction(sunkern, PBEMove.StealthRock, PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight)));

            battle.RunTurn();

            Assert.True(t0.Team.TeamStatus.HasFlag(PBETeamStatus.StealthRock));
            #endregion

            #region Yeet everyone
            Assert.True(PBEBattle.SelectActionsIfValid(t0,
                new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(munchlax, PBEMove.Splash, PBETurnTarget.AllyCenter),
                new PBETurnAction(regigigas, PBEMove.Explosion, PBETurnTarget.AllyCenter | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1,
                new PBETurnAction(budew, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(butterfree, PBEMove.RockSlide, PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight),
                new PBETurnAction(sunkern, PBEMove.Splash, PBETurnTarget.AllyRight)));

            battle.RunTurn();

            Assert.True(magikarp.HP == 0 && munchlax.HP == 0 && regigigas.HP == 0 && budew.HP == 0 && butterfree.HP == 0
                && happiny.HP != 0 && mudkip.HP != 0 && sunkern.HP != 0);
            #endregion

            #region Switch in and check
            Assert.True(PBEBattle.SelectSwitchesIfValid(t0,
                new PBESwitchIn(happiny, PBEFieldPosition.Center),
                new PBESwitchIn(mudkip, PBEFieldPosition.Right)));

            battle.RunSwitches();

            Assert.True(happiny.HP == 0 && mudkip.FieldPosition == PBEFieldPosition.Center && sunkern.FieldPosition == PBEFieldPosition.Center);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AutoCenter_Works_MultiBattle(bool left)
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var pM = new TestPokemonCollection(1);
            pM[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Protect, PBEMove.Splash);
            var pG = new TestPokemonCollection(1);
            pG[0] = new TestPokemon(settings, PBESpecies.Golem, 0, 100, PBEMove.Explosion);
            var pH = new TestPokemonCollection(1);
            pH[0] = new TestPokemon(settings, PBESpecies.Happiny, 0, 1, PBEMove.Protect, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Triple, settings,
                new[] { new PBETrainerInfo(pM, "Trainer 0"), new PBETrainerInfo(pG, "Trainer 1"), new PBETrainerInfo(pH, "Trainer 3") },
                new[] { new PBETrainerInfo(pM, "Trainer 3"), new PBETrainerInfo(pG, "Trainer 4"), new PBETrainerInfo(pH, "Trainer 5") });
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0M = battle.Trainers[0];
            PBETrainer t0G = battle.Trainers[1];
            PBETrainer t0H = battle.Trainers[2];
            PBETrainer t1M = battle.Trainers[3];
            PBETrainer t1G = battle.Trainers[4];
            PBETrainer t1H = battle.Trainers[5];
            PBEBattlePokemon magikarp0 = t0M.Party[0];
            PBEBattlePokemon golem0 = t0G.Party[0];
            PBEBattlePokemon happiny0 = t0H.Party[0];
            PBEBattlePokemon magikarp1 = t1M.Party[0];
            PBEBattlePokemon golem1 = t1G.Party[0];
            PBEBattlePokemon happiny1 = t1H.Party[0];
            #endregion

            #region Force auto-center and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0M, new PBETurnAction(magikarp0, left ? PBEMove.Splash : PBEMove.Protect, PBETurnTarget.AllyLeft)));
            Assert.True(PBEBattle.SelectActionsIfValid(t0G, new PBETurnAction(golem0, PBEMove.Explosion, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight)));
            Assert.True(PBEBattle.SelectActionsIfValid(t0H, new PBETurnAction(happiny0, left ? PBEMove.Protect : PBEMove.Splash, PBETurnTarget.AllyRight)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1M, new PBETurnAction(magikarp1, left ? PBEMove.Splash : PBEMove.Protect, PBETurnTarget.AllyLeft)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1G, new PBETurnAction(golem1, PBEMove.Explosion, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1H, new PBETurnAction(happiny1, left ? PBEMove.Protect : PBEMove.Splash, PBETurnTarget.AllyRight)));

            battle.RunTurn();

            Assert.True((left ? happiny0 : magikarp0).FieldPosition == PBEFieldPosition.Center && (left ? happiny1 : magikarp1).FieldPosition == PBEFieldPosition.Center);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
