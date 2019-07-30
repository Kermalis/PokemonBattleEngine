using Kermalis.PokemonBattleEngine.Data;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngineExtras
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
                    eng = Utils.ReadTextFile(english, fileNum);
                    fre = Utils.ReadTextFile(french, fileNum);
                    ger = Utils.ReadTextFile(german, fileNum);
                    ita = Utils.ReadTextFile(italian, fileNum);
                    jap = Utils.ReadTextFile(japanese, fileNum);
                    kor = Utils.ReadTextFile(korean, fileNum);
                    spa = Utils.ReadTextFile(spanish, fileNum);
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
