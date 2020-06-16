using Kermalis.EndianBinaryIO;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEReadOnlyPokemon : IPBEPokemon
    {
        public PBESpecies Species { get; }
        public PBEForm Form { get; }
        public PBEGender Gender { get; }
        public string Nickname { get; }
        public bool Shiny { get; }
        public byte Level { get; }
        public PBEItem Item { get; }
        public byte Friendship { get; }
        public PBEAbility Ability { get; }
        public PBENature Nature { get; }
        public IPBEStatCollection EffortValues { get; }
        public IPBEReadOnlyStatCollection IndividualValues { get; }
        public IPBEMoveset Moveset { get; }

        internal PBEReadOnlyPokemon(EndianBinaryReader r)
        {
            Species = r.ReadEnum<PBESpecies>();
            Form = r.ReadEnum<PBEForm>();
            Nickname = r.ReadStringNullTerminated();
            Level = r.ReadByte();
            Friendship = r.ReadByte();
            Shiny = r.ReadBoolean();
            Ability = r.ReadEnum<PBEAbility>();
            Nature = r.ReadEnum<PBENature>();
            Gender = r.ReadEnum<PBEGender>();
            Item = r.ReadEnum<PBEItem>();
            EffortValues = new PBEStatCollection(r);
            IndividualValues = new PBEReadOnlyStatCollection(r);
            Moveset = new PBEReadOnlyMoveset(r);
        }
    }
}
