using Kermalis.PokemonBattleEngine.Data;

namespace Kermalis.PokemonBattleEngine
{
    public static class PBECompetitivePokemonShells
    {
        public static readonly PBEPokemonShell
            Venusaur_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Venusaur,
                Nickname = "Viola",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.LifeOrb,
                Ability = PBEAbility.Chlorophyll,
                Gender = PBEGender.Female,
                Nature = PBENature.Modest,
                IVs = new byte[] { 31, 30, 30, 31, 31, 31 }, // Hidden Power: Ice/70
                EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
                Moves = new PBEMove[] { PBEMove.SleepPowder, PBEMove.EnergyBall, PBEMove.HiddenPower, PBEMove.Protect },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Charizard_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Charizard,
                Nickname = "Chomp",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.ChoiceScarf,
                Ability = PBEAbility.SolarPower,
                Gender = PBEGender.Male,
                Nature = PBENature.Modest,
                IVs = new byte[] { 31, 30, 30, 31, 31, 31 }, // Hidden Power: Ice/70
                EVs = new byte[] { 0, 0, 0, 252, 4, 252 },
                Moves = new PBEMove[] { PBEMove.FireBlast, PBEMove.AirSlash, PBEMove.FocusBlast, PBEMove.HiddenPower },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Blastoise_UU = new PBEPokemonShell
            {
                Species = PBESpecies.Blastoise,
                Nickname = "Boost",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.Leftovers,
                Ability = PBEAbility.RainDish,
                Gender = PBEGender.Male,
                Nature = PBENature.Relaxed,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 252, 0, 4, 0 },
                Moves = new PBEMove[] { PBEMove.Substitute, PBEMove.Scald, PBEMove.Protect, PBEMove.RainDance }, // Rapid Spin, Scald, Roar, Foresight
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Butterfree_RU = new PBEPokemonShell
            {
                Species = PBESpecies.Butterfree,
                Nickname = "Fluff",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.Leftovers,
                Ability = PBEAbility.Compoundeyes,
                Gender = PBEGender.Female,
                Nature = PBENature.Timid,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 0, 0, 0, 252, 4, 252 },
                Moves = new PBEMove[] { PBEMove.SleepPowder, PBEMove.QuiverDance, PBEMove.BugBuzz, PBEMove.Substitute },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Beedrill_NU = new PBEPokemonShell
            {
                Species = PBESpecies.Beedrill,
                Nickname = "Citizen",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.ChoiceScarf, // Focus Sash
                Ability = PBEAbility.Swarm,
                Gender = PBEGender.Male,
                Nature = PBENature.Jolly,
                IVs = new byte[] { 0, 31, 0, 31, 0, 31 }, // Hidden Power: Ghost/46
                EVs = new byte[] { 0, 252, 0, 0, 0, 252 },
                Moves = new PBEMove[] { PBEMove.ToxicSpikes, PBEMove.Endeavor, PBEMove.XScissor, PBEMove.Agility }, // Toxic Spikes, Endeavor, U-Turn, Tailwind
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Pikachu_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Pikachu,
                Nickname = "Poof",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.LightBall,
                Ability = PBEAbility.LightningRod,
                Gender = PBEGender.Female,
                Nature = PBENature.Rash,
                IVs = new byte[] { 31, 30, 30, 31, 31, 31 }, // Hidden Power: Ice/70
                EVs = new byte[] { 0, 4, 0, 252, 0, 252 },
                Moves = new PBEMove[] { PBEMove.Substitute, PBEMove.Thunderbolt, PBEMove.HiddenPower, PBEMove.GrassKnot }, // Fake Out, Thunderbolt, Hidden Power Ice, Grass Knot
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Ninetales_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Ninetales,
                Nickname = "Jasmine",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.FlamePlate, // Focus Sash
                Ability = PBEAbility.Drought,
                Gender = PBEGender.Female,
                Nature = PBENature.Modest,
                IVs = new byte[] { 31, 30, 30, 31, 31, 31 }, // Hidden Power: Ice/70
                EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
                Moves = new PBEMove[] { PBEMove.Flamethrower, PBEMove.HiddenPower, PBEMove.EnergyBall, PBEMove.Protect },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Farfetchd_OU = new PBEPokemonShell
            {
                Species = PBESpecies.Farfetchd,
                Nickname = "Farquaad",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.Stick,
                Ability = PBEAbility.Defiant,
                Gender = PBEGender.Male,
                Nature = PBENature.Adamant,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 4, 252, 0, 0, 0, 252 },
                Moves = new PBEMove[] { PBEMove.SwordsDance, PBEMove.Return, PBEMove.AirSlash, PBEMove.SteelWing }, // Swords Dance, Return, Brave Bird, Revenge
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Marowak_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Marowak,
                Nickname = "Mawoogo",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.ThickClub,
                Ability = PBEAbility.LightningRod,
                Gender = PBEGender.Male,
                Nature = PBENature.Brave,
                IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
                EVs = new byte[] { 252, 252, 4, 0, 0, 0 },
                Moves = new PBEMove[] { PBEMove.Earthquake, PBEMove.RockSlide, PBEMove.FirePunch, PBEMove.SwordsDance },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Ditto_Uber = new PBEPokemonShell
            {
                Species = PBESpecies.Ditto,
                Nickname = "Doot",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.ChoiceScarf,
                Ability = PBEAbility.Imposter,
                Gender = PBEGender.Genderless,
                Nature = PBENature.Relaxed,
                IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
                EVs = new byte[] { 248, 0, 252, 0, 8, 0 },
                Moves = new PBEMove[] { PBEMove.Transform, PBEMove.None, PBEMove.None, PBEMove.None },
                PPUps = new byte[] { 3, 0, 0, 0 }
            },
            Vaporeon_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Vaporeon,
                Nickname = "Pam",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.SplashPlate, // Water Gem
                Ability = PBEAbility.WaterAbsorb,
                Gender = PBEGender.Female,
                Nature = PBENature.Quiet,
                IVs = new byte[] { 31, 30, 31, 30, 31, 3 }, // Hidden Power: Grass/70
                EVs = new byte[] { 140, 0, 116, 248, 4, 0 },
                Moves = new PBEMove[] { PBEMove.MuddyWater, PBEMove.IceBeam, PBEMove.HiddenPower, PBEMove.Detect },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Jolteon_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Jolteon,
                Nickname = "Jilly",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.ZapPlate, // Focus Sash
                Ability = PBEAbility.VoltAbsorb,
                Gender = PBEGender.Female,
                Nature = PBENature.Timid,
                IVs = new byte[] { 31, 30, 30, 31, 31, 31 }, // Hidden Power: Ice/70
                EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
                Moves = new PBEMove[] { PBEMove.Thunderbolt, PBEMove.ThunderWave, PBEMove.HiddenPower, PBEMove.Protect },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Flareon_RU = new PBEPokemonShell
            {
                Species = PBESpecies.Flareon,
                Nickname = "Fugu",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.Leftovers,
                Ability = PBEAbility.FlashFire,
                Gender = PBEGender.Male,
                Nature = PBENature.Calm,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 4, 0, 252, 0 },
                Moves = new PBEMove[] { PBEMove.HyperVoice, PBEMove.Protect, PBEMove.LavaPlume, PBEMove.Toxic }, // Wish, Protect, Lava Plume, Toxic
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Crobat_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Crobat,
                Nickname = "Caro",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.BlackSludge, // Flying Gem
                Ability = PBEAbility.InnerFocus,
                Gender = PBEGender.Female,
                Nature = PBENature.Jolly,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power Dark/70
                EVs = new byte[] { 4, 252, 0, 0, 0, 252, },
                Moves = new PBEMove[] { PBEMove.Facade, PBEMove.NastyPlot, PBEMove.AirSlash, PBEMove.Protect }, // Acrobatics, Taunt, Super Fang, Protect
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Azumarill_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Azumarill,
                Nickname = "ZuWEEE",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.ChoiceBand,
                Ability = PBEAbility.HugePower,
                Gender = PBEGender.Male,
                Nature = PBENature.Adamant,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 164, 252, 0, 0, 0, 92 },
                Moves = new PBEMove[] { PBEMove.Waterfall, PBEMove.AquaJet, PBEMove.Superpower, PBEMove.IcePunch },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Espeon_Uber = new PBEPokemonShell
            {
                Species = PBESpecies.Espeon,
                Nickname = "Vaqui",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.MeadowPlate, // Focus Sash
                Ability = PBEAbility.MagicBounce,
                Gender = PBEGender.Female,
                Nature = PBENature.Timid,
                IVs = new byte[] { 31, 31, 30, 30, 31, 30 }, // Hidden Power: Fire/70
                EVs = new byte[] { 0, 0, 4, 252, 0, 252 },
                Moves = new PBEMove[] { PBEMove.ShadowBall, PBEMove.Psyshock, PBEMove.GrassKnot, PBEMove.HiddenPower }, // Shadow Ball, Yawn, Grass Knot, Hidden Power Fire
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Cradily_OU = new PBEPokemonShell
            {
                Species = PBESpecies.Cradily,
                Nickname = "PLANT",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.Leftovers,
                Ability = PBEAbility.SuctionCups,
                Gender = PBEGender.Male,
                Nature = PBENature.Careful,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 80, 0, 176, 0 },
                Moves = new PBEMove[] { PBEMove.Curse, PBEMove.RockSlide, PBEMove.StealthRock, PBEMove.SeedBomb }, // Curse, Rest, Sleep Talk, Rock Slide
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Absol_RU = new PBEPokemonShell
            {
                Species = PBESpecies.Absol,
                Nickname = "Absoul",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.DreadPlate,
                Ability = PBEAbility.SuperLuck,
                Gender = PBEGender.Male,
                Nature = PBENature.Adamant,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 0, 252, 4, 0, 0, 252 },
                Moves = new PBEMove[] { PBEMove.SwordsDance, PBEMove.RockSlide, PBEMove.NightSlash, PBEMove.Superpower }, // Sucker Punch, Superpower, Night Slash, Pursuit
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Latias_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Latias,
                Nickname = "Lani",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.LifeOrb, // Yache Berry
                Ability = PBEAbility.Levitate,
                Gender = PBEGender.Female,
                Nature = PBENature.Bold,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 252, 0, 0, 4 },
                Moves = new PBEMove[] { PBEMove.DragonPulse, PBEMove.IcyWind, PBEMove.DoubleTeam, PBEMove.Recover }, // Dragon Pulse, Icy Wind, Helping Hand, Recover
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Latios_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Latios,
                Nickname = "Loomo",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.LifeOrb,
                Ability = PBEAbility.Levitate,
                Gender = PBEGender.Male,
                Nature = PBENature.Timid,
                IVs = new byte[] { 31, 30, 31, 30, 31, 30 }, // Hidden Power: Fire/70
                EVs = new byte[] { 4, 0, 4, 248, 4, 248 },
                Moves = new PBEMove[] { PBEMove.DracoMeteor, PBEMove.Psychic, PBEMove.Protect, PBEMove.HiddenPower },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Jirachi_Uber = new PBEPokemonShell
            {
                Species = PBESpecies.Jirachi,
                Nickname = "Gina",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.Leftovers,
                Ability = PBEAbility.SereneGrace,
                Gender = PBEGender.Genderless,
                Nature = PBENature.Careful,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 4, 0, 252, 0 },
                Moves = new PBEMove[] { PBEMove.Protect, PBEMove.StealthRock, PBEMove.IronHead, PBEMove.Thunder }, // Wish, Stealth Rock, Iron Head, Roar
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Mismagius_UU = new PBEPokemonShell
            {
                Species = PBESpecies.Mismagius,
                Nickname = "Kermie",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.LifeOrb,
                Ability = PBEAbility.Levitate,
                Gender = PBEGender.Female,
                Nature = PBENature.Timid,
                IVs = new byte[] { 31, 31, 30, 30, 30, 30 }, // Hidden Power: Fighting/70
                EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
                Moves = new PBEMove[] { PBEMove.NastyPlot, PBEMove.ShadowBall, PBEMove.HiddenPower, PBEMove.Thunderbolt },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Rotom_Wash_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Rotom_Wash,
                Nickname = "Wahooey",
                Level = 100,
                Friendship = 255,
                Item = PBEItem.Leftovers, // Sitrus Berry
                Shiny = false,
                Ability = PBEAbility.Levitate,
                Gender = PBEGender.Genderless,
                Nature = PBENature.Modest,
                IVs = new byte[] { 31, 30, 31, 30, 31, 30 }, // Hidden Power: Fire/70
                EVs = new byte[] { 232, 0, 0, 56, 0, 220 },
                Moves = new PBEMove[] { PBEMove.HydroPump, PBEMove.HiddenPower, PBEMove.Discharge, PBEMove.Protect },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Uxie_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Uxie,
                Nickname = "Amy",
                Level = 100,
                Friendship = 255,
                Item = PBEItem.Leftovers, // Sitrus Berry
                Shiny = false,
                Ability = PBEAbility.Levitate,
                Gender = PBEGender.Genderless,
                Nature = PBENature.Calm,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 0, 4, 252, 0 },
                Moves = new PBEMove[] { PBEMove.CalmMind, PBEMove.LightScreen, PBEMove.Psychic, PBEMove.Protect }, // Imprison, Trick Room, Psychic, Protect
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Mesprit_UU = new PBEPokemonShell
            {
                Species = PBESpecies.Mesprit,
                Nickname = "Cindy",
                Level = 100,
                Friendship = 255,
                Item = PBEItem.LifeOrb,
                Shiny = false,
                Ability = PBEAbility.Levitate,
                Gender = PBEGender.Genderless,
                Nature = PBENature.Quiet,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 4, 252, 0, 0 },
                Moves = new PBEMove[] { PBEMove.CalmMind, PBEMove.Thunderbolt, PBEMove.Psychic, PBEMove.HiddenPower }, // Thunderbolt, Trick Room, Psychic, U-turn
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Azelf_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Azelf,
                Nickname = "Zinc",
                Level = 100,
                Friendship = 255,
                Item = PBEItem.LifeOrb,
                Shiny = false,
                Ability = PBEAbility.Levitate,
                Gender = PBEGender.Genderless,
                Nature = PBENature.Jolly,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 4, 252, 0, 0, 0, 252 },
                Moves = new PBEMove[] { PBEMove.ZenHeadbutt, PBEMove.IcePunch, PBEMove.FirePunch, PBEMove.Detect }, // Zen Headbutt, Ice Punch, Explosion, Detect
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Dialga_Uber = new PBEPokemonShell
            {
                Species = PBESpecies.Dialga,
                Nickname = "Seaweed",
                Level = 100,
                Friendship = 255,
                Item = PBEItem.Leftovers,
                Shiny = false,
                Ability = PBEAbility.Pressure,
                Gender = PBEGender.Genderless,
                Nature = PBENature.Modest,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 0, 56, 200, 0 },
                Moves = new PBEMove[] { PBEMove.StealthRock, PBEMove.DragonPulse, PBEMove.FireBlast, PBEMove.Thunder },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Palkia_Uber = new PBEPokemonShell
            {
                Species = PBESpecies.Palkia,
                Nickname = "Pokey",
                Level = 100,
                Friendship = 255,
                Item = PBEItem.LustrousOrb,
                Shiny = false,
                Ability = PBEAbility.Pressure,
                Gender = PBEGender.Genderless,
                Nature = PBENature.Timid,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
                Moves = new PBEMove[] { PBEMove.SpacialRend, PBEMove.Surf, PBEMove.FireBlast, PBEMove.Thunder },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Giratina_Origin_Uber = new PBEPokemonShell
            {
                Species = PBESpecies.Giratina_Origin,
                Nickname = "Gary",
                Level = 100,
                Friendship = 255,
                Item = PBEItem.GriseousOrb,
                Shiny = false,
                Ability = PBEAbility.Levitate,
                Gender = PBEGender.Genderless,
                Nature = PBENature.Adamant,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 164, 252, 8, 0, 0, 84 },
                Moves = new PBEMove[] { PBEMove.DragonClaw, PBEMove.ShadowSneak, PBEMove.Earthquake, PBEMove.Substitute }, // Dragon Tail, Shadow Sneak, Earthquake, Magic Coat
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Cresselia_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Cresselia,
                Nickname = "Crest",
                Level = 100,
                Friendship = 255,
                Item = PBEItem.Leftovers, // Sitrus Berry
                Shiny = false,
                Ability = PBEAbility.Levitate,
                Gender = PBEGender.Female,
                Nature = PBENature.Relaxed,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 60, 0, 196, 0 },
                Moves = new PBEMove[] { PBEMove.Psychic, PBEMove.LightScreen, PBEMove.IcyWind, PBEMove.Toxic }, // Psychic, Trick Room, Icy Wind, Safeguard
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Darkrai_Uber = new PBEPokemonShell
            {
                Species = PBESpecies.Darkrai,
                Nickname = "Kylo",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.Leftovers,
                Ability = PBEAbility.BadDreams,
                Gender = PBEGender.Genderless,
                Nature = PBENature.Timid,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
                Moves = new PBEMove[] { PBEMove.DarkVoid, PBEMove.DarkPulse, PBEMove.NastyPlot, PBEMove.Substitute },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Arceus_Normal_Uber = new PBEPokemonShell
            {
                Species = PBESpecies.Arceus_Normal,
                Nickname = "Exon",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.SilkScarf,
                Ability = PBEAbility.Multitype,
                Gender = PBEGender.Genderless,
                Nature = PBENature.Adamant,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 200, 252, 0, 0, 0, 56 },
                Moves = new PBEMove[] { PBEMove.SwordsDance, PBEMove.ExtremeSpeed, PBEMove.ShadowClaw, PBEMove.BrickBreak },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Victini_Uber = new PBEPokemonShell
            {
                Species = PBESpecies.Victini,
                Nickname = "Vicki", // ビクティニ (this is an event Victini)
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.ChoiceScarf,
                Ability = PBEAbility.VictoryStar,
                Gender = PBEGender.Genderless,
                Nature = PBENature.Naive,
                IVs = new byte[] { 30, 31, 30, 31, 31, 31 }, // Hidden Power: Ice/70
                EVs = new byte[] { 0, 252, 4, 0, 0, 252 },
                Moves = new PBEMove[] { PBEMove.VCreate, PBEMove.BoltStrike, PBEMove.GrassKnot, PBEMove.HiddenPower }, // V-create, Bolt Strike, U-turn, Hidden Power Ice
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Cofagrigus_VGC = new PBEPokemonShell
            {
                Species = PBESpecies.Cofagrigus,
                Nickname = "Coffee",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.Leftovers,
                Ability = PBEAbility.Mummy,
                Gender = PBEGender.Male,
                Nature = PBENature.Sassy,
                IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
                EVs = new byte[] { 248, 0, 8, 252, 0, 0 },
                Moves = new PBEMove[] { PBEMove.ShadowBall, PBEMove.Protect, PBEMove.WillOWisp, PBEMove.Curse }, // Shadow Ball, Will-O-Wisp, Protect, Trick Room
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Genesect_Uber = new PBEPokemonShell
            {
                Species = PBESpecies.Genesect,
                Nickname = "Jessenect",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PBEItem.ChoiceScarf,
                Ability = PBEAbility.Download,
                Gender = PBEGender.Genderless,
                Nature = PBENature.Hasty,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 0, 252, 0, 4, 0, 252 },
                Moves = new PBEMove[] { PBEMove.BugBuzz, PBEMove.IceBeam, PBEMove.Flamethrower, PBEMove.IronHead }, // U-Turn, Ice Beam, Flamethrower, Iron Head
                PPUps = new byte[] { 3, 3, 3, 3 }
            };
    }
}
