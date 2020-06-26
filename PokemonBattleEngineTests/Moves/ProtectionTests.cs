using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Moves
{
    [Collection("Utils")]
    public class ProtectionTests
    {
        public ProtectionTests(TestUtils utils, ITestOutputHelper output)
        {
            utils.SetOutputHelper(output);
        }

        // https://github.com/Kermalis/PokemonBattleEngine/issues/261
        [Theory]
        [InlineData(PBEMove.Detect)]
        [InlineData(PBEMove.Protect)]
        [InlineData(PBEMove.QuickGuard)]
        [InlineData(PBEMove.WideGuard)]
        public void Protection_Counter_Resets(PBEMove move)
        {
            #region Setup
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(PBESpecies.Mienshao, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { move, PBEMove.CalmMind })
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(PBESpecies.Magikarp, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Splash })
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Team 1"), new PBETrainerInfo(p1, "Team 2"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon mienshao = t0.Party[0];
            PBEBattlePokemon magikarp = t1.Party[0];
            #endregion

            #region Use move
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(mienshao.Id, move, PBETurnTarget.AllyCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(magikarp.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));

            battle.RunTurn();

            Assert.True(mienshao.Protection_Counter == 1);
            #endregion

            #region Use Calm Mind and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(mienshao.Id, move, PBETurnTarget.AllyCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(magikarp.Id, PBEMove.Splash, PBETurnTarget.AllyCenter) }));

            battle.RunTurn();

            Assert.True(mienshao.Protection_Counter == 0);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Feint_And_QuickGuard(bool ally)
        {
            #region Setup
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(2);
            p0[0] = new TestPokemon(PBESpecies.Lucario, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Feint })
            };
            p0[1] = new TestPokemon(PBESpecies.Mienshao, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.QuickGuard })
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(PBESpecies.MrMime, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.QuickGuard })
            };

            var battle = new PBEBattle(PBEBattleFormat.Double, settings, new PBETrainerInfo(p0, "Team 1"), new PBETrainerInfo(p1, "Team 2"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon lucario = t0.Party[0];
            PBEBattlePokemon mienshao = t0.Party[1];
            PBEBattlePokemon mrmime = t1.Party[0];
            #endregion

            #region Use move and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[]
            {
                new PBETurnAction(lucario.Id, PBEMove.Feint, ally ? PBETurnTarget.AllyRight : PBETurnTarget.FoeLeft),
                new PBETurnAction(mienshao.Id, PBEMove.QuickGuard, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight),
            }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(mrmime.Id, PBEMove.QuickGuard, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight) }));

            battle.RunTurn();

            Assert.True(TestUtils.VerifyTeamStatusHappened(battle, (ally ? t0 : t1).Team, PBETeamStatus.QuickGuard, PBETeamStatusAction.Damage, damageVictim: ally ? mienshao : mrmime) == ally);
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Fact]
        public void UserProtection_Works()
        {
            #region Setup
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(PBESpecies.Lucario, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Tackle })
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(PBESpecies.Mienshao, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { PBEMove.Protect })
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Team 1"), new PBETrainerInfo(p1, "Team 2"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon lucario = t0.Party[0];
            PBEBattlePokemon mienshao = t1.Party[0];
            #endregion

            #region Use move and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(lucario.Id, PBEMove.Tackle, PBETurnTarget.FoeCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(mienshao.Id, PBEMove.Protect, PBETurnTarget.AllyCenter) }));

            battle.RunTurn();

            Assert.True(TestUtils.VerifyStatus2Happened(battle, mienshao, lucario, PBEStatus2.Protected, PBEStatusAction.Damage) && !mienshao.Status2.HasFlag(PBEStatus2.Protected));
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }

        [Theory]
        [InlineData(PBEMove.QuickGuard, PBEMove.QuickAttack, PBETeamStatus.QuickGuard)]
        [InlineData(PBEMove.WideGuard, PBEMove.Earthquake, PBETeamStatus.WideGuard)]
        public void TeamProtection_Works(PBEMove move, PBEMove move2, PBETeamStatus teamStatus)
        {
            #region Setup
            PBERandom.SetSeed(0);
            PBESettings settings = PBESettings.DefaultSettings;

            var p0 = new TestPokemonCollection(1);
            p0[0] = new TestPokemon(PBESpecies.Lucario, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { move2 })
            };

            var p1 = new TestPokemonCollection(1);
            p1[0] = new TestPokemon(PBESpecies.Mienshao, 0, 100)
            {
                Moveset = new TestMoveset(settings, new[] { move })
            };

            var battle = new PBEBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Team 1"), new PBETrainerInfo(p1, "Team 2"));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBETrainer t0 = battle.Trainers[0];
            PBETrainer t1 = battle.Trainers[1];
            PBEBattlePokemon lucario = t0.Party[0];
            PBEBattlePokemon mienshao = t1.Party[0];
            #endregion

            #region Use move and check
            Assert.True(PBEBattle.SelectActionsIfValid(t0, new[] { new PBETurnAction(lucario.Id, move2, PBETurnTarget.FoeCenter) }));
            Assert.True(PBEBattle.SelectActionsIfValid(t1, new[] { new PBETurnAction(mienshao.Id, move, PBETurnTarget.AllyCenter) }));

            battle.RunTurn();

            Assert.True(TestUtils.VerifyTeamStatusHappened(battle, t1.Team, teamStatus, PBETeamStatusAction.Damage, damageVictim: mienshao) && !t1.Team.TeamStatus.HasFlag(teamStatus));
            #endregion

            #region Cleanup
            battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            #endregion
        }
    }
}
