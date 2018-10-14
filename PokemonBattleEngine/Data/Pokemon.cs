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
        public sbyte[] PP = new sbyte[Constants.NumMoves];

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
                for (int i = 0; i < Constants.NumMoves; i++)
                    Moves[i] = moves[i];
        }

        void SetRandomPersonality() => Personality = Utils.GetRandomUint();

        void SetDefaultMovesForCurrentLevel()
        {
            PokemonData pData = PokemonData.Data[Species];

            Move[] learnedMoves = pData.LevelUpMoves.Where(t => t.Item1 <= Level).Select(t => t.Item2).ToArray();
            int min = Math.Min(Constants.NumMoves, learnedMoves.Length);
            // Copy last learned moves
            for (int i = min - 1; i >= 0; i--)
                Moves[i] = learnedMoves[i];
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
