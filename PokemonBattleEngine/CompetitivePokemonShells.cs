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
            Charizard_VGC = new PPokemonShell
            {
                Species = PSpecies.Charizard,
                Nickname = "Chomp",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.ChoiceScarf,
                Ability = PAbility.SolarPower,
                Gender = PGender.Male,
                Nature = PNature.Modest,
                IVs = new byte[] { 31, 31, 31, 31, 31, 30 }, // Hidden Power: Ice/70
                EVs = new byte[] { 0, 0, 0, 252, 4, 252 },
                Moves = new PMove[] { PMove.FireBlast, PMove.AirSlash, PMove.FocusBlast, PMove.HiddenPower },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Blastoise_UU = new PPokemonShell
            {
                Species = PSpecies.Blastoise,
                Nickname = "Boost",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.Leftovers,
                Ability = PAbility.RainDish,
                Gender = PGender.Male,
                Nature = PNature.Relaxed,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 252, 0, 4, 0 },
                Moves = new PMove[] { PMove.Substitute, PMove.Scald, PMove.Protect, PMove.RainDance }, // Rapid Spin, Scald, Roar, Foresight
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Butterfree_RU = new PPokemonShell
            {
                Species = PSpecies.Butterfree,
                Nickname = "Fluff",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.Leftovers,
                Ability = PAbility.Compoundeyes,
                Gender = PGender.Female,
                Nature = PNature.Timid,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 0, 0, 0, 252, 4, 252 },
                Moves = new PMove[] { PMove.SleepPowder, PMove.QuiverDance, PMove.BugBuzz, PMove.Substitute },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Beedrill_NU = new PPokemonShell
            {
                Species = PSpecies.Beedrill,
                Nickname = "Citizen",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.ChoiceScarf, // Focus Sash
                Ability = PAbility.Swarm,
                Gender = PGender.Male,
                Nature = PNature.Jolly,
                IVs = new byte[] { 0, 31, 0, 31, 0, 31 }, // Hidden Power: Ghost/46
                EVs = new byte[] { 0, 252, 0, 0, 0, 252 },
                Moves = new PMove[] { PMove.ToxicSpikes, PMove.Endeavor, PMove.XScissor, PMove.Agility }, // Toxic Spikes, Endeavor, U-Turn, Tailwind
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
            Ninetales_VGC = new PPokemonShell
            {
                Species = PSpecies.Ninetales,
                Nickname = "Jasmine",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.FlamePlate, // Focus Sash
                Ability = PAbility.Drought,
                Gender = PGender.Female,
                Nature = PNature.Modest,
                IVs = new byte[] { 31, 31, 31, 31, 31, 30 }, // Hidden Power: Ice/70
                EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
                Moves = new PMove[] { PMove.Flamethrower, PMove.HiddenPower, PMove.EnergyBall, PMove.Protect },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Farfetchd_OU = new PPokemonShell
            {
                Species = PSpecies.Farfetchd,
                Nickname = "Farquaad",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.Stick,
                Ability = PAbility.Defiant,
                Gender = PGender.Male,
                Nature = PNature.Adamant,
                IVs = new byte[] { 31, 31, 31, 31, 31, 30 }, // Hidden Power: Ice/70
                EVs = new byte[] { 4, 252, 0, 0, 0, 252 },
                Moves = new PMove[] { PMove.SwordsDance, PMove.Return, PMove.AirSlash, PMove.SteelWing }, // Swords Dance, Return, Brave Bird, Revenge
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
            Cradily_OU = new PPokemonShell
            {
                Species = PSpecies.Cradily,
                Nickname = "PLANT",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.Leftovers,
                Ability = PAbility.SuctionCups,
                Gender = PGender.Male,
                Nature = PNature.Careful,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 80, 0, 176, 0 },
                Moves = new PMove[] { PMove.Curse, PMove.RockSlide, PMove.StealthRock, PMove.SeedBomb }, // Curse, Rest, Sleep Talk, Rock Slide
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
                Moves = new PMove[] { PMove.DragonPulse, PMove.IcyWind, PMove.DoubleTeam, PMove.Recover }, // Dragon Pulse, Icy Wind, Helping Hand, Recover
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
            Jirachi_Uber = new PPokemonShell
            {
                Species = PSpecies.Jirachi,
                Nickname = "Gina",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.Leftovers,
                Ability = PAbility.SereneGrace,
                Gender = PGender.Genderless,
                Nature = PNature.Careful,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 4, 0, 252, 0 },
                Moves = new PMove[] { PMove.Protect, PMove.StealthRock, PMove.IronHead, PMove.Thunder }, // Wish, Stealth Rock, Iron Head, Roar
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Mismagius_UU = new PPokemonShell
            {
                Species = PSpecies.Mismagius,
                Nickname = "Kermie",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.LifeOrb,
                Ability = PAbility.Levitate,
                Gender = PGender.Female,
                Nature = PNature.Timid,
                IVs = new byte[] { 31, 31, 30, 30, 30, 30 }, // Hidden Power: Fighting/70
                EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
                Moves = new PMove[] { PMove.NastyPlot, PMove.ShadowBall, PMove.HiddenPower, PMove.Thunderbolt },
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
            Uxie_VGC = new PPokemonShell
            {
                Species = PSpecies.Uxie,
                Nickname = "Amy",
                Level = 100,
                Friendship = 255,
                Item = PItem.Leftovers, // Sitrus Berry
                Shiny = false,
                Ability = PAbility.Levitate,
                Gender = PGender.Genderless,
                Nature = PNature.Calm,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 0, 4, 252, 0 },
                Moves = new PMove[] { PMove.CalmMind, PMove.LightScreen, PMove.Psychic, PMove.Protect }, // Imprison, Trick Room, Psychic, Protect
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Mesprit_UU = new PPokemonShell
            {
                Species = PSpecies.Mesprit,
                Nickname = "Cindy",
                Level = 100,
                Friendship = 255,
                Item = PItem.LifeOrb,
                Shiny = false,
                Ability = PAbility.Levitate,
                Gender = PGender.Genderless,
                Nature = PNature.Quiet,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 4, 252, 0, 0 },
                Moves = new PMove[] { PMove.CalmMind, PMove.Thunderbolt, PMove.Psychic, PMove.HiddenPower }, // Thunderbolt, Trick Room, Psychic, U-turn
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Dialga_Uber = new PPokemonShell
            {
                Species = PSpecies.Dialga,
                Nickname = "Seaweed",
                Level = 100,
                Friendship = 255,
                Item = PItem.Leftovers,
                Shiny = false,
                Ability = PAbility.Pressure,
                Gender = PGender.Genderless,
                Nature = PNature.Modest,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 0, 56, 200, 0 },
                Moves = new PMove[] { PMove.StealthRock, PMove.DragonPulse, PMove.FireBlast, PMove.Thunder },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Palkia_Uber = new PPokemonShell
            {
                Species = PSpecies.Palkia,
                Nickname = "Pokey",
                Level = 100,
                Friendship = 255,
                Item = PItem.LustrousOrb,
                Shiny = false,
                Ability = PAbility.Pressure,
                Gender = PGender.Genderless,
                Nature = PNature.Timid,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
                Moves = new PMove[] { PMove.SpacialRend, PMove.Surf, PMove.FireBlast, PMove.Thunder },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Giratina_Origin_Uber = new PPokemonShell
            {
                Species = PSpecies.Giratina_Origin,
                Nickname = "Gary",
                Level = 100,
                Friendship = 255,
                Item = PItem.GriseousOrb,
                Shiny = false,
                Ability = PAbility.Levitate,
                Gender = PGender.Genderless,
                Nature = PNature.Adamant,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 164, 252, 8, 0, 0, 84 },
                Moves = new PMove[] { PMove.DragonClaw, PMove.ShadowSneak, PMove.Earthquake, PMove.Substitute }, // Dragon Tail, Shadow Sneak, Earthquake, Magic Coat
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
            Arceus_Normal_Uber = new PPokemonShell
            {
                Species = PSpecies.Arceus_Normal,
                Nickname = "Exon",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.SilkScarf,
                Ability = PAbility.Multitype,
                Gender = PGender.Genderless,
                Nature = PNature.Adamant,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 200, 252, 0, 0, 0, 56 },
                Moves = new PMove[] { PMove.SwordsDance, PMove.ExtremeSpeed, PMove.ShadowClaw, PMove.BrickBreak },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Victini_Uber = new PPokemonShell
            {
                Species = PSpecies.Victini,
                Nickname = "Vicki", // ビクティニ (this is an event Victini)
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.ChoiceScarf,
                Ability = PAbility.VictoryStar,
                Gender = PGender.Genderless,
                Nature = PNature.Naive,
                IVs = new byte[] { 31, 31, 31, 31, 31, 30 }, // Hidden Power: Ice/70
                EVs = new byte[] { 0, 252, 4, 0, 0, 252 },
                Moves = new PMove[] { PMove.VCreate, PMove.BoltStrike, PMove.GrassKnot, PMove.HiddenPower }, // V-create, Bolt Strike, U-turn, Hidden Power Ice
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
