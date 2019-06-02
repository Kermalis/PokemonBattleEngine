using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngineTesting
{
    internal class LocalizationDumper
    {
        // You must dump the NARC files yourself (/a/0/0/2 in each language)
        public static void Dump(SqliteConnection con)
        {
            using (var english = new NARC(@"../../../\DumpedData\W2EnglishTexts.narc"))
            using (var french = new NARC(@"../../../\DumpedData\W2FrenchTexts.narc"))
            using (var german = new NARC(@"../../../\DumpedData\W2GermanTexts.narc"))
            using (var italian = new NARC(@"../../../\DumpedData\W2ItalianTexts.narc"))
            using (var japanese = new NARC(@"../../../\DumpedData\W2JapaneseTexts.narc"))
            using (var korean = new NARC(@"../../../\DumpedData\W2KoreanTexts.narc"))
            using (var spanish = new NARC(@"../../../\DumpedData\W2SpanishTexts.narc"))
            using (SqliteTransaction transaction = con.BeginTransaction())
            using (SqliteCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = transaction;

                string[][] eng, fre, ger, ita, jap, kor, spa;
                void LoadTexts(int fileNum)
                {
                    string[][] ReadTextFile(NARC narc)
                    {
                        using (var r = new EndianBinaryReader(narc.Files[fileNum], Endianness.LittleEndian))
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

                    eng = ReadTextFile(english);
                    fre = ReadTextFile(french);
                    ger = ReadTextFile(german);
                    ita = ReadTextFile(italian);
                    jap = ReadTextFile(japanese);
                    kor = ReadTextFile(korean);
                    spa = ReadTextFile(spanish);
                }
                void CreateTable(string tableName)
                {
                    cmd.CommandText = $"DROP TABLE IF EXISTS {tableName}";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = $"CREATE TABLE {tableName}(Id INTEGER PRIMARY KEY, English TEXT, French TEXT, German TEXT, Italian TEXT, Japanese_Kana TEXT, Japanese_Kanji TEXT, Korean TEXT, Spanish TEXT)";
                    cmd.ExecuteNonQuery();
                }
                void WriteTexts(string tableName, uint id)
                {
                    cmd.CommandText = $"INSERT INTO {tableName} VALUES(@0, @1, @2, @3, @4, @5, @6, @7, @8)";
                    cmd.Parameters.AddWithValue("@0", id);
                    cmd.Parameters.AddWithValue("@1", eng[0][id]);
                    cmd.Parameters.AddWithValue("@2", fre[0][id]);
                    cmd.Parameters.AddWithValue("@3", ger[0][id]);
                    cmd.Parameters.AddWithValue("@4", ita[0][id]);
                    cmd.Parameters.AddWithValue("@5", jap[0][id]);
                    cmd.Parameters.AddWithValue("@6", jap[1][id]);
                    cmd.Parameters.AddWithValue("@7", kor[0][id]);
                    cmd.Parameters.AddWithValue("@8", spa[0][id]);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }

                // Abilities
                {
                    void WriteAll(string tableName)
                    {
                        CreateTable(tableName);
                        for (byte i = 0; i < (byte)PBEAbility.MAX; i++)
                        {
                            WriteTexts(tableName, i);
                        }
                    }
                    LoadTexts(374);
                    WriteAll("AbilityNames");
                    LoadTexts(375);
                    WriteAll("AbilityDescriptions");
                }
                // Items
                {
                    IEnumerable<PBEItem> allItems = Enum.GetValues(typeof(PBEItem)).Cast<PBEItem>();
                    void WriteAll(string tableName)
                    {
                        CreateTable(tableName);
                        foreach (PBEItem item in allItems)
                        {
                            WriteTexts(tableName, (ushort)item);
                        }
                    }
                    LoadTexts(63);
                    WriteAll("ItemDescriptions");
                    LoadTexts(64);
                    WriteAll("ItemNames");
                }
                // Moves
                {
                    IEnumerable<PBEMove> allMoves = Enum.GetValues(typeof(PBEMove)).Cast<PBEMove>().Except(new[] { PBEMove.MAX });
                    void WriteAll(string tableName)
                    {
                        CreateTable(tableName);
                        foreach (PBEMove move in allMoves)
                        {
                            WriteTexts(tableName, (ushort)move);
                        }
                    }
                    LoadTexts(402);
                    WriteAll("MoveDescriptions");
                    LoadTexts(403);
                    WriteAll("MoveNames");
                }
                // Species
                {
                    IEnumerable<PBESpecies> allSpecies = Enum.GetValues(typeof(PBESpecies)).Cast<PBESpecies>().Where(e => (uint)e >> 0x10 == 0);
                    void WriteAll(string tableName)
                    {
                        CreateTable(tableName);
                        foreach (PBESpecies species in allSpecies)
                        {
                            WriteTexts(tableName, (ushort)species);
                        }
                    }
                    LoadTexts(90);
                    WriteAll("SpeciesNames");
                    LoadTexts(442);
                    WriteAll("SpeciesEntries");
                    LoadTexts(464);
                    WriteAll("SpeciesCategories");
                }

                transaction.Commit();
            }
        }
    }
}
