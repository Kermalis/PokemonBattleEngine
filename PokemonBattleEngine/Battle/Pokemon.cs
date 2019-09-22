using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Battle
{
    // TODO: INPC
    /// <summary>Represents a specific Pokémon during a battle.</summary>
    public sealed class PBEPokemon
    {
        /// <summary>The team this Pokémon belongs to in its battle.</summary>
        public PBETeam Team { get; }
        /// <summary>The Pokémon's ID in its battle.</summary>
        public byte Id { get; }

        /// <summary>The Pokémon's <see cref="Nickname"/> with its <see cref="GenderSymbol"/> attached.</summary>
        public string NameWithGender => Nickname + GenderSymbol;
        /// <summary>The Pokémon's <see cref="Gender"/> as a <see cref="string"/>.</summary>
        public string GenderSymbol => Gender == PBEGender.Female ? "♀" : Gender == PBEGender.Male ? "♂" : string.Empty;
        /// <summary>The Pokémon's <see cref="KnownNickname"/> with its <see cref="KnownGenderSymbol"/> attached.</summary>
        public string KnownNameWithKnownGender => KnownNickname + KnownGenderSymbol;
        /// <summary>The Pokémon's <see cref="KnownGender"/> as a <see cref="string"/>.</summary>
        public string KnownGenderSymbol => KnownGender == PBEGender.Female ? "♀" : KnownGender == PBEGender.Male ? "♂" : string.Empty;

        /// <summary>The Pokémon's current HP.</summary>
        public ushort HP { get; set; }
        /// <summary>The Pokémon's maximum HP.</summary>
        public ushort MaxHP { get; set; }
        /// <summary>The Pokémon's current HP as a percentage.</summary>
        public double HPPercentage { get; set; }
        /// <summary>The Pokémon's attack stat.</summary>
        public ushort Attack { get; set; }
        public sbyte AttackChange { get; set; }
        /// <summary>The Pokémon's defense stat.</summary>
        public ushort Defense { get; set; }
        public sbyte DefenseChange { get; set; }
        /// <summary>The Pokémon's special attack stat.</summary>
        public ushort SpAttack { get; set; }
        public sbyte SpAttackChange { get; set; }
        /// <summary>The Pokémon's special defense stat.</summary>
        public ushort SpDefense { get; set; }
        public sbyte SpDefenseChange { get; set; }
        /// <summary>The Pokémon's speed stat.</summary>
        public ushort Speed { get; set; }
        public sbyte SpeedChange { get; set; }
        public sbyte AccuracyChange { get; set; }
        public sbyte EvasionChange { get; set; }
        public PBEEffortValues EffortValues { get; }
        public PBEIndividualValues IndividualValues { get; }
        public byte Friendship { get; set; }
        /// <summary>The Pokémon's level.</summary>
        public byte Level { get; set; }
        /// <summary>The Pokémon's nature.</summary>
        public PBENature Nature { get; set; }
        /// <summary>The moveset the Pokémon had upon entering battle.</summary>
        public PBEMoveset OriginalMoveset { get; set; }

        /// <summary>The Pokémon's field position.</summary>
        public PBEFieldPosition FieldPosition { get; set; }
        /// <summary>The Pokémon's current ability.</summary>
        public PBEAbility Ability { get; set; }
        /// <summary>The ability the Pokémon had upon entering battle. </summary>
        public PBEAbility OriginalAbility { get; set; }
        /// <summary>The ability the Pokémon is known to have.</summary>
        public PBEAbility KnownAbility { get; set; }
        /// <summary>The Pokémon's gender.</summary>
        public PBEGender Gender { get; set; }
        /// <summary>The gender the Pokémon looks like (affected by transforming and disguising).</summary>
        public PBEGender KnownGender { get; set; }
        /// <summary>The Pokémon's current item.</summary>
        public PBEItem Item { get; set; }
        /// <summary>The item the Pokémon had upon entering battle.</summary>
        public PBEItem OriginalItem { get; set; }
        /// <summary>The item the Pokémon is known to have.</summary>
        public PBEItem KnownItem { get; set; }
        /// <summary>The moves the Pokémon currently has.</summary>
        public PBEBattleMoveset Moves { get; }
        /// <summary>The moves the Pokémon is known to have.</summary>
        public PBEBattleMoveset KnownMoves { get; }
        /// <summary>The nickname the Pokémon normally has.</summary>
        public string Nickname { get; set; }
        /// <summary>The nickname the Pokémon is known to have.</summary>
        public string KnownNickname { get; set; }
        /// <summary>The shininess the Pokémon normally has.</summary>
        public bool Shiny { get; set; }
        /// <summary>The shininess everyone sees the Pokémon has.</summary>
        public bool KnownShiny { get; set; }
        /// <summary>The current species of the Pokémon (affected by transforming and form changing).</summary>
        public PBESpecies Species { get; set; }
        /// <summary>The species the Pokémon was upon entering battle.</summary>
        public PBESpecies OriginalSpecies { get; set; }
        /// <summary>The species everyone sees the Pokémon as (affected by transforming, disguising, and form changing).</summary>
        public PBESpecies KnownSpecies { get; set; }
        public PBEStatus1 Status1 { get; set; }
        public PBEStatus2 Status2 { get; set; }
        /// <summary>The Pokémon's first type.</summary>
        public PBEType Type1 { get; set; }
        /// <summary>The first type everyone believes the Pokémon has.</summary>
        public PBEType KnownType1 { get; set; }
        /// <summary>The Pokémon's second type.</summary>
        public PBEType Type2 { get; set; }
        /// <summary>The second type everyone believes the Pokémon has.</summary>
        public PBEType KnownType2 { get; set; }
        public double Weight { get; set; }
        public double KnownWeight { get; set; }

        #region Statuses
        /// <summary>The counter used for <see cref="PBEStatus1.BadlyPoisoned"/> and <see cref="PBEStatus1.Asleep"/>.</summary>
        public byte Status1Counter { get; set; }
        /// <summary>The amount of turns the Pokémon will sleep for before waking.</summary>
        public byte SleepTurns { get; set; }
        /// <summary>The counter used for <see cref="PBEStatus2.Confused"/>.</summary>
        public byte ConfusionCounter { get; set; }
        /// <summary>The amount of turns the Pokémon will be confused for before snapping out of it.</summary>
        public byte ConfusionTurns { get; set; }
        /// <summary>The Pokémon that <see cref="PBEStatus2.Disguised"/> is disguised as.</summary>
        public PBEPokemon DisguisedAsPokemon { get; set; }
        /// <summary>The Pokémon that <see cref="PBEStatus2.Infatuated"/> is bound to.</summary>
        public PBEPokemon InfatuatedWithPokemon { get; set; }
        /// <summary>The position to return <see cref="PBEStatus2.LeechSeed"/> HP to on <see cref="SeededTeam"/>.</summary>
        public PBEFieldPosition SeededPosition { get; set; }
        /// <summary>The team responsible for <see cref="PBEStatus2.LeechSeed"/>.</summary>
        public PBETeam SeededTeam { get; set; }
        /// <summary>The amount of HP the Pokémon's <see cref="PBEStatus2.Substitute"/> has left.</summary>
        public ushort SubstituteHP { get; set; }
        public PBEBattleMoveset TransformBackupMoves { get; }
        #endregion

        #region Actions
        /// <summary>True if the Pokémon has successfully executed a move this turn.</summary>
        public bool HasUsedMoveThisTurn { get; set; }
        /// <summary>The action the Pokémon will try to perform when the turn is run.</summary>
        public PBETurnAction TurnAction { get; set; }
        /// <summary>The move the Pokémon is forced to use by multi-turn moves.</summary>
        public PBEMove TempLockedMove { get; set; }
        /// <summary>The targets the Pokémon is forced to target by multi-turn moves.</summary>
        public PBETurnTarget TempLockedTargets { get; set; }
        /// <summary>The move the Pokémon is forced to use by its choice item.</summary>
        public PBEMove ChoiceLockedMove { get; set; }
        #endregion

        #region Special Flags
        /// <summary>True if the Pokémon has successfully used <see cref="PBEMove.Minimize"/> which makes it succeptible to double damage from <see cref="PBEMove.Steamroller"/> and <see cref="PBEMove.Stomp"/>.</summary>
        public bool Minimize_Used { get; set; }
        /// <summary>The amount of times the Pokémon has successfully used <see cref="PBEMove.Detect"/>, <see cref="PBEMove.Protect"/>, <see cref="PBEMove.QuickGuard"/>, and/or <see cref="PBEMove.WideGuard"/> consecutively.</summary>
        public int Protection_Counter { get; set; }
        /// <summary>True if the Pokémon was originally <see cref="PBESpecies.Shaymin_Sky"/> but was <see cref="PBEStatus1.Frozen"/>, therefore forcing it to remain as <see cref="PBESpecies.Shaymin"/> when switching out.</summary>
        public bool Shaymin_CannotChangeBackToSkyForm { get; set; }
        /// <summary>The amount of turns left until a Pokémon with <see cref="PBEAbility.SlowStart"/> loses its hinderance.</summary>
        public byte SlowStart_HinderTurnsLeft { get; set; }
        /// <summary>True if the Pokémon was present at the start of the turn, which would allow <see cref="PBEAbility.SpeedBoost"/> to activate.</summary>
        public bool SpeedBoost_AbleToSpeedBoostThisTurn { get; set; }
        #endregion

        internal PBEPokemon(BinaryReader r, PBETeam team)
        {
            Team = team;
            Id = r.ReadByte();
            Species = OriginalSpecies = KnownSpecies = (PBESpecies)r.ReadUInt32();
            var pData = PBEPokemonData.GetData(Species);
            KnownType1 = Type1 = pData.Type1;
            KnownType2 = Type2 = pData.Type2;
            KnownWeight = Weight = pData.Weight;
            Nickname = KnownNickname = PBEUtils.StringFromBytes(r);
            Level = r.ReadByte();
            Friendship = r.ReadByte();
            Shiny = KnownShiny = r.ReadBoolean();
            Ability = OriginalAbility = (PBEAbility)r.ReadByte();
            KnownAbility = PBEAbility.MAX;
            Nature = (PBENature)r.ReadByte();
            Gender = KnownGender = (PBEGender)r.ReadByte();
            Item = OriginalItem = (PBEItem)r.ReadUInt16();
            KnownItem = (PBEItem)ushort.MaxValue;
            EffortValues = new PBEEffortValues(Team.Battle.Settings, r);
            IndividualValues = new PBEIndividualValues(Team.Battle.Settings, r);
            SetStats(true);
            OriginalMoveset = new PBEMoveset(Species, Level, Team.Battle.Settings, r);
            Moves = new PBEBattleMoveset(OriginalMoveset);
            KnownMoves = new PBEBattleMoveset(Team.Battle.Settings);
            TransformBackupMoves = new PBEBattleMoveset(Team.Battle.Settings);
            Team.Party.Add(this);
        }
        internal PBEPokemon(PBETeam team, byte id, PBEPokemonShell shell)
        {
            Team = team;
            Id = id;
            Species = OriginalSpecies = KnownSpecies = shell.Species;
            var pData = PBEPokemonData.GetData(Species);
            KnownType1 = Type1 = pData.Type1;
            KnownType2 = Type2 = pData.Type2;
            KnownWeight = Weight = pData.Weight;
            Nickname = KnownNickname = shell.Nickname;
            Level = shell.Level;
            Friendship = shell.Friendship;
            Shiny = KnownShiny = shell.Shiny;
            Ability = OriginalAbility = shell.Ability;
            KnownAbility = PBEAbility.MAX;
            Nature = shell.Nature;
            Gender = KnownGender = shell.Gender;
            Item = OriginalItem = shell.Item;
            KnownItem = (PBEItem)ushort.MaxValue;
            EffortValues = new PBEEffortValues(shell.EffortValues);
            IndividualValues = new PBEIndividualValues(shell.IndividualValues);
            SetStats(true);
            OriginalMoveset = new PBEMoveset(shell.Moveset);
            Moves = new PBEBattleMoveset(OriginalMoveset);
            KnownMoves = new PBEBattleMoveset(Team.Battle.Settings);
            TransformBackupMoves = new PBEBattleMoveset(Team.Battle.Settings);
            team.Party.Add(this);
        }
        // This constructor is to define a remote Pokémon
        public PBEPokemon(PBETeam team, PBEPkmnSwitchInPacket.PBESwitchInInfo info)
        {
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            Team = team;
            Id = info.PokemonId;
            FieldPosition = info.FieldPosition;
            HP = info.HP;
            MaxHP = info.MaxHP;
            HPPercentage = info.HPPercentage;
            Status1 = info.Status1;
            Level = info.Level;
            KnownAbility = Ability = OriginalAbility = PBEAbility.MAX;
            KnownGender = Gender = info.Gender;
            KnownItem = Item = OriginalItem = (PBEItem)ushort.MaxValue;
            Moves = new PBEBattleMoveset(Team.Battle.Settings);
            KnownMoves = new PBEBattleMoveset(Team.Battle.Settings);
            TransformBackupMoves = new PBEBattleMoveset(Team.Battle.Settings);
            KnownNickname = Nickname = info.Nickname;
            KnownShiny = Shiny = info.Shiny;
            KnownSpecies = Species = OriginalSpecies = info.Species;
            var pData = PBEPokemonData.GetData(KnownSpecies);
            KnownType1 = Type1 = pData.Type1;
            KnownType2 = Type2 = pData.Type2;
            KnownWeight = Weight = pData.Weight;
            Team.Party.Add(this);
        }

        public void ApplyPowerTrickChange()
        {
            ushort a = Attack;
            Attack = Defense;
            Defense = a;
        }
        /// <summary>Sets and clears all information required for switching out.</summary>
        public void ClearForSwitch()
        {
            FieldPosition = PBEFieldPosition.None;
            switch (Ability)
            {
                case PBEAbility.NaturalCure:
                {
                    Status1 = PBEStatus1.None;
                    Status1Counter = 0;
                    SleepTurns = 0;
                    break;
                }
                case PBEAbility.Regenerator:
                {
                    HP = PBEUtils.Clamp((ushort)(HP + (MaxHP / 3)), ushort.MinValue, MaxHP);
                    HPPercentage = (double)HP / MaxHP;
                    break;
                }
            }
            PBEPokemonData pData;
            if (Shaymin_CannotChangeBackToSkyForm)
            {
                pData = PBEPokemonData.GetData(Species = KnownSpecies = PBESpecies.Shaymin);
                Ability = pData.Abilities[0];
            }
            else
            {
                pData = PBEPokemonData.GetData(Species = KnownSpecies = OriginalSpecies);
                Ability = OriginalAbility;
            }
            KnownAbility = PBEAbility.MAX;
            KnownGender = Gender;
            KnownItem = (PBEItem)ushort.MaxValue;
            KnownMoves.SetUnknown();
            KnownNickname = Nickname;
            KnownShiny = Shiny;
            KnownType1 = Type1 = pData.Type1;
            KnownType2 = Type2 = pData.Type2;

            AttackChange = DefenseChange = SpAttackChange = SpDefenseChange = SpeedChange = AccuracyChange = EvasionChange = 0;

            if (Status1 == PBEStatus1.Asleep)
            {
                Status1Counter = 0;
            }
            else if (Status1 == PBEStatus1.BadlyPoisoned)
            {
                Status1Counter = 1;
            }

            ConfusionCounter = 0;
            ConfusionTurns = 0;
            DisguisedAsPokemon = null;
            SeededPosition = PBEFieldPosition.None;
            SeededTeam = null;
            SubstituteHP = 0;
            if (Status2.HasFlag(PBEStatus2.Transformed))
            {
                Moves.Reset(TransformBackupMoves);
                TransformBackupMoves.SetUnknown();
            }
            Status2 = PBEStatus2.None;

            HasUsedMoveThisTurn = false;
            TurnAction = null;
            TempLockedMove = PBEMove.None;
            TempLockedTargets = PBETurnTarget.None;
            ChoiceLockedMove = PBEMove.None;

            Minimize_Used = false;
            Protection_Counter = 0;
            SlowStart_HinderTurnsLeft = 0;
            SpeedBoost_AbleToSpeedBoostThisTurn = false;

            SetStats(false);
        }
        public void SetStats(bool calculateHP)
        {
            PBESpecies species = Species;
            PBENature nature = Nature;
            PBEEffortValues evs = EffortValues;
            PBEIndividualValues ivs = IndividualValues;
            byte level = Level;
            PBESettings settings = Team.Battle.Settings;
            if (calculateHP)
            {
                ushort hp = PBEPokemonData.CalculateStat(PBEStat.HP, species, nature, evs[PBEStat.HP].Value, ivs[PBEStat.HP].Value, level, settings);
                MaxHP = hp;
                HP = hp;
                HPPercentage = 1d;
            }
            Attack = PBEPokemonData.CalculateStat(PBEStat.Attack, species, nature, evs[PBEStat.Attack].Value, ivs[PBEStat.Attack].Value, level, settings);
            Defense = PBEPokemonData.CalculateStat(PBEStat.Defense, species, nature, evs[PBEStat.Defense].Value, ivs[PBEStat.Defense].Value, level, settings);
            SpAttack = PBEPokemonData.CalculateStat(PBEStat.SpAttack, species, nature, evs[PBEStat.SpAttack].Value, ivs[PBEStat.SpAttack].Value, level, settings);
            SpDefense = PBEPokemonData.CalculateStat(PBEStat.SpDefense, species, nature, evs[PBEStat.SpDefense].Value, ivs[PBEStat.SpDefense].Value, level, settings);
            Speed = PBEPokemonData.CalculateStat(PBEStat.Speed, species, nature, evs[PBEStat.Speed].Value, ivs[PBEStat.Speed].Value, level, settings);
        }
        /// <summary>Transforms into <paramref name="target"/> and sets <see cref="PBEStatus2.Transformed"/>.</summary>
        /// <param name="target">The Pokémon to transform into.</param>
        public void Transform(PBEPokemon target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
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
            TransformBackupMoves.Reset(Moves);
            PBEBattleMoveset.DoTransform(this, target);
            if (!Moves.Contains(ChoiceLockedMove))
            {
                ChoiceLockedMove = PBEMove.None;
            }
            Status2 |= PBEStatus2.Transformed;
            Status2 &= ~PBEStatus2.PowerTrick;
        }
        public void UpdateKnownPP(PBEMove move, int amountReduced)
        {
            if (move == PBEMove.None || move >= PBEMove.MAX || !Enum.IsDefined(typeof(PBEMove), move))
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            PBEBattleMoveset.PBEBattleMovesetSlot knownSlot = KnownMoves[move];
            knownSlot.PP += amountReduced;
            if (knownSlot.MaxPP == 0)
            {
                if (Status2.HasFlag(PBEStatus2.Transformed))
                {
                    knownSlot.MaxPP = PBEBattleMoveset.GetTransformPP(Team.Battle.Settings, move);
                }
                else if (Team.Battle.Settings.MaxPPUps == 0 || knownSlot.PP > PBEBattleMoveset.GetNonTransformPP(Team.Battle.Settings, move, (byte)(Team.Battle.Settings.MaxPPUps - 1)))
                {
                    knownSlot.MaxPP = PBEBattleMoveset.GetNonTransformPP(Team.Battle.Settings, move, Team.Battle.Settings.MaxPPUps);
                }
            }
        }

        /// <summary>Returns True if the Pokémon has <paramref name="type"/>, False otherwise.</summary>
        /// <param name="type">The type to check.</param>
        public bool HasType(PBEType type)
        {
            if (type >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }
            return Type1 == type || Type2 == type;
        }
        public bool HasCancellingAbility()
        {
            return Ability == PBEAbility.MoldBreaker || Ability == PBEAbility.Teravolt || Ability == PBEAbility.Turboblaze;
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
        public sbyte GetStatChange(PBEStat stat)
        {
            switch (stat)
            {
                case PBEStat.Attack: return AttackChange;
                case PBEStat.Defense: return DefenseChange;
                case PBEStat.SpAttack: return SpAttackChange;
                case PBEStat.SpDefense: return SpDefenseChange;
                case PBEStat.Speed: return SpeedChange;
                case PBEStat.Accuracy: return AccuracyChange;
                case PBEStat.Evasion: return EvasionChange;
                default: throw new ArgumentOutOfRangeException(nameof(stat));
            }
        }
        public sbyte SetStatChange(PBEStat stat, int value)
        {
            sbyte val = (sbyte)PBEUtils.Clamp(value, -Team.Battle.Settings.MaxStatChange, Team.Battle.Settings.MaxStatChange);
            switch (stat)
            {
                case PBEStat.Accuracy: return AccuracyChange = val;
                case PBEStat.Attack: return AttackChange = val;
                case PBEStat.Defense: return DefenseChange = val;
                case PBEStat.Evasion: return EvasionChange = val;
                case PBEStat.SpAttack: return SpAttackChange = val;
                case PBEStat.SpDefense: return SpDefenseChange = val;
                case PBEStat.Speed: return SpeedChange = val;
                default: throw new ArgumentOutOfRangeException(nameof(stat));
            }
        }
        /// <summary>Gets the type that a move will become when used by this Pokémon.</summary>
        /// <param name="move">The move to check.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="move"/> is invalid.</exception>
        public PBEType GetMoveType(PBEMove move)
        {
            if (move == PBEMove.None || move >= PBEMove.MAX || !Enum.IsDefined(typeof(PBEMove), move))
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            switch (move)
            {
                case PBEMove.HiddenPower:
                {
                    return IndividualValues.HiddenPowerType;
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
                    if (Team.Battle.ShouldDoWeatherEffects())
                    {
                        switch (Team.Battle.Weather)
                        {
                            case PBEWeather.Hailstorm: return PBEType.Ice;
                            case PBEWeather.HarshSunlight: return PBEType.Fire;
                            case PBEWeather.Rain: return PBEType.Water;
                            case PBEWeather.Sandstorm: return PBEType.Rock;
                        }
                    }
                    return PBEMoveData.Data[PBEMove.WeatherBall].Type;
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
        /// <summary>Gets the possible targets that a move can target when used by this Pokémon.</summary>
        /// <param name="move">The move to check.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="move"/> is invalid.</exception>
        public PBEMoveTarget GetMoveTargets(PBEMove move)
        {
            if (move == PBEMove.None || move >= PBEMove.MAX || !Enum.IsDefined(typeof(PBEMove), move))
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
        /// <summary>Returns True if the Pokémon is only able to use <see cref="PBEMove.Struggle"/>.</summary>
        public bool IsForcedToStruggle()
        {
            if (TempLockedMove != PBEMove.None) // Temp locked moves deduct PP on the first turn and don't on the second, so having a temp locked move means it is supposed to be used again for the second turn
            {
                return false;
            }
            else if ((ChoiceLockedMove != PBEMove.None && Moves[ChoiceLockedMove].PP == 0) // If the choice locked move has 0 pp, it is forced to struggle
                || Moves.All(s => s.PP == 0) // If all moves have 0 pp, then the user is forced to struggle
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>Returns True if the Pokémon can switch out. Does not check if the Pokémon is on the field or if there are available Pokémon to switch into.</summary>
        public bool CanSwitchOut()
        {
            return TempLockedMove == PBEMove.None;
        }
        // TODO: Make different public versions that use Known*? AIs should not be able to cheat
        public bool IsBurnPossible(PBEPokemon considerer)
        {
            if (considerer == null)
            {
                throw new ArgumentNullException(nameof(considerer));
            }
            return Status1 == PBEStatus1.None
                && !HasType(PBEType.Fire)
                && !((Ability == PBEAbility.WaterVeil || (Ability == PBEAbility.LeafGuard && Team.Battle.WillLeafGuardActivate())) && !considerer.HasCancellingAbility());
        }
        public bool IsConfusionPossible(PBEPokemon considerer)
        {
            if (considerer == null)
            {
                throw new ArgumentNullException(nameof(considerer));
            }
            return !Status2.HasFlag(PBEStatus2.Confused)
                && !(Ability == PBEAbility.OwnTempo && !considerer.HasCancellingAbility());
        }
        public bool IsFreezePossible(PBEPokemon considerer)
        {
            if (considerer == null)
            {
                throw new ArgumentNullException(nameof(considerer));
            }
            return Status1 == PBEStatus1.None
                && !HasType(PBEType.Ice)
                && !((Ability == PBEAbility.MagmaArmor || (Ability == PBEAbility.LeafGuard && Team.Battle.WillLeafGuardActivate())) && !considerer.HasCancellingAbility());
        }
        public bool IsInfatuationPossible(PBEPokemon considerer)
        {
            if (considerer == null)
            {
                throw new ArgumentNullException(nameof(considerer));
            }
            return !Status2.HasFlag(PBEStatus2.Infatuated)
                && ((Gender == PBEGender.Male && considerer.Gender == PBEGender.Female) || (Gender == PBEGender.Female && considerer.Gender == PBEGender.Male))
                && !(Ability == PBEAbility.Oblivious && !considerer.HasCancellingAbility());
        }
        public bool IsParalysisPossible(PBEPokemon considerer)
        {
            if (considerer == null)
            {
                throw new ArgumentNullException(nameof(considerer));
            }
            return Status1 == PBEStatus1.None
                && !((Ability == PBEAbility.Limber || (Ability == PBEAbility.LeafGuard && Team.Battle.WillLeafGuardActivate())) && !considerer.HasCancellingAbility());
        }
        public bool IsPoisonPossible(PBEPokemon considerer)
        {
            if (considerer == null)
            {
                throw new ArgumentNullException(nameof(considerer));
            }
            return Status1 == PBEStatus1.None
                && !HasType(PBEType.Poison)
                && !HasType(PBEType.Steel)
                && !((Ability == PBEAbility.Immunity || (Ability == PBEAbility.LeafGuard && Team.Battle.WillLeafGuardActivate())) && !considerer.HasCancellingAbility());
        }
        public bool IsSleepPossible(PBEPokemon considerer)
        {
            if (considerer == null)
            {
                throw new ArgumentNullException(nameof(considerer));
            }
            return Status1 == PBEStatus1.None
                && !((Ability == PBEAbility.Insomnia || Ability == PBEAbility.VitalSpirit || (Ability == PBEAbility.LeafGuard && Team.Battle.WillLeafGuardActivate())) && !considerer.HasCancellingAbility());
        }
        public bool IsFlinchPossible(PBEPokemon considerer)
        {
            if (considerer == null)
            {
                throw new ArgumentNullException(nameof(considerer));
            }
            return !Status2.HasFlag(PBEStatus2.Flinching)
                && !(Ability == PBEAbility.InnerFocus && !considerer.HasCancellingAbility());
        }
        public bool IsGrounded(PBEPokemon considerer)
        {
            return HasType(PBEType.Flying)
                || (Ability == PBEAbility.Levitate && !considerer.HasCancellingAbility());
        }
        /// <summary>Returns an array of moves the Pokémon can use.</summary>
        public PBEMove[] GetUsableMoves()
        {
            if (IsForcedToStruggle())
            {
                return new PBEMove[1] { PBEMove.Struggle };
            }
            else if (TempLockedMove != PBEMove.None)
            {
                return new PBEMove[1] { TempLockedMove };
            }
            else if (ChoiceLockedMove != PBEMove.None)
            {
                return new PBEMove[1] { ChoiceLockedMove }; // IsForcedToStruggle() being false means the choice locked move still has PP
            }
            else
            {
                int numMoves = Moves.Count;
                var usableMoves = new List<PBEMove>(numMoves);
                for (int i = 0; i < numMoves; i++)
                {
                    PBEBattleMoveset.PBEBattleMovesetSlot slot = Moves[i];
                    if (slot.PP > 0)
                    {
                        usableMoves.Add(slot.Move);
                    }
                }
                return usableMoves.ToArray();
            }
        }
        /// <summary>Gets the chance of a protection move succeeding (based on <see cref="Protection_Counter"/>) out of <see cref="ushort.MaxValue"/>.</summary>
        public ushort GetProtectionChance()
        {
            int count = Protection_Counter;
            return count == 0 ? ushort.MaxValue : (ushort)(ushort.MaxValue / (count * 2));
        }

        internal void ToBytes(List<byte> bytes)
        {
            bytes.Add(Id);
            bytes.AddRange(BitConverter.GetBytes((uint)OriginalSpecies));
            PBEUtils.StringToBytes(bytes, Nickname);
            bytes.Add(Level);
            bytes.Add(Friendship);
            bytes.Add((byte)(Shiny ? 1 : 0));
            bytes.Add((byte)OriginalAbility);
            bytes.Add((byte)Nature);
            bytes.Add((byte)Gender);
            bytes.AddRange(BitConverter.GetBytes((ushort)OriginalItem));
            EffortValues.ToBytes(bytes);
            IndividualValues.ToBytes(bytes);
            OriginalMoveset.ToBytes(bytes);
        }

        // Will only be accurate for the host
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Nickname}/{Species} {GenderSymbol} Lv.{Level}");
            sb.AppendLine($"HP: {HP}/{MaxHP} ({HPPercentage:P2})");
            sb.Append($"Types: {PBELocalizedString.GetTypeName(Type1).English}");
            if (Type2 != PBEType.None)
            {
                sb.Append($"/{PBELocalizedString.GetTypeName(Type2).English}");
            }
            sb.AppendLine();
            sb.Append($"Known types: {PBELocalizedString.GetTypeName(KnownType1).English}");
            if (KnownType2 != PBEType.None)
            {
                sb.Append($"/{PBELocalizedString.GetTypeName(KnownType2).English}");
            }
            sb.AppendLine();
            sb.AppendLine($"Position: {Team.TrainerName}'s {FieldPosition}");
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
            if (Status2.HasFlag(PBEStatus2.Infatuated))
            {
                sb.AppendLine($"Infatuated with: {InfatuatedWithPokemon.Nickname}");
            }
            if (Status2.HasFlag(PBEStatus2.LeechSeed))
            {
                sb.AppendLine($"Seeded position: {SeededTeam.TrainerName}'s {SeededPosition}");
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
            sb.AppendLine($"Ability: {PBELocalizedString.GetAbilityName(Ability).English}");
            sb.AppendLine($"Known ability: {(KnownAbility == PBEAbility.MAX ? "???" : PBELocalizedString.GetAbilityName(KnownAbility).English)}");
            sb.AppendLine($"Item: {PBELocalizedString.GetItemName(Item).English}");
            sb.AppendLine($"Known item: {(KnownItem == (PBEItem)ushort.MaxValue ? "???" : PBELocalizedString.GetItemName(KnownItem).English)}");
            if (Moves.Contains(PBEMove.Frustration) || Moves.Contains(PBEMove.Return))
            {
                sb.AppendLine($"Friendship: {Friendship} ({Friendship / byte.MaxValue:P2})");
            }
            if (Moves.Contains(PBEMove.HiddenPower))
            {
                sb.AppendLine($"{PBELocalizedString.GetMoveName(PBEMove.HiddenPower).English}: {PBELocalizedString.GetTypeName(IndividualValues.HiddenPowerType).English}:{IndividualValues.HiddenPowerBasePower}");
            }
            sb.Append("Moves: ");
            for (int i = 0; i < Team.Battle.Settings.NumMoves; i++)
            {
                PBEBattleMoveset.PBEBattleMovesetSlot slot = Moves[i];
                PBEMove move = slot.Move;
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(PBELocalizedString.GetMoveName(slot.Move).English);
                if (move != PBEMove.None)
                {
                    sb.Append($" ({slot.PP}/{slot.MaxPP})");
                }
            }
            sb.AppendLine();
            sb.Append("Known moves: ");
            for (int i = 0; i < Team.Battle.Settings.NumMoves; i++)
            {
                PBEBattleMoveset.PBEBattleMovesetSlot slot = KnownMoves[i];
                PBEMove move = slot.Move;
                int pp = slot.PP;
                int maxPP = slot.MaxPP;
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(move == PBEMove.MAX ? "???" : PBELocalizedString.GetMoveName(move).English);
                if (move != PBEMove.None && move != PBEMove.MAX)
                {
                    sb.Append($" ({pp}{(maxPP == 0 ? ")" : $"/{maxPP})")}");
                }
            }
            sb.AppendLine();
            sb.Append($"Usable moves: {string.Join(", ", GetUsableMoves().Select(m => PBELocalizedString.GetMoveName(m).English))}");
            return sb.ToString();
        }

        internal bool IsDisposed { get; private set; }
        internal void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                if (Id != byte.MaxValue)
                {
                    EffortValues.CanDispose = true;
                    EffortValues.Dispose();
                    IndividualValues.CanDispose = true;
                    IndividualValues.Dispose();
                    OriginalMoveset.CanDispose = true;
                    OriginalMoveset.Dispose();
                }
            }
        }
    }
}
