using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Packets;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Data
{
    /// <summary>
    /// Represents a specific Pokémon during a battle.
    /// </summary>
    public sealed class PBEPokemon
    {
        // TODO: Include battle this pokemon belongs to

        /// <summary>
        /// The Pokémon's ID for the current battle.
        /// </summary>
        public byte Id { get; }
        /// <summary>
        /// True indicates the Pokémon is owned by team 0, False indicates the Pokémon is owned by team 1.
        /// </summary>
        public bool LocalTeam { get; }
        /// <summary>
        /// The shell that was used to construct this Pokémon.
        /// </summary>
        public PBEPokemonShell Shell { get; }

        /// <summary>
        /// Returns a string that represents the relation of this Pokémon to the trainer.
        /// </summary>
        /// <param name="firstLetterCapitalized">True if this string will be used at the start of a sentence, False otherwise.</param>
        public string NameForTrainer(bool firstLetterCapitalized)
        {
            string prefix;
            if (firstLetterCapitalized)
            {
                if (LocalTeam)
                {
                    prefix = "";
                }
                else
                {
                    prefix = "The foe's ";
                }
            }
            else
            {
                if (LocalTeam)
                {
                    prefix = "";
                }
                else
                {
                    prefix = "the foe's ";
                }
            }
            return prefix + Shell.Nickname;
        }
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

        public PBESpecies Species { get; set; }
        public bool Shiny { get; set; }
        public PBEAbility Ability { get; set; }
        public PBEType Type1 { get; set; }
        public PBEType Type2 { get; set; }
        public double Weight { get; set; }
        public PBEItem Item { get; set; }
        public PBEFieldPosition FieldPosition { get; set; }
        public PBEStatus1 Status1 { get; set; }
        public PBEStatus2 Status2 { get; set; }
        // These are in a set order; see BattleEffects->ApplyStatChange()
        public sbyte AttackChange, DefenseChange, SpAttackChange, SpDefenseChange, SpeedChange, AccuracyChange, EvasionChange;

        /// <summary>
        /// The counter used for <see cref="PBEStatus1.BadlyPoisoned"/> and <see cref="PBEStatus1.Asleep"/>.
        /// </summary>
        public byte Status1Counter { get; set; }
        /// <summary>
        /// The amount of turns the Pokémon will sleep for before waking.
        /// </summary>
        public byte SleepTurns { get; internal set; }
        /// <summary>
        /// The counter used for <see cref="PBEStatus2.Confused"/>.
        /// </summary>
        public byte ConfusionCounter { get; set; }
        /// <summary>
        /// The amount of turns the Pokémon will be confused for before snapping out of it.
        /// </summary>
        public byte ConfusionTurns { get; internal set; }
        /// <summary>
        /// The amount of consecutive turns the Pokémon has used protection.
        /// </summary>
        public byte ProtectCounter { get; set; }
        /// <summary>
        /// The amount of HP the Pokémon's <see cref="PBEStatus2.Substitute"/> has left.
        /// </summary>
        public ushort SubstituteHP { get; set; }
        /// <summary>
        /// The position to return <see cref="PBEStatus2.LeechSeed"/> HP to on the opposing team.
        /// </summary>
        public PBEFieldPosition SeededPosition { get; set; }

        public PBEAction PreviousAction, SelectedAction;
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

        // Stats & PP are set from the shell info
        public PBEPokemon(bool localTeam, byte id, PBEPokemonShell shell, PBESettings settings)
        {
            LocalTeam = localTeam;
            Id = id;
            Shell = shell;
            Species = Shell.Species;
            Shiny = Shell.Shiny;
            Ability = Shell.Ability;
            Item = Shell.Item;
            SelectedAction.PokemonId = id;
            CalculateStats(settings);
            HP = MaxHP;
            Moves = Shell.Moves;
            PP = new byte[Moves.Length];
            MaxPP = new byte[Moves.Length];
            for (int i = 0; i < Moves.Length; i++)
            {
                PBEMove move = Shell.Moves[i];
                if (move != PBEMove.None)
                {
                    byte tier = PBEMoveData.Data[move].PPTier;
                    int movePP = (tier * settings.PPMultiplier) + (tier * Shell.PPUps[i]);
                    PP[i] = MaxPP[i] = (byte)movePP;
                }
            }
            PBEPokemonData pData = PBEPokemonData.Data[Species];
            Type1 = pData.Type1;
            Type2 = pData.Type2;
            Weight = pData.Weight;
        }
        // This constructor is to define an unknown remote Pokémon
        public PBEPokemon(bool localTeam, PBEPkmnSwitchInPacket.PBESwitchInInfo info, PBESettings settings)
        {
            LocalTeam = localTeam;
            Id = info.PokemonId;
            Shell = new PBEPokemonShell
            {
                Species = info.Species,
                Shiny = info.Shiny,
                Nickname = info.Nickname,
                Level = info.Level,
                Gender = info.Gender,
                Ability = PBEAbility.MAX,
                Item = PBEItem.MAX,
                Nature = PBENature.MAX,
                Moves = new PBEMove[settings.NumMoves],
                PPUps = new byte[settings.NumMoves],
                EVs = new byte[6],
                IVs = new byte[6]
            };
            Species = info.Species;
            Shiny = info.Shiny;
            Ability = PBEAbility.MAX;
            Item = PBEItem.MAX;
            Moves = new PBEMove[settings.NumMoves];
            PP = new byte[Moves.Length];
            MaxPP = new byte[Moves.Length];
            for (int i = 0; i < Moves.Length; i++)
            {
                Moves[i] = PBEMove.MAX;
            }
            PBEPokemonData pData = PBEPokemonData.Data[Species];
            Type1 = pData.Type1;
            Type2 = pData.Type2;
            Weight = pData.Weight;
        }

        /// <summary>
        /// Returns True if the Pokémon has <paramref name="type"/>, False otherwise.
        /// </summary>
        /// <param name="type">The type to check.</param>
        public bool HasType(PBEType type) => Type1 == type || Type2 == type;

        /// <summary>
        /// Calculates and sets the Pokémon's stats based on its level, IVs, EVs, nature, and species.
        /// </summary>
        /// <param name="settings"></param>
        void CalculateStats(PBESettings settings)
        {
            PBEPokemonData pData = PBEPokemonData.Data[Species];

            MaxHP = (ushort)(((2 * pData.HP + Shell.IVs[0] + (Shell.EVs[0] / 4)) * Shell.Level / settings.MaxLevel) + Shell.Level + 10);

            int i = 0;
            ushort OtherStat(byte baseVal)
            {
                double natureMultiplier = 1 + (PBEPokemonData.NatureBoosts[Shell.Nature][i] * settings.NatureStatBoost);
                ushort val = (ushort)((((2 * baseVal + Shell.IVs[i + 1] + (Shell.EVs[i + 1] / 4)) * Shell.Level / settings.MaxLevel) + 5) * natureMultiplier);
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
        /// <param name="settings">The battle settings.</param>
        public void ClearForSwitch(PBESettings settings)
        {
            FieldPosition = PBEFieldPosition.None;
            Species = Shell.Species;
            Ability = Shell.Ability;
            Shiny = Shell.Shiny;

            AttackChange = DefenseChange = SpAttackChange = SpDefenseChange = SpeedChange = AccuracyChange = EvasionChange = 0;

            if (Status1 == PBEStatus1.Asleep)
            {
                Status1Counter = SleepTurns;
            }
            else if (Status1 == PBEStatus1.BadlyPoisoned)
            {
                Status1Counter = 1;
            }
            Status2 &= ~PBEStatus2.Confused;
            ConfusionCounter = ConfusionTurns = 0;
            Status2 &= ~PBEStatus2.Cursed;
            Status2 &= ~PBEStatus2.LeechSeed;
            SeededPosition = PBEFieldPosition.None;
            Status2 &= ~PBEStatus2.Minimized;
            Status2 &= ~PBEStatus2.Pumped;
            Status2 &= ~PBEStatus2.Substitute;
            SubstituteHP = 0;
            Status2 &= ~PBEStatus2.Transformed;

            TempLockedMove = ChoiceLockedMove = PBEMove.None;
            TempLockedTargets = PBETarget.None;

            if (Shell.Nature != PBENature.MAX) // If the nature is unset, the program is not the host and does not own the Pokémon
            {
                CalculateStats(settings);
            }
        }

        /// <summary>
        /// Transforms into <paramref name="target"/> and sets <see cref="PBEStatus2.Transformed"/>.
        /// </summary>
        /// <param name="target">The Pokémon to transform into.</param>
        /// <param name="settings">The battle settings.</param>
        /// <remarks>Frees the Pokémon of its <see cref="ChoiceLockedMove"/>.</remarks>
        public void Transform(PBEPokemon target, PBESettings settings)
        {
            Species = target.Species;
            Shiny = target.Shiny;
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
            for (int i = 0; i < Moves.Length; i++)
            {
                byte pp = Moves[i] == PBEMove.None ? (byte)0 : settings.PPMultiplier;
                PP[i] = MaxPP[i] = pp;
            }
            ChoiceLockedMove = PBEMove.None;
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
            return PBEPokemonData.HiddenPowerTypes[((1 << 0) * a + (1 << 1) * b + (1 << 2) * c + (1 << 3) * d + (1 << 4) * e + (1 << 5) * f) * (PBEPokemonData.HiddenPowerTypes.Length - 1) / ((1 << 6) - 1)];
        }
        /// <summary>
        /// Gets the base power that <see cref="PBEMove.HiddenPower"/> will have when used by this Pokémon.
        /// </summary>
        public int GetHiddenPowerBasePower()
        {
            int a = (Shell.IVs[0] & 2) == 2 ? 1 : 0,
                b = (Shell.IVs[1] & 2) == 2 ? 1 : 0,
                c = (Shell.IVs[2] & 2) == 2 ? 1 : 0,
                d = (Shell.IVs[5] & 2) == 2 ? 1 : 0,
                e = (Shell.IVs[3] & 2) == 2 ? 1 : 0,
                f = (Shell.IVs[4] & 2) == 2 ? 1 : 0;
            // 30 is minimum, 30+40 is maximum
            return (((1 << 0) * a + (1 << 1) * b + (1 << 2) * c + (1 << 3) * d + (1 << 4) * e + (1 << 5) * f) * 40 / ((1 << 6) - 1)) + 30;
        }

        // ToBytes() and FromBytes() will only be used when the server sends you your team Ids, so they do not need to contain all info
        internal byte[] ToBytes(PBESettings settings)
        {
            var bytes = new List<byte>();
            bytes.Add(Id);
            bytes.AddRange(Shell.ToBytes(settings));
            return bytes.ToArray();
        }
        internal static PBEPokemon FromBytes(BinaryReader r, PBESettings settings)
        {
            return new PBEPokemon(true, r.ReadByte(), PBEPokemonShell.FromBytes(r, settings), settings);
        }

        public override bool Equals(object obj)
        {
            if (obj is PBEPokemon other)
            {
                return other.Id.Equals(Id);
            }
            return base.Equals(obj);
        }
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString()
        {
            bool remotePokemon = Shell.Nature == PBENature.MAX; // If the nature is unset, the program is not the host and does not own the Pokémon

            string item = Item.ToString().Replace("MAX", "???");
            string nature = Shell.Nature.ToString().Replace("MAX", "???");
            string ability = Ability.ToString().Replace("MAX", "???");
            string[] moveStrs = new string[Moves.Length];
            for (int i = 0; i < moveStrs.Length; i++)
            {
                string mStr = Moves[i].ToString().Replace("MAX", "???");
                if (!remotePokemon)
                {
                    mStr += $" {PP[i]}/{MaxPP[i]}";
                }
                moveStrs[i] = mStr;
            }
            string moves = moveStrs.Print(false);

            var sb = new StringBuilder();
            sb.AppendLine($"{Shell.Nickname}/{Species} {GenderSymbol} Lv.{Shell.Level}");
            sb.AppendLine($"HP: {HP}/{MaxHP} ({(double)HP / MaxHP:P2})");
            sb.AppendLine($"Position: {FieldPosition}");
            sb.AppendLine($"Type1: {Type1}");
            sb.AppendLine($"Type2: {Type2}");
            sb.AppendLine($"Status1: {Status1}");
            sb.AppendLine($"Status2: {Status2}");
            if (!remotePokemon && Status2.HasFlag(PBEStatus2.Substitute))
            {
                sb.AppendLine($"Substitute HP: {SubstituteHP}");
            }
            if (!remotePokemon)
            {
                sb.AppendLine($"Stats: {Attack} {Defense} {SpAttack} {SpDefense} {Speed}");
            }
            sb.AppendLine($"Stat changes: {AttackChange} {DefenseChange} {SpAttackChange} {SpDefenseChange} {SpeedChange} {AccuracyChange} {EvasionChange}");
            sb.AppendLine($"Item: {item}");
            sb.AppendLine($"Ability: {ability}");
            if (!remotePokemon)
            {
                sb.AppendLine($"Nature: {nature}");
            }
            if (!remotePokemon)
            {
                sb.AppendLine($"Hidden Power: {GetHiddenPowerType()}/{GetHiddenPowerBasePower()}");
            }
            sb.Append($"Moves: {moves}");
            return sb.ToString();
        }
    }
}
