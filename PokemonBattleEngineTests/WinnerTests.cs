using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests
{
    [Collection("Utils")]
    public class WinnerTests
    {
        public WinnerTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        // TODO: Who wins if you use PerishSong and everyone faints at the same time? Is it based on who's slowest?

        [Fact]
        public void Explosion_User_Loses_Single()
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Golem, 0, 100, PBEMove.Explosion);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon golem = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];
            #endregion

            #region Use move and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(golem, PBEMove.Explosion, PBETurnTarget.FoeCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(golem.HP == 0 && magikarp.HP == 0 // All faint
                && battle.Winner == battle.Teams[1]); // Golem's team loses
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Explosion_User_Loses_Multiple()
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(3);
            p0[0] = new TestPokemon(settings, PBESpecies.Qwilfish, 0, 1, PBEMove.Splash);
            p0[1] = new TestPokemon(settings, PBESpecies.Golem, 0, 100, PBEMove.Explosion);
            p0[2] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Splash);

            var p1 = new TestPokemonCollection(3);
            p1[0] = new TestPokemon(settings, PBESpecies.Patrat, 0, 1, PBEMove.Splash);
            p1[1] = new TestPokemon(settings, PBESpecies.Lickilicky, 0, 1, PBEMove.Splash);
            p1[2] = new TestPokemon(settings, PBESpecies.Happiny, 0, 1, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Triple, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon qwilfish = t0.Party[0];
            PBEBattlePokemon golem = t0.Party[1];
            PBEBattlePokemon magikarp = t0.Party[2];
            PBEBattlePokemon patrat = t1.Party[0];
            PBEBattlePokemon lickilicky = t1.Party[1];
            PBEBattlePokemon happiny = t1.Party[2];
            #endregion

            #region Use move and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0,
                new PBETurnAction(qwilfish, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(golem, PBEMove.Explosion, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight),
                new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyRight)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1,
                new PBETurnAction(patrat, PBEMove.Splash, PBETurnTarget.AllyLeft),
                new PBETurnAction(lickilicky, PBEMove.Splash, PBETurnTarget.AllyCenter),
                new PBETurnAction(happiny, PBEMove.Splash, PBETurnTarget.AllyRight)));

            battle.RunTurn();

            Assert.True(qwilfish.HP == 0 && golem.HP == 0 && magikarp.HP == 0 && patrat.HP == 0 && lickilicky.HP == 0 && happiny.HP == 0 // All faint
                && battle.Winner == battle.Teams[1]); // Golem's team loses
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void FinalGambit_User_Loses()
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Staraptor, 0, 100, PBEMove.FinalGambit);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon staraptor = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];
            #endregion

            #region Use FinalGambit and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(staraptor, PBEMove.FinalGambit, PBETurnTarget.FoeCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(staraptor.HP == 0 && magikarp.HP == 0 // Both fainted
                && battle.Winner == battle.Teams[1]); // Magikarp's team wins
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void HPDrain_And_LiquidOoze()
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Deoxys, PBEForm.Deoxys_Attack, 100, PBEMove.DrainPunch);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Blissey, 0, 100, PBEMove.Splash)
            {
                Ability = PBEAbility.LiquidOoze
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon deoxys = t0.Party[0];
            PBEBattlePokemon blissey = t1.Party[0];
            #endregion

            #region Use DrainPunch and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(deoxys, PBEMove.DrainPunch, PBETurnTarget.FoeCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(blissey, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(deoxys.HP == 0 && blissey.HP == 0 // Both fainted
                && battle.Winner == battle.Teams[1]); // Blissey's team wins
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        // IronBarbs/RockyHelmet/RoughSkin
        [Fact]
        public void IronBarbs_User_Loses()
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Lucario, 0, 100, PBEMove.Pound);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Ferroseed, 0, 1, PBEMove.Splash)
            {
                Ability = PBEAbility.IronBarbs
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon lucario = t0.Party[0];
            PBEBattlePokemon ferroseed = t1.Party[0];
            lucario.HP = 1;
            lucario.UpdateHPPercentage();
            #endregion

            #region Use Pound and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(lucario, PBEMove.Pound, PBETurnTarget.FoeCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(ferroseed, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(lucario.HP == 0 && ferroseed.HP == 0 // Both fainted
                && battle.Winner == battle.Teams[1]); // Ferroseed's team wins
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void LeechSeed_And_LiquidOoze()
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0; // Seed ensures LeechSeed doesn't miss
            var settings = new PBESettings { LeechSeedDenominator = 1 };
            settings.MakeReadOnly();

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Shroomish, 0, 1, PBEMove.LeechSeed);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Tentacruel, 0, 100, PBEMove.Splash)
            {
                Ability = PBEAbility.LiquidOoze
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon shroomish = t0.Party[0];
            PBEBattlePokemon tentacruel = t1.Party[0];
            #endregion

            #region Use LeechSeed and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(shroomish, PBEMove.LeechSeed, PBETurnTarget.FoeCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(tentacruel, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(shroomish.HP == 0 && tentacruel.HP == 0 // Both fainted
                && battle.Winner == battle.Teams[0]); // Shroomish's team wins
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void LifeOrb_User_Wins()
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Riolu, 0, 100, PBEMove.VacuumWave)
            {
                Item = PBEItem.LifeOrb
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon riolu = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];
            riolu.HP = 1;
            riolu.UpdateHPPercentage();
            #endregion

            #region Use HeadCharge and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(riolu, PBEMove.VacuumWave, PBETurnTarget.FoeCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(riolu.HP == 0 && magikarp.HP == 0 // Both fainted
                && battle.Winner == battle.Teams[0]); // Bouffalant's team wins
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Recoil_User_Wins()
        {
            #region Setup
            PBEUtils.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Bouffalant, 0, 100, PBEMove.HeadCharge);

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon bouffalant = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];
            bouffalant.HP = 1;
            bouffalant.UpdateHPPercentage();
            #endregion

            #region Use HeadCharge and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new PBETurnAction(bouffalant, PBEMove.HeadCharge, PBETurnTarget.FoeCenter)));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(bouffalant.HP == 0 && magikarp.HP == 0 // Both fainted
                && battle.Winner == battle.Teams[0]); // Bouffalant's team wins
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
