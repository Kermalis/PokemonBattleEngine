using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Moves;

[Collection("Utils")]
public class ProtectionTests
{
	public ProtectionTests(TestUtils _, ITestOutputHelper output)
	{
		TestUtils.SetOutputHelper(output);
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
		PBEDataProvider.GlobalRandom.Seed = 0;
		PBESettings settings = PBESettings.DefaultSettings;

		var p0 = new TestPokemonCollection(1);
		p0[0] = new TestPokemon(settings, PBESpecies.Mienshao, 0, 100, move, PBEMove.CalmMind);

		var p1 = new TestPokemonCollection(1);
		p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

		var battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
		battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

		PBETrainer t0 = battle.Trainers[0];
		PBETrainer t1 = battle.Trainers[1];
		PBEBattlePokemon mienshao = t0.Party[0];
		PBEBattlePokemon magikarp = t1.Party[0];

		battle.Begin();
		#endregion

		#region Use move
		Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(mienshao, move, PBETurnTarget.AllyCenter)));
		Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

		battle.RunTurn();

		Assert.True(mienshao.Protection_Counter == 1);
		#endregion

		#region Use Calm Mind and check
		Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(mienshao, PBEMove.CalmMind, PBETurnTarget.AllyCenter)));
		Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

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
		PBEDataProvider.GlobalRandom.Seed = 0;
		PBESettings settings = PBESettings.DefaultSettings;

		var p0 = new TestPokemonCollection(2);
		p0[0] = new TestPokemon(settings, PBESpecies.Lucario, 0, 100, PBEMove.Feint);
		p0[1] = new TestPokemon(settings, PBESpecies.Mienshao, 0, 100, PBEMove.QuickGuard);

		var p1 = new TestPokemonCollection(1);
		p1[0] = new TestPokemon(settings, PBESpecies.MrMime, 0, 100, PBEMove.QuickGuard);

		var battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Double, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
		battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

		PBETrainer t0 = battle.Trainers[0];
		PBETrainer t1 = battle.Trainers[1];
		PBEBattlePokemon lucario = t0.Party[0];
		PBEBattlePokemon mienshao = t0.Party[1];
		PBEBattlePokemon mrmime = t1.Party[0];

		battle.Begin();
		#endregion

		#region Use move and check
		Assert.True(t0.SelectActionsIfValid(out _,
			new PBETurnAction(lucario, PBEMove.Feint, ally ? PBETurnTarget.AllyRight : PBETurnTarget.FoeLeft),
			new PBETurnAction(mienshao, PBEMove.QuickGuard, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight)));
		Assert.True(t1.SelectActionsIfValid(out _,
			new PBETurnAction(mrmime, PBEMove.QuickGuard, PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight)));

		battle.RunTurn();

		Assert.True(battle.VerifyTeamStatusDamageHappened((ally ? t0 : t1).Team, PBETeamStatus.QuickGuard, ally ? mienshao : mrmime) == ally);
		#endregion

		#region Cleanup
		battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
		#endregion
	}

	[Fact]
	public void UserProtection_Works()
	{
		#region Setup
		PBEDataProvider.GlobalRandom.Seed = 0;
		PBESettings settings = PBESettings.DefaultSettings;

		var p0 = new TestPokemonCollection(1);
		p0[0] = new TestPokemon(settings, PBESpecies.Lucario, 0, 100, PBEMove.Tackle);

		var p1 = new TestPokemonCollection(1);
		p1[0] = new TestPokemon(settings, PBESpecies.Mienshao, 0, 100, PBEMove.Protect);

		var battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
		battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

		PBETrainer t0 = battle.Trainers[0];
		PBETrainer t1 = battle.Trainers[1];
		PBEBattlePokemon lucario = t0.Party[0];
		PBEBattlePokemon mienshao = t1.Party[0];

		battle.Begin();
		#endregion

		#region Use move and check
		Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(lucario, PBEMove.Tackle, PBETurnTarget.FoeCenter)));
		Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(mienshao, PBEMove.Protect, PBETurnTarget.AllyCenter)));

		battle.RunTurn();

		Assert.True(battle.VerifyStatus2Happened(mienshao, lucario, PBEStatus2.Protected, PBEStatusAction.Damage) && !mienshao.Status2.HasFlag(PBEStatus2.Protected));
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
		PBEDataProvider.GlobalRandom.Seed = 0;
		PBESettings settings = PBESettings.DefaultSettings;

		var p0 = new TestPokemonCollection(1);
		p0[0] = new TestPokemon(settings, PBESpecies.Mienshao, 0, 100, move);

		var p1 = new TestPokemonCollection(1);
		p1[0] = new TestPokemon(settings, PBESpecies.Lucario, 0, 100, move2);

		var battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
		battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

		PBETrainer t0 = battle.Trainers[0];
		PBETrainer t1 = battle.Trainers[1];
		PBEBattlePokemon mienshao = t0.Party[0];
		PBEBattlePokemon lucario = t1.Party[0];

		battle.Begin();
		#endregion

		#region Use move and check
		Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(mienshao, move, PBETurnTarget.AllyCenter)));
		Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(lucario, move2, PBETurnTarget.FoeCenter)));

		battle.RunTurn();

		Assert.True(battle.VerifyTeamStatusDamageHappened(t0.Team, teamStatus, mienshao)
			&& !t0.Team.TeamStatus.HasFlag(teamStatus));
		#endregion

		#region Cleanup
		battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
		#endregion
	}
}
