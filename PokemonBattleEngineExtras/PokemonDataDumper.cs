using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.SimpleNARC;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kermalis.PokemonBattleEngineExtras
{
    internal sealed class PokemonDataDumper
    {
        private sealed class Pokemon
        {
            public byte HP;
            public byte Attack;
            public byte Defense;
            public byte SpAttack;
            public byte SpDefense;
            public byte Speed;
            public PBEType Type1;
            public PBEType Type2;
            public PBEGenderRatio GenderRatio;
            public double Weight;
            public List<PBESpecies> PreEvolutions = new List<PBESpecies>();
            public List<PBESpecies> Evolutions = new List<PBESpecies>();
            public List<PBEAbility> Abilities = new List<PBEAbility>();
            public Dictionary<(PBEMove Move, byte Level), PBEMoveObtainMethod> LevelUpMoves = new Dictionary<(PBEMove Move, byte Level), PBEMoveObtainMethod>();
            public Dictionary<PBEMove, PBEMoveObtainMethod> OtherMoves = new Dictionary<PBEMove, PBEMoveObtainMethod>();
        }

        private static readonly Dictionary<int, PBESpecies> _gen3SpeciesIndexToPBESpecies = new Dictionary<int, PBESpecies>
        {
            { 1, PBESpecies.Bulbasaur },
            { 2, PBESpecies.Ivysaur },
            { 3, PBESpecies.Venusaur },
            { 4, PBESpecies.Charmander },
            { 5, PBESpecies.Charmeleon },
            { 6, PBESpecies.Charizard },
            { 7, PBESpecies.Squirtle },
            { 8, PBESpecies.Wartortle },
            { 9, PBESpecies.Blastoise },
            { 10, PBESpecies.Caterpie },
            { 11, PBESpecies.Metapod },
            { 12, PBESpecies.Butterfree },
            { 13, PBESpecies.Weedle },
            { 14, PBESpecies.Kakuna },
            { 15, PBESpecies.Beedrill },
            { 16, PBESpecies.Pidgey },
            { 17, PBESpecies.Pidgeotto },
            { 18, PBESpecies.Pidgeot },
            { 19, PBESpecies.Rattata },
            { 20, PBESpecies.Raticate },
            { 21, PBESpecies.Spearow },
            { 22, PBESpecies.Fearow },
            { 23, PBESpecies.Ekans },
            { 24, PBESpecies.Arbok },
            { 25, PBESpecies.Pikachu },
            { 26, PBESpecies.Raichu },
            { 27, PBESpecies.Sandshrew },
            { 28, PBESpecies.Sandslash },
            { 29, PBESpecies.Nidoran_F },
            { 30, PBESpecies.Nidorina },
            { 31, PBESpecies.Nidoqueen },
            { 32, PBESpecies.Nidoran_M },
            { 33, PBESpecies.Nidorino },
            { 34, PBESpecies.Nidoking },
            { 35, PBESpecies.Clefairy },
            { 36, PBESpecies.Clefable },
            { 37, PBESpecies.Vulpix },
            { 38, PBESpecies.Ninetales },
            { 39, PBESpecies.Jigglypuff },
            { 40, PBESpecies.Wigglytuff },
            { 41, PBESpecies.Zubat },
            { 42, PBESpecies.Golbat },
            { 43, PBESpecies.Oddish },
            { 44, PBESpecies.Gloom },
            { 45, PBESpecies.Vileplume },
            { 46, PBESpecies.Paras },
            { 47, PBESpecies.Parasect },
            { 48, PBESpecies.Venonat },
            { 49, PBESpecies.Venomoth },
            { 50, PBESpecies.Diglett },
            { 51, PBESpecies.Dugtrio },
            { 52, PBESpecies.Meowth },
            { 53, PBESpecies.Persian },
            { 54, PBESpecies.Psyduck },
            { 55, PBESpecies.Golduck },
            { 56, PBESpecies.Mankey },
            { 57, PBESpecies.Primeape },
            { 58, PBESpecies.Growlithe },
            { 59, PBESpecies.Arcanine },
            { 60, PBESpecies.Poliwag },
            { 61, PBESpecies.Poliwhirl },
            { 62, PBESpecies.Poliwrath },
            { 63, PBESpecies.Abra },
            { 64, PBESpecies.Kadabra },
            { 65, PBESpecies.Alakazam },
            { 66, PBESpecies.Machop },
            { 67, PBESpecies.Machoke },
            { 68, PBESpecies.Machamp },
            { 69, PBESpecies.Bellsprout },
            { 70, PBESpecies.Weepinbell },
            { 71, PBESpecies.Victreebel },
            { 72, PBESpecies.Tentacool },
            { 73, PBESpecies.Tentacruel },
            { 74, PBESpecies.Geodude },
            { 75, PBESpecies.Graveler },
            { 76, PBESpecies.Golem },
            { 77, PBESpecies.Ponyta },
            { 78, PBESpecies.Rapidash },
            { 79, PBESpecies.Slowpoke },
            { 80, PBESpecies.Slowbro },
            { 81, PBESpecies.Magnemite },
            { 82, PBESpecies.Magneton },
            { 83, PBESpecies.Farfetchd },
            { 84, PBESpecies.Doduo },
            { 85, PBESpecies.Dodrio },
            { 86, PBESpecies.Seel },
            { 87, PBESpecies.Dewgong },
            { 88, PBESpecies.Grimer },
            { 89, PBESpecies.Muk },
            { 90, PBESpecies.Shellder },
            { 91, PBESpecies.Cloyster },
            { 92, PBESpecies.Gastly },
            { 93, PBESpecies.Haunter },
            { 94, PBESpecies.Gengar },
            { 95, PBESpecies.Onix },
            { 96, PBESpecies.Drowzee },
            { 97, PBESpecies.Hypno },
            { 98, PBESpecies.Krabby },
            { 99, PBESpecies.Kingler },
            { 100, PBESpecies.Voltorb },
            { 101, PBESpecies.Electrode },
            { 102, PBESpecies.Exeggcute },
            { 103, PBESpecies.Exeggutor },
            { 104, PBESpecies.Cubone },
            { 105, PBESpecies.Marowak },
            { 106, PBESpecies.Hitmonlee },
            { 107, PBESpecies.Hitmonchan },
            { 108, PBESpecies.Lickitung },
            { 109, PBESpecies.Koffing },
            { 110, PBESpecies.Weezing },
            { 111, PBESpecies.Rhyhorn },
            { 112, PBESpecies.Rhydon },
            { 113, PBESpecies.Chansey },
            { 114, PBESpecies.Tangela },
            { 115, PBESpecies.Kangaskhan },
            { 116, PBESpecies.Horsea },
            { 117, PBESpecies.Seadra },
            { 118, PBESpecies.Goldeen },
            { 119, PBESpecies.Seaking },
            { 120, PBESpecies.Staryu },
            { 121, PBESpecies.Starmie },
            { 122, PBESpecies.MrMime },
            { 123, PBESpecies.Scyther },
            { 124, PBESpecies.Jynx },
            { 125, PBESpecies.Electabuzz },
            { 126, PBESpecies.Magmar },
            { 127, PBESpecies.Pinsir },
            { 128, PBESpecies.Tauros },
            { 129, PBESpecies.Magikarp },
            { 130, PBESpecies.Gyarados },
            { 131, PBESpecies.Lapras },
            { 132, PBESpecies.Ditto },
            { 133, PBESpecies.Eevee },
            { 134, PBESpecies.Vaporeon },
            { 135, PBESpecies.Jolteon },
            { 136, PBESpecies.Flareon },
            { 137, PBESpecies.Porygon },
            { 138, PBESpecies.Omanyte },
            { 139, PBESpecies.Omastar },
            { 140, PBESpecies.Kabuto },
            { 141, PBESpecies.Kabutops },
            { 142, PBESpecies.Aerodactyl },
            { 143, PBESpecies.Snorlax },
            { 144, PBESpecies.Articuno },
            { 145, PBESpecies.Zapdos },
            { 146, PBESpecies.Moltres },
            { 147, PBESpecies.Dratini },
            { 148, PBESpecies.Dragonair },
            { 149, PBESpecies.Dragonite },
            { 150, PBESpecies.Mewtwo },
            { 151, PBESpecies.Mew },
            { 152, PBESpecies.Chikorita },
            { 153, PBESpecies.Bayleef },
            { 154, PBESpecies.Meganium },
            { 155, PBESpecies.Cyndaquil },
            { 156, PBESpecies.Quilava },
            { 157, PBESpecies.Typhlosion },
            { 158, PBESpecies.Totodile },
            { 159, PBESpecies.Croconaw },
            { 160, PBESpecies.Feraligatr },
            { 161, PBESpecies.Sentret },
            { 162, PBESpecies.Furret },
            { 163, PBESpecies.Hoothoot },
            { 164, PBESpecies.Noctowl },
            { 165, PBESpecies.Ledyba },
            { 166, PBESpecies.Ledian },
            { 167, PBESpecies.Spinarak },
            { 168, PBESpecies.Ariados },
            { 169, PBESpecies.Crobat },
            { 170, PBESpecies.Chinchou },
            { 171, PBESpecies.Lanturn },
            { 172, PBESpecies.Pichu },
            { 173, PBESpecies.Cleffa },
            { 174, PBESpecies.Igglybuff },
            { 175, PBESpecies.Togepi },
            { 176, PBESpecies.Togetic },
            { 177, PBESpecies.Natu },
            { 178, PBESpecies.Xatu },
            { 179, PBESpecies.Mareep },
            { 180, PBESpecies.Flaaffy },
            { 181, PBESpecies.Ampharos },
            { 182, PBESpecies.Bellossom },
            { 183, PBESpecies.Marill },
            { 184, PBESpecies.Azumarill },
            { 185, PBESpecies.Sudowoodo },
            { 186, PBESpecies.Politoed },
            { 187, PBESpecies.Hoppip },
            { 188, PBESpecies.Skiploom },
            { 189, PBESpecies.Jumpluff },
            { 190, PBESpecies.Aipom },
            { 191, PBESpecies.Sunkern },
            { 192, PBESpecies.Sunflora },
            { 193, PBESpecies.Yanma },
            { 194, PBESpecies.Wooper },
            { 195, PBESpecies.Quagsire },
            { 196, PBESpecies.Espeon },
            { 197, PBESpecies.Umbreon },
            { 198, PBESpecies.Murkrow },
            { 199, PBESpecies.Slowking },
            { 200, PBESpecies.Misdreavus },
            { 201, PBESpecies.Unown_A },
            { 202, PBESpecies.Wobbuffet },
            { 203, PBESpecies.Girafarig },
            { 204, PBESpecies.Pineco },
            { 205, PBESpecies.Forretress },
            { 206, PBESpecies.Dunsparce },
            { 207, PBESpecies.Gligar },
            { 208, PBESpecies.Steelix },
            { 209, PBESpecies.Snubbull },
            { 210, PBESpecies.Granbull },
            { 211, PBESpecies.Qwilfish },
            { 212, PBESpecies.Scizor },
            { 213, PBESpecies.Shuckle },
            { 214, PBESpecies.Heracross },
            { 215, PBESpecies.Sneasel },
            { 216, PBESpecies.Teddiursa },
            { 217, PBESpecies.Ursaring },
            { 218, PBESpecies.Slugma },
            { 219, PBESpecies.Magcargo },
            { 220, PBESpecies.Swinub },
            { 221, PBESpecies.Piloswine },
            { 222, PBESpecies.Corsola },
            { 223, PBESpecies.Remoraid },
            { 224, PBESpecies.Octillery },
            { 225, PBESpecies.Delibird },
            { 226, PBESpecies.Mantine },
            { 227, PBESpecies.Skarmory },
            { 228, PBESpecies.Houndour },
            { 229, PBESpecies.Houndoom },
            { 230, PBESpecies.Kingdra },
            { 231, PBESpecies.Phanpy },
            { 232, PBESpecies.Donphan },
            { 233, PBESpecies.Porygon2 },
            { 234, PBESpecies.Stantler },
            { 235, PBESpecies.Smeargle },
            { 236, PBESpecies.Tyrogue },
            { 237, PBESpecies.Hitmontop },
            { 238, PBESpecies.Smoochum },
            { 239, PBESpecies.Elekid },
            { 240, PBESpecies.Magby },
            { 241, PBESpecies.Miltank },
            { 242, PBESpecies.Blissey },
            { 243, PBESpecies.Raikou },
            { 244, PBESpecies.Entei },
            { 245, PBESpecies.Suicune },
            { 246, PBESpecies.Larvitar },
            { 247, PBESpecies.Pupitar },
            { 248, PBESpecies.Tyranitar },
            { 249, PBESpecies.Lugia },
            { 250, PBESpecies.HoOh },
            { 251, PBESpecies.Celebi },
            { 277, PBESpecies.Treecko },
            { 278, PBESpecies.Grovyle },
            { 279, PBESpecies.Sceptile },
            { 280, PBESpecies.Torchic },
            { 281, PBESpecies.Combusken },
            { 282, PBESpecies.Blaziken },
            { 283, PBESpecies.Mudkip },
            { 284, PBESpecies.Marshtomp },
            { 285, PBESpecies.Swampert },
            { 286, PBESpecies.Poochyena },
            { 287, PBESpecies.Mightyena },
            { 288, PBESpecies.Zigzagoon },
            { 289, PBESpecies.Linoone },
            { 290, PBESpecies.Wurmple },
            { 291, PBESpecies.Silcoon },
            { 292, PBESpecies.Beautifly },
            { 293, PBESpecies.Cascoon },
            { 294, PBESpecies.Dustox },
            { 295, PBESpecies.Lotad },
            { 296, PBESpecies.Lombre },
            { 297, PBESpecies.Ludicolo },
            { 298, PBESpecies.Seedot },
            { 299, PBESpecies.Nuzleaf },
            { 300, PBESpecies.Shiftry },
            { 301, PBESpecies.Nincada },
            { 302, PBESpecies.Ninjask },
            { 303, PBESpecies.Shedinja },
            { 304, PBESpecies.Taillow },
            { 305, PBESpecies.Swellow },
            { 306, PBESpecies.Shroomish },
            { 307, PBESpecies.Breloom },
            { 308, PBESpecies.Spinda },
            { 309, PBESpecies.Wingull },
            { 310, PBESpecies.Pelipper },
            { 311, PBESpecies.Surskit },
            { 312, PBESpecies.Masquerain },
            { 313, PBESpecies.Wailmer },
            { 314, PBESpecies.Wailord },
            { 315, PBESpecies.Skitty },
            { 316, PBESpecies.Delcatty },
            { 317, PBESpecies.Kecleon },
            { 318, PBESpecies.Baltoy },
            { 319, PBESpecies.Claydol },
            { 320, PBESpecies.Nosepass },
            { 321, PBESpecies.Torkoal },
            { 322, PBESpecies.Sableye },
            { 323, PBESpecies.Barboach },
            { 324, PBESpecies.Whiscash },
            { 325, PBESpecies.Luvdisc },
            { 326, PBESpecies.Corphish },
            { 327, PBESpecies.Crawdaunt },
            { 328, PBESpecies.Feebas },
            { 329, PBESpecies.Milotic },
            { 330, PBESpecies.Carvanha },
            { 331, PBESpecies.Sharpedo },
            { 332, PBESpecies.Trapinch },
            { 333, PBESpecies.Vibrava },
            { 334, PBESpecies.Flygon },
            { 335, PBESpecies.Makuhita },
            { 336, PBESpecies.Hariyama },
            { 337, PBESpecies.Electrike },
            { 338, PBESpecies.Manectric },
            { 339, PBESpecies.Numel },
            { 340, PBESpecies.Camerupt },
            { 341, PBESpecies.Spheal },
            { 342, PBESpecies.Sealeo },
            { 343, PBESpecies.Walrein },
            { 344, PBESpecies.Cacnea },
            { 345, PBESpecies.Cacturne },
            { 346, PBESpecies.Snorunt },
            { 347, PBESpecies.Glalie },
            { 348, PBESpecies.Lunatone },
            { 349, PBESpecies.Solrock },
            { 350, PBESpecies.Azurill },
            { 351, PBESpecies.Spoink },
            { 352, PBESpecies.Grumpig },
            { 353, PBESpecies.Plusle },
            { 354, PBESpecies.Minun },
            { 355, PBESpecies.Mawile },
            { 356, PBESpecies.Meditite },
            { 357, PBESpecies.Medicham },
            { 358, PBESpecies.Swablu },
            { 359, PBESpecies.Altaria },
            { 360, PBESpecies.Wynaut },
            { 361, PBESpecies.Duskull },
            { 362, PBESpecies.Dusclops },
            { 363, PBESpecies.Roselia },
            { 364, PBESpecies.Slakoth },
            { 365, PBESpecies.Vigoroth },
            { 366, PBESpecies.Slaking },
            { 367, PBESpecies.Gulpin },
            { 368, PBESpecies.Swalot },
            { 369, PBESpecies.Tropius },
            { 370, PBESpecies.Whismur },
            { 371, PBESpecies.Loudred },
            { 372, PBESpecies.Exploud },
            { 373, PBESpecies.Clamperl },
            { 374, PBESpecies.Huntail },
            { 375, PBESpecies.Gorebyss },
            { 376, PBESpecies.Absol },
            { 377, PBESpecies.Shuppet },
            { 378, PBESpecies.Banette },
            { 379, PBESpecies.Seviper },
            { 380, PBESpecies.Zangoose },
            { 381, PBESpecies.Relicanth },
            { 382, PBESpecies.Aron },
            { 383, PBESpecies.Lairon },
            { 384, PBESpecies.Aggron },
            { 385, PBESpecies.Castform },
            { 386, PBESpecies.Volbeat },
            { 387, PBESpecies.Illumise },
            { 388, PBESpecies.Lileep },
            { 389, PBESpecies.Cradily },
            { 390, PBESpecies.Anorith },
            { 391, PBESpecies.Armaldo },
            { 392, PBESpecies.Ralts },
            { 393, PBESpecies.Kirlia },
            { 394, PBESpecies.Gardevoir },
            { 395, PBESpecies.Bagon },
            { 396, PBESpecies.Shelgon },
            { 397, PBESpecies.Salamence },
            { 398, PBESpecies.Beldum },
            { 399, PBESpecies.Metang },
            { 400, PBESpecies.Metagross },
            { 401, PBESpecies.Regirock },
            { 402, PBESpecies.Regice },
            { 403, PBESpecies.Registeel },
            { 404, PBESpecies.Kyogre },
            { 405, PBESpecies.Groudon },
            { 406, PBESpecies.Rayquaza },
            { 407, PBESpecies.Latias },
            { 408, PBESpecies.Latios },
            { 409, PBESpecies.Jirachi },
            { 410, PBESpecies.Deoxys },
            { 411, PBESpecies.Chimecho },
        };
        private static readonly PBEMove[] _gen3TMHMs = new PBEMove[58]
        {
            (PBEMove)264, // FocusPunch
            PBEMove.DragonClaw,
            PBEMove.WaterPulse,
            PBEMove.CalmMind,
            PBEMove.Roar,
            PBEMove.Toxic,
            PBEMove.Hail,
            PBEMove.BulkUp,
            PBEMove.BulletSeed,
            PBEMove.HiddenPower,
            PBEMove.SunnyDay,
            (PBEMove)269, // Taunt
            PBEMove.IceBeam,
            PBEMove.Blizzard,
            (PBEMove)63, // HyperBeam
            PBEMove.LightScreen,
            PBEMove.Protect,
            PBEMove.RainDance,
            PBEMove.GigaDrain,
            PBEMove.Safeguard,
            PBEMove.Frustration,
            (PBEMove)76, // SolarBeam
            PBEMove.IronTail,
            PBEMove.Thunderbolt,
            PBEMove.Thunder,
            PBEMove.Earthquake,
            PBEMove.Return,
            PBEMove.Dig,
            PBEMove.Psychic,
            PBEMove.ShadowBall,
            PBEMove.BrickBreak,
            PBEMove.DoubleTeam,
            PBEMove.Reflect,
            PBEMove.ShockWave,
            PBEMove.Flamethrower,
            PBEMove.SludgeBomb,
            PBEMove.Sandstorm,
            PBEMove.FireBlast,
            PBEMove.RockTomb,
            PBEMove.AerialAce,
            (PBEMove)259, // Torment
            PBEMove.Facade,
            (PBEMove)290, // SecretPower
            PBEMove.Rest,
            (PBEMove)213, // Attract
            (PBEMove)168, // Thief
            PBEMove.SteelWing,
            (PBEMove)285, // SkillSwap
            (PBEMove)289, // Snatch
            PBEMove.Overheat,
            PBEMove.Cut,
            PBEMove.Fly,
            PBEMove.Surf,
            PBEMove.Strength,
            PBEMove.Flash,
            PBEMove.RockSmash,
            PBEMove.Waterfall,
            PBEMove.Dive
        };
        private static readonly PBEMove[] _frlgTutorMoves = new PBEMove[15]
        {
            PBEMove.MegaPunch,
            PBEMove.SwordsDance,
            PBEMove.MegaKick,
            PBEMove.BodySlam,
            PBEMove.DoubleEdge,
            (PBEMove)68, // Counter
            PBEMove.SeismicToss,
            (PBEMove)102, // Mimic
            PBEMove.Metronome,
            PBEMove.Softboiled,
            PBEMove.DreamEater,
            PBEMove.ThunderWave,
            PBEMove.Explosion,
            PBEMove.RockSlide,
            PBEMove.Substitute
        };
        private static readonly PBEMove[] _emeraldTutorMoves = new PBEMove[30]
        {
            PBEMove.MegaPunch,
            PBEMove.SwordsDance,
            PBEMove.MegaKick,
            PBEMove.BodySlam,
            PBEMove.DoubleEdge,
            (PBEMove)68, // Counter
            PBEMove.SeismicToss,
            (PBEMove)102, // Mimic
            PBEMove.Metronome,
            PBEMove.Softboiled,
            PBEMove.DreamEater,
            PBEMove.ThunderWave,
            PBEMove.Explosion,
            PBEMove.RockSlide,
            PBEMove.Substitute,
            PBEMove.DynamicPunch,
            (PBEMove)205, // Rollout
            PBEMove.PsychUp,
            PBEMove.Snore,
            PBEMove.IcyWind,
            (PBEMove)203, // Endure
            PBEMove.MudSlap,
            PBEMove.IcePunch,
            PBEMove.Swagger,
            (PBEMove)214, // SleepTalk
            PBEMove.Swift,
            PBEMove.DefenseCurl,
            PBEMove.ThunderPunch,
            PBEMove.FirePunch,
            (PBEMove)210 // FuryCutter
        };
        private static readonly PBEMove[] _xdTutorMoves = new PBEMove[12]
        {
            PBEMove.BodySlam,
            PBEMove.DoubleEdge,
            PBEMove.SeismicToss,
            (PBEMove)102, // Mimic
            PBEMove.DreamEater,
            PBEMove.ThunderWave,
            PBEMove.Substitute,
            PBEMove.IcyWind,
            PBEMove.Swagger,
            (PBEMove)143, // SkyAttack
            PBEMove.Selfdestruct,
            (PBEMove)171 // Nightmare
        };
        private static readonly Dictionary<int, PBESpecies> _gen4SpeciesIndexToPBESpecies = new Dictionary<int, PBESpecies>
        {
            { 496, PBESpecies.Deoxys_Attack },
            { 497, PBESpecies.Deoxys_Defense },
            { 498, PBESpecies.Deoxys_Speed },
            { 499, PBESpecies.Wormadam_Sandy },
            { 500, PBESpecies.Wormadam_Trash },
            { 501, PBESpecies.Giratina_Origin },
            { 502, PBESpecies.Shaymin_Sky },
            { 503, PBESpecies.Rotom_Heat },
            { 504, PBESpecies.Rotom_Wash },
            { 505, PBESpecies.Rotom_Frost },
            { 506, PBESpecies.Rotom_Fan },
            { 507, PBESpecies.Rotom_Mow }
        };
        private static readonly PBEMove[] _gen4TMHMs = new PBEMove[100]
        {
            (PBEMove)264, // FocusPunch
            PBEMove.DragonClaw,
            PBEMove.WaterPulse,
            PBEMove.CalmMind,
            PBEMove.Roar,
            PBEMove.Toxic,
            PBEMove.Hail,
            PBEMove.BulkUp,
            PBEMove.BulletSeed,
            PBEMove.HiddenPower,
            PBEMove.SunnyDay,
            (PBEMove)269, // Taunt
            PBEMove.IceBeam,
            PBEMove.Blizzard,
            (PBEMove)63, // HyperBeam
            PBEMove.LightScreen,
            PBEMove.Protect,
            PBEMove.RainDance,
            PBEMove.GigaDrain,
            PBEMove.Safeguard,
            PBEMove.Frustration,
            (PBEMove)76, // SolarBeam
            PBEMove.IronTail,
            PBEMove.Thunderbolt,
            PBEMove.Thunder,
            PBEMove.Earthquake,
            PBEMove.Return,
            PBEMove.Dig,
            PBEMove.Psychic,
            PBEMove.ShadowBall,
            PBEMove.BrickBreak,
            PBEMove.DoubleTeam,
            PBEMove.Reflect,
            PBEMove.ShockWave,
            PBEMove.Flamethrower,
            PBEMove.SludgeBomb,
            PBEMove.Sandstorm,
            PBEMove.FireBlast,
            PBEMove.RockTomb,
            PBEMove.AerialAce,
            (PBEMove)259, // Torment
            PBEMove.Facade,
            (PBEMove)290, // SecretPower
            PBEMove.Rest,
            (PBEMove)213, // Attract
            (PBEMove)168, // Thief
            PBEMove.SteelWing,
            (PBEMove)285, // SkillSwap
            (PBEMove)289, // Snatch
            PBEMove.Overheat,
            (PBEMove)355, // Roost
            PBEMove.FocusBlast,
            PBEMove.EnergyBall,
            (PBEMove)206, // FalseSwipe
            PBEMove.Brine,
            (PBEMove)374, // Fling
            PBEMove.ChargeBeam,
            (PBEMove)203, // Endure
            PBEMove.DragonPulse,
            PBEMove.DrainPunch,
            PBEMove.WillOWisp,
            PBEMove.SilverWind,
            (PBEMove)373, // Embargo
            PBEMove.Explosion,
            PBEMove.ShadowClaw,
            (PBEMove)371, // Payback
            (PBEMove)278, // Recycle
            (PBEMove)416, // GigaImpact
            PBEMove.RockPolish,
            PBEMove.Flash,
            PBEMove.StoneEdge,
            (PBEMove)419, // Avalanche
            PBEMove.ThunderWave,
            (PBEMove)360, // GyroBall
            PBEMove.SwordsDance,
            PBEMove.StealthRock,
            PBEMove.PsychUp,
            PBEMove.Captivate,
            PBEMove.DarkPulse,
            PBEMove.RockSlide,
            PBEMove.XScissor,
            (PBEMove)214, // SleepTalk
            (PBEMove)363, // NaturalGift
            PBEMove.PoisonJab,
            PBEMove.DreamEater,
            PBEMove.GrassKnot,
            PBEMove.Swagger,
            (PBEMove)365, // Pluck
            (PBEMove)369, // Uturn
            PBEMove.Substitute,
            PBEMove.FlashCannon,
            PBEMove.TrickRoom,
            PBEMove.Cut,
            PBEMove.Fly,
            PBEMove.Surf,
            PBEMove.Strength,
            PBEMove.None, // Defog/Whirlpool - code will apply the right one
            PBEMove.RockSmash,
            PBEMove.Waterfall,
            PBEMove.RockClimb
        };
        // These tutor moves are copied from overlay_0005.bin address 0x2FF64 to ram address 0x02200CE4 on each map load (USA offsets)
        // The tutor compatibility is at the end of the table (0x3012C and 0x02200EAC [USA offsets]), starting with Bulbasaur and ending with Arceus (no form entries), and each compatibility is a bitfield of 5 bytes
        // Each tutor move entry is 0xC bytes:
        // u16 moveId
        // u8 redShard
        // u8 blueShard
        // u8 yellowShard
        // u8 greenShard
        // u16 unk1
        // u32 areaId (0 = Route 212, 1 = Survival Area, 2 = Snowpoint City)
        private static readonly PBEMove[] _ptTutorMoves = new PBEMove[38]
        {
            PBEMove.Dive,
            PBEMove.MudSlap,
            (PBEMove)210, // FuryCutter
            PBEMove.IcyWind,
            (PBEMove)205, // Rollout
            PBEMove.ThunderPunch,
            PBEMove.FirePunch,
            PBEMove.Superpower,
            PBEMove.IcePunch,
            PBEMove.IronHead,
            PBEMove.AquaTail,
            PBEMove.OminousWind,
            PBEMove.GastroAcid,
            PBEMove.Snore,
            (PBEMove)180, // Spite,
            PBEMove.AirCutter,
            PBEMove.HelpingHand,
            PBEMove.Endeavor,
            (PBEMove)200, // Outrage
            PBEMove.AncientPower,
            PBEMove.Synthesis,
            PBEMove.SignalBeam,
            PBEMove.ZenHeadbutt,
            PBEMove.VacuumWave,
            PBEMove.EarthPower,
            PBEMove.GunkShot,
            PBEMove.Twister,
            PBEMove.SeedBomb,
            PBEMove.IronDefense,
            PBEMove.MagnetRise,
            (PBEMove)387, // LastResort
            (PBEMove)340, // Bounce
            (PBEMove)271, // Trick
            PBEMove.HeatWave,
            (PBEMove)282, // KnockOff
            PBEMove.SuckerPunch,
            PBEMove.Swift,
            (PBEMove)253 // Uproar
        };
        // These tutor moves are decompressed to memory (ram address 0x022093E0 in HG, 0x022093F0 in SS) on each map load (USA offsets)
        // Each tutor move entry is 0x4 bytes:
        // u16 moveId
        // u8 bpCost
        // u8 areaId (0 = Frontier Access [top left tutor], 1 = Frontier Access [top right tutor], 2 = Frontier Access [bottom right tutor], 3 = Ilex Forest [Headbutt tutor])
        private static readonly PBEMove[] _hgssTutorMoves = new PBEMove[52]
        {
            PBEMove.Dive,
            PBEMove.MudSlap,
            (PBEMove)210, // FuryCutter
            PBEMove.IcyWind,
            (PBEMove)205, // Rollout
            PBEMove.ThunderPunch,
            PBEMove.FirePunch,
            PBEMove.Superpower,
            PBEMove.IcePunch,
            PBEMove.IronHead,
            PBEMove.AquaTail,
            PBEMove.OminousWind,
            PBEMove.GastroAcid,
            PBEMove.Snore,
            (PBEMove)180, // Spite
            PBEMove.AirCutter,
            PBEMove.HelpingHand,
            PBEMove.Endeavor,
            (PBEMove)200, // Outrage
            PBEMove.AncientPower,
            PBEMove.Synthesis,
            PBEMove.SignalBeam,
            PBEMove.ZenHeadbutt,
            PBEMove.VacuumWave,
            PBEMove.EarthPower,
            PBEMove.GunkShot,
            PBEMove.Twister,
            PBEMove.SeedBomb,
            PBEMove.IronDefense,
            PBEMove.MagnetRise,
            (PBEMove)387, // LastResort
            (PBEMove)340, // Bounce
            (PBEMove)271, // Trick
            PBEMove.HeatWave,
            (PBEMove)282, // KnockOff
            PBEMove.SuckerPunch,
            PBEMove.Swift,
            (PBEMove)253, // Uproar
            PBEMove.SuperFang,
            PBEMove.PainSplit,
            PBEMove.StringShot,
            PBEMove.Tailwind,
            (PBEMove)356, // Gravity
            PBEMove.WorrySeed,
            (PBEMove)277, // MagicCoat
            PBEMove.RolePlay,
            (PBEMove)215, // HealBell
            PBEMove.LowKick,
            (PBEMove)143, // SkyAttack
            (PBEMove)335, // Block
            (PBEMove)450, // BugBite
            PBEMove.Headbutt
        };
        private static readonly Dictionary<int, PBESpecies> _bwSpeciesIndexToPBESpecies = new Dictionary<int, PBESpecies>
        {
            { 650, PBESpecies.Deoxys_Attack },
            { 651, PBESpecies.Deoxys_Defense },
            { 652, PBESpecies.Deoxys_Speed },
            { 653, PBESpecies.Wormadam_Sandy },
            { 654, PBESpecies.Wormadam_Trash },
            { 655, PBESpecies.Shaymin_Sky },
            { 656, PBESpecies.Giratina_Origin },
            { 657, PBESpecies.Rotom_Heat },
            { 658, PBESpecies.Rotom_Wash },
            { 659, PBESpecies.Rotom_Frost },
            { 660, PBESpecies.Rotom_Fan },
            { 661, PBESpecies.Rotom_Mow },
            { 662, PBESpecies.Castform_Sunny },
            { 663, PBESpecies.Castform_Rainy },
            { 664, PBESpecies.Castform_Snowy },
            { 665, PBESpecies.Basculin_Red },
            { 666, PBESpecies.Darmanitan_Zen },
            { 667, PBESpecies.Meloetta_Pirouette }
        };
        private static readonly Dictionary<int, PBESpecies> _b2w2SpeciesIndexToPBESpecies = new Dictionary<int, PBESpecies>
        {
            { 685, PBESpecies.Deoxys_Attack },
            { 686, PBESpecies.Deoxys_Defense },
            { 687, PBESpecies.Deoxys_Speed },
            { 688, PBESpecies.Wormadam_Sandy },
            { 689, PBESpecies.Wormadam_Trash },
            { 690, PBESpecies.Shaymin_Sky },
            { 691, PBESpecies.Giratina_Origin },
            { 692, PBESpecies.Rotom_Heat },
            { 693, PBESpecies.Rotom_Wash },
            { 694, PBESpecies.Rotom_Frost },
            { 695, PBESpecies.Rotom_Fan },
            { 696, PBESpecies.Rotom_Mow },
            { 697, PBESpecies.Castform_Sunny },
            { 698, PBESpecies.Castform_Rainy },
            { 699, PBESpecies.Castform_Snowy },
            { 700, PBESpecies.Basculin_Red },
            { 701, PBESpecies.Darmanitan_Zen },
            { 702, PBESpecies.Meloetta_Pirouette },
            { 703, PBESpecies.Kyurem_White },
            { 704, PBESpecies.Kyurem_Black },
            { 705, PBESpecies.Keldeo_Resolute },
            { 706, PBESpecies.Tornadus_Therian },
            { 707, PBESpecies.Thundurus_Therian },
            { 708, PBESpecies.Landorus_Therian }
        };
        private static readonly PBEMove[] _gen5TMHMs = new PBEMove[101]
        {
            PBEMove.HoneClaws,
            PBEMove.DragonClaw,
            PBEMove.Psyshock,
            PBEMove.CalmMind,
            PBEMove.Roar,
            PBEMove.Toxic,
            PBEMove.Hail,
            PBEMove.BulkUp,
            PBEMove.Venoshock,
            PBEMove.HiddenPower,
            PBEMove.SunnyDay,
            (PBEMove)269, // Taunt
            PBEMove.IceBeam,
            PBEMove.Blizzard,
            (PBEMove)63, // HyperBeam
            PBEMove.LightScreen,
            PBEMove.Protect,
            PBEMove.RainDance,
            (PBEMove)477, // Telekinesis
            PBEMove.Safeguard,
            PBEMove.Frustration,
            (PBEMove)76, // SolarBeam
            (PBEMove)479, // SmackDown
            PBEMove.Thunderbolt,
            PBEMove.Thunder,
            PBEMove.Earthquake,
            PBEMove.Return,
            PBEMove.Dig,
            PBEMove.Psychic,
            PBEMove.ShadowBall,
            PBEMove.BrickBreak,
            PBEMove.DoubleTeam,
            PBEMove.Reflect,
            PBEMove.SludgeWave,
            PBEMove.Flamethrower,
            PBEMove.SludgeBomb,
            PBEMove.Sandstorm,
            PBEMove.FireBlast,
            PBEMove.RockTomb,
            PBEMove.AerialAce,
            (PBEMove)259, // Torment
            PBEMove.Facade,
            PBEMove.FlameCharge,
            PBEMove.Rest,
            (PBEMove)213, // Attract
            (PBEMove)168, // Thief
            PBEMove.LowSweep,
            (PBEMove)496, // Round
            (PBEMove)497, // EchoedVoice
            PBEMove.Overheat,
            (PBEMove)502, // AllySwitch
            PBEMove.FocusBlast,
            PBEMove.EnergyBall,
            (PBEMove)206, // FalseSwipe
            PBEMove.Scald,
            (PBEMove)374, // Fling
            PBEMove.ChargeBeam,
            (PBEMove)507, // SkyDrop
            (PBEMove)510, // Incinerate
            (PBEMove)511, // Quash
            PBEMove.WillOWisp,
            PBEMove.Acrobatics,
            (PBEMove)373, // Embargo
            PBEMove.Explosion,
            PBEMove.ShadowClaw,
            (PBEMove)371, // Payback
            PBEMove.Retaliate,
            (PBEMove)416, // GigaImpact
            PBEMove.RockPolish,
            PBEMove.Flash,
            PBEMove.StoneEdge,
            (PBEMove)521, // VoltSwitch
            PBEMove.ThunderWave,
            (PBEMove)360, // GyroBall
            PBEMove.SwordsDance,
            PBEMove.StruggleBug,
            PBEMove.PsychUp,
            PBEMove.Bulldoze,
            PBEMove.FrostBreath,
            PBEMove.RockSlide,
            PBEMove.XScissor,
            (PBEMove)525, // DragonTail
            PBEMove.WorkUp,
            PBEMove.PoisonJab,
            PBEMove.DreamEater,
            PBEMove.GrassKnot,
            PBEMove.Swagger,
            (PBEMove)365, // Pluck
            (PBEMove)369, // Uturn
            PBEMove.Substitute,
            PBEMove.FlashCannon,
            PBEMove.TrickRoom,
            PBEMove.WildCharge,
            PBEMove.RockSmash,
            PBEMove.Snarl,
            PBEMove.Cut,
            PBEMove.Fly,
            PBEMove.Surf,
            PBEMove.Strength,
            PBEMove.Waterfall,
            PBEMove.Dive
        };
        private static readonly PBEMove[] _gen5FreeTutorMoves = new PBEMove[7]
        {
            (PBEMove)520, // GrassPledge
            (PBEMove)519, // FirePledge
            (PBEMove)518, // WaterPledge
            (PBEMove)338, // FrenzyPlant
            (PBEMove)307, // BlastBurn
            (PBEMove)308, // HydroCannon
            PBEMove.DracoMeteor
        };
        // These tutor moves are decompressed to memory (ram address 0x021D0B38 in B2, 0x021D0B6C in W2) on each map load (USA offsets)
        // For some reason, the location order in this table is different from the Pokémon's compatibility (this table is [Humilau,Driftveil,Nacrene,Lentimas] but in Pokémon data it is [Driftveil,Lentimas,Humilau,Nacrene])
        // Each tutor move entry is 0xC bytes:
        // u32 moveId
        // u32 shardCost
        // u32 indexInList
        private static readonly PBEMove[][] _b2w2TutorMoves = new PBEMove[4][]
        {
            new PBEMove[15] // Driftveil City
            {
                (PBEMove)450, // BugBite
                (PBEMove)343, // Covet
                PBEMove.SuperFang,
                (PBEMove)530, // DualChop
                PBEMove.SignalBeam,
                PBEMove.IronHead,
                PBEMove.SeedBomb,
                PBEMove.DrillRun,
                (PBEMove)340, // Bounce
                PBEMove.LowKick,
                PBEMove.GunkShot,
                (PBEMove)253, // Uproar
                PBEMove.ThunderPunch,
                PBEMove.FirePunch,
                PBEMove.IcePunch
            },
            new PBEMove[17] // Lentimas Town
            {
                (PBEMove)277, // MagicCoat
                (PBEMove)335, // Block
                PBEMove.EarthPower,
                PBEMove.FoulPlay,
                (PBEMove)356, // Gravity
                PBEMove.MagnetRise,
                PBEMove.IronDefense,
                (PBEMove)387, // LastResort
                PBEMove.Superpower,
                PBEMove.Electroweb,
                PBEMove.IcyWind,
                PBEMove.AquaTail,
                PBEMove.DarkPulse,
                PBEMove.ZenHeadbutt,
                PBEMove.DragonPulse,
                PBEMove.HyperVoice,
                PBEMove.IronTail
            },
            new PBEMove[13] // Humilau City
            {
                (PBEMove)20, // Bind
                PBEMove.Snore,
                (PBEMove)282, // KnockOff
                PBEMove.Synthesis,
                PBEMove.HeatWave,
                PBEMove.RolePlay,
                (PBEMove)215, // HealBell
                PBEMove.Tailwind,
                (PBEMove)143, // SkyAttack
                PBEMove.PainSplit,
                PBEMove.GigaDrain,
                PBEMove.DrainPunch,
                (PBEMove)355 // Roost
            },
            new PBEMove[15] // Nacrene City
            {
                PBEMove.GastroAcid,
                PBEMove.WorrySeed,
                (PBEMove)180, // Spite
                (PBEMove)495, // AfterYou
                PBEMove.HelpingHand,
                (PBEMove)271, // Trick
                (PBEMove)478, // MagicRoom
                (PBEMove)472, // WonderRoom
                PBEMove.Endeavor,
                (PBEMove)200, // Outrage
                (PBEMove)278, // Recycle
                (PBEMove)289, // Snatch
                PBEMove.StealthRock,
                (PBEMove)214, // SleepTalk
                (PBEMove)285 // SkillSwap
            }
        };

        // You must dump everything yourself
        // The GBA ROMs must all be USA v1.0
        // Colo and XD must be USA
        // DPPt dumps use overlay files which may or may not have different offsets depending on the region, so just keep in mind I have USA versions of each game
        // HGSS/Gen5 dumps should work across all regions
        //
        // Colo and XD level-up moves are in common.fsys/common_rel.fdat
        //
        // D, P, and Pt level-up move NARC is /poketool/personal/wotbl.narc (D and P have identical level-up move NARCs)
        // D, P, and Pt egg moves are in overlay/overlay_0005.bin
        // Pt TMHM moves are in the Pokémon data NARC which is /poketool/personal/pl_personal.narc (Pt changed no TMHM compatibility from DP so I use it alone)
        // Pt tutor compatibility is in overlay/overlay_0005.bin
        // HG and SS level-up move NARC is /a/0/3/3 (HG and SS have identical level-up move NARCs)
        // HG and SS TMHM moves are in the Pokémon data NARC which is /a/0/0/2 (HG and SS have identical Pokémon data NARCs)
        // HG and SS tutor compatibility is in /fielddata/wazaoshie/waza_oshie.bin (HG and SS have identical tutor compatibility)
        // HG and SS egg move NARC is /a/2/2/9 (HG and SS have identical egg move NARCs)
        //
        // B2 and W2 evolution NARC is /a/0/1/9 (B2 and W2 have identical evolution NARCs)
        // B, W, B2, and W2 level-up move NARC is /a/0/1/8 (B and W have identical level-up move NARCs) (B2 and W2 have identical level-up move NARCs)
        // B, W, B2, and W2 TMHM moves are in the Pokémon data NARC which is /a/0/1/6 (B and W have identical Pokémon data NARCs) (B2 and W2 have identical Pokémon data NARCs)
        // B2 and W2 tutor compatibility is in the Pokémon data NARC which is /a/0/1/6 (B2 and W2 have identical Pokémon data NARCs)
        // B and W egg move NARC is /a/1/2/3, B2 and W2 egg move NARC is /a/1/2/4 (B, W, B2, and W2 have identical egg move NARCs)
        //
        // TODO: XD - Mew special tutor moves
        // TODO: FRLG - Ultimate starter tutor moves
        // TODO: D, P, Pt - Free tutor moves
        // TODO: HG, SS - Free tutor moves (aside from Headbutt)
        // TODO: BW, B2W2 - SecretSword, RelicSong
        // TODO: Rotom special moves
        // TODO: Arceus_Dragon gets DracoMeteor from free tutor moves
        // TODO: Basculin_Blue extra abilities
        // TODO: Pichu & VoltTackle
        // TODO: Egg move logic (currently we only kept the possible egg moves, but no logic that assigns them, so it is incorrect)
#pragma warning disable CS8321 // Local function is declared but never used
        public static void Run(SqliteConnection con)
        {
            using (var r = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\R.gba"), Endianness.LittleEndian))
            using (var s = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\S.gba"), Endianness.LittleEndian))
            using (var fr = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\FR.gba"), Endianness.LittleEndian))
            using (var lg = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\LG.gba"), Endianness.LittleEndian))
            using (var e = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\E.gba"), Endianness.LittleEndian))
            using (var coloCommonRel = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\Colocommon_rel.fdat"), Endianness.BigEndian))
            using (var xdCommonRel = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\XDcommon_rel.fdat"), Endianness.BigEndian))
            using (SqliteTransaction transaction = con.BeginTransaction())
            using (SqliteCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = transaction;

                var dict = new Dictionary<PBESpecies, Pokemon>();
                void AddSpecies(PBESpecies species)
                {
                    if (!dict.ContainsKey(species))
                    {
                        dict.Add(species, new Pokemon());
                    }
                }
                void AddLevelUpMove(PBESpecies species, PBEMove move, byte level, PBEMoveObtainMethod flag)
                {
                    AddSpecies(species);
                    (PBEMove, byte) key = (move, level);
                    if (dict[species].LevelUpMoves.ContainsKey(key))
                    {
                        dict[species].LevelUpMoves[key] |= flag;
                    }
                    else
                    {
                        dict[species].LevelUpMoves.Add(key, flag);
                    }
                }
                void AddOtherMove(PBESpecies species, PBEMove move, PBEMoveObtainMethod flag)
                {
                    AddSpecies(species);
                    if (dict[species].OtherMoves.ContainsKey(move))
                    {
                        dict[species].OtherMoves[move] |= flag;
                    }
                    else
                    {
                        dict[species].OtherMoves.Add(move, flag);
                    }
                }
                void Merge(params PBESpecies[] species)
                {
                    PBESpecies baseSpecies = species[0];
                    Pokemon basePkmn = dict[baseSpecies];
                    foreach (PBESpecies sp in species)
                    {
                        AddSpecies(sp);
                        Pokemon pkmn = dict[sp];
                        if (pkmn != basePkmn)
                        {
                            if (!_b2w2SpeciesIndexToPBESpecies.ContainsValue(sp)) // If the Pokémon does not have pokedata defined
                            {
                                pkmn.HP = basePkmn.HP;
                                pkmn.Attack = basePkmn.Attack;
                                pkmn.Defense = basePkmn.Defense;
                                pkmn.SpAttack = basePkmn.SpAttack;
                                pkmn.SpDefense = basePkmn.SpDefense;
                                pkmn.Speed = basePkmn.Speed;
                                pkmn.Abilities = basePkmn.Abilities;
                                pkmn.GenderRatio = basePkmn.GenderRatio;
                                pkmn.Type1 = basePkmn.Type1;
                                pkmn.Type2 = basePkmn.Type2;
                                pkmn.Weight = basePkmn.Weight;
                            }
                            foreach (KeyValuePair<(PBEMove Move, byte Level), PBEMoveObtainMethod> levelup in pkmn.LevelUpMoves)
                            {
                                AddLevelUpMove(baseSpecies, levelup.Key.Move, levelup.Key.Level, levelup.Value);
                            }
                            foreach (KeyValuePair<PBEMove, PBEMoveObtainMethod> other in pkmn.OtherMoves)
                            {
                                AddOtherMove(baseSpecies, other.Key, other.Value);
                            }
                        }
                    }
                    for (int i = 1; i < species.Length; i++)
                    {
                        Pokemon pkmn = dict[species[i]];
                        pkmn.LevelUpMoves = basePkmn.LevelUpMoves;
                        pkmn.OtherMoves = basePkmn.OtherMoves;
                    }
                }

                #region Pokémon Data

                {
                    var b2w2Pokedata = new NARC(@"../../../\DumpedData\B2W2Pokedata.narc");
                    var b2w2Evolution = new NARC(@"../../../\DumpedData\B2W2Evolution.narc");
                    for (int sp = 1; sp <= 708; sp++)
                    {
                        // Skip Egg, Bad Egg, and Pokéstar Studios Pokémon
                        if (sp <= 649 || sp >= 685)
                        {
                            if (!_b2w2SpeciesIndexToPBESpecies.TryGetValue(sp, out PBESpecies species))
                            {
                                species = (PBESpecies)sp;
                            }
                            AddSpecies(species);
                            using (var pokedata = new EndianBinaryReader(new MemoryStream(b2w2Pokedata[sp]), Endianness.LittleEndian))
                            using (var evolution = new EndianBinaryReader(new MemoryStream(b2w2Evolution[sp]), Endianness.LittleEndian))
                            {
                                Pokemon pkmn = dict[species];
                                // Pokedata
                                pkmn.HP = pokedata.ReadByte(0x0);
                                pkmn.Attack = pokedata.ReadByte(0x1);
                                pkmn.Defense = pokedata.ReadByte(0x2);
                                pkmn.SpAttack = pokedata.ReadByte(0x4);
                                pkmn.SpDefense = pokedata.ReadByte(0x5);
                                pkmn.Speed = pokedata.ReadByte(0x3);
                                pkmn.Type1 = Utils.Gen5Types[pokedata.ReadByte(0x6)];
                                pkmn.Type2 = Utils.Gen5Types[pokedata.ReadByte(0x7)];
                                if (pkmn.Type1 == pkmn.Type2)
                                {
                                    pkmn.Type2 = PBEType.None;
                                }
                                pkmn.GenderRatio = (PBEGenderRatio)pokedata.ReadByte(0x12);
                                for (int i = 0; i < 3; i++)
                                {
                                    var ability = (PBEAbility)pokedata.ReadByte(0x18 + i);
                                    if (ability != PBEAbility.None && !pkmn.Abilities.Contains(ability))
                                    {
                                        pkmn.Abilities.Add(ability);
                                    }
                                }
                                pkmn.Weight = Math.Round(pokedata.ReadUInt16(0x26) * 0.1, 1);
                                // Evolution
                                for (int i = 0; i < 7; i++)
                                {
                                    ushort method = evolution.ReadUInt16();
                                    evolution.ReadUInt16(); // Param
                                    var evo = (PBESpecies)evolution.ReadUInt16();
                                    if (method != 0)
                                    {
                                        pkmn.Evolutions.Add(evo);
                                        AddSpecies(evo);
                                        dict[evo].PreEvolutions.Add(species);
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Level Up Moves

                // Gen 3
                for (int sp = 1; sp <= 411; sp++)
                {
                    // Gen 2 Unown slots are ignored in gen 3
                    if (sp <= 251 || sp >= 277)
                    {
                        // It is the same in Ruby, Sapphire, Colo, and XD; the others have some differences
                        r.BaseStream.Position = 0x207BC8 + (sizeof(uint) * sp);
                        s.BaseStream.Position = 0x207B58 + (sizeof(uint) * sp);
                        fr.BaseStream.Position = 0x25D7B4 + (sizeof(uint) * sp);
                        lg.BaseStream.Position = 0x25D794 + (sizeof(uint) * sp);
                        e.BaseStream.Position = 0x32937C + (sizeof(uint) * sp);
                        coloCommonRel.BaseStream.Position = 0x123250 + (0x11C * sp) + 0xBA;
                        xdCommonRel.BaseStream.Position = 0x29DA8 + (0x124 * sp) + 0xC4;
                        void ReadGBALevelUpMoves(EndianBinaryReader reader, PBEMoveObtainMethod flag)
                        {
                            PBESpecies species = _gen3SpeciesIndexToPBESpecies[sp];
                            if (species == PBESpecies.Deoxys)
                            {
                                if (reader == e)
                                {
                                    species = PBESpecies.Deoxys_Speed;
                                }
                                else if (reader == lg)
                                {
                                    species = PBESpecies.Deoxys_Defense;
                                }
                                else if (reader == fr)
                                {
                                    species = PBESpecies.Deoxys_Attack;
                                }
                            }
                            reader.BaseStream.Position = reader.ReadUInt32() - 0x8000000;
                            while (true)
                            {
                                ushort val = reader.ReadUInt16();
                                if (val == 0xFFFF)
                                {
                                    break;
                                }
                                else
                                {
                                    AddLevelUpMove(species, (PBEMove)(val & 0x1FF), (byte)(val >> 9), flag);
                                }
                            }
                        }
                        ReadGBALevelUpMoves(r, PBEMoveObtainMethod.LevelUp_RSColoXD);
                        //ReadGBALevelUpMoves(s, PBEMoveObtainMethod.LevelUp_RSColoXD);
                        ReadGBALevelUpMoves(fr, PBEMoveObtainMethod.LevelUp_FR);
                        ReadGBALevelUpMoves(lg, PBEMoveObtainMethod.LevelUp_LG);
                        ReadGBALevelUpMoves(e, PBEMoveObtainMethod.LevelUp_E);
                        void ReadGCLevelUpMoves(EndianBinaryReader reader, PBEMoveObtainMethod flag)
                        {
                            PBESpecies species = _gen3SpeciesIndexToPBESpecies[sp];
                            for (int i = 0; i < 17; i++)
                            {
                                byte level = reader.ReadByte();
                                reader.ReadByte(); // Padding
                                var move = (PBEMove)reader.ReadUInt16();
                                if (move == PBEMove.None)
                                {
                                    break;
                                }
                                else
                                {
                                    AddLevelUpMove(species, move, level, flag);
                                }
                            }
                        }
                        //ReadGCLevelUpMoves(coloCommonRel, PBEMoveObtainMethod.LevelUp_RSColoXD);
                        //ReadGCLevelUpMoves(xdCommonRel, PBEMoveObtainMethod.LevelUp_RSColoXD);
                    }
                }
                // Gen 4
                {
                    var dp = new NARC(@"../../../\DumpedData\DPLevelUp.narc");
                    var pt = new NARC(@"../../../\DumpedData\PtLevelUp.narc");
                    var hgss = new NARC(@"../../../\DumpedData\HGSSLevelUp.narc");
                    for (int sp = 1; sp <= 507; sp++)
                    {
                        // 494 is Egg, 495 is Bad Egg
                        if (sp != 494 && sp != 495)
                        {
                            if (!_gen4SpeciesIndexToPBESpecies.TryGetValue(sp, out PBESpecies species))
                            {
                                species = (PBESpecies)sp;
                            }
                            void ReadLevelUpMoves(byte[] file, PBEMoveObtainMethod flag)
                            {
                                using (var reader = new EndianBinaryReader(new MemoryStream(file), Endianness.LittleEndian))
                                {
                                    while (true)
                                    {
                                        ushort val = reader.ReadUInt16();
                                        if (val == 0xFFFF)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            AddLevelUpMove(species, (PBEMove)(val & 0x1FF), (byte)(val >> 9), flag);
                                        }
                                    }
                                }
                            }
                            // DP only has 0-500
                            if (sp <= 500)
                            {
                                ReadLevelUpMoves(dp[sp], PBEMoveObtainMethod.LevelUp_DP);
                            }
                            ReadLevelUpMoves(pt[sp], PBEMoveObtainMethod.LevelUp_Pt);
                            ReadLevelUpMoves(hgss[sp], PBEMoveObtainMethod.LevelUp_HGSS);
                        }
                    }
                }
                // Gen 5
                {
                    var bw = new NARC(@"../../../\DumpedData\BWLevelUp.narc");
                    var b2w2 = new NARC(@"../../../\DumpedData\B2W2LevelUp.narc");
                    for (int sp = 1; sp <= 708; sp++)
                    {
                        void ReadLevelUpMoves(byte[] file, bool isBW)
                        {
                            if (!(isBW ? _bwSpeciesIndexToPBESpecies : _b2w2SpeciesIndexToPBESpecies).TryGetValue(sp, out PBESpecies species))
                            {
                                species = (PBESpecies)sp;
                            }
                            using (var reader = new EndianBinaryReader(new MemoryStream(file), Endianness.LittleEndian))
                            {
                                while (true)
                                {
                                    uint val = reader.ReadUInt32();
                                    if (val == 0xFFFFFFFF)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        AddLevelUpMove(species, (PBEMove)val, (byte)(val >> 0x10), isBW ? PBEMoveObtainMethod.LevelUp_BW : PBEMoveObtainMethod.LevelUp_B2W2);
                                    }
                                }
                            }
                        }
                        // BW only has 0-667 (no Egg or Bad Egg)
                        if (sp <= 667)
                        {
                            ReadLevelUpMoves(bw[sp], true);
                        }
                        // Skip Egg, Bad Egg, and Pokéstar Studios Pokémon in B2W2
                        if (sp <= 649 || sp >= 685)
                        {
                            ReadLevelUpMoves(b2w2[sp], false);
                        }
                    }
                }

                #endregion

                #region TMHM Compatibility

                // Gen 3
                for (int sp = 1; sp <= 411; sp++)
                {
                    // Gen 2 Unown slots are ignored in gen 3
                    if (sp <= 251 || sp >= 277)
                    {
                        PBESpecies species = _gen3SpeciesIndexToPBESpecies[sp];
                        // It is the same across all of gen 3, so I will only read one
                        r.BaseStream.Position = 0x1FD0F0 + (8 * sp);
                        s.BaseStream.Position = 0x1FD080 + (8 * sp);
                        fr.BaseStream.Position = 0x252BC8 + (8 * sp);
                        lg.BaseStream.Position = 0x252BA4 + (8 * sp);
                        e.BaseStream.Position = 0x31E898 + (8 * sp);
                        coloCommonRel.BaseStream.Position = 0x123250 + (0x11C * sp) + 0x34;
                        xdCommonRel.BaseStream.Position = 0x29DA8 + (0x124 * sp) + 0x34;
                        PBEMoveObtainMethod GetFlag(int i)
                        {
                            return i < 50 ? PBEMoveObtainMethod.TM_RSFRLGEColoXD : PBEMoveObtainMethod.HM_RSFRLGEColoXD;
                        }
                        void ReadGBATMHM(EndianBinaryReader reader)
                        {
                            byte[] bytes = reader.ReadBytes(8);
                            for (int i = 0; i < _gen3TMHMs.Length; i++)
                            {
                                if ((bytes[i / 8] & (1 << (i % 8))) != 0)
                                {
                                    AddOtherMove(species, _gen3TMHMs[i], GetFlag(i));
                                }
                            }
                        }
                        ReadGBATMHM(r);
                        //ReadGBATMHM(s);
                        //ReadGBATMHM(fr);
                        //ReadGBATMHM(lg);
                        //ReadGBATMHM(e);
                        void ReadGCTMHM(EndianBinaryReader reader)
                        {
                            for (int i = 0; i < _gen3TMHMs.Length; i++)
                            {
                                if (reader.ReadBoolean())
                                {
                                    AddOtherMove(species, _gen3TMHMs[i], GetFlag(i));
                                }
                            }
                        }
                        //ReadGCTMHM(coloCommonRel);
                        //ReadGCTMHM(xdCommonRel);
                    }
                }
                // Gen 4
                {
                    var dppt = new NARC(@"../../../\DumpedData\PtPokedata.narc");
                    var hgss = new NARC(@"../../../\DumpedData\HGSSPokedata.narc");
                    for (int sp = 1; sp <= 507; sp++)
                    {
                        // 494 is Egg, 495 is Bad Egg
                        if (sp != 494 && sp != 495)
                        {
                            if (!_gen4SpeciesIndexToPBESpecies.TryGetValue(sp, out PBESpecies species))
                            {
                                species = (PBESpecies)sp;
                            }
                            void ReadTMHMMoves(byte[] file, bool isDPPt)
                            {
                                using (var reader = new EndianBinaryReader(new MemoryStream(file), Endianness.LittleEndian))
                                {
                                    byte[] bytes = reader.ReadBytes(13, 0x1C);
                                    for (int i = 0; i < _gen4TMHMs.Length; i++)
                                    {
                                        if ((bytes[i / 8] & (1 << (i % 8))) != 0)
                                        {
                                            PBEMove move = _gen4TMHMs[i];
                                            if (move == PBEMove.None)
                                            {
                                                move = isDPPt ? (PBEMove)432 : (PBEMove)250;
                                            }
                                            AddOtherMove(species, move, i < 92 ? (isDPPt ? PBEMoveObtainMethod.TM_DPPt : PBEMoveObtainMethod.TM_HGSS) : (isDPPt ? PBEMoveObtainMethod.HM_DPPt : PBEMoveObtainMethod.HM_HGSS));
                                        }
                                    }
                                }
                            }
                            ReadTMHMMoves(dppt[sp], true);
                            ReadTMHMMoves(hgss[sp], false);
                        }
                    }
                }
                // Gen 5
                {
                    var bw = new NARC(@"../../../\DumpedData\BWPokedata.narc");
                    var b2w2 = new NARC(@"../../../\DumpedData\B2W2Pokedata.narc");
                    for (int sp = 1; sp <= 708; sp++)
                    {
                        void ReadTMHMMoves(byte[] file, bool isBW)
                        {
                            if (!(isBW ? _bwSpeciesIndexToPBESpecies : _b2w2SpeciesIndexToPBESpecies).TryGetValue(sp, out PBESpecies species))
                            {
                                species = (PBESpecies)sp;
                            }
                            using (var reader = new EndianBinaryReader(new MemoryStream(file), Endianness.LittleEndian))
                            {
                                byte[] bytes = reader.ReadBytes(13, 0x28);
                                for (int i = 0; i < _gen5TMHMs.Length; i++)
                                {
                                    if ((bytes[i / 8] & (1 << (i % 8))) != 0)
                                    {
                                        PBEMoveObtainMethod flag;
                                        if (i < 95)
                                        {
                                            flag = isBW ? PBEMoveObtainMethod.TM_BW : PBEMoveObtainMethod.TM_B2W2;
                                        }
                                        else
                                        {
                                            flag = PBEMoveObtainMethod.HM_BWB2W2;
                                        }
                                        AddOtherMove(species, _gen5TMHMs[i], flag);
                                    }
                                }
                            }
                        }
                        // BW only has 0-667 (no Egg or Bad Egg)
                        if (sp <= 667)
                        {
                            ReadTMHMMoves(bw[sp], true);
                        }
                        // Skip Egg, Bad Egg, and Pokéstar Studios Pokémon in B2W2
                        if (sp <= 649 || sp >= 685)
                        {
                            ReadTMHMMoves(b2w2[sp], false);
                        }
                    }
                }

                #endregion

                #region Move Tutor

                // Gen 3 - FRLGE
                for (int sp = 1; sp <= 411; sp++)
                {
                    // Gen 2 Unown slots are ignored in gen 3
                    if (sp <= 251 || sp >= 277)
                    {
                        PBESpecies species = _gen3SpeciesIndexToPBESpecies[sp];
                        // It is the same in FR and LG, so I will only read one
                        fr.BaseStream.Position = 0x459B7E + (sizeof(ushort) * sp);
                        lg.BaseStream.Position = 0x45959E + (sizeof(ushort) * sp);
                        e.BaseStream.Position = 0x615048 + (sizeof(uint) * sp);
                        void ReadTutorMoves(uint val, PBEMove[] tutorMoves, PBEMoveObtainMethod flag)
                        {
                            for (int i = 0; i < tutorMoves.Length; i++)
                            {
                                if ((val & (1u << i)) != 0)
                                {
                                    AddOtherMove(species, tutorMoves[i], flag);
                                }
                            }
                        }
                        ReadTutorMoves(fr.ReadUInt16(), _frlgTutorMoves, PBEMoveObtainMethod.MoveTutor_FRLG);
                        //ReadTutorMoves(lg.ReadUInt16(), frlgTutorMoves, PBEMoveObtainMethod.MoveTutor_FRLG);
                        ReadTutorMoves(e.ReadUInt32(), _emeraldTutorMoves, PBEMoveObtainMethod.MoveTutor_E);
                    }
                }
                // Gen 3 - XD
                for (int sp = 1; sp <= 411; sp++)
                {
                    // Gen 2 Unown slots are ignored in gen 3
                    if (sp <= 251 || sp >= 277)
                    {
                        PBESpecies species = _gen3SpeciesIndexToPBESpecies[sp];
                        xdCommonRel.BaseStream.Position = 0x29DA8 + (0x124 * sp) + 0x6E;
                        for (int i = 0; i < _xdTutorMoves.Length; i++)
                        {
                            if (xdCommonRel.ReadBoolean())
                            {
                                AddOtherMove(species, _xdTutorMoves[i], PBEMoveObtainMethod.MoveTutor_XD);
                            }
                        }
                    }
                }
                // Gen 4
                using (var pt = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\Ptoverlay_0005.bin"), Endianness.LittleEndian))
                {
                    for (int sp = 1; sp <= 493; sp++)
                    {
                        var species = (PBESpecies)sp;
                        byte[] bytes = pt.ReadBytes(5, 0x3012C + (5 * (sp - 1)));
                        for (int i = 0; i < _ptTutorMoves.Length; i++)
                        {
                            if ((bytes[i / 8] & (1 << (i % 8))) != 0)
                            {
                                AddOtherMove(species, _ptTutorMoves[i], PBEMoveObtainMethod.MoveTutor_Pt);
                            }
                        }
                    }
                }
                using (var hgss = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\HGSSTutor.bin"), Endianness.LittleEndian))
                {
                    for (int sp = 1; sp <= 505; sp++) // Includes forms but not eggs
                    {
                        PBESpecies species = sp > 493 ? _gen4SpeciesIndexToPBESpecies[sp + 2] : (PBESpecies)sp;
                        byte[] bytes = hgss.ReadBytes(8);
                        for (int i = 0; i < _hgssTutorMoves.Length; i++)
                        {
                            if ((bytes[i / 8] & (1 << (i % 8))) != 0)
                            {
                                AddOtherMove(species, _hgssTutorMoves[i], PBEMoveObtainMethod.MoveTutor_HGSS);
                            }
                        }
                    }
                }
                // Gen 5
                {
                    var bw = new NARC(@"../../../\DumpedData\BWPokedata.narc");
                    var b2w2 = new NARC(@"../../../\DumpedData\B2W2Pokedata.narc");
                    for (int sp = 1; sp <= 708; sp++)
                    {
                        void ReadFreeTutorMoves(EndianBinaryReader reader, bool isBW)
                        {
                            if (!(isBW ? _bwSpeciesIndexToPBESpecies : _b2w2SpeciesIndexToPBESpecies).TryGetValue(sp, out PBESpecies species))
                            {
                                species = (PBESpecies)sp;
                            }
                            byte val = reader.ReadByte(0x38);
                            for (int i = 0; i < _gen5FreeTutorMoves.Length; i++)
                            {
                                if ((val & (1 << i)) != 0)
                                {
                                    AddOtherMove(species, _gen5FreeTutorMoves[i], isBW ? PBEMoveObtainMethod.MoveTutor_BW : PBEMoveObtainMethod.MoveTutor_B2W2);
                                }
                            }
                        }
                        void ReadB2W2TutorMoves(EndianBinaryReader reader)
                        {
                            if (!_b2w2SpeciesIndexToPBESpecies.TryGetValue(sp, out PBESpecies species))
                            {
                                species = (PBESpecies)sp;
                            }
                            for (int i = 0; i < _b2w2TutorMoves.Length; i++)
                            {
                                uint val = reader.ReadUInt32(0x3C + (sizeof(uint) * i));
                                for (int j = 0; j < _b2w2TutorMoves[i].Length; j++)
                                {
                                    if ((val & (1u << j)) != 0)
                                    {
                                        AddOtherMove(species, _b2w2TutorMoves[i][j], PBEMoveObtainMethod.MoveTutor_B2W2);
                                    }
                                }
                            }
                        }
                        // BW only has 0-667 (no Egg or Bad Egg)
                        if (sp <= 667)
                        {
                            using (var reader = new EndianBinaryReader(new MemoryStream(bw[sp]), Endianness.LittleEndian))
                            {
                                ReadFreeTutorMoves(reader, true);
                            }
                        }
                        // Skip Egg, Bad Egg, and Pokéstar Studios Pokémon
                        if (sp <= 649 || sp >= 685)
                        {
                            using (var reader = new EndianBinaryReader(new MemoryStream(b2w2[sp]), Endianness.LittleEndian))
                            {
                                ReadFreeTutorMoves(reader, false);
                                ReadB2W2TutorMoves(reader);
                            }
                        }
                    }
                }

                #endregion

                #region Egg Moves

                // Gen 3 & Gen 4
                using (var d = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\Doverlay_0005.bin"), Endianness.LittleEndian))
                using (var p = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\Poverlay_0005.bin"), Endianness.LittleEndian))
                using (var pt = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\Ptoverlay_0005.bin"), Endianness.LittleEndian))
                using (var hgss = new EndianBinaryReader(new MemoryStream(new NARC(@"../../../\DumpedData\HGSSEgg.narc")[0]), Endianness.LittleEndian))
                {
                    // The table is the same in all five GBA games, so I will only read one
                    r.BaseStream.Position = 0x2091DC;
                    s.BaseStream.Position = 0x20916C;
                    fr.BaseStream.Position = 0x25EF0C;
                    lg.BaseStream.Position = 0x25EEEC;
                    e.BaseStream.Position = 0x32ADD8;
                    // The table is the same across DPPt, so I will only read one
                    d.BaseStream.Position = 0x20668;
                    p.BaseStream.Position = 0x20668;
                    pt.BaseStream.Position = 0x29222;
                    void ReadEggMoves(EndianBinaryReader reader, bool isGen3, PBEMoveObtainMethod flag)
                    {
                        PBESpecies species = 0;
                        while (true)
                        {
                            ushort val = reader.ReadUInt16();
                            if (val == 0xFFFF)
                            {
                                break;
                            }
                            else if (val > 20000)
                            {
                                int speciesIndex = val - 20000;
                                species = isGen3 ? _gen3SpeciesIndexToPBESpecies[speciesIndex] : (PBESpecies)speciesIndex;
                            }
                            else
                            {
                                AddOtherMove(species, (PBEMove)val, flag);
                            }
                        }
                    }
                    ReadEggMoves(r, true, PBEMoveObtainMethod.EggMove_RSFRLGE);
                    //ReadEggMoves(s, true, PBEMoveObtainMethod.EggMove_RSFRLGE);
                    //ReadEggMoves(fr, true, PBEMoveObtainMethod.EggMove_RSFRLGE);
                    //ReadEggMoves(lg, true, PBEMoveObtainMethod.EggMove_RSFRLGE);
                    //ReadEggMoves(e, true, PBEMoveObtainMethod.EggMove_RSFRLGE);
                    ReadEggMoves(d, false, PBEMoveObtainMethod.EggMove_DPPt);
                    //ReadEggMoves(p, false, PBEMoveObtainMethod.EggMove_DPPt);
                    //ReadEggMoves(pt, false, PBEMoveObtainMethod.EggMove_DPPt);
                    ReadEggMoves(hgss, false, PBEMoveObtainMethod.EggMove_HGSS);
                }
                // Gen 5
                {
                    var bwb2w2 = new NARC(@"../../../\DumpedData\BWB2W2Egg.narc");
                    for (int sp = 1; sp <= 649; sp++)
                    {
                        using (var reader = new EndianBinaryReader(new MemoryStream(bwb2w2[sp]), Endianness.LittleEndian))
                        {
                            ushort numEggMoves = reader.ReadUInt16();
                            if (numEggMoves > 0)
                            {
                                var species = (PBESpecies)sp;
                                for (int i = 0; i < numEggMoves; i++)
                                {
                                    AddOtherMove(species, (PBEMove)reader.ReadUInt16(), PBEMoveObtainMethod.EggMove_BWB2W2);
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Specific Fixes

                Merge(PBESpecies.Unown_A, PBESpecies.Unown_B, PBESpecies.Unown_C, PBESpecies.Unown_D, PBESpecies.Unown_E, PBESpecies.Unown_Exclamation, PBESpecies.Unown_F, PBESpecies.Unown_G, PBESpecies.Unown_H, PBESpecies.Unown_I, PBESpecies.Unown_J, PBESpecies.Unown_K, PBESpecies.Unown_L, PBESpecies.Unown_M, PBESpecies.Unown_N, PBESpecies.Unown_O, PBESpecies.Unown_P, PBESpecies.Unown_Q, PBESpecies.Unown_Question, PBESpecies.Unown_R, PBESpecies.Unown_S, PBESpecies.Unown_T, PBESpecies.Unown_U, PBESpecies.Unown_V, PBESpecies.Unown_W, PBESpecies.Unown_X, PBESpecies.Unown_Y, PBESpecies.Unown_Z);
                Merge(PBESpecies.Castform, PBESpecies.Castform_Rainy, PBESpecies.Castform_Snowy, PBESpecies.Castform_Sunny);
                Merge(PBESpecies.Deoxys, PBESpecies.Deoxys_Attack, PBESpecies.Deoxys_Defense, PBESpecies.Deoxys_Speed);
                Merge(PBESpecies.Burmy_Plant, PBESpecies.Burmy_Sandy, PBESpecies.Burmy_Trash);
                dict[PBESpecies.Burmy_Sandy].Evolutions.Add(PBESpecies.Wormadam_Sandy);
                dict[PBESpecies.Burmy_Sandy].Evolutions.Add(PBESpecies.Mothim);
                dict[PBESpecies.Burmy_Trash].Evolutions.Add(PBESpecies.Wormadam_Trash);
                dict[PBESpecies.Burmy_Trash].Evolutions.Add(PBESpecies.Mothim);
                dict[PBESpecies.Wormadam_Sandy].PreEvolutions.Add(PBESpecies.Burmy_Sandy);
                dict[PBESpecies.Wormadam_Trash].PreEvolutions.Add(PBESpecies.Burmy_Trash);
                dict[PBESpecies.Mothim].PreEvolutions.Add(PBESpecies.Burmy_Sandy);
                dict[PBESpecies.Mothim].PreEvolutions.Add(PBESpecies.Burmy_Trash);
                Merge(PBESpecies.Cherrim, PBESpecies.Cherrim_Sunshine);
                Merge(PBESpecies.Shellos_East, PBESpecies.Shellos_West);
                dict[PBESpecies.Shellos_West].Evolutions.Add(PBESpecies.Gastrodon_West);
                Merge(PBESpecies.Gastrodon_East, PBESpecies.Gastrodon_West);
                dict[PBESpecies.Gastrodon_West].PreEvolutions.Add(PBESpecies.Shellos_West);
                Merge(PBESpecies.Rotom, PBESpecies.Rotom_Fan, PBESpecies.Rotom_Frost, PBESpecies.Rotom_Heat, PBESpecies.Rotom_Mow, PBESpecies.Rotom_Wash);
                Merge(PBESpecies.Giratina, PBESpecies.Giratina_Origin);
                Merge(PBESpecies.Shaymin, PBESpecies.Shaymin_Sky);
                Merge(PBESpecies.Arceus, PBESpecies.Arceus_Bug, PBESpecies.Arceus_Dark, PBESpecies.Arceus_Dragon, PBESpecies.Arceus_Electric, PBESpecies.Arceus_Fighting, PBESpecies.Arceus_Fire, PBESpecies.Arceus_Flying, PBESpecies.Arceus_Ghost, PBESpecies.Arceus_Grass, PBESpecies.Arceus_Ground, PBESpecies.Arceus_Ice, PBESpecies.Arceus_Poison, PBESpecies.Arceus_Psychic, PBESpecies.Arceus_Rock, PBESpecies.Arceus_Steel, PBESpecies.Arceus_Water);
                dict[PBESpecies.Arceus_Bug].Type1 = PBEType.Bug;
                dict[PBESpecies.Arceus_Dark].Type1 = PBEType.Dark;
                dict[PBESpecies.Arceus_Dragon].Type1 = PBEType.Dragon;
                dict[PBESpecies.Arceus_Electric].Type1 = PBEType.Electric;
                dict[PBESpecies.Arceus_Fighting].Type1 = PBEType.Fighting;
                dict[PBESpecies.Arceus_Fire].Type1 = PBEType.Fire;
                dict[PBESpecies.Arceus_Flying].Type1 = PBEType.Flying;
                dict[PBESpecies.Arceus_Ghost].Type1 = PBEType.Ghost;
                dict[PBESpecies.Arceus_Grass].Type1 = PBEType.Grass;
                dict[PBESpecies.Arceus_Ground].Type1 = PBEType.Ground;
                dict[PBESpecies.Arceus_Ice].Type1 = PBEType.Ice;
                dict[PBESpecies.Arceus_Poison].Type1 = PBEType.Poison;
                dict[PBESpecies.Arceus_Psychic].Type1 = PBEType.Psychic;
                dict[PBESpecies.Arceus_Rock].Type1 = PBEType.Rock;
                dict[PBESpecies.Arceus_Steel].Type1 = PBEType.Steel;
                dict[PBESpecies.Arceus_Water].Type1 = PBEType.Water;
                Merge(PBESpecies.Darmanitan, PBESpecies.Darmanitan_Zen);
                var allDeerling = new PBESpecies[] { PBESpecies.Deerling_Autumn, PBESpecies.Deerling_Spring, PBESpecies.Deerling_Summer, PBESpecies.Deerling_Winter };
                var allSawsbuck = new PBESpecies[] { PBESpecies.Sawsbuck_Autumn, PBESpecies.Sawsbuck_Spring, PBESpecies.Sawsbuck_Summer, PBESpecies.Sawsbuck_Winter };
                Merge(allDeerling);
                dict[PBESpecies.Deerling_Autumn].Evolutions.AddRange(new[] { PBESpecies.Sawsbuck_Spring, PBESpecies.Sawsbuck_Summer, PBESpecies.Sawsbuck_Winter });
                dict[PBESpecies.Deerling_Spring].Evolutions.AddRange(allSawsbuck);
                dict[PBESpecies.Deerling_Summer].Evolutions.AddRange(allSawsbuck);
                dict[PBESpecies.Deerling_Winter].Evolutions.AddRange(allSawsbuck);
                Merge(allSawsbuck);
                dict[PBESpecies.Sawsbuck_Autumn].PreEvolutions.AddRange(new[] { PBESpecies.Deerling_Spring, PBESpecies.Deerling_Summer, PBESpecies.Deerling_Winter });
                dict[PBESpecies.Sawsbuck_Spring].PreEvolutions.AddRange(allDeerling);
                dict[PBESpecies.Sawsbuck_Summer].PreEvolutions.AddRange(allDeerling);
                dict[PBESpecies.Sawsbuck_Winter].PreEvolutions.AddRange(allDeerling);
                Merge(PBESpecies.Tornadus, PBESpecies.Tornadus_Therian);
                Merge(PBESpecies.Thundurus, PBESpecies.Thundurus_Therian);
                Merge(PBESpecies.Landorus, PBESpecies.Landorus_Therian);
                Merge(PBESpecies.Kyurem, PBESpecies.Kyurem_Black, PBESpecies.Kyurem_White);
                Merge(PBESpecies.Keldeo, PBESpecies.Keldeo_Resolute);
                Merge(PBESpecies.Meloetta, PBESpecies.Meloetta_Pirouette);
                Merge(PBESpecies.Genesect, PBESpecies.Genesect_Burn, PBESpecies.Genesect_Chill, PBESpecies.Genesect_Douse, PBESpecies.Genesect_Shock);

                #endregion

                #region Write to Database

                cmd.CommandText = "DROP TABLE IF EXISTS PokemonData";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE PokemonData(Id INTEGER PRIMARY KEY, Json TEXT)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO PokemonData VALUES(@0, @1)";
                foreach (KeyValuePair<PBESpecies, Pokemon> pkmn in dict)
                {
                    var sw = new StringWriter();
                    using (var writer = new JsonTextWriter(sw))
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName(nameof(PBEPokemonData.BaseStats));
                        writer.WriteStartArray();
                        writer.WriteValue(pkmn.Value.HP);
                        writer.WriteValue(pkmn.Value.Attack);
                        writer.WriteValue(pkmn.Value.Defense);
                        writer.WriteValue(pkmn.Value.SpAttack);
                        writer.WriteValue(pkmn.Value.SpDefense);
                        writer.WriteValue(pkmn.Value.Speed);
                        writer.WriteEndArray();
                        writer.WritePropertyName(nameof(PBEPokemonData.Type1));
                        writer.WriteValue(pkmn.Value.Type1);
                        writer.WritePropertyName(nameof(PBEPokemonData.Type2));
                        writer.WriteValue(pkmn.Value.Type2);
                        writer.WritePropertyName(nameof(PBEPokemonData.GenderRatio));
                        writer.WriteValue(pkmn.Value.GenderRatio);
                        writer.WritePropertyName(nameof(PBEPokemonData.Weight));
                        writer.WriteValue(pkmn.Value.Weight);
                        writer.WritePropertyName(nameof(PBEPokemonData.PreEvolutions));
                        writer.WriteStartArray();
                        foreach (PBESpecies sp in pkmn.Value.PreEvolutions)
                        {
                            writer.WriteValue(sp);
                        }
                        writer.WriteEndArray();
                        writer.WritePropertyName(nameof(PBEPokemonData.Evolutions));
                        writer.WriteStartArray();
                        foreach (PBESpecies sp in pkmn.Value.Evolutions)
                        {
                            writer.WriteValue(sp);
                        }
                        writer.WriteEndArray();
                        writer.WritePropertyName(nameof(PBEPokemonData.Abilities));
                        writer.WriteStartArray();
                        foreach (PBEAbility a in pkmn.Value.Abilities)
                        {
                            writer.WriteValue(a);
                        }
                        writer.WriteEndArray();
                        writer.WritePropertyName(nameof(PBEPokemonData.LevelUpMoves));
                        writer.WriteStartArray();
                        foreach (KeyValuePair<(PBEMove Move, byte Level), PBEMoveObtainMethod> levelUpMove in pkmn.Value.LevelUpMoves)
                        {
                            if (Enum.IsDefined(typeof(PBEMove), levelUpMove.Key.Move))
                            {
                                writer.WriteStartArray();
                                writer.WriteValue(levelUpMove.Key.Move);
                                writer.WriteValue(levelUpMove.Key.Level);
                                writer.WriteValue(levelUpMove.Value);
                                writer.WriteEndArray();
                            }
                        }
                        writer.WriteEndArray();
                        writer.WritePropertyName(nameof(PBEPokemonData.OtherMoves));
                        writer.WriteStartArray();
                        foreach (KeyValuePair<PBEMove, PBEMoveObtainMethod> otherMove in pkmn.Value.OtherMoves)
                        {
                            if (Enum.IsDefined(typeof(PBEMove), otherMove.Key))
                            {
                                writer.WriteStartArray();
                                writer.WriteValue(otherMove.Key);
                                writer.WriteValue(otherMove.Value);
                                writer.WriteEndArray();
                            }
                        }
                        writer.WriteEndArray();
                        writer.WriteEndObject();

                        cmd.Parameters.AddWithValue("@0", (uint)pkmn.Key);
                        cmd.Parameters.AddWithValue("@1", sw.ToString());
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                }

                #endregion

                transaction.Commit();
            }
#pragma warning restore CS8321 // Local function is declared but never used
        }
    }
}
