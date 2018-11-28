using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PPokemonData
    {
        public byte HP, Attack, Defense, SpAttack, SpDefense, Speed;
        public PGenderRatio GenderRatio;
        public PType Type1, Type2;
        public PAbility[] Abilities;
        public byte MinLevel;
        public bool ShinyLocked;
        public double Weight; // Kilograms
        public Tuple<PMove, int, PMoveObtainMethod>[] LevelUpMoves;
        public Tuple<PMove, PMoveObtainMethod>[] OtherMoves;

        public bool HasAbility(PAbility ability) => Abilities.Contains(ability);
        public bool HasType(PType type) => Type1 == type || Type2 == type;

        // First is attacker, second is defender
        // Cast PType to an int for the indices
        // [1,2] = bug attacker, dark defender
        public static readonly double[,] TypeEffectiveness = new double[,]
        {
            // Defender
            //   None      Bug     Dark   Dragon Electric Fighting     Fire   Flying    Ghost    Grass   Ground      Ice   Normal   Poison  Psychic     Rock    Steel    Water
            {     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0}, // None
            {     1.0,     1.0,     2.0,     1.0,     1.0,     0.5,     0.5,     0.5,     0.5,     2.0,     1.0,     1.0,     1.0,     0.5,     2.0,     1.0,     0.5,     1.0}, // Bug
            {     1.0,     1.0,     0.5,     1.0,     1.0,     0.5,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     0.5,     1.0}, // Dark
            {     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     1.0}, // Dragon
            {     1.0,     1.0,     1.0,     0.5,     0.5,     1.0,     1.0,     2.0,     1.0,     0.5,     0.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0}, // Electric
            {     1.0,     0.5,     2.0,     1.0,     1.0,     1.0,     1.0,     0.5,     0.0,     1.0,     1.0,     2.0,     2.0,     0.5,     0.5,     2.0,     2.0,     1.0}, // Fighting
            {     1.0,     2.0,     1.0,     0.5,     1.0,     1.0,     0.5,     1.0,     1.0,     2.0,     1.0,     2.0,     1.0,     1.0,     1.0,     0.5,     2.0,     0.5}, // Fire
            {     1.0,     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     0.5,     1.0}, // Flying
            {     1.0,     1.0,     0.5,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     0.0,     1.0,     2.0,     1.0,     0.5,     1.0}, // Ghost
            {     1.0,     0.5,     1.0,     0.5,     1.0,     1.0,     0.5,     0.5,     1.0,     0.5,     2.0,     1.0,     1.0,     0.5,     1.0,     2.0,     0.5,     2.0}, // Grass
            {     1.0,     0.5,     1.0,     1.0,     2.0,     1.0,     2.0,     0.0,     1.0,     0.5,     1.0,     1.0,     1.0,     2.0,     1.0,     2.0,     2.0,     1.0}, // Ground
            {     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     2.0,     2.0,     0.5,     1.0,     1.0,     1.0,     1.0,     0.5,     0.5}, // Ice
            {     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     0.5,     1.0}, // Normal
            {     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     2.0,     0.5,     1.0,     1.0,     0.5,     1.0,     0.5,     0.0,     1.0}, // Poison
            {     1.0,     1.0,     0.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0,     0.5,     1.0,     0.5,     1.0}, // Psychic
            {     1.0,     2.0,     1.0,     1.0,     1.0,     0.5,     2.0,     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     1.0,     1.0,     1.0,     0.5,     1.0}, // Rock
            {     1.0,     1.0,     1.0,     1.0,     0.5,     1.0,     0.5,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     2.0,     0.5,     0.5}, // Steel
            {     1.0,     1.0,     1.0,     0.5,     1.0,     1.0,     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     0.5}, // Water
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
                    GenderRatio = PGenderRatio.M1_F1,
                    Type1 = PType.Electric, Type2 = PType.None,
                    Abilities = new PAbility[] { PAbility.Static, PAbility.LightningRod },
                    MinLevel = 1, // Egg
                    ShinyLocked = false,
                    Weight = 6.0,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.Agility, 33, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.Agility, 34, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS),
                        Tuple.Create(PMove.Agility, 37, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Discharge, 37, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS),
                        Tuple.Create(PMove.Discharge, 42, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.DoubleTeam, 15, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.DoubleTeam, 18, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS),
                        Tuple.Create(PMove.DoubleTeam, 21, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // electro ball (18 gen 5)
                        // feint (29 gen 4, 34 gen 5)
                        Tuple.Create(PMove.Growl, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.LightScreen, 42, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS),
                        Tuple.Create(PMove.LightScreen, 45, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.LightScreen, 50, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.QuickAttack, 11, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.QuickAttack, 13, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Slam, 20, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.Slam, 21, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS),
                        Tuple.Create(PMove.Slam, 26, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.TailWhip, 5, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.TailWhip, 6, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.Thunder, 41, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.Thunder, 45, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS),
                        Tuple.Create(PMove.Thunder, 50, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Thunderbolt, 26, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS),
                        Tuple.Create(PMove.Thunderbolt, 29, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.ThunderShock, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.ThunderWave, 8, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.ThunderWave, 10, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2)

                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {
                        // attract (gen 3 tm, gen 4 tm, gen 5 tm)
                        // bestow (gen 5 egg)
                        // bide (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.BodySlam, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD),
                        Tuple.Create(PMove.BrickBreak, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // captivate (gen 4 tm)
                        // charge (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.ChargeBeam, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // counter (frlg/e tutor)
                        // covet (b2w2 tutor)
                        // defense curl (emerald tutor)
                        Tuple.Create(PMove.Dig, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // double edge (frlg/e/xd tutor)
                        // doubleslap (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.DoubleTeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // dynamic punch (emerald tutor)
                        // echoed voice (gen 5 tm)
                        // encore (gen 3 egg, gen 4 egg, gen 5 egg)
                        // endure (emerald tutor, gen 4 tm, gen 5 egg)
                        // facade (gen 3 tm, gen 4 tm, gen 5 tm)
                        // fake out (gen 4 egg, gen 5 egg)
                        // flail (hgss egg, gen 5 egg)
                        Tuple.Create(PMove.Flash, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // fling (gen 4 tm, gen 5 tm)
                        // focus punch (gen 3 tm, gen 4 tm)
                        Tuple.Create(PMove.Frustration, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.GrassKnot, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Headbutt, PMoveObtainMethod.MoveTutor_HGSS),
                        // helping hand (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.HiddenPower, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.IronTail, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // knock off (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.LightScreen, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // lucky chant (gen 5 egg)
                        // magnet rise (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.MegaKick, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E),
                        Tuple.Create(PMove.MegaPunch, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E),
                        // mimic (frlg/e/xd tutor)
                        Tuple.Create(PMove.MudSlap, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // natural gift (gen 4 tm)
                        // present (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.Protect, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RainDance, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // rest (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Return, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // reversal (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.RockSmash, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.TM_BWB2W2),
                        // rollout (emerald tutor, pt/hgss tutor)
                        // round (gen 5 tm)
                        // secret power (gen 3 tm, gen 4 tm)
                        // seismic toss (frlg/e/xd tutor)
                        Tuple.Create(PMove.ShockWave, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS),
                        Tuple.Create(PMove.SignalBeam, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // sleep talk (emerald tutor, gen 4 tm, b2w2 tutor)
                        // snore (emerald tutor, pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.Strength, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.Substitute, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // swagger (emerald tutor, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Swift, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        Tuple.Create(PMove.Thunder, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Thunderbolt, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ThunderPunch, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2 | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.ThunderWave, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Tickle, PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.Toxic, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // volt switch (gen 5 tm)
                        // wild charge (gen 5 tm)
                        // wish (gen 3 egg, gen 4 egg, gen 5 egg)
                    }
                }
            },
            {
                PSpecies.Cubone,
                new PPokemonData
                {
                    HP = 50, Attack = 50, Defense = 95, SpAttack = 40, SpDefense = 50, Speed = 35,
                    GenderRatio = PGenderRatio.M1_F1,
                    Type1 = PType.Ground, Type2 = PType.None,
                    Abilities = new PAbility[] { PAbility.RockHead, PAbility.LightningRod, PAbility.BattleArmor },
                    MinLevel = 1, // Egg
                    ShinyLocked = false,
                    Weight = 6.5,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.BoneClub, 7, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.BoneClub, 9, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        // bonemerang (25 gen 3, 21 gen 4 & 5)
                        // bone rush (41 gen 3, 37 gen 4 & 5)
                        // double edge (45 gen 3, 43 gen 4 & 5)
                        // endeavor (41 gen 4 & 5)
                        // false swipe (33 gen 3, 27 gen 4 & 5)
                        // fling (33 gen 4 & 5)
                        Tuple.Create(PMove.FocusEnergy, 17, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.FocusEnergy, 21, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.Growl, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Headbutt, 11, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Headbutt, 13, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.Leer, 13, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Leer, 17, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        // rage (29 gen 3, 23 gen 4 & 5)
                        Tuple.Create(PMove.Retaliate, 47, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.TailWhip, 3, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.TailWhip, 5, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        // thrash (37 gen 3, 31 gen 4 & 5)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.AerialAce, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.AncientPower, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        // attract (gen 4 tm, gen 5 tm)
                        // belly drum (gen 3 egg, gen 4 egg, gen 5 egg)
                        // blizzard (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.BodySlam, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD),
                        Tuple.Create(PMove.BrickBreak, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Bulldoze, PMoveObtainMethod.TM_BWB2W2),
                        // captivate (gen 4 tm)
                        // chip away (gen 5 egg)
                        // counter (frlg/e tutor)
                        Tuple.Create(PMove.Detect, PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.Dig, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // double edge (frlg/e/xd tutor)
                        // double kick (gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.DoubleTeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // dynamicpunch (emerald tutor)
                        Tuple.Create(PMove.EarthPower, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.Earthquake, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // echoed voice (gen 5 tm)
                        // endeavor (pt/hgss tutor, b2w2 tutor)
                        // endure (emerald tutor, gen 4 tm, gen 5 egg)
                        // facade (gen 3 tm, gen 4 tm, gen 5 tm)
                        // false swipe (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.FireBlast, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.FirePunch, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.Flamethrower, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // fling (gen 4 tm, gen 5 tm)
                        // focus punch (gen 3 tm, gen 4 tm)
                        Tuple.Create(PMove.Frustration, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // fury cutter (pt/hgss tutor)
                        Tuple.Create(PMove.Headbutt, PMoveObtainMethod.MoveTutor_HGSS),
                        Tuple.Create(PMove.HiddenPower, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.IceBeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.IcyWind, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // incinerate (gen 5 tm)
                        Tuple.Create(PMove.IronDefense, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.IronHead, PMoveObtainMethod.MoveTutor_B2W2 | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.IronTail, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // knock off (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.LowKick, PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.MegaKick, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E),
                        Tuple.Create(PMove.MegaPunch, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E),
                        // mimic (frlg/e/xd tutor)
                        Tuple.Create(PMove.MudSlap, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // natural gift (gen 4 tm)
                        // perish song (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.Protect, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // rest (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Retaliate, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Return, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RockClimb, PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS),
                        Tuple.Create(PMove.RockSlide, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2 | PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS),
                        Tuple.Create(PMove.RockSmash, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RockTomb, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // round (gen 5 tm)
                        // sandstorm (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Screech, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        // secret power (gen 3 tm, gen 4 tm)
                        // seismic toss (frlg/e/xd tutor)
                        // skull bash (gen 3 egg, gen 4 egg, gen 5 egg)
                        // sleep talk (emerald tutor, gen 4 tm, b2w2 tutor)
                        // smack down (gen 5 tm)
                        // snore (emerald tutor, pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.StealthRock, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.Strength, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.Substitute, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.SunnyDay, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // swagger (e/xd tutor, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.SwordsDance, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2 | PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS),
                        // thief (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.ThunderPunch, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.Toxic, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // uproar (pt/hgss tutor, b2w2 tutor)
                    }
                }
            },
            {
                PSpecies.Marowak,
                new PPokemonData
                {
                    HP = 60, Attack = 80, Defense = 110, SpAttack = 50, SpDefense = 80, Speed = 45,
                    GenderRatio = PGenderRatio.M1_F1,
                    Type1 = PType.Ground, Type2 = PType.None,
                    Abilities = new PAbility[] { PAbility.RockHead, PAbility.LightningRod, PAbility.BattleArmor },
                    MinLevel = 14, // HGSS (Rock Tunnel)
                    ShinyLocked = false,
                    Weight = 45.0,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.BoneClub, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.BoneClub, 7, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.BoneClub, 9, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        // bonemerang (25 gen 3, 21 gen 4 & 5)
                        // bone rush (53 gen 3, 43 gen 4 & 5)
                        // double edge (61 gen 3, 53 gen 4 & 5)
                        // endeavor (49 gen 4 & 5)
                        // false swipe (39 gen 3, 27 gen 4 & 5)
                        // fling (37 gen 4 & 5)
                        Tuple.Create(PMove.FocusEnergy, 17, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.FocusEnergy, 21, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.Growl, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Headbutt, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Headbutt, 11, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Headbutt, 13, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.Leer, 13, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Leer, 17, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        // rage (32 gen 3, 23 gen 4 & 5)
                        Tuple.Create(PMove.Retaliate, 59, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.TailWhip, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.TailWhip, 3, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.TailWhip, 5, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        // thrash (46 gen 3, 33 gen 4 & 5)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.AerialAce, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.AncientPower, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        // attract (gen 4 tm, gen 5 tm)
                        // belly drum (gen 3 egg, gen 4 egg, gen 5 egg)
                        // blizzard (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.BodySlam, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD),
                        Tuple.Create(PMove.BrickBreak, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Bulldoze, PMoveObtainMethod.TM_BWB2W2),
                        // captivate (gen 4 tm)
                        // chip away (gen 5 egg)
                        // counter (frlg/e tutor)
                        Tuple.Create(PMove.Detect, PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.Dig, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // double edge (frlg/e/xd tutor)
                        // double kick (gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.DoubleTeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // dynamicpunch (emerald tutor)
                        Tuple.Create(PMove.EarthPower, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.Earthquake, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // echoed voice (gen 5 tm)
                        // endeavor (pt/hgss tutor, b2w2 tutor)
                        // endure (emerald tutor, gen 4 tm, gen 5 egg)
                        // facade (gen 3 tm, gen 4 tm, gen 5 tm)
                        // false swipe (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.FireBlast, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.FirePunch, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.Flamethrower, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // fling (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.FocusBlast, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // focus punch (gen 3 tm, gen 4 tm)
                        Tuple.Create(PMove.Frustration, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // fury cutter (pt/hgss tutor)
                        // giga impact (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Headbutt, PMoveObtainMethod.MoveTutor_HGSS),
                        Tuple.Create(PMove.HiddenPower, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // hyper beam (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.IceBeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.IcyWind, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // incinerate (gen 5 tm)
                        Tuple.Create(PMove.IronDefense, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.IronHead, PMoveObtainMethod.MoveTutor_B2W2 | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.IronTail, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // knock off (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.LowKick, PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.MegaKick, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E),
                        Tuple.Create(PMove.MegaPunch, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E),
                        // mimic (frlg/e/xd tutor)
                        Tuple.Create(PMove.MudSlap, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // natural gift (gen 4 tm)
                        // outrage (pt/hgss tutor, b2w2 tutor)
                        // perish song (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.Protect, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // rest (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Retaliate, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Return, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RockClimb, PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS),
                        Tuple.Create(PMove.RockSlide, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2 | PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS),
                        Tuple.Create(PMove.RockSmash, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RockTomb, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // round (gen 5 tm)
                        // sandstorm (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Screech, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        // secret power (gen 3 tm, gen 4 tm)
                        // seismic toss (frlg/e/xd tutor)
                        // skull bash (gen 3 egg, gen 4 egg, gen 5 egg)
                        // sleep talk (emerald tutor, gen 4 tm, b2w2 tutor)
                        // smack down (gen 5 tm)
                        // snore (emerald tutor, pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.StealthRock, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.StoneEdge, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Strength, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.Substitute, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.SunnyDay, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // swagger (e/xd tutor, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.SwordsDance, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2 | PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS),
                        // thief (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.ThunderPunch, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.Toxic, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // uproar (pt/hgss tutor, b2w2 tutor)
                    }
                }
            },
            {
                PSpecies.Ditto,
                new PPokemonData
                {
                    HP = 48, Attack = 48, Defense = 48, SpAttack = 48, SpDefense = 48, Speed = 48,
                    GenderRatio = PGenderRatio.M0_F0,
                    Type1 = PType.Normal, Type2 = PType.None,
                    Abilities = new PAbility[] { PAbility.Limber, PAbility.Imposter },
                    MinLevel = 10, // HGSS (Route 34, Route 35)
                    ShinyLocked = false,
                    Weight = 4.0,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.Transform, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {

                    }
                }
            },
            {
                PSpecies.Crobat,
                new PPokemonData
                {
                    HP = 85, Attack = 90, Defense = 80, SpAttack = 70, SpDefense = 80, Speed = 130,
                    GenderRatio = PGenderRatio.M1_F1,
                    Type1 = PType.Poison, Type2 = PType.Flying,
                    Abilities = new PAbility[] { PAbility.InnerFocus, PAbility.Infiltrator },
                    MinLevel = 11, // Evolve MinLevel Golbat (DPPt Oreburgh Gate level 10)
                    ShinyLocked = false,
                    Weight = 75.0,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        // acrobatics (bw 39, b2w2 33)
                        Tuple.Create(PMove.AirCutter, 27, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.AirCutter, 28, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.AirCutter, 35, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.AirSlash, 51, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS),
                        Tuple.Create(PMove.AirSlash, 52, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.AirSlash, 57, PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.Astonish, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Astonish, 6, PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.Astonish, 8, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Astonish, 9, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.Astonish, 11, PMoveObtainMethod.LevelUp_RSE),
                        Tuple.Create(PMove.Bite, 12, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Bite, 13, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.Bite, 16, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.ConfuseRay, 19, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.ConfuseRay, 21, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.ConfuseRay, 28, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.CrossPoison, 1, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // haze (56 gen 3, 45 gen 4, 51 bw, 47 b2w2)
                        // leech life (1 gen 3 & 4 & 5)
                        // mean look (42 gen 3, 33 gen 4, 33 bw, 38 b2w2)
                        Tuple.Create(PMove.PoisonFang, 39, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS),
                        Tuple.Create(PMove.PoisonFang, 42, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.PoisonFang, 45, PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.PoisonFang, 49, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.Screech, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Supersonic, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Supersonic, 4, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Supersonic, 5, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.Supersonic, 6, PMoveObtainMethod.LevelUp_RSE),
                        Tuple.Create(PMove.Supersonic, 11, PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.Swift, 24, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.WingAttack, 15, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.WingAttack, 17, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.WingAttack, 21, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {
                        // acrobatics (gen 5 tm)
                        Tuple.Create(PMove.AerialAce, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.AirCutter, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // attract (gen 3 tm, gen 4 tm, gen 5 tm)
                        // brave bird (gen 4 egg, gen 5 egg)
                        // captivate (gen 4 tm)
                        Tuple.Create(PMove.Curse, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.DarkPulse, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // defog (dppt hm, gen 5 egg)
                        // double edge (frlg/e/xd tutor)
                        Tuple.Create(PMove.DoubleTeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // endure (emerald tutor, gen 4 tm)
                        // facade (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.FaintAttack, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.Fly, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.Frustration, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // giga drain (gen 3 tm, gen 4 tm, gen 5 egg, b2w2 tutor)
                        // giga impact (gen 4 tm, gen 5 tm)
                        // gust (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.HeatWave, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.HiddenPower, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // hyper beam (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Hypnosis, PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        // mimic (frlg/e/xd tutor)
                        Tuple.Create(PMove.NastyPlot, PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        // natural gift (gen 4 tm)
                        Tuple.Create(PMove.OminousWind, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // payback (gen 4 tm, gen 5 tm)
                        // pluck (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Protect, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // pursuit (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.QuickAttack, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.RainDance, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // rest (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Return, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // roost (gen 4 tm, gen 4 egg, b2w2 tutor)
                        // round (gen 5 tm)
                        // safeguard (gen 3 tm)
                        // secret power (gen 3 tm, gen 4 tm)
                        Tuple.Create(PMove.ShadowBall, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // sky attack (hgss tutor, b2w2 tutor)
                        // sleep talk (emerald tutor, gen 4 tm, b2w2 tutor)
                        Tuple.Create(PMove.SludgeBomb, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // snatch (gen 3 tm, gen 4 tm, b2w2 tutor)
                        // snore (emerald tutor, pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.SteelWing, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.Substitute, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.SunnyDay, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // super fang (hgss tutor, b2w2 tutor)
                        // swagger (e/xd tutor, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Swift, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // tailwind (hgss tutor, b2w2 tutor)
                        // taunt (gen 3 tm, gen 4 tm, gen 5 tm)
                        // thief (gen 3 tm, gen 4 tm, gen 5 tm)
                        // torment (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Toxic, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // twister (pt/hgss tutor)
                        // uproar (pt/hgss tutor, b2w2 tutor)
                        // u-turn (gen 4 tm, gen 5 tm)
                        // venoshock (gen 5 tm)
                        // whirlwind (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.XScissor, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ZenHeadbutt, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2 | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2)
                    }
                }
            },
            {
                PSpecies.Pichu,
                new PPokemonData
                {
                    HP = 20, Attack = 40, Defense = 15, SpAttack = 35, SpDefense = 35, Speed = 60,
                    GenderRatio = PGenderRatio.M1_F1,
                    Type1 = PType.Electric, Type2 = PType.None,
                    Abilities = new PAbility[] { PAbility.Static, PAbility.LightningRod },
                    MinLevel = 1, // Egg
                    ShinyLocked = false,
                    Weight = 2.0,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.Charm, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.NastyPlot, 18, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.SweetKiss, 11, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.SweetKiss, 13, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.TailWhip, 5, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.TailWhip, 6, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.ThunderShock, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.ThunderWave, 8, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.ThunderWave, 10, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {
                        // attract (gen 3 tm, gen 4 tm, gen 5 tm)
                        // bestow (gen 5 egg)
                        // bide (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.BodySlam, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD),
                        // captivate (gen 4 tm)
                        // charge (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.ChargeBeam, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // counter (frlg/e tutor)
                        // covet (b2w2 tutor)
                        // defense curl (emerald tutor)
                        // double edge (frlg/e/xd tutor)
                        // double slap (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.DoubleTeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // echoed voice (gen 5 tm)
                        // encore (gen 3 egg, gen 4 egg, gen 5 egg)
                        // endure (emerald tutor, gen 4 tm, gen 5 egg)
                        // facade (gen 3 tm, gen 4 tm, gen 5 tm)
                        // fake out (gen 4 egg, gen 5 egg)
                        // flail (hgss egg, gen 5 egg)
                        Tuple.Create(PMove.Flash, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // fling (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Frustration, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.GrassKnot, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Headbutt, PMoveObtainMethod.MoveTutor_HGSS),
                        // helping hand (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.HiddenPower, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.IronTail, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.LightScreen, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // lucky chant (gen 5 egg)
                        // magnet rise (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.MegaKick, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E),
                        Tuple.Create(PMove.MegaPunch, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E),
                        // mimic (frlg/e/xd tutor)
                        Tuple.Create(PMove.MudSlap, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // natural gift (gen 4 tm)
                        // present (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.Protect, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RainDance, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // rest (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Return, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // reversal (gen 3 egg, gen 4 egg, gen 5 egg)
                        // rollout (emerald tutor, pt/hgss tutor)
                        // round (gen 5 tm)
                        // secret power (gen 3 tm, gen 4 tm)
                        // seismic toss (frlg/e/xd tutor)
                        Tuple.Create(PMove.ShockWave, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS),
                        Tuple.Create(PMove.SignalBeam, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // sleep talk (emerald tutor, gen 4 tm, b2w2 tutor)
                        // snore (emerald tutor, pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.Substitute, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // swagger (e/xd tutor, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Swift, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        Tuple.Create(PMove.Thunder, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Thunderbolt, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ThunderPunch, PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.ThunderWave, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Tickle, PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.Toxic, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // uproar (pt/hgss tutor, b2w2 tutor)
                        // volt switch (gen 5 tm)
                        // volt tackle (emerald egg, gen 4 egg, gen 5 egg)
                        // wild charge (gen 5 tm)
                        // wish (gen 3 egg, gen 4 egg, gen 5 egg)
                    }
                }
            },
            {
                PSpecies.Azumarill,
                new PPokemonData
                {
                    HP = 100, Attack = 50, Defense = 80, SpAttack = 50, SpDefense = 80, Speed = 50,
                    GenderRatio = PGenderRatio.M1_F1,
                    Type1 = PType.Water, Type2 = PType.None,
                    Abilities = new PAbility[] { PAbility.ThickFat, PAbility.HugePower, PAbility.SapSipper },
                    MinLevel = 5, // B2W2 (Route 20, Flocessy Ranch, Relic Passage)
                    ShinyLocked = false,
                    Weight = 28.5,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        // aqua ring (27 gen 4 & bw, 31 b2w2)
                        Tuple.Create(PMove.AquaTail, 21, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.AquaTail, 47, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.Bubble, 1, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.BubbleBeam, 13, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.BubbleBeam, 20, PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.BubbleBeam, 24, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS),
                        // defense curl (1 & 3 gen 3, 1 & 2 gen 4, 1 & 2 bw, 10 b2w2)
                        // double edge (34 gen 3, 33 gen 4 & bw, 25 b2w2)
                        // helping hand (16 b2w2)
                        Tuple.Create(PMove.HydroPump, 46, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.HydroPump, 54, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.HydroPump, 57, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.RainDance, 35, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.RainDance, 40, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.RainDance, 45, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        // rollout (15 gen 3 & gen 4 & bw, 10 b2w2)
                        Tuple.Create(PMove.Superpower, 42, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Tackle, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.TailWhip, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.TailWhip, 2, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.TailWhip, 6, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.TailWhip, 7, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.WaterGun, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.WaterGun, 7, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.WaterGun, 10, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        // water sport (1 & 5 b2w2)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.Amnesia, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.AquaJet, PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.AquaTail, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // attract (gen 3 tm, gen 4 tm, gen 5 tm)
                        // belly drum (gen 3 egg, gen 4 egg, gen 5 egg)
                        // blizzard (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.BodySlam, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        // bounce (b2w2 tutor)
                        Tuple.Create(PMove.BrickBreak, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Bulldoze, PMoveObtainMethod.TM_BWB2W2),
                        // captivate (gen 4 tm)
                        // covet (b2w2 tutor)
                        // defense curl (emerald tutor)
                        Tuple.Create(PMove.Dig, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Dive, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        // double edge (frlg/e/xd tutor)
                        Tuple.Create(PMove.DoubleTeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // dynamicpunch (emerald tutor)
                        // encore (gen 3 egg, gen 4 egg, gen 5 egg)
                        // endure (emerald tutor, gen 4 tm)
                        // facade (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.FakeTears, PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        // fling (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.FocusBlast, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // focus punch (gen 3 tm, gen 4 tm)
                        Tuple.Create(PMove.Frustration, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // future sight (gen 3 egg, gen 4 egg, gen 5 egg)
                        // giga impact (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.GrassKnot, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // hail (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Headbutt, PMoveObtainMethod.MoveTutor_HGSS),
                        // helping hand (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.HiddenPower, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // hyper beam (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.HyperVoice, PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.IceBeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.IcePunch, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.IcyWind, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.IronTail, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // knock off (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.LightScreen, PMoveObtainMethod.TM_BWB2W2 | PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS),
                        Tuple.Create(PMove.MegaKick, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E),
                        Tuple.Create(PMove.MegaPunch, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E),
                        // mimic (frlg/e/xd tutor)
                        Tuple.Create(PMove.MuddyWater, PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.MudSlap, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // natural gift (gen 4 tm)
                        // perish song (gen 3 egg, gen 4 egg, gen 5 egg)
                        // present (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.Protect, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RainDance, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // refresh (gen 3 egg, gen 4 egg, gen 5 egg)
                        // rest (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Return, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RockSmash, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.TM_BWB2W2),
                        // rollout (emerald tutor, pt/hgss tutor)
                        // round (gen 5 tm)
                        Tuple.Create(PMove.Scald, PMoveObtainMethod.TM_BWB2W2),
                        // secret power (gen 3 tm, gen 4 tm)
                        // seismic toss (frlg/e tutor)
                        Tuple.Create(PMove.Sing, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.Slam, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        // sleep talk (emerald tutor, gen 4 tm, b2w2 tutor)
                        // snore (emerald tutor, pt/hgss tutor, b2w2 tutor)
                        // soak (gen 5 egg)
                        Tuple.Create(PMove.Strength, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.Substitute, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2 | PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS),
                        Tuple.Create(PMove.Superpower, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2 | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.Supersonic, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.Surf, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        // swagger (e/xd tutor, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Swift, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        Tuple.Create(PMove.Tickle, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.Toxic, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Waterfall, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.WaterPulse, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS),
                        // water sport (gen 5 egg)
                        // whirlpool (hgss hm)
                        Tuple.Create(PMove.Workup, PMoveObtainMethod.TM_BWB2W2)
                    }
                }
            },
            {
                PSpecies.Unown_A,
                new PPokemonData
                {
                    HP = 48, Attack = 72, Defense = 48, SpAttack = 72, SpDefense = 48, Speed = 48,
                    GenderRatio = PGenderRatio.M0_F0,
                    Type1 = PType.Psychic, Type2 = PType.None,
                    Abilities = new PAbility[] { PAbility.Levitate },
                    MinLevel = 5, // HGSS (Ruins of Alph)
                    ShinyLocked = false,
                    Weight = 5.0,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.HiddenPower, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {

                    }
                }
            },
            {
                PSpecies.Absol,
                new PPokemonData
                {
                    HP = 65, Attack = 130, Defense = 60, SpAttack = 75, SpDefense = 60, Speed = 75,
                    GenderRatio = PGenderRatio.M1_F1,
                    Type1 = PType.Dark, Type2 = PType.None,
                    Abilities = new PAbility[] { PAbility.Pressure, PAbility.SuperLuck, PAbility.Justified },
                    MinLevel = 1, // Egg
                    ShinyLocked = false,
                    Weight = 47.0,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.Bite, 20, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Bite, 21, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.Bite, 28, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.Detect, 44, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Detect, 49, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.DoubleTeam, 25, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.DoubleTeam, 31, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.DoubleTeam, 33, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        // feint (1 dppt/hgss/gen5)
                        // future sight (41 gen 3 & 4, 41 bw, 36 b2w2)
                        Tuple.Create(PMove.Leer, 4, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Leer, 5, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        // me first (52 gen 4, 57 bw, 60 b2w2)
                        Tuple.Create(PMove.NightSlash, 41, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.NightSlash, 52, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        // perish song (46 gen 3, 65 gen 4 & 5)
                        Tuple.Create(PMove.PsychoCut, 49, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.PsychoCut, 60, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        // pursuit (20 gen 4, 20 bw, 12 b2w2)
                        Tuple.Create(PMove.QuickAttack, 9, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.QuickAttack, 12, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.QuickAttack, 13, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        // razor wind (17 gen 3 & 4, 17 bw, 57 b2w2)
                        Tuple.Create(PMove.Scratch, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Slash, 28, PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Slash, 36, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        // sucker punch (44 gen 4, 44 bw, 52 b2w2)
                        Tuple.Create(PMove.SwordsDance, 25, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW),
                        Tuple.Create(PMove.SwordsDance, 26, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.SwordsDance, 33, PMoveObtainMethod.LevelUp_B2W2),
                        // taunt (9 gen 3 & 4, 9 bw, 17 b2w2)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.AerialAce, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // assurance (gen 4 egg, gen 5 egg)
                        // attract (gen 3 tm, gen 4 tm, gen 5 tm)
                        // baton pass (gen 3 egg, gen 4 egg, gen 5 egg)
                        // blizzard (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.BodySlam, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD),
                        // bounce (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.CalmMind, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // captivate (gen 4 tm)
                        Tuple.Create(PMove.ChargeBeam, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // counter (frlg/e tutor)
                        Tuple.Create(PMove.Curse, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.Cut, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.DarkPulse, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // double edge (gen 3 egg, frlg/e/xd tutor, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.DoubleTeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // dream eater (frlg/e/xd tutor, gen 4 tm, gen 5 tm)
                        // echoed voice (gen 5 tm)
                        // endure (emerald tutor, gen 4 tm)
                        // facade (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.FaintAttack, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        // false swipe (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.FireBlast, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Flamethrower, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Flash, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // foul play (b2w2 tutor)
                        Tuple.Create(PMove.Frustration, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // fury cutter (emerald tutor, pt/hgss tutor)
                        // giga impact (gen 4 tm, gen 5 tm)
                        // hail (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Headbutt, PMoveObtainMethod.MoveTutor_HGSS),
                        // hex (gen 5 egg)
                        Tuple.Create(PMove.HiddenPower, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.HoneClaws, PMoveObtainMethod.TM_BWB2W2),
                        // hyper beam (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.IceBeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.IcyWind, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // incinerate (gen 5 tm)
                        Tuple.Create(PMove.IronTail, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // knock off (pt/hgss tutor, b2w2 tutor)
                        // magic coat (gen 3 egg, gen 4 egg, hgss tutor, gen 5 egg, b2w2 tutor)
                        // mean look (gen 4 egg, gen 5 egg)
                        // me first (gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.Megahorn, PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        // mimic (frlg/e/xd tutor)
                        Tuple.Create(PMove.MudSlap, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // natural gift (gen 4 tm)
                        // nightmare (xd tutor)
                        // payback (gen 4 tm, gen 5 tm)
                        // perish song (gen 5 egg)
                        Tuple.Create(PMove.Protect, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // psych up (emerald tutor, gen 4 tm, gen 5 tm)
                        // punishment (gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.RainDance, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // rest (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Retaliate, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Return, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RockSlide, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RockSmash, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RockTomb, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // role play (hgss tutor, b2w2 tutor)
                        // round (gen 5 tm)
                        // sandstorm (gen 3 tm, gen 4 tm, gen 5 tm)
                        // secret power (gen 3 tm, gen 4 tm)
                        Tuple.Create(PMove.ShadowBall, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ShadowClaw, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ShockWave, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS),
                        // sleep talk (emerald tutor, gen 4 tm, b2w2 tutor)
                        Tuple.Create(PMove.Snarl, PMoveObtainMethod.TM_BWB2W2),
                        // snatch (gen 3 tm, gen 4 tm, b2w2 tutor)
                        // snore (emerald tutor, pt/hgss tutor, b2w2 tutor)
                        // spite (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.StoneEdge, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Strength, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.Substitute, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2 | PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS),
                        // sucker punch (gen 4 egg, pt/hgss tutor, gen 5 egg)
                        Tuple.Create(PMove.SunnyDay, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Superpower, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // swagger (emerald/xd tutor, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Swift, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        Tuple.Create(PMove.SwordsDance, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // taunt (gen 3 tm, gen 4 tm, gen 5 tm)
                        // thief (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Thunder, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Thunderbolt, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ThunderWave, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // torment (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Toxic, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.WaterPulse, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS),
                        Tuple.Create(PMove.WillOWisp, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.XScissor, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ZenHeadbutt, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2 | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2)
                    }
                }
            },
            {
                PSpecies.Clamperl,
                new PPokemonData
                {
                    HP = 35, Attack = 64, Defense = 85, SpAttack = 74, SpDefense = 55, Speed = 32,
                    GenderRatio = PGenderRatio.M1_F1,
                    Type1 = PType.Water, Type2 = PType.None,
                    Abilities = new PAbility[] { PAbility.ShellArmor, PAbility.Rattled },
                    MinLevel = 1, // Egg
                    ShinyLocked = false,
                    Weight = 52.5,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        // clamp (1 gen 3 & 4 & 5)
                        Tuple.Create(PMove.IronDefense, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.ShellSmash, 51, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.WaterGun, 1, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // whirlpool (1 gen 3 & 4 & 5)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {
                        // aqua ring (gen 4 egg, gen 5 egg)
                        // attract (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Barrier, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        // blizzard (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.BodySlam, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        // brine (gen 4 tm, gen 5 egg)
                        // captivate (gen 4 tm)
                        Tuple.Create(PMove.ConfuseRay, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.Dive, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        // double edge (frlg/e/xd tutor)
                        Tuple.Create(PMove.DoubleTeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // endure (emerald tutor, gen 4 tm, gen 5 egg)
                        // facade (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Frustration, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // hail (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.HiddenPower, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.IceBeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.IcyWind, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.IronDefense, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // mimic (frlg/e/xd tutor)
                        Tuple.Create(PMove.MuddyWater, PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        // mud sport (gen 3 egg, gen 4 egg, gen 5 egg)
                        Tuple.Create(PMove.Protect, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RainDance, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // refresh (gen 3 egg, gen 4 egg, gen 5 egg)
                        // rest (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Return, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // round (gen 5 tm)
                        Tuple.Create(PMove.Scald, PMoveObtainMethod.TM_BWB2W2),
                        // secret power (gen 3 tm, gen 4 tm)
                        // sleep talk (emerald tutor, gen 4 tm, b2w2 tutor)
                        // snore (emerald tutor, pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.Substitute, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Supersonic, PMoveObtainMethod.EggMove_RSFRLG | PMoveObtainMethod.EggMove_E | PMoveObtainMethod.EggMove_DPPt | PMoveObtainMethod.EggMove_HGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.Surf, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        // swagger (e/xd tutor, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Toxic, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Waterfall, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.WaterPulse, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.EggMove_BWB2W2),
                        // whirlpool (hgss hm)
                    }
                }
            },
            {
                PSpecies.Latias,
                new PPokemonData
                {
                    HP = 80, Attack = 80, Defense = 90, SpAttack = 110, SpDefense = 130, Speed = 110,
                    GenderRatio = PGenderRatio.M0_F1,
                    Type1 = PType.Dragon, Type2 = PType.Psychic,
                    Abilities = new PAbility[] { PAbility.Levitate },
                    MinLevel = 35, // HG (Roaming)
                    ShinyLocked = false,
                    Weight = 40.0,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.Charm, 50, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.Charm, 55, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.DragonBreath, 20, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.DragonPulse, 70, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS),
                        Tuple.Create(PMove.DragonPulse, 80, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // guard split (75 gen 5)
                        // healing wish (60 gen 4, 85 gen 5)
                        // heal pulse (65 gen 5)
                        // helping hand (10 gen 3 & 4 & 5)
                        Tuple.Create(PMove.MistBall, 35, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Psychic, 40, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.Psychic, 60, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Psychic, 65, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS),
                        // psycho shift (50 gen 4 & 5)
                        // psywave (1 gen 3 & 4 & 5)
                        // recover (45 gen 3 & 4 & 5)
                        // reflect type (70 gen 5)
                        // refresh (30 gen 3 & 4 & 5)
                        // safeguard (15 gen 3 & 4 & 5)
                        // water sport (25 gen 3 & 4 & 5)
                        // wish (5 gen 3 & 4 & 5)
                        Tuple.Create(PMove.ZenHeadbutt, 40, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.AerialAce, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // attract (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.BodySlam, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD),
                        Tuple.Create(PMove.Bulldoze, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.CalmMind, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // captivate (gen 4 tm)
                        Tuple.Create(PMove.ChargeBeam, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // covet (b2w2 tutor)
                        Tuple.Create(PMove.Cut, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        // defog (dppt hm)
                        Tuple.Create(PMove.Dive, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        // double edge (frlg/e/xd tutor)
                        Tuple.Create(PMove.DoubleTeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.DracoMeteor, PMoveObtainMethod.MoveTutor_DP | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_BW | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.DragonClaw, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.DragonPulse, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // dream eater (frlg/e/xd tutor, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Earthquake, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // endure (emerald tutor, gen 4 tm)
                        Tuple.Create(PMove.EnergyBall, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // facade (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Flash, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Fly, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.Frustration, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // fury cutter (emerald tutor, pt/hgss tutor)
                        // giga impact (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.GrassKnot, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // helping hand (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.HiddenPower, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.HoneClaws, PMoveObtainMethod.TM_BWB2W2),
                        // hyper beam (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.IceBeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.IcyWind, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // last resort (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.LightScreen, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // magic coat (hgss tutor, b2w2 tutor)
                        // magic room (b2w2 tutor)
                        // mimic (frlg/e/xd tutor)
                        Tuple.Create(PMove.MudSlap, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // natural gift (gen 4 tm)
                        // outrage (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.Protect, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Psychic, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // psych up (emerald tutor, gen 4 tm, gen 5 tm)
                        // psyshock (gen 5 tm)
                        Tuple.Create(PMove.RainDance, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Reflect, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // rest (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Retaliate, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Return, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // roar (gen 3 tm, gen 4 tm, gen 5 tm)
                        // role play (hgss tutor, b2w2 tutor)
                        // roost (gen 4 tm, b2w2 tutor)
                        // round (gen 5 tm)
                        // safeguard (gen 4 tm, gen 5 tm)
                        // sandstorm (gen 3 tm, gen 4 tm, gen 5 tm)
                        // secret power (gen 3 tm, gen 4 tm)
                        Tuple.Create(PMove.ShadowBall, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ShadowClaw, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ShockWave, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS),
                        // sleep talk (emerald tutor, gen 4 tm, b2w2 tutor)
                        // snore (emerald tutor, pt/hgss tutor, b2w2 tutor)
                        // solarbeam (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.SteelWing, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS),
                        Tuple.Create(PMove.Substitute, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // sucker punch (pt/hgss tutor)
                        Tuple.Create(PMove.SunnyDay, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Surf, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        // swagger (e/xd tutor, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Swift, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // tailwind (hgss tutor, b2w2 tutor)
                        // telekinesis (gen 5 tm)
                        Tuple.Create(PMove.Thunder, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Thunderbolt, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ThunderWave, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Toxic, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // trick (pt/hgss tutor, b2w2 tutor)
                        // twister (pt/hgss tutor)
                        Tuple.Create(PMove.Waterfall, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.WaterPulse, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS),
                        // whirlpool (hgss hm)
                        Tuple.Create(PMove.ZenHeadbutt, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2)
                    }
                }
            },
            {
                PSpecies.Latios,
                new PPokemonData
                {
                    HP = 80, Attack = 90, Defense = 80, SpAttack = 130, SpDefense = 110, Speed = 110,
                    GenderRatio = PGenderRatio.M1_F0,
                    Type1 = PType.Dragon, Type2 = PType.Psychic,
                    Abilities = new PAbility[] { PAbility.Levitate },
                    MinLevel = 35, // SS (Roaming)
                    ShinyLocked = false,
                    Weight = 60.0,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.DragonBreath, 20, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.DragonDance, 50, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.DragonDance, 55, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.DragonPulse, 70, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS),
                        Tuple.Create(PMove.DragonPulse, 80, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // heal block (5 gen 4 & 5)
                        // heal pulse (65 gen 5)
                        // helping hand (10 gen 3 & 4 & 5)
                        Tuple.Create(PMove.LusterPurge, 35, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // memento (5 gen 3, 60 gen 4, 85 gen 3)
                        // power split (75 gen 5)
                        Tuple.Create(PMove.Protect, 25, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG | PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Psychic, 40, PMoveObtainMethod.LevelUp_RSE | PMoveObtainMethod.LevelUp_FRLG),
                        Tuple.Create(PMove.Psychic, 60, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Psychic, 65, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS),
                        // psycho shift (50 gen 4 & 5)
                        // psywave (1 gen 3 & 4 & 5)
                        // recover (45 gen 3 & 4 & 5)
                        // refresh (30 gen 3 & 4 & 5)
                        // safeguard (15 gen 3 & 4 & 5)
                        // telekinesis (70 gen 5)
                        Tuple.Create(PMove.ZenHeadbutt, 40, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.AerialAce, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // attract (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.BodySlam, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD),
                        Tuple.Create(PMove.Bulldoze, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.CalmMind, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // captivate (gen 4 tm)
                        Tuple.Create(PMove.ChargeBeam, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Cut, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        // defog (dppt hm)
                        Tuple.Create(PMove.Dive, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        // double edge (frlg/e/xd tutor)
                        Tuple.Create(PMove.DoubleTeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.DracoMeteor, PMoveObtainMethod.MoveTutor_DP | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_BW | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.DragonClaw, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.DragonPulse, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // dream eater (frlg/e/xd tutor, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Earthquake, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // endure (emerald tutor, gen 4 tm)
                        Tuple.Create(PMove.EnergyBall, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // facade (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Flash, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Fly, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.Frustration, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // fury cutter (emerald tutor, pt/hgss tutor)
                        // giga impact (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.GrassKnot, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // helping hand (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.HiddenPower, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.HoneClaws, PMoveObtainMethod.TM_BWB2W2),
                        // hyper beam (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.IceBeam, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.IcyWind, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // last resort (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.LightScreen, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // magic coat (hgss tutor, b2w2 tutor)
                        // mimic (frlg/e/xd tutor)
                        Tuple.Create(PMove.MudSlap, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // natural gift (gen 4 tm)
                        // outrage (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.Protect, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Psychic, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // psych up (emerald tutor, gen 4 tm, gen 5 tm)
                        // psyshock (gen 5 tm)
                        Tuple.Create(PMove.RainDance, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Reflect, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // rest (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Retaliate, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Return, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // roar (gen 3 tm, gen 4 tm, gen 5 tm)
                        // roost (gen 4 tm, b2w2 tutor)
                        // round (gen 5 tm)
                        // safeguard (gen 4 tm, gen 5 tm)
                        // sandstorm (gen 3 tm, gen 4 tm, gen 5 tm)
                        // secret power (gen 3 tm, gen 4 tm)
                        Tuple.Create(PMove.ShadowBall, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ShadowClaw, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ShockWave, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS),
                        // sleep talk (emerald tutor, gen 4 tm, b2w2 tutor)
                        // snore (emerald tutor, pt/hgss tutor, b2w2 tutor)
                        // solarbeam (gen 3 tm, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.SteelWing, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS),
                        Tuple.Create(PMove.Substitute, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.SunnyDay, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Surf, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        // swagger (e/xd tutor, gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Swift, PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // tailwind (hgss tutor, b2w2 tutor)
                        // telekinesis (gen 5 tm)
                        Tuple.Create(PMove.Thunder, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Thunderbolt, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ThunderWave, PMoveObtainMethod.MoveTutor_FRLG | PMoveObtainMethod.MoveTutor_E | PMoveObtainMethod.MoveTutor_XD | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Toxic, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // trick (pt/hgss tutor, b2w2 tutor)
                        // twister (pt/hgss tutor)
                        Tuple.Create(PMove.Waterfall, PMoveObtainMethod.HM_RSFRLGE | PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.WaterPulse, PMoveObtainMethod.TM_RSFRLGE | PMoveObtainMethod.TM_DPPtHGSS),
                        // whirlpool (hgss hm)
                        // wonder room (b2w2 tutor)
                        Tuple.Create(PMove.ZenHeadbutt, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2)
                    }
                }
            },
            {
                PSpecies.Rotom,
                new PPokemonData
                {
                    HP = 50, Attack = 50, Defense = 77, SpAttack = 95, SpDefense = 77, Speed = 91,
                    GenderRatio = PGenderRatio.M0_F0,
                    Type1 = PType.Electric, Type2 = PType.Ghost,
                    Abilities = new PAbility[] { PAbility.Levitate },
                    MinLevel = 1, // Egg
                    ShinyLocked = false,
                    Weight = 0.3,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.Astonish, 1, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // charge (43 gen 4, 57 gen 5)
                        Tuple.Create(PMove.ConfuseRay, 1, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Discharge, 50, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS),
                        Tuple.Create(PMove.Discharge, 64, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.DoubleTeam, 15, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // electro ball (43 gen 5)
                        // hex (50 gen 5)
                        Tuple.Create(PMove.OminousWind, 29, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.ShockWave, 22, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Substitute, 36, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.ThunderShock, 1, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.ThunderWave, 1, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // trick (1 gen 4 & 5)
                        // uproar (8 gen 4 & 5)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.ChargeBeam, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.DarkPulse, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.DoubleTeam, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // dream eater (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Electroweb, PMoveObtainMethod.MoveTutor_B2W2),
                        // endure (gen 4 tm)
                        // facade (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Flash, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Frustration, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.HiddenPower, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.LightScreen, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.MudSlap, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // natural gift (gen 4 tm)
                        Tuple.Create(PMove.OminousWind, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        Tuple.Create(PMove.PainSplit, PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.Protect, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // psych up (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.RainDance, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Reflect, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // rest (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Return, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // round (gen 5 tm)
                        // secret power (gen 4 tm)
                        Tuple.Create(PMove.ShadowBall, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ShockWave, PMoveObtainMethod.TM_DPPtHGSS),
                        Tuple.Create(PMove.SignalBeam, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // sleep talk (gen 4 tm, b2w2 tutor)
                        // snatch (gen 4 tm, b2w2 tutor)
                        // snore (pt/hgss tutor, b2w2 tutor)
                        // spite (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.Substitute, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // sucker punch (pt/hgss tutor)
                        Tuple.Create(PMove.SunnyDay, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // swagger (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Swift, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // telekinesis (gen 5 tm)
                        // thief (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Thunder, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Thunderbolt, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ThunderWave, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Toxic, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // trick (pt/hgss tutor, b2w2 tutor)
                        // uproar (pt/hgss tutor, b2w2 tutor)
                        // volt switch (gen 5 tm)
                        Tuple.Create(PMove.WillOWisp, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2)
                    }
                }
            },
            {
                PSpecies.Cresselia,
                new PPokemonData
                {
                    HP = 120, Attack = 70, Defense = 120, SpAttack = 75, SpDefense = 130, Speed = 85,
                    GenderRatio = PGenderRatio.M0_F1,
                    Type1 = PType.Psychic, Type2 = PType.None,
                    Abilities = new PAbility[] { PAbility.Levitate },
                    MinLevel = 50, // DPPt (Roaming)
                    ShinyLocked = false,
                    Weight = 85.6,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.AuroraBeam, 29, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Confusion, 1, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.DoubleTeam, 1, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // future sight (38 gen 4 & 5)
                        // lunar dance (84 gen 4 & 5)
                        // mist (20 gen 4 & 5)
                        Tuple.Create(PMove.Moonlight, 57, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Psychic, 93, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.PsychoCut, 66, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // psycho shift (75 gen 4 & 5)
                        // safeguard (11 gen 4 & 5)
                        Tuple.Create(PMove.Slash, 47, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {
                        // attract (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.CalmMind, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // captivate (gen 4 tm)
                        Tuple.Create(PMove.ChargeBeam, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.DoubleTeam, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // dream eater (gen 4 tm, gen 5 tm)
                        // endure (gen 4 tm)
                        Tuple.Create(PMove.EnergyBall, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // facade (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Flash, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Frustration, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // fury cutter (pt/hgss tutor)
                        // giga impact (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.GrassKnot, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // gravity (hgss tutor, b2w2 tutor)
                        // helping hand (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.HiddenPower, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // hyper beam (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.IceBeam, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.IcyWind, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.LightScreen, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // magic coat (hgss tutor, b2w2 tutor)
                        // magic room (b2w2 tutor)
                        Tuple.Create(PMove.MudSlap, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // natural gift (gen 4 tm)
                        Tuple.Create(PMove.Protect, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Psychic, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // psych up (gen 4 tm, gen 5 tm)
                        // psyshock (gen 5 tm)
                        Tuple.Create(PMove.RainDance, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // recycle (gen 4 tm, b2w2 tutor)
                        Tuple.Create(PMove.Reflect, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // rest (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Return, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // round (gen 5 tm)
                        // safeguard (gen 4 tm, gen 5 tm)
                        // secret power (gen 4 tm)
                        Tuple.Create(PMove.ShadowBall, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.SignalBeam, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // skill swap (gen 4 tm, b2w2 tutor)
                        // sleep talk (gen 4 tm, b2w2 tutor)
                        // snore (pt/hgss tutor, b2w2 tutor)
                        // solarbeam (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Substitute, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.SunnyDay, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // swagger (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Swift, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // telekinesis (gen 5 tm)
                        Tuple.Create(PMove.ThunderWave, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Toxic, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // trick (pt/hgss tutor, b2w2 tutor)
                        // trick room (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.ZenHeadbutt, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2)
                    }
                }
            },
            {
                PSpecies.Darkrai,
                new PPokemonData
                {
                    HP = 70, Attack = 90, Defense = 90, SpAttack = 135, SpDefense = 90, Speed = 125,
                    GenderRatio = PGenderRatio.M0_F0,
                    Type1 = PType.Dark, Type2 = PType.None,
                    Abilities = new PAbility[] { PAbility.BadDreams },
                    MinLevel = 40, // DP (Newmoon Island)
                    ShinyLocked = false,
                    Weight = 50.5,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.DarkPulse, 93, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.DarkVoid, 66, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // disable (1 gen 4 & 5)
                        Tuple.Create(PMove.DoubleTeam, 47, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // dream eater (84 gen 4 & 5)
                        // embargo (75 dp)
                        Tuple.Create(PMove.FaintAttack, 29, PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // haze (57 gen 4 & 5)
                        Tuple.Create(PMove.Hypnosis, 20, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.NastyPlot, 75, PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // nightmare (38 gen 4 & 5)
                        // night shade (1 dp)
                        Tuple.Create(PMove.OminousWind, 1, PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // pursuit (29 dp)
                        Tuple.Create(PMove.QuickAttack, 11, PMoveObtainMethod.LevelUp_DP | PMoveObtainMethod.LevelUp_Pt | PMoveObtainMethod.LevelUp_HGSS | PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.AerialAce, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // blizzard (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.BrickBreak, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.CalmMind, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ChargeBeam, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Cut, PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.DarkPulse, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.DoubleTeam, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // drain punch (gen 4 tm, b2w2 tutor)
                        // dream eater (gen 4 tm, gen 5 tm)
                        // embargo (gen 4 tm, gen 5 tm)
                        // endure (gen 4 tm)
                        // facade (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Flash, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // fling (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.FocusBlast, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // focus punch (gen 4 tm)
                        // foul play (b2w2 tutor)
                        Tuple.Create(PMove.Frustration, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // giga impact (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Headbutt, PMoveObtainMethod.MoveTutor_HGSS),
                        Tuple.Create(PMove.HiddenPower, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // hyper beam (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.IceBeam, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.IcyWind, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS | PMoveObtainMethod.MoveTutor_B2W2),
                        // incinerate (gen 5 tm)
                        // knock off (pt/hgss tutor, b2w2 tutor)
                        // last resort (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.MudSlap, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // natural gift (gen 4 tm)
                        Tuple.Create(PMove.OminousWind, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        // payback (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.PoisonJab, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Protect, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Psychic, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // psych up (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.RainDance, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // rest (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Retaliate, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Return, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RockClimb, PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS),
                        Tuple.Create(PMove.RockSlide, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RockSmash, PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RockTomb, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // round (gen 5 tm)
                        // secret power (gen 4 tm)
                        Tuple.Create(PMove.ShadowBall, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ShadowClaw, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ShockWave, PMoveObtainMethod.TM_DPPtHGSS),
                        // sleep talk (gen 4 tm, b2w2 tutor)
                        Tuple.Create(PMove.SludgeBomb, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Snarl, PMoveObtainMethod.TM_BWB2W2),
                        // snatch (gen 4 tm, b2w2 tutor)
                        // snore (pt/hgss tutor, b2w2 tutor)
                        // spite (pt/hgss tutor, b2w2 tutor)
                        Tuple.Create(PMove.Strength, PMoveObtainMethod.HM_DPPt | PMoveObtainMethod.HM_HGSS | PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.Substitute, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // sucker punch (pt/hgss tutor)
                        Tuple.Create(PMove.SunnyDay, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // swagger (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Swift, PMoveObtainMethod.MoveTutor_Pt | PMoveObtainMethod.MoveTutor_HGSS),
                        Tuple.Create(PMove.SwordsDance, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // taunt (gen 4 tm, gen 5 tm)
                        // thief (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Thunder, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Thunderbolt, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ThunderWave, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // torment (gen 4 tm, gen 5 tm)
                        Tuple.Create(PMove.Toxic, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // trick (b2w2 tutor)
                        Tuple.Create(PMove.WillOWisp, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2),
                        // wonder room (b2w2 tutor)
                        Tuple.Create(PMove.XScissor, PMoveObtainMethod.TM_DPPtHGSS | PMoveObtainMethod.TM_BWB2W2)
                    }
                }
            },
            {
                PSpecies.Cofagrigus,
                new PPokemonData
                {
                    HP = 58, Attack = 50, Defense = 145, SpAttack = 95, SpDefense = 105, Speed = 30,
                    GenderRatio = PGenderRatio.M1_F1,
                    Type1 = PType.Ghost, Type2 = PType.None,
                    Abilities = new PAbility[] { PAbility.Mummy },
                    MinLevel = 34, // Evolve Yamask
                    ShinyLocked = false,
                    Weight = 76.5,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.Astonish, 1, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Curse, 29, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // destiny bond (57 gen 5)
                        // disable (1 & 5 gen 5)
                        // grudge (45 gen 5)
                        // guard split (33 gen 5)
                        // haze (1 & 9 gen 5)
                        // hex (17 gen 5)
                        // mean look (51 gen 5)
                        // night shade (13 gen 5)
                        Tuple.Create(PMove.OminousWind, 25, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // power split (33 gen 5)
                        Tuple.Create(PMove.Protect, 1, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.ScaryFace, 34, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.ShadowBall, 39, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.WillOWisp, 21, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {
                        // after you (b2w2 tutor)
                        // attract (gen 5 tm)
                        // block (b2w2 tutor)
                        Tuple.Create(PMove.CalmMind, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.DarkPulse, PMoveObtainMethod.MoveTutor_B2W2),
                        // disable (gen 5 egg)
                        Tuple.Create(PMove.DoubleTeam, PMoveObtainMethod.TM_BWB2W2),
                        // dream eater (gen 5 tm)
                        // embargo (gen 5 tm)
                        // endure (gen 5 egg)
                        Tuple.Create(PMove.EnergyBall, PMoveObtainMethod.TM_BWB2W2),
                        // facade (gen 5 tm)
                        Tuple.Create(PMove.FakeTears, PMoveObtainMethod.EggMove_BWB2W2),
                        Tuple.Create(PMove.Flash, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Frustration, PMoveObtainMethod.TM_BWB2W2),
                        // giga impact (gen 5 tm)
                        Tuple.Create(PMove.GrassKnot, PMoveObtainMethod.TM_BWB2W2),
                        // heal block (gen 5 egg)
                        Tuple.Create(PMove.HiddenPower, PMoveObtainMethod.TM_BWB2W2),
                        // hyper beam (gen 5 tm)
                        // imprison (gen 5 egg)
                        Tuple.Create(PMove.IronDefense, PMoveObtainMethod.MoveTutor_B2W2),
                        // knock off (b2w2 tutor)
                        // magic coat (b2w2 tutor)
                        // memento (gen 5 egg)
                        Tuple.Create(PMove.NastyPlot, PMoveObtainMethod.EggMove_BWB2W2),
                        // nightmare (gen 5 egg)
                        Tuple.Create(PMove.PainSplit, PMoveObtainMethod.MoveTutor_B2W2),
                        // payback (gen 5 tm)
                        Tuple.Create(PMove.Protect, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Psychic, PMoveObtainMethod.TM_BWB2W2),
                        // psych up (gen 5 tm)
                        Tuple.Create(PMove.RainDance, PMoveObtainMethod.TM_BWB2W2),
                        // rest (gen 5 tm)
                        Tuple.Create(PMove.Return, PMoveObtainMethod.TM_BWB2W2),
                        // role play (b2w2 tutor)
                        // round (gen 5 tm)
                        // safeguard (gen 5 tm)
                        Tuple.Create(PMove.ShadowBall, PMoveObtainMethod.TM_BWB2W2),
                        // skill swap (b2w2 tutor)
                        // sleep talk (b2w2 tutor)
                        // snatch (b2w2 tutor)
                        // snore (b2w2 tutor)
                        // spite (b2w2 tutor)
                        Tuple.Create(PMove.Substitute, PMoveObtainMethod.TM_BWB2W2),
                        // swagger (gen 5 tm)
                        // telekinesis (gen 5 tm)
                        // thief (gen 5 tm)
                        Tuple.Create(PMove.Toxic, PMoveObtainMethod.TM_BWB2W2),
                        // trick (b2w2 tutor)
                        // trick room (gen 5 tm)
                        Tuple.Create(PMove.WillOWisp, PMoveObtainMethod.TM_BWB2W2),
                        // wonder room (b2w2 tutor)
                    }
                }
            },
            {
                PSpecies.Genesect,
                new PPokemonData
                {
                    HP = 71, Attack = 120, Defense = 95, SpAttack = 120, SpDefense = 95, Speed = 99,
                    GenderRatio = PGenderRatio.M0_F0,
                    Type1 = PType.Bug, Type2 = PType.Steel,
                    Abilities = new PAbility[] { PAbility.Download },
                    MinLevel = 15, // Event (Plasma Genesect)
                    ShinyLocked = false,
                    Weight = 82.5,
                    LevelUpMoves = new Tuple<PMove, int, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.BugBuzz, 55, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.FlameCharge, 18, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // fury cutter (7 gen 5)
                        // hyper beam (73 gen 5)
                        // lock on (11 gen 5)
                        Tuple.Create(PMove.MagnetBomb, 22, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // magnet rise (1 gen 5)
                        Tuple.Create(PMove.MetalClaw, 1, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.MetalSound, 33, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.QuickAttack, 1, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.Screech, 1, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // selfdestruct (77 gen 5)
                        Tuple.Create(PMove.SignalBeam, 40, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // simple beam (62 gen 5)
                        Tuple.Create(PMove.Slash, 29, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.TechnoBlast, 1, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        // tri attack (44 gen 5)
                        Tuple.Create(PMove.XScissor, 51, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2),
                        Tuple.Create(PMove.ZapCannon, 66, PMoveObtainMethod.LevelUp_BW | PMoveObtainMethod.LevelUp_B2W2)
                    },
                    OtherMoves = new Tuple<PMove, PMoveObtainMethod>[]
                    {
                        Tuple.Create(PMove.AerialAce, PMoveObtainMethod.TM_BWB2W2),
                        // blizzard (gen 5 tm)
                        // bug bite (b2w2 tutor)
                        Tuple.Create(PMove.ChargeBeam, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.DarkPulse, PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.DoubleTeam, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Electroweb, PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.EnergyBall, PMoveObtainMethod.TM_BWB2W2),
                        // explosion (gen 5 tm)
                        // facade (gen 5 tm)
                        Tuple.Create(PMove.FlameCharge, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Flamethrower, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Flash, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.FlashCannon, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Fly, PMoveObtainMethod.HM_BWB2W2),
                        Tuple.Create(PMove.Frustration, PMoveObtainMethod.TM_BWB2W2),
                        // giga drain (b2w2 tutor)
                        // giga impact (gen 5 tm)
                        // gravity (b2w2 tutor)
                        Tuple.Create(PMove.GunkShot, PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.HiddenPower, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.HoneClaws, PMoveObtainMethod.TM_BWB2W2),
                        // hyper beam (gen 5 tm)
                        Tuple.Create(PMove.IceBeam, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.IronDefense, PMoveObtainMethod.MoveTutor_B2W2),
                        Tuple.Create(PMove.IronHead, PMoveObtainMethod.MoveTutor_B2W2),
                        // last resort (b2w2 tutor)
                        Tuple.Create(PMove.LightScreen, PMoveObtainMethod.TM_BWB2W2),
                        // magic coat (b2w2 tutor)
                        // magnet rise (b2w2 tutor)
                        Tuple.Create(PMove.Protect, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Psychic, PMoveObtainMethod.TM_BWB2W2),
                        // recycle (b2w2 tutor)
                        Tuple.Create(PMove.Reflect, PMoveObtainMethod.TM_BWB2W2),
                        // rest (gen 5 tm)
                        Tuple.Create(PMove.Return, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.RockPolish, PMoveObtainMethod.TM_BWB2W2),
                        // round (gen 5 tm)
                        Tuple.Create(PMove.ShadowClaw, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.SignalBeam, PMoveObtainMethod.MoveTutor_B2W2),
                        // sleep talk (b2w2 tutor)
                        // snore (b2w2 tutor)
                        // solarbeam (gen 5 tm)
                        Tuple.Create(PMove.StruggleBug, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Substitute, PMoveObtainMethod.TM_BWB2W2),
                        // swagger (gen 5 tm)
                        Tuple.Create(PMove.Thunder, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Thunderbolt, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ThunderWave, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.Toxic, PMoveObtainMethod.TM_BWB2W2),
                        // u-turn (gen 5 tm)
                        Tuple.Create(PMove.XScissor, PMoveObtainMethod.TM_BWB2W2),
                        Tuple.Create(PMove.ZenHeadbutt, PMoveObtainMethod.MoveTutor_B2W2)
                    }
                }
            }
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

            IEnumerable<Tuple<PMove, PMoveObtainMethod>> otherMoves = Data[PSpecies.Rotom].OtherMoves;
            Data.Add(PSpecies.Rotom_Fan, (PPokemonData)Data[PSpecies.Rotom].MemberwiseClone());
            Data[PSpecies.Rotom_Fan].HP = 50;
            Data[PSpecies.Rotom_Fan].HP = 65;
            Data[PSpecies.Rotom_Fan].HP = 107;
            Data[PSpecies.Rotom_Fan].HP = 105;
            Data[PSpecies.Rotom_Fan].HP = 107;
            Data[PSpecies.Rotom_Fan].HP = 86;
            Data[PSpecies.Rotom_Fan].Type2 = PType.Flying;
            Data[PSpecies.Rotom_Fan].OtherMoves = otherMoves.Concat(new Tuple<PMove, PMoveObtainMethod>[] { Tuple.Create(PMove.AirSlash, PMoveObtainMethod.Forme) }).ToArray();
            Data.Add(PSpecies.Rotom_Frost, (PPokemonData)Data[PSpecies.Rotom].MemberwiseClone());
            Data[PSpecies.Rotom_Frost].HP = 50;
            Data[PSpecies.Rotom_Frost].HP = 65;
            Data[PSpecies.Rotom_Frost].HP = 107;
            Data[PSpecies.Rotom_Frost].HP = 105;
            Data[PSpecies.Rotom_Frost].HP = 107;
            Data[PSpecies.Rotom_Frost].HP = 86;
            Data[PSpecies.Rotom_Frost].Type2 = PType.Ice;
            //Data[PSpecies.Rotom_Frost].OtherMoves = otherMoves.Concat(new PMove[] { PMove.Blizzard }).ToArray();
            Data.Add(PSpecies.Rotom_Heat, (PPokemonData)Data[PSpecies.Rotom].MemberwiseClone());
            Data[PSpecies.Rotom_Heat].HP = 50;
            Data[PSpecies.Rotom_Heat].HP = 65;
            Data[PSpecies.Rotom_Heat].HP = 107;
            Data[PSpecies.Rotom_Heat].HP = 105;
            Data[PSpecies.Rotom_Heat].HP = 107;
            Data[PSpecies.Rotom_Heat].HP = 86;
            Data[PSpecies.Rotom_Heat].Type2 = PType.Fire;
            Data[PSpecies.Rotom_Heat].OtherMoves = otherMoves.Concat(new Tuple<PMove, PMoveObtainMethod>[] { Tuple.Create(PMove.Overheat, PMoveObtainMethod.Forme) }).ToArray();
            Data.Add(PSpecies.Rotom_Mow, (PPokemonData)Data[PSpecies.Rotom].MemberwiseClone());
            Data[PSpecies.Rotom_Mow].HP = 50;
            Data[PSpecies.Rotom_Mow].HP = 65;
            Data[PSpecies.Rotom_Mow].HP = 107;
            Data[PSpecies.Rotom_Mow].HP = 105;
            Data[PSpecies.Rotom_Mow].HP = 107;
            Data[PSpecies.Rotom_Mow].HP = 86;
            Data[PSpecies.Rotom_Mow].Type2 = PType.Grass;
            //Data[PSpecies.Rotom_Mow].OtherMoves = otherMoves.Concat(new PMove[] { PMove.LeafStorm }).ToArray();
            Data.Add(PSpecies.Rotom_Wash, (PPokemonData)Data[PSpecies.Rotom].MemberwiseClone());
            Data[PSpecies.Rotom_Wash].HP = 50;
            Data[PSpecies.Rotom_Wash].HP = 65;
            Data[PSpecies.Rotom_Wash].HP = 107;
            Data[PSpecies.Rotom_Wash].HP = 105;
            Data[PSpecies.Rotom_Wash].HP = 107;
            Data[PSpecies.Rotom_Wash].HP = 86;
            Data[PSpecies.Rotom_Wash].Type2 = PType.Water;
            Data[PSpecies.Rotom_Wash].OtherMoves = otherMoves.Concat(new Tuple<PMove, PMoveObtainMethod>[] { Tuple.Create(PMove.HydroPump, PMoveObtainMethod.Forme) }).ToArray();

            Data.Add(PSpecies.Genesect_Burn, (PPokemonData)Data[PSpecies.Genesect].MemberwiseClone());
            Data.Add(PSpecies.Genesect_Chill, (PPokemonData)Data[PSpecies.Genesect].MemberwiseClone());
            Data.Add(PSpecies.Genesect_Douse, (PPokemonData)Data[PSpecies.Genesect].MemberwiseClone());
            Data.Add(PSpecies.Genesect_Shock, (PPokemonData)Data[PSpecies.Genesect].MemberwiseClone());
        }
    }
}
