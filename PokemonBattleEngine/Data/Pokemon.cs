using Kermalis.PokemonBattleEngine.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public class Pokemon
    {
        public readonly Species Species;
        public readonly Gender Gender;
        public readonly Nature Nature;
        public int HP, MaxHP;
        public int Attack, Defense, SpAttack, SpDefense, Speed;
        public int Level;
        public Status Status;
        public Ability Ability;
        public Item Item;

        public Move[] Moves = new Move[Constants.NumMoves];
        public int[] PP = new int[Constants.NumMoves];

        public Pokemon(Species species, int level = 1, Item item = Item.None, Move[] moves = null, Nature nature = Nature.MAX)
        {
            PokemonData pData = PokemonData.Data[species];

            Species = species;
            Gender = GetRandomGenderForSpecies(Species);
            Ability = pData.Ability1; // Temporary
            Level = level;
            Item = item;

            if (moves == null)
                SetDefaultMovesForCurrentLevel();
            else
                SetMovesAndPP(moves);

            if (nature >= Nature.MAX)
                Nature = (Nature)Utils.RNG.Next(0, (int)Nature.MAX);

            CalculateStats();
            HP = MaxHP;
        }

        static readonly Dictionary<Nature, sbyte[]> NatureStatTable = new Dictionary<Nature, sbyte[]>
        {
            //                               Atk   Def SpAtk SpDef   Spd
            { Nature.Adamant, new sbyte[] {   +1,    0,   -1,    0,    0} },
            { Nature.Bashful, new sbyte[] {    0,    0,    0,    0,    0} },
            { Nature.Bold,    new sbyte[] {   -1,   +1,    0,    0,    0} },
            { Nature.Brave,   new sbyte[] {   +1,    0,    0,    0,   -1} },
            { Nature.Calm,    new sbyte[] {   -1,    0,    0,   +1,    0} },
            { Nature.Careful, new sbyte[] {    0,    0,   -1,   +1,    0} },
            { Nature.Docile,  new sbyte[] {    0,    0,    0,    0,    0} },
            { Nature.Gentle,  new sbyte[] {    0,   -1,    0,   +1,    0} },
            { Nature.Hardy,   new sbyte[] {    0,    0,    0,    0,    0} },
            { Nature.Hasty,   new sbyte[] {    0,   -1,    0,    0,   +1} },
            { Nature.Impish,  new sbyte[] {    0,   +1,   -1,    0,    0} },
            { Nature.Jolly,   new sbyte[] {    0,    0,   -1,    0,   +1} },
            { Nature.Lax,     new sbyte[] {    0,   +1,    0,   -1,    0} },
            { Nature.Loney,   new sbyte[] {   +1,   -1,    0,    0,    0} },
            { Nature.Mild,    new sbyte[] {    0,   -1,   +1,    0,    0} },
            { Nature.Modest,  new sbyte[] {   -1,    0,   +1,    0,    0} },
            { Nature.Naive,   new sbyte[] {    0,    0,    0,   -1,   +1} },
            { Nature.Naughty, new sbyte[] {   +1,    0,    0,   -1,    0} },
            { Nature.Quiet,   new sbyte[] {    0,    0,   +1,    0,   -1} },
            { Nature.Quirky,  new sbyte[] {    0,    0,    0,    0,    0} },
            { Nature.Rash,    new sbyte[] {    0,    0,   +1,   -1,    0} },
            { Nature.Relaxed, new sbyte[] {    0,   +1,    0,    0,   -1} },
            { Nature.Sassy,   new sbyte[] {    0,    0,    0,   +1,   -1} },
            { Nature.Serious, new sbyte[] {    0,    0,    0,    0,    0} },
            { Nature.Timid,   new sbyte[] {   -1,    0,    0,    0,   +1} },
        };
        void CalculateStats(int[] IVs = null, int[] EVs = null)
        {
            PokemonData pData = PokemonData.Data[Species];

            if (IVs == null)
                IVs = new int[6];
            if (EVs == null)
                EVs = new int[6];

            MaxHP = ((2 * pData.HP + IVs[0] + (EVs[0] / 4)) * Level / Constants.MaxLevel) + Level + 10;
            
            int i = 0;
            int OtherStat(int baseVal)
            {
                double natureMultiplier = 1 + (NatureStatTable[Nature][i] * 0.1);
                int val = (int)((((2 * baseVal + IVs[i + 1] + (EVs[i + 1] / 4)) * Level / Constants.MaxLevel) + 5) * natureMultiplier);
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
            PokemonData pData = PokemonData.Data[Species];

            Move[] learnedMoves = pData.LevelUpMoves.Where(t => t.Item1 <= Level).Select(t => t.Item2).ToArray();
            SetMovesAndPP(learnedMoves);
        }
        void SetMovesAndPP(Move[] moves)
        {
            int min = Math.Min(Constants.NumMoves, moves.Length);
            // Copy last "min" amount of moves from the array
            for (int i = min - 1; i >= 0; i--)
            {
                Move newMove = moves[i];
                Moves[i] = newMove;
                PP[i] = MoveData.Data[newMove].PP;
            }
        }

        static Gender GetRandomGenderForSpecies(Species species)
        {
            PokemonData pData = PokemonData.Data[species];

            switch (pData.GenderRatio)
            {
                case Gender.Male:
                case Gender.Female:
                case Gender.Genderless:
                    return pData.GenderRatio;
            }

            byte b = (byte)Utils.RNG.Next(0, byte.MaxValue + 1);
            if ((byte)pData.GenderRatio > b)
                return Gender.Female;
            else
                return Gender.Male;
        }

        public override string ToString() => $"{Species} Lv.{Level} {Nature} {Gender} {Ability} {HP}/{MaxHP} HP";
    }
}
