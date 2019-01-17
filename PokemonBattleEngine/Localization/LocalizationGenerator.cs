using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Localization
{
    static class PBELocalizationGenerator
    {
        public static void GenerateAbilities()
        {
            string[][] names = new WebClient().DownloadString("https://raw.githubusercontent.com/veekun/pokedex/master/pokedex/data/csv/ability_names.csv").Split('\n').Skip(1).Select(s => s.Split(',')).ToArray();
            IEnumerable<PBEAbility> allAbilities = Enum.GetValues(typeof(PBEAbility)).Cast<PBEAbility>().Except(new[] { PBEAbility.None, PBEAbility.MAX }).OrderBy(e => e.ToString());

            var sb = new StringBuilder();
            sb.AppendLine("using Kermalis.PokemonBattleEngine.Data;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Collections.ObjectModel;");
            sb.AppendLine();
            sb.AppendLine("namespace Kermalis.PokemonBattleEngine.Localization");
            sb.AppendLine("{");
            sb.AppendLine("    public static class PBEAbilityLocalization");
            sb.AppendLine("    {");
            sb.AppendLine("        public static ReadOnlyDictionary<PBEAbility, PBELocalizedString> Names { get; } = new ReadOnlyDictionary<PBEAbility, PBELocalizedString>(new Dictionary<PBEAbility, PBELocalizedString>()");
            sb.AppendLine("        {");
            sb.AppendLine("            { PBEAbility.None, new PBELocalizedString(\"--\", \"--\", \"--\", \"--\", \"--\", \"--\", \"--\") },");
            foreach (PBEAbility ability in allAbilities)
            {
                string abilityID = ((byte)ability).ToString();
                string japanese = names.Single(s => s[0] == abilityID && s[1] == "1")[2];
                string korean = names.Single(s => s[0] == abilityID && s[1] == "3")[2];
                string french = names.Single(s => s[0] == abilityID && s[1] == "5")[2];
                string german = names.Single(s => s[0] == abilityID && s[1] == "6")[2];
                string spanish = names.Single(s => s[0] == abilityID && s[1] == "7")[2];
                string italian = names.Single(s => s[0] == abilityID && s[1] == "8")[2];
                string english = names.Single(s => s[0] == abilityID && s[1] == "9")[2];
                switch (ability)
                {
                    case PBEAbility.Analytic: spanish = "Cálc. Final"; break;
                    case PBEAbility.ArenaTrap: french = "Piège"; break;
                    case PBEAbility.BattleArmor: spanish = "Armad. Bat."; break;
                    case PBEAbility.Chlorophyll: french = "Chlorophyle"; break;
                    case PBEAbility.Compoundeyes: spanish = "Ojocompuesto"; english = "Compoundeyes"; break;
                    case PBEAbility.CursedBody: spanish = "Cue. Maldito"; break;
                    case PBEAbility.EffectSpore: spanish = "Efec. Espora"; break;
                    case PBEAbility.FlareBoost: spanish = "Ím. Ardiente"; break;
                    case PBEAbility.FlashFire: spanish = "Absor. Fuego"; break;
                    case PBEAbility.HeavyMetal: spanish = "Met. Pesado"; break;
                    case PBEAbility.InnerFocus: italian = "Fuocodentro"; break;
                    case PBEAbility.LeafGuard: french = "Feuil. Garde"; break;
                    case PBEAbility.LightMetal: spanish = "Met. Liviano"; break;
                    case PBEAbility.Lightningrod: english = "Lightningrod"; break;
                    case PBEAbility.MagicBounce: spanish = "Espejomágico"; break;
                    case PBEAbility.MarvelScale: french = "Écaille Spé."; spanish = "Escama Esp."; break;
                    case PBEAbility.ShadowTag: spanish = "Sombratrampa"; break;
                    case PBEAbility.SheerForce: spanish = "Pot. Bruta"; break;
                    case PBEAbility.Sniper: spanish = "Francotirad."; break;
                    case PBEAbility.Static: spanish = "Elec. Estát."; break;
                    case PBEAbility.ToxicBoost: spanish = "Ím. Tóxico"; break;
                    case PBEAbility.VitalSpirit: spanish = "Espír. Vital"; break;
                    case PBEAbility.VoltAbsorb: spanish = "Absor. Elec."; break;
                    case PBEAbility.WaterAbsorb: spanish = "Absor. Agua"; break;
                    case PBEAbility.WeakArmor: spanish = "Arm. Frágil"; break;
                }
                sb.AppendLine($"            {{ PBEAbility.{ability}, new PBELocalizedString(\"{japanese}\", \"{korean}\", \"{french}\", \"{german}\", \"{spanish}\", \"{italian}\", \"{english}\") }}{(ability == allAbilities.Last() ? string.Empty : ",")}");
            }
            sb.AppendLine("        });");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(@"D:\Development\GitHub\PokemonBattleEngine\PokemonBattleEngine\Localization\AbilityLocalization.cs", sb.ToString());
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
            sb.AppendLine("            { PBEItem.None, new PBELocalizedString(\"--\", \"--\", \"--\", \"--\", \"--\", \"--\", \"--\") },");
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

            File.WriteAllText(@"D:\Development\GitHub\PokemonBattleEngine\PokemonBattleEngine\Localization\ItemLocalization.cs", sb.ToString());
        }

        public static void GenerateMoves()
        {
            // "10,000,000 volt thunderbolt" will break but it isn't needed
            string[][] names = new WebClient().DownloadString("https://raw.githubusercontent.com/veekun/pokedex/master/pokedex/data/csv/move_names.csv").Split('\n').Skip(1).Select(s => s.Split(',')).ToArray();

            var sb = new StringBuilder();
            sb.AppendLine("using Kermalis.PokemonBattleEngine.Data;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Collections.ObjectModel;");
            sb.AppendLine();
            sb.AppendLine("namespace Kermalis.PokemonBattleEngine.Localization");
            sb.AppendLine("{");
            sb.AppendLine("    public static class PBEMoveLocalization");
            sb.AppendLine("    {");
            sb.AppendLine("        public static ReadOnlyDictionary<PBEMove, PBELocalizedString> Names { get; } = new ReadOnlyDictionary<PBEMove, PBELocalizedString>(new Dictionary<PBEMove, PBELocalizedString>()");
            sb.AppendLine("        {");
            sb.AppendLine("            { PBEMove.None, new PBELocalizedString(\"--\", \"--\", \"--\", \"--\", \"--\", \"--\", \"--\") },");
            for (ushort i = 1; i < (ushort)PBEMove.MAX; i++)
            {
                bool moveExists = Enum.IsDefined(typeof(PBEMove), i);
                string moveID = i.ToString();
                string japanese = names.Single(s => s[0] == moveID && s[1] == "1")[2];
                string korean = names.Single(s => s[0] == moveID && s[1] == "3")[2];
                string french = names.Single(s => s[0] == moveID && s[1] == "5")[2];
                string german = names.Single(s => s[0] == moveID && s[1] == "6")[2];
                string spanish = names.Single(s => s[0] == moveID && s[1] == "7")[2];
                string italian = names.Single(s => s[0] == moveID && s[1] == "8")[2];
                string english = names.Single(s => s[0] == moveID && s[1] == "9")[2];
                switch ((PBEMove)i)
                {
                    case PBEMove.AcidArmor: spanish = "Armad. Ácida"; break;
                    case PBEMove.AncientPower: french = "Pouv.Antique"; english = "AncientPower"; break;
                    case PBEMove.AttackOrder: french = "Appel Attak"; break;
                    case (PBEMove)226: german = "Staffette"; break; // PBEMove.BatonPass
                    case PBEMove.BoltStrike: french = "ChargeFoudre"; spanish = "At. Fulgor"; break;
                    case PBEMove.BubbleBeam: english = "BubbleBeam"; break;
                    case (PBEMove)499: spanish = "Nieblaclara"; break; // PBEMove.ClearSmog
                    case (PBEMove)160: french = "Adaptation"; break; // PBEMove.Conversion
                    case (PBEMove)68: spanish = "Contador"; italian = "Contatore"; break; // PBEMove.Counter
                    case PBEMove.CrushClaw: french = "Éclategriffe"; break;
                    case PBEMove.DefendOrder: french = "Appel Défens"; break;
                    case (PBEMove)194: french = "Prlvt Destin"; spanish = "Mismodestino"; italian = "Destinobbl."; break; // PBEMove.DestinyBond
                    case (PBEMove)3: spanish = "Doblebofetón"; english = "DoubleSlap"; break; // PBEMove.DoubleSlap
                    case PBEMove.DracoMeteor: french = "Draco Météor"; break;
                    case PBEMove.DragonBreath: english = "DreagonBreath"; break;
                    case PBEMove.DynamicPunch: spanish = "Puñodinámico"; english = "DynamicPunch"; break;
                    case PBEMove.ExtremeSpeed: french = "Vit.Extrême"; spanish = "Vel. Extrema"; english = "ExtremeSpeed"; break;
                    case PBEMove.FaintAttack: english = "FaintAttack"; break;
                    case PBEMove.FeatherDance: english = "FeatherDance"; break;
                    case (PBEMove)424: spanish = "Colm. Ígneo"; break; // PBEMove.FireFang
                    case (PBEMove)83: french = "Danseflamme"; break; // PBEMove.FireSpin
                    case PBEMove.Flamethrower: french = "Lance-Flamme"; break;
                    case PBEMove.FlashCannon: spanish = "Foco Respl."; break;
                    case (PBEMove)338: german = "Fauna-Statue"; break; // PBEMove.FrenzyPlant
                    case PBEMove.FrostBreath: french = "SouffleGlacé"; break;
                    case (PBEMove)210: spanish = "Cortefuria"; break; // PBEMove.FuryCutter
                    case (PBEMove)202: italian = "Gigassorbim."; break; // PBEMove.GigaDrain
                    case PBEMove.Glaciate: french = "ÈreGlaciaire"; break;
                    case PBEMove.Glare: french = "Intimidation"; italian = "Bagliore"; break;
                    case (PBEMove)520: german = "Pflanzsäulen"; break; // PBEMove.GrassPledge
                    case PBEMove.GrassWhistle: english = "GrassWhistle"; break;
                    case (PBEMove)470: french = "PartageGarde"; break; // PBEMove.GuardSplit
                    case (PBEMove)385: spanish = "Cambia Def."; break; // PBEMove.GuardSwap
                    case PBEMove.HeavySlam: spanish = "Cuerpopesado"; break;
                    case PBEMove.HiddenPower: french = "Puiss. Cachée"; break;
                    case (PBEMove)136: spanish = "Pat. S. Alta"; english = "Hi Jump Kick"; break; // PBEMove.HiJumpKick
                    case (PBEMove)532: spanish = "Astadrenaje"; break; // PBEMove.HornLeech
                    case PBEMove.HyperFang: spanish = "Hip. Colmillo"; break;
                    case (PBEMove)423: spanish = "Colm. Hielo"; break; // PBEMove.IceFang
                    case PBEMove.IcePunch: french = "Poinglace"; break;
                    case PBEMove.IronDefense: spanish = "Def. Férrea"; break;
                    case PBEMove.IronHead: spanish = "Cabezahierro"; break;
                    case PBEMove.KarateChop: spanish = "Golpe Karate"; break;
                    case (PBEMove)387: french = "Dernierecour"; break; // PBEMove.LastResort
                    case PBEMove.LeafStorm: french = "Tempêteverte"; break;
                    case PBEMove.LeafTornado: spanish = "Ciclón Hojas"; break;
                    case (PBEMove)72: italian = "Megassorbim."; break; // PBEMove.MegaDrain
                    case (PBEMove)368: spanish = "Repr. Metal."; break; // PBEMove.MetalBurst
                    case PBEMove.MeteorMash: french = "Poing Météor"; break;
                    case (PBEMove)429: spanish = "Disp. Espejo"; break; // PBEMove.MirrorShot
                    case PBEMove.MudShot: spanish = "Disp. Lodo"; break;
                    case (PBEMove)300: spanish = "Chapoteolodo"; break; // PBEMove.MudSport
                    case (PBEMove)101: french = "Ténèbres"; italian = "Ombra Nott."; break; // PBEMove.NightShade
                    case PBEMove.OminousWind: spanish = "Vien. Aciago"; break;
                    case PBEMove.PoisonFang: french = "Crochetvenin"; break;
                    case PBEMove.PoisonPowder: english = "PoisonPowder"; break;
                    case (PBEMove)471: french = "PartageForce"; break; // PBEMove.PowerSplit
                    case (PBEMove)384: spanish = "Cambia Fue."; break; // PBEMove.PowerSwap
                    case PBEMove.PsychoCut: spanish = "Psico-corte"; break;
                    case (PBEMove)375: spanish = "Psico-cambio"; break; // PBEMove.PsychoShift
                    case PBEMove.QuickAttack: spanish = "At. Rápido"; break;
                    case (PBEMove)476: french = "PoudreFureur"; break; // PBEMove.RagePowder
                    case (PBEMove)13: spanish = "V. Cortante"; break; // PBEMove.RazorWind
                    case (PBEMove)574: french = "ChantAntique"; spanish = "Cantoarcaico"; break; // PBEMove.RelicSong
                    case PBEMove.Return: spanish = "Retroceso"; break;
                    case PBEMove.SacredFire: spanish = "Fuegosagrado"; break;
                    case (PBEMove)533: spanish = "Espadasanta"; break; // PBEMove.SacredSword
                    case PBEMove.SandAttack: english = "Sand-Attack"; break;
                    case PBEMove.Sandstorm: french = "Tempêtesable"; spanish = "Torm. Arena"; break;
                    case PBEMove.SecretSword: spanish = "Sablemístico"; break;
                    case (PBEMove)69: italian = "Mov. Sismico"; break; // PBEMove.SeismicToss
                    case (PBEMove)120: spanish = "Autodestruc"; english = "Selfdestruct"; break; // PBEMove.Selfdestruct
                    case (PBEMove)508: french = "Chgt Vitesse"; spanish = "Cambiomarcha"; break; // PBEMove.ShiftGear
                    case PBEMove.Slam: spanish = "Portazo"; break;
                    case (PBEMove)265: english = "SmellingSalt"; break; // PBEMove.SmellingSalt
                    case PBEMove.SmokeScreen: spanish = "Pantallahumo"; english = "SmokeScreen"; break;
                    case PBEMove.Softboiled: english = "Softboiled"; break;
                    case (PBEMove)76: english = "SolarBeam"; break; // PBEMove.SolarBeam
                    case (PBEMove)49: english = "SonicBoom"; break; // PBEMove.SonicBoom
                    case PBEMove.Steamroller: spanish = "Rodillo Púas"; break;
                    case (PBEMove)500: french = "ForceAjoutée"; spanish = "Poderreserva"; break; // PBEMove.StoredPower
                    case PBEMove.StringShot: spanish = "Disp. Demora"; break;
                    case PBEMove.Struggle: spanish = "Combate"; break;
                    case PBEMove.TechnoBlast: french = "TechnoBuster"; break;
                    case PBEMove.Teleport: spanish = "Teletransp"; break;
                    case (PBEMove)422: spanish = "Colm. Rayo"; break; // PBEMove.ThunderFang
                    case PBEMove.ThunderPunch: english = "ThunderPunch"; break;
                    case PBEMove.ThunderShock: english = "ThunderShock"; break;
                    case PBEMove.Transform: spanish = "Transform"; break;
                    case (PBEMove)167: spanish = "Triplepatada"; break; // PBEMove.TripleKick
                    case (PBEMove)41: spanish = "Dobleataque"; break; // PBEMove.Twineedle
                    case PBEMove.VCreate: french = "CoupVictoire"; break;
                    case PBEMove.Venoshock: spanish = "Cargatóxica"; break;
                    case PBEMove.ViceGrip: english = "ViceGrip"; break;
                    case (PBEMove)521: french = "ChangeÉclair"; break; // PBEMove.VoltSwitch
                    case PBEMove.WingAttack: italian = "Att. d'Ala"; break;
                    case (PBEMove)35: spanish = "Repetición"; break; // PBEMove.Wrap
                }
                sb.AppendLine($"            {(moveExists ? string.Empty : "// ")}{{ PBEMove.{(PBEMove)i}, new PBELocalizedString(\"{japanese}\", \"{korean}\", \"{french}\", \"{german}\", \"{spanish}\", \"{italian}\", \"{english}\") }}{(i == (ushort)(PBEMove.MAX - 1) ? string.Empty : ",")}");
            }
            sb.AppendLine("        });");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(@"D:\Development\GitHub\PokemonBattleEngine\PokemonBattleEngine\Localization\MoveLocalization.cs", sb.ToString());
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
                    case PBESpecies.Genesect: numForms = 5; break;
                    case PBESpecies.Giratina: numForms = 2; break;
                    case PBESpecies.Landorus: numForms = 2; break;
                    case PBESpecies.Rotom: numForms = 6; break;
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

            File.WriteAllText(@"D:\Development\GitHub\PokemonBattleEngine\PokemonBattleEngine\Localization\PokemonLocalization.cs", sb.ToString());
        }
    }
}
