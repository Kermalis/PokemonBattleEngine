using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
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
        public void AutoCenter_Works(bool faintLeft)
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
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
            Assert.Null(t0.SelectActionsIfValid(
                new PBETurnAction(magikarp0, faintLeft ? PBEMove.Splash : PBEMove.Protect, PBETurnTarget.AllyLeft),
                new PBETurnAction(golem0, PBEMove.Explosion, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight),
                new PBETurnAction(happiny0, faintLeft ? PBEMove.Protect : PBEMove.Splash, PBETurnTarget.AllyRight)));
            Assert.Null(t1.SelectActionsIfValid(
                new PBETurnAction(magikarp1, faintLeft ? PBEMove.Splash : PBEMove.Protect, PBETurnTarget.AllyLeft),
                new PBETurnAction(golem1, PBEMove.Explosion, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight),
                new PBETurnAction(happiny1, faintLeft ? PBEMove.Protect : PBEMove.Splash, PBETurnTarget.AllyRight)));

            battle.RunTurn();

            Assert.True((faintLeft ? happiny0 : magikarp0).FieldPosition == PBEFieldPosition.Center && (faintLeft ? happiny1 : magikarp1).FieldPosition == PBEFieldPosition.Center);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        // https://github.com/Kermalis/PokemonBattleEngine/issues/318
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AutoCenter_Works_Despite_Available(bool faintLeft)
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 2; // Seed ensures protect doesn't fail
            PBESettings settings = PBESettings.DefaultSettings;

            var p0L = new TestPokemonCollection(faintLeft ? 1 : 2);
            var p0C = new TestPokemonCollection(1);
            var p0R = new TestPokemonCollection(faintLeft ? 2 : 1);
            var p1 = new TestPokemonCollection(4);
            p0L[0] = p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Protect, PBEMove.Splash);
            p0C[0] = p1[1] = new TestPokemon(settings, PBESpecies.Golem, 0, 100, PBEMove.Explosion);
            p0R[0] = p1[2] = new TestPokemon(settings, PBESpecies.Happiny, 0, 1, PBEMove.Protect, PBEMove.Splash);
            (faintLeft ? p0R : p0L)[1] = p1[3] = new TestPokemon(settings, PBESpecies.Weezing, 0, 100, PBEMove.Explosion);

            var battle = new PBEBattle(PBEBattleFormat.Triple, settings,
                new[] { new PBETrainerInfo(p0L, "Trainer 0"), new PBETrainerInfo(p0C, "Trainer 1"), new PBETrainerInfo(p0R, "Trainer 2") },
                new[] { new PBETrainerInfo(p1, "Trainer 3") });
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0L = battle.Trainers[0];
            PBETrainer t0C = battle.Trainers[1];
            PBETrainer t0R = battle.Trainers[2];
            PBETrainer t1 = battle.Trainers[3];
            PBEBattlePokemon magikarp0 = t0L.Party[0];
            PBEBattlePokemon golem0 = t0C.Party[0];
            PBEBattlePokemon happiny0 = t0R.Party[0];
            PBEBattlePokemon weezing0 = (faintLeft ? t0R : t0L).Party[1];
            PBEBattlePokemon magikarp1 = t1.Party[0];
            PBEBattlePokemon golem1 = t1.Party[1];
            PBEBattlePokemon happiny1 = t1.Party[2];
            PBEBattlePokemon weezing1 = t1.Party[3];
            #endregion

            #region Force switch-in from trainer 3
            Assert.Null(t0L.SelectActionsIfValid(
                new PBETurnAction(magikarp0, faintLeft ? PBEMove.Splash : PBEMove.Protect, PBETurnTarget.AllyLeft)));
            Assert.Null(t0C.SelectActionsIfValid(
                new PBETurnAction(golem0, PBEMove.Explosion, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight)));
            Assert.Null(t0R.SelectActionsIfValid(
                new PBETurnAction(happiny0, faintLeft ? PBEMove.Protect : PBEMove.Splash, PBETurnTarget.AllyRight)));
            Assert.Null(t1.SelectActionsIfValid(
                new PBETurnAction(magikarp1, faintLeft ? PBEMove.Splash : PBEMove.Protect, PBETurnTarget.AllyLeft),
                new PBETurnAction(golem1, PBEMove.Explosion, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight),
                new PBETurnAction(happiny1, faintLeft ? PBEMove.Protect : PBEMove.Splash, PBETurnTarget.AllyRight)));

            battle.RunTurn();

            Assert.False((faintLeft ? happiny0 : magikarp0).FieldPosition == PBEFieldPosition.Center && (faintLeft ? happiny1 : magikarp1).FieldPosition == PBEFieldPosition.Center);
            Assert.True(t0L.SwitchInsRequired == 0 && t0C.SwitchInsRequired == 0 && t0R.SwitchInsRequired == 0 && t1.SwitchInsRequired == 1);
            #endregion

            #region Switch-in
            Assert.Null(t1.SelectSwitchesIfValid(
                new PBESwitchIn(weezing1, PBEFieldPosition.Center)));

            battle.RunSwitches();
            #endregion

            #region Force auto-center and check
            Assert.Null((faintLeft ? t0R : t0L).SelectActionsIfValid(
                new PBETurnAction(faintLeft ? happiny0 : magikarp0, PBEMove.Protect, faintLeft ? PBETurnTarget.AllyRight : PBETurnTarget.AllyLeft)));
            Assert.Null(t1.SelectActionsIfValid(
                new PBETurnAction(weezing1, PBEMove.Explosion, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight),
                new PBETurnAction(faintLeft ? happiny1 : magikarp1, PBEMove.Protect, faintLeft ? PBETurnTarget.AllyRight : PBETurnTarget.AllyLeft)));

            battle.RunTurn();

            Assert.True((faintLeft ? happiny0 : magikarp0).FieldPosition == PBEFieldPosition.Center && (faintLeft ? happiny1 : magikarp1).FieldPosition == PBEFieldPosition.Center);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void AutoCenter_ActivatesFromHazard()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 2; // Seed ensures Regigigas doesn't flinch and Rock Slide hits
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
            Assert.Null(t0.SelectActionsIfValid(
                new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(munchlax, PBEMove.Splash, PBETurnTarget.AllyCenter),
                new PBETurnAction(happiny, regigigas)));
            Assert.Null(t1.SelectActionsIfValid(
                new PBETurnAction(budew, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(butterfree, PBEMove.RockSlide, PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight),
                new PBETurnAction(sunkern, PBEMove.StealthRock, PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight)));

            battle.RunTurn();

            Assert.True(t0.Team.TeamStatus.HasFlag(PBETeamStatus.StealthRock));
            #endregion

            #region Yeet everyone
            Assert.Null(t0.SelectActionsIfValid(
                new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(munchlax, PBEMove.Splash, PBETurnTarget.AllyCenter),
                new PBETurnAction(regigigas, PBEMove.Explosion, PBETurnTarget.AllyCenter | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter)));
            Assert.Null(t1.SelectActionsIfValid(
                new PBETurnAction(budew, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(butterfree, PBEMove.RockSlide, PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight),
                new PBETurnAction(sunkern, PBEMove.Splash, PBETurnTarget.AllyRight)));

            battle.RunTurn();

            Assert.True(magikarp.HP == 0 && munchlax.HP == 0 && regigigas.HP == 0 && budew.HP == 0 && butterfree.HP == 0
                && happiny.HP != 0 && mudkip.HP != 0 && sunkern.HP != 0);
            #endregion

            #region Switch in and check
            Assert.Null(t0.SelectSwitchesIfValid(
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
        public void AutoCenter_Works_MultiBattle(bool faintLeft)
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var pL = new TestPokemonCollection(1);
            pL[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Protect, PBEMove.Splash);
            var pC = new TestPokemonCollection(1);
            pC[0] = new TestPokemon(settings, PBESpecies.Golem, 0, 100, PBEMove.Explosion);
            var pR = new TestPokemonCollection(1);
            pR[0] = new TestPokemon(settings, PBESpecies.Happiny, 0, 1, PBEMove.Protect, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Triple, settings,
                new[] { new PBETrainerInfo(pL, "Trainer 0"), new PBETrainerInfo(pC, "Trainer 1"), new PBETrainerInfo(pR, "Trainer 2") },
                new[] { new PBETrainerInfo(pL, "Trainer 3"), new PBETrainerInfo(pC, "Trainer 4"), new PBETrainerInfo(pR, "Trainer 5") });
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0L = battle.Trainers[0];
            PBETrainer t0C = battle.Trainers[1];
            PBETrainer t0R = battle.Trainers[2];
            PBETrainer t1L = battle.Trainers[3];
            PBETrainer t1C = battle.Trainers[4];
            PBETrainer t1R = battle.Trainers[5];
            PBEBattlePokemon magikarp0 = t0L.Party[0];
            PBEBattlePokemon golem0 = t0C.Party[0];
            PBEBattlePokemon happiny0 = t0R.Party[0];
            PBEBattlePokemon magikarp1 = t1L.Party[0];
            PBEBattlePokemon golem1 = t1C.Party[0];
            PBEBattlePokemon happiny1 = t1R.Party[0];
            #endregion

            #region Force auto-center and check
            Assert.Null(t0L.SelectActionsIfValid(new PBETurnAction(magikarp0, faintLeft ? PBEMove.Splash : PBEMove.Protect, PBETurnTarget.AllyLeft)));
            Assert.Null(t0C.SelectActionsIfValid(new PBETurnAction(golem0, PBEMove.Explosion, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight)));
            Assert.Null(t0R.SelectActionsIfValid(new PBETurnAction(happiny0, faintLeft ? PBEMove.Protect : PBEMove.Splash, PBETurnTarget.AllyRight)));
            Assert.Null(t1L.SelectActionsIfValid(new PBETurnAction(magikarp1, faintLeft ? PBEMove.Splash : PBEMove.Protect, PBETurnTarget.AllyLeft)));
            Assert.Null(t1C.SelectActionsIfValid(new PBETurnAction(golem1, PBEMove.Explosion, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight)));
            Assert.Null(t1R.SelectActionsIfValid(new PBETurnAction(happiny1, faintLeft ? PBEMove.Protect : PBEMove.Splash, PBETurnTarget.AllyRight)));

            battle.RunTurn();

            Assert.True((faintLeft ? happiny0 : magikarp0).FieldPosition == PBEFieldPosition.Center && (faintLeft ? happiny1 : magikarp1).FieldPosition == PBEFieldPosition.Center);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
