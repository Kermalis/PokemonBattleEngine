using System.Collections.Generic;

namespace PokemonBattleEngine.Data
{
    class MoveData
    {
        public Type Type { get; private set; }
        public MoveCategory Category { get; private set; }
        public Effect Effect { get; private set; }
        public double EffectChance { get; private set; }
        public int PP { get; private set; }
        public int Power { get; private set; }
        public double Accuracy { get; private set; } // Below 0 indicates an attack that doesn't miss
        public int Priority { get; private set; }
        public bool MakesContact { get; private set; }
        public bool AffectedByProtect { get; private set; }
        public bool AffectedByMagicCoat { get; private set; }
        public bool AffectedBySnatch { get; private set; }
        public bool AffectedByMirrorMove { get; private set; }
        public bool AffectedByKingsRock { get; private set; }
        public PossibleTarget Targets { get; private set; }

        public static Dictionary<Move, MoveData> Data = new Dictionary<Move, MoveData>()
        {
            {
                Move.DarkPulse,
                new MoveData
                {
                    Type = Type.Dark, Category = MoveCategory.Special,
                    Effect = Effect.Flinch, EffectChance = 0.2,
                    PP = 15, Power = 80, Accuracy = 1, Priority = 0,
                    MakesContact = false, AffectedByProtect = true, AffectedByMagicCoat = false, AffectedBySnatch = false, AffectedByMirrorMove = true, AffectedByKingsRock = true,
                    Targets = PossibleTarget.Any
                }
            },
            {
                Move.DragonPulse,
                new MoveData
                {
                    Type = Type.Dragon, Category = MoveCategory.Special,
                    Effect = Effect.None, EffectChance = 0,
                    PP = 10, Power = 90, Accuracy = 1, Priority = 0,
                    MakesContact = false, AffectedByProtect = true, AffectedByMagicCoat = false, AffectedBySnatch = false, AffectedByMirrorMove = true, AffectedByKingsRock = true,
                    Targets = PossibleTarget.Any
                }
            },
            {
                Move.HydroPump,
                new MoveData
                {
                    Type = Type.Water, Category = MoveCategory.Special,
                    Effect = Effect.None, EffectChance = 0,
                    PP = 5, Power = 120, Accuracy = 0.8, Priority = 0,
                    MakesContact = false, AffectedByProtect = true, AffectedByMagicCoat = false, AffectedBySnatch = false, AffectedByMirrorMove = true, AffectedByKingsRock = false,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.Psychic,
                new MoveData
                {
                    Type = Type.Psychic, Category = MoveCategory.Special,
                    Effect = Effect.LowerSPDEFBy1, EffectChance = 0.1,
                    PP = 10, Power = 90, Accuracy = 1, Priority = 0,
                    MakesContact = false, AffectedByProtect = true, AffectedByMagicCoat = false, AffectedBySnatch = false, AffectedByMirrorMove = true, AffectedByKingsRock = false,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.Tackle,
                new MoveData
                {
                    Type = Type.Normal, Category = MoveCategory.Physical,
                    Effect = Effect.None, EffectChance = 0,
                    PP = 35, Power = 50, Accuracy = 1, Priority = 0,
                    MakesContact = true, AffectedByProtect = true, AffectedByMagicCoat = false, AffectedBySnatch = false, AffectedByMirrorMove = true, AffectedByKingsRock = true,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.Thunder,
                new MoveData
                {
                    Type = Type.Electric, Category = MoveCategory.Special,
                    Effect = Effect.Paralyze, EffectChance = 0.3,
                    PP = 10, Power = 120, Accuracy = 0.7, Priority = 0,
                    MakesContact = false, AffectedByProtect = true, AffectedByMagicCoat = false, AffectedBySnatch = false, AffectedByMirrorMove = true, AffectedByKingsRock = false,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
        };
    }
}
