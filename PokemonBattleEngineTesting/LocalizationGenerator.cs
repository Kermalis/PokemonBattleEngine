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
            sb.AppendLine("using Kermalis.PokemonBattleEngine.Data;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Collections.ObjectModel;");
            sb.AppendLine();
            sb.AppendLine("namespace Kermalis.PokemonBattleEngine.Localization");
            sb.AppendLine("{");
            sb.AppendLine("    public static class PBEAbilityLocalization");
            sb.AppendLine("    {");
            // Names
            sb.AppendLine("        public static ReadOnlyDictionary<PBEAbility, PBELocalizedString> Names { get; } = new ReadOnlyDictionary<PBEAbility, PBELocalizedString>(new Dictionary<PBEAbility, PBELocalizedString>()");
            sb.AppendLine("        {");
            string[][] eng = english.ReadTextFile(374);
            string[][] fre = french.ReadTextFile(374);
            string[][] ger = german.ReadTextFile(374);
            string[][] ita = italian.ReadTextFile(374);
            string[][] jap = japanese.ReadTextFile(374);
            string[][] kor = korean.ReadTextFile(374);
            string[][] spa = spanish.ReadTextFile(374);
            foreach (PBEAbility ability in allAbilities)
            {
                byte i = (byte)ability;
                sb.AppendLine($"            {{ PBEAbility.{ability}, new PBELocalizedString(\"{jap[0][i]}\", \"{kor[0][i]}\", \"{fre[0][i]}\", \"{ger[0][i]}\", \"{spa[0][i]}\", \"{ita[0][i]}\", \"{eng[0][i]}\") }}{(ability == lastAbility ? string.Empty : ",")}");
            }
            sb.AppendLine("        });");
            // Descriptions
            sb.AppendLine("        public static ReadOnlyDictionary<PBEAbility, PBELocalizedString> Descriptions { get; } = new ReadOnlyDictionary<PBEAbility, PBELocalizedString>(new Dictionary<PBEAbility, PBELocalizedString>()");
            sb.AppendLine("        {");
            eng = english.ReadTextFile(375);
            fre = french.ReadTextFile(375);
            ger = german.ReadTextFile(375);
            ita = italian.ReadTextFile(375);
            jap = japanese.ReadTextFile(375);
            kor = korean.ReadTextFile(375);
            spa = spanish.ReadTextFile(375);
            foreach (PBEAbility ability in allAbilities)
            {
                byte i = (byte)ability;
                sb.AppendLine($"            {{ PBEAbility.{ability}, new PBELocalizedString(\"{jap[0][i]}\", \"{kor[0][i]}\", \"{fre[0][i]}\", \"{ger[0][i]}\", \"{spa[0][i]}\", \"{ita[0][i]}\", \"{eng[0][i]}\") }}{(ability == lastAbility ? string.Empty : ",")}");
            }
            sb.AppendLine("        });");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            File.WriteAllText(@"../../../../\PokemonBattleEngine\Localization\AbilityLocalization.cs", sb.ToString());
        }

        public static void GenerateItems()
        {
            // Last string[] is 1 length for some reason
            string[][] ids = new WebClient().DownloadString("https://raw.githubusercontent.com/veekun/pokedex/master/pokedex/data/csv/item_game_indices.csv").Split('\n').Skip(1).Select(s => s.Split(',')).Where(s => s.Length != 1).ToArray();
            string[][] names = new WebClient().DownloadString("https://raw.githubusercontent.com/veekun/pokedex/master/pokedex/data/csv/item_names.csv").Split('\n').Skip(1).Select(s => s.Split(',')).ToArray();
            IEnumerable<PBEItem> allItems = Enum.GetValues(typeof(PBEItem)).Cast<PBEItem>().Except(new[] { PBEItem.None }).OrderBy(e => e.ToString());

            var sb = new StringBuilder();
            sb.AppendLine("using Kermalis.PokemonBattleEngine.Data;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Collections.ObjectModel;");
            sb.AppendLine();
            sb.AppendLine("namespace Kermalis.PokemonBattleEngine.Localization");
            sb.AppendLine("{");
            sb.AppendLine("    public static class PBEItemLocalization");
            sb.AppendLine("    {");
            sb.AppendLine("        public static ReadOnlyDictionary<PBEItem, PBELocalizedString> Names { get; } = new ReadOnlyDictionary<PBEItem, PBELocalizedString>(new Dictionary<PBEItem, PBELocalizedString>()");
            sb.AppendLine("        {");
            sb.AppendLine($"            {{ PBEItem.None, new PBELocalizedString(\"{nop}\", \"{nop}\", \"{nop}\", \"{nop}\", \"{nop}\", \"{nop}\", \"{nop}\") }},");
            foreach (PBEItem item in allItems)
            {
                string theirID = ids.Single(s => s[1] == "5" && s[2] == ((ushort)item).ToString())[0];
                string japanese = names.Single(s => s[0] == theirID && s[1] == "1")[2];
                string korean = names.Single(s => s[0] == theirID && s[1] == "3")[2];
                string french = names.Single(s => s[0] == theirID && s[1] == "5")[2];
                string german = names.Single(s => s[0] == theirID && s[1] == "6")[2];
                string spanish = names.Single(s => s[0] == theirID && s[1] == "7")[2];
                string italian = names.Single(s => s[0] == theirID && s[1] == "8")[2];
                string english = names.Single(s => s[0] == theirID && s[1] == "9")[2];
                switch (item)
                {
                    case PBEItem.AmuletCoin: spanish = "Mon. Amuleto"; break;
                    case PBEItem.ArmorFossil: french = "Foss. Armure"; break;
                    case PBEItem.BalmMushroom: english = "BalmMushroom"; break;
                    case PBEItem.BigRoot: french = "Grosseracine"; break;
                    case PBEItem.BindingBand: french = "B. Étreinte"; spanish = "Ban. Atadura"; break;
                    case PBEItem.BlackBelt: french = "Ceint.Noire"; break;
                    case PBEItem.BlackGlasses: french = "Lunet.Noires"; english = "BlackGlasses"; break;
                    case PBEItem.BlkApricorn: spanish = "Bonguri Neg"; italian = "Ghic. Nera"; english = "Blk Apricorn"; break;
                    case PBEItem.BluApricorn: spanish = "Bonguri Azu"; italian = "Ghic. Blu"; english = "Blu Apricorn"; break;
                    case PBEItem.BrightPowder: french = "Poudreclaire"; english = "BrightPowder"; break;
                    case PBEItem.BugGem: french = "Joyau Insect"; spanish = "G. Bicho"; italian = "Bijoucoleot."; break;
                    case PBEItem.ChoiceBand: french = "Band. Choix"; spanish = "Cin. Elegida"; break;
                    case PBEItem.ChoiceScarf: french = "Mouch. Choix"; spanish = "Pañ. Elegido"; break;
                    case PBEItem.ChoiceSpecs: french = "Lunet. Choix"; break;
                    case PBEItem.CleanseTag: french = "Rune Purif."; break;
                    case PBEItem.ClawFossil: french = "Foss. Griffe"; break;
                    case PBEItem.CleverWing: spanish = "P. Mente"; break;
                    case PBEItem.CometShard: french = "Morc. Comète"; spanish = "Frag. Cometa"; break;
                    case PBEItem.CoverFossil: french = "Foss. Plaque"; break;
                    case PBEItem.DarkGem: french = "Joyau Ténèbr"; spanish = "G. Siniestra"; break;
                    case PBEItem.DeepSeaScale: french = "Écailleocéan"; english = "DeepSeaScale"; break;
                    case PBEItem.DeepSeaTooth: italian = "Denteabissi"; english = "DeepSeaTooth"; break;
                    case PBEItem.DireHit: japanese = "クリティカッター"; break;
                    case PBEItem.DomeFossil: french = "Fossile Dome"; break;
                    case PBEItem.DragonFang: spanish = "Colmillodrag"; break;
                    case PBEItem.DragonGem: spanish = "G. Dragón"; break;
                    case PBEItem.DragonScale: french = "Ecailledraco"; break;
                    case PBEItem.ElectricGem: spanish = "G. Eléctrica"; break;
                    case PBEItem.EnergyPowder: french = "Poudrénergie"; spanish = "Polvoenergía"; english = "EnergyPowder"; break;
                    case PBEItem.EnergyRoot: french = "Racinénergie"; break;
                    case PBEItem.Everstone: spanish = "Piedraeterna"; break;
                    case PBEItem.ExpShare: italian = "Condiv. Esp."; break;
                    case PBEItem.FightingGem: spanish = "G. Lucha"; break;
                    case PBEItem.FireGem: spanish = "G. Fuego"; break;
                    case PBEItem.FlyingGem: spanish = "G. Voladora"; break;
                    case PBEItem.FocusSash: french = "Ceint. Force"; break;
                    case PBEItem.FullHeal: spanish = "Restau. Todo"; break;
                    case PBEItem.FullIncense: spanish = "Incie. Lento"; break;
                    case PBEItem.GeniusWing: spanish = "P. Intelecto"; break;
                    case PBEItem.GhostGem: french = "Joyau Spectr"; german = "Geistjuwel"; spanish = "G. Fantasma"; break;
                    case PBEItem.GrassGem: german = "Pflanzjuwel"; spanish = "G. Planta"; break;
                    case PBEItem.GrnApricorn: spanish = "Bonguri Ver"; italian = "Ghic. Verde"; english = "Grn Apricorn"; break;
                    case PBEItem.GroundGem: spanish = "G. Tierra"; break;
                    case PBEItem.HealthWing: spanish = "P. Vigor"; break;
                    case PBEItem.HeartScale: french = "Écaillecœur"; spanish = "Esc. Corazón"; break;
                    case PBEItem.IceGem: spanish = "G. Hielo"; italian = "Bijoughiac."; break;
                    case PBEItem.LaxIncense: spanish = "Incie. Suave"; break;
                    case PBEItem.LeafStone: french = "Pierreplante"; break;
                    case PBEItem.LuckIncense: spanish = "Incie. Duplo"; break;
                    case PBEItem.MachoBrace: french = "Brac. Macho"; spanish = "Vestidura"; break;
                    case PBEItem.MaxRepel: spanish = "Máx. Repel"; italian = "Repell. Max"; break;
                    case PBEItem.MaxRevive: spanish = "Máx. Revivir"; italian = "Revital. Max"; break;
                    case PBEItem.MentalHerb: spanish = "Hier. Mental"; break;
                    case PBEItem.MetalCoat: spanish = "Rev. Metálico"; break;
                    case PBEItem.MiracleSeed: french = "Grain Miracl"; break;
                    case PBEItem.MuscleBand: french = "Band. Muscle"; break;
                    case PBEItem.MuscleWing: spanish = "P. Músculo"; break;
                    case PBEItem.MysticWater: italian = "Acquamagica"; break;
                    case PBEItem.NeverMeltIce: french = "Glacéternel"; english = "NeverMeltIce"; break;
                    case PBEItem.NormalGem: spanish = "G. Normal"; break;
                    case PBEItem.OddIncense: french = "Bizar. Encens"; spanish = "Incie. Raro"; break;
                    case PBEItem.OddKeystone: spanish = "P. Espíritu"; break;
                    case PBEItem.ParalyzeHeal: spanish = "Antiparaliz"; break;
                    case PBEItem.PlumeFossil: french = "Foss. Plume"; break;
                    case PBEItem.PnkApricorn: spanish = "Bonguri Ros"; italian = "Ghic. Rosa"; english = "Pnk Apricorn"; break;
                    case PBEItem.PoisonGem: spanish = "G. Veneno"; break;
                    case PBEItem.PowerAnklet: french = "Chaîne. Pouv."; break;
                    case PBEItem.PowerBand: french = "Band. Pouv."; break;
                    case PBEItem.PowerBelt: french = "Ceint. Pouv."; break;
                    case PBEItem.PowerBracer: french = "Poign. Pouv."; break;
                    case PBEItem.PowerHerb: french = "Herbe Pouv."; break;
                    case PBEItem.PowerLens: french = "Lent. Pouv."; break;
                    case PBEItem.PowerWeight: french = "Poids Pouv."; break;
                    case PBEItem.PPMax: spanish = "Máx. PP"; break;
                    case PBEItem.PrettyWing: spanish = "P. Bella"; break;
                    case PBEItem.PsychicGem: spanish = "G. Psíquica"; break;
                    case PBEItem.PureIncense: spanish = "Incie. Puro"; break;
                    case PBEItem.RazorClaw: spanish = "Garrafilada"; break;
                    case PBEItem.RazorFang: spanish = "Colmillagudo"; break;
                    case PBEItem.ReaperCloth: spanish = "Telaterrible"; break;
                    case PBEItem.RedApricorn: spanish = "Bonguri Roj"; italian = "Ghic. Rossa"; break;
                    case PBEItem.RelicCrown: spanish = "Cor. Antigua"; break;
                    case PBEItem.RelicStatue: spanish = "Efi. Antigua"; break;
                    case PBEItem.ResistWing: spanish = "P. Aguante"; break;
                    case PBEItem.Revive: italian = "Revitaliz."; break;
                    case PBEItem.RingTarget: french = "Pt de Mire"; break;
                    case PBEItem.RockGem: german = "Gesteinjuwel"; spanish = "G. Roca"; break;
                    case PBEItem.RockyHelmet: spanish = "Cas. Dentado"; break;
                    case PBEItem.RootFossil: french = "Foss. Racine"; break;
                    case PBEItem.RoseIncense: spanish = "Inc. Floral"; break;
                    case PBEItem.SacredAsh: french = "Cendresacrée"; break;
                    case PBEItem.SeaIncense: spanish = "Incie. Mar."; break;
                    case PBEItem.ShoalShell: french = "Co. Trefonds"; spanish = "C. Cardumen"; break;
                    case PBEItem.SilkScarf: french = "Mouch. Soie"; italian = "Sciarpaseta"; break;
                    case PBEItem.SilverPowder: french = "Poudre Arg."; english = "SilverPowder"; break;
                    case PBEItem.SkullFossil: french = "Foss. Crâne"; break;
                    case PBEItem.SoftSand: italian = "Sabbiasoffice"; break;
                    case PBEItem.StableMulch: spanish = "Ab. Fijador"; break;
                    case PBEItem.Stardust: french = "Pouss.Étoile"; break;
                    case PBEItem.StarPiece: french = "Morc. Étoile"; spanish = "Tr. Estrella"; break;
                    case PBEItem.SteelGem: spanish = "G. Acero"; break;
                    case PBEItem.SunStone: french = "Pierresoleil"; break;
                    case PBEItem.SuperRepel: spanish = "Superrepel"; italian = "Superrepell."; break;
                    case PBEItem.SweetHeart: spanish = "Cor. Dulce"; break;
                    case PBEItem.SwiftWing: spanish = "P. Ímpetu"; break;
                    case PBEItem.Thunderstone: french = "Pierrefoundr"; spanish = "Piedratrueno"; english = "Thunderstone"; break;
                    case PBEItem.TinyMushroom: english = "TinyMushroom"; break;
                    case PBEItem.TwistedSpoon: french = "Cuillertordu"; italian = "Cucchiaiotorto"; english = "TwistedSpoon"; break;
                    case PBEItem.WaterGem: spanish = "G. Agua"; break;
                    case PBEItem.WhiteFlute: french = "Flûteblanche"; break;
                    case PBEItem.WhiteHerb: french = "Herbeblanche"; spanish = "Hier. Blanca"; break;
                    case PBEItem.WiseGlasses: french = "Lunet. Sages"; spanish = "Gafas Esp."; break;
                    case PBEItem.WhtApricorn: spanish = "Bongui Bla"; italian = "Ghic. Biana"; english = "Wht Apricorn"; break;
                    case PBEItem.XDefend: english = "X Defend"; break;
                    case PBEItem.XSpecial: italian = "Special X"; english = "X Special"; break;
                    case PBEItem.YellowFlute: spanish = "Fl. Amarilla"; italian = "Flauto Gial."; break;
                    case PBEItem.YellowShard: spanish = "P. Amarilla"; italian = "Coccio Gial."; break;
                    case PBEItem.YlwApricorn: spanish = "Bonguri Ama"; italian = "Ghic. Gialla"; english = "Ylw Apricorn"; break;
                    case PBEItem.ZoomLens: french = "Lentil. Zoom"; break;
                }
                sb.AppendLine($"            {{ PBEItem.{item}, new PBELocalizedString(\"{japanese}\", \"{korean}\", \"{french}\", \"{german}\", \"{spanish}\", \"{italian}\", \"{english}\") }}{(item == allItems.Last() ? string.Empty : ",")}");
            }
            sb.AppendLine("        });");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            File.WriteAllText(@"../../../../\PokemonBattleEngine\Localization\ItemLocalization.cs", sb.ToString());
        }

        public void GenerateMoves()
        {
            ushort lastMove = (ushort)(PBEMove.MAX - 1);
            var sb = new StringBuilder();
            sb.AppendLine("using Kermalis.PokemonBattleEngine.Data;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Collections.ObjectModel;");
            sb.AppendLine();
            sb.AppendLine("namespace Kermalis.PokemonBattleEngine.Localization");
            sb.AppendLine("{");
            sb.AppendLine("    public static class PBEMoveLocalization");
            sb.AppendLine("    {");
            // Names
            string[][] eng = english.ReadTextFile(403);
            string[][] fre = french.ReadTextFile(403);
            string[][] ger = german.ReadTextFile(403);
            string[][] ita = italian.ReadTextFile(403);
            string[][] jap = japanese.ReadTextFile(403);
            string[][] kor = korean.ReadTextFile(403);
            string[][] spa = spanish.ReadTextFile(403);
            sb.AppendLine("        public static ReadOnlyDictionary<PBEMove, PBELocalizedString> Names { get; } = new ReadOnlyDictionary<PBEMove, PBELocalizedString>(new Dictionary<PBEMove, PBELocalizedString>()");
            sb.AppendLine("        {");
            for (ushort i = 0; i < (ushort)PBEMove.MAX; i++)
            {
                sb.AppendLine($"            {(Enum.IsDefined(typeof(PBEMove), i) ? string.Empty : "// ")}{{ PBEMove.{(PBEMove)i}, new PBELocalizedString(\"{jap[0][i]}\", \"{kor[0][i]}\", \"{fre[0][i]}\", \"{ger[0][i]}\", \"{spa[0][i]}\", \"{ita[0][i]}\", \"{eng[0][i]}\") }}{(i == lastMove ? string.Empty : ",")}");
            }
            sb.AppendLine("        });");
            // Descriptions
            eng = english.ReadTextFile(402);
            fre = french.ReadTextFile(402);
            ger = german.ReadTextFile(402);
            ita = italian.ReadTextFile(402);
            jap = japanese.ReadTextFile(402);
            kor = korean.ReadTextFile(402);
            spa = spanish.ReadTextFile(402);
            sb.AppendLine();
            sb.AppendLine("        public static ReadOnlyDictionary<PBEMove, PBELocalizedString> Descriptions { get; } = new ReadOnlyDictionary<PBEMove, PBELocalizedString>(new Dictionary<PBEMove, PBELocalizedString>()");
            sb.AppendLine("        {");
            for (ushort i = 0; i < (ushort)PBEMove.MAX; i++)
            {
                sb.AppendLine($"            {(Enum.IsDefined(typeof(PBEMove), i) ? string.Empty : "// ")}{{ PBEMove.{(PBEMove)i}, new PBELocalizedString(\"{jap[0][i]}\", \"{kor[0][i]}\", \"{fre[0][i]}\", \"{ger[0][i]}\", \"{spa[0][i]}\", \"{ita[0][i]}\", \"{eng[0][i]}\") }}{(i == lastMove ? string.Empty : ",")}");
            }
            sb.AppendLine("        });");
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
