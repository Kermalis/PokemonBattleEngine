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
        public bool ShinyLocked;
        public double Weight; // Kilograms
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
                    ShinyLocked = false,
                    Weight = 6.0,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.Growl),
                        Tuple.Create(1, PMove.ThunderShock),
                        Tuple.Create(1, PMove.Charm), // As Pichu
                        Tuple.Create(5, PMove.TailWhip),
                        // follow me 5 // gen 3 pichu event
                        Tuple.Create(5, PMove.TeeterDance), // Gen 3 Pichu Event
                        // thunder wave 8 // 8 As Pichu, 10
                        Tuple.Create(11, PMove.SweetKiss), // As Pichu
                        Tuple.Create(13, PMove.QuickAttack),
                        // electro ball 18
                        Tuple.Create(18, PMove.NastyPlot), // As Pichu
                        Tuple.Create(21, PMove.DoubleTeam),
                        Tuple.Create(26, PMove.Slam),
                        Tuple.Create(29, PMove.Thunderbolt),
                        Tuple.Create(30, PMove.Sing), // Gen 5 Event (Singing Pikachu)
                        // last resort 30 // gen 4 event (Kyoto Cross Media Experience 2009 Pikachu)
                        // endeavor 30 // gen 4 pichu event
                        // feint 34
                        Tuple.Create(37, PMove.Agility),
                        Tuple.Create(42, PMove.Discharge),
                        Tuple.Create(45, PMove.LightScreen),
                        Tuple.Create(50, PMove.ExtremeSpeed), // Gen 5 event (ExtremeSpeed Pikachu)
                        // yawn 50 // gen 4 event (Sleeping Pikachu)
                        // rest 50 // gen 4 event (Sleeping Pikachu)
                        Tuple.Create(50, PMove.Thunder),
                    },
                    OtherMoves = new PMove[]
                    {
                        // dig
                        // brick break
                        // facade
                        // reset
                        // attract
                        // round
                        // echoed voice
                        // fling
                        // volt switch
                        // thunder wave
                        // swagger
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
                        // uproar // tutor as Pichu
                        // volt tackle // egg as pichu
                        PMove.BodySlam, // Gen 3 Move Tutor
                        PMove.ChargeBeam, // TM
                        PMove.DoubleTeam, // TM
                        PMove.Flash, // TM
                        PMove.Frustration, // TM
                        PMove.GrassKnot, // TM
                        PMove.Headbutt, // Gen 4 Move Tutor
                        PMove.HiddenPower, // TM
                        PMove.IronTail, // Move Tutor
                        PMove.LightScreen, // TM
                        PMove.MegaPunch, // Gen 3 Move Tutor
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Protect, // TM
                        PMove.RainDance, // TM
                        PMove.Return, // TM
                        PMove.RockSmash, // TM
                        PMove.ShockWave, // Gen 4 TM
                        PMove.SignalBeam, // Move Tutor
                        PMove.Strength, // HM
                        PMove.Substitute, // TM
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
                    ShinyLocked = false,
                    Weight = 6.5,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.Growl),
                        Tuple.Create(3, PMove.TailWhip),
                        Tuple.Create(7, PMove.BoneClub),
                        // 10 perish song (dream world)
                        Tuple.Create(10, PMove.LowKick), // Dream World
                        Tuple.Create(11, PMove.Headbutt),
                        Tuple.Create(13, PMove.Leer),
                        Tuple.Create(17, PMove.FocusEnergy),
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
                        // blizzard
                        // smack down
                        // earthquake
                        // dig
                        // brick break
                        // sandstorm
                        // facade
                        // rest
                        // attract
                        // thief
                        // round
                        // echoed voice
                        // false swipe
                        // fling
                        // incinerate
                        // swagger
                        // ancient power // egg
                        // belly drum
                        // chip away
                        // double kick
                        // endure
                        // perish song
                        // skull bash
                        // endeavor // tutor
                        // knock off
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
                        PMove.Bulldoze, // TM
                        PMove.Detect, // Egg Move
                        PMove.DoubleTeam, // TM
                        PMove.EarthPower, // Move Tutor
                        PMove.FireBlast, // TM
                        PMove.FirePunch, // Move Tutor
                        PMove.Flamethrower, // TM
                        PMove.Frustration, // TM
                        PMove.HiddenPower, // TM
                        PMove.IceBeam, // TM
                        PMove.IcyWind, // Move Tutor
                        PMove.IronDefense, // Move Tutor
                        PMove.IronHead, // Egg Move / Move Tutor
                        PMove.IronTail, // Move Tutor
                        PMove.LowKick, // Move Tutor
                        PMove.MegaPunch, // Gen 3 Move Tutor
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Protect, // TM
                        PMove.Retaliate, // TM
                        PMove.Return, // TM
                        PMove.RockSlide, // TM
                        PMove.RockSmash, // TM
                        PMove.RockTomb, // TM
                        PMove.Screech, // Egg Move
                        PMove.Strength, // HM
                        PMove.Substitute, // TM
                        PMove.SunnyDay, // TM
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
                    ShinyLocked = false,
                    Weight = 45.0,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.Growl),
                        Tuple.Create(1, PMove.BoneClub),
                        Tuple.Create(1, PMove.Headbutt),
                        Tuple.Create(1, PMove.TailWhip),
                        Tuple.Create(13, PMove.Leer),
                        Tuple.Create(17, PMove.FocusEnergy),
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
                        // blizzard
                        // hyper beam
                        // smack down
                        // earthquake
                        // dig
                        // brick break
                        // sandstorm
                        // facade
                        // rest
                        // attract
                        // thief
                        // round
                        // echoed voice
                        // false swipe
                        // fling
                        // incinerate
                        // giga impact
                        // swagger
                        // ancient power // egg move
                        // belly drum
                        // chip away
                        // double kick
                        // endure
                        // perish song
                        // skull bash
                        // endeavor // tutor
                        // knock off
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
                        PMove.Bulldoze, // TM
                        PMove.Detect, // Egg Move
                        PMove.DoubleTeam, // TM
                        PMove.EarthPower, // TM
                        PMove.FireBlast, // TM
                        PMove.FirePunch, // Move Tutor
                        PMove.Flamethrower, // TM
                        PMove.FocusBlast, // TM
                        PMove.Frustration, // TM
                        PMove.HiddenPower, // TM
                        PMove.IceBeam, // TM
                        PMove.IcyWind, // Move Tutor
                        PMove.IronDefense, // Move Tutor
                        PMove.IronHead, // Egg Move / Move Tutor
                        PMove.IronTail, // Move Tutor
                        PMove.LowKick, // Move Tutor
                        PMove.MegaPunch, // Gen 3 Move Tutor
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Protect, // TM
                        PMove.Retaliate, // TM
                        PMove.Return, // TM
                        PMove.RockSlide, // TM
                        PMove.RockSmash, // TM
                        PMove.RockTomb, // TM
                        PMove.Screech, // Egg Move
                        PMove.StoneEdge, // TM
                        PMove.Strength, // HM
                        PMove.Substitute, // TM
                        PMove.SunnyDay, // TM
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
                    ShinyLocked = false,
                    Weight = 4.0,
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
                    ShinyLocked = false,
                    Weight = 2.0,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.ThunderShock),
                        Tuple.Create(1, PMove.Charm),
                        Tuple.Create(5, PMove.TailWhip),
                        // follow me 5 // Gen 3 event (PokéPark Egg Pichu)
                        Tuple.Create(5, PMove.TeeterDance), // Gen 3 Event (Teeter Dance Pichu, Pokémon Stamp Ruby and Sapphire Contest Pichu)
                        // thunder wave 8 // Gen 3 8, Gen 4 / Gen 5 10
                        Tuple.Create(11, PMove.SweetKiss), // Gen 3
                        Tuple.Create(13, PMove.SweetKiss), // Gen 4 / 5
                        // nasty plot 18                        
                        // endeavor 30 // gen 4 event (Shokotan Pikachu-colored Pichu, Mikena Pichu, GameStop Pichu)
                    },
                    OtherMoves = new PMove[]
                    {
                        // facade
                        // rest
                        // attract
                        // round
                        // echoed voice
                        // fling
                        // volt switch
                        // thunder wave
                        // swagger
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
                        PMove.BodySlam, // Gen 3 Move Tutor
                        PMove.ChargeBeam, // TM
                        PMove.DoubleTeam, // TM
                        PMove.Flash, // TM
                        PMove.Frustration, // TM
                        PMove.GrassKnot, // TM
                        PMove.Headbutt, // Gen 4 Move Tutor
                        PMove.HiddenPower, // TM
                        PMove.LightScreen, // TM
                        PMove.MegaPunch, // Gen 3 Move Tutor
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Protect, // TM
                        PMove.RainDance, // TM
                        PMove.Return, // TM
                        PMove.ShockWave, // Gen 4 TM
                        PMove.Substitute, // TM
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
                    ShinyLocked = false,
                    Weight = 28.5,
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
                        Tuple.Create(21, PMove.AquaTail),
                        // double edge 25 // Gen 3 34, Gen 4 / BW 33, B2W2 25
                        // aqua ring 23 // Gen 3 / Gen 4 23 as Marill, B2W2 31
                        // superpower 42 // B2W2 42
                        Tuple.Create(32, PMove.RainDance), // Gen 3 / Gen 4 as Marill
                        Tuple.Create(35, PMove.RainDance), // B2W2
                        Tuple.Create(42, PMove.HydroPump), // Gen 4 as Marill
                        Tuple.Create(46, PMove.HydroPump), // B2W2
                        // splash 1 // As Azurill
                    },
                    OtherMoves = new PMove[]
                    {
                        // hail
                        // blizzard
                        // hyper beam
                        // dig
                        // brick break
                        // facade
                        // reset
                        // attract
                        // round
                        // fling
                        // giga impact
                        // swagger
                        // belly drum // egg move
                        // encore
                        // future sight
                        // perish song
                        // present
                        // refresh
                        // soak
                        // superpower
                        // water sport
                        // bounce // tutor
                        // covet
                        // helping hand
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
                        // whirlpool // gen 4 HM
                        // bounce // move tutor as azurill / marill
                        // uproar // move tutor as azurill
                        PMove.Amnesia, // Egg Move
                        PMove.AquaJet, // Egg Move
                        PMove.AquaTail, // Move Tutor
                        PMove.BodySlam, // Egg Move
                        PMove.Bulldoze, // TM
                        PMove.Dive, // HM
                        PMove.DoubleTeam, // TM
                        PMove.FakeTears, // Move Tutor
                        PMove.FocusBlast, // TM
                        PMove.Frustration, // TM
                        PMove.GrassKnot, // TM
                        PMove.Headbutt, // Gen 4 Move Tutor
                        PMove.HiddenPower, // TM
                        PMove.HyperVoice, // Move Tutor
                        PMove.IceBeam, // TM
                        PMove.IcePunch, // Move Tutor
                        PMove.IcyWind, // Move Tutor
                        PMove.IronTail, // Move Tutor
                        PMove.LightScreen, // TM
                        PMove.MegaPunch, // Gen 3 Move Tutor
                        PMove.MuddyWater, // Egg Move
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Protect, // TM
                        PMove.RainDance, // TM
                        PMove.Return, // TM
                        PMove.RockSmash, // TM
                        PMove.Scald, // TM
                        PMove.Sing, // Egg Move
                        PMove.Slam, // Egg Move
                        PMove.Strength, // HM
                        PMove.Substitute, // TM
                        PMove.Supersonic, // Egg Move
                        PMove.Surf, // HM
                        PMove.Swift, // Gen 4 Move Tutor
                        PMove.Tickle, // Egg Move
                        PMove.Toxic, // TM
                        PMove.Waterfall, // HM
                        PMove.WaterPulse, // Gen 4 TM
                        PMove.Workup, // TM
                    }
                }
            },
            {
                PSpecies.Unown_A,
                new PPokemonData
                {
                    HP = 48, Attack = 72, Defense = 48, SpAttack = 72, SpDefense = 48, Speed = 48,
                    GenderRatio = PGender.Genderless,
                    Type1 = PType.Psychic, Type2 = PType.Psychic,
                    Abilities = new PAbility[] { PAbility.Levitate },
                    MinLevel = 5, // HGSS (Ruins of Alph)
                    ShinyLocked = false,
                    Weight = 5.0,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.HiddenPower),
                    },
                    OtherMoves = new PMove[]
                    {

                    }
                }
            },
            {
                PSpecies.Absol,
                new PPokemonData
                {
                    HP = 65, Attack = 130, Defense = 60, SpAttack = 75, SpDefense = 60, Speed = 75,
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Dark, Type2 = PType.Dark,
                    Abilities = new PAbility[] { PAbility.Pressure, PAbility.SuperLuck, PAbility.Justified },
                    MinLevel = 1, // Egg
                    ShinyLocked = false,
                    Weight = 47.0,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.Scratch),
                        // 1 feint
                        Tuple.Create(4, PMove.Leer),
                        // 5 wish (gen 3 event [Wish Absol])
                        Tuple.Create(9, PMove.QuickAttack),
                        // 10 superpower (dream world)
                        Tuple.Create(10, PMove.Megahorn), // Dream World
                        // pursuit (12 b2w2)
                        // taunt (9 bw, 17 b2w2)
                        Tuple.Create(20, PMove.Bite),
                        Tuple.Create(25, PMove.DoubleTeam),
                        Tuple.Create(28, PMove.Slash),
                        Tuple.Create(25, PMove.SwordsDance), // BW
                        Tuple.Create(33, PMove.SwordsDance), // B2W2
                        // 36 future sight
                        Tuple.Create(41, PMove.NightSlash),
                        Tuple.Create(44, PMove.Detect),
                        Tuple.Create(49, PMove.PsychoCut),
                        // sucker punch (44 bw, 52 bw2)
                        // razor wind (17 bw, 57 bw2)
                        // me first (57 bw, 60 bw2)
                        // perish song (46 gen 3, 65 gen 4/5)
                    },
                    OtherMoves = new PMove[]
                    {
                        // hail // tm
                        // taunt
                        // blizzard
                        // hyper beam
                        // sandstorm
                        // torment
                        // facade
                        // rest
                        // attract
                        // thief
                        // round
                        // echoed voice
                        // false swipe
                        // incinerate
                        // payback
                        // giga impact
                        // psych up
                        // dream eater
                        // swagger
                        // assurance // egg move
                        // baton pass
                        // curse
                        // double edge
                        // hex
                        // magic coat
                        // me first
                        // mean look
                        // perish song
                        // punishment
                        // sucker punch
                        // bounce // move tutor
                        // foul play
                        // knock off
                        // magic coat
                        // role play
                        // sleep talk
                        // snatch
                        // snore
                        // spite
                        // superpower
                        // captivate // gen 4 tm
                        // counter // gen 3 tutor
                        // endure // gen 4 tm
                        // fury cutter // gen 4 tutor
                        // mimic // gen 3 tutor
                        // natural gift // gen 4 tm
                        // secret power // gen 4 tm
                        PMove.AerialAce, // TM
                        PMove.BodySlam, // Gen 3 Move Tutor
                        PMove.CalmMind, // TM
                        PMove.ChargeBeam, // TM
                        PMove.Cut, // HM
                        PMove.DarkPulse, // Move Tutor
                        PMove.DoubleTeam, // TM
                        PMove.FaintAttack, // Egg Move
                        PMove.FireBlast, // TM
                        PMove.Flamethrower, // TM
                        PMove.Flash, // TM
                        PMove.Frustration, // TM
                        PMove.Headbutt, // Gen 4 Move Tutor
                        PMove.HiddenPower, // TM
                        PMove.HoneClaws, // TM
                        PMove.IceBeam, // TM
                        PMove.IcyWind, // Move Tutor
                        PMove.IronTail, // Move Tutor
                        PMove.Megahorn, // Egg Move
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Protect, // TM
                        PMove.RainDance, // TM
                        PMove.Retaliate, // TM
                        PMove.Return, // TM
                        PMove.RockSlide, // TM
                        PMove.RockSmash, // TM
                        PMove.RockTomb, // TM
                        PMove.ShadowBall, // TM
                        PMove.ShadowClaw, // TM
                        PMove.ShockWave, // Gen 4 TM
                        PMove.Snarl, // TM
                        PMove.StoneEdge, // TM
                        PMove.Strength, // HM
                        PMove.Substitute, // TM
                        PMove.SunnyDay, // TM
                        PMove.Swift, // Gen 4 Move Tutor
                        PMove.SwordsDance, // TM
                        PMove.Thunder, // TM
                        PMove.Thunderbolt, // TM
                        PMove.ThunderWave, // TM
                        PMove.Toxic, // TM
                        PMove.WaterPulse, // Gen 4 TM
                        PMove.WillOWisp, // TM
                        PMove.XScissor, // TM
                        PMove.ZenHeadbutt, // Egg Move / Move Tutor
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
                    ShinyLocked = false,
                    Weight = 52.5,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        // clamp 1
                        Tuple.Create(1, PMove.IronDefense),
                        Tuple.Create(1, PMove.WaterGun),
                        // whirlpool 1
                        // 10 captivate (dream world)
                        // 10 aqua ring (dream world)
                        Tuple.Create(51, PMove.ShellSmash),
                    },
                    OtherMoves = new PMove[]
                    {
                        // hail
                        // blizzard
                        // facade
                        // rest
                        // attract
                        // round
                        // swagger
                        // aqua ring // egg move
                        // brine
                        // endure
                        // mud sport
                        // refresh
                        // sleep talk // tutor
                        // snore
                        // captivate // gen 4 tm
                        // double edge // gen 3 tutor
                        // mimic // gen 3 tutor
                        // natural gift // gen 4 tm
                        // secret power // gen 4 tm
                        PMove.Barrier, // Egg Move
                        PMove.BodySlam, // Egg Move
                        PMove.ConfuseRay, // Egg Move
                        PMove.Dive, // HM
                        PMove.DoubleTeam, // TM
                        PMove.Frustration, // TM
                        PMove.HiddenPower, // TM
                        PMove.IceBeam, // TM
                        PMove.IcyWind, // Move Tutor
                        PMove.IronDefense, // Move Tutor
                        PMove.MuddyWater, // Egg Move
                        PMove.Protect, // TM
                        PMove.RainDance, // TM
                        PMove.Return, // TM
                        PMove.Scald, // TM
                        PMove.Substitute, // TM
                        PMove.Supersonic, // Egg Move
                        PMove.Surf, // HM
                        PMove.Toxic, // TM
                        PMove.Waterfall, // HM
                        PMove.WaterPulse, // Egg Move
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
                    ShinyLocked = false,
                    Weight = 40.0,
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
                        Tuple.Create(40, PMove.ZenHeadbutt),
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
                        // psyshock
                        // roar
                        // hyper beam
                        // telekinesis
                        // safeguard
                        // solar beam
                        // earthquake
                        // sandstorm
                        // facade
                        // rest
                        // attract
                        // round
                        // giga impact
                        // thunder wave
                        // psych up
                        // dream eater
                        // swagger
                        // covet // tutor
                        // helping hand // tutor
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
                        // captivate // gen 4 tm
                        // defog // gen 4 hm
                        // double edge // gen 3 tutor
                        // endure // gen 4 tm
                        // fury cutter // gen 4 tutor
                        // mimic // gen 3 tutor
                        // natural gift // gen 4 tm
                        // secret power // gen 4 tm
                        // sucker punch // gen 4 tutor
                        // twister // gen 4 tutor
                        // whirlpool // gen 4 hm
                        PMove.AerialAce, // TM
                        PMove.BodySlam, // Gen 3 Move Tutor
                        PMove.Bulldoze, // TM
                        PMove.CalmMind, // TM
                        PMove.ChargeBeam, // TM
                        PMove.Cut, // HM
                        PMove.Dive, // HM
                        PMove.DracoMeteor, // Move Tutor
                        PMove.DragonClaw, // TM
                        PMove.DragonPulse, // Move Tutor
                        PMove.DoubleTeam, // TM
                        PMove.EnergyBall, // TM
                        PMove.Flash, // TM
                        PMove.Frustration, // TM
                        PMove.GrassKnot, // TM
                        PMove.HiddenPower, // TM
                        PMove.HoneClaws, // TM
                        PMove.IceBeam, // TM
                        PMove.IcyWind, // Move Tutor
                        PMove.LightScreen, // TM
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Protect, // TM
                        PMove.Psychic, // TM
                        PMove.RainDance, // TM
                        PMove.Reflect, // TM
                        PMove.Retaliate, // TM
                        PMove.Return, // TM
                        PMove.ShadowBall, // TM
                        PMove.ShadowClaw, // TM
                        PMove.ShockWave, // Gen 4 TM
                        PMove.SteelWing, // TM
                        PMove.Substitute, // TM
                        PMove.SunnyDay, // TM
                        PMove.Surf, // HM
                        PMove.Swift, // Gen 4 Move Tutor
                        PMove.Thunder, // TM
                        PMove.Thunderbolt, // TM
                        PMove.Toxic, // TM
                        PMove.Waterfall, // HM
                        PMove.WaterPulse, // Gen 4 TM
                        PMove.ZenHeadbutt, // Move Tutor
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
                    ShinyLocked = false,
                    Weight = 60.0,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        // psywave 1
                        // heal block 5
                        // helping hand 10
                        // safeguard 15
                        Tuple.Create(20, PMove.DragonBreath),
                        Tuple.Create(25, PMove.Protect),
                        // refresh 30
                        Tuple.Create(35, PMove.LusterPurge),
                        // telekinesis 70
                        // power split 75
                        // memento 5 // Gen 3 5, Gen 4 60, Gen 5 85
                        Tuple.Create(40, PMove.Psychic), // Gen 3
                        Tuple.Create(40, PMove.ZenHeadbutt),
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
                        // psyshock
                        // roar
                        // hyper beam
                        // telekinesis
                        // safeguard
                        // solar beam
                        // earthquake
                        // sandstorm
                        // facade
                        // rest
                        // attract
                        // round
                        // giga impact
                        // thunder wave
                        // psych up
                        // dream eater
                        // swagger
                        // helping hand // tutor
                        // last resort // tutor
                        // magic coat // tutor
                        // outrage // tutor
                        // roost // tutor
                        // sleep talk // tutor
                        // snore // tutor
                        // tailwind // tutor
                        // trick // tutor
                        // wonder room // tutor
                        // captivate // gen 4 tm
                        // defog // gen 4 hm
                        // double edge // gen 3 tutor
                        // endure // gen 4 tm
                        // fury cutter // gen 4 tutor
                        // mimic // gen 3 tutor
                        // natural gift // gen 4 tm
                        // secret power // gen 4 tm
                        // twister // gen 4 tutor
                        // whirlpool // gen 4 hm
                        PMove.AerialAce, // TM
                        PMove.BodySlam, // Gen 3 Move Tutor
                        PMove.Bulldoze, // TM
                        PMove.CalmMind, // TM
                        PMove.ChargeBeam, // TM
                        PMove.Cut, // HM
                        PMove.Dive, // HM
                        PMove.DracoMeteor, // Move Tutor
                        PMove.DragonClaw, // TM
                        PMove.DragonPulse, // Move Tutor
                        PMove.DoubleTeam, // TM
                        PMove.EnergyBall, // TM
                        PMove.Flash, // TM
                        PMove.Frustration, // TM
                        PMove.GrassKnot, // TM
                        PMove.HiddenPower, // TM
                        PMove.HoneClaws, // TM
                        PMove.IceBeam, // TM
                        PMove.IcyWind, // Move Tutor
                        PMove.LightScreen, // TM
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Protect, // TM
                        PMove.Psychic, // TM
                        PMove.RainDance, // TM
                        PMove.Reflect, // TM
                        PMove.Retaliate, // TM
                        PMove.Return, // TM
                        PMove.ShadowBall, // TM
                        PMove.ShadowClaw, // TM
                        PMove.ShockWave, // Gen 4 TM
                        PMove.SteelWing, // TM
                        PMove.Substitute, // TM
                        PMove.SunnyDay, // TM
                        PMove.Surf, // HM
                        PMove.Swift, // Gen 4 Move Tutor
                        PMove.Thunder, // TM
                        PMove.Thunderbolt, // TM
                        PMove.Toxic, // TM
                        PMove.Waterfall, // HM
                        PMove.WaterPulse, // Gen 4 TM
                        PMove.ZenHeadbutt, // Move Tutor
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
                    ShinyLocked = false,
                    Weight = 85.6,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.Confusion),
                        Tuple.Create(1, PMove.DoubleTeam),
                        // safeguard 11
                        // mist 20
                        Tuple.Create(29, PMove.AuroraBeam),
                        // future sight 38
                        Tuple.Create(47, PMove.Slash),
                        Tuple.Create(57, PMove.Moonlight),
                        Tuple.Create(66, PMove.PsychoCut),
                        // psycho shift 75
                        // lunar dance 84
                        Tuple.Create(93, PMove.Psychic),
                    },
                    OtherMoves = new PMove[]
                    {
                        // psyshock
                        // hyper beam
                        // telekinesis
                        // safeguard
                        // solar beam
                        // facade
                        // rest
                        // attract
                        // round
                        // giga impact
                        // thunder wave
                        // psych up
                        // dream eater
                        // swagger
                        // trick room
                        // gravity // move tutor
                        // helping hand // tutor
                        // magic coat // tutor
                        // magic room // tutor
                        // recycle // tutor
                        // skill swap // tutor
                        // sleep talk // tutor
                        // snore // tutor
                        // trick // tutor
                        // captivate // gen 4 tm
                        // endure // gen 4 tm
                        // fury cutter // gen 4 tutor
                        // natural gift // gen 4 tm
                        // secret power // gen 4 tm
                        PMove.CalmMind, // TM
                        PMove.ChargeBeam, // TM
                        PMove.DoubleTeam, // TM
                        PMove.EnergyBall, // TM
                        PMove.Flash, // TM
                        PMove.Frustration, // TM
                        PMove.GrassKnot, // TM
                        PMove.HiddenPower, // TM
                        PMove.IceBeam, // TM
                        PMove.IcyWind, // Move Tutor
                        PMove.LightScreen, // TM
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.Protect, // TM
                        PMove.Psychic, // TM
                        PMove.RainDance, // TM
                        PMove.Reflect, // TM
                        PMove.Return, // TM
                        PMove.ShadowBall, // TM
                        PMove.SignalBeam, // Move Tutor
                        PMove.Substitute, // TM
                        PMove.SunnyDay, // TM
                        PMove.Swift, // Gen 4 Move Tutor
                        PMove.Toxic, // TM
                        PMove.ZenHeadbutt, // Move Tutor
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
                    ShinyLocked = false,
                    Weight = 50.5,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        // disable 1
                        // night shade 1 // DP
                        // ominous wind 1
                        Tuple.Create(11, PMove.QuickAttack),
                        Tuple.Create(20, PMove.Hypnosis),
                        // pursuit 29 // DP
                        Tuple.Create(29, PMove.FaintAttack),
                        // nightmare 38
                        Tuple.Create(47, PMove.DoubleTeam),
                        // haze 57
                        Tuple.Create(50, PMove.DarkVoid), // Events
                        Tuple.Create(66, PMove.DarkVoid),
                        // roar of time 50 // gen 4 event (Alamos Darkrai, Nintendo of Korea Darkrai, Cinema Darkrai)
                        // spacial rend 50 // gen 4 event (Alamos Darkrai, Nintendo of Korea Darkrai, Cinema Darkrai)
                        // embargo 75 // DP
                        Tuple.Create(75, PMove.NastyPlot),
                        // dream eater 84
                        Tuple.Create(93, PMove.DarkPulse),
                    },
                    OtherMoves = new PMove[]
                    {
                        // taunt
                        // blizzard
                        // hyper beam
                        // brick break
                        // torment
                        // facade
                        // rest
                        // thief
                        // round
                        // fling
                        // incinerate
                        // embargo
                        // payback
                        // giga impact
                        // thunder wave
                        // psych up
                        // dream eater
                        // swagger
                        // drain punch // move tutor
                        // foul play // move tutor
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
                        // sucker punch // gen 4 move tutor
                        PMove.AerialAce, // TM
                        PMove.CalmMind, // TM
                        PMove.ChargeBeam, // TM
                        PMove.Cut, // HM
                        PMove.DarkPulse, // Move tutor
                        PMove.DoubleTeam, // TM
                        PMove.Flash, // TM
                        PMove.FocusBlast, // TM
                        PMove.Frustration, // TM
                        PMove.Headbutt, // Gen 4 Move Tutor
                        PMove.HiddenPower, // TM
                        PMove.IceBeam, // TM
                        PMove.IcyWind, // Move Tutor
                        PMove.MudSlap, // Gen 4 Move Tutor
                        PMove.PoisonJab, // TM
                        PMove.Protect, // TM
                        PMove.Psychic, // TM
                        PMove.RainDance, // TM
                        PMove.Retaliate, // TM
                        PMove.Return, // TM
                        PMove.RockSlide, // TM
                        PMove.RockSmash, // TM
                        PMove.RockTomb, // TM
                        PMove.ShadowBall, // TM
                        PMove.ShadowClaw, // TM
                        PMove.ShockWave, // Gen 4 TM
                        PMove.SludgeBomb, // TM
                        PMove.Snarl, // TM
                        PMove.Strength, // HM
                        PMove.Substitute, // TM
                        PMove.SunnyDay, // TM
                        PMove.Swift, // Gen 4 Move Tutor
                        PMove.SwordsDance, // TM
                        PMove.Thunder, // TM
                        PMove.Thunderbolt, // TM
                        PMove.Toxic, // TM
                        PMove.WillOWisp, // TM
                        PMove.XScissor, // TM
                    }
                }
            },
            {
                PSpecies.Cofagrigus,
                new PPokemonData
                {
                    HP = 58, Attack = 50, Defense = 145, SpAttack = 95, SpDefense = 105, Speed = 30,
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Ghost, Type2 = PType.Ghost,
                    Abilities = new PAbility[] { PAbility.Mummy },
                    MinLevel = 34, // Evolve Yamask
                    ShinyLocked = false,
                    Weight = 76.5,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.Astonish),
                        Tuple.Create(1, PMove.Protect),
                        // 1 disable
                        // 1 haze
                        // 5 disable
                        // 9 haze
                        // 13 night shade
                        // 17 hex                        
                        Tuple.Create(21, PMove.WillOWisp),
                        // 25 ominous wind
                        // 29 curse
                        // 33 power split
                        // 33 guard split
                        Tuple.Create(34, PMove.ScaryFace),
                        Tuple.Create(37, PMove.ShadowBall), // As Yamask
                        Tuple.Create(39, PMove.ShadowBall),
                        // 41 grudge as yamask
                        // 45 grudge
                        // 45 mean look as yamask
                        // 51 mean look
                        // 49 destiny bond as yamask
                        // 57 destiny bond
                    },
                    OtherMoves = new PMove[]
                    {
                        // hyper beam // tm
                        // telekinesis
                        // safeguard
                        // facade
                        // rest
                        // attract
                        // thief
                        // round
                        // embargo
                        // payback
                        // giga impact
                        // psych up
                        // dream eater
                        // swagger
                        // trick room
                        // disable // egg move
                        // endure
                        // heal block
                        // imprison
                        // memento
                        // nightmare
                        // after you // move tutor
                        // block
                        // knock off
                        // magic coat
                        // pain split
                        // role play
                        // skill swap
                        // sleep talk
                        // snatch
                        // snore
                        // spite
                        // trick
                        // wonder room
                        PMove.CalmMind, // TM
                        PMove.DarkPulse, // Move Tutor
                        PMove.DoubleTeam, // TM
                        PMove.EnergyBall, // TM
                        PMove.FakeTears, // Egg Move
                        PMove.Flash, // TM
                        PMove.Frustration, // TM
                        PMove.GrassKnot, // TM
                        PMove.HiddenPower, // TM
                        PMove.IronDefense, // Move Tutor
                        PMove.NastyPlot, // Egg Move
                        PMove.Protect, // TM
                        PMove.Psychic, // TM
                        PMove.RainDance, // TM
                        PMove.Return, // TM
                        PMove.ShadowBall, // TM
                        PMove.Substitute, // TM
                        PMove.Toxic, // TM
                        PMove.WillOWisp, // TM
                    }
                }
            },
            {
                PSpecies.Genesect,
                new PPokemonData
                {
                    HP = 71, Attack = 120, Defense = 95, SpAttack = 120, SpDefense = 95, Speed = 99,
                    GenderRatio = PGender.Genderless,
                    Type1 = PType.Bug, Type2 = PType.Steel,
                    Abilities = new PAbility[] { PAbility.Download },
                    MinLevel = 15, // Event (Plasma Genesect)
                    ShinyLocked = false,
                    Weight = 82.5,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.MetalClaw),
                        Tuple.Create(1, PMove.QuickAttack),
                        Tuple.Create(1, PMove.Screech),
                        // 1 magnet rise
                        // 1 techno blast
                        // 7 fury cutter
                        // 11 lock-on
                        Tuple.Create(15, PMove.MagnetBomb), // Event (Plasma Genesect)
                        Tuple.Create(15, PMove.SignalBeam), // Event (Plasma Genesect)
                        Tuple.Create(18, PMove.FlameCharge),
                        Tuple.Create(22, PMove.MagnetBomb),
                        Tuple.Create(29, PMove.Slash),
                        Tuple.Create(33, PMove.MetalSound),
                        Tuple.Create(40, PMove.SignalBeam),
                        // 44 tri attack
                        Tuple.Create(51, PMove.XScissor),
                        Tuple.Create(55, PMove.BugBuzz),
                        // 62 simple beam
                        Tuple.Create(66, PMove.ZapCannon),
                        // 73 hyper beam
                        // 77 self-destruct
                        Tuple.Create(100, PMove.ExtremeSpeed), // Event (Cinema Genesect)
                        Tuple.Create(100, PMove.BlazeKick), // Event (Cinema Genesect)
                        Tuple.Create(100, PMove.ShiftGear), // Event (Cinema Genesect)
                    },
                    OtherMoves = new PMove[]
                    {
                        // fly // hm
                        // blizzard // tm
                        // hyper beam
                        // solar beam
                        // facade
                        // rest
                        // round
                        // explosion
                        // giga impact
                        // swagger
                        // u-turn
                        // bug bite // tutor
                        // giga drain
                        // gravity
                        // last resort
                        // magic coat
                        // magnet rise
                        // recycle
                        // sleep talk
                        // snore
                        PMove.AerialAce, // TM
                        PMove.ChargeBeam, // TM
                        PMove.DarkPulse, // Move Tutor
                        PMove.DoubleTeam, // TM
                        PMove.Electroweb, // Move Tutor
                        PMove.EnergyBall, // TM
                        PMove.FlameCharge, // TM
                        PMove.Flamethrower, // TM
                        PMove.Flash, // TM
                        PMove.FlashCannon, // TM
                        PMove.Frustration, // TM
                        PMove.GunkShot, // Move Tutor
                        PMove.HiddenPower, // TM
                        PMove.HoneClaws, // TM
                        PMove.IceBeam, // TM
                        PMove.IronDefense, // Move Tutor
                        PMove.IronHead, // Move Tutor
                        PMove.LightScreen, // TM
                        PMove.Protect, // TM
                        PMove.Psychic, // TM
                        PMove.Reflect, // TM
                        PMove.Return, // TM
                        PMove.RockPolish, // TM
                        PMove.ShadowClaw, // TM
                        PMove.SignalBeam, // Move Tutor
                        PMove.StruggleBug, // TM
                        PMove.Substitute, // TM
                        PMove.Thunder, // TM
                        PMove.Thunderbolt, // TM
                        PMove.ThunderWave, // TM
                        PMove.Toxic, // TM
                        PMove.XScissor, // TM
                        PMove.ZenHeadbutt, // Move Tutor
                    }
                }
            },
        };

        // Formes
        static PPokemonData()
        {
            Data.Add(PSpecies.Unown_B, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_C, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_D, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_E, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_F, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_G, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_H, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_I, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_J, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_K, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_L, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_M, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_N, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_O, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_P, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_Q, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_R, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_S, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_T, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_U, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_V, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_W, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_X, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_Y, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_Z, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_Exclamation, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());
            Data.Add(PSpecies.Unown_Question, (PPokemonData)Data[PSpecies.Unown_A].MemberwiseClone());

            Data.Add(PSpecies.Genesect_Burn, (PPokemonData)Data[PSpecies.Genesect].MemberwiseClone());
            Data.Add(PSpecies.Genesect_Chill, (PPokemonData)Data[PSpecies.Genesect].MemberwiseClone());
            Data.Add(PSpecies.Genesect_Douse, (PPokemonData)Data[PSpecies.Genesect].MemberwiseClone());
            Data.Add(PSpecies.Genesect_Shock, (PPokemonData)Data[PSpecies.Genesect].MemberwiseClone());
        }
    }
}
