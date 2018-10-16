using Kermalis.PokemonBattleEngine.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PPokemonShell
    {
        // TODO: Gender
        public PSpecies Species;
        public byte Level = PConstants.MaxLevel, Friendship = byte.MaxValue;
        public PAbility Ability;
        public PNature Nature = (PNature)PUtils.RNG.Next(0, (int)PNature.MAX);
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
                throw new ArgumentOutOfRangeException("Species");
            }

            // Validate Level
            if (Level == 0 || Level > PConstants.MaxLevel)
                throw new ArgumentOutOfRangeException("Level");

            // Validate Ability
            if (Ability != pData.Ability1 && Ability != pData.Ability2 && Ability != pData.AbilityHidden)
                throw new ArgumentOutOfRangeException("Ability");

            // Validate Nature
            if (Nature >= PNature.MAX)
                throw new ArgumentOutOfRangeException("Nature");

            // Validate Item
            if (Item != PItem.None)
            {
                try
                {
                    var iData = PItemData.Data[Item];
                }
                catch
                {
                    throw new ArgumentOutOfRangeException("Item");
                }
            }

            // Validate EVs
            if (EVs == null || EVs.Length != 6)
                throw new ArgumentOutOfRangeException("EVs");
            int evTotal = 0;
            for (int i = 0; i < 6; i++)
            {
                evTotal += EVs[i];
                if (evTotal > 510)
                    throw new ArgumentOutOfRangeException("EVs");
            }
            // Validate IVs
            if (IVs == null || IVs.Length != 6)
                throw new ArgumentOutOfRangeException("IVs");
            if (IVs.Any(e => e > 31))
                throw new ArgumentOutOfRangeException("IVs");

            // Validate Moves
            // Moves being full of PMove.None will force the moves to be chosen in the Pokemon class
            if (Moves == null || Moves.Length > PConstants.NumMoves)
                throw new ArgumentOutOfRangeException("Moves");
            if (Moves.Any(m => m != PMove.None))
            {
                IEnumerable<PMove> legalMoves = pData.LevelUpMoves.Select(t => t.Item2).Concat(pData.OtherMoves);
                if (Moves.Any(m => !legalMoves.Contains(m)))
                    throw new ArgumentOutOfRangeException("Moves");
            }
        }
    }

    internal class PPokemon
    {
        public readonly PSpecies Species;
        public readonly PGender Gender;
        public readonly PNature Nature;
        public ushort HP, MaxHP, Attack, Defense, SpAttack, SpDefense, Speed;
        public byte Level, Friendship;
        public PStatus Status;
        public PAbility Ability;
        public PItem Item;

        public byte[] EVs = new byte[6], IVs = new byte[6];
        public PMove[] Moves = new PMove[PConstants.NumMoves];
        public byte[] PP = new byte[PConstants.NumMoves];

        public PPokemon(PPokemonShell shell)
        {
            Species = shell.Species;
            PPokemonData pData = PPokemonData.Data[Species];

            Gender = GetRandomGenderForSpecies(Species);

            Ability = shell.Ability;
            Level = shell.Level;
            Friendship = shell.Friendship;
            Item = shell.Item;
            Nature = shell.Nature;

            for (int i = 0; i < 6; i++)
                EVs[i] = shell.EVs[i];
            for (int i = 0; i < 6; i++)
                IVs[i] = shell.IVs[i];

            if (shell.Moves.All(m => m == PMove.None))
                SetDefaultMovesForCurrentLevel();
            else
                SetMovesAndPP(shell.Moves);

            CalculateStats();
            HP = MaxHP;
        }

        static readonly Dictionary<PNature, sbyte[]> NatureStatTable = new Dictionary<PNature, sbyte[]>
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
            PPokemonData pData = PPokemonData.Data[Species];

            MaxHP = (ushort)(((2 * pData.HP + IVs[0] + (EVs[0] / 4)) * Level / PConstants.MaxLevel) + Level + 10);

            int i = 0;
            ushort OtherStat(byte baseVal)
            {
                double natureMultiplier = 1 + (NatureStatTable[Nature][i] * 0.1);
                ushort val = (ushort)((((2 * baseVal + IVs[i + 1] + (EVs[i + 1] / 4)) * Level / PConstants.MaxLevel) + 5) * natureMultiplier);
                i++;
                return val;
            }
            Attack = OtherStat(pData.Attack);
            Defense = OtherStat(pData.Defense);
            SpAttack = OtherStat(pData.SpAttack);
            SpDefense = OtherStat(pData.SpDefense);
            Speed = OtherStat(pData.Speed);
        }

        void SetDefaultMovesForCurrentLevel()
        {
            PPokemonData pData = PPokemonData.Data[Species];

            PMove[] learnedMoves = pData.LevelUpMoves.Where(t => t.Item1 <= Level).Select(t => t.Item2).ToArray();
            SetMovesAndPP(learnedMoves);
        }
        void SetMovesAndPP(PMove[] moves)
        {
            int min = Math.Min(PConstants.NumMoves, moves.Length);
            // Copy last "min" amount of moves from the array
            for (int i = min - 1; i >= 0; i--)
            {
                PMove newMove = moves[i];
                Moves[i] = newMove;
                PP[i] = PMoveData.Data[newMove].PP;
            }
        }

        static PGender GetRandomGenderForSpecies(PSpecies species)
        {
            PPokemonData pData = PPokemonData.Data[species];

            switch (pData.GenderRatio)
            {
                case PGender.Male:
                case PGender.Female:
                case PGender.Genderless:
                    return pData.GenderRatio;
            }

            byte b = (byte)PUtils.RNG.Next(0, byte.MaxValue + 1);
            if ((byte)pData.GenderRatio > b)
                return PGender.Female;
            else
                return PGender.Male;
        }

        public override string ToString() => $"{Species} Lv.{Level} {Nature} {Gender} {Ability} {HP}/{MaxHP} HP";
    }
}
