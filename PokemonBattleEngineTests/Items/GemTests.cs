using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Items
{
    [Collection("Utils")]
    public class GemTests
    {
        public GemTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        // TODO: Do gems activate even if Explosion hits nobody?
        // TODO: Do gems activate for WaterAbsorb, LightningRod, etc?

        [Theory]
        [InlineData(PBEMove.Megahorn, PBEItem.BugGem)]
        [InlineData(PBEMove.FoulPlay, PBEItem.DarkGem)]
        [InlineData(PBEMove.DracoMeteor, PBEItem.DragonGem)]
        [InlineData(PBEMove.BoltStrike, PBEItem.ElectricGem)]
        [InlineData(PBEMove.Superpower, PBEItem.FightingGem)]
        [InlineData(PBEMove.Eruption, PBEItem.FireGem)]
        [InlineData(PBEMove.Hurricane, PBEItem.FlyingGem)]
        [InlineData(PBEMove.ShadowBall, PBEItem.GhostGem)]
        [InlineData(PBEMove.LeafStorm, PBEItem.GrassGem)]
        [InlineData(PBEMove.Earthquake, PBEItem.GroundGem)]
        [InlineData(PBEMove.Blizzard, PBEItem.IceGem)]
        [InlineData(PBEMove.MegaKick, PBEItem.NormalGem)]
        [InlineData(PBEMove.GunkShot, PBEItem.PoisonGem)]
        [InlineData(PBEMove.PsychoBoost, PBEItem.PsychicGem)]
        [InlineData(PBEMove.StoneEdge, PBEItem.RockGem)]
        [InlineData(PBEMove.IronTail, PBEItem.SteelGem)]
        [InlineData(PBEMove.WaterSpout, PBEItem.WaterGem)]
        public void Gem_Works(PBEMove move, PBEItem item)
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 1; // Seed ensures all moves do not miss
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Mew, 0, 1, move)
            {
                Item = item
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon mew = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];
            #endregion

            #region Use and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(mew, move, PBETurnTarget.FoeCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(!battle.VerifyMoveResultHappened(mew, magikarp, PBEResult.Missed) // Did not miss
                && battle.VerifyItemHappened(mew, mew, item, PBEItemAction.Consumed) // Gem consumed
                && mew.Item == PBEItem.None); // Properly removed
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Gem_Activates__FixedDamage__Bug(bool bugFix)
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            var settings = new PBESettings { BugFix = bugFix };
            settings.MakeReadOnly();

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Swellow, 0, 1, PBEMove.Endeavor)
            {
                Item = PBEItem.NormalGem
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon swellow = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];
            #endregion

            #region Use and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(swellow, PBEMove.Endeavor, PBETurnTarget.FoeCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            if (settings.BugFix)
            {
                Assert.True(!battle.VerifyItemHappened(swellow, swellow, PBEItem.NormalGem, PBEItemAction.Consumed)
                    && swellow.Item == PBEItem.NormalGem); // Not consumed
            }
            else
            {
                Assert.True(battle.VerifyItemHappened(swellow, swellow, PBEItem.NormalGem, PBEItemAction.Consumed)
                    && swellow.Item == PBEItem.None); // Buggy
            }
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Gem_Does_Not_Activate__Effectiveness()
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(2);
            p0[0] = new TestPokemon(settings, PBESpecies.Excadrill, 0, 100, PBEMove.Earthquake)
            {
                Item = PBEItem.GroundGem
            };
            p0[1] = new TestPokemon(settings, PBESpecies.Starly, 0, 1, PBEMove.Splash);

            var p1 = new TestPokemonCollection(2);
            p1[0] = new TestPokemon(settings, PBESpecies.Rotom, 0, 1, PBEMove.Splash)
            {
                Ability = PBEAbility.Levitate
            };
            p1[1] = new TestPokemon(settings, PBESpecies.Shedinja, 0, 1, PBEMove.Splash)
            {
                Ability = PBEAbility.WonderGuard
            };

            var battle = new PBEBattle(PBEBattleFormat.Double, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon excadrill = t0.Party[0];
            PBEBattlePokemon starly = t0.Party[1];
            PBEBattlePokemon rotom = t1.Party[0];
            PBEBattlePokemon shedinja = t1.Party[1];
            #endregion

            #region Use and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0,
                new PBETurnAction(excadrill, PBEMove.Earthquake, PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight),
                new PBETurnAction(starly, PBEMove.Splash, PBETurnTarget.AllyRight)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1,
                new PBETurnAction(rotom, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(shedinja, PBEMove.Splash, PBETurnTarget.AllyRight)));

            battle.RunTurn();

            Assert.True(battle.VerifyMoveResultHappened(excadrill, starly, PBEResult.Ineffective_Type) && starly.HPPercentage == 1 // Doesn't affect Flying
                && battle.VerifyMoveResultHappened(excadrill, rotom, PBEResult.Ineffective_Ability) && rotom.HPPercentage == 1 // Doesn't affect Levitate
                && battle.VerifyMoveResultHappened(excadrill, shedinja, PBEResult.Ineffective_Ability) && shedinja.HPPercentage == 1 // Doesn't affect WonderGuard
                && excadrill.Item == PBEItem.GroundGem); // Gem not consumed
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        // Failing is technically "effectiveness", but here's another test just because I can
        [Fact]
        public void Gem_Does_Not_Activate__Fail()
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Spiritomb, 0, 100, PBEMove.SuckerPunch)
            {
                Item = PBEItem.DarkGem
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon spiritomb = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];
            #endregion

            #region Use and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(spiritomb, PBEMove.SuckerPunch, PBETurnTarget.FoeCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(battle.VerifyMoveResultHappened(spiritomb, magikarp, PBEResult.InvalidConditions) && magikarp.HPPercentage == 1 // Fail
                && spiritomb.Item == PBEItem.DarkGem); // Gem not consumed
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Gem_Does_Not_Activate__Miss()
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 1; // Seed ensures all miss
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(2);
            p0[0] = new TestPokemon(settings, PBESpecies.Corsola, 0, 100, PBEMove.Earthquake)
            {
                Ability = PBEAbility.Hustle,
                Item = PBEItem.GroundGem
            };
            p0[1] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Splash);

            var p1 = new TestPokemonCollection(2);
            p1[0] = new TestPokemon(settings, PBESpecies.Happiny, 0, 1, PBEMove.Splash);
            p1[1] = new TestPokemon(settings, PBESpecies.Qwilfish, 0, 1, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Double, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon corsola = t0.Party[0];
            PBEBattlePokemon magikarp = t0.Party[1];
            PBEBattlePokemon happiny = t1.Party[0];
            PBEBattlePokemon qwilfish = t1.Party[1];
            corsola.AccuracyChange = (sbyte)-settings.MaxStatChange;
            magikarp.EvasionChange = settings.MaxStatChange;
            happiny.EvasionChange = settings.MaxStatChange;
            qwilfish.EvasionChange = settings.MaxStatChange;
            #endregion

            #region Use and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0,
                new PBETurnAction(corsola, PBEMove.Earthquake, PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight),
                new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyRight)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1,
                new PBETurnAction(happiny, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(qwilfish, PBEMove.Splash, PBETurnTarget.AllyRight)));

            battle.RunTurn();

            Assert.True(battle.VerifyMoveResultHappened(corsola, magikarp, PBEResult.Missed) && magikarp.HPPercentage == 1 // Miss everyone
                && battle.VerifyMoveResultHappened(corsola, happiny, PBEResult.Missed) && happiny.HPPercentage == 1
                && battle.VerifyMoveResultHappened(corsola, qwilfish, PBEResult.Missed) && qwilfish.HPPercentage == 1
                && corsola.Item == PBEItem.GroundGem); // Gem not consumed
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        // Protection is technically "missing", but here's another test just because I can
        [Fact]
        public void Gem_Does_Not_Activate__Protection()
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Excadrill, 0, 100, PBEMove.Earthquake)
            {
                Item = PBEItem.GroundGem
            };

            var p1 = new TestPokemonCollection(2);
            p1[0] = new TestPokemon(settings, PBESpecies.Happiny, 0, 1, PBEMove.WideGuard);
            p1[1] = new TestPokemon(settings, PBESpecies.Qwilfish, 0, 1, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Double, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon excadrill = t0.Party[0];
            PBEBattlePokemon happiny = t1.Party[0];
            PBEBattlePokemon qwilfish = t1.Party[1];
            #endregion

            #region Use and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0,
                new PBETurnAction(excadrill, PBEMove.Earthquake, PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1,
                new PBETurnAction(happiny, PBEMove.WideGuard, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight),
                new PBETurnAction(qwilfish, PBEMove.Splash, PBETurnTarget.AllyRight)));

            battle.RunTurn();

            Assert.True(battle.VerifyTeamStatusHappened(t1.Team, PBETeamStatus.WideGuard, PBETeamStatusAction.Damage, damageVictim: happiny) && happiny.HPPercentage == 1
                && battle.VerifyTeamStatusHappened(t1.Team, PBETeamStatus.WideGuard, PBETeamStatusAction.Damage, damageVictim: qwilfish) && qwilfish.HPPercentage == 1
                && excadrill.Item == PBEItem.GroundGem); // Gem not consumed
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
