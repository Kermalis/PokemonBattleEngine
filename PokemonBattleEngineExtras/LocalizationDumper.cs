using Kermalis.PokemonBattleEngine.Data;
using Kermalis.SimpleNARC;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngineExtras
{
    internal static class LocalizationDumper
    {
        // You must dump the NARC files yourself (/a/0/0/2 in each language)
        public static void Run(SqliteConnection con)
        {
            var english = new NARC(@"../../../\DumpedData\W2EnglishTexts.narc");
            var french = new NARC(@"../../../\DumpedData\W2FrenchTexts.narc");
            var german = new NARC(@"../../../\DumpedData\W2GermanTexts.narc");
            var italian = new NARC(@"../../../\DumpedData\W2ItalianTexts.narc");
            var japanese = new NARC(@"../../../\DumpedData\W2JapaneseTexts.narc");
            var korean = new NARC(@"../../../\DumpedData\W2KoreanTexts.narc");
            var spanish = new NARC(@"../../../\DumpedData\W2SpanishTexts.narc");
            using (SqliteTransaction transaction = con.BeginTransaction())
            using (SqliteCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = transaction;

                void CreateTable(string tableName)
                {
                    cmd.CommandText = $"DROP TABLE IF EXISTS {tableName}";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = $"CREATE TABLE {tableName}(Id INTEGER PRIMARY KEY, English TEXT, French TEXT, German TEXT, Italian TEXT, Japanese_Kana TEXT, Japanese_Kanji TEXT, Korean TEXT, Spanish TEXT)";
                    cmd.ExecuteNonQuery();
                }
                void Insert(string tableName, uint id, string e, string f, string g, string i, string jkana, string jkanji, string k, string s)
                {
                    cmd.CommandText = $"INSERT INTO {tableName} VALUES(@0, @1, @2, @3, @4, @5, @6, @7, @8)";
                    cmd.Parameters.AddWithValue("@0", id);
                    cmd.Parameters.AddWithValue("@1", e);
                    cmd.Parameters.AddWithValue("@2", f);
                    cmd.Parameters.AddWithValue("@3", g);
                    cmd.Parameters.AddWithValue("@4", i);
                    cmd.Parameters.AddWithValue("@5", jkana);
                    cmd.Parameters.AddWithValue("@6", jkanji);
                    cmd.Parameters.AddWithValue("@7", k);
                    cmd.Parameters.AddWithValue("@8", s);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }

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
                void WriteTexts(string tableName, uint id, int text)
                {
                    Insert(tableName, id, eng[0][text], fre[0][text], ger[0][text], ita[0][text], jap[0][text], jap[1][text], kor[0][text], spa[0][text]);
                }

                // Abilities
                {
                    void WriteAll(string tableName)
                    {
                        CreateTable(tableName);
                        for (byte i = 0; i < (byte)PBEAbility.MAX; i++)
                        {
                            WriteTexts(tableName, i, i);
                        }
                    }
                    LoadTexts(374);
                    WriteAll("AbilityNames");
                    LoadTexts(375);
                    WriteAll("AbilityDescriptions");
                }
                // Forms
                {
                    void InsertForm(string tableName, PBESpecies species, PBEForm form, string e, string f, string g, string i, string jkana, string jkanji, string k, string s)
                    {
                        cmd.CommandText = $"INSERT INTO {tableName} VALUES(@0, @1, @2, @3, @4, @5, @6, @7, @8, @9)";
                        cmd.Parameters.AddWithValue("@0", (ushort)species);
                        cmd.Parameters.AddWithValue("@1", (byte)form);
                        cmd.Parameters.AddWithValue("@2", e);
                        cmd.Parameters.AddWithValue("@3", f);
                        cmd.Parameters.AddWithValue("@4", g);
                        cmd.Parameters.AddWithValue("@5", i);
                        cmd.Parameters.AddWithValue("@6", jkana);
                        cmd.Parameters.AddWithValue("@7", jkanji);
                        cmd.Parameters.AddWithValue("@8", k);
                        cmd.Parameters.AddWithValue("@9", s);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    void WriteFormTexts(string tableName, PBESpecies species, PBEForm form, int text)
                    {
                        InsertForm(tableName, species, form, eng[0][text], fre[0][text], ger[0][text], ita[0][text], jap[0][text], jap[1][text], kor[0][text], spa[0][text]);
                    }
                    void WriteUnown(string tableName, PBEForm form, string letter)
                    {
                        InsertForm(tableName, PBESpecies.Unown, form, letter, letter, letter, letter, letter, letter, letter, letter);
                    }
                    void WriteArceusGenesect(string tableName, PBESpecies species, PBEForm form, PBEType type)
                    {
                        int text = -1;
                        for (int i = 0; i < Utils.Gen5Types.Length; i++)
                        {
                            if (Utils.Gen5Types[i] == type)
                            {
                                text = i;
                                break;
                            }
                        }
                        WriteFormTexts(tableName, species, form, text);
                    }
                    LoadTexts(450);
                    const string tableName = "FormNames";
                    cmd.CommandText = $"DROP TABLE IF EXISTS {tableName}";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = $"CREATE TABLE {tableName}(Species INTEGER, Form INTEGER, English TEXT, French TEXT, German TEXT, Italian TEXT, Japanese_Kana TEXT, Japanese_Kanji TEXT, Korean TEXT, Spanish TEXT)";
                    cmd.ExecuteNonQuery();
                    WriteFormTexts(tableName, PBESpecies.Castform, PBEForm.Castform, 351);
                    WriteFormTexts(tableName, PBESpecies.Deoxys, PBEForm.Deoxys, 386);
                    WriteFormTexts(tableName, PBESpecies.Burmy, PBEForm.Burmy_Plant, 412);
                    WriteFormTexts(tableName, PBESpecies.Wormadam, PBEForm.Wormadam_Plant, 413);
                    WriteFormTexts(tableName, PBESpecies.Cherrim, PBEForm.Cherrim, 421);
                    WriteFormTexts(tableName, PBESpecies.Shellos, PBEForm.Shellos_West, 422);
                    WriteFormTexts(tableName, PBESpecies.Gastrodon, PBEForm.Gastrodon_West, 423);
                    WriteFormTexts(tableName, PBESpecies.Rotom, PBEForm.Rotom, 479);
                    WriteFormTexts(tableName, PBESpecies.Giratina, PBEForm.Giratina, 487);
                    WriteFormTexts(tableName, PBESpecies.Shaymin, PBEForm.Shaymin, 492);
                    WriteFormTexts(tableName, PBESpecies.Basculin, PBEForm.Basculin_Red, 550);
                    WriteFormTexts(tableName, PBESpecies.Darmanitan, PBEForm.Darmanitan, 555);
                    WriteFormTexts(tableName, PBESpecies.Deerling, PBEForm.Deerling_Spring, 585);
                    WriteFormTexts(tableName, PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Spring, 586);
                    WriteFormTexts(tableName, PBESpecies.Tornadus, PBEForm.Tornadus, 641);
                    WriteFormTexts(tableName, PBESpecies.Thundurus, PBEForm.Thundurus, 642);
                    WriteFormTexts(tableName, PBESpecies.Landorus, PBEForm.Landorus, 645);
                    WriteFormTexts(tableName, PBESpecies.Kyurem, PBEForm.Kyurem, 646);
                    WriteFormTexts(tableName, PBESpecies.Keldeo, PBEForm.Keldeo, 647);
                    WriteFormTexts(tableName, PBESpecies.Meloetta, PBEForm.Meloetta, 648);
                    WriteFormTexts(tableName, PBESpecies.Castform, PBEForm.Castform_Sunny, 678);
                    WriteFormTexts(tableName, PBESpecies.Castform, PBEForm.Castform_Rainy, 679);
                    WriteFormTexts(tableName, PBESpecies.Castform, PBEForm.Castform_Snowy, 680);
                    WriteFormTexts(tableName, PBESpecies.Deoxys, PBEForm.Deoxys_Attack, 681);
                    WriteFormTexts(tableName, PBESpecies.Deoxys, PBEForm.Deoxys_Defense, 682);
                    WriteFormTexts(tableName, PBESpecies.Deoxys, PBEForm.Deoxys_Speed, 683);
                    WriteFormTexts(tableName, PBESpecies.Burmy, PBEForm.Burmy_Sandy, 684);
                    WriteFormTexts(tableName, PBESpecies.Burmy, PBEForm.Burmy_Trash, 685);
                    WriteFormTexts(tableName, PBESpecies.Wormadam, PBEForm.Wormadam_Sandy, 686);
                    WriteFormTexts(tableName, PBESpecies.Wormadam, PBEForm.Wormadam_Trash, 687);
                    WriteFormTexts(tableName, PBESpecies.Cherrim, PBEForm.Cherrim_Sunshine, 688);
                    WriteFormTexts(tableName, PBESpecies.Shellos, PBEForm.Shellos_East, 689);
                    WriteFormTexts(tableName, PBESpecies.Gastrodon, PBEForm.Gastrodon_East, 690);
                    WriteFormTexts(tableName, PBESpecies.Rotom, PBEForm.Rotom_Heat, 691);
                    WriteFormTexts(tableName, PBESpecies.Rotom, PBEForm.Rotom_Wash, 692);
                    WriteFormTexts(tableName, PBESpecies.Rotom, PBEForm.Rotom_Frost, 693);
                    WriteFormTexts(tableName, PBESpecies.Rotom, PBEForm.Rotom_Fan, 694);
                    WriteFormTexts(tableName, PBESpecies.Rotom, PBEForm.Rotom_Mow, 695);
                    WriteFormTexts(tableName, PBESpecies.Giratina, PBEForm.Giratina_Origin, 696);
                    WriteFormTexts(tableName, PBESpecies.Shaymin, PBEForm.Shaymin_Sky, 697);
                    WriteFormTexts(tableName, PBESpecies.Basculin, PBEForm.Basculin_Blue, 714);
                    WriteFormTexts(tableName, PBESpecies.Darmanitan, PBEForm.Darmanitan_Zen, 715);
                    WriteFormTexts(tableName, PBESpecies.Deerling, PBEForm.Deerling_Summer, 716);
                    WriteFormTexts(tableName, PBESpecies.Deerling, PBEForm.Deerling_Autumn, 717);
                    WriteFormTexts(tableName, PBESpecies.Deerling, PBEForm.Deerling_Winter, 718);
                    WriteFormTexts(tableName, PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Summer, 719);
                    WriteFormTexts(tableName, PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Autumn, 720);
                    WriteFormTexts(tableName, PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Winter, 721);
                    WriteFormTexts(tableName, PBESpecies.Tornadus, PBEForm.Tornadus_Therian, 722);
                    WriteFormTexts(tableName, PBESpecies.Thundurus, PBEForm.Thundurus_Therian, 723);
                    WriteFormTexts(tableName, PBESpecies.Landorus, PBEForm.Landorus_Therian, 724);
                    WriteFormTexts(tableName, PBESpecies.Kyurem, PBEForm.Kyurem_White, 725);
                    WriteFormTexts(tableName, PBESpecies.Kyurem, PBEForm.Kyurem_Black, 726);
                    WriteFormTexts(tableName, PBESpecies.Keldeo, PBEForm.Keldeo_Resolute, 727);
                    WriteFormTexts(tableName, PBESpecies.Meloetta, PBEForm.Meloetta_Pirouette, 728);
                    // All Unown forms are called "One Form", all Arceus forms are called "Arceus", and all Genesect forms are called "Genesect", so I'm changing them here
                    WriteUnown(tableName, PBEForm.Unown_A, "A");
                    WriteUnown(tableName, PBEForm.Unown_B, "B");
                    WriteUnown(tableName, PBEForm.Unown_C, "C");
                    WriteUnown(tableName, PBEForm.Unown_D, "D");
                    WriteUnown(tableName, PBEForm.Unown_E, "E");
                    WriteUnown(tableName, PBEForm.Unown_Exclamation, "!");
                    WriteUnown(tableName, PBEForm.Unown_F, "F");
                    WriteUnown(tableName, PBEForm.Unown_G, "G");
                    WriteUnown(tableName, PBEForm.Unown_H, "H");
                    WriteUnown(tableName, PBEForm.Unown_I, "I");
                    WriteUnown(tableName, PBEForm.Unown_J, "J");
                    WriteUnown(tableName, PBEForm.Unown_K, "K");
                    WriteUnown(tableName, PBEForm.Unown_L, "L");
                    WriteUnown(tableName, PBEForm.Unown_M, "M");
                    WriteUnown(tableName, PBEForm.Unown_N, "N");
                    WriteUnown(tableName, PBEForm.Unown_O, "O");
                    WriteUnown(tableName, PBEForm.Unown_P, "P");
                    WriteUnown(tableName, PBEForm.Unown_Q, "Q");
                    WriteUnown(tableName, PBEForm.Unown_Question, "?");
                    WriteUnown(tableName, PBEForm.Unown_R, "R");
                    WriteUnown(tableName, PBEForm.Unown_S, "S");
                    WriteUnown(tableName, PBEForm.Unown_T, "T");
                    WriteUnown(tableName, PBEForm.Unown_U, "U");
                    WriteUnown(tableName, PBEForm.Unown_V, "V");
                    WriteUnown(tableName, PBEForm.Unown_W, "W");
                    WriteUnown(tableName, PBEForm.Unown_X, "X");
                    WriteUnown(tableName, PBEForm.Unown_Y, "Y");
                    WriteUnown(tableName, PBEForm.Unown_Z, "Z");
                    LoadTexts(398); // Load types texts
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus, PBEType.Normal);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Bug, PBEType.Bug);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Dark, PBEType.Dark);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Dragon, PBEType.Dragon);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Electric, PBEType.Electric);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Fighting, PBEType.Fighting);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Fire, PBEType.Fire);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Flying, PBEType.Flying);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Ghost, PBEType.Ghost);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Grass, PBEType.Grass);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Ground, PBEType.Ground);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Ice, PBEType.Ice);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Poison, PBEType.Poison);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Psychic, PBEType.Psychic);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Rock, PBEType.Rock);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Steel, PBEType.Steel);
                    WriteArceusGenesect(tableName, PBESpecies.Arceus, PBEForm.Arceus_Water, PBEType.Water);
                    WriteArceusGenesect(tableName, PBESpecies.Genesect, PBEForm.Genesect, PBEType.Normal);
                    WriteArceusGenesect(tableName, PBESpecies.Genesect, PBEForm.Genesect_Burn, PBEType.Fire);
                    WriteArceusGenesect(tableName, PBESpecies.Genesect, PBEForm.Genesect_Chill, PBEType.Ice);
                    WriteArceusGenesect(tableName, PBESpecies.Genesect, PBEForm.Genesect_Douse, PBEType.Water);
                    WriteArceusGenesect(tableName, PBESpecies.Genesect, PBEForm.Genesect_Shock, PBEType.Electric);
                }
                // Genders (Does not have PBEGender.Genderless)
                {
                    LoadTexts(441);
                    const string tableName = "GenderNames";
                    CreateTable(tableName);
                    WriteTexts(tableName, (byte)PBEGender.Female, 115);
                    Insert(tableName, (byte)PBEGender.Genderless, "Unknown", "Inconnu", "Unbekannt", "Sconosciuto", "不明のすがた", "不明のすがた", "불명의 모습", "Desconocido");
                    WriteTexts(tableName, (byte)PBEGender.Male, 114);
                }
                // Items
                {
                    PBEItem[] allItems = Enum.GetValues<PBEItem>();
                    void WriteAll(string tableName)
                    {
                        CreateTable(tableName);
                        foreach (PBEItem item in allItems)
                        {
                            ushort i = (ushort)item;
                            WriteTexts(tableName, i, i);
                        }
                    }
                    LoadTexts(63);
                    WriteAll("ItemDescriptions");
                    LoadTexts(64);
                    WriteAll("ItemNames");
                }
                // Moves
                {
                    void WriteAll(string tableName)
                    {
                        CreateTable(tableName);
                        for (ushort i = 0; i < (ushort)PBEMove.MAX; i++)
                        {
                            WriteTexts(tableName, i, i);
                        }
                    }
                    LoadTexts(402);
                    WriteAll("MoveDescriptions");
                    LoadTexts(403);
                    WriteAll("MoveNames");
                }
                // Natures
                {
                    LoadTexts(379);
                    const string tableName = "NatureNames";
                    CreateTable(tableName);
                    for (byte i = 0; i < (byte)PBENature.MAX; i++)
                    {
                        WriteTexts(tableName, i, i + 35); // Nature 0 is at entry 35 in this file
                    }
                }
                // Species
                {
                    void WriteAll(string tableName)
                    {
                        CreateTable(tableName);
                        for (ushort i = 1; i < (ushort)PBESpecies.MAX; i++)
                        {
                            WriteTexts(tableName, i, i);
                        }
                    }
                    LoadTexts(90);
                    WriteAll("SpeciesNames");
                    LoadTexts(442);
                    WriteAll("SpeciesEntries");
                    LoadTexts(464);
                    WriteAll("SpeciesCategories");
                }
                // Stats (Non-Japanese languages do not have PBEStat.Accuracy or PBEStat.Evasion)
                {
                    LoadTexts(372);
                    const string tableName = "StatNames";
                    CreateTable(tableName);
                    WriteTexts(tableName, (byte)PBEStat.HP, 0);
                    WriteTexts(tableName, (byte)PBEStat.Attack, 1);
                    WriteTexts(tableName, (byte)PBEStat.Defense, 2);
                    WriteTexts(tableName, (byte)PBEStat.SpAttack, 4);
                    WriteTexts(tableName, (byte)PBEStat.SpDefense, 5);
                    WriteTexts(tableName, (byte)PBEStat.Speed, 3);
                    Insert(tableName, (byte)PBEStat.Accuracy, "Accuracy", "Précision", "Genauigkeit", "Precisione", jap[0][6], jap[1][6], "명중률", "Precisión");
                    Insert(tableName, (byte)PBEStat.Evasion, "Evasiveness", "Esquive", "Fluchtwert", "Elusione", jap[0][7], jap[1][7], "회피율", "Evasión");
                }
                // Types (Does not have PBEType.None)
                {
                    LoadTexts(398);
                    const string tableName = "TypeNames";
                    CreateTable(tableName);
                    const string none =  "-----";
                    Insert(tableName, (byte)PBEType.None, none, none, none, none, none, none, none, none);
                    for (byte i = 0; i < Utils.Gen5Types.Length; i++)
                    {
                        WriteTexts(tableName, (byte)Utils.Gen5Types[i], i);
                    }
                }

                transaction.Commit();
            }
        }
    }
}
