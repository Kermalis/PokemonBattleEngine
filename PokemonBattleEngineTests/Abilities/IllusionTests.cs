using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests.Abilities;

[Collection("Utils")]
public class IllusionTests
{
	public IllusionTests(TestUtils _, ITestOutputHelper output)
	{
		TestUtils.SetOutputHelper(output);
	}

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

		var battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
		battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

		PBETrainer t1 = battle.Trainers[1];
		PBEBattlePokemon zoroark1 = t1.Party[0];

		battle.Begin();
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

		var battle = PBEBattle.CreateWildBattle(PBEBattleFormat.Double, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBEWildInfo(p1));
		battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

		PBETrainer t1 = battle.Trainers[1];
		PBEBattlePokemon zoroark = t1.Party[0];

		battle.Begin();
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

		var battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Double, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false));
		battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

		PBETrainer t1 = battle.Trainers[1];
		PBEBattlePokemon zoroark = t1.Party[0];

		battle.Begin();
		#endregion

		#region Check
		Assert.True(zoroark.Status2.HasFlag(PBEStatus2.Disguised));
		#endregion

		#region Cleanup
		battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
		#endregion
	}

	[Fact]
	public void Illusion_Does_Not_Copy_Just_Swapped_Mon()
	{
		#region Setup
		PBEDataProvider.GlobalRandom.Seed = 0;
		PBESettings settings = PBESettings.DefaultSettings;

		var p0 = new TestPokemonCollection(1);
		p0[0] = new TestPokemon(settings, PBESpecies.Happiny, 0, 1, PBEMove.Splash);

		var p1 = new TestPokemonCollection(3);
		p1[0] = new TestPokemon(settings, PBESpecies.Feebas, 0, 1, PBEMove.Splash);
		p1[1] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Splash);
		p1[2] = new TestPokemon(settings, PBESpecies.Zoroark, 0, 100, PBEMove.Splash)
		{
			Ability = PBEAbility.Illusion
		};

		var battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false),
				battleTerrain: PBEBattleTerrain.Snow);
		battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

		PBETrainer t0 = battle.Trainers[0];
		PBETrainer t1 = battle.Trainers[1];
		PBEBattlePokemon happiny = t0.Party[0];
		PBEBattlePokemon feebas = t1.Party[0];
		PBEBattlePokemon zoroark = t1.Party[2];

		battle.Begin();
		#endregion

		#region Swap Feebas for Zoroark and check
		Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(happiny, PBEMove.Splash, PBETurnTarget.AllyCenter)));
		Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(feebas, zoroark)));

		battle.RunTurn();

		Assert.True(zoroark.KnownSpecies == PBESpecies.Zoroark && zoroark.KnownForm == 0);
		#endregion

		#region Cleanup
		battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
		#endregion
	}

	[Fact]
	public void Illusion_Copies_Just_Swapped_Mon()
	{
		#region Setup
		PBEDataProvider.GlobalRandom.Seed = 0;
		PBESettings settings = PBESettings.DefaultSettings;

		var p0 = new TestPokemonCollection(1);
		p0[0] = new TestPokemon(settings, PBESpecies.Happiny, 0, 1, PBEMove.Splash);

		var p1 = new TestPokemonCollection(4);
		p1[0] = new TestPokemon(settings, PBESpecies.Trubbish, 0, 10, PBEMove.Splash); // Trubbish needs more speed to swap first
		p1[1] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Splash);
		p1[2] = new TestPokemon(settings, PBESpecies.Zoroark, 0, 100, PBEMove.Splash)
		{
			Ability = PBEAbility.Illusion
		};
		p1[3] = new TestPokemon(settings, PBESpecies.Feebas, 0, 1, PBEMove.Splash);

		var battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Double, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false),
				battleTerrain: PBEBattleTerrain.Snow);
		battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

		PBETrainer t0 = battle.Trainers[0];
		PBETrainer t1 = battle.Trainers[1];
		PBEBattlePokemon happiny = t0.Party[0];
		PBEBattlePokemon trubbish = t1.Party[0];
		PBEBattlePokemon magikarp = t1.Party[1];
		PBEBattlePokemon zoroark = t1.Party[2];
		PBEBattlePokemon feebas = t1.Party[3];

		battle.Begin();
		#endregion

		#region Swap Trubbish and Magikarp for Feebas and Zoroark then check
		Assert.True(t0.SelectActionsIfValid(out _,
			new PBETurnAction(happiny, PBEMove.Splash, PBETurnTarget.AllyLeft)));
		Assert.True(t1.SelectActionsIfValid(out _,
			new PBETurnAction(trubbish, feebas),
			new PBETurnAction(magikarp, zoroark)));

		battle.RunTurn();

		Assert.True(zoroark.KnownSpecies == trubbish.Species && zoroark.KnownForm == trubbish.Form);
		#endregion

		#region Cleanup
		battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
		#endregion
	}

	[Fact]
	public void Illusion_Copies_Shaymin_Reversion()
	{
		#region Setup
		PBEDataProvider.GlobalRandom.Seed = 1; // Seed ensures SecretPower freezes
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

		var battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false),
				battleTerrain: PBEBattleTerrain.Snow);
		battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

		PBETrainer t0 = battle.Trainers[0];
		PBETrainer t1 = battle.Trainers[1];
		PBEBattlePokemon happiny = t0.Party[0];
		PBEBattlePokemon shaymin = t1.Party[0];
		PBEBattlePokemon zoroark = t1.Party[1];
		PBEBattlePokemon magikarp = t1.Party[2];

		battle.Begin();
		#endregion

		#region Freeze Shaymin
		Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(happiny, PBEMove.SecretPower, PBETurnTarget.FoeCenter)));
		Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(shaymin, PBEMove.Splash, PBETurnTarget.AllyCenter)));

		battle.RunTurn();

		Assert.True(shaymin.Status1 == PBEStatus1.Frozen && shaymin.Form == PBEForm.Shaymin);
		#endregion

		#region Swap Shaymin for Magikarp
		Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(happiny, PBEMove.Splash, PBETurnTarget.AllyCenter)));
		Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(shaymin, magikarp)));

		battle.RunTurn();

		Assert.True(t1.Party[2] == shaymin);
		#endregion

		#region Swap Magikarp for Zoroark and check
		Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(happiny, PBEMove.Splash, PBETurnTarget.AllyCenter)));
		Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(magikarp, zoroark)));

		battle.RunTurn();

		Assert.True(zoroark.KnownSpecies == PBESpecies.Shaymin && zoroark.KnownForm == PBEForm.Shaymin);
		#endregion

		#region Cleanup
		battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
		#endregion
	}

	[Fact]
	public void Illusion_Works_Wild()
	{
		#region Setup
		PBEDataProvider.GlobalRandom.Seed = 0;
		PBESettings settings = PBESettings.DefaultSettings;

		var p0 = new TestPokemonCollection(1);
		p0[0] = new TestPokemon(settings, PBESpecies.Magikarp, 0, 1, PBEMove.Tackle);

		var p1 = new TestPokemonCollection(1);
		p1[0] = new TestPokemon(settings, PBESpecies.Zoroark, 0, 100, PBEMove.Splash)
		{
			Ability = PBEAbility.Illusion,
			CaughtBall = PBEItem.None
		};

		var battle = PBEBattle.CreateWildBattle(PBEBattleFormat.Single, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBEWildInfo(p1));
		battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;

		PBETrainer t0 = battle.Trainers[0];
		PBETrainer t1 = battle.Trainers[1];
		PBEBattlePokemon magikarp = t0.Party[0];
		PBEBattlePokemon zoroark = t1.Party[0];

		zoroark.Status2 |= PBEStatus2.Disguised;
		zoroark.KnownGender = PBEGender.Genderless;
		zoroark.KnownCaughtBall = PBEItem.None;
		zoroark.KnownShiny = false;
		zoroark.KnownSpecies = PBESpecies.Entei;
		zoroark.KnownForm = 0;
		zoroark.KnownNickname = zoroark.KnownSpecies.ToString();
		IPBEPokemonData pData = PBEDataProvider.Instance.GetPokemonData(zoroark.KnownSpecies, zoroark.KnownForm);
		zoroark.KnownType1 = pData.Type1;
		zoroark.KnownType2 = pData.Type2;

		battle.Begin();
		#endregion

		#region Check that the disguise works
		Assert.True(zoroark.Status2.HasFlag(PBEStatus2.Disguised)
			&& ((PBEWildPkmnAppearedPacket)battle.Events.Single(p => p is PBEWildPkmnAppearedPacket)).Pokemon[0].IsDisguised);
		#endregion

		#region Break the disguise and check
		Assert.True(t0.SelectActionsIfValid(out _, new PBETurnAction(magikarp, PBEMove.Tackle, PBETurnTarget.FoeCenter)));
		Assert.True(t1.SelectActionsIfValid(out _, new PBETurnAction(zoroark, PBEMove.Splash, PBETurnTarget.AllyCenter)));

		battle.RunTurn();

		Assert.True(!zoroark.Status2.HasFlag(PBEStatus2.Disguised)
			&& zoroark.KnownSpecies == PBESpecies.Zoroark);
		#endregion

		#region Cleanup
		battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
		#endregion
	}
}
