using Kermalis.PokemonBattleEngine.Data;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Localization
{
    static class PBELocalizationGenerator
    {
        public static void GenerateMoves()
        {
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
            sb.AppendLine("            { PBEMove.None, new PBELocalizedString(\"--\", \"--\") },");
            string data = new WebClient().DownloadString("https://raw.githubusercontent.com/veekun/pokedex/master/pokedex/data/csv/move_names.csv");
            string[][] lines = data.Split('\n').Skip(1).Select(s => s.Split(',')).ToArray(); // "10,000,000 volt thunderbolt" will break but it isn't needed
            for (ushort i = 1; i < (ushort)PBEMove.MAX; i++)
            {
                bool moveExists = Enum.IsDefined(typeof(PBEMove), i);
                string japanese = lines.Single(s => s[0] == i.ToString() && s[1] == 1.ToString())[2];
                string french; // = lines.Single(s => s[0] == i.ToString() && s[1] == 5.ToString())[2];
                string german; // 6
                string spanish; // 7
                string italian; // 8
                string english = lines.Single(s => s[0] == i.ToString() && s[1] == 9.ToString())[2];
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
                    case (PBEMove)68: italian = "Contatore"; spanish = "Contador"; break; // PBEMove.Counter
                    case PBEMove.CrushClaw: french = "Éclategriffe"; break;
                    case PBEMove.DefendOrder: french = "Appel Défens"; break;
                    case (PBEMove)194: french = "Prlvt Destin"; italian = "Destinobbl."; spanish = "Mismodestino"; break; // PBEMove.DestinyBond
                    case (PBEMove)3: english = "DoubleSlap"; spanish = "Doblebofetón"; break; // PBEMove.DoubleSlap
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
                sb.AppendLine($"            {(moveExists ? string.Empty : "// ")}{{ PBEMove.{(PBEMove)i}, new PBELocalizedString(\"{english}\", \"{japanese}\") }}{(i == (ushort)(PBEMove.MAX - 1) ? string.Empty : ",")}");
            }
            sb.AppendLine("        });");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            File.WriteAllText(@"D:\Development\GitHub\PokemonBattleEngine\PokemonBattleEngine\Localization\MoveLocalization.cs", sb.ToString());
        }
    }
}
