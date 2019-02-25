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
        public string NameWithGender => Shell.Nickname + GenderSymbol;
        /// <summary>
        /// The Pokémon's gender as a string.
        /// </summary>
        public string GenderSymbol => Shell.Gender == PBEGender.Female ? "♀" : Shell.Gender == PBEGender.Male ? "♂" : string.Empty;

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
        /// <summary>
        /// The Pokémon's defense stat.
        /// </summary>
        public ushort Defense { get; set; }
        /// <summary>
        /// The Pokémon's special attack stat.
        /// </summary>
        public ushort SpAttack { get; set; }
        /// <summary>
        /// The Pokémon's special defense stat.
        /// </summary>
        public ushort SpDefense { get; set; }
        /// <summary>
        /// The Pokémon's speed stat.
        /// </summary>
        public ushort Speed { get; set; }
        public PBEMove[] Moves { get; set; }
        public byte[] PP { get; set; }
        public byte[] MaxPP { get; set; }

        public PBEFieldPosition FieldPosition { get; set; }
        public PBEAbility Ability { get; set; }
        public PBEItem Item { get; set; }
        public PBEStatus1 Status1 { get; set; }
        public PBEStatus2 Status2 { get; set; }
        public PBEType Type1 { get; set; }
        public PBEType Type2 { get; set; }
        public double Weight { get; set; }

        public PBEGender KnownGender { get; set; }
        public string KnownNickname { get; set; }
        public bool KnownShiny { get; set; }
        public PBESpecies KnownSpecies { get; set; }

        public sbyte AttackChange { get; set; }
        public sbyte DefenseChange { get; set; }
        public sbyte SpAttackChange { get; set; }
        public sbyte SpDefenseChange { get; set; }
        public sbyte SpeedChange { get; set; }
        public sbyte AccuracyChange { get; set; }
        public sbyte EvasionChange { get; set; }

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
        public PBEPokemon(PBETeam team, byte id, PBEPokemonShell shell)
        {
            Team = team;
            SelectedAction.PokemonId = Id = id;
            Shell = shell;
            KnownGender = Shell.Gender;
            KnownNickname = Shell.Nickname;
            KnownShiny = Shell.Shiny;
            KnownSpecies = Shell.Species;
            Ability = Shell.Ability;
            Item = Shell.Item;
            CalculateStats();
            HP = MaxHP;
            HPPercentage = 1.0;
            Moves = Shell.Moves;
            PP = new byte[Moves.Length];
            MaxPP = new byte[Moves.Length];
            for (int i = 0; i < Moves.Length; i++)
            {
                PBEMove move = Shell.Moves[i];
                if (move != PBEMove.None)
                {
                    byte tier = PBEMoveData.Data[move].PPTier;
                    PP[i] = MaxPP[i] = (byte)Math.Max(1, (tier * Team.Battle.Settings.PPMultiplier) + (tier * Shell.PPUps[i]));
                }
            }
            PBEPokemonData pData = PBEPokemonData.Data[Shell.Species];
            Type1 = pData.Type1;
            Type2 = pData.Type2;
            Weight = pData.Weight;
            Team.Party.Add(this);
        }
        // This constructor is to define a remote Pokémon
        public PBEPokemon(PBETeam team, PBEPkmnSwitchInPacket.PBESwitchInInfo info)
        {
            Team = team;
            Id = info.PokemonId;
            Shell = new PBEPokemonShell
            {
                Species = KnownSpecies = info.Species,
                Shiny = KnownShiny = info.Shiny,
                Nickname = KnownNickname = info.Nickname,
                Level = info.Level,
                Gender = KnownGender = info.Gender,
                Ability = Ability = PBEAbility.MAX,
                Item = Item = (PBEItem)ushort.MaxValue,
                Nature = PBENature.MAX,
                Moves = new PBEMove[Team.Battle.Settings.NumMoves],
                PPUps = new byte[Team.Battle.Settings.NumMoves],
                EVs = new byte[6],
                IVs = new byte[6]
            };
            Moves = new PBEMove[Shell.Moves.Length];
            PP = new byte[Moves.Length];
            MaxPP = new byte[Moves.Length];
            for (int i = 0; i < Moves.Length; i++)
            {
                Shell.Moves[i] = Moves[i] = PBEMove.MAX;
            }
            FieldPosition = info.FieldPosition;
            HP = info.HP;
            MaxHP = info.MaxHP;
            HPPercentage = info.HPPercentage;
            Status1 = info.Status1;
            PBEPokemonData pData = PBEPokemonData.Data[Shell.Species];
            Type1 = pData.Type1;
            Type2 = pData.Type2;
            Weight = pData.Weight;
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

        /// <summary>
        /// Calculates and sets the Pokémon's stats based on its level, IVs, EVs, nature, and species.
        /// </summary>
        void CalculateStats()
        {
            PBEPokemonData pData = PBEPokemonData.Data[Shell.Species];

            MaxHP = (ushort)(Shell.Species == PBESpecies.Shedinja ? 1 : (((2 * pData.HP + Shell.IVs[0] + (Shell.EVs[0] / 4)) * Shell.Level / Team.Battle.Settings.MaxLevel) + Shell.Level + 10));

            int i = 0;
            ushort OtherStat(byte baseVal)
            {
                double natureMultiplier = 1.0 + (PBEPokemonData.NatureBoosts[Shell.Nature][i] * Team.Battle.Settings.NatureStatBoost);
                ushort val = (ushort)((((2 * baseVal + Shell.IVs[i + 1] + (Shell.EVs[i + 1] / 4)) * Shell.Level / Team.Battle.Settings.MaxLevel) + 5) * natureMultiplier);
                i++;
                return val;
            }
            Attack = OtherStat(pData.Attack);
            Defense = OtherStat(pData.Defense);
            SpAttack = OtherStat(pData.SpAttack);
            SpDefense = OtherStat(pData.SpDefense);
            Speed = OtherStat(pData.Speed);
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
            Ability = Shell.Ability;
            KnownGender = Shell.Gender;
            KnownNickname = Shell.Nickname;
            KnownShiny = Shell.Shiny;
            KnownSpecies = Shell.Species;

            AttackChange = DefenseChange = SpAttackChange = SpDefenseChange = SpeedChange = AccuracyChange = EvasionChange = 0;

            if (Status1 == PBEStatus1.Asleep)
            {
                Status1Counter = SleepTurns;
            }
            else if (Status1 == PBEStatus1.BadlyPoisoned)
            {
                Status1Counter = 1;
            }

            Status2 = PBEStatus2.None;
            ConfusionCounter = ConfusionTurns = 0;
            DisguisedAsPokemon = null;
            SeededPosition = PBEFieldPosition.None;
            SubstituteHP = 0;

            TempLockedMove = ChoiceLockedMove = PBEMove.None;
            TempLockedTargets = PBETarget.None;

            ExecutedMoves.Clear();

            if (Id != byte.MaxValue)
            {
                CalculateStats();
            }
        }

        /// <summary>
        /// Transforms into <paramref name="target"/> and sets <see cref="PBEStatus2.Transformed"/>.
        /// </summary>
        /// <param name="target">The Pokémon to transform into.</param>
        /// <remarks>Frees the Pokémon of its <see cref="ChoiceLockedMove"/>.</remarks>
        public void Transform(PBEPokemon target)
        {
            KnownSpecies = target.Shell.Species;
            KnownShiny = target.Shell.Shiny;
            KnownGender = target.Shell.Gender;
            Ability = target.Ability;
            Type1 = target.Type1;
            Type2 = target.Type2;
            Weight = target.Weight;
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
            Moves = (PBEMove[])target.Moves.Clone();
            if (Id != byte.MaxValue)
            {
                for (int i = 0; i < Moves.Length; i++)
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
            int a = Shell.IVs[0] & 1,
                b = Shell.IVs[1] & 1,
                c = Shell.IVs[2] & 1,
                d = Shell.IVs[5] & 1,
                e = Shell.IVs[3] & 1,
                f = Shell.IVs[4] & 1;
            return PBEPokemonData.HiddenPowerTypes[((1 << 0) * a + (1 << 1) * b + (1 << 2) * c + (1 << 3) * d + (1 << 4) * e + (1 << 5) * f) * (PBEPokemonData.HiddenPowerTypes.Count - 1) / ((1 << 6) - 1)];
        }
        /// <summary>
        /// Gets the base power that <see cref="PBEMove.HiddenPower"/> will have when used by this Pokémon.
        /// </summary>
        public byte GetHiddenPowerBasePower()
        {
            const byte mininumBasePower = 30,
                maximumBasePower = 70;
            int a = (Shell.IVs[0] & 2) == 2 ? 1 : 0,
                b = (Shell.IVs[1] & 2) == 2 ? 1 : 0,
                c = (Shell.IVs[2] & 2) == 2 ? 1 : 0,
                d = (Shell.IVs[5] & 2) == 2 ? 1 : 0,
                e = (Shell.IVs[3] & 2) == 2 ? 1 : 0,
                f = (Shell.IVs[4] & 2) == 2 ? 1 : 0;
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
                if ((ex.Move == PBEMove.Detect || ex.Move == PBEMove.Protect || ex.Move == PBEMove.WideGuard) && ex.FailReason == PBEFailReason.None)
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

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Shell.Nickname}/{Shell.Species} {GenderSymbol} Lv.{Shell.Level}");
            if (Id == byte.MaxValue)
            {
                sb.AppendLine($"HP: {HPPercentage:P2}");
            }
            else
            {
                sb.AppendLine($"HP: {HP}/{MaxHP} ({HPPercentage:P2})");
            }
            sb.AppendLine($"Position: {FieldPosition}");
            sb.AppendLine($"Type1: {Type1}");
            sb.AppendLine($"Type2: {Type2}");
            sb.AppendLine($"Status1: {Status1}");
            sb.AppendLine($"Status2: {Status2}");
            if (Id != byte.MaxValue && Status2.HasFlag(PBEStatus2.Substitute))
            {
                sb.AppendLine($"Substitute HP: {SubstituteHP}");
            }
            if (Id != byte.MaxValue)
            {
                sb.AppendLine($"Stats: A: {Attack} D: {Defense} SA: {SpAttack} SD: {SpDefense} S: {Speed}");
            }
            sb.AppendLine($"Stat changes: A: {AttackChange} D: {DefenseChange} SA: {SpAttackChange} SD: {SpDefenseChange} S: {SpeedChange} AC: {AccuracyChange} E: {EvasionChange}");
            sb.AppendLine($"Item: {(Item == (PBEItem)ushort.MaxValue ? "???" : PBEItemLocalization.Names[Item].English)}");
            sb.AppendLine($"Ability: {(Ability == PBEAbility.MAX ? "???" : PBEAbilityLocalization.Names[Ability].English)}");
            if (Id != byte.MaxValue)
            {
                sb.AppendLine($"Nature: {(Shell.Nature == PBENature.MAX ? "???" : Shell.Nature.ToString())}");
            }
            if (Id != byte.MaxValue)
            {
                sb.AppendLine($"Hidden Power: {GetHiddenPowerType()}/{GetHiddenPowerBasePower()}");
            }
            string[] moveStrs = new string[Moves.Length];
            for (int i = 0; i < moveStrs.Length; i++)
            {
                string mStr = Moves[i] == PBEMove.MAX ? "???" : PBEMoveLocalization.Names[Moves[i]].English;
                if (Id != byte.MaxValue)
                {
                    mStr += $" {PP[i]}/{MaxPP[i]}";
                }
                moveStrs[i] = mStr;
            }
            sb.Append($"Moves: {string.Join(", ", moveStrs)}");
            return sb.ToString();
        }
    }
}
