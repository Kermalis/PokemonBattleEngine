using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kermalis.PokemonBattleEngine.DefaultData.Data;

public sealed partial class PBEDDEventPokemon
{
	public static ReadOnlyDictionary<PBESpecies, ReadOnlyCollection<PBEDDEventPokemon>> Events { get; } = new(new Dictionary<PBESpecies, ReadOnlyCollection<PBEDDEventPokemon>>
	{
		{
			PBESpecies.Absol,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // 5th anniversary egg
				(
					new byte[] { 3 }, PBESpecies.Absol, 5, null, null, new PBEAbility[] { PBEAbility.Pressure }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Scratch, PBEMove.Leer, PBEMove.Spite, PBEMove.None }
				),
				new PBEDDEventPokemon // 5th anniversary egg | Pokémon Stamp RS magazine raffle
				(
					new byte[] { 3 }, PBESpecies.Absol, 5, null, null, new PBEAbility[] { PBEAbility.Pressure }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Scratch, PBEMove.Leer, PBEMove.Wish, PBEMove.None }
				),
				new PBEDDEventPokemon // Pokémon Box promotion
				(
					new byte[] { 3 }, PBESpecies.Absol, 35, false, null, new PBEAbility[] { PBEAbility.Pressure }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.RazorWind, PBEMove.Bite, PBEMove.SwordsDance, PBEMove.Spite }
				),
				new PBEDDEventPokemon // Pokémon Box promotion
				(
					new byte[] { 3 }, PBESpecies.Absol, 35, false, null, new PBEAbility[] { PBEAbility.Pressure }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.RazorWind, PBEMove.Bite, PBEMove.SwordsDance, PBEMove.Wish }
				),
				new PBEDDEventPokemon // Journey across America | Party of the decade
				(
					new byte[] { 3 }, PBESpecies.Absol, 70, false, null, new PBEAbility[] { PBEAbility.Pressure }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.DoubleTeam, PBEMove.Slash, PBEMove.FutureSight, PBEMove.PerishSong }
				)
			})
		},
		{
			PBESpecies.Arceus,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Jewel of Life promotion
				(
					new byte[] { 4 }, PBESpecies.Arceus, 100, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.Multitype }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Judgment, PBEMove.RoarOfTime, PBEMove.SpacialRend, PBEMove.ShadowForce }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Arceus, 100, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.Multitype }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Recover, PBEMove.HyperBeam, PBEMove.PerishSong, PBEMove.Judgment }
				)
			})
		},
		{
			PBESpecies.Audino,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // 2010 birthday
				(
					new byte[] { 5 }, PBESpecies.Audino, 30, false, PBEGender.Female, new PBEAbility[] { PBEAbility.Healer }, new PBENature[] { PBENature.Calm },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.HealPulse, PBEMove.HelpingHand, PBEMove.Refresh, PBEMove.DoubleSlap }
				),
				new PBEDDEventPokemon // 2011 birthday | 2012 birthday
				(
					new byte[] { 5 }, PBESpecies.Audino, 30, false, PBEGender.Female, new PBEAbility[] { PBEAbility.Healer }, new PBENature[] { PBENature.Jolly, PBENature.Serious },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.HealPulse, PBEMove.HelpingHand, PBEMove.Refresh, PBEMove.Present }
				)
			})
		},
		{
			PBESpecies.Axew,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Iris's Axew
				(
					new byte[] { 5 }, PBESpecies.Axew, 1, null, PBEGender.Male, new PBEAbility[] { PBEAbility.MoldBreaker }, new PBENature[] { PBENature.Naive },
					new byte?[] { null, null, null, null, null, 31 }, new PBEMove[] { PBEMove.Scratch, PBEMove.DragonRage, PBEMove.None, PBEMove.None }
				),
				new PBEDDEventPokemon // Pokémon Searcher BW promotion
				(
					new byte[] { 5 }, PBESpecies.Axew, 10, false, null, new PBEAbility[] { PBEAbility.MoldBreaker }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.DragonRage, PBEMove.Return, PBEMove.Endure, PBEMove.DragonClaw }
				),
				new PBEDDEventPokemon // Best Wishes Iris's Axew
				(
					new byte[] { 5 }, PBESpecies.Axew, 30, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Rivalry }, new PBENature[] { PBENature.Naive },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.DragonRage, PBEMove.Scratch, PBEMove.Outrage, PBEMove.GigaImpact }
				)
			})
		},
		{
			PBESpecies.Azurill,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // PCNY
				(
					new byte[] { 3 }, PBESpecies.Azurill, 5, false, null, new PBEAbility[] { PBEAbility.HugePower, PBEAbility.ThickFat }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Splash, PBEMove.Charm, PBEMove.None, PBEMove.None }
				)
			})
		},
		{
			PBESpecies.Blastoise,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Journey across America | Party of the decade
				(
					new byte[] { 3 }, PBESpecies.Blastoise, 70, false, null, new PBEAbility[] { PBEAbility.Torrent }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Protect, PBEMove.RainDance, PBEMove.SkullBash, PBEMove.HydroPump }
				)
			})
		},
		{
			PBESpecies.Blaziken,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Journey across America | Party of the decade
				(
					new byte[] { 3 }, PBESpecies.Blaziken, 70, false, null, new PBEAbility[] { PBEAbility.Blaze }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.BlazeKick, PBEMove.Slash, PBEMove.MirrorMove, PBEMove.SkyUppercut }
				),
				new PBEDDEventPokemon // Train station
				(
					new byte[] { 5 }, PBESpecies.Blaziken, 50, null, null, new PBEAbility[] { PBEAbility.Blaze }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.FlareBlitz, PBEMove.HiJumpKick, PBEMove.ThunderPunch, PBEMove.StoneEdge }
				)
			})
		},
		{
			PBESpecies.Bulbasaur,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Gather more Pokémon
				(
					new byte[] { 3 }, PBESpecies.Bulbasaur, 10, false, null, new PBEAbility[] { PBEAbility.Overgrow }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Tackle, PBEMove.Growl, PBEMove.LeechSeed, PBEMove.VineWhip }
				),
				new PBEDDEventPokemon // Journey across America
				(
					new byte[] { 3 }, PBESpecies.Bulbasaur, 70, false, null, new PBEAbility[] { PBEAbility.Overgrow }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.SweetScent, PBEMove.Growth, PBEMove.Synthesis, PBEMove.SolarBeam }
				),
				new PBEDDEventPokemon // Kanto starter egg
				(
					new byte[] { 5 }, PBESpecies.Bulbasaur, 1, null, null, new PBEAbility[] { PBEAbility.Overgrow }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, 31, null, null, null }, new PBEMove[] { PBEMove.FalseSwipe, PBEMove.Block, PBEMove.FrenzyPlant, PBEMove.WeatherBall }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Bulbasaur, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Chlorophyll }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Tackle, PBEMove.Growl, PBEMove.LeechSeed, PBEMove.VineWhip }
				)
			})
		},
		{
			PBESpecies.Chandelure,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Powerful Tag
				(
					new byte[] { 5 }, PBESpecies.Chandelure, 50, false, PBEGender.Female, new PBEAbility[] { PBEAbility.FlashFire }, new PBENature[] { PBENature.Modest },
					new byte?[] { null, null, null, 31, null, null }, new PBEMove[] { PBEMove.HeatWave, PBEMove.ShadowBall, PBEMove.EnergyBall, PBEMove.Psychic }
				)
			})
		},
		{
			PBESpecies.Chansey,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // PCNY
				(
					new byte[] { 3 }, PBESpecies.Chansey, 5, null, PBEGender.Female, new PBEAbility[] { PBEAbility.NaturalCure, PBEAbility.SereneGrace }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.SweetScent, PBEMove.Wish, PBEMove.None, PBEMove.None }
				),
				new PBEDDEventPokemon // Gather more Pokémon
				(
					new byte[] { 3 }, PBESpecies.Chansey, 10, false, PBEGender.Female, new PBEAbility[] { PBEAbility.NaturalCure, PBEAbility.SereneGrace }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Pound, PBEMove.Growl, PBEMove.TailWhip, PBEMove.Refresh }
				)
			})
		},
		{
			PBESpecies.Charizard,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Journey across America | Top 10 distribution | Party of the decade
				(
					new byte[] { 3 }, PBESpecies.Charizard, 70, false, null, new PBEAbility[] { PBEAbility.Blaze }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.WingAttack, PBEMove.Slash, PBEMove.DragonRage, PBEMove.FireSpin }
				)
			})
		},
		{
			PBESpecies.Charmander,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Gather more Pokémon
				(
					new byte[] { 3 }, PBESpecies.Charmander, 10, false, null, new PBEAbility[] { PBEAbility.Blaze }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Scratch, PBEMove.Growl, PBEMove.Ember, PBEMove.None }
				),
				new PBEDDEventPokemon // 2007 birthday | 2008 birthday | 2009 birthday | 2010 birthday
				(
					new byte[] { 4 }, PBESpecies.Charmander, 40, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Blaze }, new PBENature[] { PBENature.Hardy, PBENature.Mild, PBENature.Naive, PBENature.Naughty },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Return, PBEMove.HiddenPower, PBEMove.QuickAttack, PBEMove.Howl }
				),
				new PBEDDEventPokemon // Kanto starter eggs
				(
					new byte[] { 5 }, PBESpecies.Charmander, 1, null, null, new PBEAbility[] { PBEAbility.Blaze }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, 31 }, new PBEMove[] { PBEMove.FalseSwipe, PBEMove.Block, PBEMove.BlastBurn, PBEMove.Acrobatics }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Charmander, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.SolarPower }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Scratch, PBEMove.Growl, PBEMove.Ember, PBEMove.SmokeScreen }
				)
			})
		},
		{
			PBESpecies.Chimchar,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // 2009 birthday | 2010 birthday
				(
					new byte[] { 4 }, PBESpecies.Chimchar, 40, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Blaze }, new PBENature[] { PBENature.Hardy, PBENature.Mild },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Flamethrower, PBEMove.ThunderPunch, PBEMove.GrassKnot, PBEMove.HelpingHand }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Chimchar, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.IronFist }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Scratch, PBEMove.Leer, PBEMove.Ember, PBEMove.Taunt }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Chimchar, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.IronFist }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Leer, PBEMove.Ember, PBEMove.Taunt, PBEMove.FakeOut }
				)
			})
		},
		{
			PBESpecies.Cradily,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // PCNY
				(
					new byte[] { 3 }, PBESpecies.Cradily, 40, false, null, new PBEAbility[] { PBEAbility.SuctionCups }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Acid, PBEMove.Ingrain, PBEMove.ConfuseRay, PBEMove.Amnesia }
				)
			})
		},
		{
			PBESpecies.Cresselia,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // World championships 2013
				(
					new byte[] { 5 }, PBESpecies.Cresselia, 68, false, PBEGender.Female, new PBEAbility[] { PBEAbility.Levitate }, new PBENature[] { PBENature.Modest },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.IceBeam, PBEMove.Psyshock, PBEMove.EnergyBall, PBEMove.HiddenPower }
				)
			})
		},
		{
			PBESpecies.Crobat,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // World championships 2010
				(
					new byte[] { 4 }, PBESpecies.Crobat, 30, false, PBEGender.Male, new PBEAbility[] { PBEAbility.InnerFocus }, new PBENature[] { PBENature.Timid },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.HeatWave, PBEMove.AirSlash, PBEMove.SludgeBomb, PBEMove.SuperFang }
				)
			})
		},
		{
			PBESpecies.Cubchoo,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Pokémon Smash! Cubchoo
				(
					new byte[] { 5 }, PBESpecies.Cubchoo, 15, false, null, new PBEAbility[] { PBEAbility.SnowCloak }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.PowderSnow, PBEMove.Growl, PBEMove.Bide, PBEMove.IcyWind }
				)
			})
		},
		{
			PBESpecies.Darkrai,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // The Rise of Darkrai promotion
				(
					new byte[] { 4 }, PBESpecies.Darkrai, 50, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.BadDreams }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.RoarOfTime, PBEMove.SpacialRend, PBEMove.Nightmare, PBEMove.Hypnosis }
				),
				new PBEDDEventPokemon // Almia Darkrai
				(
					new byte[] { 4 }, PBESpecies.Darkrai, 50, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.BadDreams }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.DarkVoid, PBEMove.DarkPulse, PBEMove.ShadowBall, PBEMove.DoubleTeam }
				),
				new PBEDDEventPokemon // Winter 2010 | Victini movie promotions | Winter 2011 | May 2012
				(
					new byte[] { 5 }, PBESpecies.Darkrai, 50, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.BadDreams }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.DarkVoid, PBEMove.OminousWind, PBEMove.FaintAttack, PBEMove.Nightmare }
				)
			})
		},
		{
			PBESpecies.Deino,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Year of the dragon
				(
					new byte[] { 5 }, PBESpecies.Deino, 1, true, null, new PBEAbility[] { PBEAbility.Hustle }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Tackle, PBEMove.DragonRage, PBEMove.None, PBEMove.None }
				)
			})
		},
		{
			PBESpecies.Dialga,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // 2013 shiny creation trio
				(
					new byte[] { 5 }, PBESpecies.Dialga, 100, true, PBEGender.Genderless, new PBEAbility[] { PBEAbility.Pressure }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.DragonPulse, PBEMove.DracoMeteor, PBEMove.RoarOfTime, PBEMove.AuraSphere }
				)
			})
		},
		{
			PBESpecies.Druddigon,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Year of the dragon
				(
					new byte[] { 5 }, PBESpecies.Druddigon, 1, true, null, new PBEAbility[] { PBEAbility.RoughSkin, PBEAbility.SheerForce }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Leer, PBEMove.Scratch, PBEMove.None, PBEMove.None }
				)
			})
		},
		{
			PBESpecies.Eevee,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Eevee collection promotion
				(
					new byte[] { 4 }, PBESpecies.Eevee, 10, false, PBEGender.Female, new PBEAbility[] { PBEAbility.Adaptability }, new PBENature[] { PBENature.Lonely },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Covet, PBEMove.Bite, PBEMove.HelpingHand, PBEMove.Attract }
				),
				new PBEDDEventPokemon // World championships 2010
				(
					new byte[] { 4 }, PBESpecies.Eevee, 50, true, PBEGender.Male, new PBEAbility[] { PBEAbility.Adaptability }, new PBENature[] { PBENature.Hardy },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.IronTail, PBEMove.TrumpCard, PBEMove.Flail, PBEMove.QuickAttack }
				),
				new PBEDDEventPokemon // Ikimono-gakari promotion
				(
					new byte[] { 5 }, PBESpecies.Eevee, 50, false, PBEGender.Female, new PBEAbility[] { PBEAbility.Adaptability }, new PBENature[] { PBENature.Hardy },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Sing, PBEMove.Return, PBEMove.EchoedVoice, PBEMove.Attract }
				)
			})
		},
		{
			PBESpecies.Emboar,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Pokémon Center 15th anniversary
				(
					new byte[] { 5 }, PBESpecies.Emboar, 100, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Blaze }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.FlareBlitz, PBEMove.HammerArm, PBEMove.WildCharge, PBEMove.HeadSmash }
				)
			})
		},
		{
			PBESpecies.Empoleon,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Pokémon Center 15th anniversary
				(
					new byte[] { 5 }, PBESpecies.Empoleon, 100, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Torrent }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.HydroPump, PBEMove.IceBeam, PBEMove.AquaJet, PBEMove.GrassKnot }
				)
			})
		},
		{
			PBESpecies.Espeon,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Journey across America | Party of the decade
				(
					new byte[] { 3 }, PBESpecies.Espeon, 70, false, null, new PBEAbility[] { PBEAbility.Synchronize }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Psybeam, PBEMove.PsychUp, PBEMove.Psychic, PBEMove.MorningSun }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Espeon, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.MagicBounce }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.TailWhip, PBEMove.Tackle, PBEMove.HelpingHand, PBEMove.SandAttack }
				)
			})
		},
		{
			PBESpecies.Farfetchd,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // PCNY
				(
					new byte[] { 3 }, PBESpecies.Farfetchd, 5, null, null, new PBEAbility[] { PBEAbility.InnerFocus, PBEAbility.KeenEye }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Yawn, PBEMove.Wish, PBEMove.None, PBEMove.None }
				)
			})
		},
		{
			PBESpecies.Flareon,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Flareon, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Guts }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.TailWhip, PBEMove.Tackle, PBEMove.HelpingHand, PBEMove.SandAttack }
				)
			})
		},
		{
			PBESpecies.Garchomp,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Strongest class
				(
					new byte[] { 5 }, PBESpecies.Garchomp, 100, false, PBEGender.Male, new PBEAbility[] { PBEAbility.SandVeil }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Outrage, PBEMove.Earthquake, PBEMove.SwordsDance, PBEMove.StoneEdge }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Garchomp, 48, false, PBEGender.Male, new PBEAbility[] { PBEAbility.RoughSkin }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Slash, PBEMove.DragonClaw, PBEMove.Dig, PBEMove.Crunch }
				)
			})
		},
		{
			PBESpecies.Genesect,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Plasma Genesect
				(
					new byte[] { 5 }, PBESpecies.Genesect, 50, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.Download }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.TechnoBlast, PBEMove.MagnetBomb, PBEMove.SolarBeam, PBEMove.SignalBeam }
				),
				new PBEDDEventPokemon // Genesect movie promotion
				(
					new byte[] { 5 }, PBESpecies.Genesect, 100, true, PBEGender.Genderless, new PBEAbility[] { PBEAbility.Download }, new PBENature[] { PBENature.Hasty },
					new byte?[] { null, 31, null, null, null, 31 }, new PBEMove[] { PBEMove.ExtremeSpeed, PBEMove.TechnoBlast, PBEMove.BlazeKick, PBEMove.ShiftGear }
				)
			})
		},
		{
			PBESpecies.Giratina,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // 2013 shiny creation trio
				(
					new byte[] { 5 }, PBESpecies.Giratina, 100, true, PBEGender.Genderless, new PBEAbility[] { PBEAbility.Pressure }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.DragonPulse, PBEMove.DragonClaw, PBEMove.AuraSphere, PBEMove.ShadowForce }
				)
			})
		},
		{
			PBESpecies.Glaceon,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Glaceon, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.IceBody }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.TailWhip, PBEMove.Tackle, PBEMove.HelpingHand, PBEMove.SandAttack }
				)
			})
		},
		{
			PBESpecies.Golurk,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Victini movies promotion
				(
					new byte[] { 5 }, PBESpecies.Golurk, 70, true, PBEGender.Genderless, new PBEAbility[] { PBEAbility.IronFist }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.ShadowPunch, PBEMove.HyperBeam, PBEMove.GyroBall, PBEMove.HammerArm }
				)
			})
		},
		{
			PBESpecies.Gorebyss,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // PCNY
				(
					new byte[] { 3 }, PBESpecies.Gorebyss, 20, false, null, new PBEAbility[] { PBEAbility.SwiftSwim }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Whirlpool, PBEMove.Confusion, PBEMove.Agility, PBEMove.None }
				)
			})
		},
		{
			PBESpecies.Haxorus,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Iris's Haxorus
				(
					new byte[] { 5 }, PBESpecies.Haxorus, 59, false, PBEGender.Female, new PBEAbility[] { PBEAbility.MoldBreaker }, new PBENature[] { PBENature.Naive },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Earthquake, PBEMove.DualChop, PBEMove.XScissor, PBEMove.DragonDance }
				)
			})
		},
		{
			PBESpecies.Huntail,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // PCNY
				(
					new byte[] { 3 }, PBESpecies.Huntail, 20, false, null, new PBEAbility[] { PBEAbility.SwiftSwim }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Whirlpool, PBEMove.Bite, PBEMove.Screech, PBEMove.None }
				)
			})
		},
		{
			PBESpecies.Hydreigon,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Victini movies promotion
				(
					new byte[] { 5 }, PBESpecies.Hydreigon, 70, true, PBEGender.Male, new PBEAbility[] { PBEAbility.Levitate }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.HyperVoice, PBEMove.DragonBreath, PBEMove.Flamethrower, PBEMove.FocusBlast }
				)
			})
		},
		{
			PBESpecies.Infernape,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Pokémon Center 15th anniversary
				(
					new byte[] { 5 }, PBESpecies.Infernape, 100, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Blaze }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.FireBlast, PBEMove.CloseCombat, PBEMove.Uturn, PBEMove.GrassKnot }
				)
			})
		},
		{
			PBESpecies.Jirachi,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Jirachi Wish Maker promotion | Wishmaker Jirachi | Channel Jirachi | 2004 Jirachi | 2005 Jirachi | 2006 Jirachi | 2007 Jirachi | 2008 Jirachi | 2010 (Korea) Jirachi
				(
					new byte[] { 3, 4 }, PBESpecies.Jirachi, 5, null, PBEGender.Genderless, new PBEAbility[] { PBEAbility.SereneGrace }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Wish, PBEMove.Confusion, PBEMove.Rest, PBEMove.None }
				),
				new PBEDDEventPokemon // PokéPark Jirachi
				(
					new byte[] { 3 }, PBESpecies.Jirachi, 30, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.SereneGrace }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Wish, PBEMove.Psychic, PBEMove.HelpingHand, PBEMove.Rest }
				),
				new PBEDDEventPokemon // 2009 Jirachi | 2010 (rest of the world) Jirachi
				(
					new byte[] { 4 }, PBESpecies.Jirachi, 5, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.SereneGrace }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Wish, PBEMove.Confusion, PBEMove.Rest, PBEMove.DracoMeteor }
				),
				new PBEDDEventPokemon // Decolora Jirachi
				(
					new byte[] { 5 }, PBESpecies.Jirachi, 50, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.SereneGrace }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.HealingWish, PBEMove.Psychic, PBEMove.Swift, PBEMove.MeteorMash }
				),
				new PBEDDEventPokemon // Character Fair Jirachi
				(
					new byte[] { 5 }, PBESpecies.Jirachi, 50, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.SereneGrace }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Wish, PBEMove.HealingWish, PBEMove.CosmicPower, PBEMove.MeteorMash }
				),
				new PBEDDEventPokemon // 2013 Jirachi
				(
					new byte[] { 5 }, PBESpecies.Jirachi, 50, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.SereneGrace }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.DracoMeteor, PBEMove.MeteorMash, PBEMove.Wish, PBEMove.FollowMe }
				),
				new PBEDDEventPokemon // Chilseok Jirachi
				(
					new byte[] { 5 }, PBESpecies.Jirachi, 50, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.SereneGrace }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Wish, PBEMove.HealingWish, PBEMove.Swift, PBEMove.Return }
				)
			})
		},
		{
			PBESpecies.Jolteon,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Jolteon, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.QuickFeet }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.TailWhip, PBEMove.Tackle, PBEMove.HelpingHand, PBEMove.SandAttack }
				)
			})
		},
		{
			PBESpecies.Karrablast,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Trade for Evolution!
				(
					new byte[] { 5 }, PBESpecies.Karrablast, 30, false, null, new PBEAbility[] { PBEAbility.ShedSkin, PBEAbility.Swarm }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.FuryAttack, PBEMove.Headbutt, PBEMove.FalseSwipe, PBEMove.BugBuzz }
				),
				new PBEDDEventPokemon // Summer 2011
				(
					new byte[] { 5 }, PBESpecies.Karrablast, 50, false, null, new PBEAbility[] { PBEAbility.ShedSkin, PBEAbility.Swarm }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Megahorn, PBEMove.TakeDown, PBEMove.XScissor, PBEMove.Flail }
				)
			})
		},
		{
			PBESpecies.Keldeo,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Kyurem VS. The Sword of Justice promotion
				(
					new byte[] { 5 }, PBESpecies.Keldeo, 15, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.Justified }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.AquaJet, PBEMove.Leer, PBEMove.DoubleKick, PBEMove.BubbleBeam }
				),
				new PBEDDEventPokemon // Winter 2013
				(
					new byte[] { 5 }, PBESpecies.Keldeo, 50, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.Justified }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.SacredSword, PBEMove.HydroPump, PBEMove.AquaJet, PBEMove.SwordsDance }
				)
			})
		},
		{
			PBESpecies.Latias,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Journey across America | Top 10 distribution | Party of the decade
				(
					new byte[] { 3 }, PBESpecies.Latias, 70, false, PBEGender.Female, new PBEAbility[] { PBEAbility.Levitate }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.MistBall, PBEMove.Psychic, PBEMove.Recover, PBEMove.Charm }
				)
			})
		},
		{
			PBESpecies.Latios,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Journey across America | Top 10 distribution | Party of the decade
				(
					new byte[] { 3 }, PBESpecies.Latios, 70, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Levitate }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.LusterPurge, PBEMove.Psychic, PBEMove.Recover, PBEMove.DragonDance }
				)
			})
		},
		{
			PBESpecies.Leafeon,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Leafeon, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Chlorophyll }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.TailWhip, PBEMove.Tackle, PBEMove.HelpingHand, PBEMove.SandAttack }
				)
			})
		},
		{
			PBESpecies.Liepard,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Se Jun's Liepard
				(
					new byte[] { 5 }, PBESpecies.Liepard, 20, false, PBEGender.Female, new PBEAbility[] { PBEAbility.Prankster }, new PBENature[] { PBENature.Jolly },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.FakeOut, PBEMove.FoulPlay, PBEMove.Encore, PBEMove.Swagger }
				)
			})
		},
		{
			PBESpecies.Lileep,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Pokémon Adventure Camp fossil Pokémon
				(
					new byte[] { 5 }, PBESpecies.Lileep, 15, false, null, new PBEAbility[] { PBEAbility.SuctionCups }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Recover, PBEMove.RockSlide, PBEMove.Constrict, PBEMove.Acid }
				)
			})
		},
		{
			PBESpecies.Lucario,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // PalCity Lucario
				(
					new byte[] { 4 }, PBESpecies.Lucario, 50, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Steadfast }, new PBENature[] { PBENature.Modest },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.AuraSphere, PBEMove.DarkPulse, PBEMove.DragonPulse, PBEMove.WaterPulse }
				),
				new PBEDDEventPokemon // World championships 2008
				(
					new byte[] { 4 }, PBESpecies.Lucario, 30, false, PBEGender.Male, new PBEAbility[] { PBEAbility.InnerFocus }, new PBENature[] { PBENature.Adamant },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.ForcePalm, PBEMove.BoneRush, PBEMove.SunnyDay, PBEMove.BlazeKick }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Lucario, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Justified }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Detect, PBEMove.MetalClaw, PBEMove.Counter, PBEMove.BulletPunch }
				),
				new PBEDDEventPokemon // Powerful Tag
				(
					new byte[] { 5 }, PBESpecies.Lucario, 50, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Justified }, new PBENature[] { PBENature.Naughty },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.BulletPunch, PBEMove.CloseCombat, PBEMove.StoneEdge, PBEMove.ShadowClaw }
				)
			})
		},
		{
			PBESpecies.Magikarp,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // GTS Ryuuta
				(
					new byte[] { 4 }, PBESpecies.Magikarp, 4, false, PBEGender.Male, new PBEAbility[] { PBEAbility.SwiftSwim }, new PBENature[] { PBENature.Modest },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Splash, PBEMove.None, PBEMove.None, PBEMove.None }
				),
				new PBEDDEventPokemon // GTS Nana
				(
					new byte[] { 4 }, PBESpecies.Magikarp, 5, false, PBEGender.Female, new PBEAbility[] { PBEAbility.SwiftSwim }, new PBENature[] { PBENature.Lonely },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Splash, PBEMove.None, PBEMove.None, PBEMove.None }
				),
				new PBEDDEventPokemon // GTS Utz
				(
					new byte[] { 4 }, PBESpecies.Magikarp, 5, false, PBEGender.Male, new PBEAbility[] { PBEAbility.SwiftSwim }, new PBENature[] { PBENature.Relaxed },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Splash, PBEMove.None, PBEMove.None, PBEMove.None }
				),
				new PBEDDEventPokemon // GTS Ruirui
				(
					new byte[] { 4 }, PBESpecies.Magikarp, 6, false, PBEGender.Female, new PBEAbility[] { PBEAbility.SwiftSwim }, new PBENature[] { PBENature.Rash },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Splash, PBEMove.None, PBEMove.None, PBEMove.None }
				),
				new PBEDDEventPokemon // GTS Nory
				(
					new byte[] { 4 }, PBESpecies.Magikarp, 7, false, PBEGender.Female, new PBEAbility[] { PBEAbility.SwiftSwim }, new PBENature[] { PBENature.Hardy },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Splash, PBEMove.None, PBEMove.None, PBEMove.None }
				),
				new PBEDDEventPokemon // Pokémon Center relocation
				(
					new byte[] { 5 }, PBESpecies.Magikarp, 99, true, null, new PBEAbility[] { PBEAbility.SwiftSwim }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Flail, PBEMove.HydroPump, PBEMove.Bounce, PBEMove.Splash }
				)
			})
		},
		{
			PBESpecies.Meloetta,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Kyruem VS. The Sword of Justice promotion
				(
					new byte[] { 5 }, PBESpecies.Meloetta, 15, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.SereneGrace }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Round, PBEMove.QuickAttack, PBEMove.Confusion, PBEMove.None }
				),
				new PBEDDEventPokemon // Summer 2013
				(
					new byte[] { 5 }, PBESpecies.Meloetta, 50, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.SereneGrace }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Round, PBEMove.TeeterDance, PBEMove.Psychic, PBEMove.CloseCombat }
				)
			})
		},
		{
			PBESpecies.Metagross,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Train station
				(
					new byte[] { 5 }, PBESpecies.Metagross, 50, null, PBEGender.Genderless, new PBEAbility[] { PBEAbility.ClearBody }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.MeteorMash, PBEMove.Earthquake, PBEMove.BulletPunch, PBEMove.HammerArm }
				),
				new PBEDDEventPokemon // Strongest class
				(
					new byte[] { 5 }, PBESpecies.Metagross, 100, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.ClearBody }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.BulletPunch, PBEMove.ZenHeadbutt, PBEMove.HammerArm, PBEMove.IcePunch }
				),
				new PBEDDEventPokemon // World championships 2013
				(
					new byte[] { 5 }, PBESpecies.Metagross, 45, true, PBEGender.Genderless, new PBEAbility[] { PBEAbility.ClearBody }, new PBENature[] { PBENature.Adamant },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.MeteorMash, PBEMove.Earthquake, PBEMove.ZenHeadbutt, PBEMove.Protect }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Metagross, 45, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.LightMetal }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.IronDefense, PBEMove.Agility, PBEMove.HammerArm, PBEMove.DoubleEdge }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Metagross, 45, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.LightMetal }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Psychic, PBEMove.MeteorMash, PBEMove.HammerArm, PBEMove.DoubleEdge }
				),
				new PBEDDEventPokemon // Steven's Metagross
				(
					new byte[] { 5 }, PBESpecies.Metagross, 58, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.ClearBody }, new PBENature[] { PBENature.Serious },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Earthquake, PBEMove.HyperBeam, PBEMove.Psychic, PBEMove.MeteorMash }
				)
			})
		},
		{
			PBESpecies.Metang,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Gale of Darkness demo promotion
				(
					new byte[] { 3 }, PBESpecies.Metang, 30, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.ClearBody }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.TakeDown, PBEMove.Confusion, PBEMove.MetalClaw, PBEMove.Refresh }
				)
			})
		},
		{
			PBESpecies.Minun,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // PokéPark Minun
				(
					new byte[] { 3 }, PBESpecies.Minun, 5, null, null, new PBEAbility[] { PBEAbility.Minus }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Growl, PBEMove.ThunderWave, PBEMove.MudSport, PBEMove.None }
				),
				new PBEDDEventPokemon // Gather more Pokémon
				(
					new byte[] { 3 }, PBESpecies.Minun, 10, false, null, new PBEAbility[] { PBEAbility.Minus }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Growl, PBEMove.ThunderWave, PBEMove.QuickAttack, PBEMove.None }
				)
			})
		},
		{
			PBESpecies.Misdreavus,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Gather more Pokémon
				(
					new byte[] { 3 }, PBESpecies.Misdreavus, 10, false, null, new PBEAbility[] { PBEAbility.Levitate }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Growl, PBEMove.Psywave, PBEMove.Spite, PBEMove.None }
				)
			})
		},
		{
			PBESpecies.Ninetales,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Powerful Tag
				(
					new byte[] { 5 }, PBESpecies.Ninetales, 50, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Drought }, new PBENature[] { PBENature.Bold },
					new byte?[] { null, null, 31, null, null, null }, new PBEMove[] { PBEMove.HeatWave, PBEMove.SolarBeam, PBEMove.Psyshock, PBEMove.WillOWisp }
				)
			})
		},
		{
			PBESpecies.Palkia,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // 2013 shiny creation trio
				(
					new byte[] { 5 }, PBESpecies.Palkia, 100, true, PBEGender.Genderless, new PBEAbility[] { PBEAbility.Pressure }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.HydroPump, PBEMove.DracoMeteor, PBEMove.SpacialRend, PBEMove.AuraSphere }
				)
			})
		},
		{
			PBESpecies.Pansage,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Cilan's Pansage
				(
					new byte[] { 5 }, PBESpecies.Pansage, 1, null, PBEGender.Male, new PBEAbility[] { PBEAbility.Gluttony }, new PBENature[] { PBENature.Brave },
					new byte?[] { null, 31, null, null, null, null }, new PBEMove[] { PBEMove.BulletSeed, PBEMove.Bite, PBEMove.SolarBeam, PBEMove.Dig }
				),
				new PBEDDEventPokemon // Best Wishes Cilan's Pansage
				(
					new byte[] { 5 }, PBESpecies.Pansage, 30, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Gluttony }, new PBENature[] { PBENature.Serious },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.BulletSeed, PBEMove.SolarBeam, PBEMove.RockTomb, PBEMove.Dig }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Pansage, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Overgrow }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Leer, PBEMove.Lick, PBEMove.VineWhip, PBEMove.LeafStorm }
				)
			})
		},
		{
			PBESpecies.Pichu,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // 5th anniversary egg | Pokémon Stamp RS magazine raffle
				(
					new byte[] { 3 }, PBESpecies.Pichu, 5, null, null, new PBEAbility[] { PBEAbility.Static }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.ThunderShock, PBEMove.Charm, PBEMove.TeeterDance, PBEMove.None }
				),
				new PBEDDEventPokemon // 5th anniversary egg
				(
					new byte[] { 3 }, PBESpecies.Pichu, 5, null, null, new PBEAbility[] { PBEAbility.Static }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.ThunderShock, PBEMove.Charm, PBEMove.Wish, PBEMove.None }
				),
				new PBEDDEventPokemon // Pokémon Box bonus egg
				(
					new byte[] { 3 }, PBESpecies.Pichu, 5, null, null, new PBEAbility[] { PBEAbility.Static }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.ThunderShock, PBEMove.Charm, PBEMove.Surf, PBEMove.None }
				),
				new PBEDDEventPokemon // PokéPark Pichu
				(
					new byte[] { 3 }, PBESpecies.Pichu, 5, null, null, new PBEAbility[] { PBEAbility.Static }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.ThunderShock, PBEMove.Charm, PBEMove.FollowMe, PBEMove.None }
				),
				new PBEDDEventPokemon // Red and Green 12th anniversary
				(
					new byte[] { 4 }, PBESpecies.Pichu, 1, null, null, new PBEAbility[] { PBEAbility.Static }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.VoltTackle, PBEMove.Thunderbolt, PBEMove.GrassKnot, PBEMove.Return }
				),
				new PBEDDEventPokemon // Jewel of Life promotion
				(
					new byte[] { 4 }, PBESpecies.Pichu, 30, true, PBEGender.Male, new PBEAbility[] { PBEAbility.Static }, new PBENature[] { PBENature.Jolly },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Charge, PBEMove.VoltTackle, PBEMove.Endeavor, PBEMove.Endure }
				)
			})
		},
		{
			PBESpecies.Pidove,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Ash's Pidove
				(
					new byte[] { 5 }, PBESpecies.Pidove, 1, null, PBEGender.Female, new PBEAbility[] { PBEAbility.SuperLuck }, new PBENature[] { PBENature.Hardy },
					new byte?[] { null, 31, null, null, null, null }, new PBEMove[] { PBEMove.Gust, PBEMove.QuickAttack, PBEMove.AirCutter, PBEMove.None }
				)
			})
		},
		{
			PBESpecies.Pikachu,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // PCNY stone promotion
				(
					new byte[] { 3 }, PBESpecies.Pikachu, 50, false, null, new PBEAbility[] { PBEAbility.Static }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Thunderbolt, PBEMove.Agility, PBEMove.Thunder, PBEMove.LightScreen }
				),
				new PBEDDEventPokemon // All Nippon Airways Pikachu (Gen 3)
				(
					new byte[] { 3 }, PBESpecies.Pikachu, 10, false, null, new PBEAbility[] { PBEAbility.Static }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Fly, PBEMove.ThunderShock, PBEMove.TailWhip, PBEMove.ThunderWave }
				),
				new PBEDDEventPokemon // Yokohama Pokémon Center opening
				(
					new byte[] { 3 }, PBESpecies.Pikachu, 10, false, null, new PBEAbility[] { PBEAbility.Static }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.ThunderShock, PBEMove.Growl, PBEMove.ThunderWave, PBEMove.Surf }
				),
				new PBEDDEventPokemon // GW Festival | Sapporo Pokémon Center opening
				(
					new byte[] { 3 }, PBESpecies.Pikachu, 10, false, null, new PBEAbility[] { PBEAbility.Static }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Growl, PBEMove.TailWhip, PBEMove.ThunderWave, PBEMove.Fly }
				),
				new PBEDDEventPokemon // Gather more Pokémon | Colosseum Pikachu
				(
					new byte[] { 3 }, PBESpecies.Pikachu, 10, false, null, new PBEAbility[] { PBEAbility.Static }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.ThunderShock, PBEMove.Growl, PBEMove.TailWhip, PBEMove.ThunderWave }
				),
				new PBEDDEventPokemon // Journey across America | Top 10 distribution
				(
					new byte[] { 3 }, PBESpecies.Pikachu, 70, false, null, new PBEAbility[] { PBEAbility.Static }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Thunderbolt, PBEMove.Agility, PBEMove.Thunder, PBEMove.LightScreen }
				),
				new PBEDDEventPokemon // Party of the decade
				(
					new byte[] { 3 }, PBESpecies.Pikachu, 70, false, null, new PBEAbility[] { PBEAbility.Static }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Thunderbolt, PBEMove.Thunder, PBEMove.LightScreen, PBEMove.Fly }
				),
				new PBEDDEventPokemon // Battle Revolution Pikachu
				(
					new byte[] { 4 }, PBESpecies.Pikachu, 10, false, PBEGender.Female, new PBEAbility[] { PBEAbility.Static }, new PBENature[] { PBENature.Hardy },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.VoltTackle, PBEMove.Surf, PBEMove.TailWhip, PBEMove.ThunderWave }
				),
				new PBEDDEventPokemon // TCG world championships 2007
				(
					new byte[] { 4 }, PBESpecies.Pikachu, 50, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Static }, new PBENature[] { PBENature.Hardy },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Surf, PBEMove.Thunderbolt, PBEMove.LightScreen, PBEMove.QuickAttack }
				),
				new PBEDDEventPokemon // Nintendo Spot promotion | Nintendo Zone promotion | 7-Eleven Pikachu
				(
					new byte[] { 4 }, PBESpecies.Pikachu, 20, false, null, new PBEAbility[] { PBEAbility.Static }, new PBENature[] { PBENature.Bashful, PBENature.Docile },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.QuickAttack, PBEMove.ThunderShock, PBEMove.TailWhip, PBEMove.Present }
				),
				new PBEDDEventPokemon // Yokohama Pokémon Center reopening | 2008 birthday | 2009 birthday
				(
					new byte[] { 4 }, PBESpecies.Pikachu, 40, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Static }, new PBENature[] { PBENature.Mild, PBENature.Modest },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Surf, PBEMove.Thunder, PBEMove.Protect, PBEMove.None }
				),
				new PBEDDEventPokemon // Sleeping Pikachu Collection promotion
				(
					new byte[] { 4 }, PBESpecies.Pikachu, 50, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Static }, new PBENature[] { PBENature.Relaxed },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Rest, PBEMove.SleepTalk, PBEMove.Yawn, PBEMove.Snore }
				),
				new PBEDDEventPokemon // Character Fair Pikachu
				(
					new byte[] { 4 }, PBESpecies.Pikachu, 30, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Static }, new PBENature[] { PBENature.Brave },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.VoltTackle, PBEMove.QuickAttack, PBEMove.Thunderbolt, PBEMove.IronTail }
				),
				new PBEDDEventPokemon // Kyoto Cross Media Experience Pikachu
				(
					new byte[] { 4 }, PBESpecies.Pikachu, 30, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Static }, new PBENature[] { PBENature.Naughty },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.LastResort, PBEMove.Present, PBEMove.Thunderbolt, PBEMove.QuickAttack }
				),
				new PBEDDEventPokemon // Ario Pikachu
				(
					new byte[] { 4 }, PBESpecies.Pikachu, 20, false, PBEGender.Female, new PBEAbility[] { PBEAbility.Static }, new PBENature[] { PBENature.Bashful },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Present, PBEMove.QuickAttack, PBEMove.ThunderWave, PBEMove.TailWhip }
				),
				new PBEDDEventPokemon // Ash's Pikachu
				(
					new byte[] { 4 }, PBESpecies.Pikachu, 50, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Static }, new PBENature[] { PBENature.Naughty },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.VoltTackle, PBEMove.IronTail, PBEMove.QuickAttack, PBEMove.Thunderbolt }
				),
				new PBEDDEventPokemon // All Nippon Airways Pikachu (Gen 5)
				(
					new byte[] { 5 }, PBESpecies.Pikachu, 50, false, null, new PBEAbility[] { PBEAbility.Static }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Fly, PBEMove.IronTail, PBEMove.ElectroBall, PBEMove.QuickAttack }
				),
				new PBEDDEventPokemon // Singing Pikachu
				(
					new byte[] { 5 }, PBESpecies.Pikachu, 30, false, PBEGender.Female, new PBEAbility[] { PBEAbility.Lightningrod }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Sing, PBEMove.TeeterDance, PBEMove.Encore, PBEMove.ElectroBall }
				),
				new PBEDDEventPokemon // ExtremeSpeed Pikachu
				(
					new byte[] { 5 }, PBESpecies.Pikachu, 50, null, PBEGender.Female, new PBEAbility[] { PBEAbility.Static }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.ExtremeSpeed, PBEMove.Thunderbolt, PBEMove.GrassKnot, PBEMove.BrickBreak }
				),
				new PBEDDEventPokemon // Pikachu Festival | Pika Pika Carnival | Summer 2012 Pikachu | Strongest class
				(
					new byte[] { 5 }, PBESpecies.Pikachu, 100, null, PBEGender.Female, new PBEAbility[] { PBEAbility.Static }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Thunder, PBEMove.VoltTackle, PBEMove.GrassKnot, PBEMove.QuickAttack }
				),
				new PBEDDEventPokemon // World championships 2012
				(
					new byte[] { 5 }, PBESpecies.Pikachu, 50, false, PBEGender.Female, new PBEAbility[] { PBEAbility.Lightningrod }, new PBENature[] { PBENature.Timid },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Fly, PBEMove.Thunderbolt, PBEMove.GrassKnot, PBEMove.Protect }
				),
				new PBEDDEventPokemon // Pokémon Center 15th anniversary
				(
					new byte[] { 5 }, PBESpecies.Pikachu, 100, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Lightningrod }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.VoltTackle, PBEMove.QuickAttack, PBEMove.Feint, PBEMove.VoltSwitch }
				),
				new PBEDDEventPokemon // Best Wishes Ash's Pikachu
				(
					new byte[] { 5 }, PBESpecies.Pikachu, 50, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Static }, new PBENature[] { PBENature.Brave },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Thunderbolt, PBEMove.QuickAttack, PBEMove.IronTail, PBEMove.ElectroBall }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Pikachu, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Lightningrod }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.ThunderShock, PBEMove.TailWhip, PBEMove.ThunderWave, PBEMove.Headbutt }
				)
			})
		},
		{
			PBESpecies.Piplup,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Pokémon Searcher BW promotion
				(
					new byte[] { 5 }, PBESpecies.Piplup, 15, null, null, new PBEAbility[] { PBEAbility.Torrent }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.HydroPump, PBEMove.FeatherDance, PBEMove.WaterSport, PBEMove.Peck }
				),
				new PBEDDEventPokemon // Dawn's Piplup
				(
					new byte[] { 5 }, PBESpecies.Piplup, 15, false, null, new PBEAbility[] { PBEAbility.Torrent }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Sing, PBEMove.Round, PBEMove.FeatherDance, PBEMove.Peck }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Piplup, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Defiant }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Pound, PBEMove.Growl, PBEMove.Bubble, PBEMove.None }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Piplup, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Defiant }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Pound, PBEMove.Growl, PBEMove.Bubble, PBEMove.FeatherDance }
				)
			})
		},
		{
			PBESpecies.Plusle,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // PokéPark Plusle
				(
					new byte[] { 3 }, PBESpecies.Plusle, 5, null, null, new PBEAbility[] { PBEAbility.Plus }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Growl, PBEMove.ThunderWave, PBEMove.WaterSport, PBEMove.None }
				),
				new PBEDDEventPokemon // Gather more Pokémon
				(
					new byte[] { 3 }, PBESpecies.Plusle, 10, false, null, new PBEAbility[] { PBEAbility.Plus }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Growl, PBEMove.ThunderWave, PBEMove.QuickAttack, PBEMove.None }
				)
			})
		},
		{
			PBESpecies.Politoed,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Yamamoto's Tournament
				(
					new byte[] { 5 }, PBESpecies.Politoed, 50, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Drizzle }, new PBENature[] { PBENature.Calm },
					new byte?[] { 31, 13, 31, 5, 31, 5 }, new PBEMove[] { PBEMove.Scald, PBEMove.IceBeam, PBEMove.PerishSong, PBEMove.Protect }
				)
			})
		},
		{
			PBESpecies.Poliwag,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Egg Pokémon Present
				(
					new byte[] { 3 }, PBESpecies.Poliwag, 5, null, null, new PBEAbility[] { PBEAbility.Damp, PBEAbility.WaterAbsorb }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Bubble, PBEMove.SweetKiss, PBEMove.None, PBEMove.None }
				)
			})
		},
		{
			PBESpecies.Psyduck,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Pokémon Trade and Battle Day
				(
					new byte[] { 3 }, PBESpecies.Psyduck, 27, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Damp }, new PBENature[] { PBENature.Lax },
					new byte?[] { 31, 16, 12, 29, 31, 14 }, new PBEMove[] { PBEMove.TailWhip, PBEMove.Disable, PBEMove.Confusion, PBEMove.Screech }
				),
				new PBEDDEventPokemon // PokéPark Psyduck
				(
					new byte[] { 3 }, PBESpecies.Psyduck, 5, null, null, new PBEAbility[] { PBEAbility.CloudNine, PBEAbility.Damp }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.WaterSport, PBEMove.Scratch, PBEMove.TailWhip, PBEMove.MudSport }
				),
				new PBEDDEventPokemon // GTS Psyducks
				(
					new byte[] { 4 }, PBESpecies.Psyduck, 1, false, null, new PBEAbility[] { PBEAbility.CloudNine, PBEAbility.Damp }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Psychic, PBEMove.ConfuseRay, PBEMove.Yawn, PBEMove.MudBomb }
				)
			})
		},
		{
			PBESpecies.Regirock,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Lucario and the Mystery of Mew promotion
				(
					new byte[] { 3 }, PBESpecies.Regirock, 40, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.ClearBody }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Curse, PBEMove.Superpower, PBEMove.AncientPower, PBEMove.HyperBeam }
				)
			})
		},
		{
			PBESpecies.Reshiram,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Victini movies promotion
				(
					new byte[] { 5 }, PBESpecies.Reshiram, 100, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.Turboblaze }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.BlueFlare, PBEMove.FusionFlare, PBEMove.Mist, PBEMove.DracoMeteor }
				)
			})
		},
		{
			PBESpecies.Riolu,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Pokémon Ranger
				(
					new byte[] { 4 }, PBESpecies.Riolu, 40, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Steadfast }, new PBENature[] { PBENature.Serious },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.AuraSphere, PBEMove.ShadowClaw, PBEMove.BulletPunch, PBEMove.DrainPunch }
				)
			})
		},
		{
			PBESpecies.Rotom,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Best Wishes Professor Oak's Rotom
				(
					new byte[] { 5 }, PBESpecies.Rotom, 10, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.Levitate }, new PBENature[] { PBENature.Naughty },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Uproar, PBEMove.Astonish, PBEMove.Trick, PBEMove.ThunderShock }
				)
			})
		},
		{
			PBESpecies.Samurott,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Pokémon Center 15th anniversary
				(
					new byte[] { 5 }, PBESpecies.Samurott, 100, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Torrent }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.HydroPump, PBEMove.IceBeam, PBEMove.Megahorn, PBEMove.Superpower }
				)
			})
		},
		{
			PBESpecies.Scrafty,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // World championships 2011
				(
					new byte[] { 5 }, PBESpecies.Scrafty, 50, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Moxie }, new PBENature[] { PBENature.Brave },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.FirePunch, PBEMove.Payback, PBEMove.DrainPunch, PBEMove.Substitute }
				)
			})
		},
		{
			PBESpecies.Scraggy,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Ash's Scraggy
				(
					new byte[] { 5 }, PBESpecies.Scraggy, 1, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Moxie }, new PBENature[] { PBENature.Adamant },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Leer, PBEMove.LowKick, PBEMove.Headbutt, PBEMove.HiJumpKick }
				)
			})
		},
		{
			PBESpecies.Serperior,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Pokémon Center 15th anniversary
				(
					new byte[] { 5 }, PBESpecies.Serperior, 100, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Torrent }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.LeafStorm, PBEMove.Substitute, PBEMove.GigaDrain, PBEMove.LeechSeed }
				)
			})
		},
		{
			PBESpecies.Shedinja,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // PCNY Monster Week 1
				(
					new byte[] { 3 }, PBESpecies.Shedinja, 50, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.WonderGuard }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Spite, PBEMove.ConfuseRay, PBEMove.ShadowBall, PBEMove.Grudge }
				)
			})
		},
		{
			PBESpecies.Shelmet,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Trade for Evolution!
				(
					new byte[] { 5 }, PBESpecies.Shelmet, 30, false, null, new PBEAbility[] { PBEAbility.Hydration, PBEAbility.ShellArmor }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.StruggleBug, PBEMove.MegaDrain, PBEMove.Yawn, PBEMove.Protect }
				),
				new PBEDDEventPokemon // Summer 2011
				(
					new byte[] { 5 }, PBESpecies.Shelmet, 50, false, null, new PBEAbility[] { PBEAbility.Hydration, PBEAbility.ShellArmor }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Encore, PBEMove.GigaDrain, PBEMove.BodySlam, PBEMove.BugBuzz }
				)
			})
		},
		{
			PBESpecies.Skitty,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // PokéPark Skitty
				(
					new byte[] { 3 }, PBESpecies.Skitty, 5, null, null, new PBEAbility[] { PBEAbility.CuteCharm }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Growl, PBEMove.Tackle, PBEMove.TailWhip, PBEMove.Rollout }
				),
				new PBEDDEventPokemon // Gather more Pokémon
				(
					new byte[] { 3 }, PBESpecies.Skitty, 10, false, null, new PBEAbility[] { PBEAbility.CuteCharm }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Growl, PBEMove.Tackle, PBEMove.TailWhip, PBEMove.Attract }
				),
				new PBEDDEventPokemon // Pokémon Box bonus egg
				(
					new byte[] { 3 }, PBESpecies.Skitty, 5, null, null, new PBEAbility[] { PBEAbility.CuteCharm }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Tackle, PBEMove.Growl, PBEMove.TailWhip, PBEMove.PayDay }
				)
			})
		},
		{
			PBESpecies.Smeargle,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Gather more Pokémon
				(
					new byte[] { 3 }, PBESpecies.Smeargle, 10, false, null, new PBEAbility[] { PBEAbility.OwnTempo }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Sketch, PBEMove.None, PBEMove.None, PBEMove.None }
				),
				new PBEDDEventPokemon // World championships 2013
				(
					new byte[] { 5 }, PBESpecies.Smeargle, 50, false, PBEGender.Female, new PBEAbility[] { PBEAbility.Technician }, new PBENature[] { PBENature.Jolly },
					new byte?[] { null, 31, null, null, null, 31 }, new PBEMove[] { PBEMove.FalseSwipe, PBEMove.Spore, PBEMove.OdorSleuth, PBEMove.MeanLook }
				)
			})
		},
		{
			PBESpecies.Snivy,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Pokémon Center Tohoku Snivy
				(
					new byte[] { 5 }, PBESpecies.Snivy, 5, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Overgrow }, new PBENature[] { PBENature.Hardy },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Growth, PBEMove.Synthesis, PBEMove.EnergyBall, PBEMove.Aromatherapy }
				)
			})
		},
		{
			PBESpecies.Squirtle,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Gather more Pokémon
				(
					new byte[] { 3 }, PBESpecies.Squirtle, 10, false, null, new PBEAbility[] { PBEAbility.Torrent }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Tackle, PBEMove.TailWhip, PBEMove.Bubble, PBEMove.Withdraw }
				),
				new PBEDDEventPokemon // Kanto starter egg
				(
					new byte[] { 5 }, PBESpecies.Squirtle, 1, null, null, new PBEAbility[] { PBEAbility.Torrent }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.FalseSwipe, PBEMove.Block, PBEMove.HydroCannon, PBEMove.FollowMe }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Squirtle, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.RainDish }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Tackle, PBEMove.TailWhip, PBEMove.Bubble, PBEMove.Withdraw }
				)
			})
		},
		{
			PBESpecies.Thundurus,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Milos Island Pokémon
				(
					new byte[] { 5 }, PBESpecies.Thundurus, 70, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Prankster }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Thunder, PBEMove.HammerArm, PBEMove.FocusBlast, PBEMove.WildCharge }
				)
			})
		},
		{
			PBESpecies.Tirtouga,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Pokémon Adventure Camp fossil Pokémon
				(
					new byte[] { 5 }, PBESpecies.Tirtouga, 15, false, null, new PBEAbility[] { PBEAbility.SolidRock, PBEAbility.Sturdy }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Bite, PBEMove.Protect, PBEMove.AquaJet, PBEMove.BodySlam }
				)
			})
		},
		{
			PBESpecies.Torchic,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Gather more Pokémon
				(
					new byte[] { 3 }, PBESpecies.Torchic, 10, false, null, new PBEAbility[] { PBEAbility.Blaze }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Scratch, PBEMove.Growl, PBEMove.FocusEnergy, PBEMove.Ember }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Torchic, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.SpeedBoost }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Scratch, PBEMove.Growl, PBEMove.FocusEnergy, PBEMove.Ember }
				)
			})
		},
		{
			PBESpecies.Tornadus,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Milos Island Pokémon
				(
					new byte[] { 5 }, PBESpecies.Tornadus, 70, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Prankster }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Hurricane, PBEMove.HammerArm, PBEMove.AirSlash, PBEMove.HiddenPower }
				)
			})
		},
		{
			PBESpecies.Torterra,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Pokémon Center 15th anniversary
				(
					new byte[] { 5 }, PBESpecies.Torterra, 100, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Overgrow }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.WoodHammer, PBEMove.Earthquake, PBEMove.Outrage, PBEMove.StoneEdge }
				)
			})
		},
		{
			PBESpecies.Tropius,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // PCNY
				(
					new byte[] { 3 }, PBESpecies.Tropius, 30, false, null, new PBEAbility[] { PBEAbility.Chlorophyll }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.RazorLeaf, PBEMove.Stomp, PBEMove.SweetScent, PBEMove.Whirlwind }
				),
				new PBEDDEventPokemon // Pokémon Sunday promotion
				(
					new byte[] { 4 }, PBESpecies.Tropius, 53, false, PBEGender.Female, new PBEAbility[] { PBEAbility.Chlorophyll }, new PBENature[] { PBENature.Jolly },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.AirSlash, PBEMove.Synthesis, PBEMove.SunnyDay, PBEMove.SolarBeam }
				)
			})
		},
		{
			PBESpecies.Umbreon,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Journey across America | Party of the decade
				(
					new byte[] { 3 }, PBESpecies.Umbreon, 70, false, null, new PBEAbility[] { PBEAbility.Synchronize }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.FaintAttack, PBEMove.MeanLook, PBEMove.Screech, PBEMove.Moonlight }
				),
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Umbreon, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.InnerFocus }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.TailWhip, PBEMove.Tackle, PBEMove.HelpingHand, PBEMove.SandAttack }
				)
			})
		},
		{
			PBESpecies.Vaporeon,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Global Link promotion
				(
					new byte[] { 5 }, PBESpecies.Vaporeon, 10, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Hydration }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.TailWhip, PBEMove.Tackle, PBEMove.HelpingHand, PBEMove.SandAttack }
				)
			})
		},
		{
			PBESpecies.Victini,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // M14 promotion
				(
					new byte[] { 5 }, PBESpecies.Victini, 50, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.VictoryStar }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.VCreate, PBEMove.FusionFlare, PBEMove.FusionBolt, PBEMove.SearingShot }
				),
				new PBEDDEventPokemon // Pokémon Center Tohoku Victini
				(
					new byte[] { 5 }, PBESpecies.Victini, 100, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.VictoryStar }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.VCreate, PBEMove.BlueFlare, PBEMove.BoltStrike, PBEMove.Glaciate }
				)
			})
		},
		{
			PBESpecies.Volcarona,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Korean Nationals 2012
				(
					new byte[] { 5 }, PBESpecies.Volcarona, 100, true, PBEGender.Male, new PBEAbility[] { PBEAbility.FlameBody }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.QuiverDance, PBEMove.BugBuzz, PBEMove.FieryDance, PBEMove.MorningSun }
				),
				new PBEDDEventPokemon // Alder's Volcarona
				(
					new byte[] { 5 }, PBESpecies.Volcarona, 77, false, PBEGender.Male, new PBEAbility[] { PBEAbility.FlameBody }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.BugBuzz, PBEMove.Overheat, PBEMove.HyperBeam, PBEMove.QuiverDance }
				)
			})
		},
		{
			PBESpecies.Vulpix,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Pokémon Trade and Battle Day
				(
					new byte[] { 3 }, PBESpecies.Vulpix, 18, false, PBEGender.Female, new PBEAbility[] { PBEAbility.FlashFire }, new PBENature[] { PBENature.Quirky },
					new byte?[] { 15, 6, 3, 25, 13, 22 }, new PBEMove[] { PBEMove.TailWhip, PBEMove.Roar, PBEMove.QuickAttack, PBEMove.WillOWisp }
				)
			})
		},
		{
			PBESpecies.Whimsicott,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Powerful Tag
				(
					new byte[] { 5 }, PBESpecies.Whimsicott, 50, false, PBEGender.Female, new PBEAbility[] { PBEAbility.Prankster }, new PBENature[] { PBENature.Timid },
					new byte?[] { null, null, null, null, null, 31 }, new PBEMove[] { PBEMove.Swagger, PBEMove.GigaDrain, PBEMove.BeatUp, PBEMove.HelpingHand }
				)
			})
		},
		{
			PBESpecies.Zekrom,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // Victini movies promotion
				(
					new byte[] { 5 }, PBESpecies.Zekrom, 100, false, PBEGender.Genderless, new PBEAbility[] { PBEAbility.Teravolt }, PBEDataUtils.AllNatures,
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.BoltStrike, PBEMove.FusionBolt, PBEMove.Haze, PBEMove.Outrage }
				)
			})
		},
		{
			PBESpecies.Zoroark,
			new ReadOnlyCollection<PBEDDEventPokemon>(new[]
			{
				new PBEDDEventPokemon // 2011
				(
					new byte[] { 5 }, PBESpecies.Zoroark, 50, false, PBEGender.Male, new PBEAbility[] { PBEAbility.Illusion }, new PBENature[] { PBENature.Quirky },
					new byte?[] { null, null, null, null, null, null }, new PBEMove[] { PBEMove.Agility, PBEMove.Embargo, PBEMove.Punishment, PBEMove.Snarl }
				)
			})
		}
	});
}
