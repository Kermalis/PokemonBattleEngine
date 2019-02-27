using System;
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
                br.BaseStream.Position = 0x18;
                uint numFiles = br.ReadUInt32();
                Files = new MemoryStream[numFiles];
                var startOffsets = new uint[numFiles];
                var endOffsets = new uint[numFiles];
                for (int i = 0; i < numFiles; i++)
                {
                    startOffsets[i] = br.ReadUInt32();
                    endOffsets[i] = br.ReadUInt32();
                }
                long BTNFOffset = br.BaseStream.Position;
                br.BaseStream.Position += 0x4;
                long GMIFOffset = br.ReadUInt32() + BTNFOffset;
                for (int i = 0; i < numFiles; i++)
                {
                    br.BaseStream.Position = GMIFOffset + startOffsets[i] + 0x8;
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

        public string[][] ReadTextFile(int fileNum)
        {
            using (var r = new BinaryReader(Files[fileNum]))
            {
                ushort numBlocks = r.ReadUInt16();
                ushort numEntries = r.ReadUInt16();
                r.ReadUInt32(); // fileSize
                r.ReadUInt32(); // padding
                var texts = new string[numBlocks][];
                var blockOffsets = new uint[numBlocks];
                for (int i = 0; i < numBlocks; i++)
                {
                    texts[i] = new string[numEntries];
                    blockOffsets[i] = r.ReadUInt32();
                }
                for (int i = 0; i < numBlocks; i++)
                {
                    r.BaseStream.Position = blockOffsets[i];
                    r.ReadUInt32(); // blockSize
                    var stringOffsets = new uint[numEntries];
                    var stringLengths = new ushort[numEntries];
                    for (int j = 0; j < numEntries; j++)
                    {
                        stringOffsets[j] = r.ReadUInt32();
                        stringLengths[j] = r.ReadUInt16();
                        r.ReadUInt16(); // textFlags[j]
                    }
                    for (int j = 0; j < numEntries; j++)
                    {
                        r.BaseStream.Position = blockOffsets[i] + stringOffsets[j];
                        var encoded = new ushort[stringLengths[j]];
                        for (int k = 0; k < stringLengths[j]; k++)
                        {
                            encoded[k] = r.ReadUInt16();
                        }
                        int key = encoded[stringLengths[j] - 1] ^ 0xFFFF;
                        var decoded = new int[stringLengths[j]];
                        for (int k = stringLengths[j] - 1; k >= 0; k--)
                        {
                            decoded[k] = encoded[k] ^ key;
                            key = ((key >> 3) | (key << 13)) & 0xFFFF;
                        }
                        for (int k = 0; k < stringLengths[j]; k++)
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
                                    case '"': car = "\\\""; break;
                                    case 0x246D: car = "♂"; break;
                                    case 0x246E: car = "♀"; break;
                                    case 0xFFFE: car = "\\n"; break;
                                    default: car = ((char)c).ToString(); break;
                                }
                                texts[i][j] += car;
                            }
                        }
                    }
                }
                return texts;
            }
        }
    }
}
