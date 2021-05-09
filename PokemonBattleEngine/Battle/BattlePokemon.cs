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
    public sealed class PBEBattlePokemon : IPBEPokemonKnownTypes, IPBEPokemonTypes, IPBESpeciesForm
    {
        public PBEBattle Battle { get; }
        public PBETeam Team { get; }
        public PBETrainer Trainer { get; }
        public byte Id { get; }
        public bool IsWild => Team.IsWild;
        public bool PBEIgnore { get; }

        public bool CanBattle => HP > 0 && !PBEIgnore;

        #region Basic Properties
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
        public PBEReadOnlyStatCollection OriginalEffortValues { get; }
        public PBEStatCollection EffortValues { get; }
        public PBEReadOnlyStatCollection IndividualValues { get; }
        public byte Friendship { get; set; }
        public byte OriginalLevel { get; set; }
        /// <summary>The Pokémon's level.</summary>
        public byte Level { get; set; }
        public uint OriginalEXP { get; set; }
        public uint EXP { get; set; }
        /// <summary>The Pokémon's nature.</summary>
        public PBENature Nature { get; set; }
        /// <summary>The moveset the Pokémon had upon entering battle.</summary>
        public PBEReadOnlyPartyMoveset OriginalMoveset { get; }

        /// <summary>The Pokémon's field position.</summary>
        public PBEFieldPosition FieldPosition { get; set; }
        /// <summary>The Pokémon's current ability.</summary>
        public PBEAbility Ability { get; set; }
        /// <summary>The ability the Pokémon is known to have.</summary>
        public PBEAbility KnownAbility { get; set; }
        /// <summary>The ability the Pokémon had upon entering battle.</summary>
        public PBEAbility OriginalAbility { get; set; }
        /// <summary>The ability the Pokémon will regain upon switching out, fainting, or the battle ending.</summary>
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
        /// <summary>The Pokémon's current ball (affected by catching).</summary>
        public PBEItem CaughtBall { get; set; }
        /// <summary>The ball the Pokémon is known to be in (affected by disguising).</summary>
        public PBEItem KnownCaughtBall { get; set; }
        /// <summary>The ball the Pokémon was in upon entering battle.</summary>
        public PBEItem OriginalCaughtBall { get; set; }
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
        public bool Pokerus { get; set; }
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
        public PBEStatus1 OriginalStatus1 { get; set; }
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
        #endregion

        #region Statuses
        /// <summary>The counter used for <see cref="PBEStatus1.BadlyPoisoned"/> and <see cref="PBEStatus1.Asleep"/>.</summary>
        public byte Status1Counter { get; set; }
        /// <summary>The amount of turns the Pokémon will sleep for before waking.</summary>
        public byte SleepTurns { get; set; }
        /// <summary>The counter used for <see cref="PBEStatus2.Confused"/>.</summary>
        public byte ConfusionCounter { get; set; }
        /// <summary>The amount of turns the Pokémon will be confused for before snapping out of it.</summary>
        public byte ConfusionTurns { get; set; }
        /// <summary>The Pokémon that <see cref="PBEStatus2.Infatuated"/> is bound to.</summary>
        public PBEBattlePokemon InfatuatedWithPokemon { get; set; }
        /// <summary>The amount of turns until <see cref="PBEStatus2.MagnetRise"/> ends.</summary>
        public byte MagnetRiseTurns { get; set; }
        /// <summary>The Pokémon that <see cref="PBEStatus2.LockOn"/> is bound to.</summary>
        public PBEBattlePokemon LockOnPokemon { get; set; }
        public byte LockOnTurns { get; set; }
        /// <summary>The amount of times the Pokémon has successfully used <see cref="PBEMoveEffect.Protect"/>, <see cref="PBEMoveEffect.QuickGuard"/>, and/or <see cref="PBEMoveEffect.WideGuard"/> consecutively.</summary>
        public int Protection_Counter { get; set; }
        public bool Protection_Used { get; set; }
        public PBERoostTypes RoostTypes { get; set; }
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
        /// <summary>True if the Pokémon has successfully used <see cref="PBEMoveEffect.Minimize"/> which makes it succeptible to double damage from <see cref="PBEMoveFlag.DoubleDamageMinimized"/>.</summary>
        public bool Minimize_Used { get; set; }
        /// <summary>The amount of turns left until a Pokémon with <see cref="PBEAbility.SlowStart"/> loses its hinderance.</summary>
        public byte SlowStart_HinderTurnsLeft { get; set; }
        /// <summary>True if the Pokémon was present at the start of the turn, which would allow <see cref="PBEAbility.SpeedBoost"/> to activate.</summary>
        public bool SpeedBoost_AbleToSpeedBoostThisTurn { get; set; }
        #endregion

        public List<PBEBattlePokemon> EXPPokemon { get; } = new List<PBEBattlePokemon>();

        #region Constructors
        private PBEBattlePokemon(PBETrainer trainer, byte id,
            PBESpecies species, PBEForm form, string nickname, byte level, uint exp, byte friendship, bool shiny, bool pokerus,
            PBEAbility ability, PBENature nature, PBEGender gender, PBEItem item, PBEItem caughtBall,
            PBEReadOnlyStatCollection evs, PBEReadOnlyStatCollection ivs, PBEReadOnlyPartyMoveset moves)
        {
            Battle = trainer.Battle;
            Team = trainer.Team;
            Trainer = trainer;
            Id = id;
            Species = OriginalSpecies = KnownSpecies = species;
            Form = OriginalForm = KnownForm = RevertForm = form;
            IPBEPokemonData pData = PBEDataProvider.Instance.GetPokemonData(species, form);
            KnownType1 = Type1 = pData.Type1;
            KnownType2 = Type2 = pData.Type2;
            KnownWeight = Weight = pData.Weight;
            Nickname = KnownNickname = nickname;
            Level = OriginalLevel = level;
            EXP = OriginalEXP = exp;
            Friendship = friendship;
            Shiny = KnownShiny = shiny;
            Pokerus = pokerus;
            Ability = OriginalAbility = RevertAbility = ability;
            KnownAbility = PBEAbility.MAX;
            Nature = nature;
            Gender = KnownGender = gender;
            Item = OriginalItem = item;
            KnownItem = (PBEItem)ushort.MaxValue;
            KnownCaughtBall = CaughtBall = OriginalCaughtBall = caughtBall;
            OriginalEffortValues = evs;
            EffortValues = new PBEStatCollection(evs);
            IndividualValues = new PBEReadOnlyStatCollection(ivs);
            SetStats(true, false);
            OriginalMoveset = moves;
            PBESettings settings = Battle.Settings;
            Moves = new PBEBattleMoveset(settings, moves);
            KnownMoves = new PBEBattleMoveset(settings);
            TransformBackupMoves = new PBEBattleMoveset(settings);
        }
        private PBEBattlePokemon(PBETrainer trainer, byte id, IPBEPokemon pkmn, PBEReadOnlyPartyMoveset moves)
            : this(trainer, id,
                  pkmn.Species, pkmn.Form, pkmn.Nickname, pkmn.Level, pkmn.EXP, pkmn.Friendship, pkmn.Shiny, pkmn.Pokerus,
                  pkmn.Ability, pkmn.Nature, pkmn.Gender, pkmn.Item, pkmn.CaughtBall,
                  new PBEReadOnlyStatCollection(pkmn.EffortValues), new PBEReadOnlyStatCollection(pkmn.IndividualValues), moves)
        {
            PBEIgnore = pkmn.PBEIgnore;
        }
        internal PBEBattlePokemon(PBETrainer trainer, byte id, IPBEPokemon pkmn)
            : this(trainer, id, pkmn, new PBEReadOnlyPartyMoveset(trainer.Battle.Settings, pkmn.Moveset))
        {
            trainer.Party.Add(this);
        }
        internal PBEBattlePokemon(PBETrainer trainer, byte id, IPBEPartyPokemon pkmn)
            : this(trainer, id, pkmn, new PBEReadOnlyPartyMoveset(pkmn.Moveset))
        {
            ushort hp = pkmn.HP;
            if (hp > MaxHP)
            {
                throw new ArgumentOutOfRangeException(nameof(pkmn.HP));
            }
            HP = hp;
            UpdateHPPercentage();
            PBEStatus1 status1 = pkmn.Status1;
            if (status1 >= PBEStatus1.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(pkmn.Status1));
            }
            if (status1 == PBEStatus1.BadlyPoisoned)
            {
                Status1Counter = 1;
            }
            Status1 = OriginalStatus1 = status1;
            byte sleepTurns = pkmn.SleepTurns;
            if (status1 != PBEStatus1.Asleep && sleepTurns != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pkmn.SleepTurns));
            }
            SleepTurns = sleepTurns;
            trainer.Party.Add(this);
        }
        internal PBEBattlePokemon(PBETrainer trainer, PBEBattlePacket.PBETeamInfo.PBETrainerInfo.PBEBattlePokemonInfo info)
            : this(trainer, info.Id,
                  info.Species, info.Form, info.Nickname, info.Level, info.EXP, info.Friendship, info.Shiny, info.Pokerus,
                  info.Ability, info.Nature, info.Gender, info.Item, info.CaughtBall,
                  info.EffortValues, info.IndividualValues, info.Moveset)
        {
            Status1 = info.Status1;
            if (Status1 == PBEStatus1.BadlyPoisoned)
            {
                Status1Counter = 1;
            }
        }
        private PBEBattlePokemon(PBETrainer trainer, IPBEPkmnAppearedInfo_Hidden info)
        {
            if (trainer is null)
            {
                throw new ArgumentNullException(nameof(trainer));
            }
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            Battle = trainer.Battle;
            Team = trainer.Team;
            Trainer = trainer;
            FieldPosition = info.FieldPosition;
            HPPercentage = info.HPPercentage;
            Status1 = info.Status1;
            if (Status1 == PBEStatus1.BadlyPoisoned)
            {
                Status1Counter = 1;
            }
            Level = info.Level;
            KnownAbility = Ability = PBEAbility.MAX;
            KnownGender = Gender = info.Gender;
            KnownItem = Item = (PBEItem)ushort.MaxValue;
            Moves = new PBEBattleMoveset(Battle.Settings); // For Transform
            KnownMoves = new PBEBattleMoveset(Battle.Settings);
            TransformBackupMoves = new PBEBattleMoveset(Battle.Settings); // For Transform
            KnownNickname = Nickname = info.Nickname;
            KnownShiny = Shiny = info.Shiny;
            KnownSpecies = Species = info.Species;
            KnownForm = Form = info.Form;
            IPBEPokemonData pData = PBEDataProvider.Instance.GetPokemonData(KnownSpecies, KnownForm);
            KnownType1 = Type1 = pData.Type1;
            KnownType2 = Type2 = pData.Type2;
            KnownWeight = Weight = pData.Weight;
            trainer.Party.Add(this);
            Battle.ActiveBattlers.Add(this);
        }
        private PBEBattlePokemon(PBETrainer trainer, IPBEPkmnSwitchInInfo_Hidden info)
            : this(trainer, (IPBEPkmnAppearedInfo_Hidden)info)
        {
            KnownCaughtBall = CaughtBall = info.CaughtBall;
        }
        public PBEBattlePokemon(PBETrainer trainer, PBEPkmnSwitchInPacket_Hidden.PBEPkmnSwitchInInfo info)
            : this(trainer, (IPBEPkmnSwitchInInfo_Hidden)info) { }
        public PBEBattlePokemon(PBEBattle battle, PBEWildPkmnAppearedPacket_Hidden.PBEWildPkmnInfo info)
            : this(battle.Teams[1].Trainers[0], info) { }
        #endregion

        public void AddEXPPokemon(PBEBattlePokemon pkmn)
        {
            if (!EXPPokemon.Contains(pkmn))
            {
                EXPPokemon.Add(pkmn);
            }
        }

        public void ApplyPowerTrickChange()
        {
            ushort a = Attack;
            Attack = Defense;
            Defense = a;
        }
        /// <summary>Applies effects that occur on switching out or escaping such as <see cref="PBEAbility.NaturalCure"/>.</summary>
        public void ApplyNaturalCure()
        {
            if (!PBEIgnore && Ability == PBEAbility.NaturalCure)
            {
                Status1 = PBEStatus1.None;
                Status1Counter = 0;
                SleepTurns = 0;
            }
        }
        private void ResetSpecies()
        {
            Species = KnownSpecies = OriginalSpecies;
            Form = KnownForm = RevertForm;
            Ability = KnownAbility = RevertAbility;
            IPBEPokemonData pData = PBEDataProvider.Instance.GetPokemonData(this);
            KnownAbility = PBEAbility.MAX;
            KnownGender = Gender;
            KnownItem = (PBEItem)ushort.MaxValue;
            KnownMoves.SetUnknown();
            KnownNickname = Nickname;
            KnownShiny = Shiny;
            KnownType1 = Type1 = pData.Type1;
            KnownType2 = Type2 = pData.Type2;
        }
        private void ResetVolatileStuff()
        {
            ConfusionCounter = 0;
            ConfusionTurns = 0;
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
        }
        /// <summary>Sets and clears all information required for switching out.</summary>
        public void ClearForSwitch()
        {
            EXPPokemon.Clear();
            FieldPosition = PBEFieldPosition.None;
            ApplyNaturalCure();
            if (Ability == PBEAbility.Regenerator)
            {
                HP = PBEUtils.Clamp((ushort)(HP + (MaxHP / 3)), ushort.MinValue, MaxHP);
                UpdateHPPercentage();
            }
            ResetSpecies();
            ClearStatChanges();
            if (Status1 == PBEStatus1.Asleep)
            {
                Status1Counter = 0;
            }
            else if (Status1 == PBEStatus1.BadlyPoisoned)
            {
                Status1Counter = 1;
            }
            ResetVolatileStuff();
            SetStats(false, false);
        }
        /// <summary>Sets and clears all information required for fainting.</summary>
        public void ClearForFaint()
        {
            FieldPosition = PBEFieldPosition.None;
            ResetSpecies();
            ClearStatChanges();
            Status1 = PBEStatus1.None;
            ResetVolatileStuff();
            SetStats(false, false);
        }
        public void SetStats(bool calculateHP, bool hpLevelUp)
        {
            IPBEPokemonData pData = PBEDataProvider.Instance.GetPokemonData(this);
            PBENature nature = Nature;
            PBEStatCollection evs = EffortValues;
            PBEReadOnlyStatCollection ivs = IndividualValues;
            byte level = Level;
            PBESettings settings = Battle.Settings;
            if (calculateHP)
            {
                ushort oldHP = MaxHP;
                ushort hp = PBEDataUtils.CalculateStat(pData, PBEStat.HP, nature, evs.HP, ivs.HP, level, settings);
                MaxHP = hp;
                if (hpLevelUp)
                {
                    HP += (ushort)(hp - oldHP);
                }
                else
                {
                    HP = hp;
                }
                UpdateHPPercentage();
            }
            Attack = PBEDataUtils.CalculateStat(pData, PBEStat.Attack, nature, evs.Attack, ivs.Attack, level, settings);
            Defense = PBEDataUtils.CalculateStat(pData, PBEStat.Defense, nature, evs.Defense, ivs.Defense, level, settings);
            SpAttack = PBEDataUtils.CalculateStat(pData, PBEStat.SpAttack, nature, evs.SpAttack, ivs.SpAttack, level, settings);
            SpDefense = PBEDataUtils.CalculateStat(pData, PBEStat.SpDefense, nature, evs.SpDefense, ivs.SpDefense, level, settings);
            Speed = PBEDataUtils.CalculateStat(pData, PBEStat.Speed, nature, evs.Speed, ivs.Speed, level, settings);
        }
        /// <summary>Copies the <paramref name="target"/>, does not set <see cref="PBEStatus2.Transformed"/>.</summary>
        /// <param name="target">The Pokémon to transform into.</param>
        public void Transform(PBEBattlePokemon target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (Trainer != target.Trainer)
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
            KnownForm = target.KnownForm = Form = target.Form;
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
            if (move == PBEMove.None || move >= PBEMove.MAX || !PBEDataUtils.IsMoveUsable(move))
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            PBEBattleMoveset.PBEBattleMovesetSlot knownSlot = KnownMoves[move];
            knownSlot.PP += amountReduced;
            if (knownSlot.MaxPP == 0)
            {
                if (Status2.HasFlag(PBEStatus2.Transformed))
                {
                    knownSlot.MaxPP = PBEBattleMoveset.GetTransformPP(Battle.Settings, move);
                }
                else if (Battle.Settings.MaxPPUps == 0 || knownSlot.PP > PBEBattleMoveset.GetNonTransformPP(Battle.Settings, move, (byte)(Battle.Settings.MaxPPUps - 1)))
                {
                    knownSlot.MaxPP = PBEBattleMoveset.GetNonTransformPP(Battle.Settings, move, Battle.Settings.MaxPPUps);
                }
            }
        }
        /// <summary>Divides <see cref="HP"/> by <see cref="MaxHP"/> and places the result in <see cref="HPPercentage"/>.</summary>
        public void UpdateHPPercentage()
        {
            HPPercentage = (double)HP / MaxHP;
        }
        public void StartRoost()
        {
            PBERoostTypes t = PBERoostTypes.None;
            if (Type1 == PBEType.Flying)
            {
                if (Type2 == PBEType.None)
                {
                    Type1 = PBEType.Normal; // Pure flying-type becomes Normal-type
                    t |= PBERoostTypes.Type1;
                }
                else
                {
                    Type1 = Type2;
                    Type2 = PBEType.None;
                    t |= PBERoostTypes.Type2;
                }
            }
            if (Type2 == PBEType.Flying)
            {
                Type2 = PBEType.None;
                t |= PBERoostTypes.Type2;
            }
            if (KnownType1 == PBEType.Flying)
            {
                if (KnownType2 == PBEType.None)
                {
                    KnownType1 = PBEType.Normal;
                    t |= PBERoostTypes.KnownType1;
                }
                else
                {
                    KnownType1 = KnownType2;
                    KnownType2 = PBEType.None;
                    t |= PBERoostTypes.KnownType2;
                }
            }
            if (KnownType2 == PBEType.Flying)
            {
                KnownType2 = PBEType.None;
                t |= PBERoostTypes.KnownType2;
            }
            RoostTypes = t;
        }
        public void EndRoost()
        {
            PBERoostTypes t = RoostTypes;
            if (t.HasFlag(PBERoostTypes.Type1))
            {
                Type1 = PBEType.Flying;
            }
            if (t.HasFlag(PBERoostTypes.Type2))
            {
                Type2 = PBEType.Flying;
            }
            if (t.HasFlag(PBERoostTypes.KnownType1))
            {
                KnownType1 = PBEType.Flying;
            }
            if (t.HasFlag(PBERoostTypes.KnownType2))
            {
                KnownType2 = PBEType.Flying;
            }
            RoostTypes = PBERoostTypes.None;
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
            sbyte maxStatChange = Battle.Settings.MaxStatChange;
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
        public PBEResult IsStatChangePossible(PBEStat stat, PBEBattlePokemon causer, int change, out sbyte oldValue, out sbyte newValue, bool useKnownInfo = false, bool ignoreSubstitute = false)
        {
            if (causer is null)
            {
                throw new ArgumentNullException(nameof(causer));
            }

            oldValue = GetStatChange(stat);

            if (causer != this && !ignoreSubstitute && (useKnownInfo ? KnownStatus2 : Status2).HasFlag(PBEStatus2.Substitute))
            {
                newValue = oldValue;
                return PBEResult.Ineffective_Substitute;
            }

            // These abilities do not activate when the Pokémon changes its own stat
            if (causer != this && !causer.HasCancellingAbility())
            {
                switch (useKnownInfo ? KnownAbility : Ability)
                {
                    case PBEAbility.BigPecks:
                    {
                        if (change < 0 && stat == PBEStat.Defense)
                        {
                            newValue = oldValue;
                            return PBEResult.Ineffective_Ability;
                        }
                        break;
                    }
                    case PBEAbility.ClearBody:
                    case PBEAbility.WhiteSmoke:
                    {
                        if (change < 0)
                        {
                            newValue = oldValue;
                            return PBEResult.Ineffective_Ability;
                        }
                        break;
                    }
                    case PBEAbility.HyperCutter:
                    {
                        if (change < 0 && stat == PBEStat.Attack)
                        {
                            newValue = oldValue;
                            return PBEResult.Ineffective_Ability;
                        }
                        break;
                    }
                    case PBEAbility.KeenEye:
                    {
                        if (change < 0 && stat == PBEStat.Accuracy)
                        {
                            newValue = oldValue;
                            return PBEResult.Ineffective_Ability;
                        }
                        break;
                    }
                }
            }

            // Verified: Contrary/Simple are silent
            // These abilities activate when the Pokémon changes its own stat
            if (causer == this || !causer.HasCancellingAbility())
            {
                switch (useKnownInfo ? KnownAbility : Ability)
                {
                    case PBEAbility.Contrary: change *= -1; break;
                    case PBEAbility.Simple: change *= 2; break;
                }
            }

            sbyte maxStatChange = Battle.Settings.MaxStatChange;
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
        /// <summary>For use with <see cref="PBEMoveEffect.Punishment"/> and <see cref="PBEMoveEffect.StoredPower"/>.</summary>
        public int GetPositiveStatTotal()
        {
            return GetStatsGreaterThan(0).Sum(s => GetStatChange(s));
        }
        public PBEType GetMoveType(IPBEMoveData mData, bool useKnownInfo = false)
        {
            if (mData == null)
            {
                throw new ArgumentNullException(nameof(mData));
            }
            if (!mData.IsMoveUsable())
            {
                throw new ArgumentOutOfRangeException(nameof(mData));
            }
            switch (mData.Effect)
            {
                case PBEMoveEffect.HiddenPower:
                {
                    if (!useKnownInfo)
                    {
                        return IndividualValues.GetHiddenPowerType();
                    }
                    break;
                }
                case PBEMoveEffect.Judgment:
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
                    }
                    break;
                }
                case PBEMoveEffect.Struggle:
                {
                    return PBEType.None;
                }
                case PBEMoveEffect.TechnoBlast:
                {
                    switch (useKnownInfo ? KnownItem : Item)
                    {
                        case PBEItem.BurnDrive: return PBEType.Fire;
                        case PBEItem.ChillDrive: return PBEType.Ice;
                        case PBEItem.DouseDrive: return PBEType.Water;
                        case PBEItem.ShockDrive: return PBEType.Electric;
                    }
                    break;
                }
                case PBEMoveEffect.WeatherBall:
                {
                    if (Battle.ShouldDoWeatherEffects())
                    {
                        switch (Battle.Weather)
                        {
                            case PBEWeather.Hailstorm: return PBEType.Ice;
                            case PBEWeather.HarshSunlight: return PBEType.Fire;
                            case PBEWeather.Rain: return PBEType.Water;
                            case PBEWeather.Sandstorm: return PBEType.Rock;
                        }
                    }
                    break;
                }
            }
            if ((useKnownInfo ? KnownAbility : Ability) == PBEAbility.Normalize)
            {
                return PBEType.Normal;
            }
            return mData.Type;
        }
        public PBEType GetMoveType(PBEMove move, bool useKnownInfo = false)
        {
            if (move == PBEMove.None || move >= PBEMove.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            return GetMoveType(PBEDataProvider.Instance.GetMoveData(move), useKnownInfo: useKnownInfo);
        }
        public PBEMoveTarget GetMoveTargets(IPBEMoveData mData)
        {
            if (mData == null)
            {
                throw new ArgumentNullException(nameof(mData));
            }
            if (!mData.IsMoveUsable())
            {
                throw new ArgumentOutOfRangeException(nameof(mData));
            }
            if (mData.Effect == PBEMoveEffect.Curse)
            {
                if (this.HasType(PBEType.Ghost))
                {
                    return PBEMoveTarget.SingleSurrounding;
                }
                return PBEMoveTarget.Self;
            }
            return mData.Targets;
        }
        /// <summary>Gets the possible targets that a move can target when used by this Pokémon.</summary>
        /// <param name="move">The move to check.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="move"/> is invalid.</exception>
        public PBEMoveTarget GetMoveTargets(PBEMove move)
        {
            if (move == PBEMove.None || move >= PBEMove.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            return GetMoveTargets(PBEDataProvider.Instance.GetMoveData(move));
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
        public bool CanHitThroughSafeguard()
        {
            return Ability == PBEAbility.Infiltrator;
        }
        public PBEResult IsAttractionPossible(PBEBattlePokemon causer, bool useKnownInfo = false, bool ignoreCurrentStatus = false)
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
            if (!kGender.IsOppositeGender(causer.Gender))
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
        public PBEResult IsBurnPossible(PBEBattlePokemon other, bool useKnownInfo = false, bool ignoreSubstitute = false, bool ignoreCurrentStatus = false, bool ignoreSafeguard = false)
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
            if (!ignoreSafeguard && other?.CanHitThroughSafeguard() == true && Team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
            {
                return PBEResult.Ineffective_Safeguard;
            }
            if (this.HasType(PBEType.Fire, useKnownInfo))
            {
                return PBEResult.Ineffective_Type;
            }
            PBEAbility kAbility = useKnownInfo ? KnownAbility : Ability;
            if (other?.HasCancellingAbility() != true && (kAbility == PBEAbility.WaterVeil || (kAbility == PBEAbility.LeafGuard && Battle.WillLeafGuardActivate())))
            {
                return PBEResult.Ineffective_Ability;
            }
            return PBEResult.Success;
        }
        public PBEResult IsConfusionPossible(PBEBattlePokemon other, bool useKnownInfo = false, bool ignoreSubstitute = false, bool ignoreCurrentStatus = false, bool ignoreSafeguard = false)
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
            if (!ignoreSafeguard && other?.CanHitThroughSafeguard() == true && Team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
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
        public PBEResult IsFreezePossible(PBEBattlePokemon other, bool useKnownInfo = false, bool ignoreSubstitute = false, bool ignoreCurrentStatus = false, bool ignoreSafeguard = false)
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
            if (!ignoreSafeguard && other?.CanHitThroughSafeguard() == true && Team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
            {
                return PBEResult.Ineffective_Safeguard;
            }
            if (this.HasType(PBEType.Ice, useKnownInfo))
            {
                return PBEResult.Ineffective_Type;
            }
            PBEAbility kAbility = useKnownInfo ? KnownAbility : Ability;
            if (other?.HasCancellingAbility() != true && (kAbility == PBEAbility.MagmaArmor || (kAbility == PBEAbility.LeafGuard && Battle.WillLeafGuardActivate())))
            {
                return PBEResult.Ineffective_Ability;
            }
            return PBEResult.Success;
        }
        public PBEResult IsFlinchPossible(PBEBattlePokemon other, bool useKnownInfo = false)
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
        public PBEResult IsGrounded(PBEBattlePokemon other, bool useKnownInfo = false)
        {
            if (this.HasType(PBEType.Flying, useKnownInfo))
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
            if (this.HasType(PBEType.Grass, useKnownInfo))
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
        public PBEResult IsParalysisPossible(PBEBattlePokemon other, bool useKnownInfo = false, bool ignoreSubstitute = false, bool ignoreCurrentStatus = false, bool ignoreSafeguard = false)
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
            if (!ignoreSafeguard && other?.CanHitThroughSafeguard() == true && Team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
            {
                return PBEResult.Ineffective_Safeguard;
            }
            PBEAbility kAbility = useKnownInfo ? KnownAbility : Ability;
            if (other?.HasCancellingAbility() != true && (kAbility == PBEAbility.Limber || (kAbility == PBEAbility.LeafGuard && Battle.WillLeafGuardActivate())))
            {
                return PBEResult.Ineffective_Ability;
            }
            return PBEResult.Success;
        }
        public PBEResult IsPoisonPossible(PBEBattlePokemon other, bool useKnownInfo = false, bool ignoreSubstitute = false, bool ignoreCurrentStatus = false, bool ignoreSafeguard = false)
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
            if (!ignoreSafeguard && other?.CanHitThroughSafeguard() == true && Team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
            {
                return PBEResult.Ineffective_Safeguard;
            }
            if (this.HasType(PBEType.Poison, useKnownInfo) || this.HasType(PBEType.Steel, useKnownInfo))
            {
                return PBEResult.Ineffective_Type;
            }
            PBEAbility kAbility = useKnownInfo ? KnownAbility : Ability;
            if (other?.HasCancellingAbility() != true && (kAbility == PBEAbility.Immunity || (kAbility == PBEAbility.LeafGuard && Battle.WillLeafGuardActivate())))
            {
                return PBEResult.Ineffective_Ability;
            }
            return PBEResult.Success;
        }
        public PBEResult IsSleepPossible(PBEBattlePokemon other, bool useKnownInfo = false, bool ignoreSubstitute = false, bool ignoreCurrentStatus = false, bool ignoreSafeguard = false)
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
            if (!ignoreSafeguard && other?.CanHitThroughSafeguard() == true && Team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
            {
                return PBEResult.Ineffective_Safeguard;
            }
            PBEAbility kAbility = useKnownInfo ? KnownAbility : Ability;
            if (other?.HasCancellingAbility() != true && (kAbility == PBEAbility.Insomnia || (kAbility == PBEAbility.LeafGuard && Battle.WillLeafGuardActivate()) || kAbility == PBEAbility.VitalSpirit))
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
        public PBEResult IsTransformPossible(PBEBattlePokemon other, bool useKnownInfo = false)
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

        // Will only be accurate for the host
        public override string ToString()
        {
            var sb = new StringBuilder();
            string formStr = PBEDataUtils.HasForms(Species, false) ? $" ({PBEDataProvider.Instance.GetFormName(this).English})" : string.Empty;
            sb.AppendLine($"{Nickname}/{PBEDataProvider.Instance.GetSpeciesName(Species).English}{formStr} {Gender.ToSymbol()} Lv.{Level}");
            sb.AppendLine($"HP: {HP}/{MaxHP} ({HPPercentage:P2})");
            sb.Append($"Types: {PBEDataProvider.Instance.GetTypeName(Type1).English}");
            if (Type2 != PBEType.None)
            {
                sb.Append($"/{PBEDataProvider.Instance.GetTypeName(Type2).English}");
            }
            sb.AppendLine();
            sb.Append($"Known types: {PBEDataProvider.Instance.GetTypeName(KnownType1).English}");
            if (KnownType2 != PBEType.None)
            {
                sb.Append($"/{PBEDataProvider.Instance.GetTypeName(KnownType2).English}");
            }
            sb.AppendLine();
            sb.AppendLine($"Position: {Team.CombinedName}'s {FieldPosition}");
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
                formStr = PBEDataUtils.HasForms(KnownSpecies, false) ? $" ({PBEDataProvider.Instance.GetFormName(KnownSpecies, KnownForm).English})" : string.Empty;
                sb.AppendLine($"Disguised as: {KnownNickname}/{PBEDataProvider.Instance.GetSpeciesName(KnownSpecies).English}{formStr} {KnownGender.ToSymbol()}");
            }
            if (Battle.BattleFormat != PBEBattleFormat.Single)
            {
                if (Status2.HasFlag(PBEStatus2.Infatuated))
                {
                    sb.AppendLine($"Infatuated with: {InfatuatedWithPokemon.Trainer.Name}'s {InfatuatedWithPokemon.Nickname}");
                }
                if (Status2.HasFlag(PBEStatus2.LeechSeed))
                {
                    sb.AppendLine($"Seeded position: {SeededTeam.CombinedName}'s {SeededPosition}");
                }
                if (Status2.HasFlag(PBEStatus2.LockOn))
                {
                    sb.AppendLine($"Taking aim at: {LockOnPokemon.Trainer.Name}'s {LockOnPokemon.Nickname}");
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
            sb.AppendLine($"Ability: {PBEDataProvider.Instance.GetAbilityName(Ability).English}");
            sb.AppendLine($"Known ability: {(KnownAbility == PBEAbility.MAX ? "???" : PBEDataProvider.Instance.GetAbilityName(KnownAbility).English)}");
            sb.AppendLine($"Item: {PBEDataProvider.Instance.GetItemName(Item).English}");
            sb.AppendLine($"Known item: {(KnownItem == (PBEItem)ushort.MaxValue ? "???" : PBEDataProvider.Instance.GetItemName(KnownItem).English)}");
            if (Moves.Contains(PBEMoveEffect.Frustration) || Moves.Contains(PBEMoveEffect.Return))
            {
                sb.AppendLine($"Friendship: {Friendship} ({Friendship / byte.MaxValue:P2})");
            }
            if (Moves.Contains(PBEMoveEffect.HiddenPower))
            {
                sb.AppendLine($"{PBEDataProvider.Instance.GetMoveName(PBEMove.HiddenPower).English}: {PBEDataProvider.Instance.GetTypeName(IndividualValues.GetHiddenPowerType()).English}|{IndividualValues.GetHiddenPowerBasePower(Battle.Settings)}");
            }
            sb.Append("Moves: ");
            for (int i = 0; i < Battle.Settings.NumMoves; i++)
            {
                PBEBattleMoveset.PBEBattleMovesetSlot slot = Moves[i];
                PBEMove move = slot.Move;
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(PBEDataProvider.Instance.GetMoveName(slot.Move).English);
                if (move != PBEMove.None)
                {
                    sb.Append($" ({slot.PP}/{slot.MaxPP})");
                }
            }
            sb.AppendLine();
            sb.Append("Known moves: ");
            for (int i = 0; i < Battle.Settings.NumMoves; i++)
            {
                PBEBattleMoveset.PBEBattleMovesetSlot slot = KnownMoves[i];
                PBEMove move = slot.Move;
                int pp = slot.PP;
                int maxPP = slot.MaxPP;
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(move == PBEMove.MAX ? "???" : PBEDataProvider.Instance.GetMoveName(move).English);
                if (move != PBEMove.None && move != PBEMove.MAX)
                {
                    sb.Append($" ({pp}{(maxPP == 0 ? ")" : $"/{maxPP})")}");
                }
            }
            sb.AppendLine();
            sb.Append($"Usable moves: {string.Join(", ", GetUsableMoves().Select(m => PBEDataProvider.Instance.GetMoveName(m).English))}");
            return sb.ToString();
        }
    }
}
