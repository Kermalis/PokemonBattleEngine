using Kermalis.PokemonBattleEngine.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PPokemonShell
    {
        public PSpecies Species;
        public byte Level = PConstants.MaxLevel, Friendship = byte.MaxValue;
        public PAbility Ability;
        public PNature Nature = (PNature)PUtils.RNG.Next(0, (int)PNature.MAX);
        public PGender Gender = PGender.Genderless;
        public PItem Item;
        public byte[] EVs = new byte[6], IVs = new byte[6];
        public PMove[] Moves = new PMove[PConstants.NumMoves];

        public static void ValidateMany(IEnumerable<PPokemonShell> shells)
        {
            var arr = shells.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i].Validate();
            }
        }
        public void Validate()
        {
            // Validate Species
            PPokemonData pData;
            try
            {
                pData = PPokemonData.Data[Species];
            }
            catch
            {
                throw new ArgumentOutOfRangeException(nameof(Species));
            }

            // Validate Level
            if (Level == 0 || Level > PConstants.MaxLevel)
                throw new ArgumentOutOfRangeException(nameof(Level));

            // Validate Ability
            if (Ability != pData.Ability1 && Ability != pData.Ability2 && Ability != pData.AbilityHidden)
                throw new ArgumentOutOfRangeException(nameof(Ability));

            // Validate Nature
            if (Nature >= PNature.MAX)
                throw new ArgumentOutOfRangeException(nameof(Nature));

            // Validate Gender
            if ((Gender != PGender.Male && Gender != PGender.Female && Gender != PGender.Genderless)
                || (Gender == PGender.Male && (pData.GenderRatio == PGender.Female || pData.GenderRatio == PGender.Genderless))
                || (Gender == PGender.Female && (pData.GenderRatio == PGender.Male || pData.GenderRatio == PGender.Genderless))
                || (Gender == PGender.Genderless && pData.GenderRatio != PGender.Genderless)
                )
                throw new ArgumentOutOfRangeException(nameof(Gender));

            // Validate Item
            if (Item != PItem.None)
            {
                try
                {
                    var iData = PItemData.Data[Item];
                }
                catch
                {
                    throw new ArgumentOutOfRangeException(nameof(Item));
                }
            }

            // Validate EVs
            if (EVs == null || EVs.Length != 6 || EVs.Select(e => (int)e).Sum() > 510)
                throw new ArgumentOutOfRangeException(nameof(EVs));
            // Validate IVs
            if (IVs == null || IVs.Length != 6 || IVs.Any(e => e > 31))
                throw new ArgumentOutOfRangeException(nameof(IVs));

            // Validate Moves
            IEnumerable<PMove> legalMoves = pData.LevelUpMoves.Where(t => t.Item1 <= Level).Select(t => t.Item2).Union(pData.OtherMoves);
            if (Moves == null || Moves.Length > PConstants.NumMoves || Moves.Any(m => m != PMove.None && !legalMoves.Contains(m)) || Moves.All(m => m == PMove.None))
                throw new ArgumentOutOfRangeException(nameof(Moves));
        }

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes((ushort)Species));
            bytes.Add(Level);
            bytes.Add(Friendship);
            bytes.Add((byte)Ability);
            bytes.Add((byte)Nature);
            bytes.Add((byte)Gender);
            bytes.AddRange(BitConverter.GetBytes((ushort)Item));
            bytes.AddRange(EVs);
            bytes.AddRange(IVs);
            for (int i = 0; i < PConstants.NumMoves; i++)
                bytes.AddRange(BitConverter.GetBytes((ushort)Moves[i]));
            return bytes.ToArray();
        }
        internal static PPokemonShell FromBytes(BinaryReader r)
        {
            var pkmn = new PPokemonShell
            {
                Species = (PSpecies)r.ReadUInt16(),
                Level = r.ReadByte(),
                Friendship = r.ReadByte(),
                Ability = (PAbility)r.ReadByte(),
                Nature = (PNature)r.ReadByte(),
                Gender = (PGender)r.ReadByte(),
                Item = (PItem)r.ReadUInt16()
            };
            for (int j = 0; j < 6; j++)
                pkmn.EVs[j] = r.ReadByte();
            for (int j = 0; j < 6; j++)
                pkmn.IVs[j] = r.ReadByte();
            for (int j = 0; j < PConstants.NumMoves; j++)
                pkmn.Moves[j] = (PMove)r.ReadUInt16();
            return pkmn;
        }
    }

    public class PPokemon
    {
        public readonly PPokemonShell Shell;

        public PStatus Status;
        public PStatus2 Status2;
        public ushort HP, MaxHP, Attack, Defense, SpAttack, SpDefense, Speed;
        // These are in a set order; see BattleEffects->ApplyStatChange()
        public sbyte AttackChange, DefenseChange, SpAttackChange, SpDefenseChange, SpeedChange, AccuracyChange, EvasionChange;
        public byte[] PP = new byte[PConstants.NumMoves];

        public PPokemon(PPokemonShell shell)
        {
            Shell = shell;
            CalculateStats();
            HP = MaxHP;
        }

        static readonly Dictionary<PNature, sbyte[]> natureStatTable = new Dictionary<PNature, sbyte[]>
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
        void CalculateStats()
        {
            PPokemonData pData = PPokemonData.Data[Shell.Species];

            MaxHP = (ushort)(((2 * pData.HP + Shell.IVs[0] + (Shell.EVs[0] / 4)) * Shell.Level / PConstants.MaxLevel) + Shell.Level + 10);

            int i = 0;
            ushort OtherStat(byte baseVal)
            {
                double natureMultiplier = 1 + (natureStatTable[Shell.Nature][i] * 0.1);
                ushort val = (ushort)((((2 * baseVal + Shell.IVs[i + 1] + (Shell.EVs[i + 1] / 4)) * Shell.Level / PConstants.MaxLevel) + 5) * natureMultiplier);
                i++;
                return val;
            }
            Attack = OtherStat(pData.Attack);
            Defense = OtherStat(pData.Defense);
            SpAttack = OtherStat(pData.SpAttack);
            SpDefense = OtherStat(pData.SpDefense);
            Speed = OtherStat(pData.Speed);
        }

        public override string ToString() => $"{Shell.Species} Lv.{Shell.Level} {Shell.Nature} {Shell.Gender} {Shell.Ability} {HP}/{MaxHP} HP";
    }
}
