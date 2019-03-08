using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Localization;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Battle
{
    /// <summary>
    /// Represents a specific Pokémon during a battle.
    /// </summary>
    public sealed class PBEPokemon
    {
        /// <summary>
        /// The team this Pokémon belongs to in its battle.
        /// </summary>
        public PBETeam Team { get; }
        /// <summary>
        /// The Pokémon's ID in its battle.
        /// </summary>
        public byte Id { get; }
        /// <summary>
        /// The shell that was used to create this Pokémon.
        /// </summary>
        public PBEPokemonShell Shell { get; }

        /// <summary>
        /// The Pokémon's nickname with its gender attached.
        /// </summary>
        public string NameWithGender => Nickname + GenderSymbol;
        /// <summary>
        /// The Pokémon's gender as a string.
        /// </summary>
        public string GenderSymbol => Gender == PBEGender.Female ? "♀" : Gender == PBEGender.Male ? "♂" : string.Empty;
        /// <summary>
        /// The Pokémon's known nickname with its known gender attached.
        /// </summary>
        public string KnownNameWithKnownGender => KnownNickname + KnownGenderSymbol;
        /// <summary>
        /// The Pokémon's known gender as a string.
        /// </summary>
        public string KnownGenderSymbol => KnownGender == PBEGender.Female ? "♀" : KnownGender == PBEGender.Male ? "♂" : string.Empty;

        /// <summary>
        /// The Pokémon's current HP.
        /// </summary>
        public ushort HP { get; set; }
        /// <summary>
        /// The Pokémon's maximum HP.
        /// </summary>
        public ushort MaxHP { get; set; }
        /// <summary>
        /// The Pokémon's current HP as a percentage.
        /// </summary>
        public double HPPercentage { get; set; }
        /// <summary>
        /// The Pokémon's attack stat.
        /// </summary>
        public ushort Attack { get; set; }
        public sbyte AttackChange { get; set; }
        /// <summary>
        /// The Pokémon's defense stat.
        /// </summary>
        public ushort Defense { get; set; }
        public sbyte DefenseChange { get; set; }
        /// <summary>
        /// The Pokémon's special attack stat.
        /// </summary>
        public ushort SpAttack { get; set; }
        public sbyte SpAttackChange { get; set; }
        /// <summary>
        /// The Pokémon's special defense stat.
        /// </summary>
        public ushort SpDefense { get; set; }
        public sbyte SpDefenseChange { get; set; }
        /// <summary>
        /// The Pokémon's speed stat.
        /// </summary>
        public ushort Speed { get; set; }
        public sbyte SpeedChange { get; set; }
        public sbyte AccuracyChange { get; set; }
        public sbyte EvasionChange { get; set; }
        public byte[] EVs { get; set; }
        public byte Friendship { get; set; }
        public byte[] IVs { get; set; }
        /// <summary>
        /// The Pokémon's level.
        /// </summary>
        public byte Level { get; set; }
        /// <summary>
        /// The Pokémon's nature.
        /// </summary>
        public PBENature Nature { get; set; }
        public byte[] PPUps { get; set; }

        /// <summary>
        /// The Pokémon's field position.
        /// </summary>
        public PBEFieldPosition FieldPosition { get; set; }
        /// <summary>
        /// The Pokémon's current ability.
        /// </summary>
        public PBEAbility Ability { get; set; }
        /// <summary>
        /// The Pokémon's normal ability.
        /// </summary>
        public PBEAbility OriginalAbility { get; set; }
        /// <summary>
        /// The ability the Pokémon is known to have.
        /// </summary>
        public PBEAbility KnownAbility { get; set; }
        /// <summary>
        /// The Pokémon's gender.
        /// </summary>
        public PBEGender Gender { get; set; }
        /// <summary>
        /// The gender the Pokémon looks like (affected by transforming and disguising).
        /// </summary>
        public PBEGender KnownGender { get; set; }
        /// <summary>
        /// The Pokémon's current item.
        /// </summary>
        public PBEItem Item { get; set; }
        /// <summary>
        /// The item the Pokémon is known to have.
        /// </summary>
        public PBEItem KnownItem { get; set; }
        /// <summary>
        /// The moves the Pokémon currently has.
        /// </summary>
        public PBEMove[] Moves { get; set; }
        /// <summary>
        /// The moves the Pokémon is known to have.
        /// </summary>
        public PBEMove[] KnownMoves { get; set; }
        public byte[] PP { get; set; }
        public byte[] MaxPP { get; set; }
        /// <summary>
        /// The nickname the Pokémon normally has.
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// The nickname the Pokémon is known to have.
        /// </summary>
        public string KnownNickname { get; set; }
        /// <summary>
        /// The shininess the Pokémon normally has.
        /// </summary>
        public bool Shiny { get; set; }
        /// <summary>
        /// The shininess everyone sees the Pokémon has.
        /// </summary>
        public bool KnownShiny { get; set; }
        /// <summary>
        /// The current species of the Pokémon (affected by transforming or forme changing).
        /// </summary>
        public PBESpecies Species { get; set; }
        /// <summary>
        /// The species the Pokémon normally is.
        /// </summary>
        public PBESpecies OriginalSpecies { get; set; }
        /// <summary>
        /// The species everyone sees the Pokémon as (affected by transforming, disguising, or forme changing).
        /// </summary>
        public PBESpecies KnownSpecies { get; set; }
        public PBEStatus1 Status1 { get; set; }
        public PBEStatus2 Status2 { get; set; }
        /// <summary>
        /// The Pokémon's first type.
        /// </summary>
        public PBEType Type1 { get; set; }
        /// <summary>
        /// The first type everyone believes the Pokémon has.
        /// </summary>
        public PBEType KnownType1 { get; set; }
        /// <summary>
        /// The Pokémon's second type.
        /// </summary>
        public PBEType Type2 { get; set; }
        /// <summary>
        /// The second type everyone believes the Pokémon has.
        /// </summary>
        public PBEType KnownType2 { get; set; }
        public double Weight { get; set; }
        public double KnownWeight { get; set; }

        #region Statuses
        /// <summary>
        /// The counter used for <see cref="PBEStatus1.BadlyPoisoned"/> and <see cref="PBEStatus1.Asleep"/>.
        /// </summary>
        public byte Status1Counter { get; set; }
        /// <summary>
        /// The amount of turns the Pokémon will sleep for before waking.
        /// </summary>
        public byte SleepTurns { get; set; }
        /// <summary>
        /// The counter used for <see cref="PBEStatus2.Confused"/>.
        /// </summary>
        public byte ConfusionCounter { get; set; }
        /// <summary>
        /// The amount of turns the Pokémon will be confused for before snapping out of it.
        /// </summary>
        public byte ConfusionTurns { get; set; }
        public PBEPokemon DisguisedAsPokemon { get; set; } // Illusion
        /// <summary>
        /// The position to return <see cref="PBEStatus2.LeechSeed"/> HP to on the opposing team.
        /// </summary>
        public PBEFieldPosition SeededPosition { get; set; }
        /// <summary>
        /// The amount of HP the Pokémon's <see cref="PBEStatus2.Substitute"/> has left.
        /// </summary>
        public ushort SubstituteHP { get; set; }
        public PBEMove[] PreTransformMoves { get; set; }
        public byte[] PreTransformPP { get; set; }
        public byte[] PreTransformMaxPP { get; set; }
        #endregion

        #region Actions
        public PBEAction SelectedAction; // Must be a field
        public List<PBEExecutedMove> ExecutedMoves { get; } = new List<PBEExecutedMove>();
        /// <summary>
        /// The move the Pokémon is forced to use by multi-turn moves.
        /// </summary>
        public PBEMove TempLockedMove { get; set; }
        /// <summary>
        /// The targets the Pokémon is forced to target by multi-turn moves.
        /// </summary>
        public PBETarget TempLockedTargets { get; set; }
        /// <summary>
        /// The move the Pokémon is forced to use by its choice item.
        /// </summary>
        public PBEMove ChoiceLockedMove { get; set; }
        #endregion

        // Stats & PP are set from the shell info
        internal PBEPokemon(PBETeam team, byte id, PBEPokemonShell shell)
        {
            Team = team;
            SelectedAction.PokemonId = Id = id;
            Shell = shell;
            Ability = OriginalAbility = Shell.Ability;
            KnownAbility = PBEAbility.MAX;
            Gender = KnownGender = Shell.Gender;
            Item = Shell.Item;
            KnownItem = (PBEItem)ushort.MaxValue;
            Moves = (PBEMove[])Shell.Moves.Clone();
            KnownMoves = new PBEMove[Team.Battle.Settings.NumMoves];
            for (int i = 0; i < Team.Battle.Settings.NumMoves; i++)
            {
                KnownMoves[i] = PBEMove.MAX;
            }
            Nickname = KnownNickname = Shell.Nickname;
            Shiny = KnownShiny = Shell.Shiny;
            Species = OriginalSpecies = KnownSpecies = Shell.Species;
            PBEPokemonData pData = PBEPokemonData.Data[Species];
            KnownType1 = Type1 = pData.Type1;
            KnownType2 = Type2 = pData.Type2;
            KnownWeight = Weight = pData.Weight;
            EVs = (byte[])Shell.EVs.Clone();
            Friendship = Shell.Friendship;
            IVs = (byte[])Shell.IVs.Clone();
            Level = Shell.Level;
            Nature = Shell.Nature;
            SetStats();
            HP = MaxHP;
            HPPercentage = 1.0;
            PP = new byte[Team.Battle.Settings.NumMoves];
            MaxPP = new byte[Team.Battle.Settings.NumMoves];
            for (int i = 0; i < Team.Battle.Settings.NumMoves; i++)
            {
                PBEMove move = Moves[i];
                if (move != PBEMove.None)
                {
                    byte tier = PBEMoveData.Data[move].PPTier;
                    PP[i] = MaxPP[i] = (byte)Math.Max(1, (tier * Team.Battle.Settings.PPMultiplier) + (tier * Shell.PPUps[i]));
                }
            }
            Team.Party.Add(this);
        }
        // This constructor is to define a remote Pokémon
        public PBEPokemon(PBETeam team, PBEPkmnSwitchInPacket.PBESwitchInInfo info)
        {
            Team = team;
            Id = info.PokemonId;
            FieldPosition = info.FieldPosition;
            HP = info.HP;
            MaxHP = info.MaxHP;
            HPPercentage = info.HPPercentage;
            Status1 = info.Status1;
            Level = info.Level;
            KnownAbility = PBEAbility.MAX;
            KnownGender = info.Gender;
            KnownItem = (PBEItem)ushort.MaxValue;
            Moves = new PBEMove[Team.Battle.Settings.NumMoves];
            KnownMoves = new PBEMove[Team.Battle.Settings.NumMoves];
            for (int i = 0; i < Team.Battle.Settings.NumMoves; i++)
            {
                KnownMoves[i] = Moves[i] = PBEMove.MAX;
            }
            PP = new byte[Team.Battle.Settings.NumMoves];
            MaxPP = new byte[Team.Battle.Settings.NumMoves];
            KnownNickname = info.Nickname;
            KnownShiny = info.Shiny;
            Species = KnownSpecies = info.Species;
            PBEPokemonData pData = PBEPokemonData.Data[KnownSpecies];
            KnownType1 = Type1 = pData.Type1;
            KnownType2 = Type2 = pData.Type2;
            KnownWeight = Weight = pData.Weight;
            Team.Party.Add(this);
        }

        /// <summary>
        /// Returns True if the Pokémon has <paramref name="type"/>, False otherwise.
        /// </summary>
        /// <param name="type">The type to check.</param>
        public bool HasType(PBEType type)
        {
            return Type1 == type || Type2 == type;
        }
        public PBEStat[] GetChangedStats()
        {
            var list = new List<PBEStat>(7);
            if (AttackChange != 0)
            {
                list.Add(PBEStat.Attack);
            }
            if (DefenseChange != 0)
            {
                list.Add(PBEStat.Defense);
            }
            if (SpAttackChange != 0)
            {
                list.Add(PBEStat.SpAttack);
            }
            if (SpDefenseChange != 0)
            {
                list.Add(PBEStat.SpDefense);
            }
            if (SpeedChange != 0)
            {
                list.Add(PBEStat.Speed);
            }
            if (AccuracyChange != 0)
            {
                list.Add(PBEStat.Accuracy);
            }
            if (EvasionChange != 0)
            {
                list.Add(PBEStat.Evasion);
            }
            return list.ToArray();
        }

        void SetStats()
        {
            MaxHP = PBEPokemonData.CalculateStat(PBEStat.HP, Species, Nature, EVs[0], IVs[0], Level, Team.Battle.Settings);
            Attack = PBEPokemonData.CalculateStat(PBEStat.Attack, Species, Nature, EVs[1], IVs[1], Level, Team.Battle.Settings);
            Defense = PBEPokemonData.CalculateStat(PBEStat.Defense, Species, Nature, EVs[2], IVs[2], Level, Team.Battle.Settings);
            SpAttack = PBEPokemonData.CalculateStat(PBEStat.SpAttack, Species, Nature, EVs[3], IVs[3], Level, Team.Battle.Settings);
            SpDefense = PBEPokemonData.CalculateStat(PBEStat.SpDefense, Species, Nature, EVs[4], IVs[4], Level, Team.Battle.Settings);
            Speed = PBEPokemonData.CalculateStat(PBEStat.Speed, Species, Nature, EVs[5], IVs[5], Level, Team.Battle.Settings);
        }

        /// <summary>
        /// Sets and clears all information required for switching out.
        /// </summary>
        public void ClearForSwitch()
        {
            FieldPosition = PBEFieldPosition.None;
            switch (Ability)
            {
                case PBEAbility.NaturalCure:
                    {
                        Status1 = PBEStatus1.None;
                        Status1Counter = SleepTurns = 0;
                        break;
                    }
                case PBEAbility.Regenerator:
                    {
                        HP = PBEUtils.Clamp((ushort)(HP + (MaxHP / 3)), ushort.MinValue, MaxHP);
                        HPPercentage = (double)HP / MaxHP;
                        break;
                    }
            }
            Ability = OriginalAbility;
            KnownAbility = PBEAbility.MAX;
            KnownGender = Gender;
            KnownItem = (PBEItem)ushort.MaxValue;
            for (int i = 0; i < Team.Battle.Settings.NumMoves; i++)
            {
                KnownMoves[i] = PBEMove.MAX;
            }
            KnownNickname = Nickname;
            KnownShiny = Shiny;
            Species = KnownSpecies = OriginalSpecies;
            PBEPokemonData pData = PBEPokemonData.Data[Species];
            KnownType1 = Type1 = pData.Type1;
            KnownType2 = Type2 = pData.Type2;

            AttackChange = DefenseChange = SpAttackChange = SpDefenseChange = SpeedChange = AccuracyChange = EvasionChange = 0;

            if (Status1 == PBEStatus1.Asleep)
            {
                Status1Counter = SleepTurns;
            }
            else if (Status1 == PBEStatus1.BadlyPoisoned)
            {
                Status1Counter = 1;
            }

            ConfusionCounter = ConfusionTurns = 0;
            DisguisedAsPokemon = null;
            SeededPosition = PBEFieldPosition.None;
            SubstituteHP = 0;
            if (Status2.HasFlag(PBEStatus2.Transformed))
            {
                Moves = (PBEMove[])PreTransformMoves.Clone();
                PP = (byte[])PreTransformPP.Clone();
                MaxPP = (byte[])PreTransformMaxPP.Clone();
                PreTransformMoves = null;
                PreTransformPP = null;
                PreTransformMaxPP = null;
            }
            Status2 = PBEStatus2.None;

            TempLockedMove = ChoiceLockedMove = PBEMove.None;
            TempLockedTargets = PBETarget.None;

            ExecutedMoves.Clear();

            if (Id != byte.MaxValue)
            {
                SetStats();
            }
        }

        /// <summary>
        /// Transforms into <paramref name="target"/> and sets <see cref="PBEStatus2.Transformed"/>.
        /// </summary>
        /// <param name="target">The Pokémon to transform into.</param>
        /// <remarks>Frees the Pokémon of its <see cref="ChoiceLockedMove"/>.</remarks>
        public void Transform(PBEPokemon target)
        {
            if (Team != target.Team)
            {
                KnownAbility = target.KnownAbility = Ability = target.Ability;
                KnownType1 = target.KnownType1 = Type1 = target.Type1;
                KnownType2 = target.KnownType2 = Type2 = target.Type2;
                KnownWeight = target.KnownWeight = Weight = target.Weight;
            }
            else
            {
                Ability = target.Ability;
                KnownAbility = target.KnownAbility;
                Type1 = target.Type1;
                KnownType1 = target.KnownType1;
                Type2 = target.Type2;
                KnownType2 = target.KnownType2;
                Weight = target.Weight;
                KnownWeight = target.KnownWeight;
            }
            KnownGender = target.KnownGender = target.Gender;
            KnownShiny = target.KnownShiny = target.Shiny;
            KnownSpecies = target.KnownSpecies = Species = target.Species;
            Attack = target.Attack;
            Defense = target.Defense;
            SpAttack = target.SpAttack;
            SpDefense = target.SpDefense;
            Speed = target.Speed;
            AttackChange = target.AttackChange;
            DefenseChange = target.DefenseChange;
            SpAttackChange = target.SpAttackChange;
            SpDefenseChange = target.SpDefenseChange;
            SpeedChange = target.SpeedChange;
            AccuracyChange = target.AccuracyChange;
            EvasionChange = target.EvasionChange;
            PreTransformMoves = (PBEMove[])Moves.Clone();
            PreTransformPP = (byte[])PP.Clone();
            PreTransformMaxPP = (byte[])MaxPP.Clone();
            for (int i = 0; i < Team.Battle.Settings.NumMoves; i++)
            {
                if (Team != target.Team)
                {
                    KnownMoves[i] = target.KnownMoves[i] = Moves[i] = target.Moves[i];
                }
                else
                {
                    Moves[i] = target.Moves[i];
                    KnownMoves[i] = target.KnownMoves[i];
                }
                if (Id != byte.MaxValue)
                {
                    PP[i] = MaxPP[i] = (byte)(Moves[i] == PBEMove.None ? 0 : PBEMoveData.Data[Moves[i]].PPTier == 0 ? 1 : Team.Battle.Settings.PPMultiplier);
                }
            }
            if (!Moves.Contains(ChoiceLockedMove))
            {
                ChoiceLockedMove = PBEMove.None;
            }
            Status2 |= PBEStatus2.Transformed;
        }

        /// <summary>
        /// Gets the type that <see cref="PBEMove.HiddenPower"/> will become when used by this Pokémon.
        /// </summary>
        public PBEType GetHiddenPowerType()
        {
            int a = IVs[0] & 1,
                b = IVs[1] & 1,
                c = IVs[2] & 1,
                d = IVs[5] & 1,
                e = IVs[3] & 1,
                f = IVs[4] & 1;
            return PBEPokemonData.HiddenPowerTypes[((1 << 0) * a + (1 << 1) * b + (1 << 2) * c + (1 << 3) * d + (1 << 4) * e + (1 << 5) * f) * (PBEPokemonData.HiddenPowerTypes.Count - 1) / ((1 << 6) - 1)];
        }
        /// <summary>
        /// Gets the base power that <see cref="PBEMove.HiddenPower"/> will have when used by this Pokémon.
        /// </summary>
        public byte GetHiddenPowerBasePower()
        {
            const byte mininumBasePower = 30,
                maximumBasePower = 70;
            int a = (IVs[0] & 2) == 2 ? 1 : 0,
                b = (IVs[1] & 2) == 2 ? 1 : 0,
                c = (IVs[2] & 2) == 2 ? 1 : 0,
                d = (IVs[5] & 2) == 2 ? 1 : 0,
                e = (IVs[3] & 2) == 2 ? 1 : 0,
                f = (IVs[4] & 2) == 2 ? 1 : 0;
            return (byte)((((1 << 0) * a + (1 << 1) * b + (1 << 2) * c + (1 << 3) * d + (1 << 4) * e + (1 << 5) * f) * (maximumBasePower - mininumBasePower) / ((1 << 6) - 1)) + mininumBasePower);
        }
        /// <summary>
        /// Gets the type that a move will become when used by this Pokémon.
        /// </summary>
        /// <param name="move">The move to check.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="move"/> is invalid.</exception>
        public PBEType GetMoveType(PBEMove move)
        {
            if (move == PBEMove.None || move >= PBEMove.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            switch (move)
            {
                case PBEMove.HiddenPower:
                    {
                        return GetHiddenPowerType();
                    }
                case PBEMove.Judgment:
                    {
                        switch (Item)
                        {
                            case PBEItem.DracoPlate: return PBEType.Dragon;
                            case PBEItem.DreadPlate: return PBEType.Dark;
                            case PBEItem.EarthPlate: return PBEType.Ground;
                            case PBEItem.FistPlate: return PBEType.Fighting;
                            case PBEItem.FlamePlate: return PBEType.Fire;
                            case PBEItem.IciclePlate: return PBEType.Ice;
                            case PBEItem.InsectPlate: return PBEType.Bug;
                            case PBEItem.IronPlate: return PBEType.Steel;
                            case PBEItem.MeadowPlate: return PBEType.Grass;
                            case PBEItem.MindPlate: return PBEType.Psychic;
                            case PBEItem.SkyPlate: return PBEType.Flying;
                            case PBEItem.SplashPlate: return PBEType.Water;
                            case PBEItem.SpookyPlate: return PBEType.Ghost;
                            case PBEItem.StonePlate: return PBEType.Rock;
                            case PBEItem.ToxicPlate: return PBEType.Poison;
                            case PBEItem.ZapPlate: return PBEType.Electric;
                            default: return PBEMoveData.Data[PBEMove.Judgment].Type;
                        }
                    }
                case PBEMove.TechnoBlast:
                    {
                        switch (Item)
                        {
                            case PBEItem.BurnDrive: return PBEType.Fire;
                            case PBEItem.ChillDrive: return PBEType.Ice;
                            case PBEItem.DouseDrive: return PBEType.Water;
                            case PBEItem.ShockDrive: return PBEType.Electric;
                            default: return PBEMoveData.Data[PBEMove.TechnoBlast].Type;
                        }
                    }
                case PBEMove.WeatherBall:
                    {
                        switch (Team.Battle.Weather)
                        {
                            case PBEWeather.Hailstorm: return PBEType.Ice;
                            case PBEWeather.HarshSunlight: return PBEType.Fire;
                            case PBEWeather.Rain: return PBEType.Water;
                            case PBEWeather.Sandstorm: return PBEType.Rock;
                            default: return PBEMoveData.Data[PBEMove.WeatherBall].Type;
                        }
                    }
                default:
                    {
                        if (Ability == PBEAbility.Normalize)
                        {
                            return PBEType.Normal;
                        }
                        else
                        {
                            return PBEMoveData.Data[move].Type;
                        }
                    }
            }
        }
        /// <summary>
        /// Gets the possible targets that a move can target when used by this Pokémon.
        /// </summary>
        /// <param name="move">The move to check.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="move"/> is invalid.</exception>
        public PBEMoveTarget GetMoveTargets(PBEMove move)
        {
            if (move == PBEMove.None || move >= PBEMove.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            else if (move == PBEMove.Curse)
            {
                if (HasType(PBEType.Ghost))
                {
                    return PBEMoveTarget.SingleSurrounding;
                }
                else
                {
                    return PBEMoveTarget.Self;
                }
            }
            else
            {
                return PBEMoveData.Data[move].Targets;
            }
        }
        /// <summary>
        /// Returns True if the Pokémon is only able to use <see cref="PBEMove.Struggle"/>, False otherwise.
        /// </summary>
        public bool IsForcedToStruggle()
        {
            return
                (ChoiceLockedMove != PBEMove.None && PP[Array.IndexOf(Moves, ChoiceLockedMove)] == 0)
                || PP.All(p => p == 0);
        }
        /// <summary>
        /// Gets the chance of a protection move succeeding, out of <see cref="ushort.MaxValue"/>.
        /// </summary>
        public ushort GetProtectionChance()
        {
            ushort chance = ushort.MaxValue;
            for (int i = ExecutedMoves.Count - 1; i >= 0; i--)
            {
                PBEExecutedMove ex = ExecutedMoves[i];
                if ((ex.Move == PBEMove.Detect || ex.Move == PBEMove.Protect || ex.Move == PBEMove.WideGuard) && ex.Targets[0].FailReason == PBEFailReason.None)
                {
                    chance /= 2;
                }
                else
                {
                    break;
                }
            }
            return chance;
        }

        // ToBytes() and FromBytes() will only be used when the server sends you your team Ids, so they do not need to contain all info
        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.Add(Id);
            bytes.AddRange(Shell.ToBytes());
            return bytes.ToArray();
        }
        internal static PBEPokemon FromBytes(BinaryReader r, PBETeam team)
        {
            return new PBEPokemon(team, r.ReadByte(), PBEPokemonShell.FromBytes(r));
        }

        // Will only be accurate for the host
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Nickname}/{OriginalSpecies} {GenderSymbol} Lv.{Level}");
            sb.AppendLine($"HP: {HP}/{MaxHP} ({HPPercentage:P2})");
            sb.AppendLine($"Types: {Type1}/{Type2}");
            sb.AppendLine($"Position: {FieldPosition}");
            sb.AppendLine($"Status1: {Status1}");
            if (Status1 == PBEStatus1.Asleep)
            {
                sb.AppendLine($"Sleep turns: {Status1Counter}/{SleepTurns}");
            }
            else if (Status1 == PBEStatus1.BadlyPoisoned)
            {
                sb.AppendLine($"Toxic Counter: {Status1Counter}");
            }
            sb.AppendLine($"Status2: {Status2}");
            if (Status2.HasFlag(PBEStatus2.Confused))
            {
                sb.AppendLine($"Confusion turns: {ConfusionCounter}/{ConfusionTurns}");
            }
            if (Status2.HasFlag(PBEStatus2.Disguised))
            {
                sb.AppendLine($"Disguised as: {DisguisedAsPokemon.Nickname}");
            }
            if (Status2.HasFlag(PBEStatus2.LeechSeed))
            {
                sb.AppendLine($"Seeded position: {SeededPosition}");
            }
            if (Status2.HasFlag(PBEStatus2.Substitute))
            {
                sb.AppendLine($"Substitute HP: {SubstituteHP}");
            }
            sb.AppendLine($"Stats: [A] {Attack}, [D] {Defense}, [SA] {SpAttack}, [SD] {SpDefense}, [S] {Speed}, [W] {Weight:0.0}");
            PBEStat[] statChanges = GetChangedStats();
            if (statChanges.Length > 0)
            {
                var statStrs = new List<string>(7);
                if (Array.IndexOf(statChanges, PBEStat.Attack) != -1)
                {
                    statStrs.Add($"[A] x{PBEBattle.GetStatChangeModifier(AttackChange, false):0.00}");
                }
                if (Array.IndexOf(statChanges, PBEStat.Defense) != -1)
                {
                    statStrs.Add($"[D] x{PBEBattle.GetStatChangeModifier(DefenseChange, false):0.00}");
                }
                if (Array.IndexOf(statChanges, PBEStat.SpAttack) != -1)
                {
                    statStrs.Add($"[SA] x{PBEBattle.GetStatChangeModifier(SpAttackChange, false):0.00}");
                }
                if (Array.IndexOf(statChanges, PBEStat.SpDefense) != -1)
                {
                    statStrs.Add($"[SD] x{PBEBattle.GetStatChangeModifier(SpDefenseChange, false):0.00}");
                }
                if (Array.IndexOf(statChanges, PBEStat.Speed) != -1)
                {
                    statStrs.Add($"[S] x{PBEBattle.GetStatChangeModifier(SpeedChange, false):0.00}");
                }
                if (Array.IndexOf(statChanges, PBEStat.Accuracy) != -1)
                {
                    statStrs.Add($"[AC] x{PBEBattle.GetStatChangeModifier(AccuracyChange, true):0.00}");
                }
                if (Array.IndexOf(statChanges, PBEStat.Evasion) != -1)
                {
                    statStrs.Add($"[E] x{PBEBattle.GetStatChangeModifier(EvasionChange, true):0.00}");
                }
                sb.AppendLine($"Stat changes: {string.Join(", ", statStrs)}");
            }
            sb.AppendLine($"Ability: {PBEAbilityLocalization.Names[Ability].English}");
            sb.AppendLine($"Known ability: {(KnownAbility == PBEAbility.MAX ? "???" : PBEAbilityLocalization.Names[KnownAbility].English)}");
            sb.AppendLine($"Item: {PBEItemLocalization.Names[Item].English}");
            sb.AppendLine($"Known item: {(KnownItem == (PBEItem)ushort.MaxValue ? "???" : PBEItemLocalization.Names[KnownItem].English)}");
            // TODO: Usable moves
            if (Moves.Contains(PBEMove.HiddenPower))
            {
                sb.AppendLine($"Hidden Power: {GetHiddenPowerType()}/{GetHiddenPowerBasePower()}");
            }
            string[] moveStrs = new string[Moves.Length];
            for (int i = 0; i < moveStrs.Length; i++)
            {
                moveStrs[i] = $"{PBEMoveLocalization.Names[Moves[i]].English} {PP[i]}/{MaxPP[i]}";
            }
            sb.AppendLine($"Moves: {string.Join(", ", moveStrs)}");
            sb.Append($"Known moves: {string.Join(", ", KnownMoves.Select(m => m == PBEMove.MAX ? "???" : PBEMoveLocalization.Names[m].English))}");
            return sb.ToString();
        }
    }
}
