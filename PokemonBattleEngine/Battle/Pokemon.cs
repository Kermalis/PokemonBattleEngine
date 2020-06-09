using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Battle
{
    // TODO: INPC
    /// <summary>Represents a specific Pokémon during a battle.</summary>
    public sealed class PBEPokemon : IPBEPokemonTypes
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
        /// <summary>The ability the Pokémon is known to have.</summary>
        public PBEAbility KnownAbility { get; set; }
        /// <summary>The ability the Pokémon had upon entering battle. </summary>
        public PBEAbility OriginalAbility { get; set; }
        /// <summary>The ability the Pokémon will regain upon switching out, fainting, or the battle ending. </summary>
        public PBEAbility RevertAbility { get; set; }
        /// <summary>The Pokémon's gender.</summary>
        public PBEGender Gender { get; set; }
        /// <summary>The gender the Pokémon looks like (affected by transforming and disguising).</summary>
        public PBEGender KnownGender { get; set; }
        /// <summary>The Pokémon's current item.</summary>
        public PBEItem Item { get; set; }
        /// <summary>The item the Pokémon is known to have.</summary>
        public PBEItem KnownItem { get; set; }
        /// <summary>The item the Pokémon had upon entering battle.</summary>
        public PBEItem OriginalItem { get; set; }
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
        /// <summary>The species everyone sees the Pokémon as (affected by transforming, disguising, and form changing).</summary>
        public PBESpecies KnownSpecies { get; set; }
        /// <summary>The species the Pokémon was upon entering battle.</summary>
        public PBESpecies OriginalSpecies { get; set; }
        public PBEForm Form { get; set; }
        public PBEForm KnownForm { get; set; }
        public PBEForm OriginalForm { get; set; }
        public PBEForm RevertForm { get; set; }
        public PBEStatus1 Status1 { get; set; }
        public PBEStatus2 Status2 { get; set; }
        public PBEStatus2 KnownStatus2 { get; set; }
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
        /// <summary>The amount of turns until <see cref="PBEStatus2.MagnetRise"/> ends.</summary>
        public byte MagnetRiseTurns { get; set; }
        /// <summary>The Pokémon that <see cref="PBEStatus2.LockOn"/> is bound to.</summary>
        public PBEPokemon LockOnPokemon { get; set; }
        public byte LockOnTurns { get; set; }
        /// <summary>The amount of times the Pokémon has successfully used <see cref="PBEMove.Detect"/>, <see cref="PBEMove.Protect"/>, <see cref="PBEMove.QuickGuard"/>, and/or <see cref="PBEMove.WideGuard"/> consecutively.</summary>
        public int Protection_Counter { get; set; }
        public bool Protection_Used { get; set; }
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
        /// <summary>The amount of turns left until a Pokémon with <see cref="PBEAbility.SlowStart"/> loses its hinderance.</summary>
        public byte SlowStart_HinderTurnsLeft { get; set; }
        /// <summary>True if the Pokémon was present at the start of the turn, which would allow <see cref="PBEAbility.SpeedBoost"/> to activate.</summary>
        public bool SpeedBoost_AbleToSpeedBoostThisTurn { get; set; }
        #endregion

        internal PBEPokemon(EndianBinaryReader r, PBETeam team)
        {
            Team = team;
            Id = r.ReadByte();
            Species = OriginalSpecies = KnownSpecies = r.ReadEnum<PBESpecies>();
            Form = OriginalForm = KnownForm = RevertForm = r.ReadEnum<PBEForm>();
            var pData = PBEPokemonData.GetData(Species, Form);
            KnownType1 = Type1 = pData.Type1;
            KnownType2 = Type2 = pData.Type2;
            KnownWeight = Weight = pData.Weight;
            Nickname = KnownNickname = r.ReadStringNullTerminated();
            Level = r.ReadByte();
            Friendship = r.ReadByte();
            Shiny = KnownShiny = r.ReadBoolean();
            Ability = OriginalAbility = RevertAbility = r.ReadEnum<PBEAbility>();
            KnownAbility = PBEAbility.MAX;
            Nature = r.ReadEnum<PBENature>();
            Gender = KnownGender = r.ReadEnum<PBEGender>();
            Item = OriginalItem = r.ReadEnum<PBEItem>();
            KnownItem = (PBEItem)ushort.MaxValue;
            EffortValues = new PBEEffortValues(Team.Battle.Settings, r);
            IndividualValues = new PBEIndividualValues(Team.Battle.Settings, r);
            SetStats(true);
            OriginalMoveset = new PBEMoveset(Species, Form, Level, Team.Battle.Settings, r);
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
            Form = OriginalForm = KnownForm = RevertForm = shell.Form;
            var pData = PBEPokemonData.GetData(Species, Form);
            KnownType1 = Type1 = pData.Type1;
            KnownType2 = Type2 = pData.Type2;
            KnownWeight = Weight = pData.Weight;
            Nickname = KnownNickname = shell.Nickname;
            Level = shell.Level;
            Friendship = shell.Friendship;
            Shiny = KnownShiny = shell.Shiny;
            Ability = OriginalAbility = RevertAbility = shell.Ability;
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
            KnownAbility = Ability = RevertAbility = OriginalAbility = PBEAbility.MAX;
            KnownGender = Gender = info.Gender;
            KnownItem = Item = OriginalItem = (PBEItem)ushort.MaxValue;
            Moves = new PBEBattleMoveset(Team.Battle.Settings);
            KnownMoves = new PBEBattleMoveset(Team.Battle.Settings);
            TransformBackupMoves = new PBEBattleMoveset(Team.Battle.Settings);
            KnownNickname = Nickname = info.Nickname;
            KnownShiny = Shiny = info.Shiny;
            KnownSpecies = Species = OriginalSpecies = info.Species;
            KnownForm = RevertForm = Form = OriginalForm = info.Form;
            var pData = PBEPokemonData.GetData(KnownSpecies, KnownForm);
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
            Species = KnownSpecies = OriginalSpecies;
            Form = KnownForm = RevertForm;
            Ability = KnownAbility = RevertAbility;
            var pData = PBEPokemonData.GetData(Species, Form);
            KnownAbility = PBEAbility.MAX;
            KnownGender = Gender;
            KnownItem = (PBEItem)ushort.MaxValue;
            KnownMoves.SetUnknown();
            KnownNickname = Nickname;
            KnownShiny = Shiny;
            KnownType1 = Type1 = pData.Type1;
            KnownType2 = Type2 = pData.Type2;

            ClearStatChanges();

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
            LockOnPokemon = null;
            LockOnTurns = 0;
            MagnetRiseTurns = 0;
            Protection_Counter = 0;
            Protection_Used = false;
            SeededPosition = PBEFieldPosition.None;
            SeededTeam = null;
            SubstituteHP = 0;
            if (Status2.HasFlag(PBEStatus2.Transformed))
            {
                Moves.Reset(TransformBackupMoves);
                TransformBackupMoves.SetUnknown();
            }
            Status2 = PBEStatus2.None;
            KnownStatus2 = PBEStatus2.None;

            HasUsedMoveThisTurn = false;
            TurnAction = null;
            TempLockedMove = PBEMove.None;
            TempLockedTargets = PBETurnTarget.None;
            ChoiceLockedMove = PBEMove.None;

            Minimize_Used = false;
            SlowStart_HinderTurnsLeft = 0;
            SpeedBoost_AbleToSpeedBoostThisTurn = false;

            SetStats(false);
        }
        public void SetStats(bool calculateHP)
        {
            var pData = PBEPokemonData.GetData(Species, Form);
            PBENature nature = Nature;
            PBEEffortValues evs = EffortValues;
            PBEIndividualValues ivs = IndividualValues;
            byte level = Level;
            PBESettings settings = Team.Battle.Settings;
            if (calculateHP)
            {
                ushort hp = PBEDataUtils.CalculateStat(pData, PBEStat.HP, nature, evs.HP, ivs.HP, level, settings);
                MaxHP = hp;
                HP = hp;
                HPPercentage = 1d;
            }
            Attack = PBEDataUtils.CalculateStat(pData, PBEStat.Attack, nature, evs.Attack, ivs.Attack, level, settings);
            Defense = PBEDataUtils.CalculateStat(pData, PBEStat.Defense, nature, evs.Defense, ivs.Defense, level, settings);
            SpAttack = PBEDataUtils.CalculateStat(pData, PBEStat.SpAttack, nature, evs.SpAttack, ivs.SpAttack, level, settings);
            SpDefense = PBEDataUtils.CalculateStat(pData, PBEStat.SpDefense, nature, evs.SpDefense, ivs.SpDefense, level, settings);
            Speed = PBEDataUtils.CalculateStat(pData, PBEStat.Speed, nature, evs.Speed, ivs.Speed, level, settings);
        }
        /// <summary>Copies the <paramref name="target"/>, does not set <see cref="PBEStatus2.Transformed"/>.</summary>
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

        public bool HasType(PBEType type, bool useKnownInfo = false)
        {
            if (type >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }
            return ((useKnownInfo ? KnownType1 : Type1) == type) || ((useKnownInfo ? KnownType2 : Type2) == type);
        }
        public bool ReceivesSTAB(PBEType type, bool useKnownInfo = false)
        {
            return type != PBEType.None && HasType(type, useKnownInfo: useKnownInfo);
        }
        public bool HasCancellingAbility(bool useKnownInfo = false)
        {
            PBEAbility kAbility = useKnownInfo ? KnownAbility : Ability;
            return kAbility == PBEAbility.MoldBreaker || kAbility == PBEAbility.Teravolt || kAbility == PBEAbility.Turboblaze;
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
        public PBEStat[] GetStatsLessThan(int i)
        {
            var list = new List<PBEStat>(7);
            if (AttackChange < i)
            {
                list.Add(PBEStat.Attack);
            }
            if (DefenseChange < i)
            {
                list.Add(PBEStat.Defense);
            }
            if (SpAttackChange < i)
            {
                list.Add(PBEStat.SpAttack);
            }
            if (SpDefenseChange < i)
            {
                list.Add(PBEStat.SpDefense);
            }
            if (SpeedChange < i)
            {
                list.Add(PBEStat.Speed);
            }
            if (AccuracyChange < i)
            {
                list.Add(PBEStat.Accuracy);
            }
            if (EvasionChange < i)
            {
                list.Add(PBEStat.Evasion);
            }
            return list.ToArray();
        }
        public PBEStat[] GetStatsGreaterThan(int i)
        {
            var list = new List<PBEStat>(7);
            if (AttackChange > i)
            {
                list.Add(PBEStat.Attack);
            }
            if (DefenseChange > i)
            {
                list.Add(PBEStat.Defense);
            }
            if (SpAttackChange > i)
            {
                list.Add(PBEStat.SpAttack);
            }
            if (SpDefenseChange > i)
            {
                list.Add(PBEStat.SpDefense);
            }
            if (SpeedChange > i)
            {
                list.Add(PBEStat.Speed);
            }
            if (AccuracyChange > i)
            {
                list.Add(PBEStat.Accuracy);
            }
            if (EvasionChange > i)
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
            sbyte maxStatChange = Team.Battle.Settings.MaxStatChange;
            sbyte val = (sbyte)PBEUtils.Clamp(value, -maxStatChange, maxStatChange);
            switch (stat)
            {
                case PBEStat.Accuracy: AccuracyChange = val; break;
                case PBEStat.Attack: AttackChange = val; break;
                case PBEStat.Defense: DefenseChange = val; break;
                case PBEStat.Evasion: EvasionChange = val; break;
                case PBEStat.SpAttack: SpAttackChange = val; break;
                case PBEStat.SpDefense: SpDefenseChange = val; break;
                case PBEStat.Speed: SpeedChange = val; break;
                default: throw new ArgumentOutOfRangeException(nameof(stat));
            }
            return val;
        }
        public PBEResult IsStatChangePossible(PBEStat stat, PBEPokemon other, int change, out sbyte oldValue, out sbyte newValue, bool useKnownInfo = false)
        {
            oldValue = GetStatChange(stat);

            // Verified: Contrary/Simple are silent
            if (other?.HasCancellingAbility() != true)
            {
                switch (useKnownInfo ? KnownAbility : Ability)
                {
                    case PBEAbility.ClearBody:
                    case PBEAbility.WhiteSmoke:
                    {
                        if (change < 0 && other != this)
                        {
                            newValue = oldValue;
                            return PBEResult.Ineffective_Ability;
                        }
                        break;
                    }
                    case PBEAbility.Contrary: change *= -1; break;
                    case PBEAbility.Simple: change *= 2; break;
                }
            }

            sbyte maxStatChange = Team.Battle.Settings.MaxStatChange;
            newValue = (sbyte)PBEUtils.Clamp(oldValue + change, -maxStatChange, maxStatChange);

            if (oldValue == newValue)
            {
                return PBEResult.Ineffective_Stat;
            }
            return PBEResult.Success;
        }
        public void ClearStatChanges()
        {
            AttackChange = 0;
            DefenseChange = 0;
            SpAttackChange = 0;
            SpDefenseChange = 0;
            SpeedChange = 0;
            AccuracyChange = 0;
            EvasionChange = 0;
        }
        /// <summary>For use with <see cref="PBEMove.Punishment"/> and <see cref="PBEMove.StoredPower"/>.</summary>
        public int GetPositiveStatTotal()
        {
            return GetStatsGreaterThan(0).Sum(s => GetStatChange(s));
        }
        /// <summary>Gets the type that a move will become when used by this Pokémon.</summary>
        /// <param name="move">The move to check.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="move"/> is invalid.</exception>
        public PBEType GetMoveType(PBEMove move, bool useKnownInfo = false)
        {
            if (move == PBEMove.None || move >= PBEMove.MAX || !Enum.IsDefined(typeof(PBEMove), move))
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            switch (move)
            {
                case PBEMove.HiddenPower:
                {
                    return useKnownInfo ? PBEType.None : IndividualValues.HiddenPowerType;
                }
                case PBEMove.Judgment:
                {
                    switch (useKnownInfo ? KnownItem : Item)
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
                case PBEMove.Struggle:
                {
                    return PBEType.None;
                }
                case PBEMove.TechnoBlast:
                {
                    switch (useKnownInfo ? KnownItem : Item)
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
                    if ((useKnownInfo ? KnownAbility : Ability) == PBEAbility.Normalize)
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
        public PBEResult IsAttractionPossible(PBEPokemon causer, bool useKnownInfo = false, bool ignoreCurrentStatus = false)
        {
            if (causer == null)
            {
                throw new ArgumentNullException(nameof(causer));
            }
            if (causer == this)
            {
                return PBEResult.InvalidConditions;
            }
            PBEStatus2 kStatus2 = useKnownInfo ? KnownStatus2 : Status2;
            if (!ignoreCurrentStatus && kStatus2.HasFlag(PBEStatus2.Infatuated))
            {
                return PBEResult.Ineffective_Status;
            }
            PBEGender kGender = useKnownInfo ? KnownGender : Gender;
            if (kGender == PBEGender.Genderless || causer.Gender == PBEGender.Genderless || kGender == causer.Gender)
            {
                return PBEResult.Ineffective_Gender;
            }
            PBEAbility kAbility = useKnownInfo ? KnownAbility : Ability;
            if (!causer.HasCancellingAbility() && kAbility == PBEAbility.Oblivious)
            {
                return PBEResult.Ineffective_Ability;
            }
            return PBEResult.Success;
        }
        public PBEResult IsBurnPossible(PBEPokemon other, bool useKnownInfo = false, bool ignoreSubstitute = false, bool ignoreCurrentStatus = false, bool ignoreSafeguard = false)
        {
            PBEStatus2 kStatus2 = useKnownInfo ? KnownStatus2 : Status2;
            if (!ignoreSubstitute && kStatus2.HasFlag(PBEStatus2.Substitute))
            {
                return PBEResult.Ineffective_Substitute;
            }
            if (!ignoreCurrentStatus && Status1 != PBEStatus1.None)
            {
                return PBEResult.Ineffective_Status;
            }
            if (!ignoreSafeguard && Team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
            {
                return PBEResult.Ineffective_Safeguard;
            }
            if (HasType(PBEType.Fire, useKnownInfo: useKnownInfo))
            {
                return PBEResult.Ineffective_Type;
            }
            PBEAbility kAbility = useKnownInfo ? KnownAbility : Ability;
            if (other?.HasCancellingAbility() != true && (kAbility == PBEAbility.WaterVeil || (kAbility == PBEAbility.LeafGuard && Team.Battle.WillLeafGuardActivate())))
            {
                return PBEResult.Ineffective_Ability;
            }
            return PBEResult.Success;
        }
        public PBEResult IsConfusionPossible(PBEPokemon other, bool useKnownInfo = false, bool ignoreSubstitute = false, bool ignoreCurrentStatus = false, bool ignoreSafeguard = false)
        {
            PBEStatus2 kStatus2 = useKnownInfo ? KnownStatus2 : Status2;
            if (!ignoreSubstitute && kStatus2.HasFlag(PBEStatus2.Substitute))
            {
                return PBEResult.Ineffective_Substitute;
            }
            if (!ignoreCurrentStatus && kStatus2.HasFlag(PBEStatus2.Confused))
            {
                return PBEResult.Ineffective_Status;
            }
            if (!ignoreSafeguard && Team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
            {
                return PBEResult.Ineffective_Safeguard;
            }
            PBEAbility kAbility = useKnownInfo ? KnownAbility : Ability;
            if (other?.HasCancellingAbility() != true && kAbility == PBEAbility.OwnTempo)
            {
                return PBEResult.Ineffective_Ability;
            }
            return PBEResult.Success;
        }
        public PBEResult IsFreezePossible(PBEPokemon other, bool useKnownInfo = false, bool ignoreSubstitute = false, bool ignoreCurrentStatus = false, bool ignoreSafeguard = false)
        {
            PBEStatus2 kStatus2 = useKnownInfo ? KnownStatus2 : Status2;
            if (!ignoreSubstitute && kStatus2.HasFlag(PBEStatus2.Substitute))
            {
                return PBEResult.Ineffective_Substitute;
            }
            if (!ignoreCurrentStatus && Status1 != PBEStatus1.None)
            {
                return PBEResult.Ineffective_Status;
            }
            if (!ignoreSafeguard && Team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
            {
                return PBEResult.Ineffective_Safeguard;
            }
            if (HasType(PBEType.Ice, useKnownInfo: useKnownInfo))
            {
                return PBEResult.Ineffective_Type;
            }
            PBEAbility kAbility = useKnownInfo ? KnownAbility : Ability;
            if (other?.HasCancellingAbility() != true && (kAbility == PBEAbility.MagmaArmor || (kAbility == PBEAbility.LeafGuard && Team.Battle.WillLeafGuardActivate())))
            {
                return PBEResult.Ineffective_Ability;
            }
            return PBEResult.Success;
        }
        public PBEResult IsFlinchPossible(PBEPokemon other, bool useKnownInfo = false)
        {
            PBEStatus2 kStatus2 = useKnownInfo ? KnownStatus2 : Status2;
            if (kStatus2.HasFlag(PBEStatus2.Substitute))
            {
                return PBEResult.Ineffective_Substitute;
            }
            if (kStatus2.HasFlag(PBEStatus2.Flinching))
            {
                return PBEResult.Ineffective_Status;
            }
            PBEAbility kAbility = useKnownInfo ? KnownAbility : Ability;
            if (other?.HasCancellingAbility() != true && kAbility == PBEAbility.InnerFocus)
            {
                return PBEResult.Ineffective_Ability;
            }
            return PBEResult.Success;
        }
        public PBEResult IsGrounded(PBEPokemon other, bool useKnownInfo = false)
        {
            if (HasType(PBEType.Flying, useKnownInfo: useKnownInfo))
            {
                return PBEResult.Ineffective_Type;
            }
            PBEAbility kAbility = useKnownInfo ? KnownAbility : Ability;
            if (other?.HasCancellingAbility() != true && kAbility == PBEAbility.Levitate)
            {
                return PBEResult.Ineffective_Ability;
            }
            PBEStatus2 kStatus2 = useKnownInfo ? KnownStatus2 : Status2;
            if (kStatus2.HasFlag(PBEStatus2.MagnetRise))
            {
                return PBEResult.Ineffective_MagnetRise;
            }
            return PBEResult.Success;
        }
        public PBEResult IsLeechSeedPossible(bool useKnownInfo = false)
        {
            PBEStatus2 kStatus2 = useKnownInfo ? KnownStatus2 : Status2;
            if (kStatus2.HasFlag(PBEStatus2.Substitute))
            {
                return PBEResult.Ineffective_Substitute;
            }
            if (kStatus2.HasFlag(PBEStatus2.LeechSeed))
            {
                return PBEResult.Ineffective_Status;
            }
            if (HasType(PBEType.Grass, useKnownInfo: useKnownInfo))
            {
                return PBEResult.Ineffective_Type;
            }
            return PBEResult.Success;
        }
        public PBEResult IsMagnetRisePossible(bool useKnownInfo = false)
        {
            PBEStatus2 kStatus2 = useKnownInfo ? KnownStatus2 : Status2;
            if (kStatus2.HasFlag(PBEStatus2.MagnetRise))
            {
                return PBEResult.Ineffective_Status;
            }
            return PBEResult.Success;
        }
        public PBEResult IsParalysisPossible(PBEPokemon other, bool useKnownInfo = false, bool ignoreSubstitute = false, bool ignoreCurrentStatus = false, bool ignoreSafeguard = false)
        {
            PBEStatus2 kStatus2 = useKnownInfo ? KnownStatus2 : Status2;
            if (!ignoreSubstitute && kStatus2.HasFlag(PBEStatus2.Substitute))
            {
                return PBEResult.Ineffective_Substitute;
            }
            if (!ignoreCurrentStatus && Status1 != PBEStatus1.None)
            {
                return PBEResult.Ineffective_Status;
            }
            if (!ignoreSafeguard && Team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
            {
                return PBEResult.Ineffective_Safeguard;
            }
            PBEAbility kAbility = useKnownInfo ? KnownAbility : Ability;
            if (other?.HasCancellingAbility() != true && (kAbility == PBEAbility.Limber || (kAbility == PBEAbility.LeafGuard && Team.Battle.WillLeafGuardActivate())))
            {
                return PBEResult.Ineffective_Ability;
            }
            return PBEResult.Success;
        }
        public PBEResult IsPoisonPossible(PBEPokemon other, bool useKnownInfo = false, bool ignoreSubstitute = false, bool ignoreCurrentStatus = false, bool ignoreSafeguard = false)
        {
            PBEStatus2 kStatus2 = useKnownInfo ? KnownStatus2 : Status2;
            if (!ignoreSubstitute && kStatus2.HasFlag(PBEStatus2.Substitute))
            {
                return PBEResult.Ineffective_Substitute;
            }
            if (!ignoreCurrentStatus && Status1 != PBEStatus1.None)
            {
                return PBEResult.Ineffective_Status;
            }
            if (!ignoreSafeguard && Team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
            {
                return PBEResult.Ineffective_Safeguard;
            }
            if (HasType(PBEType.Poison, useKnownInfo: useKnownInfo) || HasType(PBEType.Steel, useKnownInfo: useKnownInfo))
            {
                return PBEResult.Ineffective_Type;
            }
            PBEAbility kAbility = useKnownInfo ? KnownAbility : Ability;
            if (other?.HasCancellingAbility() != true && (kAbility == PBEAbility.Immunity || (kAbility == PBEAbility.LeafGuard && Team.Battle.WillLeafGuardActivate())))
            {
                return PBEResult.Ineffective_Ability;
            }
            return PBEResult.Success;
        }
        public PBEResult IsSleepPossible(PBEPokemon other, bool useKnownInfo = false, bool ignoreSubstitute = false, bool ignoreCurrentStatus = false, bool ignoreSafeguard = false)
        {
            PBEStatus2 kStatus2 = useKnownInfo ? KnownStatus2 : Status2;
            if (!ignoreSubstitute && kStatus2.HasFlag(PBEStatus2.Substitute))
            {
                return PBEResult.Ineffective_Substitute;
            }
            if (!ignoreCurrentStatus && Status1 != PBEStatus1.None)
            {
                return PBEResult.Ineffective_Status;
            }
            if (!ignoreSafeguard && Team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
            {
                return PBEResult.Ineffective_Safeguard;
            }
            PBEAbility kAbility = useKnownInfo ? KnownAbility : Ability;
            if (other?.HasCancellingAbility() != true && (kAbility == PBEAbility.Insomnia || (kAbility == PBEAbility.LeafGuard && Team.Battle.WillLeafGuardActivate()) || kAbility == PBEAbility.VitalSpirit))
            {
                return PBEResult.Ineffective_Ability;
            }
            return PBEResult.Success;
        }
        public PBEResult IsSubstitutePossible(bool useKnownInfo = false, bool ignoreCurrentStatus = false)
        {
            PBEStatus2 kStatus2 = useKnownInfo ? KnownStatus2 : Status2;
            if (!ignoreCurrentStatus && kStatus2.HasFlag(PBEStatus2.Substitute))
            {
                return PBEResult.Ineffective_Status;
            }
            int hpRequired = MaxHP / 4;
            if (hpRequired < 1 || HP <= hpRequired)
            {
                return PBEResult.Ineffective_Stat;
            }
            return PBEResult.Success;
        }
        public PBEResult IsTransformPossible(PBEPokemon other, bool useKnownInfo = false)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            PBEStatus2 kStatus2 = useKnownInfo ? KnownStatus2 : Status2;
            if (kStatus2.HasFlag(PBEStatus2.Substitute))
            {
                return PBEResult.Ineffective_Substitute;
            }
            if (kStatus2.HasFlag(PBEStatus2.Disguised) || kStatus2.HasFlag(PBEStatus2.Transformed) || other.Status2.HasFlag(PBEStatus2.Transformed))
            {
                return PBEResult.Ineffective_Status;
            }
            return PBEResult.Success;
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

        internal void ToBytes(EndianBinaryWriter w)
        {
            w.Write(Id);
            w.Write(OriginalSpecies);
            w.Write(OriginalForm);
            w.Write(Nickname, true);
            w.Write(Level);
            w.Write(Friendship);
            w.Write(Shiny);
            w.Write(OriginalAbility);
            w.Write(Nature);
            w.Write(Gender);
            w.Write(OriginalItem);
            EffortValues.ToBytes(w);
            IndividualValues.ToBytes(w);
            OriginalMoveset.ToBytes(w);
        }

        // Will only be accurate for the host
        public override string ToString()
        {
            var sb = new StringBuilder();
            string formStr = PBEDataUtils.HasForms(Species, false) ? $" ({PBELocalizedString.GetFormName(Species, Form)})" : string.Empty;
            sb.AppendLine($"{Nickname}/{Species}{formStr} {GenderSymbol} Lv.{Level}");
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
                sb.AppendLine($"Asleep turns: {Status1Counter}/{SleepTurns}");
            }
            else if (Status1 == PBEStatus1.BadlyPoisoned)
            {
                sb.AppendLine($"Toxic counter: {Status1Counter}");
            }
            sb.AppendLine($"Status2: {Status2}");
            if (Status2.HasFlag(PBEStatus2.Confused))
            {
                sb.AppendLine($"Confusion turns: {ConfusionCounter}/{ConfusionTurns}");
            }
            if (Status2.HasFlag(PBEStatus2.Disguised))
            {
                sb.AppendLine($"Disguised as: {DisguisedAsPokemon.Team.TrainerName}'s {DisguisedAsPokemon.Nickname}");
            }
            if (Team.Battle.BattleFormat != PBEBattleFormat.Single)
            {
                if (Status2.HasFlag(PBEStatus2.Infatuated))
                {
                    sb.AppendLine($"Infatuated with: {InfatuatedWithPokemon.Team.TrainerName}'s {InfatuatedWithPokemon.Nickname}");
                }
                if (Status2.HasFlag(PBEStatus2.LeechSeed))
                {
                    sb.AppendLine($"Seeded position: {SeededTeam.TrainerName}'s {SeededPosition}");
                }
                if (Status2.HasFlag(PBEStatus2.LockOn))
                {
                    sb.AppendLine($"Taking aim at: {LockOnPokemon.Team.TrainerName}'s {LockOnPokemon.Nickname}");
                }
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
                sb.AppendLine($"{PBELocalizedString.GetMoveName(PBEMove.HiddenPower).English}: {PBELocalizedString.GetTypeName(IndividualValues.HiddenPowerType).English}|{IndividualValues.HiddenPowerBasePower}");
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
