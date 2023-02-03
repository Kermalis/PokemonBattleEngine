using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests;

[Collection("Utils")]
public class ThrowTests
{
	public ThrowTests(TestUtils _, ITestOutputHelper output)
	{
		TestUtils.SetOutputHelper(output);
	}

	[Theory]
	[InlineData(PBEBattleFormat.Single, 1, false)]
	[InlineData(PBEBattleFormat.Single, 2, true)]
	[InlineData(PBEBattleFormat.Single, 3, true)]
	[InlineData(PBEBattleFormat.Single, 4, true)]
	[InlineData(PBEBattleFormat.Double, 1, false)]
	[InlineData(PBEBattleFormat.Double, 2, false)]
	[InlineData(PBEBattleFormat.Double, 3, true)]
	[InlineData(PBEBattleFormat.Double, 4, true)]
	[InlineData(PBEBattleFormat.Triple, 3, false)]
	[InlineData(PBEBattleFormat.Triple, 4, true)]
	public void Wild_Battle_Throws_For_Illegal_Party_Size(PBEBattleFormat format, int count, bool expectException)
	{
		#region Setup and check
		PBEDataProvider.GlobalRandom.Seed = 0;
		PBESettings settings = PBESettings.DefaultSettings;

		var p0 = new TestPokemonCollection(1);
		p0[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash);

		var p1 = new TestPokemonCollection(count);
		p1[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 100, PBEMove.Splash)
		{
			CaughtBall = PBEItem.None
		};
		for (int i = 1; i < count; i++)
		{
			p1[i] = p1[0];
		}

		if (expectException)
		{
			Assert.Throws<ArgumentException>(() => PBEBattle.CreateWildBattle(format, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBEWildInfo(p1)));
		}
		#endregion
	}
}
