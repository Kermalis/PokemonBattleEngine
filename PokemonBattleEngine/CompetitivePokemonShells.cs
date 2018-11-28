using Kermalis.PokemonBattleEngine.Data;

namespace Kermalis.PokemonBattleEngine
{
    public static class PCompetitivePokemonShells
    {
        public static readonly PPokemonShell
            Pikachu_NU = new PPokemonShell
            {
                Species = PSpecies.Pikachu,
                Nickname = "Poof",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.LightBall,
                Ability = PAbility.LightningRod,
                Gender = PGender.Female,
                Nature = PNature.Timid,
                IVs = new byte[] { 31, 31, 31, 31, 31, 30 }, // Hidden Power: Ice/70
                EVs = new byte[] { 0, 0, 4, 252, 0, 252 },
                Moves = new PMove[] { PMove.Substitute, PMove.Thunderbolt, PMove.HiddenPower, PMove.GrassKnot },
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
            Ditto_UU = new PPokemonShell
            {
                Species = PSpecies.Ditto,
                Nickname = "Doot",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.ChoiceScarf,
                Ability = PAbility.Imposter,
                Gender = PGender.Genderless,
                Nature = PNature.Sassy,
                IVs = new byte[] { 31, 31, 31, 31, 31, 30 }, // Hidden Power: Ice/70
                EVs = new byte[] { 252, 0, 4, 0, 0, 252 },
                Moves = new PMove[] { PMove.Transform, PMove.None, PMove.None, PMove.None },
                PPUps = new byte[] { 3, 0, 0, 0 }
            },
            Crobat_OU = new PPokemonShell
            {
                Species = PSpecies.Crobat,
                Nickname = "Caro",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.BlackSludge,
                Ability = PAbility.InnerFocus,
                Gender = PGender.Female,
                Nature = PNature.Jolly,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power Dark/70
                EVs = new byte[] { 252, 0, 4, 0, 0, 252, },
                Moves = new PMove[] { PMove.Facade, PMove.NastyPlot, PMove.AirSlash, PMove.Toxic }, // Taunt, Roost, Brave Bird, Toxic
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Azumarill_UU = new PPokemonShell
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
                EVs = new byte[] { 252, 252, 0, 0, 0, 4 },
                Moves = new PMove[] { PMove.Waterfall, PMove.AquaJet, PMove.Return, PMove.IcePunch },
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
            Latias_OU = new PPokemonShell
            {
                Species = PSpecies.Latias,
                Nickname = "Lani",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.LifeOrb,
                Ability = PAbility.Levitate,
                Gender = PGender.Female,
                Nature = PNature.Timid,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
                Moves = new PMove[] { PMove.DracoMeteor, PMove.Fly, PMove.Surf, PMove.PsychUp }, // Draco Meteor, Surf, Psyshock, Roost
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Latios_OU = new PPokemonShell
            {
                Species = PSpecies.Latios,
                Nickname = "Loomo",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.ChoiceSpecs,
                Ability = PAbility.Levitate,
                Gender = PGender.Male,
                Nature = PNature.Timid,
                IVs = new byte[] { 31, 30, 31, 30, 31, 30 }, // Hidden Power: Fire/70
                EVs = new byte[] { 0, 0, 0, 252, 4, 252 },
                Moves = new PMove[] { PMove.DracoMeteor, PMove.LusterPurge, PMove.Surf, PMove.HiddenPower }, // Draco Meteor, Surf, Psyshock, Hidden Power Fire
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Rotom_Wash_OU = new PPokemonShell
            {
                Species = PSpecies.Rotom_Wash,
                Nickname = "Wahooey",
                Level = 100,
                Friendship = 255,
                Item = PItem.Leftovers,
                Shiny = false,
                Ability = PAbility.Levitate,
                Gender = PGender.Genderless,
                Nature = PNature.Modest,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 232, 0, 0, 56, 0, 220 },
                Moves = new PMove[] { PMove.HydroPump, PMove.WillOWisp, PMove.Discharge, PMove.PainSplit }, // Volt Switch, Hydro Pump, Will-O-Wisp, Pain Split
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Cresselia_UU = new PPokemonShell
            {
                Species = PSpecies.Cresselia,
                Nickname = "Crest",
                Level = 100,
                Friendship = 255,
                Item = PItem.Leftovers,
                Shiny = false,
                Ability = PAbility.Levitate,
                Gender = PGender.Female,
                Nature = PNature.Bold,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
                EVs = new byte[] { 252, 0, 252, 0, 0, 4 },
                Moves = new PMove[] { PMove.Psychic, PMove.Moonlight, PMove.IceBeam, PMove.Toxic },
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
            Cofagrigus_UU = new PPokemonShell
            {
                Species = PSpecies.Cofagrigus,
                Nickname = "Coffee",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.Leftovers,
                Ability = PAbility.Mummy,
                Gender = PGender.Male,
                Nature = PNature.Quiet,
                IVs = new byte[] { 31, 31, 30, 30, 30, 30 }, // Hidden Power: Fighting/70
                EVs = new byte[] { 248, 0, 8, 252, 0, 0 },
                Moves = new PMove[] { PMove.ShadowBall, PMove.HiddenPower, PMove.NastyPlot, PMove.Curse }, // Shadow Ball, Hidden Power Fighting, Nasty Plot, Trick Room
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
