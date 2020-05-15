using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed partial class PBEPokemonData
    {
        public ReadOnlyCollection<byte> BaseStats { get; }
        public PBEType Type1 { get; }
        public PBEType Type2 { get; }
        public PBEGenderRatio GenderRatio { get; }
        /// <summary>Weight in Kilograms</summary>
        public double Weight { get; }
        public ReadOnlyCollection<PBESpecies> PreEvolutions { get; }
        public ReadOnlyCollection<PBESpecies> Evolutions { get; }
        public ReadOnlyCollection<PBEAbility> Abilities { get; }
        public ReadOnlyCollection<(PBEMove Move, byte Level, PBEMoveObtainMethod ObtainMethod)> LevelUpMoves { get; }
        public ReadOnlyCollection<(PBEMove Move, PBEMoveObtainMethod ObtainMethod)> OtherMoves { get; }

        private PBEPokemonData(byte[] baseStats,
            PBEType type1, PBEType type2, PBEGenderRatio genderRatio, double weight,
            List<PBESpecies> preEvolutions,
            List<PBESpecies> evolutions,
            List<PBEAbility> abilities,
            List<(PBEMove Move, byte Level, PBEMoveObtainMethod ObtainMethod)> levelUpMoves,
            List<(PBEMove Move, PBEMoveObtainMethod ObtainMethod)> otherMoves)
        {
            BaseStats = new ReadOnlyCollection<byte>(baseStats);
            Type1 = type1; Type2 = type2; GenderRatio = genderRatio; Weight = weight;
            PreEvolutions = new ReadOnlyCollection<PBESpecies>(preEvolutions);
            Evolutions = new ReadOnlyCollection<PBESpecies>(evolutions);
            Abilities = new ReadOnlyCollection<PBEAbility>(abilities);
            LevelUpMoves = new ReadOnlyCollection<(PBEMove Move, byte Level, PBEMoveObtainMethod ObtainMethod)>(levelUpMoves);
            OtherMoves = new ReadOnlyCollection<(PBEMove Move, PBEMoveObtainMethod ObtainMethod)>(otherMoves);
        }

        public bool HasAbility(PBEAbility ability)
        {
            if (ability >= PBEAbility.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(ability));
            }
            return Abilities.Contains(ability);
        }
        public bool HasType(PBEType type)
        {
            if (type >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }
            return Type1 == type || Type2 == type;
        }

        #region Database Querying

        private class SearchResult
        {
            public uint Id { get; set; }
            public string Json { get; set; }
        }
        public static PBEPokemonData GetData(PBESpecies species)
        {
            if (!Enum.IsDefined(typeof(PBESpecies), species))
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            string json = PBEUtils.QueryDatabase<SearchResult>($"SELECT * FROM PokemonData WHERE Id={(uint)species}")[0].Json;
            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                reader.Read(); // {
                reader.Read(); // "BaseStats":
                reader.Read(); // [
                byte[] baseStats = new byte[6];
                for (int i = 0; i < 6; i++)
                {
                    reader.Read();
                    baseStats[i] = Convert.ToByte(reader.Value);
                }
                reader.Read(); // ]
                reader.Read(); // "Type1":
                reader.Read();
                var type1 = (PBEType)Convert.ToByte(reader.Value);
                reader.Read(); // "Type2":
                reader.Read();
                var type2 = (PBEType)Convert.ToByte(reader.Value);
                reader.Read(); // "GenderRatio":
                reader.Read();
                var genderRatio = (PBEGenderRatio)Convert.ToByte(reader.Value);
                reader.Read(); // "Weight":
                reader.Read();
                double weight = Convert.ToDouble(reader.Value);
                reader.Read(); // "PreEvolutions":
                reader.Read(); // [
                var preEvolutions = new List<PBESpecies>();
                while (true)
                {
                    reader.Read();
                    if (reader.TokenType == JsonToken.Integer)
                    {
                        preEvolutions.Add((PBESpecies)Convert.ToUInt32(reader.Value));
                    }
                    else if (reader.TokenType == JsonToken.EndArray) // ]
                    {
                        break;
                    }
                }
                reader.Read(); // "Evolutions":
                reader.Read(); // [
                var evolutions = new List<PBESpecies>();
                while (true)
                {
                    reader.Read();
                    if (reader.TokenType == JsonToken.Integer)
                    {
                        evolutions.Add((PBESpecies)Convert.ToUInt32(reader.Value));
                    }
                    else if (reader.TokenType == JsonToken.EndArray) // ]
                    {
                        break;
                    }
                }
                reader.Read(); // "Abilities":
                reader.Read(); // [
                var abilities = new List<PBEAbility>();
                while (true)
                {
                    reader.Read();
                    if (reader.TokenType == JsonToken.Integer)
                    {
                        abilities.Add((PBEAbility)Convert.ToByte(reader.Value));
                    }
                    else if (reader.TokenType == JsonToken.EndArray) // ]
                    {
                        break;
                    }
                }
                reader.Read(); // "LevelUpMoves":
                reader.Read(); // [
                var levelUpMoves = new List<(PBEMove Move, byte Level, PBEMoveObtainMethod ObtainMethod)>();
                while (true)
                {
                    reader.Read();
                    if (reader.TokenType == JsonToken.StartArray) // [
                    {
                        reader.Read();
                        var move = (PBEMove)Convert.ToUInt16(reader.Value);
                        reader.Read();
                        byte level = Convert.ToByte(reader.Value);
                        reader.Read();
                        var method = (PBEMoveObtainMethod)Convert.ToUInt64(reader.Value);
                        levelUpMoves.Add((move, level, method));
                        reader.Read(); // ]
                    }
                    else if (reader.TokenType == JsonToken.EndArray) // ]
                    {
                        break;
                    }
                }
                reader.Read(); // "OtherMoves":
                reader.Read(); // [
                var otherMoves = new List<(PBEMove Move, PBEMoveObtainMethod ObtainMethod)>();
                while (true)
                {
                    reader.Read();
                    if (reader.TokenType == JsonToken.StartArray) // [
                    {
                        reader.Read();
                        var move = (PBEMove)Convert.ToUInt16(reader.Value);
                        reader.Read();
                        var method = (PBEMoveObtainMethod)Convert.ToUInt64(reader.Value);
                        otherMoves.Add((move, method));
                        reader.Read(); // ]
                    }
                    else if (reader.TokenType == JsonToken.EndArray) // ]
                    {
                        break;
                    }
                }
                return new PBEPokemonData(baseStats, type1, type2, genderRatio, weight, preEvolutions, evolutions, abilities, levelUpMoves, otherMoves); // TODO: Cache?
            }
        }

        #endregion
    }
}
