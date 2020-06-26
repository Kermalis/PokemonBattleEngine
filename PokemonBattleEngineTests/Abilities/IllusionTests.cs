using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
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

        [Fact]
        public void Illusion_Does_Not_Copy_Same_Species()
        {
            #region Setup
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(PBESpecies.Magikarp, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };

            var p1 = new TestPokemonCollection(2);
            p1[0] = new TestPokemon(PBESpecies.Zoroark, 0, 100)
            {
                Ability = PBEAbility.Illusion,
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };
            p1[1] = p1[0];

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Team 1"), new PBETrainerInfo(p1, "Team 2"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon zoroark1 = t1.Party[0];
            #endregion

            #region Check
            Assert.True(zoroark1.DisguisedAsPokemon == null);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void Illusion_Copies_Shaymin_Reversion()
        {
            #region Setup
            PBERandom.SetSeed(0); // Seed ensures SecretPower freezes
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(PBESpecies.Happiny, 0, 100)
            {
                Ability = PBEAbility.SereneGrace,
                Moveset = new TestMoveset(settings, new[] { PBEMove.SecretPower, PBEMove.Splash })
            };

            var p1 = new TestPokemonCollection(3);
            p1[0] = new TestPokemon(PBESpecies.Shaymin, PBEForm.Shaymin_Sky, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };
            p1[1] = new TestPokemon(PBESpecies.Zoroark, 0, 100)
            {
                Ability = PBEAbility.Illusion,
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };
            p1[2] = new TestPokemon(PBESpecies.Magikarp, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Team 1"), new PBETrainerInfo(p1, "Team 2"), battleTerrain: PBEBattleTerrain.Snow);
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
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(happiny.Id, PBEMove.SecretPower, PBETurnTarget.FoeCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(shaymin.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));

            battle.RunTurn();

            Assert.True(shaymin.Status1 == PBEStatus1.Frozen && shaymin.Form == PBEForm.Shaymin);
            #endregion

            #region Swap Shaymin for Magikarp
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(happiny.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(shaymin.Id, magikarp.Id) }));

            battle.RunTurn();

            Assert.True(t1.Party[2] == shaymin);
            #endregion

            #region Swap Magikarp for Zoroark and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(happiny.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(magikarp.Id, zoroark.Id) }));

            battle.RunTurn();

            Assert.True(zoroark.DisguisedAsPokemon == shaymin && zoroark.KnownForm == PBEForm.Shaymin);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
