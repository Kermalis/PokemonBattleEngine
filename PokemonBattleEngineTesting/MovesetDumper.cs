using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngineTesting
{
    class MovesetDumper
    {
        static readonly Dictionary<int, PBESpecies> gen3SpeciesIndexToPBESpecies = new Dictionary<int, PBESpecies>
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
            { 16, (PBESpecies)16 },
            { 17, (PBESpecies)17 },
            { 18, (PBESpecies)18 },
            { 19, (PBESpecies)19 },
            { 20, (PBESpecies)20 },
            { 21, (PBESpecies)21 },
            { 22, (PBESpecies)22 },
            { 23, (PBESpecies)23 },
            { 24, (PBESpecies)24 },
            { 25, PBESpecies.Pikachu },
            { 26, PBESpecies.Raichu },
            { 27, (PBESpecies)27 },
            { 28, (PBESpecies)28 },
            { 29, (PBESpecies)29 },
            { 30, (PBESpecies)30 },
            { 31, (PBESpecies)31 },
            { 32, (PBESpecies)32 },
            { 33, (PBESpecies)33 },
            { 34, (PBESpecies)34 },
            { 35, (PBESpecies)35 },
            { 36, (PBESpecies)36 },
            { 37, PBESpecies.Vulpix },
            { 38, PBESpecies.Ninetales },
            { 39, (PBESpecies)39 },
            { 40, (PBESpecies)40 },
            { 41, PBESpecies.Zubat },
            { 42, PBESpecies.Golbat },
            { 43, (PBESpecies)43 },
            { 44, (PBESpecies)44 },
            { 45, (PBESpecies)45 },
            { 46, (PBESpecies)46 },
            { 47, (PBESpecies)47 },
            { 48, (PBESpecies)48 },
            { 49, (PBESpecies)49 },
            { 50, (PBESpecies)50 },
            { 51, (PBESpecies)51 },
            { 52, (PBESpecies)52 },
            { 53, (PBESpecies)53 },
            { 54, PBESpecies.Psyduck },
            { 55, PBESpecies.Golduck },
            { 56, (PBESpecies)56 },
            { 57, (PBESpecies)57 },
            { 58, (PBESpecies)58 },
            { 59, (PBESpecies)59 },
            { 60, PBESpecies.Poliwag },
            { 61, PBESpecies.Poliwhirl },
            { 62, PBESpecies.Poliwrath },
            { 63, (PBESpecies)63 },
            { 64, (PBESpecies)64 },
            { 65, (PBESpecies)65 },
            { 66, (PBESpecies)66 },
            { 67, (PBESpecies)67 },
            { 68, (PBESpecies)68 },
            { 69, (PBESpecies)69 },
            { 70, (PBESpecies)70 },
            { 71, (PBESpecies)71 },
            { 72, (PBESpecies)72 },
            { 73, (PBESpecies)73 },
            { 74, (PBESpecies)74 },
            { 75, (PBESpecies)75 },
            { 76, (PBESpecies)76 },
            { 77, (PBESpecies)77 },
            { 78, (PBESpecies)78 },
            { 79, (PBESpecies)79 },
            { 80, (PBESpecies)80 },
            { 81, (PBESpecies)81 },
            { 82, (PBESpecies)82 },
            { 83, PBESpecies.Farfetchd },
            { 84, (PBESpecies)84 },
            { 85, (PBESpecies)85 },
            { 86, (PBESpecies)86 },
            { 87, (PBESpecies)87 },
            { 88, (PBESpecies)88 },
            { 89, (PBESpecies)89 },
            { 90, (PBESpecies)90 },
            { 91, (PBESpecies)91 },
            { 92, (PBESpecies)92 },
            { 93, (PBESpecies)93 },
            { 94, (PBESpecies)94 },
            { 95, (PBESpecies)95 },
            { 96, (PBESpecies)96 },
            { 97, (PBESpecies)97 },
            { 98, (PBESpecies)98 },
            { 99, (PBESpecies)99 },
            { 100, (PBESpecies)100 },
            { 101, (PBESpecies)101 },
            { 102, (PBESpecies)102 },
            { 103, (PBESpecies)103 },
            { 104, PBESpecies.Cubone },
            { 105, PBESpecies.Marowak },
            { 106, (PBESpecies)106 },
            { 107, (PBESpecies)107 },
            { 108, (PBESpecies)108 },
            { 109, (PBESpecies)109 },
            { 110, (PBESpecies)110 },
            { 111, (PBESpecies)111 },
            { 112, (PBESpecies)112 },
            { 113, PBESpecies.Chansey },
            { 114, (PBESpecies)114 },
            { 115, (PBESpecies)115 },
            { 116, (PBESpecies)116 },
            { 117, (PBESpecies)117 },
            { 118, (PBESpecies)118 },
            { 119, (PBESpecies)119 },
            { 120, (PBESpecies)120 },
            { 121, (PBESpecies)121 },
            { 122, (PBESpecies)122 },
            { 123, (PBESpecies)123 },
            { 124, (PBESpecies)124 },
            { 125, (PBESpecies)125 },
            { 126, (PBESpecies)126 },
            { 127, (PBESpecies)127 },
            { 128, (PBESpecies)128 },
            { 129, PBESpecies.Magikarp },
            { 130, PBESpecies.Gyarados },
            { 131, (PBESpecies)131 },
            { 132, PBESpecies.Ditto },
            { 133, PBESpecies.Eevee },
            { 134, PBESpecies.Vaporeon },
            { 135, PBESpecies.Jolteon },
            { 136, PBESpecies.Flareon },
            { 137, (PBESpecies)137 },
            { 138, (PBESpecies)138 },
            { 139, (PBESpecies)139 },
            { 140, (PBESpecies)140 },
            { 141, (PBESpecies)141 },
            { 142, (PBESpecies)142 },
            { 143, (PBESpecies)143 },
            { 144, (PBESpecies)144 },
            { 145, (PBESpecies)145 },
            { 146, (PBESpecies)146 },
            { 147, (PBESpecies)147 },
            { 148, (PBESpecies)148 },
            { 149, (PBESpecies)149 },
            { 150, (PBESpecies)150 },
            { 151, (PBESpecies)151 },
            { 152, (PBESpecies)152 },
            { 153, (PBESpecies)153 },
            { 154, (PBESpecies)154 },
            { 155, (PBESpecies)155 },
            { 156, (PBESpecies)156 },
            { 157, (PBESpecies)157 },
            { 158, (PBESpecies)158 },
            { 159, (PBESpecies)159 },
            { 160, (PBESpecies)160 },
            { 161, (PBESpecies)161 },
            { 162, (PBESpecies)162 },
            { 163, (PBESpecies)163 },
            { 164, (PBESpecies)164 },
            { 165, (PBESpecies)165 },
            { 166, (PBESpecies)166 },
            { 167, (PBESpecies)167 },
            { 168, (PBESpecies)168 },
            { 169, PBESpecies.Crobat },
            { 170, (PBESpecies)170 },
            { 171, (PBESpecies)171 },
            { 172, PBESpecies.Pichu },
            { 173, (PBESpecies)173 },
            { 174, (PBESpecies)174 },
            { 175, (PBESpecies)175 },
            { 176, (PBESpecies)176 },
            { 177, (PBESpecies)177 },
            { 178, (PBESpecies)178 },
            { 179, (PBESpecies)179 },
            { 180, (PBESpecies)180 },
            { 181, (PBESpecies)181 },
            { 182, (PBESpecies)182 },
            { 183, PBESpecies.Marill },
            { 184, PBESpecies.Azumarill },
            { 185, (PBESpecies)185 },
            { 186, PBESpecies.Politoed },
            { 187, (PBESpecies)187 },
            { 188, (PBESpecies)188 },
            { 189, (PBESpecies)189 },
            { 190, (PBESpecies)190 },
            { 191, (PBESpecies)191 },
            { 192, (PBESpecies)192 },
            { 193, (PBESpecies)193 },
            { 194, (PBESpecies)194 },
            { 195, (PBESpecies)195 },
            { 196, PBESpecies.Espeon },
            { 197, PBESpecies.Umbreon },
            { 198, (PBESpecies)198 },
            { 199, (PBESpecies)199 },
            { 200, PBESpecies.Misdreavus },
            { 201, PBESpecies.Unown_A },
            { 202, (PBESpecies)202 },
            { 203, (PBESpecies)203 },
            { 204, (PBESpecies)204 },
            { 205, (PBESpecies)205 },
            { 206, (PBESpecies)206 },
            { 207, (PBESpecies)207 },
            { 208, (PBESpecies)208 },
            { 209, (PBESpecies)209 },
            { 210, (PBESpecies)210 },
            { 211, (PBESpecies)211 },
            { 212, (PBESpecies)212 },
            { 213, (PBESpecies)213 },
            { 214, (PBESpecies)214 },
            { 215, (PBESpecies)215 },
            { 216, (PBESpecies)216 },
            { 217, (PBESpecies)217 },
            { 218, (PBESpecies)218 },
            { 219, (PBESpecies)219 },
            { 220, (PBESpecies)220 },
            { 221, (PBESpecies)221 },
            { 222, (PBESpecies)222 },
            { 223, (PBESpecies)223 },
            { 224, (PBESpecies)224 },
            { 225, (PBESpecies)225 },
            { 226, (PBESpecies)226 },
            { 227, (PBESpecies)227 },
            { 228, (PBESpecies)228 },
            { 229, (PBESpecies)229 },
            { 230, (PBESpecies)230 },
            { 231, (PBESpecies)231 },
            { 232, (PBESpecies)232 },
            { 233, (PBESpecies)233 },
            { 234, (PBESpecies)234 },
            { 235, PBESpecies.Smeargle },
            { 236, (PBESpecies)236 },
            { 237, (PBESpecies)237 },
            { 238, (PBESpecies)238 },
            { 239, (PBESpecies)239 },
            { 240, (PBESpecies)240 },
            { 241, (PBESpecies)241 },
            { 242, PBESpecies.Blissey },
            { 243, (PBESpecies)243 },
            { 244, (PBESpecies)244 },
            { 245, (PBESpecies)245 },
            { 246, (PBESpecies)246 },
            { 247, (PBESpecies)247 },
            { 248, (PBESpecies)248 },
            { 249, (PBESpecies)249 },
            { 250, (PBESpecies)250 },
            { 251, (PBESpecies)251 },
            { 277, (PBESpecies)252 },
            { 278, (PBESpecies)253 },
            { 279, (PBESpecies)254 },
            { 280, PBESpecies.Torchic },
            { 281, PBESpecies.Combusken },
            { 282, PBESpecies.Blaziken },
            { 283, (PBESpecies)258 },
            { 284, (PBESpecies)259 },
            { 285, (PBESpecies)260 },
            { 286, (PBESpecies)261 },
            { 287, (PBESpecies)262 },
            { 288, (PBESpecies)263 },
            { 289, (PBESpecies)264 },
            { 290, (PBESpecies)265 },
            { 291, (PBESpecies)266 },
            { 292, (PBESpecies)267 },
            { 293, (PBESpecies)268 },
            { 294, (PBESpecies)269 },
            { 295, (PBESpecies)270 },
            { 296, (PBESpecies)271 },
            { 297, (PBESpecies)272 },
            { 298, (PBESpecies)273 },
            { 299, (PBESpecies)274 },
            { 300, (PBESpecies)275 },
            { 301, PBESpecies.Nincada },
            { 302, PBESpecies.Ninjask },
            { 303, PBESpecies.Shedinja },
            { 304, (PBESpecies)276 },
            { 305, (PBESpecies)277 },
            { 306, (PBESpecies)285 },
            { 307, (PBESpecies)286 },
            { 308, (PBESpecies)327 },
            { 309, (PBESpecies)278 },
            { 310, (PBESpecies)279 },
            { 311, (PBESpecies)283 },
            { 312, (PBESpecies)284 },
            { 313, (PBESpecies)320 },
            { 314, (PBESpecies)321 },
            { 315, PBESpecies.Skitty },
            { 316, PBESpecies.Delcatty },
            { 317, (PBESpecies)352 },
            { 318, (PBESpecies)343 },
            { 319, (PBESpecies)344 },
            { 320, (PBESpecies)299 },
            { 321, (PBESpecies)324 },
            { 322, (PBESpecies)302 },
            { 323, (PBESpecies)339 },
            { 324, (PBESpecies)340 },
            { 325, (PBESpecies)370 },
            { 326, (PBESpecies)341 },
            { 327, (PBESpecies)342 },
            { 328, (PBESpecies)349 },
            { 329, (PBESpecies)350 },
            { 330, (PBESpecies)318 },
            { 331, (PBESpecies)319 },
            { 332, (PBESpecies)328 },
            { 333, (PBESpecies)329 },
            { 334, (PBESpecies)330 },
            { 335, (PBESpecies)296 },
            { 336, (PBESpecies)297 },
            { 337, (PBESpecies)309 },
            { 338, (PBESpecies)310 },
            { 339, (PBESpecies)322 },
            { 340, (PBESpecies)323 },
            { 341, (PBESpecies)363 },
            { 342, (PBESpecies)364 },
            { 343, (PBESpecies)365 },
            { 344, (PBESpecies)331 },
            { 345, (PBESpecies)332 },
            { 346, (PBESpecies)361 },
            { 347, (PBESpecies)362 },
            { 348, (PBESpecies)337 },
            { 349, (PBESpecies)338 },
            { 350, PBESpecies.Azurill },
            { 351, (PBESpecies)325 },
            { 352, (PBESpecies)326 },
            { 353, PBESpecies.Plusle },
            { 354, PBESpecies.Minun },
            { 355, (PBESpecies)303 },
            { 356, (PBESpecies)307 },
            { 357, (PBESpecies)308 },
            { 358, (PBESpecies)333 },
            { 359, (PBESpecies)334 },
            { 360, (PBESpecies)360 },
            { 361, (PBESpecies)355 },
            { 362, (PBESpecies)356 },
            { 363, (PBESpecies)315 },
            { 364, (PBESpecies)287 },
            { 365, (PBESpecies)288 },
            { 366, (PBESpecies)289 },
            { 367, (PBESpecies)316 },
            { 368, (PBESpecies)317 },
            { 369, PBESpecies.Tropius },
            { 370, (PBESpecies)293 },
            { 371, (PBESpecies)294 },
            { 372, (PBESpecies)295 },
            { 373, PBESpecies.Clamperl },
            { 374, PBESpecies.Huntail },
            { 375, PBESpecies.Gorebyss },
            { 376, PBESpecies.Absol },
            { 377, (PBESpecies)353 },
            { 378, (PBESpecies)354 },
            { 379, (PBESpecies)336 },
            { 380, (PBESpecies)335 },
            { 381, (PBESpecies)369 },
            { 382, (PBESpecies)304 },
            { 383, (PBESpecies)305 },
            { 384, (PBESpecies)306 },
            { 385, (PBESpecies)351 },
            { 386, (PBESpecies)313 },
            { 387, (PBESpecies)314 },
            { 388, PBESpecies.Lileep },
            { 389, PBESpecies.Cradily },
            { 390, (PBESpecies)347 },
            { 391, (PBESpecies)348 },
            { 392, (PBESpecies)280 },
            { 393, (PBESpecies)281 },
            { 394, (PBESpecies)282 },
            { 395, (PBESpecies)371 },
            { 396, (PBESpecies)372 },
            { 397, (PBESpecies)373 },
            { 398, PBESpecies.Beldum },
            { 399, PBESpecies.Metang },
            { 400, PBESpecies.Metagross },
            { 401, PBESpecies.Regirock },
            { 402, (PBESpecies)378 },
            { 403, (PBESpecies)379 },
            { 404, (PBESpecies)382 },
            { 405, (PBESpecies)383 },
            { 406, (PBESpecies)384 },
            { 407, PBESpecies.Latias },
            { 408, PBESpecies.Latios },
            { 409, (PBESpecies)385 },
            { 410, (PBESpecies)386 },
            { 411, (PBESpecies)358 },
            { 413, PBESpecies.Unown_B },
            { 414, PBESpecies.Unown_C },
            { 415, PBESpecies.Unown_D },
            { 416, PBESpecies.Unown_E },
            { 417, PBESpecies.Unown_F },
            { 418, PBESpecies.Unown_G },
            { 419, PBESpecies.Unown_H },
            { 420, PBESpecies.Unown_I },
            { 421, PBESpecies.Unown_J },
            { 422, PBESpecies.Unown_K },
            { 423, PBESpecies.Unown_L },
            { 424, PBESpecies.Unown_M },
            { 425, PBESpecies.Unown_N },
            { 426, PBESpecies.Unown_O },
            { 427, PBESpecies.Unown_P },
            { 428, PBESpecies.Unown_Q },
            { 429, PBESpecies.Unown_R },
            { 430, PBESpecies.Unown_S },
            { 431, PBESpecies.Unown_T },
            { 432, PBESpecies.Unown_U },
            { 433, PBESpecies.Unown_V },
            { 434, PBESpecies.Unown_W },
            { 435, PBESpecies.Unown_X },
            { 436, PBESpecies.Unown_Y },
            { 437, PBESpecies.Unown_Z },
            { 438, PBESpecies.Unown_Exclamation },
            { 439, PBESpecies.Unown_Question }
        };
        static readonly PBEMove[] gen3TMHMIndexToPBEMove = new PBEMove[58]
        {
            (PBEMove)264, // FocusPunch
            PBEMove.DragonClaw,
            PBEMove.WaterPulse,
            PBEMove.CalmMind,
            PBEMove.Roar,
            PBEMove.Toxic,
            PBEMove.Hail,
            PBEMove.BulkUp,
            (PBEMove)331, // BulletSeed
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
            (PBEMove)219, // Safeguard
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
        static readonly PBEMove[] emeraldTutorMoves = new PBEMove[30]
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
        static readonly Dictionary<int, PBESpecies> gen4SpeciesIndexToPBESpecies = new Dictionary<int, PBESpecies>
        {
            { 496, (PBESpecies)(386 | (1 << 0x10)) }, // Deoxys_Attack
            { 497, (PBESpecies)(386 | (2 << 0x10)) }, // Deoxys_Defense
            { 498, (PBESpecies)(386 | (3 << 0x10)) }, // Deoxys_Speed
            { 499, (PBESpecies)(413 | (1 << 0x10)) }, // Wormadam_Sandy
            { 500, (PBESpecies)(413 | (2 << 0x10)) }, // Wormadam_Trash
            { 501, PBESpecies.Giratina_Origin },
            { 502, (PBESpecies)(492 | (1 << 0x10)) }, // Shaymin_Sky
            // Not sure on the order of the Rotoms, but they all have the same level up moves & tm moves
            { 503, PBESpecies.Rotom_Fan },
            { 504, PBESpecies.Rotom_Frost },
            { 505, PBESpecies.Rotom_Heat },
            { 506, PBESpecies.Rotom_Mow },
            { 507, PBESpecies.Rotom_Wash }
        };
        static readonly PBEMove[] gen4TMHMIndexToPBEMove = new PBEMove[100]
        {
            (PBEMove)264, // FocusPunch
            PBEMove.DragonClaw,
            PBEMove.WaterPulse,
            PBEMove.CalmMind,
            PBEMove.Roar,
            PBEMove.Toxic,
            PBEMove.Hail,
            PBEMove.BulkUp,
            (PBEMove)331, // BulletSeed
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
            (PBEMove)219, // Safeguard
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
            (PBEMove)445, // Captivate
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

        // You must dump everything yourself
        // The GBA ROMs must all be v1.0
        // D, P, and Pt level-up move NARC is /poketool/personal/wotbl.narc (D and P have identical level-up move NARCs)
        // Pt TMHM moves are in the Pokémon data NARC which is /poketool/personal/pl_personal.narc for Pt (Pt changed no TMHM compatibility from DP so I use it alone)
        // HG and SS level-up move NARC is /a/0/3/3 (HG and SS have identical level-up move NARCs)
        // HG and SS TMHM moves are in the Pokémon data NARC which is /a/0/0/2 (HG and SS have identical Pokémon data NARCs)
        // B, W, B2, and W2 level-up move NARC is /a/0/1/8
        // TODO: Colo, XD, B, W, B2, W2 - levelup & tm
        // TODO: Colo, XD, D, P, Pt, HG, SS, B, W, B2, W2 - egg & tutor
        // TODO: Pichu & Volt Tackle (and check for other egg move special cases)
        // TODO: Share moves across formes
        public static void Dump()
        {
            using (var rStream = new FileStream(@"../../../\DumpedData\R.gba", FileMode.Open))
            using (var sStream = new FileStream(@"../../../\DumpedData\S.gba", FileMode.Open))
            using (var frStream = new FileStream(@"../../../\DumpedData\FR.gba", FileMode.Open))
            using (var lgStream = new FileStream(@"../../../\DumpedData\LG.gba", FileMode.Open))
            using (var eStream = new FileStream(@"../../../\DumpedData\E.gba", FileMode.Open))
            using (var r = new BinaryReader(rStream))
            using (var s = new BinaryReader(sStream))
            using (var fr = new BinaryReader(frStream))
            using (var lg = new BinaryReader(lgStream))
            using (var e = new BinaryReader(eStream))
            {
                var sb = new StringBuilder();
                var levelup = new Dictionary<PBESpecies, Dictionary<Tuple<int, PBEMove>, string>>();
                var tmhm = new Dictionary<PBESpecies, Dictionary<PBEMove, string>>();
                var tutor = new Dictionary<PBESpecies, Dictionary<PBEMove, string>>();
                var egg = new Dictionary<PBESpecies, Dictionary<PBEMove, string>>();

                sb.AppendLine("LEVELUP");

                #region Level Up Moves

                // Gen 3
                for (int sp = 1; sp <= 411; sp++)
                {
                    // Gen 2 Unown slots are ignored in gen 3
                    if (sp > 251 && sp < 277)
                    {
                        continue;
                    }
                    // It is the same in Ruby and Sapphire, but the rest have some differences
                    r.BaseStream.Position = 0x207BC8 + (sizeof(uint) * sp);
                    s.BaseStream.Position = 0x207B58 + (sizeof(uint) * sp);
                    fr.BaseStream.Position = 0x25D7B4 + (sizeof(uint) * sp);
                    lg.BaseStream.Position = 0x25D794 + (sizeof(uint) * sp);
                    e.BaseStream.Position = 0x32937C + (sizeof(uint) * sp);
                    void ReadLevelUpMoves(BinaryReader reader, string flag)
                    {
                        PBESpecies species = gen3SpeciesIndexToPBESpecies[sp];
                        if (species == (PBESpecies)386)
                        {
                            if (reader == e)
                            {
                                species = (PBESpecies)(386 | (3 << 0x10)); // Deoxys_Speed
                            }
                            if (reader == lg)
                            {
                                species = (PBESpecies)(386 | (2 << 0x10)); // Deoxys_Defense
                            }
                            if (reader == fr)
                            {
                                species = (PBESpecies)(386 | (1 << 0x10)); // Deoxys_Attack
                            }
                        }
                        if (!levelup.ContainsKey(species))
                        {
                            levelup.Add(species, new Dictionary<Tuple<int, PBEMove>, string>());
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
                                int level = val >> 9;
                                var move = (PBEMove)(val & 0x1FF);
                                Tuple<int, PBEMove> tupleThatExists = levelup[species].Keys.SingleOrDefault(k => k.Item1 == level && k.Item2 == move);
                                if (tupleThatExists != null)
                                {
                                    levelup[species][tupleThatExists] += $" | {flag}";
                                }
                                else
                                {
                                    levelup[species].Add(Tuple.Create(level, move), flag);
                                }
                            }
                        }
                    }
                    ReadLevelUpMoves(r, "PBEMoveObtainMethod.LevelUp_RS");
                    ReadLevelUpMoves(fr, "PBEMoveObtainMethod.LevelUp_FR");
                    ReadLevelUpMoves(lg, "PBEMoveObtainMethod.LevelUp_LG");
                    ReadLevelUpMoves(e, "PBEMoveObtainMethod.LevelUp_E");
                }
                // Gen 4
                using (var dp = new NARC(@"../../../\DumpedData\DPLevelUp.narc"))
                using (var pt = new NARC(@"../../../\DumpedData\PtLevelUp.narc"))
                using (var hgss = new NARC(@"../../../\DumpedData\HGSSLevelUp.narc"))
                {
                    for (int sp = 1; sp <= 507; sp++)
                    {
                        // 494 is Egg, 495 is Bad Egg
                        if (sp == 494 || sp == 495)
                        {
                            continue;
                        }
                        PBESpecies species = gen4SpeciesIndexToPBESpecies.ContainsKey(sp) ? gen4SpeciesIndexToPBESpecies[sp] : (PBESpecies)sp;
                        if (!levelup.ContainsKey(species))
                        {
                            levelup.Add(species, new Dictionary<Tuple<int, PBEMove>, string>());
                        }
                        void ReadLevelUpMoves(MemoryStream file, string flag)
                        {
                            using (var reader = new BinaryReader(file))
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
                                        int level = val >> 9;
                                        var move = (PBEMove)(val & 0x1FF);
                                        Tuple<int, PBEMove> tupleThatExists = levelup[species].Keys.SingleOrDefault(k => k.Item1 == level && k.Item2 == move);
                                        if (tupleThatExists != null)
                                        {
                                            levelup[species][tupleThatExists] += $" | {flag}";
                                        }
                                        else
                                        {
                                            levelup[species].Add(Tuple.Create(level, move), flag);
                                        }
                                    }
                                }
                            }
                        }
                        // DP only has 0-500
                        if (sp <= 500)
                        {
                            ReadLevelUpMoves(dp.Files[sp], "PBEMoveObtainMethod.LevelUp_DP");
                        }
                        ReadLevelUpMoves(pt.Files[sp], "PBEMoveObtainMethod.LevelUp_Pt");
                        ReadLevelUpMoves(hgss.Files[sp], "PBEMoveObtainMethod.LevelUp_HGSS");
                    }
                }

                #endregion

                foreach (KeyValuePair<PBESpecies, Dictionary<Tuple<int, PBEMove>, string>> speciesPair in levelup)
                {
                    sb.AppendLine($"// PBESpecies.{speciesPair.Key}:");
                    foreach (KeyValuePair<Tuple<int, PBEMove>, string> movePair in speciesPair.Value)
                    {
                        sb.AppendLine($"{(Enum.IsDefined(typeof(PBEMove), movePair.Key.Item2) ? string.Empty : "// ")}Tuple.Create(PBEMove.{movePair.Key.Item2}, {movePair.Key.Item1}, {movePair.Value}),");
                    }
                }
                sb.AppendLine();
                sb.AppendLine("TMHM");

                #region TMHM Compatibility

                // Gen 3
                for (int sp = 1; sp <= 411; sp++)
                {
                    // Gen 2 Unown slots are ignored in gen 3
                    if (sp > 251 && sp < 277)
                    {
                        continue;
                    }
                    // It is the same in all 5 GBA games, so I will only read one
                    r.BaseStream.Position = 0x1FD0F0 + (8 * sp);
                    s.BaseStream.Position = 0x1FD080 + (8 * sp);
                    fr.BaseStream.Position = 0x252BC8 + (8 * sp);
                    lg.BaseStream.Position = 0x252BA4 + (8 * sp);
                    e.BaseStream.Position = 0x31E898 + (8 * sp);
                    byte[] bytes = r.ReadBytes(8);
                    PBESpecies species = gen3SpeciesIndexToPBESpecies[sp];
                    tmhm.Add(species, new Dictionary<PBEMove, string>());
                    for (int i = 0; i < gen3TMHMIndexToPBEMove.Length; i++)
                    {
                        if ((bytes[i / 8] & (1 << (i % 8))) != 0)
                        {
                            tmhm[species].Add(gen3TMHMIndexToPBEMove[i], $"PBEMoveObtainMethod.{(i < 50 ? "TM" : "HM")}_RSFRLGE");
                        }
                    }
                }
                // Gen 4
                using (var dppt = new NARC(@"../../../\DumpedData\PtPokedata.narc"))
                using (var hgss = new NARC(@"../../../\DumpedData\HGSSPokedata.narc"))
                {
                    for (int sp = 1; sp <= 507; sp++)
                    {
                        // 494 is Egg, 495 is Bad Egg
                        if (sp == 494 || sp == 495)
                        {
                            continue;
                        }
                        PBESpecies species = gen4SpeciesIndexToPBESpecies.ContainsKey(sp) ? gen4SpeciesIndexToPBESpecies[sp] : (PBESpecies)sp;
                        if (!tmhm.ContainsKey(species))
                        {
                            tmhm.Add(species, new Dictionary<PBEMove, string>());
                        }
                        void ReadTMHM(MemoryStream file, bool isDPPt)
                        {
                            using (var reader = new BinaryReader(file))
                            {
                                reader.BaseStream.Position = 0x1C;
                                byte[] bytes = reader.ReadBytes(13);
                                for (int i = 0; i < gen4TMHMIndexToPBEMove.Length; i++)
                                {
                                    if ((bytes[i / 8] & (1 << (i % 8))) != 0)
                                    {
                                        PBEMove move = gen4TMHMIndexToPBEMove[i];
                                        if (move == PBEMove.None)
                                        {
                                            move = isDPPt ? (PBEMove)432 : (PBEMove)250;
                                        }
                                        string flag = $"PBEMoveObtainMethod.{(i < 92 ? "TM" : "HM")}_{(isDPPt ? "DPPt" : "HGSS")}";
                                        if (tmhm[species].ContainsKey(move))
                                        {
                                            tmhm[species][move] += $" | {flag}";
                                        }
                                        else
                                        {
                                            tmhm[species].Add(move, flag);
                                        }
                                    }
                                }
                            }
                        }
                        ReadTMHM(dppt.Files[sp], true);
                        ReadTMHM(hgss.Files[sp], false);
                    }
                }

                #endregion

                foreach (KeyValuePair<PBESpecies, Dictionary<PBEMove, string>> speciesPair in tmhm)
                {
                    sb.AppendLine($"// PBESpecies.{speciesPair.Key}:");
                    foreach (KeyValuePair<PBEMove, string> movePair in speciesPair.Value)
                    {
                        sb.AppendLine($"{(Enum.IsDefined(typeof(PBEMove), movePair.Key) ? string.Empty : "// ")}Tuple.Create(PBEMove.{movePair.Key}, {movePair.Value}),");
                    }
                }
                sb.AppendLine();
                sb.AppendLine("TUTOR");

                #region Move Tutor

                // Gen 3
                // Emerald
                for (int sp = 1; sp <= 411; sp++)
                {
                    // Gen 2 Unown slots are ignored in gen 3
                    if (sp > 251 && sp < 277)
                    {
                        continue;
                    }
                    e.BaseStream.Position = 0x615048 + (sizeof(uint) * sp);
                    uint val = e.ReadUInt32();
                    PBESpecies species = gen3SpeciesIndexToPBESpecies[sp];
                    tutor.Add(species, new Dictionary<PBEMove, string>());
                    for (int i = 0; i < emeraldTutorMoves.Length; i++)
                    {
                        if ((val & (1u << i)) != 0)
                        {
                            tutor[species].Add(emeraldTutorMoves[i], "PBEMoveObtainMethod.MoveTutor_E");
                        }
                    }
                }

                #endregion
                foreach (KeyValuePair<PBESpecies, Dictionary<PBEMove, string>> speciesPair in tutor)
                {
                    sb.AppendLine($"// PBESpecies.{speciesPair.Key}:");
                    foreach (KeyValuePair<PBEMove, string> movePair in speciesPair.Value)
                    {
                        sb.AppendLine($"{(Enum.IsDefined(typeof(PBEMove), movePair.Key) ? string.Empty : "// ")}Tuple.Create(PBEMove.{movePair.Key}, {movePair.Value}),");
                    }
                }
                sb.AppendLine();
                sb.AppendLine("EGG");

                #region Egg Moves

                // Gen 3
                {
                    // The table is the same in all 5 GBA games, so I will only read one
                    // Pichu learning Volt Tackle in E is the only exception and is not in the table; it is extra code in the daycare backend
                    r.BaseStream.Position = 0x2091DC;
                    s.BaseStream.Position = 0x20916C;
                    fr.BaseStream.Position = 0x25EF0C;
                    lg.BaseStream.Position = 0x25EEEC;
                    e.BaseStream.Position = 0x32ADD8;
                    PBESpecies species = 0;
                    while (true)
                    {
                        ushort val = r.ReadUInt16();
                        if (val == 0xFFFF)
                        {
                            break;
                        }
                        else if (val > 20000)
                        {
                            species = gen3SpeciesIndexToPBESpecies[val - 20000];
                            egg.Add(species, new Dictionary<PBEMove, string>());
                        }
                        else
                        {
                            egg[species].Add((PBEMove)val, "PBEMoveObtainMethod.EggMove_RSFRLGE");
                        }
                    }
                }

                #endregion

                foreach (KeyValuePair<PBESpecies, Dictionary<PBEMove, string>> speciesPair in egg)
                {
                    sb.AppendLine($"// PBESpecies.{speciesPair.Key}:");
                    foreach (KeyValuePair<PBEMove, string> movePair in speciesPair.Value)
                    {
                        sb.AppendLine($"{(Enum.IsDefined(typeof(PBEMove), movePair.Key) ? string.Empty : "// ")}Tuple.Create(PBEMove.{movePair.Key}, {movePair.Value}),");
                    }
                }

                File.WriteAllText(@"../../../\DumpedData\Dumped\Moves.txt", sb.ToString());
            }
        }
    }
}
