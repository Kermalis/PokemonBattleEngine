using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Kermalis.PokemonBattleEngineTesting
{
    class LocalizationGenerator
    {
        static readonly string nop = "--";
        readonly NARC english, french, german, italian, japanese, korean, spanish;

        public LocalizationGenerator()
        {
            english = new NARC(@"../../../\LocalizedData\English.narc");
            french = new NARC(@"../../../\LocalizedData\French.narc");
            german = new NARC(@"../../../\LocalizedData\German.narc");
            italian = new NARC(@"../../../\LocalizedData\Italian.narc");
            japanese = new NARC(@"../../../\LocalizedData\Japanese.narc");
            korean = new NARC(@"../../../\LocalizedData\Korean.narc");
            spanish = new NARC(@"../../../\LocalizedData\Spanish.narc");
        }

        public void GenerateAbilities()
        {
            IEnumerable<PBEAbility> allAbilities = new[] { PBEAbility.None }.Concat(Enum.GetValues(typeof(PBEAbility)).Cast<PBEAbility>().Except(new[] { PBEAbility.None, PBEAbility.MAX }).OrderBy(e => e.ToString()));
            PBEAbility lastAbility = allAbilities.Last();
            var sb = new StringBuilder();
            string[][] eng, fre, ger, ita, jap, kor, spa;
            void LoadTexts(int fileNum)
            {
                eng = english.ReadTextFile(fileNum);
                fre = french.ReadTextFile(fileNum);
                ger = german.ReadTextFile(fileNum);
                ita = italian.ReadTextFile(fileNum);
                jap = japanese.ReadTextFile(fileNum);
                kor = korean.ReadTextFile(fileNum);
                spa = spanish.ReadTextFile(fileNum);
            }
            void WriteAll()
            {
                sb.AppendLine("        {");
                foreach (PBEAbility ability in allAbilities)
                {
                    byte i = (byte)ability;
                    sb.AppendLine($"            {{ PBEAbility.{ability}, new PBELocalizedString(\"{jap[0][i]}\", \"{kor[0][i]}\", \"{fre[0][i]}\", \"{ger[0][i]}\", \"{spa[0][i]}\", \"{ita[0][i]}\", \"{eng[0][i]}\") }}{(ability == lastAbility ? string.Empty : ",")}");
                }
                sb.AppendLine("        });");
            }

            sb.AppendLine("using Kermalis.PokemonBattleEngine.Data;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Collections.ObjectModel;");
            sb.AppendLine();
            sb.AppendLine("namespace Kermalis.PokemonBattleEngine.Localization");
            sb.AppendLine("{");
            sb.AppendLine("    public static class PBEAbilityLocalization");
            sb.AppendLine("    {");
            sb.AppendLine("        public static ReadOnlyDictionary<PBEAbility, PBELocalizedString> Names { get; } = new ReadOnlyDictionary<PBEAbility, PBELocalizedString>(new Dictionary<PBEAbility, PBELocalizedString>()");
            LoadTexts(374);
            WriteAll();
            sb.AppendLine();
            sb.AppendLine("        public static ReadOnlyDictionary<PBEAbility, PBELocalizedString> Descriptions { get; } = new ReadOnlyDictionary<PBEAbility, PBELocalizedString>(new Dictionary<PBEAbility, PBELocalizedString>()");
            LoadTexts(375);
            WriteAll();
            sb.AppendLine("    }");
            sb.AppendLine("}");
            File.WriteAllText(@"../../../../\PokemonBattleEngine\Localization\AbilityLocalization.cs", sb.ToString());
        }
        public void GenerateItems()
        {
            IEnumerable<PBEItem> allItems = new[] { PBEItem.None }.Concat(Enum.GetValues(typeof(PBEItem)).Cast<PBEItem>().Except(new[] { PBEItem.None }).OrderBy(e => e.ToString()));
            PBEItem lastItem = allItems.Last();
            var sb = new StringBuilder();
            string[][] eng, fre, ger, ita, jap, kor, spa;
            void LoadTexts(int fileNum)
            {
                eng = english.ReadTextFile(fileNum);
                fre = french.ReadTextFile(fileNum);
                ger = german.ReadTextFile(fileNum);
                ita = italian.ReadTextFile(fileNum);
                jap = japanese.ReadTextFile(fileNum);
                kor = korean.ReadTextFile(fileNum);
                spa = spanish.ReadTextFile(fileNum);
            }
            void WriteAll()
            {
                sb.AppendLine("        {");
                foreach (PBEItem item in allItems)
                {
                    ushort i = (ushort)item;
                    sb.AppendLine($"            {{ PBEItem.{item}, new PBELocalizedString(\"{jap[0][i]}\", \"{kor[0][i]}\", \"{fre[0][i]}\", \"{ger[0][i]}\", \"{spa[0][i]}\", \"{ita[0][i]}\", \"{eng[0][i]}\") }}{(item == lastItem ? string.Empty : ",")}");
                }
                sb.AppendLine("        });");
            }

            sb.AppendLine("using Kermalis.PokemonBattleEngine.Data;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Collections.ObjectModel;");
            sb.AppendLine();
            sb.AppendLine("namespace Kermalis.PokemonBattleEngine.Localization");
            sb.AppendLine("{");
            sb.AppendLine("    public static class PBEItemLocalization");
            sb.AppendLine("    {");
            sb.AppendLine("        public static ReadOnlyDictionary<PBEItem, PBELocalizedString> Names { get; } = new ReadOnlyDictionary<PBEItem, PBELocalizedString>(new Dictionary<PBEItem, PBELocalizedString>()");
            LoadTexts(64);
            WriteAll();
            sb.AppendLine();
            sb.AppendLine("        public static ReadOnlyDictionary<PBEItem, PBELocalizedString> Descriptions { get; } = new ReadOnlyDictionary<PBEItem, PBELocalizedString>(new Dictionary<PBEItem, PBELocalizedString>()");
            LoadTexts(63);
            WriteAll();
            sb.AppendLine("    }");
            sb.AppendLine("}");
            File.WriteAllText(@"../../../../\PokemonBattleEngine\Localization\ItemLocalization.cs", sb.ToString());
        }
        public void GenerateMoves()
        {
            ushort lastMove = (ushort)(PBEMove.MAX - 1);
            var sb = new StringBuilder();
            string[][] eng, fre, ger, ita, jap, kor, spa;
            void LoadTexts(int fileNum)
            {
                eng = english.ReadTextFile(fileNum);
                fre = french.ReadTextFile(fileNum);
                ger = german.ReadTextFile(fileNum);
                ita = italian.ReadTextFile(fileNum);
                jap = japanese.ReadTextFile(fileNum);
                kor = korean.ReadTextFile(fileNum);
                spa = spanish.ReadTextFile(fileNum);
            }
            void WriteAll()
            {
                sb.AppendLine("        {");
                for (ushort i = 0; i <= lastMove; i++)
                {
                    sb.AppendLine($"            {(Enum.IsDefined(typeof(PBEMove), i) ? string.Empty : "// ")}{{ PBEMove.{(PBEMove)i}, new PBELocalizedString(\"{jap[0][i]}\", \"{kor[0][i]}\", \"{fre[0][i]}\", \"{ger[0][i]}\", \"{spa[0][i]}\", \"{ita[0][i]}\", \"{eng[0][i]}\") }}{(i == lastMove ? string.Empty : ",")}");
                }
                sb.AppendLine("        });");
            }

            sb.AppendLine("using Kermalis.PokemonBattleEngine.Data;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Collections.ObjectModel;");
            sb.AppendLine();
            sb.AppendLine("namespace Kermalis.PokemonBattleEngine.Localization");
            sb.AppendLine("{");
            sb.AppendLine("    public static class PBEMoveLocalization");
            sb.AppendLine("    {");
            sb.AppendLine("        public static ReadOnlyDictionary<PBEMove, PBELocalizedString> Names { get; } = new ReadOnlyDictionary<PBEMove, PBELocalizedString>(new Dictionary<PBEMove, PBELocalizedString>()");
            LoadTexts(403);
            WriteAll();
            sb.AppendLine();
            sb.AppendLine("        public static ReadOnlyDictionary<PBEMove, PBELocalizedString> Descriptions { get; } = new ReadOnlyDictionary<PBEMove, PBELocalizedString>(new Dictionary<PBEMove, PBELocalizedString>()");
            LoadTexts(402);
            WriteAll();
            sb.AppendLine("    }");
            sb.AppendLine("}");
            File.WriteAllText(@"../../../../\PokemonBattleEngine\Localization\MoveLocalization.cs", sb.ToString());
        }
        public static void GeneratePokemon()
        {
            string[][] names = new WebClient().DownloadString("https://raw.githubusercontent.com/veekun/pokedex/master/pokedex/data/csv/pokemon_species_names.csv").Split('\n').Skip(1).Select(s => s.Split(',')).ToArray();
            const uint numSpecies = 649;

            var sb = new StringBuilder();
            sb.AppendLine("using Kermalis.PokemonBattleEngine.Data;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Collections.ObjectModel;");
            sb.AppendLine();
            sb.AppendLine("namespace Kermalis.PokemonBattleEngine.Localization");
            sb.AppendLine("{");
            sb.AppendLine("    public static class PBEPokemonLocalization");
            sb.AppendLine("    {");
            sb.AppendLine("        public static ReadOnlyDictionary<PBESpecies, PBELocalizedString> Names { get; } = new ReadOnlyDictionary<PBESpecies, PBELocalizedString>(new Dictionary<PBESpecies, PBELocalizedString>()");
            sb.AppendLine("        {");
            for (uint i = 1; i <= numSpecies; i++)
            {
                bool speciesExists = Enum.IsDefined(typeof(PBESpecies), i);
                string speciesID = i.ToString();
                string japanese = names.Single(s => s[0] == speciesID && s[1] == "1")[2];
                string korean = names.Single(s => s[0] == speciesID && s[1] == "3")[2];
                string french = names.Single(s => s[0] == speciesID && s[1] == "5")[2];
                string german = names.Single(s => s[0] == speciesID && s[1] == "6")[2];
                string spanish = names.Single(s => s[0] == speciesID && s[1] == "7")[2];
                string italian = names.Single(s => s[0] == speciesID && s[1] == "8")[2];
                string english = names.Single(s => s[0] == speciesID && s[1] == "9")[2];
                uint numForms;
                switch ((PBESpecies)i)
                {
                    case PBESpecies.Arceus: numForms = 17; break;
                    case PBESpecies.Basculin_Blue: numForms = 2; break;
                    case PBESpecies.Darmanitan: numForms = 2; break;
                    case PBESpecies.Deerling_Autumn: numForms = 4; break;
                    case PBESpecies.Genesect: numForms = 5; break;
                    case PBESpecies.Giratina: numForms = 2; break;
                    case PBESpecies.Keldeo: numForms = 2; break;
                    case PBESpecies.Kyurem: numForms = 3; break;
                    case PBESpecies.Landorus: numForms = 2; break;
                    case PBESpecies.Meloetta: numForms = 2; break;
                    case PBESpecies.Rotom: numForms = 6; break;
                    case PBESpecies.Sawsbuck_Autumn: numForms = 4; break;
                    case PBESpecies.Thundurus: numForms = 2; break;
                    case PBESpecies.Tornadus: numForms = 2; break;
                    case PBESpecies.Unown_A: numForms = 28; break;
                    default: numForms = 1; break;
                }
                for (uint j = 0; j < numForms; j++)
                {
                    sb.AppendLine($"            {(speciesExists ? string.Empty : "// ")}{{ PBESpecies.{(PBESpecies)(i | (j << 0x10))}, new PBELocalizedString(\"{japanese}\", \"{korean}\", \"{french}\", \"{german}\", \"{spanish}\", \"{italian}\", \"{english}\") }}{(i == numSpecies && j == numForms - 1 ? string.Empty : ",")}");
                }
            }
            sb.AppendLine("        });");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            File.WriteAllText(@"../../../../\PokemonBattleEngine\Localization\PokemonLocalization.cs", sb.ToString());
        }
    }
}
