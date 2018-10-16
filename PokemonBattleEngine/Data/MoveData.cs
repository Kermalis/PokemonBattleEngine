using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Data
{
    class MoveData
    {
        public Type Type;
        public MoveCategory Category;
        public MoveEffect Effect;
        public int EffectParam;
        public byte PP, Power, Accuracy; // 0 power or accuracy will show up as --
        public sbyte Priority;
        public MoveFlags Flags;
        public PossibleTarget Targets;

        public static Dictionary<Move, MoveData> Data = new Dictionary<Move, MoveData>()
        {
            {
                Move.AquaJet,
                new MoveData
                {
                    Type = Type.Water, Category = MoveCategory.Physical,
                    Effect = MoveEffect.Hit, EffectParam = 0,
                    PP = 20, Power = 40, Accuracy = 100, Priority = +1,
                    Flags = MoveFlags.MakesContact | MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.DarkPulse,
                new MoveData
                {
                    Type = Type.Dark, Category = MoveCategory.Special,
                    Effect = MoveEffect.Hit__MaybeFlinch, EffectParam = 20,
                    PP = 15, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove,
                    Targets = PossibleTarget.Any
                }
            },
            {
                Move.DragonPulse,
                new MoveData
                {
                    Type = Type.Dragon, Category = MoveCategory.Special,
                    Effect = MoveEffect.Hit, EffectParam = 0,
                    PP = 10, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove,
                    Targets = PossibleTarget.Any
                }
            },
            {
                Move.HydroPump,
                new MoveData
                {
                    Type = Type.Water, Category = MoveCategory.Special,
                    Effect = MoveEffect.Hit, EffectParam = 0,
                    PP = 5, Power = 120, Accuracy = 80, Priority = 0,
                    Flags = MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.IceBeam,
                new MoveData
                {
                    Type = Type.Ice, Category = MoveCategory.Special,
                    Effect = MoveEffect.Hit__MaybeFreeze, EffectParam = 10,
                    PP = 10, Power = 95, Accuracy = 100, Priority = 0,
                    Flags = MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.IcePunch,
                new MoveData
                {
                    Type = Type.Ice, Category = MoveCategory.Physical,
                    Effect = MoveEffect.Hit__MaybeFreeze, EffectParam = 10,
                    PP = 15, Power = 75, Accuracy = 100, Priority = 0,
                    Flags = MoveFlags.MakesContact | MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.Moonlight,
                new MoveData
                {
                    Type = Type.Normal, Category = MoveCategory.Status,
                    Effect = MoveEffect.Moonlight, EffectParam = 0,
                    PP = 5, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = MoveFlags.AffectedBySnatch,
                    Targets = PossibleTarget.Self
                }
            },
            {
                Move.Psychic,
                new MoveData
                {
                    Type = Type.Psychic, Category = MoveCategory.Special,
                    Effect = MoveEffect.Hit__MaybeLower_SPDEF_By1, EffectParam = 10,
                    PP = 10, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.Retaliate,
                new MoveData
                {
                    Type = Type.Normal, Category = MoveCategory.Physical,
                    Effect = MoveEffect.Hit, EffectParam = 0,
                    PP = 5, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.Return,
                new MoveData
                {
                    Type = Type.Normal, Category = MoveCategory.Physical,
                    Effect = MoveEffect.Hit, EffectParam = 0,
                    PP = 5, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.ShellSmash,
                new MoveData
                {
                    Type = Type.Normal, Category = MoveCategory.Status,
                    Effect = MoveEffect.Lower_DEF_SPDEF_By1_Raise_ATK_SPATK_SPD_By2, EffectParam = 100,
                    PP = 15, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = MoveFlags.AffectedBySnatch,
                    Targets = PossibleTarget.Self
                }
            },
            {
                Move.Tackle,
                new MoveData
                {
                    Type = Type.Normal, Category = MoveCategory.Physical,
                    Effect = MoveEffect.Hit, EffectParam = 0,
                    PP = 35, Power = 50, Accuracy = 100, Priority = 0,
                    Flags = MoveFlags.MakesContact | MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.Thunder,
                new MoveData
                {
                    Type = Type.Electric, Category = MoveCategory.Special,
                    Effect = MoveEffect.Hit__MaybeParalyze, EffectParam = 30,
                    PP = 10, Power = 120, Accuracy = 70, Priority = 0,
                    Flags = MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.Toxic,
                new MoveData
                {
                    Type = Type.Poison, Category = MoveCategory.Status,
                    Effect = MoveEffect.Toxic, EffectParam = 0,
                    PP = 10, Power = 0, Accuracy = 90, Priority = 0,
                    Flags = MoveFlags.AffectedByProtect | MoveFlags.AffectedByMagicCoat | MoveFlags.AffectedByMirrorMove,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
            {
                Move.Transform,
                new MoveData
                {
                    Type = Type.Normal, Category = MoveCategory.Status,
                    Effect = MoveEffect.Transform, EffectParam = 0,
                    PP = 10, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = MoveFlags.None,
                    Targets = PossibleTarget.Self
                }
            },
            {
                Move.Waterfall,
                new MoveData
                {
                    Type = Type.Water, Category = MoveCategory.Physical,
                    Effect = MoveEffect.Hit__MaybeFlinch, EffectParam = 20,
                    PP = 15, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = MoveFlags.MakesContact | MoveFlags.AffectedByProtect | MoveFlags.AffectedByMirrorMove,
                    Targets = PossibleTarget.AnySurrounding
                }
            },
        };
    }
}
