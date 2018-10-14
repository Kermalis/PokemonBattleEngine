using PokemonBattleEngine.Util;
using System;
using System.Linq;

namespace PokemonBattleEngine.Data
{
    class Pokemon
    {
        public Status Status;
        public Ability Ability;
        public Item Item;
        public int HP;
        public int MaxHP { get; private set; }
        public int Level { get; private set; }

        public Move[] Moves = new Move[Constants.NumMoves];
        public int[] PP = new int[Constants.NumMoves];

        public int Attack, Defense, SpAttack, SpDefense, Speed;

        public readonly Species Species;
        public uint Personality { get; private set; }
        
        public Pokemon(Species species, int level = 1, Item item = Item.None, Move[] moves = null, uint personality = 0)
        {
            // Set species
            Species = species;

            // Set personality
            Personality = personality == 0 ? (uint)Utils.RNG.Next() : personality;

            // Set level
            Level = level;

            // Set item
            Item = item;

            // Set moves
            if (moves == null)
                SetDefaultMovesForCurrentLevel();
            else
                SetMovesAndPP(moves);

            // Ability
            Ability = GetAbilityFromSpeciesAndPersonality(Species, Personality);

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

        static Ability GetAbilityFromSpeciesAndPersonality(Species species, uint personality)
        {
            PokemonData pData = PokemonData.Data[species];

            // TODO: Hidden ability
            if ((personality & 0xFFFF) % 2 == 0 || pData.Ability2 == Ability.None)
                return pData.Ability1;
            else
                return pData.Ability2;
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

        public override string ToString() => $"{Species} Lv.{Level} {GetGender()} {Ability} {HP}/{MaxHP} HP";
    }
}
