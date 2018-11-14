using Kermalis.PokemonBattleEngine.Data;

namespace Kermalis.PokemonBattleEngine
{
    public static class PCompetitivePokemonShells
    {
        public static readonly PPokemonShell
            Pikachu_NU = new PPokemonShell
            {
                Species = PSpecies.Pikachu,
                Nickname = "Pikachu",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.LightBall,
                Ability = PAbility.LightningRod,
                Gender = PGender.Female,
                Nature = PNature.Timid,
                IVs = new byte[] { 31, 31, 31, 31, 31, 30 }, // Hidden Power Ice/70
                EVs = new byte[] { 0, 0, 4, 252, 0, 252 },
                Moves = new PMove[] { PMove.Substitute, PMove.Thunderbolt, PMove.HiddenPower, PMove.GrassKnot },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Ditto_UU = new PPokemonShell
            {
                Species = PSpecies.Ditto,
                Nickname = "Doot",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.Leftovers, // Choice Scarf
                Ability = PAbility.HugePower, // Imposter
                Gender = PGender.Genderless,
                Nature = PNature.Sassy,
                IVs = new byte[] { 31, 31, 31, 31, 31, 30 }, // Hidden Power Ice/70
                EVs = new byte[] { 252, 0, 4, 0, 0, 252 },
                Moves = new PMove[] { PMove.Transform, PMove.None, PMove.None, PMove.None },
                PPUps = new byte[] { 3, 0, 0, 0 }
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
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power Dark/70
                EVs = new byte[] { 252, 252, 0, 0, 0, 4 },
                Moves = new PMove[] { PMove.Waterfall, PMove.AquaJet, PMove.Return, PMove.IcePunch },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Latias_OU = new PPokemonShell
            {
                Species = PSpecies.Latias,
                Nickname = "Latias",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.Leftovers, // life orb
                Ability = PAbility.Levitate,
                Gender = PGender.Female,
                Nature = PNature.Timid,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power Dark/70
                EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
                Moves = new PMove[] { PMove.DracoMeteor, PMove.LightScreen, PMove.Surf, PMove.HiddenPower }, // draco meteor, surf, psyshock, roost
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Latios_OU = new PPokemonShell
            {
                Species = PSpecies.Latios,
                Nickname = "Latios",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.Leftovers, // choice specs
                Ability = PAbility.Levitate,
                Gender = PGender.Male,
                Nature = PNature.Timid,
                IVs = new byte[] { 31, 30, 31, 30, 31, 30 }, // Hidden Power Fire/70
                EVs = new byte[] { 0, 0, 0, 252, 4, 252 },
                Moves = new PMove[] { PMove.DracoMeteor, PMove.LightScreen, PMove.Surf, PMove.HiddenPower }, // draco meteor, surf, psyshock, hidden power fire
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Cresselia_UU = new PPokemonShell
            {
                Species = PSpecies.Cresselia,
                Nickname = "Crest",
                Level = 100,
                Friendship = 255,
                Item = PItem.Leftovers,
                Shiny = true,
                Ability = PAbility.Levitate,
                Gender = PGender.Female,
                Nature = PNature.Bold,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power Dark/70
                EVs = new byte[] { 252, 0, 252, 0, 0, 4 },
                Moves = new PMove[] { PMove.Psychic, PMove.Moonlight, PMove.IceBeam, PMove.Toxic },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Darkrai_Uber = new PPokemonShell
            {
                Species = PSpecies.Darkrai,
                Nickname = "Darkrai",
                Level = 100,
                Friendship = 255,
                Shiny = true,
                Item = PItem.Leftovers,
                Ability = PAbility.BadDreams,
                Gender = PGender.Genderless,
                Nature = PNature.Timid,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power Dark/70
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
                IVs = new byte[] { 31, 31, 30, 30, 30, 30 }, // Hidden Power Fighting/70
                EVs = new byte[] { 248, 0, 8, 252, 0, 0 },
                Moves = new PMove[] { PMove.ShadowBall, PMove.HiddenPower, PMove.NastyPlot, PMove.ShadowBall }, // Shadow ball, hidden power fighting, nasty plot, trick room
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            Genesect_Uber = new PPokemonShell
            {
                Species = PSpecies.Genesect_Shock,
                Nickname = "Jessenect",
                Level = 100,
                Friendship = 255,
                Shiny = false,
                Item = PItem.ShockDrive, // Choice Scarf
                Ability = PAbility.Download,
                Gender = PGender.Genderless,
                Nature = PNature.Hasty,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power Dark/70
                EVs = new byte[] { 0, 252, 0, 4, 0, 252 },
                Moves = new PMove[] { PMove.TechnoBlast, PMove.IceBeam, PMove.Flamethrower, PMove.IronHead }, // U-turn, Ice Beam, Flamethrower, Iron Head
                PPUps = new byte[] { 3, 3, 3, 3 }
            };
    }
}
