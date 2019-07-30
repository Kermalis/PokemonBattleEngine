using System;
using System.IO;
using System.Text;

namespace Kermalis.PokemonBattleEngineExtras
{
    internal class NARCTextDumper
    {
        private static readonly string[] langs = new string[] { "English", "French", "German", "Italian", "Japanese", "Korean", "Spanish" };

        // You must dump the NARC files yourself (/a/0/0/2 in each language)
        public static void Dump()
        {
            for (int lang = 0; lang < langs.Length; lang++)
            {
                string l = langs[lang];
                string dir = Path.Combine(@"../../../\DumpedData\Dumped\Texts", l);
                Directory.CreateDirectory(dir);
                using (var narc = new NARC($@"../../../\DumpedData\W2{l}Texts.narc"))
                {
                    for (int file = 0; file < narc.Files.Length; file++)
                    {
                        string[][] fileTexts = Utils.ReadTextFile(narc, file);
                        for (int block = 0; block < fileTexts.Length; block++)
                        {
                            string[] b = fileTexts[block];
                            string s = string.Empty;
                            for (int entry = 0; entry < b.Length; entry++)
                            {
                                s += "Entry " + entry + ':' + Environment.NewLine;
                                s += b[entry] + Environment.NewLine + Environment.NewLine;
                            }
                            File.WriteAllText(Path.Combine(dir, $"{file}_{block}.txt"), s, Encoding.Unicode);
                        }
                    }
                }
            }
        }
    }
}
