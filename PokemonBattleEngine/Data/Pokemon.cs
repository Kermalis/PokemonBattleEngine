using PokemonBattleEngine.Util;
using System;
using System.Linq;

namespace PokemonBattleEngine.Data
{
    class Pokemon
    {
        public Status Status;
        public Item Item;
        public int HP;
        public int MaxHP { get; private set; }
        public int Level { get; private set; }

        public Move[] Moves = new Move[Constants.NumMoves];
        public byte[] PP = new byte[Constants.NumMoves];

        public readonly Species Species;
        public uint Personality { get; private set; }
        
        public Pokemon(Species species, int level = 1, Item item = Item.None, Move[] moves = null, uint personality = 0)
        {
            // Set species
            Species = species;

            // Set personality
            if (personality == 0)
                SetRandomPersonality();
            else
                Personality = personality;

            // Set level
            Level = level;

            // Set item
            Item = item;

            // Set moves
            if (moves == null)
                SetDefaultMovesForCurrentLevel();
            else
                SetMovesAndPP(moves);
        }

        void SetRandomPersonality() => Personality = Utils.GetRandomUint();

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

        static Gender GetGenderFromSpeciesAndPersonality(Species species, uint personality)
        {
            PokemonData pData = PokemonData.Data[species];

            switch (pData.GenderRatio)
            {
                case Gender.Male:
                case Gender.Female:
                case Gender.Genderless:
                    return pData.GenderRatio;
            }

            if ((byte)pData.GenderRatio > (personality & 0xFF))
                return Gender.Female;
            else
                return Gender.Male;
        }
        public Gender GetGender() => GetGenderFromSpeciesAndPersonality(Species, Personality);

        public override string ToString() => $"{Species} Lv.{Level} {GetGender()}";
    }
}
