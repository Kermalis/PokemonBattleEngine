using System.Collections.Generic;

namespace PokemonBattleEngine.Data
{
    class MoveData
    {
        public Type Type;
        public MoveCategory Category;
        public Effect Effect;
        public int EffectParam;
        public int PP, Power, Priority;
        public double Accuracy; // Below 0 indicates an attack that doesn't miss
        public MoveFlags Flags;
        public PossibleTarget Targets;

        public static Dictionary<Move, MoveData> Data = new Dictionary<Move, MoveData>()
        {
            {
                Move.DarkPulse,
                new MoveData
                {
                    Type = Type.Dark, Category = MoveCategory.Special,
                    Effect = Effect.Flinch_Hit, EffectParam = 20,
                    PP = 15, Power = 80, Accuracy = 1, Priority = 0,
                    Flags = MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove | MoveFlags.AffectedByKingsRock,
                    Targets = PossibleTarget.Any
                }
            },
            {
                Move.DragonPulse,
                new MoveData
                {
                    Type = Type.Dragon, Category = MoveCategory.Special,
                    Effect = Effect.Hit, EffectParam = 0,
                    PP = 10, Power = 90, Accuracy = 1, Priority = 0,
                    Flags = MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove | MoveFlags.AffectedByKingsRock,
                    Targets = PossibleTarget.Any
                }
            },
            {
                Move.HydroPump,
                new MoveData
                {
                    Type = Type.Water, Category = MoveCategory.Special,
                    Effect = Effect.Hit, EffectParam = 0,
                    PP = 5, Power = 120, Accuracy = 0.8, Priority = 0,
                    Flags = MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove | MoveFlags.AffectedByKingsRock,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.Psychic,
                new MoveData
                {
                    Type = Type.Psychic, Category = MoveCategory.Special,
                    Effect = Effect.Lower_SPDEF_By1_Hit, EffectParam = 10,
                    PP = 10, Power = 90, Accuracy = 1, Priority = 0,
                    Flags = MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.Retaliate,
                new MoveData
                {
                    Type = Type.Normal, Category = MoveCategory.Physical,
                    Effect = Effect.Hit, EffectParam = 0,
                    PP = 5, Power = 70, Accuracy = 1, Priority = 0,
                    Flags = MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove | MoveFlags.AffectedByKingsRock,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.ShellSmash,
                new MoveData
                {
                    Type = Type.Normal, Category = MoveCategory.Status,
                    Effect = Effect.Lower_DEF_SPDEF_By1_Raise_ATK_SPATK_SPD_By2, EffectParam = 100,
                    PP = 15, Power = 0, Accuracy = -1, Priority = 0,
                    Flags = MoveFlags.AffectedBySnatch,
                    Targets = PossibleTarget.Self
                }
            },
            {
                Move.Tackle,
                new MoveData
                {
                    Type = Type.Normal, Category = MoveCategory.Physical,
                    Effect = Effect.Hit, EffectParam = 0,
                    PP = 35, Power = 50, Accuracy = 1, Priority = 0,
                    Flags = MoveFlags.MakesContact | MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove | MoveFlags.AffectedByKingsRock,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.Thunder,
                new MoveData
                {
                    Type = Type.Electric, Category = MoveCategory.Special,
                    Effect = Effect.Paralyze_Hit, EffectParam = 30,
                    PP = 10, Power = 120, Accuracy = 0.7, Priority = 0,
                    Flags = MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.Transform,
                new MoveData
                {
                    Type = Type.Normal, Category = MoveCategory.Status,
                    Effect = Effect.Transform, EffectParam = 0,
                    PP = 10, Power = 0, Accuracy = -1, Priority = 0,
                    Flags = MoveFlags.None,
                    Targets = PossibleTarget.Self
                }
            },
        };
    }
}
