using Kermalis.PokemonBattleEngine.Data;

namespace Kermalis.PokemonBattleEngine
{
    public static class PCompetitivePokemonShells
    {
        public static readonly PPokemonShell
            Venusaur_VGC = new PPokemonShell
            {
                Species = PSpecies.Venusaur,
                Nickname = "Viola",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.LifeOrb,
                Ability = PAbility.Chlorophyll,
                Gender = PGender.Female,
                Nature = PNature.Modest,
                IVs = new byte[] { 31, 31, 31, 31, 31, 30 }, // Hidden Power: Ice/70
                EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
                Moves = new PMove[] { PMove.SleepPowder, PMove.EnergyBall, PMove.HiddenPower, PMove.Protect },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Pikachu_VGC = new PPokemonShell
            {
                Species = PSpecies.Pikachu,
                Nickname = "Poof",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.LightBall,
                Ability = PAbility.LightningRod,
                Gender = PGender.Female,
                Nature = PNature.Rash,
                IVs = new byte[] { 31, 31, 31, 31, 31, 30 }, // Hidden Power: Ice/70
                EVs = new byte[] { 0, 4, 0, 252, 0, 252 },
                Moves = new PMove[] { PMove.Substitute, PMove.Thunderbolt, PMove.HiddenPower, PMove.GrassKnot }, // Fake Out, Thunderbolt, Hidden Power Ice, Grass Knot
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Marowak_VGC = new PPokemonShell
            {
                Species = PSpecies.Marowak,
                Nickname = "Mawoogo",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.ThickClub,
                Ability = PAbility.LightningRod,
                Gender = PGender.Male,
                Nature = PNature.Brave,
                IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
                EVs = new byte[] { 252, 252, 4, 0, 0, 0 },
                Moves = new PMove[] { PMove.Earthquake, PMove.RockSlide, PMove.FirePunch, PMove.SwordsDance },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Ditto_Uber = new PPokemonShell
            {
                Species = PSpecies.Ditto,
                Nickname = "Doot",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.ChoiceScarf,
                Ability = PAbility.Imposter,
                Gender = PGender.Genderless,
                Nature = PNature.Relaxed,
                IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
                EVs = new byte[] { 248, 0, 252, 0, 8, 0 },
                Moves = new PMove[] { PMove.Transform, PMove.None, PMove.None, PMove.None },
                PPUps = new byte[] { 3, 0, 0, 0 }
            },
            Crobat_VGC = new PPokemonShell
            {
                Species = PSpecies.Crobat,
                Nickname = "Caro",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.BlackSludge, // Flying Gem
                Ability = PAbility.InnerFocus,
                Gender = PGender.Female,
                Nature = PNature.Jolly,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power Dark/70
                EVs = new byte[] { 4, 252, 0, 0, 0, 252, },
                Moves = new PMove[] { PMove.Facade, PMove.NastyPlot, PMove.AirSlash, PMove.Protect }, // Acrobatics, Taunt, Super Fang, Protect
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Azumarill_VGC = new PPokemonShell
            {
                Species = PSpecies.Azumarill,
                Nickname = "ZuWEEE",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.ChoiceBand,
                Ability = PAbility.HugePower,
                Gender = PGender.Male,
                Nature = PNature.Adamant,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 164, 252, 0, 0, 0, 92 },
                Moves = new PMove[] { PMove.Waterfall, PMove.AquaJet, PMove.Superpower, PMove.IcePunch },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Absol_RU = new PPokemonShell
            {
                Species = PSpecies.Absol,
                Nickname = "Absoul",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.DreadPlate,
                Ability = PAbility.SuperLuck,
                Gender = PGender.Male,
                Nature = PNature.Adamant,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 0, 252, 4, 0, 0, 252 },
                Moves = new PMove[] { PMove.SwordsDance, PMove.RockSlide, PMove.NightSlash, PMove.Superpower }, // Sucker Punch, Superpower, Night Slash, Pursuit
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Latias_VGC = new PPokemonShell
            {
                Species = PSpecies.Latias,
                Nickname = "Lani",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.LifeOrb, // Yache Berry
                Ability = PAbility.Levitate,
                Gender = PGender.Female,
                Nature = PNature.Bold,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 252, 0, 0, 4 },
                Moves = new PMove[] { PMove.DragonPulse, PMove.IcyWind, PMove.NastyPlot, PMove.PsychUp }, // Dragon Pulse, Icy Wind, Helping Hand, Recover
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Latios_VGC = new PPokemonShell
            {
                Species = PSpecies.Latios,
                Nickname = "Loomo",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.LifeOrb,
                Ability = PAbility.Levitate,
                Gender = PGender.Male,
                Nature = PNature.Timid,
                IVs = new byte[] { 31, 30, 31, 30, 31, 30 }, // Hidden Power: Fire/70
                EVs = new byte[] { 4, 0, 4, 248, 4, 248 },
                Moves = new PMove[] { PMove.DracoMeteor, PMove.Psychic, PMove.Protect, PMove.HiddenPower },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Rotom_Wash_VGC = new PPokemonShell
            {
                Species = PSpecies.Rotom_Wash,
                Nickname = "Wahooey",
                Level = 100,
                Friendship = 255,
                Item = PItem.Leftovers, // Sitrus Berry
                Shiny = false,
                Ability = PAbility.Levitate,
                Gender = PGender.Genderless,
                Nature = PNature.Modest,
                IVs = new byte[] { 31, 30, 31, 30, 31, 30 }, // Hidden Power: Fire/70
                EVs = new byte[] { 232, 0, 0, 56, 0, 220 },
                Moves = new PMove[] { PMove.HydroPump, PMove.HiddenPower, PMove.Discharge, PMove.Protect },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Cresselia_VGC = new PPokemonShell
            {
                Species = PSpecies.Cresselia,
                Nickname = "Crest",
                Level = 100,
                Friendship = 255,
                Item = PItem.Leftovers, // Sitrus Berry
                Shiny = false,
                Ability = PAbility.Levitate,
                Gender = PGender.Female,
                Nature = PNature.Relaxed,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 60, 0, 196, 0 },
                Moves = new PMove[] { PMove.Psychic, PMove.LightScreen, PMove.IcyWind, PMove.Toxic }, // Psychic, Trick Room, Icy Wind, Safeguard
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Darkrai_Uber = new PPokemonShell
            {
                Species = PSpecies.Darkrai,
                Nickname = "Kylo",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.Leftovers,
                Ability = PAbility.BadDreams,
                Gender = PGender.Genderless,
                Nature = PNature.Timid,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
                Moves = new PMove[] { PMove.DarkVoid, PMove.DarkPulse, PMove.NastyPlot, PMove.Substitute },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Cofagrigus_VGC = new PPokemonShell
            {
                Species = PSpecies.Cofagrigus,
                Nickname = "Coffee",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.Leftovers,
                Ability = PAbility.Mummy,
                Gender = PGender.Male,
                Nature = PNature.Sassy,
                IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
                EVs = new byte[] { 248, 0, 8, 252, 0, 0 },
                Moves = new PMove[] { PMove.ShadowBall, PMove.Protect, PMove.WillOWisp, PMove.Curse }, // Shadow Ball, Will-O-Wisp, Protect, Trick Room
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Genesect_Uber = new PPokemonShell
            {
                Species = PSpecies.Genesect,
                Nickname = "Jessenect",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.ChoiceScarf,
                Ability = PAbility.Download,
                Gender = PGender.Genderless,
                Nature = PNature.Hasty,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 0, 252, 0, 4, 0, 252 },
                Moves = new PMove[] { PMove.BugBuzz, PMove.IceBeam, PMove.Flamethrower, PMove.IronHead }, // U-Turn, Ice Beam, Flamethrower, Iron Head
                PPUps = new byte[] { 3, 3, 3, 3 }
            };
    }
}
