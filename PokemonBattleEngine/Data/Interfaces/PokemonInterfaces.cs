using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Data
{
    // Not separating this into IPBEWildPokemon for these reasons:
    // 1: A lot of work to do that
    // 2: If someone wants to do pal park or catch released Pokémon etc, they'd need all these things
    // 3: If they want just some things (like effort values pre-seeded) they'd also need this
    public interface IPBEPokemon
    {
        PBESpecies Species { get; }
        PBEForm Form { get; }
        PBEGender Gender { get; }
        string Nickname { get; }
        bool Shiny { get; }
        byte Level { get; }
        //uint EXP { get; }
        PBEItem Item { get; }
        byte Friendship { get; }
        PBEAbility Ability { get; }
        PBENature Nature { get; }
        PBEItem CaughtBall { get; }
        IPBEStatCollection EffortValues { get; }
        IPBEReadOnlyStatCollection IndividualValues { get; }
        IPBEMoveset Moveset { get; }
    }
    public interface IPBEPartyPokemon : IPBEPokemon
    {
        ushort HP { get; }
        PBEStatus1 Status1 { get; }
        byte SleepTurns { get; }
        new IPBEPartyMoveset Moveset { get; }
    }
    public interface IPBEPokemonCollection<T> : IReadOnlyList<T> where T : IPBEPokemon
    {
    }
    public interface IPBEPokemonCollection : IReadOnlyList<IPBEPokemon>
    {
    }
    public interface IPBEPartyPokemonCollection<T> : IReadOnlyList<T> where T : IPBEPartyPokemon
    {
    }
    public interface IPBEPartyPokemonCollection : IReadOnlyList<IPBEPartyPokemon>
    {
    }

    public interface IPBEPokemonTypes
    {
        /// <summary>The Pokémon's first type.</summary>
        PBEType Type1 { get; }
        /// <summary>The Pokémon's second type.</summary>
        PBEType Type2 { get; }
    }
    public interface IPBEPokemonKnownTypes
    {
        /// <summary>The first type everyone believes the Pokémon has.</summary>
        PBEType KnownType1 { get; }
        /// <summary>The second type everyone believes the Pokémon has.</summary>
        PBEType KnownType2 { get; }
    }

    public static class PBEPokemonInterfaceExtensions
    {
        public static bool HasType(this IPBEPokemonTypes pkmn, PBEType type)
        {
            if (pkmn == null)
            {
                throw new ArgumentException(nameof(pkmn));
            }
            if (type >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }
            return pkmn.Type1 == type || pkmn.Type2 == type;
        }
        public static bool HasType_Known(this IPBEPokemonKnownTypes pkmn, PBEType type)
        {
            if (pkmn == null)
            {
                throw new ArgumentException(nameof(pkmn));
            }
            if (type >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }
            return pkmn.KnownType1 == type || pkmn.KnownType2 == type;
        }
        public static bool HasType<T>(this T pkmn, PBEType type, bool useKnownInfo) where T : IPBEPokemonKnownTypes, IPBEPokemonTypes
        {
            return useKnownInfo ? HasType_Known(pkmn, type) : HasType(pkmn, type);
        }
        public static bool ReceivesSTAB(this IPBEPokemonTypes pkmn, PBEType type)
        {
            return type != PBEType.None && HasType(pkmn, type);
        }
        public static bool ReceivesSTAB_Known(this IPBEPokemonKnownTypes pkmn, PBEType type)
        {
            return type != PBEType.None && HasType_Known(pkmn, type);
        }
        public static bool ReceivesSTAB<T>(this T pkmn, PBEType type, bool useKnownInfo) where T : IPBEPokemonKnownTypes, IPBEPokemonTypes
        {
            return useKnownInfo ? ReceivesSTAB_Known(pkmn, type) : ReceivesSTAB(pkmn, type);
        }

        internal static void ToBytes(this IPBEPokemon pkmn, EndianBinaryWriter w)
        {
            w.Write(pkmn.Species);
            w.Write(pkmn.Form);
            w.Write(pkmn.Nickname, true);
            w.Write(pkmn.Level);
            w.Write(pkmn.Friendship);
            w.Write(pkmn.Shiny);
            w.Write(pkmn.Ability);
            w.Write(pkmn.Nature);
            w.Write(pkmn.CaughtBall);
            w.Write(pkmn.Gender);
            w.Write(pkmn.Item);
            pkmn.EffortValues.ToBytes(w);
            pkmn.IndividualValues.ToBytes(w);
            pkmn.Moveset.ToBytes(w);
        }
        internal static void ToJson(this IPBEPokemon pkmn, JsonTextWriter w)
        {
            w.WriteStartObject();
            w.WritePropertyName(nameof(IPBEPokemon.Species));
            PBESpecies species = pkmn.Species;
            w.WriteValue(species.ToString());
            if (PBEDataUtils.HasForms(species, true))
            {
                w.WritePropertyName(nameof(IPBEPokemon.Form));
                w.WriteValue(PBEDataUtils.GetNameOfForm(species, pkmn.Form));
            }
            w.WritePropertyName(nameof(IPBEPokemon.Nickname));
            w.WriteValue(pkmn.Nickname);
            w.WritePropertyName(nameof(IPBEPokemon.Level));
            w.WriteValue(pkmn.Level);
            w.WritePropertyName(nameof(IPBEPokemon.Friendship));
            w.WriteValue(pkmn.Friendship);
            w.WritePropertyName(nameof(IPBEPokemon.Shiny));
            w.WriteValue(pkmn.Shiny);
            w.WritePropertyName(nameof(IPBEPokemon.Ability));
            w.WriteValue(pkmn.Ability.ToString());
            w.WritePropertyName(nameof(IPBEPokemon.Nature));
            w.WriteValue(pkmn.Nature.ToString());
            w.WritePropertyName(nameof(IPBEPokemon.CaughtBall));
            w.WriteValue(pkmn.CaughtBall.ToString());
            w.WritePropertyName(nameof(IPBEPokemon.Gender));
            w.WriteValue(pkmn.Gender.ToString());
            w.WritePropertyName(nameof(IPBEPokemon.Item));
            w.WriteValue(pkmn.Item.ToString());
            w.WritePropertyName(nameof(IPBEPokemon.EffortValues));
            pkmn.EffortValues.ToJson(w);
            w.WritePropertyName(nameof(IPBEPokemon.IndividualValues));
            pkmn.IndividualValues.ToJson(w);
            w.WritePropertyName(nameof(IPBEPokemon.Moveset));
            pkmn.Moveset.ToJson(w);
            w.WriteEndObject();
        }

        internal static void ToBytes(this IPBEPokemonCollection party, EndianBinaryWriter w)
        {
            byte count = (byte)party.Count;
            w.Write(count);
            for (int i = 0; i < count; i++)
            {
                party[i].ToBytes(w);
            }
        }
    }
}
