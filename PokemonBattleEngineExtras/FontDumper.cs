using Kermalis.EndianBinaryIO;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Kermalis.PokemonBattleEngineExtras
{
    internal class FontDumper
    {
        private class Character
        {
            public byte SpaceWidth;
            public byte Width;
            public byte[][] Bitmap;

            public Character(EndianBinaryReader reader, byte maxWidth, byte height)
            {
                SpaceWidth = reader.ReadByte(); // Width of transparency after the char
                Width = reader.ReadByte(); // Width of this char
                reader.ReadByte(); // ?
                Bitmap = new byte[height][];
                for (int i = 0; i < height; i++)
                {
                    Bitmap[i] = new byte[maxWidth];
                }
                int curBit = 0;
                byte curByte = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < maxWidth; x++)
                    {
                        if (curBit == 0)
                        {
                            curByte = reader.ReadByte();
                        }
                        Bitmap[y][x] = (byte)((curByte >> (2 * (3 - curBit))) % 4);
                        curBit = (curBit + 1) % 4;
                    }
                }
            }
        }

        private static readonly Color[] colors = new Color[] { Color.Transparent, Color.White, Color.Black }; // value 02 is never used
        public static void Dump()
        {
            using (var narc = new NARC(@"../../../\DumpedData\Gen5Font.narc"))
            {
                void Save(int fileNum)
                {
                    using (var r = new EndianBinaryReader(narc.Files[fileNum], Endianness.LittleEndian))
                    {
                        // PLGC
                        r.BaseStream.Position = 0x30;
                        int PLGCSize = r.ReadInt32();
                        byte maxWidth = r.ReadByte();
                        byte height = r.ReadByte();
                        ushort lengthPerChar = r.ReadUInt16();
                        r.ReadByte(); // ? (Really close values to maxWidth)
                        r.ReadByte(); // "Kerning"
                        r.ReadByte(); // Bits per pixel (always 2)
                        r.ReadByte(); // Orientation (always 0)
                        int numChars = (PLGCSize - 0x10) / lengthPerChar;
                        var chars = new Character[numChars];
                        for (int i = 0; i < numChars; i++)
                        {
                            chars[i] = new Character(r, maxWidth, height);
                        }

                        // HDWC
                        // Must be aligned by 4
                        while (r.BaseStream.Position % 4 != 0)
                        {
                            r.BaseStream.Position++;
                        }
                        r.ReadBytes(4); // "HDWC"
                        int HDWCSize = r.ReadInt32();
                        r.BaseStream.Position += HDWCSize - 8; // -8 for the 8 bytes we just read of HDWC

                        // PAMC (Character map)
                        var dict = new Dictionary<ushort, ushort>();
                        // Must be aligned by 4
                        while (r.BaseStream.Position % 4 != 0)
                        {
                            r.BaseStream.Position++;
                        }
                        r.ReadBytes(4); // "PAMC"
                        r.ReadInt32(); // PAMCSize

                        // Each PAMC points to the next until there is no valid offset left
                        long nextPAMCOffset = r.BaseStream.Position;
                        while (nextPAMCOffset < r.BaseStream.Length)
                        {
                            r.BaseStream.Position = nextPAMCOffset;
                            ushort firstCharCode = r.ReadUInt16();
                            ushort lastCharCode = r.ReadUInt16();
                            uint type = r.ReadUInt32();
                            nextPAMCOffset = r.ReadInt32();
                            switch (type)
                            {
                                case 0:
                                {
                                    ushort charIndex = r.ReadUInt16();
                                    for (ushort i = firstCharCode; i <= lastCharCode; i++)
                                    {
                                        dict.Add(i, charIndex++);
                                    }
                                    break;
                                }
                                case 1:
                                {
                                    for (ushort i = firstCharCode; i <= lastCharCode; i++)
                                    {
                                        dict.Add(i, r.ReadUInt16());
                                    }
                                    break;
                                }
                                case 2:
                                {
                                    ushort numDefinitions = r.ReadUInt16();
                                    for (ushort i = 0; i < numDefinitions; ++i)
                                    {
                                        dict.Add(r.ReadUInt16(), r.ReadUInt16());
                                    }
                                    break;
                                }
                            }
                        }

                        string path = @"../../../\DumpedData\Dumped\Fonts\" + fileNum;
                        Directory.CreateDirectory(path);
                        foreach (KeyValuePair<ushort, ushort> pair in dict)
                        {
                            if (pair.Value != ushort.MaxValue)
                            {
                                Character car = chars[pair.Value];
                                using (var b = new Bitmap(car.Width + car.SpaceWidth, height))
                                {
                                    for (int y = 0; y < car.Bitmap.Length; y++)
                                    {
                                        for (int x = 0; x < car.Width; x++)
                                        {
                                            b.SetPixel(x, y, colors[car.Bitmap[y][x]]);
                                        }
                                    }
                                    b.Save(path + '\\' + pair.Key.ToString("X4") + ".png");
                                }
                            }
                        }
                    }
                }
                Save(0);
                Save(1);
                Save(2);
                Save(3);
                Save(4);
            }
        }
    }
}
