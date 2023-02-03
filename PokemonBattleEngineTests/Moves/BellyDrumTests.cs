using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Moves;

[Collection("Utils")]
public class BellyDrumTests
{
	public BellyDrumTests(TestUtils _, ITestOutputHelper output)
	{
		TestUtils.SetOutputHelper(output);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void BellyDrum_Contrary__Bug(bool bugFix)
	{
		#region Setup
		PBEDataProvider.GlobalRandom.Seed = 0;
		var settings = new PBESettings { BugFix = bugFix };
		settings.MakeReadOnly();

		var p0 = new TestPokemonCollection(1);
		p0[0] = new TestPokemon(settings, PBESpecies.Hariyama, 0, 100, PBEMove.BellyDrum)
		{
			Ability = PBEAbility.Contrary
		};

		var p1 = new TestPokemonCollection(1);
		p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

		var battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
		battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

		PBETrainer t0 = battle.Trainers[0];
		PBETrainer t1 = battle.Trainers[1];
		PBEBattlePokemon hariyama = t0.Party[0];
		PBEBattlePokemon magikarp = t1.Party[0];
		hariyama.AttackChange = settings.MaxStatChange;

		battle.Begin();
		#endregion

		#region Use and check
		Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(hariyama, PBEMove.BellyDrum, PBETurnTarget.AllyCenter)));
		Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(magikarp, PBEMove.Splash, PBETurnTarget.AllyCenter)));

		battle.RunTurn();

		if (settings.BugFix)
		{
			Assert.True(!battle.VerifyMoveResultHappened(hariyama, hariyama, PBEResult.InvalidConditions)
				&& hariyama.AttackChange == -settings.MaxStatChange); // Stat minimized because of Contrary
		}
		else
		{
			Assert.True(battle.VerifyMoveResultHappened(hariyama, hariyama, PBEResult.InvalidConditions)
				&& hariyama.AttackChange == settings.MaxStatChange); // Buggy
		}
		#endregion

		#region Cleanup
		battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
		#endregion
	}
}
