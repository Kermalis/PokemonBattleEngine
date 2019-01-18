using Kermalis.PokemonBattleEngine.Data;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine
{
    /// <summary>
    /// A class that contains default competitive Pokémon.
    /// </summary>
    public static class PBECompetitivePokemonShells
    {
        public static PBEPokemonShell Absol_RU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Absol,
            Nickname = "Absoul",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.DreadPlate,
            Ability = PBEAbility.SuperLuck,
            Gender = PBEGender.Male,
            Nature = PBENature.Adamant,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 0, 252, 4, 0, 0, 252 },
            Moves = new PBEMove[] { PBEMove.SwordsDance, PBEMove.RockSlide, PBEMove.NightSlash, PBEMove.Superpower }, // Sucker Punch, Superpower, Night Slash, Pursuit
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Alomomola_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Alomomola,
            Nickname = "Aloha",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.WacanBerry,
            Ability = PBEAbility.Healer,
            Gender = PBEGender.Female,
            Nature = PBENature.Careful,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 0, 4, 252, 0, 252, 0 },
            Moves = new PBEMove[] { PBEMove.Substitute, PBEMove.Facade, PBEMove.Waterfall, PBEMove.Protect }, // Helping Hand, Heal Pulse, Waterfall, Wide Guard
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Arceus_Uber { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Arceus,
            Nickname = "Exon",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.SilkScarf,
            Ability = PBEAbility.Multitype,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Adamant,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 200, 252, 0, 0, 0, 56 },
            Moves = new PBEMove[] { PBEMove.SwordsDance, PBEMove.ExtremeSpeed, PBEMove.ShadowClaw, PBEMove.BrickBreak },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Azelf_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Azelf,
            Nickname = "Zinc",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.Levitate,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Jolly,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 4, 252, 0, 0, 0, 252 },
            Moves = new PBEMove[] { PBEMove.ZenHeadbutt, PBEMove.IcePunch, PBEMove.Explosion, PBEMove.Detect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Azumarill_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Azumarill,
            Nickname = "ZuWEEE",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.ChoiceBand,
            Ability = PBEAbility.HugePower,
            Gender = PBEGender.Male,
            Nature = PBENature.Adamant,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 164, 252, 0, 0, 0, 92 },
            Moves = new PBEMove[] { PBEMove.Waterfall, PBEMove.AquaJet, PBEMove.Superpower, PBEMove.IcePunch },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        /// <summary>
        /// <para>Beedrill NU</para>
        /// <para>Item: Choice Scarf</para>
        /// <para>Ability: Swarm</para>
        /// <para>Nature: Jolly (+Spe -SpAtk)</para>
        /// <para>IVs: 0 HP, 0 Def, 0 SpDef</para>
        /// <para>Hidden Power: Ghost/46</para>
        /// <para>EVs: 252 Atk, 252 Spe</para>
        /// <para>Moves: Toxic Spikes, Endeavor, X-Scissor, Agility</para>
        /// </summary>
        public static PBEPokemonShell Beedrill_NU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Beedrill,
            Nickname = "Citizen",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.FocusSash,
            Ability = PBEAbility.Swarm,
            Gender = PBEGender.Male,
            Nature = PBENature.Jolly,
            IVs = new byte[] { 0, 31, 0, 31, 0, 31 }, // Hidden Power: Ghost/46
            EVs = new byte[] { 0, 252, 0, 0, 0, 252 },
            Moves = new PBEMove[] { PBEMove.ToxicSpikes, PBEMove.Endeavor, PBEMove.XScissor, PBEMove.Agility }, // Toxic Spikes, Endeavor, U-Turn, Tailwind
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        /// <summary>
        /// <para>Blastoise UU</para>
        /// <para>Item: Leftovers</para>
        /// <para>Ability: Rain Dish</para>
        /// <para>Nature: Relaxed (+Def -Spe)</para>
        /// <para>Hidden Power: Dark/70</para>
        /// <para>EVs: 252 HP, 252 Def, 4 SpDef</para>
        /// <para>Moves: Substitute, Scald, Protect, Rain Dance</para>
        /// </summary>
        public static PBEPokemonShell Blastoise_UU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Blastoise,
            Nickname = "Boost",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.RainDish,
            Gender = PBEGender.Male,
            Nature = PBENature.Relaxed,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
            EVs = new byte[] { 252, 0, 252, 0, 4, 0 },
            Moves = new PBEMove[] { PBEMove.Substitute, PBEMove.Scald, PBEMove.Roar, PBEMove.RainDance }, // Rapid Spin, Scald, Roar, Foresight
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Blaziken_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Blaziken,
            Nickname = "Brenda",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.SpeedBoost,
            Gender = PBEGender.Female,
            Nature = PBENature.Adamant,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 4, 252, 0, 0, 0, 252 },
            Moves = new PBEMove[] { PBEMove.FlareBlitz, PBEMove.SkyUppercut, PBEMove.RockSlide, PBEMove.Protect }, // Flare Blitz, High Jump Kick, Rock Slide, Protect
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Blissey_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Blissey,
            Nickname = "Eggina",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.NaturalCure,
            Gender = PBEGender.Female,
            Nature = PBENature.Calm,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 252, 0, 4, 0 },
            Moves = new PBEMove[] { PBEMove.Toxic, PBEMove.Softboiled, PBEMove.EggBomb, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Bouffalant_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Bouffalant,
            Nickname = "Bob",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.SapSipper,
            Gender = PBEGender.Male,
            Nature = PBENature.Brave,
            IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
            EVs = new byte[] { 252, 252, 0, 0, 4, 0 },
            Moves = new PBEMove[] { PBEMove.HeadCharge, PBEMove.WildCharge, PBEMove.Earthquake, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        /// <summary>
        /// <para>Butterfree RU</para>
        /// <para>Item: Leftovers</para>
        /// <para>Ability: Compoundeyes</para>
        /// <para>Nature: Timid (+Spe -Atk)</para>
        /// <para>Hidden Power: Dark/70</para>
        /// <para>EVs: 252 SpAtk, 4 SpDef, 252 Spe</para>
        /// <para>Moves: Sleep Powder, Quiver Dance, Bug Buzz, Substitute</para>
        /// </summary>
        public static PBEPokemonShell Butterfree_RU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Butterfree,
            Nickname = "Fluff",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.Compoundeyes,
            Gender = PBEGender.Female,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
            EVs = new byte[] { 0, 0, 0, 252, 4, 252 },
            Moves = new PBEMove[] { PBEMove.SleepPowder, PBEMove.QuiverDance, PBEMove.BugBuzz, PBEMove.Substitute },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Carracosta_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Carracosta,
            Nickname = "Tortwa",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.WaterGem,
            Ability = PBEAbility.Sturdy,
            Gender = PBEGender.Male,
            Nature = PBENature.Jolly,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 4, 252, 0, 0, 0, 252 },
            Moves = new PBEMove[] { PBEMove.ShellSmash, PBEMove.Waterfall, PBEMove.RockSlide, PBEMove.AquaJet },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Chandelure_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Chandelure,
            Nickname = "Candela",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.FireGem,
            Ability = PBEAbility.FlashFire,
            Gender = PBEGender.Female,
            Nature = PBENature.Quiet,
            IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
            EVs = new byte[] { 252, 0, 0, 252, 4, 0 },
            Moves = new PBEMove[] { PBEMove.HeatWave, PBEMove.ShadowBall, PBEMove.EnergyBall, PBEMove.Protect }, // Heat Wave, Shadow Ball, Trick Room, Protect
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        /// <summary>
        /// <para>Charizard VGC</para>
        /// <para>Item: Choice Scarf</para>
        /// <para>Ability: Solar Power</para>
        /// <para>Nature: Modest (+SpAtk -Atk)</para>
        /// <para>Hidden Power: Ice/70</para>
        /// <para>EVs: 252 SpAtk, 4 SpDef, 252 Spe</para>
        /// <para>Moves: Fire Blast, Air Slash, Focus Blast, Hidden Power </para>
        /// </summary>
        public static PBEPokemonShell Charizard_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Charizard,
            Nickname = "Chomp",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.ChoiceScarf,
            Ability = PBEAbility.SolarPower,
            Gender = PBEGender.Male,
            Nature = PBENature.Modest,
            IVs = new byte[] { 31, 30, 30, 31, 31, 31 },
            EVs = new byte[] { 0, 0, 0, 252, 4, 252 },
            Moves = new PBEMove[] { PBEMove.FireBlast, PBEMove.AirSlash, PBEMove.FocusBlast, PBEMove.HiddenPower },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Cofagrigus_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Cofagrigus,
            Nickname = "Coffee",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.Mummy,
            Gender = PBEGender.Male,
            Nature = PBENature.Sassy,
            IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
            EVs = new byte[] { 248, 0, 8, 252, 0, 0 },
            Moves = new PBEMove[] { PBEMove.ShadowBall, PBEMove.Protect, PBEMove.WillOWisp, PBEMove.Curse }, // Shadow Ball, Will-O-Wisp, Protect, Trick Room
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Cradily_OU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Cradily,
            Nickname = "PLANT",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.SuctionCups,
            Gender = PBEGender.Male,
            Nature = PBENature.Careful,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 80, 0, 176, 0 },
            Moves = new PBEMove[] { PBEMove.Curse, PBEMove.RockSlide, PBEMove.StealthRock, PBEMove.SeedBomb }, // Curse, Rest, Sleep Talk, Rock Slide
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Cresselia_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Cresselia,
            Nickname = "Crest",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.SitrusBerry,
            Ability = PBEAbility.Levitate,
            Gender = PBEGender.Female,
            Nature = PBENature.Relaxed,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 60, 0, 196, 0 },
            Moves = new PBEMove[] { PBEMove.Psychic, PBEMove.LightScreen, PBEMove.IcyWind, PBEMove.Toxic }, // Psychic, Trick Room, Icy Wind, Safeguard
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Crobat_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Crobat,
            Nickname = "Caro",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.FlyingGem,
            Ability = PBEAbility.InnerFocus,
            Gender = PBEGender.Female,
            Nature = PBENature.Jolly,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power Dark/70
            EVs = new byte[] { 4, 252, 0, 0, 0, 252, },
            Moves = new PBEMove[] { PBEMove.AirSlash, PBEMove.NastyPlot, PBEMove.SuperFang, PBEMove.Protect }, // Acrobatics, Taunt, Super Fang, Protect
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Cryogonal_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Cryogonal,
            Nickname = "Fug",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.FocusSash,
            Ability = PBEAbility.Levitate,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Hasty,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 0, 28, 0, 252, 0, 228 },
            Moves = new PBEMove[] { PBEMove.IceBeam, PBEMove.HiddenPower, PBEMove.Reflect, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Darkrai_Uber { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Darkrai,
            Nickname = "Kylo",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.BadDreams,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.DarkVoid, PBEMove.DarkPulse, PBEMove.NastyPlot, PBEMove.Substitute },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Delcatty_NU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Delcatty,
            Nickname = "Anika",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.WonderSkin,
            Gender = PBEGender.Female,
            Nature = PBENature.Bold,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 200, 0, 0, 56 },
            Moves = new PBEMove[] { PBEMove.ShadowBall, PBEMove.CalmMind, PBEMove.HyperVoice, PBEMove.Substitute }, // Baton Pass, Calm Mind, Hyper Voice, Substitute
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Dialga_Uber { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Dialga,
            Nickname = "Seaweed",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.Pressure,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Modest,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 0, 56, 200, 0 },
            Moves = new PBEMove[] { PBEMove.StealthRock, PBEMove.DragonPulse, PBEMove.FireBlast, PBEMove.Thunder },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Ditto_Uber { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Ditto,
            Nickname = "Doot",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.ChoiceScarf,
            Ability = PBEAbility.Imposter,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Relaxed,
            IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
            EVs = new byte[] { 248, 0, 252, 0, 8, 0 },
            Moves = new PBEMove[] { PBEMove.Transform, PBEMove.None, PBEMove.None, PBEMove.None },
            PPUps = new byte[] { 3, 0, 0, 0 }
        };
        public static PBEPokemonShell Druddigon_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Druddigon,
            Nickname = "Teddy",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.RoughSkin,
            Gender = PBEGender.Male,
            Nature = PBENature.Brave,
            IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
            EVs = new byte[] { 252, 252, 0, 0, 4, 0 },
            Moves = new PBEMove[] { PBEMove.DragonClaw, PBEMove.RockSlide, PBEMove.Superpower, PBEMove.Protect }, // Dragon Claw, Sucker Punch, Superpower, Protect
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Eelektross_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Eelektross,
            Nickname = "Evan",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.SitrusBerry,
            Ability = PBEAbility.Levitate,
            Gender = PBEGender.Male,
            Nature = PBENature.Modest,
            IVs = new byte[] { 31, 31, 31, 31, 31, 30 }, // Hidden Power: Ice/70
            EVs = new byte[] { 252, 0, 0, 252, 4, 0 },
            Moves = new PBEMove[] { PBEMove.Discharge, PBEMove.Flamethrower, PBEMove.HiddenPower, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Emboar_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Emboar,
            Nickname = "Brick",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.ExpertBelt,
            Ability = PBEAbility.Blaze,
            Gender = PBEGender.Male,
            Nature = PBENature.Brave,
            IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
            EVs = new byte[] { 252, 252, 0, 0, 4, 0 },
            Moves = new PBEMove[] { PBEMove.FlareBlitz, PBEMove.HammerArm, PBEMove.RockSlide, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Emolga_NU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Emolga,
            Nickname = "Helo",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.MotorDrive,
            Gender = PBEGender.Female,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 0, 0, 4, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.Agility, PBEMove.LightScreen, PBEMove.SignalBeam, PBEMove.AirSlash }, // Tailwind, Encore, U-turn, Air Slash
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Empoleon_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Empoleon,
            Nickname = "Eve",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.ChopleBerry,
            Ability = PBEAbility.Torrent,
            Gender = PBEGender.Female,
            Nature = PBENature.Calm,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 4, 252, 0, 0 },
            Moves = new PBEMove[] { PBEMove.Scald, PBEMove.IcyWind, PBEMove.GrassKnot, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Espeon_Uber { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Espeon,
            Nickname = "Vaqui",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.FocusSash,
            Ability = PBEAbility.MagicBounce,
            Gender = PBEGender.Female,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 31, 30, 30, 31, 30 }, // Hidden Power: Fire/70
            EVs = new byte[] { 0, 0, 4, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.ShadowBall, PBEMove.Psyshock, PBEMove.GrassKnot, PBEMove.HiddenPower }, // Shadow Ball, Yawn, Grass Knot, Hidden Power Fire
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Excadrill_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Excadrill,
            Nickname = "Edna",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.ChopleBerry,
            Ability = PBEAbility.SandRush,
            Gender = PBEGender.Female,
            Nature = PBENature.Adamant,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 60, 252, 0, 0, 0, 196 },
            Moves = new PBEMove[] { PBEMove.Earthquake, PBEMove.RockSlide, PBEMove.XScissor, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        /// <summary>
        /// <para>Farfetch'd OU</para>
        /// <para>Item: Stick</para>
        /// <para>Ability: Defiant</para>
        /// <para>Nature: Adamant (+Atk -SpAtk)</para>
        /// <para>Hidden Power: Dark/70</para>
        /// <para>EVs: 4 Atk, 252 Atk, 252 Spe</para>
        /// <para>Moves: Swords Dance, Return, Air Slash, Steel Wing</para>
        /// </summary>
        public static PBEPokemonShell Farfetchd_OU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Farfetchd,
            Nickname = "Farquaad",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Stick,
            Ability = PBEAbility.Defiant,
            Gender = PBEGender.Male,
            Nature = PBENature.Adamant,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
            EVs = new byte[] { 4, 252, 0, 0, 0, 252 },
            Moves = new PBEMove[] { PBEMove.SwordsDance, PBEMove.Return, PBEMove.BraveBird, PBEMove.SteelWing }, // Swords Dance, Return, Brave Bird, Revenge
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Ferrothorn_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Ferrothorn,
            Nickname = "Pointy",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.IronBarbs,
            Gender = PBEGender.Male,
            Nature = PBENature.Relaxed,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 148, 0, 108, 0 },
            Moves = new PBEMove[] { PBEMove.LeechSeed, PBEMove.Protect, PBEMove.PowerWhip, PBEMove.ThunderWave }, // Leech Seed, Protect, Power Whip, Gyro Ball
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Flareon_RU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Flareon,
            Nickname = "Fugu",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.FlashFire,
            Gender = PBEGender.Male,
            Nature = PBENature.Calm,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 4, 0, 252, 0 },
            Moves = new PBEMove[] { PBEMove.HyperVoice, PBEMove.Protect, PBEMove.LavaPlume, PBEMove.Toxic }, // Wish, Protect, Lava Plume, Toxic
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Garchomp_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Garchomp,
            Nickname = "Gru",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.YacheBerry,
            Ability = PBEAbility.RoughSkin,
            Gender = PBEGender.Male,
            Nature = PBENature.Jolly,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 4, 252, 0, 0, 0, 252 },
            Moves = new PBEMove[] { PBEMove.DragonClaw, PBEMove.Earthquake, PBEMove.Crunch, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Genesect_Uber { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Genesect,
            Nickname = "Jessenect",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.ChoiceScarf,
            Ability = PBEAbility.Download,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Hasty,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 0, 252, 0, 4, 0, 252 },
            Moves = new PBEMove[] { PBEMove.BugBuzz, PBEMove.IceBeam, PBEMove.Flamethrower, PBEMove.IronHead }, // U-Turn, Ice Beam, Flamethrower, Iron Head
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Giratina_Origin_Uber { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Giratina_Origin,
            Nickname = "Gary",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.GriseousOrb,
            Ability = PBEAbility.Levitate,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Adamant,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 164, 252, 8, 0, 0, 84 },
            Moves = new PBEMove[] { PBEMove.DragonClaw, PBEMove.ShadowSneak, PBEMove.Earthquake, PBEMove.Substitute }, // Dragon Tail, Shadow Sneak, Earthquake, Magic Coat
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Glaceon_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Glaceon,
            Nickname = "Gizzard",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.IronBall,
            Ability = PBEAbility.SnowCloak,
            Gender = PBEGender.Male,
            Nature = PBENature.Quiet,
            IVs = new byte[] { 31, 31, 31, 30, 30, 3 }, // Hidden Power: Ground/70
            EVs = new byte[] { 252, 0, 0, 248, 8, 0 },
            Moves = new PBEMove[] { PBEMove.Blizzard, PBEMove.ShadowBall, PBEMove.HiddenPower, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Golduck_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Golduck,
            Nickname = "Wawa",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.FocusSash,
            Ability = PBEAbility.CloudNine,
            Gender = PBEGender.Male,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.Scald, PBEMove.IcyWind, PBEMove.PsychUp, PBEMove.Protect }, // Scald, Icy Wind, Encore, Protect
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Gorebyss_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Gorebyss,
            Nickname = "Googi",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.SwiftSwim,
            Gender = PBEGender.Female,
            Nature = PBENature.Modest,
            IVs = new byte[] { 31, 30, 30, 31, 30, 31 }, // Hidden Power: Bug/70
            EVs = new byte[] { 28, 0, 0, 252, 0, 228 },
            Moves = new PBEMove[] { PBEMove.MuddyWater, PBEMove.IceBeam, PBEMove.ShellSmash, PBEMove.HiddenPower },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Gothitelle_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Gothitelle,
            Nickname = "Jazzy",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.MentalHerb,
            Ability = PBEAbility.Frisk,
            Gender = PBEGender.Female,
            Nature = PBENature.Quiet,
            IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
            EVs = new byte[] { 252, 0, 92, 164, 0, 0 },
            Moves = new PBEMove[] { PBEMove.Flatter, PBEMove.Psychic, PBEMove.ShadowBall, PBEMove.Protect }, // Trick Room, Psychic, Shadow Ball, Protect
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Gyarados_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Gyarados,
            Nickname = "GOOOO",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.WacanBerry,
            Ability = PBEAbility.Intimidate,
            Gender = PBEGender.Male,
            Nature = PBENature.Adamant,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 52, 140, 0, 20, 44 },
            Moves = new PBEMove[] { PBEMove.Waterfall, PBEMove.ThunderWave, PBEMove.StoneEdge, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Haxorus_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Haxorus,
            Nickname = "Andrew",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LumBerry,
            Ability = PBEAbility.MoldBreaker,
            Gender = PBEGender.Male,
            Nature = PBENature.Adamant,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 76, 252, 0, 0, 0, 180 },
            Moves = new PBEMove[] { PBEMove.DragonClaw, PBEMove.RockSlide, PBEMove.BrickBreak, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Huntail_UU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Huntail,
            Nickname = "Hoogi",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.WhiteHerb,
            Ability = PBEAbility.WaterVeil,
            Gender = PBEGender.Male,
            Nature = PBENature.Jolly,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 72, 0, 0, 184 },
            Moves = new PBEMove[] { PBEMove.ShellSmash, PBEMove.Crunch, PBEMove.Substitute, PBEMove.Waterfall }, // Shell Smash, Baton Pass, Substitute, Waterfall
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Hydreigon_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Hydreigon,
            Nickname = "Heidi",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.Levitate,
            Gender = PBEGender.Female,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.DracoMeteor, PBEMove.DarkPulse, PBEMove.Flamethrower, PBEMove.Surf },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Infernape_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Infernape,
            Nickname = "Harmony",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.FocusSash,
            Ability = PBEAbility.Blaze,
            Gender = PBEGender.Female,
            Nature = PBENature.Naive,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 0, 116, 0, 140, 0, 252 },
            Moves = new PBEMove[] { PBEMove.HiddenPower, PBEMove.CloseCombat, PBEMove.Overheat, PBEMove.Protect }, // Fake Out, Close Combat, Overheat, Encore
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Jellicent_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Jellicent,
            Nickname = "Juice",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.SitrusBerry,
            Ability = PBEAbility.WaterAbsorb,
            Gender = PBEGender.Female,
            Nature = PBENature.Quiet,
            IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
            EVs = new byte[] { 252, 0, 4, 252, 0, 0 },
            Moves = new PBEMove[] { PBEMove.Recover, PBEMove.WaterSpout, PBEMove.ShadowBall, PBEMove.WillOWisp }, // Trick Room, Water Spout, Shadow Ball, Will-O-Wisp
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Jirachi_Uber { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Jirachi,
            Nickname = "Gina",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.SereneGrace,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Careful,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 4, 0, 252, 0 },
            Moves = new PBEMove[] { PBEMove.Protect, PBEMove.StealthRock, PBEMove.IronHead, PBEMove.Thunder }, // Wish, U-turn, Iron Head, Thunder
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Jolteon_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Jolteon,
            Nickname = "Jilly",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.FocusSash,
            Ability = PBEAbility.VoltAbsorb,
            Gender = PBEGender.Female,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 30, 30, 31, 31, 31 }, // Hidden Power: Ice/70
            EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.Thunderbolt, PBEMove.ThunderWave, PBEMove.HiddenPower, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Kyurem_White_Uber { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Kyurem_White,
            Nickname = "Keke",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.HabanBerry,
            Ability = PBEAbility.Turboblaze,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 0, 0, 0, 252, 4, 252 },
            Moves = new PBEMove[] { PBEMove.DracoMeteor, PBEMove.IceBeam, PBEMove.EarthPower, PBEMove.Substitute }, // Draco Meteor, Ice Beam, Earth Power, Roost
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Landorus_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Landorus,
            Nickname = "Leandro",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.YacheBerry,
            Ability = PBEAbility.SandForce,
            Gender = PBEGender.Male,
            Nature = PBENature.Jolly,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 4, 252, 0, 0, 0, 252 },
            Moves = new PBEMove[] { PBEMove.Earthquake, PBEMove.RockSlide, PBEMove.Explosion, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Latias_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Latias,
            Nickname = "Lani",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.YacheBerry,
            Ability = PBEAbility.Levitate,
            Gender = PBEGender.Female,
            Nature = PBENature.Bold,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 252, 0, 0, 4 },
            Moves = new PBEMove[] { PBEMove.DragonPulse, PBEMove.IcyWind, PBEMove.DoubleTeam, PBEMove.Recover }, // Dragon Pulse, Icy Wind, Helping Hand, Recover
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Latios_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Latios,
            Nickname = "Loomo",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.Levitate,
            Gender = PBEGender.Male,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 30, 31, 30, 31, 30 }, // Hidden Power: Fire/70
            EVs = new byte[] { 4, 0, 4, 248, 4, 248 },
            Moves = new PBEMove[] { PBEMove.DracoMeteor, PBEMove.Psychic, PBEMove.Protect, PBEMove.HiddenPower },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Leafeon_RU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Leafeon,
            Nickname = "Gift",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.Chlorophyll,
            Gender = PBEGender.Male,
            Nature = PBENature.Jolly,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 4, 0, 0, 252 },
            Moves = new PBEMove[] { PBEMove.SwordsDance, PBEMove.XScissor, PBEMove.LeafBlade, PBEMove.Synthesis }, // Swords Dance, Baton Pass, Leaf Blade, Synthesis
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Lucario_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Lucario,
            Nickname = "Carly",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.ChopleBerry,
            Ability = PBEAbility.InnerFocus,
            Gender = PBEGender.Female,
            Nature = PBENature.Adamant,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 4, 252, 0, 0, 0, 252 },
            Moves = new PBEMove[] { PBEMove.CloseCombat, PBEMove.ExtremeSpeed, PBEMove.IcePunch, PBEMove.Detect }, // Close Combat, Extreme Speed, Ice Punch, Follow Me
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Luxray_NU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Luxray,
            Nickname = "Mittens",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.FlameOrb,
            Ability = PBEAbility.Guts,
            Gender = PBEGender.Male,
            Nature = PBENature.Jolly,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 4, 252, 0, 0, 0, 252 },
            Moves = new PBEMove[] { PBEMove.WildCharge, PBEMove.Superpower, PBEMove.Facade, PBEMove.Crunch },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Marowak_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Marowak,
            Nickname = "Mawoogo",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.ThickClub,
            Ability = PBEAbility.Lightningrod,
            Gender = PBEGender.Male,
            Nature = PBENature.Brave,
            IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
            EVs = new byte[] { 252, 252, 4, 0, 0, 0 },
            Moves = new PBEMove[] { PBEMove.Earthquake, PBEMove.RockSlide, PBEMove.FirePunch, PBEMove.SwordsDance },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Meloetta_OU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Meloetta,
            Nickname = "Melody",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.ExpertBelt,
            Ability = PBEAbility.SereneGrace,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Naive,
            IVs = new byte[] { 31, 31, 31, 31, 31, 30 }, // Hidden Power: Ice/70
            EVs = new byte[] { 0, 64, 0, 252, 0, 192 },
            Moves = new PBEMove[] { PBEMove.TeeterDance, PBEMove.CloseCombat, PBEMove.ShadowBall, PBEMove.HiddenPower }, // Relic Song, Close Combat, Shadow Ball, Hidden Power
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Mesprit_UU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Mesprit,
            Nickname = "Cindy",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.Levitate,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Quiet,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 4, 252, 0, 0 },
            Moves = new PBEMove[] { PBEMove.CalmMind, PBEMove.Thunderbolt, PBEMove.Psychic, PBEMove.HiddenPower }, // Thunderbolt, Trick Room, Psychic, U-turn
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Metagross_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Metagross,
            Nickname = "Crizzy",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.NormalGem,
            Ability = PBEAbility.ClearBody,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Adamant,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 252, 0, 0, 0, 4 },
            Moves = new PBEMove[] { PBEMove.MeteorMash, PBEMove.BulletPunch, PBEMove.Earthquake, PBEMove.Explosion },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Minun_NU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Minun,
            Nickname = "MIKEY",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.Minus,
            Gender = PBEGender.Male,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 4, 0, 0, 252 },
            Moves = new PBEMove[] { PBEMove.NastyPlot, PBEMove.Thunderbolt, PBEMove.HiddenPower, PBEMove.QuickAttack }, // Nasty Plot, Thunderbolt, Encore, Baton Pass
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Mismagius_UU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Mismagius,
            Nickname = "Kermie",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.Levitate,
            Gender = PBEGender.Female,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 31, 30, 30, 30, 30 }, // Hidden Power: Fighting/70
            EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.NastyPlot, PBEMove.ShadowBall, PBEMove.HiddenPower, PBEMove.Thunderbolt },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        /// <summary>
        /// <para>Ninetales VGC</para>
        /// <para>Item: Flame Plate</para>
        /// <para>Ability: Drought</para>
        /// <para>Nature: Modest (+SpAtk -Atk)</para>
        /// <para>Hidden Power: Ice/70</para>
        /// <para>EVs: 4 HP, 252 SpAtk, 252 Spe</para>
        /// <para>Moves: Flamethrower, Hidden Power, Energy Ball, Protect</para>
        /// </summary>
        public static PBEPokemonShell Ninetales_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Ninetales,
            Nickname = "Jasmine",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.FocusSash,
            Ability = PBEAbility.Drought,
            Gender = PBEGender.Female,
            Nature = PBENature.Modest,
            IVs = new byte[] { 31, 30, 30, 31, 31, 31 },
            EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.Flamethrower, PBEMove.HiddenPower, PBEMove.EnergyBall, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Ninjask_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Ninjask,
            Nickname = "Nut",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.SpeedBoost,
            Gender = PBEGender.Male,
            Nature = PBENature.Adamant,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 252, 0, 0, 0, 4 },
            Moves = new PBEMove[] { PBEMove.XScissor, PBEMove.AerialAce, PBEMove.Dig, PBEMove.Protect }, // X-Scissor, Aerial Ace, Baton Pass, Protect
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Pachirisu_NU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Pachirisu,
            Nickname = "Poolala",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.ZapPlate,
            Ability = PBEAbility.VoltAbsorb,
            Gender = PBEGender.Female,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 156, 0, 0, 56, 44, 252 },
            Moves = new PBEMove[] { PBEMove.GrassKnot, PBEMove.ThunderWave, PBEMove.Thunderbolt, PBEMove.SuperFang }, // Fling, Covet, Volt Switch, Super Fang
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Palkia_Uber { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Palkia,
            Nickname = "Pokey",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LustrousOrb,
            Ability = PBEAbility.Pressure,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.SpacialRend, PBEMove.Surf, PBEMove.FireBlast, PBEMove.Thunder },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        /// <summary>
        /// <para>Pikachu VGC</para>
        /// <para>Item: Light Ball</para>
        /// <para>Ability: Lightningrod</para>
        /// <para>Nature: Rash (+SpAtk -SpDef)</para>
        /// <para>Hidden Power: Ice/70</para>
        /// <para>EVs: 4 Atk, 252 SpAtk, 252 Spe</para>
        /// <para>Moves: Substitute, Thunderbolt, Hidden Power, Grass Knot</para>
        /// </summary>
        public static PBEPokemonShell Pikachu_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Pikachu,
            Nickname = "Poof",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LightBall,
            Ability = PBEAbility.Lightningrod,
            Gender = PBEGender.Female,
            Nature = PBENature.Rash,
            IVs = new byte[] { 31, 30, 30, 31, 31, 31 },
            EVs = new byte[] { 0, 4, 0, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.Protect, PBEMove.Thunderbolt, PBEMove.HiddenPower, PBEMove.GrassKnot }, // Fake Out, Thunderbolt, Hidden Power Ice, Grass Knot
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Plusle_NU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Plusle,
            Nickname = "Peanut",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.Plus,
            Gender = PBEGender.Female,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 30, 30, 31, 31, 31 }, // Hidden Power: Ice/70
            EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.NastyPlot, PBEMove.Thunderbolt, PBEMove.HiddenPower, PBEMove.QuickAttack }, // Nasty Plot, Thunderbolt, Hidden Power Ice, Baton Pass
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Politoed_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Politoed,
            Nickname = "Patrick",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.WaterGem,
            Ability = PBEAbility.Drizzle,
            Gender = PBEGender.Male,
            Nature = PBENature.Modest,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 0, 180, 76, 0 },
            Moves = new PBEMove[] { PBEMove.Scald, PBEMove.IceBeam, PBEMove.Psychic, PBEMove.Protect }, // Scald, Ice Beam, Encore, Protect
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Poliwrath_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Poliwrath,
            Nickname = "Peter",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.SitrusBerry,
            Ability = PBEAbility.SwiftSwim,
            Gender = PBEGender.Male,
            Nature = PBENature.Adamant,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 168, 252, 4, 0, 4, 80 },
            Moves = new PBEMove[] { PBEMove.Waterfall, PBEMove.BrickBreak, PBEMove.Earthquake, PBEMove.Protect }, // Waterfall, Brick Break, Belly Drum, Protect
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Raichu_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Raichu,
            Nickname = "Archu",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.FocusSash,
            Ability = PBEAbility.Lightningrod,
            Gender = PBEGender.Male,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.Discharge, PBEMove.GrassKnot, PBEMove.FocusBlast, PBEMove.Protect }, // Discharge, Fake Out, Focus Blast, Encore
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Regirock_UU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Regirock,
            Nickname = "Rayman",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.ClearBody,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Careful,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 4, 0, 0, 252, 0 },
            Moves = new PBEMove[] { PBEMove.StoneEdge, PBEMove.Superpower, PBEMove.Earthquake, PBEMove.Curse }, // Stone Edge, Rest, Sleep Talk, Curse
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Reshiram_Uber { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Reshiram,
            Nickname = "Rubber",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.Turboblaze,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Hasty,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 0, 4, 0, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.BlueFlare, PBEMove.DragonPulse, PBEMove.Earthquake, PBEMove.StoneEdge }, // Blue Flare, Dragon Pulse, Roost, Stone Edge
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Rotom_Wash_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Rotom_Wash,
            Nickname = "Wahooey",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.SitrusBerry,
            Ability = PBEAbility.Levitate,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Modest,
            IVs = new byte[] { 31, 30, 31, 30, 31, 30 }, // Hidden Power: Fire/70
            EVs = new byte[] { 232, 0, 0, 56, 0, 220 },
            Moves = new PBEMove[] { PBEMove.HydroPump, PBEMove.HiddenPower, PBEMove.Discharge, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Samurott_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Samurott,
            Nickname = "Slurp",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.Torrent,
            Gender = PBEGender.Female,
            Nature = PBENature.Brave,
            IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
            EVs = new byte[] { 252, 252, 0, 0, 4, 0 },
            Moves = new PBEMove[] { PBEMove.SwordsDance, PBEMove.AquaJet, PBEMove.Megahorn, PBEMove.Detect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Serperior_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Serperior,
            Nickname = "Stacy",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.MentalHerb,
            Ability = PBEAbility.Contrary,
            Gender = PBEGender.Female,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 4, 4, 4, 244 },
            Moves = new PBEMove[] { PBEMove.LightScreen, PBEMove.Reflect, PBEMove.EnergyBall, PBEMove.Glare }, // Light Screen, Reflect, Giga Drain, Taunt
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Shedinja_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Shedinja,
            Nickname = "Shu",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.FocusSash,
            Ability = PBEAbility.WonderGuard,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Lonely,
            IVs = new byte[] { 31, 31, 11, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 0, 252, 0, 0, 0, 252 },
            Moves = new PBEMove[] { PBEMove.XScissor, PBEMove.ShadowClaw, PBEMove.Protect, PBEMove.SwordsDance },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Sigilyph_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Sigilyph,
            Nickname = "Tora",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.MagicGuard,
            Gender = PBEGender.Female,
            Nature = PBENature.Modest,
            IVs = new byte[] { 31, 31, 11, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 36, 0, 252, 220, 0, 0 },
            Moves = new PBEMove[] { PBEMove.Protect, PBEMove.IceBeam, PBEMove.Psychic, PBEMove.ShadowBall }, // Tailwind, Ice Beam, Psychic, Shadow Ball
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Smeargle_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Smeargle,
            Nickname = "Leonardo",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.MentalHerb,
            Ability = PBEAbility.Moody,
            Gender = PBEGender.Male,
            Nature = PBENature.Jolly,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 4, 0, 0, 0, 252 },
            Moves = new PBEMove[] { PBEMove.Spore, PBEMove.Reflect, PBEMove.Brine, PBEMove.Hex }, // Fake Out, Spore, Follow Me, Encore
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Thundurus_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Thundurus,
            Nickname = "Tundr",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.YacheBerry,
            Ability = PBEAbility.Prankster,
            Gender = PBEGender.Male,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 31, 31, 30, 30, 30 }, // Hidden Power: Flying/70
            EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.Thunder, PBEMove.RainDance, PBEMove.HiddenPower, PBEMove.Substitute },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Tornadus_Therian_Uber { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Tornadus_Therian,
            Nickname = "Winston",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.Regenerator,
            Gender = PBEGender.Male,
            Nature = PBENature.Naive,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 0, 4, 0, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.AerialAce, PBEMove.Superpower, PBEMove.GrassKnot, PBEMove.Protect }, // Hurricane, Superpower, U-Turn, Taunt
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Torterra_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Torterra,
            Nickname = "Sandy",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.FocusSash,
            Ability = PBEAbility.Overgrow,
            Gender = PBEGender.Female,
            Nature = PBENature.Brave,
            IVs = new byte[] { 31, 31, 31, 31, 31, 0 }, // Hidden Power: Ice/64
            EVs = new byte[] { 252, 252, 4, 0, 0, 0 },
            Moves = new PBEMove[] { PBEMove.SeedBomb, PBEMove.Earthquake, PBEMove.RockSlide, PBEMove.Protect }, // Seed Bomb, Earthquake, Rock Slide, Wide Guard
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Tropius_NU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Tropius,
            Nickname = "Bucky",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.SitrusBerry,
            Ability = PBEAbility.Harvest,
            Gender = PBEGender.Male,
            Nature = PBENature.Bold,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 248, 0, 144, 0, 0, 116 },
            Moves = new PBEMove[] { PBEMove.LeechSeed, PBEMove.Substitute, PBEMove.AirSlash, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Umbreon_UU { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Umbreon,
            Nickname = "Poppy",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.Leftovers,
            Ability = PBEAbility.Synchronize,
            Gender = PBEGender.Female,
            Nature = PBENature.Calm,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 4, 0, 252, 0 },
            Moves = new PBEMove[] { PBEMove.Snarl, PBEMove.DarkPulse, PBEMove.Moonlight, PBEMove.Protect }, // Wish, Foul Play, Heal Bell, Protect
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Uxie_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Uxie,
            Nickname = "Amy",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.SitrusBerry,
            Ability = PBEAbility.Levitate,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Calm,
            IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power: Dark/70
            EVs = new byte[] { 252, 0, 0, 4, 252, 0 },
            Moves = new PBEMove[] { PBEMove.CalmMind, PBEMove.LightScreen, PBEMove.Psychic, PBEMove.Protect }, // Imprison, Trick Room, Psychic, Protect
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Vaporeon_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Vaporeon,
            Nickname = "Pam",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.WaterGem,
            Ability = PBEAbility.WaterAbsorb,
            Gender = PBEGender.Female,
            Nature = PBENature.Quiet,
            IVs = new byte[] { 31, 30, 31, 30, 31, 3 }, // Hidden Power: Grass/70
            EVs = new byte[] { 140, 0, 116, 248, 4, 0 },
            Moves = new PBEMove[] { PBEMove.MuddyWater, PBEMove.IceBeam, PBEMove.HiddenPower, PBEMove.Detect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        /// <summary>
        /// <para>Venusaur VGC</para>
        /// <para>Item: Life Orb</para>
        /// <para>Ability: Chlorophyll</para>
        /// <para>Nature: Modest (+SpAtk -Atk)</para>
        /// <para>Hidden Power: Ice/70</para>
        /// <para>EVs: 4 HP, 252 SpAtk, 252 Spe</para>
        /// <para>Moves: Sleep Powder, Energy Ball, Hidden Power, Protect</para>
        /// </summary>
        public static PBEPokemonShell Venusaur_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Venusaur,
            Nickname = "Viola",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.LifeOrb,
            Ability = PBEAbility.Chlorophyll,
            Gender = PBEGender.Female,
            Nature = PBENature.Modest,
            IVs = new byte[] { 31, 30, 30, 31, 31, 31 },
            EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.SleepPowder, PBEMove.EnergyBall, PBEMove.HiddenPower, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Victini_Uber { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Victini,
            Nickname = "ビクティニ", // This is an event Victini
            Level = 100,
            Friendship = 255,
            Item = PBEItem.ChoiceScarf,
            Ability = PBEAbility.VictoryStar,
            Gender = PBEGender.Genderless,
            Nature = PBENature.Naive,
            IVs = new byte[] { 30, 31, 30, 31, 31, 31 }, // Hidden Power: Ice/70
            EVs = new byte[] { 0, 252, 4, 0, 0, 252 },
            Moves = new PBEMove[] { PBEMove.VCreate, PBEMove.BoltStrike, PBEMove.GrassKnot, PBEMove.HiddenPower }, // V-create, Bolt Strike, U-turn, Hidden Power Ice
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Volcarona_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Volcarona,
            Nickname = "Vicki",
            Level = 100,
            Friendship = 255,
            Item = PBEItem.FocusSash,
            Ability = PBEAbility.FlameBody,
            Gender = PBEGender.Female,
            Nature = PBENature.Timid,
            IVs = new byte[] { 31, 30, 30, 31, 31, 31 }, // Hidden Power: Ice/70
            EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
            Moves = new PBEMove[] { PBEMove.HeatWave, PBEMove.BugBuzz, PBEMove.HiddenPower, PBEMove.Protect },
            PPUps = new byte[] { 3, 3, 3, 3 }
        };
        public static PBEPokemonShell Whimsicott_VGC { get; } = new PBEPokemonShell
        {
            Species = PBESpecies.Whimsicott,
            Nickname = "Lucas",
            Level = 1,
            Friendship = 255,
            Item = PBEItem.FocusSash,
            Ability = PBEAbility.Prankster,
            Gender = PBEGender.Male,
            Nature = PBENature.Sassy,
            IVs = new byte[] { 0, 0, 29, 0, 0, 0 }, // Hidden Power: Fighting/30
            EVs = new byte[] { 0, 0, 0, 0, 80, 0 },
            Moves = new PBEMove[] { PBEMove.FakeTears, PBEMove.Endeavor, PBEMove.LightScreen, PBEMove.Protect }, // Trick Room, Endeavor, Encore, Protect
            PPUps = new byte[] { 3, 3, 3, 3 }
        };

        /// <summary>
        /// Creates a random team with pre-defined competitive Pokémon shells.
        /// </summary>
        /// <param name="numPkmn">The amount of Pokémon to add to the team.</param>
        public static IEnumerable<PBEPokemonShell> CreateRandomTeam(int numPkmn)
        {
            PBEPokemonShell[] possiblePokemon = typeof(PBECompetitivePokemonShells).GetProperties().Select(p => (PBEPokemonShell)p.GetValue(null)).ToArray();
            possiblePokemon.Shuffle();
            IEnumerable<PBEPokemonShell> team = possiblePokemon.Take(numPkmn);
            foreach (PBEPokemonShell pkmn in team)
            {
                pkmn.Shiny = PBEUtils.RNG.NextShiny(pkmn.Species);
            }
            return team;
        }
    }
}
