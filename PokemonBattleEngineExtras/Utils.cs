using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.SimpleNARC;
using System.IO;

namespace Kermalis.PokemonBattleEngineExtras
{
    internal sealed class Utils
    {
        public static PBEType[] Gen5Types { get; } = new PBEType[17]
        {
            PBEType.Normal,
            PBEType.Fighting,
            PBEType.Flying,
            PBEType.Poison,
            PBEType.Ground,
            PBEType.Rock,
            PBEType.Bug,
            PBEType.Ghost,
            PBEType.Steel,
            PBEType.Fire,
            PBEType.Water,
            PBEType.Grass,
            PBEType.Electric,
            PBEType.Psychic,
            PBEType.Ice,
            PBEType.Dragon,
            PBEType.Dark
        };

        public static string[][] ReadTextFile(NARC narc, int fileNum)
        {
            using (var r = new EndianBinaryReader(new MemoryStream(narc[fileNum]), Endianness.LittleEndian))
            {
                ushort numBlocks = r.ReadUInt16();
                ushort numEntries = r.ReadUInt16();
                r.ReadUInt32(); // fileSize
                r.ReadUInt32(); // padding
                string[][] texts = new string[numBlocks][];
                uint[] blockOffsets = new uint[numBlocks];
                for (int i = 0; i < numBlocks; i++)
                {
                    texts[i] = new string[numEntries];
                    blockOffsets[i] = r.ReadUInt32();
                }
                for (int i = 0; i < numBlocks; i++)
                {
                    r.BaseStream.Position = blockOffsets[i];
                    r.ReadUInt32(); // blockSize
                    uint[] stringOffsets = new uint[numEntries];
                    ushort[] stringLengths = new ushort[numEntries];
                    for (int j = 0; j < numEntries; j++)
                    {
                        stringOffsets[j] = r.ReadUInt32();
                        stringLengths[j] = r.ReadUInt16();
                        r.ReadUInt16(); // textFlags[j]
                    }
                    for (int j = 0; j < numEntries; j++)
                    {
                        r.BaseStream.Position = blockOffsets[i] + stringOffsets[j];
                        ushort len = stringLengths[j];
                        ushort[] encoded = new ushort[len];
                        for (int k = 0; k < len; k++)
                        {
                            encoded[k] = r.ReadUInt16();
                        }
                        int key = encoded[len - 1] ^ 0xFFFF;
                        int[] decoded = new int[len];
                        for (int k = len - 1; k >= 0; k--)
                        {
                            decoded[k] = encoded[k] ^ key;
                            key = ((key >> 3) | (key << 13)) & 0xFFFF;
                        }
                        string text = string.Empty; // Prevent null entries
                        for (int k = 0; k < len; k++)
                        {
                            int c = decoded[k];
                            if (c == 0xFFFF)
                            {
                                break;
                            }
                            else
                            {
                                string car;
                                switch (c)
                                {
                                    case '"': car = "”"; break;
                                    case 0x246D: car = "♂"; break;
                                    case 0x246E: car = "♀"; break;
                                    case 0x2486: car = "[PK]"; break;
                                    case 0x2487: car = "[MN]"; break;
                                    case 0xFFFE: car = "\n"; break;
                                    default: car = ((char)c).ToString(); break;
                                }
                                text += car;
                            }
                        }
                        texts[i][j] = text;
                    }
                }
                return texts;
            }
        }
    }
}
