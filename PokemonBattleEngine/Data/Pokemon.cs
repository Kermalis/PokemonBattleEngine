using PokemonBattleEngine.Util;

namespace PokemonBattleEngine.Data
{
    class Pokemon
    {
        public Status Status;
        public int HP;

        public readonly Species Species;
        public uint Personality { get; private set; }

        public PokemonData PData => PokemonData.Data[Species];

        public Pokemon(Species species)
        {
            Species = species;
            SetRandomPersonality();
        }
        public Pokemon(Species species, uint personality)
        {
            Species = species;
            Personality = personality;
        }

        void SetRandomPersonality() => Personality = Utils.GetRandomUint();

        public Gender GetGender()
        {
            switch (PData.GenderRatio)
            {
                case Gender.Male:
                case Gender.Female:
                case Gender.Genderless:
                    return PData.GenderRatio;
            }

            if ((byte)PData.GenderRatio > (Personality & 0xFF))
                return Gender.Female;
            else
                return Gender.Male;
        }

        public override string ToString() => $"{Species} {GetGender()}";
    }
}
