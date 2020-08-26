using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Abilities
{
    [Collection("Utils")]
    public class IllusionTests
    {
        public IllusionTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        // TODO: Wild Pkmn can be disguised (and shows in wild packet)

        [Fact]
        public void Illusion_Does_Not_Copy_Same_Species()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var p1 = new TestPokemonCollection(2);
            p1[0] = p1[1] = new TestPokemon(settings, PBESpecies.Zoroark, 0, 100, PBEMove.Splash)
            {
                Ability = PBEAbility.Illusion
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon zoroark1 = t1.Party[0];
            #endregion

            #region Check
            Assert.False(zoroark1.Status2.HasFlag(PBEStatus2.Disguised));
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Illusion_Does_Not_Copy_Active_Wild_Teammate()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var p1 = new TestPokemonCollection(2);
            p1[0] = new TestPokemon(settings, PBESpecies.Zoroark, 0, 100, PBEMove.Splash)
            {
                Ability = PBEAbility.Illusion,
                CaughtBall = PBEItem.None
            };
            p1[1] = p0[0];

            var battle = new PBEBattle(PBEBattleFormat.Double, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBEWildInfo(p1));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon zoroark = t1.Party[0];
            #endregion

            #region Check
            Assert.False(zoroark.Status2.HasFlag(PBEStatus2.Disguised));
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Illusion_Does_Copy_Active_Trainer_Teammate()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0;
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var p1 = new TestPokemonCollection(2);
            p1[0] = new TestPokemon(settings, PBESpecies.Zoroark, 0, 100, PBEMove.Splash)
            {
                Ability = PBEAbility.Illusion
            };
            p1[1] = p0[0];

            var battle = new PBEBattle(PBEBattleFormat.Double, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon zoroark = t1.Party[0];
            #endregion

            #region Check
            Assert.True(zoroark.Status2.HasFlag(PBEStatus2.Disguised));
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Illusion_Copies_Shaymin_Reversion()
        {
            #region Setup
            PBEDataProvider.GlobalRandom.Seed = 0; // Seed ensures SecretPower freezes
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(settings, PBESpecies.Happiny, 0, 100, PBEMove.SecretPower, PBEMove.Splash)
            {
                Ability = PBEAbility.SereneGrace
            };

            var p1 = new TestPokemonCollection(3);
            p1[0] = new TestPokemon(settings, PBESpecies.Shaymin, PBEForm.Shaymin_Sky, 100, PBEMove.Splash);
            p1[1] = new TestPokemon(settings, PBESpecies.Zoroark, 0, 100, PBEMove.Splash)
            {
                Ability = PBEAbility.Illusion
            };
            p1[2] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0"), new PBETrainerInfo(p1, "Trainer 1"),
                battleTerrain: PBEBattleTerrain.Snow);
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon happiny = t0.Party[0];
            PBEBattlePokemon shaymin = t1.Party[0];
            PBEBattlePokemon zoroark = t1.Party[1];
            PBEBattlePokemon magikarp = t1.Party[2];
            #endregion

            #region Freeze Shaymin
            Assert.Null(t0.SelectActionsIfValid(new PBETurnAction(happiny, PBEMove.SecretPower, PBETurnTarget.FoeCenter)));
            Assert.Null(t1.SelectActionsIfValid(new PBETurnAction(shaymin, PBEMove.Splash, PBETurnTarget.AllyCenter)));

            battle.RunTurn();

            Assert.True(shaymin.Status1 == PBEStatus1.Frozen && shaymin.Form == PBEForm.Shaymin);
            #endregion

            #region Swap Shaymin for Magikarp
            Assert.Null(t0.SelectActionsIfValid(new PBETurnAction(happiny, PBEMove.Splash, PBETurnTarget.AllyCenter)));
            Assert.Null(t1.SelectActionsIfValid(new PBETurnAction(shaymin, magikarp)));

            battle.RunTurn();

            Assert.True(t1.Party[2] == shaymin);
            #endregion

            #region Swap Magikarp for Zoroark and check
            Assert.Null(t0.SelectActionsIfValid(new PBETurnAction(happiny, PBEMove.Splash, PBETurnTarget.AllyCenter)));
            Assert.Null(t1.SelectActionsIfValid(new PBETurnAction(magikarp, zoroark)));

            battle.RunTurn();

            Assert.True(zoroark.KnownSpecies == PBESpecies.Shaymin && zoroark.KnownForm == PBEForm.Shaymin);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
