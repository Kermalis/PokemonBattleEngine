using PokemonBattleEngine.Util;
using System;
using System.Linq;

namespace PokemonBattleEngine.Data
{
    class Pokemon
    {
        public readonly Species Species;
        public readonly Gender Gender;
        public readonly Nature Nature;
        public int HP;
        public int MaxHP { get; private set; }
        public int Attack, Defense, SpAttack, SpDefense, Speed;
        public int Level { get; private set; }
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

            // Temporary
            HP = MaxHP = Attack = Defense = SpAttack = SpDefense = Speed = 100;
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
