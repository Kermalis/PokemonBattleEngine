using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PPokemonData
    {
        public byte HP, Attack, Defense, SpAttack, SpDefense, Speed;
        public PGender GenderRatio;
        public PType Type1, Type2;
        public PAbility[] Abilities;
        public byte MinLevel;
        public Tuple<int, PMove>[] LevelUpMoves;
        public PMove[] OtherMoves;

        public bool HasAbility(PAbility ability) => Abilities.Contains(ability);
        public bool HasType(PType type) => Type1 == type || Type2 == type;

        // First is attacker, second is defender
        // Cast PType to an int for the indices
        // [0,1] = bug attacker, dark defender
        public static readonly double[,] TypeEffectiveness = new double[,]
        {
            // Defender
            //    Bug     Dark   Dragon Electric Fighting     Fire   Flying    Ghost    Grass   Ground      Ice   Normal   Poison  Psychic     Rock    Steel    Water
            {     1.0,     2.0,     1.0,     1.0,     0.5,     0.5,     0.5,     0.5,     2.0,     1.0,     1.0,     1.0,     0.5,     2.0,     1.0,     0.5,     1.0}, // Bug
            {     1.0,     0.5,     1.0,     1.0,     0.5,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     0.5,     1.0}, // Dark
            {     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     1.0}, // Dragon
            {     1.0,     1.0,     0.5,     0.5,     1.0,     1.0,     2.0,     1.0,     0.5,     0.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0}, // Electric
            {     0.5,     2.0,     1.0,     1.0,     1.0,     1.0,     0.5,     0.0,     1.0,     1.0,     2.0,     2.0,     0.5,     0.5,     2.0,     2.0,     1.0}, // Fighting
            {     2.0,     1.0,     0.5,     1.0,     1.0,     0.5,     1.0,     1.0,     2.0,     1.0,     2.0,     1.0,     1.0,     1.0,     0.5,     2.0,     0.5}, // Fire
            {     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     0.5,     1.0}, // Flying
            {     1.0,     0.5,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     0.0,     1.0,     2.0,     1.0,     0.5,     1.0}, // Ghost
            {     0.5,     1.0,     0.5,     1.0,     1.0,     0.5,     0.5,     1.0,     1.0,     2.0,     1.0,     1.0,     0.5,     1.0,     2.0,     0.5,     2.0}, // Grass
            {     0.5,     1.0,     1.0,     2.0,     1.0,     2.0,     0.0,     1.0,     0.5,     1.0,     1.0,     1.0,     2.0,     1.0,     2.0,     2.0,     1.0}, // Ground
            {     1.0,     1.0,     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     2.0,     2.0,     0.5,     1.0,     1.0,     1.0,     1.0,     0.5,     0.5}, // Ice
            {     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     0.5,     1.0}, // Normal
            {     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     2.0,     0.5,     1.0,     1.0,     0.5,     1.0,     0.5,     0.0,     1.0}, // Poison
            {     1.0,     0.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0,     0.5,     1.0,     0.5,     1.0}, // Psychic
            {     2.0,     1.0,     1.0,     1.0,     0.5,     2.0,     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     1.0,     1.0,     1.0,     0.5,     1.0}, // Rock
            {     1.0,     1.0,     1.0,     0.5,     1.0,     0.5,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     2.0,     0.5,     0.5}, // Steel
            {     1.0,     1.0,     0.5,     1.0,     1.0,     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     0.5}, // Water
                                                                                                                                                                        // Attacker
        };
        public static readonly Dictionary<PNature, sbyte[]> NatureBoosts = new Dictionary<PNature, sbyte[]>
        {
            //                                Atk   Def SpAtk SpDef   Spd
            { PNature.Adamant, new sbyte[] {   +1,    0,   -1,    0,    0} },
            { PNature.Bashful, new sbyte[] {    0,    0,    0,    0,    0} },
            { PNature.Bold,    new sbyte[] {   -1,   +1,    0,    0,    0} },
            { PNature.Brave,   new sbyte[] {   +1,    0,    0,    0,   -1} },
            { PNature.Calm,    new sbyte[] {   -1,    0,    0,   +1,    0} },
            { PNature.Careful, new sbyte[] {    0,    0,   -1,   +1,    0} },
            { PNature.Docile,  new sbyte[] {    0,    0,    0,    0,    0} },
            { PNature.Gentle,  new sbyte[] {    0,   -1,    0,   +1,    0} },
            { PNature.Hardy,   new sbyte[] {    0,    0,    0,    0,    0} },
            { PNature.Hasty,   new sbyte[] {    0,   -1,    0,    0,   +1} },
            { PNature.Impish,  new sbyte[] {    0,   +1,   -1,    0,    0} },
            { PNature.Jolly,   new sbyte[] {    0,    0,   -1,    0,   +1} },
            { PNature.Lax,     new sbyte[] {    0,   +1,    0,   -1,    0} },
            { PNature.Loney,   new sbyte[] {   +1,   -1,    0,    0,    0} },
            { PNature.Mild,    new sbyte[] {    0,   -1,   +1,    0,    0} },
            { PNature.Modest,  new sbyte[] {   -1,    0,   +1,    0,    0} },
            { PNature.Naive,   new sbyte[] {    0,    0,    0,   -1,   +1} },
            { PNature.Naughty, new sbyte[] {   +1,    0,    0,   -1,    0} },
            { PNature.Quiet,   new sbyte[] {    0,    0,   +1,    0,   -1} },
            { PNature.Quirky,  new sbyte[] {    0,    0,    0,    0,    0} },
            { PNature.Rash,    new sbyte[] {    0,    0,   +1,   -1,    0} },
            { PNature.Relaxed, new sbyte[] {    0,   +1,    0,    0,   -1} },
            { PNature.Sassy,   new sbyte[] {    0,    0,    0,   +1,   -1} },
            { PNature.Serious, new sbyte[] {    0,    0,    0,    0,    0} },
            { PNature.Timid,   new sbyte[] {   -1,    0,    0,    0,   +1} },
        };
        public static readonly PType[] HiddenPowerTypes = new PType[]
        {
            PType.Fighting, // 7.8125 %
            PType.Flying,   // 6.2500 %
            PType.Poison,   // 6.2500 %
            PType.Ground,   // 6.2500 %
            PType.Rock,     // 6.2500 %
            PType.Bug,      // 7.8125 %
            PType.Ghost,    // 6.2500 %
            PType.Steel,    // 6.2500 %
            PType.Fire,     // 6.2500 %
            PType.Water,    // 6.2500 %
            PType.Grass,    // 7.8125 %
            PType.Electric, // 6.2500 %
            PType.Psychic,  // 6.2500 %
            PType.Ice,      // 6.2500 %
            PType.Dragon,   // 6.2500 %
            PType.Dark      // 1.5625 %
        };


        public static Dictionary<PSpecies, PPokemonData> Data = new Dictionary<PSpecies, PPokemonData>()
        {
            {
                PSpecies.Pikachu,
                new PPokemonData
                {
                    HP = 35, Attack = 55, Defense = 30, SpAttack = 50, SpDefense = 40, Speed = 90,
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Electric, Type2 = PType.Electric,
                    Abilities = new PAbility[] { PAbility.Static, PAbility.LightningRod },
                    MinLevel = 1, // Egg
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.Growl),
                        Tuple.Create(1, PMove.ThunderShock),
                        Tuple.Create(1, PMove.Charm), // As Pichu
                        Tuple.Create(5, PMove.TailWhip),
                        // follow me 5 // gen 3 pichu event
                        // teeter dance 5 // gen 3 pichu event
                        // thunder wave 8 // 8 As Pichu, 10
                        // sweet kiss 11 // As Pichu
                        Tuple.Create(13, PMove.QuickAttack),
                        // electro ball 18
                        Tuple.Create(18, PMove.NastyPlot), // As Pichu
                        Tuple.Create(21, PMove.DoubleTeam),
                        Tuple.Create(26, PMove.Slam),
                        Tuple.Create(29, PMove.Thunderbolt),
                        // sing 30 // gen 5 event (Singing Pikachu)
                        // last resort 30 // gen 4 event (Kyoto Cross Media Experience 2009 Pikachu)
                        // endeavor 30 // gen 4 pichu event
                        // feint 34
                        Tuple.Create(37, PMove.Agility),
                        // discharge 42
                        // light screen 45
                        Tuple.Create(50, PMove.ExtremeSpeed), // Gen 5 event (ExtremeSpeed Pikachu)
                        // yawn 50 // gen 4 event (Sleeping Pikachu)
                        // rest 50 // gen 4 event (Sleeping Pikachu)
                        Tuple.Create(50, PMove.Thunder),
                    },
                    OtherMoves = new PMove[]
                    {
                        // light screen
                        // protect
                        // rain dance
                        // dig
                        // brick break
                        // facade
                        // reset
                        // attract
                        // round
                        // echoed voice
                        // fling
                        // charge beam
                        // volt switch
                        // thunder wave
                        // grass knot
                        // swagger
                        // substitute
                        // wild charge
                        // bestow // egg
                        // bide
                        // charge
                        // double slap
                        // encore
                        // endure
                        // fake out
                        // flail
                        // lucky chant
                        // present
                        // reversal
                        // wish
                        // covet // tutor
                        // helping hand
                        // knock off
                        // magnet rise
                        // signal beam
                        // sleep talk
                        // snore
                        // captivate // gen 4 tm
                        // counter // gen 3 tutor
                        // defense curl // gen 3 tutor
                        // double edge // gen 3 tutor
                        // dynamic punch // gen 3 tutor
                        // focus punch // gen 4 tm
                        // mega kick // gen 3 tutor
                        // mimic // gen 3 tutor
                        // natural gift // gen 4 tm
                        // rollout // gen 4 tutor
                        // secret power // gen 4 tm
                        // seismic toss // gen 3 tutor
                        // shock wave // gen 4 tm
                        // uproar // tutor as Pichu
                        // volt tackle // egg as pichu
                        PMove.BodySlam, // Gen 3 Move Tutor
                        PMove.DoubleTeam, // TM
                        PMove.Flash, // TM
                        PMove.Frustration, // TM
                        PMove.Headbutt, // Gen 4 Move Tutor
                        PMove.HiddenPower, // TM
                        PMove.IronTail, // Move Tutor
                        PMove.MegaPunch, // Gen 3 Move Tutor
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Return, // TM
                        PMove.RockSmash, // TM
                        PMove.Strength, // HM
                        PMove.Swift, // Gen 4 Move Tutor
                        PMove.Tickle, // Egg Move
                        PMove.Thunder, // TM
                        PMove.Thunderbolt, // TM
                        PMove.ThunderPunch, // Egg Move, Move Tutor
                        PMove.Toxic, // TM
                    }
                }
            },
            {
                PSpecies.Cubone,
                new PPokemonData
                {
                    HP = 50, Attack = 50, Defense = 95, SpAttack = 40, SpDefense = 50, Speed = 35,
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Ground, Type2 = PType.Ground,
                    Abilities = new PAbility[] { PAbility.RockHead, PAbility.LightningRod, PAbility.BattleArmor },
                    MinLevel = 1, // Egg
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.Growl),
                        Tuple.Create(3, PMove.TailWhip),
                        Tuple.Create(7, PMove.BoneClub),
                        Tuple.Create(11, PMove.Headbutt),
                        Tuple.Create(13, PMove.Leer),
                        // focus energy 17
                        // bonemerang 21
                        // rage 23
                        // false swipe 27
                        // thrash 31
                        // fling 33
                        // bone rush 37
                        // endeavor 41
                        // double edge 43
                        Tuple.Create(47, PMove.Retaliate),
                    },
                    OtherMoves = new PMove[]
                    {
                        // sunny day
                        // blizzard
                        // protect
                        // smack down
                        // earthquake
                        // dig
                        // brick break
                        // sandstorm
                        // rock tomb
                        // facade
                        // rest
                        // attract
                        // thief
                        // round
                        // echoed voice
                        // false swipe
                        // fling
                        // incinerate
                        // bulldoze
                        // swagger
                        // substitute
                        // ancient power // egg
                        // belly drum
                        // chip away
                        // detect
                        // double kick
                        // endure
                        // iron head
                        // perish song
                        // skull bash
                        // earth power // tutor
                        // endeavor
                        // icy wind
                        // iron defense
                        // iron head
                        // knock off
                        // low kick
                        // sleep talk
                        // snore
                        // stealth rock
                        // uproar
                        // captivate // gen 4 tm
                        // counter // gen 3 tutor
                        // dynamic punch // gen 3 tutor
                        // focus punch // gen 4 tm
                        // fury cutter // gen 4 tutor
                        // mega kick // gen 3 tutor
                        // mimic // gen 3 tutor
                        // natural gift // gen 4 tm
                        // secret power // gen 4 tm
                        // seismic toss // gen 3 tutor
                        PMove.AerialAce, // TM
                        PMove.BodySlam, // Gen 3 Move Tutor
                        PMove.DoubleTeam, // TM
                        PMove.FireBlast, // TM
                        PMove.FirePunch, // Move Tutor
                        PMove.Flamethrower, // TM
                        PMove.Frustration, // TM
                        PMove.HiddenPower, // TM
                        PMove.IceBeam, // TM
                        PMove.IronTail, // Move Tutor
                        PMove.MegaPunch, // Gen 3 Move Tutor
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Retaliate, // TM
                        PMove.Return, // TM
                        PMove.RockSlide, // TM
                        PMove.RockSmash, // TM
                        PMove.Screech, // Egg Move
                        PMove.Strength, // HM
                        PMove.SwordsDance, // TM
                        PMove.ThunderPunch, // Move Tutor
                        PMove.Toxic, // TM
                    }
                }
            },
            {
                PSpecies.Marowak,
                new PPokemonData
                {
                    HP = 60, Attack = 80, Defense = 110, SpAttack = 50, SpDefense = 80, Speed = 45,
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Ground, Type2 = PType.Ground,
                    Abilities = new PAbility[] { PAbility.RockHead, PAbility.LightningRod, PAbility.BattleArmor },
                    MinLevel = 14, // HGSS (Rock Tunnel)
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.Growl),
                        Tuple.Create(1, PMove.BoneClub),
                        Tuple.Create(1, PMove.Headbutt),
                        Tuple.Create(1, PMove.TailWhip),
                        Tuple.Create(13, PMove.Leer),
                        // focus energy 17
                        // bonemerang 21
                        // rage 23
                        // false swipe 27
                        // thrash 31 // 31 as Cubone, 33
                        // fling 33 // 33 as Cubone, 37
                        // bone rush 37 // 37 as Cubone, 43
                        // endeavor 41 // 41 as Cubone, 49
                        // double edge 43 // 43 as Cubone, 53
                        Tuple.Create(47, PMove.Retaliate), // As Cubone
                        Tuple.Create(59, PMove.Retaliate),
                    },
                    OtherMoves = new PMove[]
                    {
                        // sunny day
                        // blizzard
                        // hyper beam
                        // protect
                        // smack down
                        // earthquake
                        // dig
                        // brick break
                        // sandstorm
                        // rock tomb
                        // facade
                        // rest
                        // attract
                        // thief
                        // round
                        // echoed voice
                        // focus blast
                        // false swipe
                        // fling
                        // incinerate
                        // giga impact
                        // stone edge
                        // bulldoze
                        // swagger
                        // substitute
                        // ancient power // egg move
                        // belly drum
                        // chip away
                        // detect
                        // double kick
                        // endure
                        // iron head
                        // perish song
                        // skull bash
                        // earth power // move tutor
                        // endeavor
                        // icy wind
                        // iron defense
                        // iron head
                        // knock off
                        // low kick
                        // outrage
                        // sleep talk
                        // snore
                        // stealth rock
                        // uproar
                        // captivate // gen 4 tm
                        // counter // gen 3 tutor
                        // dynamic punch // gen 3 tutor
                        // focus punch // gen 4 tm
                        // fury cutter // gen 4 tutor
                        // mega kick // gen 3 tutor
                        // mimic // gen 3 tutor
                        // natural gift // gen 4 tm
                        // secret power // gen 4 tm
                        // seismic toss // gen 3 tutor
                        PMove.AerialAce, // TM
                        PMove.BodySlam, // Gen 3 Move Tutor
                        PMove.DoubleTeam, // TM
                        PMove.FireBlast, // TM
                        PMove.FirePunch, // Move Tutor
                        PMove.Flamethrower, // TM
                        PMove.Frustration, // TM
                        PMove.HiddenPower, // TM
                        PMove.IceBeam, // TM
                        PMove.IronTail, // Move Tutor
                        PMove.MegaPunch, // Gen 3 Move Tutor
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Retaliate, // TM
                        PMove.Return, // TM
                        PMove.RockSlide, // TM
                        PMove.RockSmash, // TM
                        PMove.Screech, // Egg Move
                        PMove.Strength, // HM
                        PMove.SwordsDance, // TM
                        PMove.ThunderPunch, // Move Tutor
                        PMove.Toxic, // TM
                    }
                }
            },
            {
                PSpecies.Ditto,
                new PPokemonData
                {
                    HP = 48, Attack = 48, Defense = 48, SpAttack = 48, SpDefense = 48, Speed = 48,
                    GenderRatio = PGender.Genderless,
                    Type1 = PType.Normal, Type2 = PType.Normal,
                    Abilities = new PAbility[] { PAbility.Limber, PAbility.Imposter },
                    MinLevel = 10, // HGSS (Route 34, Route 35)
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.Transform),
                    },
                    OtherMoves = new PMove[]
                    {

                    }
                }
            },
            {
                PSpecies.Pichu,
                new PPokemonData
                {
                    HP = 20, Attack = 40, Defense = 15, SpAttack = 35, SpDefense = 35, Speed = 60,
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Electric, Type2 = PType.Electric,
                    Abilities = new PAbility[] { PAbility.Static, PAbility.LightningRod },
                    MinLevel = 1, // Egg
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.ThunderShock),
                        Tuple.Create(1, PMove.Charm),
                        Tuple.Create(5, PMove.TailWhip),
                        // follow me 5 // Gen 3 event (PokéPark Egg Pichu)
                        // teeter dance 5 // Gen 3 event (Teeter Dance Pichu, Pokémon Stamp Ruby and Sapphire Contest Pichu)
                        // thunder wave 8 // Gen 3 8, Gen 4 / Gen 5 10
                        // sweet kiss 11 // Gen 3 11, Gen 4 / Gen 5 13
                        // nasty plot 18                        
                        // endeavor 30 // gen 4 event (Shokotan Pikachu-colored Pichu, Mikena Pichu, GameStop Pichu)
                    },
                    OtherMoves = new PMove[]
                    {
                        // light screen
                        // protect
                        // rain dance
                        // facade
                        // rest
                        // attract
                        // round
                        // echoed voice
                        // fling
                        // charge beam
                        // volt switch
                        // thunder wave
                        // grass knot
                        // swagger
                        // substitute
                        // wild charge
                        // volt tackle // egg
                        // bestow
                        // bide
                        // charge
                        // double slap
                        // encore
                        // endure
                        // fake out
                        // flail
                        // lucky chant
                        // present
                        // reversal
                        // wish
                        // captivate // gen 4 tm
                        // counter // gen 3 tutor
                        // defense curl // gen 3 tutor
                        // double edge // gen 3 tutor
                        // mega kick // gen 3 tutor
                        // mimic // gen 3 tutor
                        // natural gift // gen 4 tm
                        // rollout // gen 4 tutor
                        // secret power // gen 4 tm
                        // seismic toss // gen 3 tutor
                        // shock wave // gen 4 tm
                        PMove.BodySlam, // Gen 3 Move Tutor
                        PMove.DoubleTeam, // TM
                        PMove.Flash, // TM
                        PMove.Frustration, // TM
                        PMove.Headbutt, // Gen 4 Move Tutor
                        PMove.HiddenPower, // TM
                        PMove.MegaPunch, // Gen 3 Move Tutor
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Return, // TM
                        PMove.Swift, // Gen 4 Move Tutor
                        PMove.Tickle, // Egg Move
                        PMove.Thunder, //TM
                        PMove.Thunderbolt, // TM
                        PMove.ThunderPunch, // Egg Move
                        PMove.Toxic, // TM
                    }
                }
            },
            {
                PSpecies.Azumarill,
                new PPokemonData
                {
                    HP = 100, Attack = 50, Defense = 80, SpAttack = 50, SpDefense = 80, Speed = 50,
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Water, Type2 = PType.Water,
                    Abilities = new PAbility[] { PAbility.ThickFat, PAbility.HugePower, PAbility.SapSipper },
                    MinLevel = 5, // B2W2 (Route 20, Flocessy Ranch, Relic Passage)
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        // tackle 1
                        Tuple.Create(1, PMove.Bubble),
                        Tuple.Create(1, PMove.TailWhip),
                        Tuple.Create(1, PMove.WaterGun),
                        // water sport 1 // B2W2 Level 1 & 5
                        // defense curl 1 // Gen 3 1 & 3, Gen 4 / BW 1 & 2, B2W2 10
                        Tuple.Create(2, PMove.Charm), // Gen 4 / BW as Azurill
                        Tuple.Create(10, PMove.Charm), // B2W2 as Azurill
                        // rollout 10 // Gen 3 / Gen 4 / BW 15, B2W2 10
                        Tuple.Create(13, PMove.BubbleBeam),
                        // helping hand 16 // B2W2 16
                        // aqua tail 21 // Gen 4 / BW 47, B2W2 21
                        // double edge 25 // Gen 3 34, Gen 4 / BW 33, B2W2 25
                        // aqua ring 23 // Gen 3 / Gen 4 23 as Marill, B2W2 31
                        // superpower 42 // B2W2 42
                        // rain dance 32 // Gen 3 / Gen 4 32 as Marill, B2W2 35
                        Tuple.Create(42, PMove.HydroPump), // Gen 4 as Marill
                        Tuple.Create(46, PMove.HydroPump), // B2W2
                        // splash 1 // As Azurill
                    },
                    OtherMoves = new PMove[]
                    {
                        // surf
                        // dive
                        // hail
                        // blizzard
                        // hyper beam
                        // light screen
                        // protect
                        // rain dance
                        // dig
                        // brick break
                        // facade
                        // reset
                        // attract
                        // round
                        // focus blast
                        // scald
                        // fling
                        // giga impact
                        // bulldoze
                        // work up
                        // grass knot
                        // swagger
                        // substitute
                        // belly drum // egg move
                        // encore
                        // fake tears
                        // future sight
                        // muddy water
                        // perish song
                        // present
                        // refresh
                        // sing
                        // soak
                        // superpower
                        // supersonic
                        // water sport // egg move
                        // aqua tail // tutor
                        // bounce
                        // covet
                        // helping hand
                        // hyper voice
                        // icy wind
                        // knock off
                        // sleep talk
                        // snore
                        // superpower
                        // captivate // gen 4 tm
                        // dynamic punch // gen 3 tutor
                        // endure // gen 4 tm
                        // focus punch // gen 4 tm
                        // mega kick // gen 3 tutor
                        // mimic // gen 3 tutor
                        // natural gift // gen 4 tm
                        // secret power // gen 4 tm
                        // seismic toss // gen 3 tutor
                        // water pulse // gen 4 tm
                        // whirlpool // gen 4 HM
                        // bounce // move tutor as azurill / marill
                        // uproar // move tutor as azurill
                        PMove.Amnesia, // Egg Move
                        PMove.AquaJet, // Egg Move
                        PMove.BodySlam, // Egg Move
                        PMove.DoubleTeam, // TM
                        PMove.Frustration, // TM
                        PMove.Headbutt, // Gen 4 Move Tutor
                        PMove.HiddenPower, // TM
                        PMove.IceBeam, // TM
                        PMove.IcePunch, // Move Tutor
                        PMove.IronTail, // Move Tutor
                        PMove.MegaPunch, // Gen 3 Move Tutor
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Return, // TM
                        PMove.RockSmash, // TM
                        PMove.Slam, // Egg Move
                        PMove.Strength, // HM
                        PMove.Swift, // Gen 4 Move Tutor
                        PMove.Tickle, // Egg Move
                        PMove.Toxic, // TM
                        PMove.Waterfall, // HM
                    }
                }
            },
            {
                PSpecies.Clamperl,
                new PPokemonData
                {
                    HP = 35, Attack = 64, Defense = 85, SpAttack = 74, SpDefense = 55, Speed = 32,
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Water, Type2 = PType.Water,
                    Abilities = new PAbility[] { PAbility.ShellArmor, PAbility.Rattled },
                    MinLevel = 1, // Egg
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        // clamp 1
                        // iron defense 1
                        Tuple.Create(1, PMove.WaterGun),
                        // whirlpool 1
                        Tuple.Create(51, PMove.ShellSmash),
                    },
                    OtherMoves = new PMove[]
                    {
                        // surf
                        // dive
                        // hail
                        // blizzard
                        // protect
                        // rain dance
                        // facade
                        // rest
                        // attract
                        // round
                        // scald
                        // swagger
                        // substitute
                        // aqua ring // egg move
                        // barrier
                        // brine
                        // confuse ray
                        // endure
                        // mud sport
                        // muddy water
                        // refresh
                        // supersonic
                        // water pulse
                        // icy wind // tutor
                        // iron defense
                        // sleep talk
                        // snore
                        // captivate // gen 4 tm
                        // double edge // gen 3 tutor
                        // mimic // gen 3 tutor
                        // natural gift // gen 4 tm
                        // secret power // gen 4 tm
                        PMove.BodySlam, // Egg Move
                        PMove.DoubleTeam, // TM
                        PMove.Frustration, // TM
                        PMove.HiddenPower, // TM
                        PMove.IceBeam, // TM
                        PMove.Return, // TM
                        PMove.Toxic, // TM
                        PMove.Waterfall, // HM
                    }
                }
            },
            {
                PSpecies.Latias,
                new PPokemonData
                {
                    HP = 80, Attack = 80, Defense = 90, SpAttack = 110, SpDefense = 130, Speed = 110,
                    GenderRatio = PGender.Female,
                    Type1 = PType.Dragon, Type2 = PType.Psychic,
                    Abilities = new PAbility[] { PAbility.Levitate },
                    MinLevel = 35, // HG (Roaming)
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        // psywave 1
                        // wish 5
                        // helping hand 10
                        // safeguard 15
                        Tuple.Create(20, PMove.DragonBreath),
                        // water sport 25
                        // refresh 30
                        // mist ball 35
                        Tuple.Create(40, PMove.Psychic), // Gen 3
                        // zen headbutt 40
                        // recover 45
                        // psycho shift 50
                        Tuple.Create(50, PMove.Charm), // Gen 3
                        Tuple.Create(55, PMove.Charm), // Gen 4-5
                        Tuple.Create(60, PMove.Psychic), // Gen 5
                        // healing wish 60 // Gen 4 60, Gen 5 85
                        // heal pulse 65
                        Tuple.Create(70, PMove.DragonPulse), // Gen 4
                        // reflect type 70
                        // guard split 75
                        Tuple.Create(80, PMove.DragonPulse), // Gen 5
                    },
                    OtherMoves = new PMove[]
                    {
                        // fly
                        // surf
                        // dive
                        // hone claws
                        // psyshock
                        // roar
                        // sunny day
                        // hyper beam
                        // light screen
                        // protect
                        // rain dance
                        // telekinesis
                        // safeguard
                        // solar beam
                        // earthquake
                        // reflect
                        // sandstorm
                        // facade
                        // rest
                        // attract
                        // round
                        // energy ball
                        // charge beam
                        // shadow claw
                        // giga impact
                        // thunder wave
                        // psych up
                        // bulldoze
                        // dream eater
                        // grass knot
                        // swagger
                        // substitute
                        // covet // tutor
                        // draco meteor // tutor
                        // helping hand // tutor
                        // icy wind // tutor
                        // last resort // tutor
                        // magic coat // tutor
                        // magic room
                        // outrage
                        // role play
                        // roost
                        // sleep talk
                        // snore
                        // tailwind
                        // trick
                        // zen headbutt // tutor
                        // captivate // gen 4 tm
                        // defog // gen 4 hm
                        // double edge // gen 3 tutor
                        // endure // gen 4 tm
                        // fury cutter // gen 4 tutor
                        // mimic // gen 3 tutor
                        // natural gift // gen 4 tm
                        // secret power // gen 4 tm
                        // shock wave // gen 4 tm
                        // sucker punch // gen 4 tutor
                        // twister // gen 4 tutor
                        // water pulse // gen 4 tm
                        // whirlpool // gen 4 hm
                        PMove.AerialAce, // TM
                        PMove.BodySlam, // Gen 3 Move Tutor
                        PMove.CalmMind, // TM
                        PMove.Cut, // HM
                        PMove.DragonClaw, // TM
                        PMove.DragonPulse, // Move Tutor
                        PMove.DoubleTeam, // TM
                        PMove.Flash, // TM
                        PMove.Frustration, // TM
                        PMove.HiddenPower, // TM
                        PMove.IceBeam, // TM
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Psychic, // TM
                        PMove.Retaliate, // TM
                        PMove.Return, // TM
                        PMove.ShadowBall, // TM
                        PMove.SteelWing, // TM
                        PMove.Swift, // Gen 4 Move Tutor
                        PMove.Thunder, // TM
                        PMove.Thunderbolt, // TM
                        PMove.Toxic, // TM
                        PMove.Waterfall, // HM
                    }
                }
            },
            {
                PSpecies.Latios,
                new PPokemonData
                {
                    HP = 80, Attack = 90, Defense = 80, SpAttack = 130, SpDefense = 110, Speed = 110,
                    GenderRatio = PGender.Male,
                    Type1 = PType.Dragon, Type2 = PType.Psychic,
                    Abilities = new PAbility[] { PAbility.Levitate },
                    MinLevel = 35, // SS (Roaming)
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        // psywave 1
                        // heal block 5
                        // helping hand 10
                        // safeguard 15
                        Tuple.Create(20, PMove.DragonBreath),
                        // protect 25
                        // refresh 30
                        // luster purge 35
                        // telekinesis 70
                        // power split 75
                        // memento 5 // Gen 3 5, Gen 4 60, Gen 5 85
                        Tuple.Create(40, PMove.Psychic), // Gen 3
                        // zen headbutt 40
                        // recover 45
                        // psycho shift 50
                        Tuple.Create(50, PMove.DragonDance), // Gen 3
                        Tuple.Create(55, PMove.DragonDance), // Gen 4 / Gen 5
                        Tuple.Create(60, PMove.Psychic), // Gen 5
                        // heal pulse 65
                        Tuple.Create(70, PMove.DragonPulse), // Gen 4
                        Tuple.Create(80, PMove.DragonPulse), // Gen 5
                    },
                    OtherMoves = new PMove[]
                    {
                        // fly
                        // surf
                        // dive
                        // hone claws
                        // psyshock
                        // roar
                        // sunny day
                        // hyper beam
                        // light screen
                        // protect
                        // rain dance
                        // telekinesis
                        // safeguard
                        // solar beam
                        // earthquake
                        // reflect
                        // sandstorm
                        // facade
                        // rest
                        // attract
                        // round
                        // energy ball
                        // charge beam
                        // shadow claw
                        // giga impact
                        // thunder wave
                        // psych up
                        // bulldoze
                        // dream eater
                        // grass knot
                        // swagger
                        // substitute
                        // draco meteor // tutor
                        // helping hand // tutor
                        // icy wind // tutor
                        // last resort // tutor
                        // magic coat // tutor
                        // outrage // tutor
                        // roost // tutor
                        // sleep talk // tutor
                        // snore // tutor
                        // tailwind // tutor
                        // trick // tutor
                        // wonder room // tutor
                        // zen headbutt // tutor
                        // captivate // gen 4 tm
                        // defog // gen 4 hm
                        // double edge // gen 3 tutor
                        // endure // gen 4 tm
                        // fury cutter // gen 4 tutor
                        // mimic // gen 3 tutor
                        // natural gift // gen 4 tm
                        // secret power // gen 4 tm
                        // shock wave // gen 4 tm
                        // twister // gen 4 tutor
                        // water pulse // gen 4 tm
                        // whirlpool // gen 4 hm
                        PMove.AerialAce, // TM
                        PMove.BodySlam, // Gen 3 Move Tutor
                        PMove.CalmMind, // TM
                        PMove.Cut, // HM
                        PMove.DragonClaw, // TM
                        PMove.DragonPulse, // Move Tutor
                        PMove.DoubleTeam, // TM
                        PMove.Flash, // TM
                        PMove.Frustration, // TM
                        PMove.HiddenPower, // TM
                        PMove.IceBeam, // TM
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Psychic, // TM
                        PMove.Retaliate, // TM
                        PMove.Return, // TM
                        PMove.ShadowBall, // TM
                        PMove.SteelWing, // TM
                        PMove.Swift, // Gen 4 Move Tutor
                        PMove.Thunder, // TM
                        PMove.Thunderbolt, // TM
                        PMove.Toxic, // TM
                        PMove.Waterfall, // HM
                    }
                }
            },
            {
                PSpecies.Cresselia,
                new PPokemonData
                {
                    HP = 120, Attack = 70, Defense = 120, SpAttack = 75, SpDefense = 130, Speed = 85,
                    GenderRatio = PGender.Female,
                    Type1 = PType.Psychic, Type2 = PType.Psychic,
                    Abilities = new PAbility[] { PAbility.Levitate },
                    MinLevel = 50, // DPPt (Roaming)
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        // confusion 1
                        Tuple.Create(1, PMove.DoubleTeam),
                        // safeguard 11
                        // mist 20
                        Tuple.Create(29, PMove.AuroraBeam),
                        // future sight 38
                        // slash 47
                        Tuple.Create(57, PMove.Moonlight),
                        // psycho cut 66
                        // psycho shift 75
                        // lunar dance 84
                        Tuple.Create(93, PMove.Psychic),
                    },
                    OtherMoves = new PMove[]
                    {
                        // psyshock
                        // sunny day
                        // hyper beam
                        // light screen
                        // protect
                        // rain dance
                        // telekinesis
                        // safeguard
                        // solar beam
                        // Reflect
                        // facade
                        // rest
                        // attract
                        // round
                        // energy ball
                        // charge beam
                        // giga impact
                        // thunder wave
                        // psych up
                        // dream eater
                        // grass knot
                        // swagger
                        // substitute
                        // trick room
                        // gravity // move tutor
                        // helping hand // tutor
                        // icy wind // tutor
                        // magic coat // tutor
                        // magic room // tutor
                        // recycle // tutor
                        // signal beam // tutor
                        // skill swap // tutor
                        // sleep talk // tutor
                        // snore // tutor
                        // trick // tutor
                        // zen headbutt // tutor
                        // captivate // gen 4 tm
                        // endure // gen 4 tm
                        // fury cutter // gen 4 tutor
                        // natural gift // gen 4 tm
                        // secret power // gen 4 tm
                        PMove.CalmMind, // TM
                        PMove.DoubleTeam, // TM
                        PMove.Flash, // TM
                        PMove.Frustration, // TM
                        PMove.HiddenPower, // TM
                        PMove.IceBeam, // TM
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Psychic, // TM
                        PMove.Return, // TM
                        PMove.ShadowBall, // TM
                        PMove.Swift, // Gen 4 Move Tutor
                        PMove.Toxic, // TM
                    }
                }
            },
            {
                PSpecies.Darkrai,
                new PPokemonData
                {
                    HP = 70, Attack = 90, Defense = 90, SpAttack = 135, SpDefense = 90, Speed = 125,
                    GenderRatio = PGender.Genderless,
                    Type1 = PType.Dark, Type2 = PType.Dark,
                    Abilities = new PAbility[] { PAbility.BadDreams },
                    MinLevel = 40, // DP (Newmoon Island)
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        // disable 1
                        // night shade 1 // DP
                        // ominous wind 1
                        Tuple.Create(11, PMove.QuickAttack),
                        // hypnosis 20
                        // pursuit 29 // DP
                        // faint attack 29
                        // nightmare 38
                        Tuple.Create(47, PMove.DoubleTeam),
                        // haze 57
                        // dark void 50 // Events
                        // dark void 66
                        // roar of time 50 // gen 4 event (Alamos Darkrai, Nintendo of Korea Darkrai, Cinema Darkrai)
                        // spacial rend 50 // gen 4 event (Alamos Darkrai, Nintendo of Korea Darkrai, Cinema Darkrai)
                        // embargo 75 // DP
                        Tuple.Create(75, PMove.NastyPlot),
                        // dream eater 84
                        Tuple.Create(93, PMove.DarkPulse),
                    },
                    OtherMoves = new PMove[]
                    {
                        // sunny day
                        // taunt
                        // blizzard
                        // hyper beam
                        // protect
                        // rain dance
                        // brick break
                        // rock tomb
                        // torment
                        // facade
                        // rest
                        // thief
                        // round
                        // focus blast
                        // fling
                        // charge beam
                        // incinerate
                        // will-o-wisp
                        // embargo
                        // shadow claw
                        // payback
                        // giga impact
                        // thunder wave
                        // psych up
                        // poison jab
                        // dream eater
                        // swagger
                        // substitute
                        // snarl
                        // drain punch // move tutor
                        // foul play // move tutor
                        // icy wind // move tutor
                        // knock off // move tutor
                        // last resort // move tutor
                        // sleep talk // move tutor
                        // snatch // move tutor
                        // snore // move tutor
                        // spite // move tutor
                        // trick // move tutor
                        // wonder room // move tutor
                        // endure // gen 4 TM
                        // focus punch // gen 4 TM
                        // natural gift // gen 4 TM
                        // secret power // gen 4 TM
                        // shock wave // gen 4 TM
                        // sucker punch // gen 4 move tutor
                        PMove.AerialAce, // TM
                        PMove.CalmMind, // TM
                        PMove.Cut, // HM
                        PMove.DarkPulse, // Move tutor
                        PMove.DoubleTeam, // TM
                        PMove.Flash, // TM
                        PMove.Frustration, // TM
                        PMove.Headbutt, // Gen 4 Move Tutor
                        PMove.HiddenPower, // TM
                        PMove.IceBeam, // TM
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Psychic, // TM
                        PMove.Retaliate, // TM
                        PMove.Return, // TM
                        PMove.RockSlide, // TM
                        PMove.RockSmash, // TM
                        PMove.ShadowBall, // TM
                        PMove.SludgeBomb, // TM
                        PMove.Strength, // HM
                        PMove.Swift, // Gen 4 Move Tutor
                        PMove.SwordsDance, // TM
                        PMove.Thunder, // TM
                        PMove.Thunderbolt, // TM
                        PMove.Toxic, // TM
                        PMove.XScissor, // TM
                    }
                }
            },
        };
    }
}
