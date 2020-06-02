using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEPokemonData
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
            PBESpecies[] preEvolutions,
            PBESpecies[] evolutions,
            PBEAbility[] abilities,
            (PBEMove Move, byte Level, PBEMoveObtainMethod ObtainMethod)[] levelUpMoves,
            (PBEMove Move, PBEMoveObtainMethod ObtainMethod)[] otherMoves)
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
        public bool ReceivesSTAB(PBEType type)
        {
            // type ArgumentOutOfRangeException will happen in HasType()
            return type != PBEType.None && HasType(type);
        }

        #region Database Querying

        private class SearchResult
        {
            public ushort Species { get; set; }
            public byte Form { get; set; }
            public byte HP { get; set; }
            public byte Attack { get; set; }
            public byte Defense { get; set; }
            public byte SpAttack { get; set; }
            public byte SpDefense { get; set; }
            public byte Speed { get; set; }
            public byte Type1 { get; set; }
            public byte Type2 { get; set; }
            public byte GenderRatio { get; set; }
            public double Weight { get; set; }
            public string PreEvolutions { get; set; }
            public string Evolutions { get; set; }
            public string Abilities { get; set; }
            public string LevelUpMoves { get; set; }
            public string OtherMoves { get; set; }
        }
        private static readonly char[] _split1Chars = new char[1] { '|' };

        public static PBEPokemonData GetData(PBESpecies species, PBEForm form)
        {
            PBEPokemonShell.ValidateSpecies(species, form, false);
            List<SearchResult> results = PBEUtils.QueryDatabase<SearchResult>($"SELECT * FROM PokemonData WHERE Species={(ushort)species}");
            SearchResult result = results[0];
            foreach (SearchResult r in results)
            {
                if (r.Form == (byte)form)
                {
                    result = r;
                    break;
                }
            }

            byte[] baseStats = new byte[6] { result.HP, result.Attack, result.Defense, result.SpAttack, result.SpDefense, result.Speed };
            var type1 = (PBEType)result.Type1;
            var type2 = (PBEType)result.Type2;
            var genderRatio = (PBEGenderRatio)result.GenderRatio;
            double weight = result.Weight;

            string[] split1 = result.PreEvolutions.Split(_split1Chars, StringSplitOptions.RemoveEmptyEntries);
            var preEvolutions = new PBESpecies[split1.Length];
            for (int i = 0; i < preEvolutions.Length; i++)
            {
                preEvolutions[i] = (PBESpecies)ushort.Parse(split1[i]);
            }

            split1 = result.Evolutions.Split(_split1Chars, StringSplitOptions.RemoveEmptyEntries);
            var evolutions = new PBESpecies[split1.Length];
            for (int i = 0; i < evolutions.Length; i++)
            {
                evolutions[i] = (PBESpecies)ushort.Parse(split1[i]);
            }

            split1 = result.Abilities.Split(_split1Chars, StringSplitOptions.RemoveEmptyEntries);
            var abilities = new PBEAbility[split1.Length];
            for (int i = 0; i < abilities.Length; i++)
            {
                abilities[i] = (PBEAbility)byte.Parse(split1[i]);
            }

            split1 = result.LevelUpMoves.Split(_split1Chars, StringSplitOptions.RemoveEmptyEntries);
            var levelUpMoves = new (PBEMove, byte, PBEMoveObtainMethod)[split1.Length];
            for (int i = 0; i < levelUpMoves.Length; i++)
            {
                string[] split2 = split1[i].Split(',');
                levelUpMoves[i] = ((PBEMove)ushort.Parse(split2[0]), byte.Parse(split2[1]), (PBEMoveObtainMethod)ulong.Parse(split2[2]));
            }

            split1 = result.OtherMoves.Split(_split1Chars, StringSplitOptions.RemoveEmptyEntries);
            var otherMoves = new (PBEMove, PBEMoveObtainMethod)[split1.Length];
            for (int i = 0; i < otherMoves.Length; i++)
            {
                string[] split2 = split1[i].Split(',');
                otherMoves[i] = ((PBEMove)ushort.Parse(split2[0]), (PBEMoveObtainMethod)ulong.Parse(split2[1]));
            }
            return new PBEPokemonData(baseStats, type1, type2, genderRatio, weight, preEvolutions, evolutions, abilities, levelUpMoves, otherMoves); // TODO: Cache
        }

        #endregion
    }
}
