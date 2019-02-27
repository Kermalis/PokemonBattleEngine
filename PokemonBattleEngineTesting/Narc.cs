using System;
using System.Collections.Generic;
using System.IO;

namespace Kermalis.PokemonBattleEngineTesting
{
    class NARC : IDisposable
    {
        public MemoryStream[] Files;

        public NARC(string path)
        {
            using (var br = new BinaryReader(File.OpenRead(path)))
            {
                long FatbOffset = 0x10;
                br.BaseStream.Position = 0x18;
                long FntbOffset = (br.ReadUInt32() * 0x8) + FatbOffset + 0xC;
                br.BaseStream.Position = FntbOffset + 0x4;
                long FimgOffset = br.ReadUInt32() + FntbOffset;
                br.BaseStream.Position = 0x18;
                uint numFiles = br.ReadUInt32();
                Files = new MemoryStream[numFiles];
                var startOffsets = new uint[numFiles];
                var endOffsets = new uint[numFiles];
                br.BaseStream.Position = FatbOffset + 0xC;
                for (int i = 0; i < numFiles; i++)
                {
                    startOffsets[i] = br.ReadUInt32();
                    endOffsets[i] = br.ReadUInt32();
                }
                for (int i = 0; i < numFiles; i++)
                {
                    br.BaseStream.Position = FimgOffset + startOffsets[i] + 0x8;
                    Files[i] = new MemoryStream(br.ReadBytes((int)(endOffsets[i] - startOffsets[i])));
                }
            }
        }
        public void Dispose()
        {
            if (Files != null)
            {
                for (int i = 0; i < Files.Length; i++)
                {
                    Files[i].Dispose();
                }
            }
            Files = null;
        }

        // https://github.com/projectpokemon/PPRE/blob/master/nds/txt.py
        public string[][] ReadTextFile(int fileNum)
        {
            using (var r = new BinaryReader(Files[fileNum]))
            {
                ushort numBlocks = r.ReadUInt16();
                ushort numEntries = r.ReadUInt16();
                r.ReadUInt32(); // fileSize
                r.ReadUInt32(); // padding?

                var texts = new string[numBlocks][];
                var blockoffsets = new uint[numBlocks];
                for (int i = 0; i < numBlocks; i++)
                {
                    texts[i] = new string[numEntries];
                    blockoffsets[i] = r.ReadUInt32();
                }

                for (int i = 0; i < numBlocks; i++)
                {
                    r.BaseStream.Position = blockoffsets[i];
                    r.ReadUInt32(); // blockSize
                    var tableoffsets = new uint[numEntries];
                    var charcounts = new ushort[numEntries];
                    var textflags = new ushort[numEntries];
                    for (int j = 0; j < numEntries; j++)
                    {
                        tableoffsets[j] = r.ReadUInt32();
                        charcounts[j] = r.ReadUInt16();
                        textflags[j] = r.ReadUInt16();
                    }

                    for (int j = 0; j < numEntries; j++)
                    {
                        var encchars = new List<ushort>();
                        r.BaseStream.Position = blockoffsets[i] + tableoffsets[j];
                        for (int k = 0; k < charcounts[j]; k++)
                        {
                            encchars.Add(r.ReadUInt16());
                        }
                        int key = encchars[encchars.Count - 1] ^ 0xFFFF;
                        var decchars = new List<int>();
                        while (encchars.Count > 0)
                        {
                            int car = encchars[encchars.Count - 1] ^ key;
                            encchars.RemoveAt(encchars.Count - 1);
                            key = ((key >> 3) | (key << 13)) & 0xFFFF;
                            if (decchars.Count == 0)
                            {
                                decchars.Add(car);
                            }
                            else
                            {
                                decchars.Insert(0, car);
                            }
                        }
                        if (decchars[0] == 0xF100)
                        {
                            decchars.RemoveAt(0);
                            var newstring = new List<int>();
                            int container = 0;
                            int bit = 0;
                            while (decchars.Count > 0)
                            {
                                container |= decchars[0] << bit;
                                decchars.RemoveAt(0);
                                bit += 0x10;
                                while (bit >= 9)
                                {
                                    bit -= 9;
                                    int c = container & 0x1FF;
                                    if (c == 0x1FF)
                                    {
                                        newstring.Add(0xFFFF);
                                    }
                                    else
                                    {
                                        newstring.Add(c);
                                    }
                                    container >>= 9;
                                }
                            }
                            decchars = newstring;
                        }
                        while (decchars.Count > 0)
                        {
                            int c = decchars[0];
                            decchars.RemoveAt(0);
                            if (c == 0xFFFF)
                            {
                                break;
                            }
                            else if (c == 0xFFFE)
                            {
                                texts[i][j] += "\\n";
                            }
                            else if (c < 20 || c > 0xF000)
                            {
                                texts[i][j] += "\\x" + c.ToString("X4");
                            }
                            else if (c == 0xF000)
                            {
                                try
                                {
                                    int kind = decchars[0];
                                    decchars.RemoveAt(0);
                                    int count = decchars[0];
                                    decchars.RemoveAt(0);
                                    if (kind == 0xBE00 && count == 0)
                                    {
                                        texts[i][j] += "\\f";
                                        continue;
                                    }
                                    if (kind == 0xBE01 && count == 0)
                                    {
                                        texts[i][j] += "\\r";
                                        continue;
                                    }
                                    texts[i][j] += "VAR(";
                                    var args = new List<int> { kind };
                                    for (int k = 0; k < count; k++)
                                    {
                                        args.Add(decchars[0]);
                                        decchars.RemoveAt(0);
                                    }
                                    texts[i][j] += string.Join(", ", args) + ')';
                                }
                                catch
                                {
                                    break;
                                }
                            }
                            else
                            {
                                texts[i][j] += (char)c;
                            }
                        }
                    }
                }

                return texts;
            }
        }
    }
}
