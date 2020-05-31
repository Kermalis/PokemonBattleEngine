using Kermalis.PokemonBattleEngine.Data;
using Kermalis.SimpleNARC;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngineExtras
{
    internal sealed class LocalizationDumper
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
                    void WriteFormTexts(string tableName, PBESpecies species, PBEForm form, int text)
                    {
                        cmd.CommandText = $"INSERT INTO {tableName} VALUES(@0, @1, @2, @3, @4, @5, @6, @7, @8, @9)";
                        cmd.Parameters.AddWithValue("@0", (ushort)species);
                        cmd.Parameters.AddWithValue("@1", (byte)form);
                        cmd.Parameters.AddWithValue("@2", eng[0][text]);
                        cmd.Parameters.AddWithValue("@3", fre[0][text]);
                        cmd.Parameters.AddWithValue("@4", ger[0][text]);
                        cmd.Parameters.AddWithValue("@5", ita[0][text]);
                        cmd.Parameters.AddWithValue("@6", jap[0][text]);
                        cmd.Parameters.AddWithValue("@7", jap[1][text]);
                        cmd.Parameters.AddWithValue("@8", kor[0][text]);
                        cmd.Parameters.AddWithValue("@9", spa[0][text]);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    LoadTexts(450);
                    const string tableName = "FormNames";
                    cmd.CommandText = $"DROP TABLE IF EXISTS {tableName}";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = $"CREATE TABLE {tableName}(Species INTEGER, Form INTEGER, English TEXT, French TEXT, German TEXT, Italian TEXT, Japanese_Kana TEXT, Japanese_Kanji TEXT, Korean TEXT, Spanish TEXT)";
                    cmd.ExecuteNonQuery();
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_A, 201);
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
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus, 493);
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
                    WriteFormTexts(tableName, PBESpecies.Genesect, PBEForm.Genesect, 649);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_B, 651);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_C, 652);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_D, 653);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_E, 654);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_F, 655);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_G, 656);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_H, 657);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_I, 658);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_J, 659);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_K, 660);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_L, 661);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_M, 662);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_N, 663);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_O, 664);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_P, 665);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_Q, 666);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_R, 667);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_S, 668);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_T, 669);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_U, 670);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_V, 671);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_W, 672);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_X, 673);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_Y, 674);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_Z, 675);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_Exclamation, 676);
                    WriteFormTexts(tableName, PBESpecies.Unown, PBEForm.Unown_Question, 677);
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
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Fighting, 698);
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Flying, 699);
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Poison, 700);
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Ground, 701);
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Rock, 702);
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Bug, 703);
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Ghost, 704);
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Steel, 705);
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Fire, 706);
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Water, 707);
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Grass, 708);
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Electric, 709);
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Psychic, 710);
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Ice, 711);
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Dragon, 712);
                    WriteFormTexts(tableName, PBESpecies.Arceus, PBEForm.Arceus_Dark, 713);
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
                    WriteFormTexts(tableName, PBESpecies.Genesect, PBEForm.Genesect_Douse, 729);
                    WriteFormTexts(tableName, PBESpecies.Genesect, PBEForm.Genesect_Shock, 730);
                    WriteFormTexts(tableName, PBESpecies.Genesect, PBEForm.Genesect_Burn, 731);
                    WriteFormTexts(tableName, PBESpecies.Genesect, PBEForm.Genesect_Chill, 732);
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
                    IEnumerable<PBEItem> allItems = Enum.GetValues(typeof(PBEItem)).Cast<PBEItem>();
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
                    IEnumerable<PBEMove> allMoves = Enum.GetValues(typeof(PBEMove)).Cast<PBEMove>().Except(new[] { PBEMove.MAX });
                    void WriteAll(string tableName)
                    {
                        CreateTable(tableName);
                        foreach (PBEMove move in allMoves)
                        {
                            ushort i = (ushort)move;
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
                        for (ushort i = 1; i <= 649; i++)
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
