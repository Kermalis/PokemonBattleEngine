using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.SimpleNARC;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngineExtras
{
    internal sealed partial class PokemonDataDumper
    {
        // You must dump everything yourself
        // The GBA ROMs must all be USA v1.0
        // Colo and XD must be USA
        // DPPt dumps use overlay files which may or may not have different offsets depending on the region, so just keep in mind I have USA versions of each game
        // HGSS/Gen5 dumps should work across all regions
        //
        // Colo and XD level-up moves are in common.fsys/common_rel.fdat
        //
        // D, P, and Pt level-up move NARC is /poketool/personal/wotbl.narc (D and P have identical level-up move NARCs)
        // D, P, and Pt egg moves are in overlay/overlay_0005.bin
        // Pt TMHM moves are in the Pokémon data NARC which is /poketool/personal/pl_personal.narc (Pt changed no TMHM compatibility from DP so I use it alone)
        // Pt tutor compatibility is in overlay/overlay_0005.bin
        // HG and SS level-up move NARC is /a/0/3/3 (HG and SS have identical level-up move NARCs)
        // HG and SS TMHM moves are in the Pokémon data NARC which is /a/0/0/2 (HG and SS have identical Pokémon data NARCs)
        // HG and SS tutor compatibility is in /fielddata/wazaoshie/waza_oshie.bin (HG and SS have identical tutor compatibility)
        // HG and SS egg move NARC is /a/2/2/9 (HG and SS have identical egg move NARCs)
        //
        // B2 and W2 evolution NARC is /a/0/1/9 (B2 and W2 have identical evolution NARCs)
        // B, W, B2, and W2 level-up move NARC is /a/0/1/8 (B and W have identical level-up move NARCs) (B2 and W2 have identical level-up move NARCs)
        // B, W, B2, and W2 TMHM moves are in the Pokémon data NARC which is /a/0/1/6 (B and W have identical Pokémon data NARCs) (B2 and W2 have identical Pokémon data NARCs)
        // B2 and W2 tutor compatibility is in the Pokémon data NARC which is /a/0/1/6 (B2 and W2 have identical Pokémon data NARCs)
        // B and W egg move NARC is /a/1/2/3, B2 and W2 egg move NARC is /a/1/2/4 (B, W, B2, and W2 have identical egg move NARCs)
        public static void Run(SqliteConnection con)
        {
#pragma warning disable CS8321 // Local function is declared but never used
            using (var r = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\R.gba"), Endianness.LittleEndian))
            using (var s = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\S.gba"), Endianness.LittleEndian))
            using (var fr = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\FR.gba"), Endianness.LittleEndian))
            using (var lg = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\LG.gba"), Endianness.LittleEndian))
            using (var e = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\E.gba"), Endianness.LittleEndian))
            using (var coloCommonRel = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\Colocommon_rel.fdat"), Endianness.BigEndian))
            using (var xdCommonRel = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\XDcommon_rel.fdat"), Endianness.BigEndian))
            using (SqliteTransaction transaction = con.BeginTransaction())
            using (SqliteCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = transaction;

                var dict = new Dictionary<(PBESpecies, PBEForm), Pokemon>();
                #region Helpers
                Pokemon AddSpecies((PBESpecies, PBEForm) key)
                {
                    if (!dict.TryGetValue(key, out Pokemon pkmn))
                    {
                        pkmn = new Pokemon();
                        dict.Add(key, pkmn);
                    }
                    return pkmn;
                }
                void AddLevelUpMove((PBESpecies, PBEForm) key, PBEMove move, byte level, PBEMoveObtainMethod flag)
                {
                    Pokemon pkmn = AddSpecies(key);
                    Dictionary<(PBEMove, byte), PBEMoveObtainMethod> levelUp = pkmn.LevelUpMoves;
                    (PBEMove, byte) lKey = (move, level);
                    if (levelUp.ContainsKey(lKey))
                    {
                        levelUp[lKey] |= flag;
                    }
                    else
                    {
                        levelUp.Add(lKey, flag);
                    }
                }
                void AddOtherMove((PBESpecies, PBEForm) key, PBEMove move, PBEMoveObtainMethod flag)
                {
                    Pokemon pkmn = AddSpecies(key);
                    Dictionary<PBEMove, PBEMoveObtainMethod> other = pkmn.OtherMoves;
                    if (other.ContainsKey(move))
                    {
                        other[move] |= flag;
                    }
                    else
                    {
                        other.Add(move, flag);
                    }
                }
                (PBESpecies, PBEForm) GetGen3Key(EndianBinaryReader reader, int sp)
                {
                    PBESpecies species = _gen3SpeciesIndexToPBESpecies[sp];
                    PBEForm form = 0;
                    if (species == PBESpecies.Deoxys)
                    {
                        if (reader == e)
                        {
                            form = PBEForm.Deoxys_Speed;
                        }
                        else if (reader == lg)
                        {
                            form = PBEForm.Deoxys_Defense;
                        }
                        else if (reader == fr)
                        {
                            form = PBEForm.Deoxys_Attack;
                        }
                    }
                    return (species, form);
                }
                void AddEvolution((PBESpecies, PBEForm) baybee, (PBESpecies, PBEForm) dadee)
                {
                    AddSpecies(baybee).Evolutions.Add(dadee);
                    AddSpecies(dadee).PreEvolutions.Add(baybee);
                }
                #endregion

                #region Pokémon Data
                {
                    var b2w2Pokedata = new NARC(@"../../../\DumpedData\B2W2Pokedata.narc");
                    var b2w2Evolution = new NARC(@"../../../\DumpedData\B2W2Evolution.narc");
                    for (int sp = 1; sp <= 708; sp++)
                    {
                        // Skip Egg, Bad Egg, and Pokéstar Studios Pokémon
                        if (sp <= 649 || sp >= 685)
                        {
                            if (!_b2w2SpeciesIndexToPBESpecies.TryGetValue(sp, out (PBESpecies, PBEForm) key))
                            {
                                key = ((PBESpecies)sp, 0);
                            }
                            Pokemon pkmn = AddSpecies(key);
                            using (var pokedata = new EndianBinaryReader(new MemoryStream(b2w2Pokedata[sp]), Endianness.LittleEndian))
                            using (var evolution = new EndianBinaryReader(new MemoryStream(b2w2Evolution[sp]), Endianness.LittleEndian))
                            {
                                // Pokedata
                                pkmn.HP = pokedata.ReadByte(0x0);
                                pkmn.Attack = pokedata.ReadByte(0x1);
                                pkmn.Defense = pokedata.ReadByte(0x2);
                                pkmn.SpAttack = pokedata.ReadByte(0x4);
                                pkmn.SpDefense = pokedata.ReadByte(0x5);
                                pkmn.Speed = pokedata.ReadByte(0x3);
                                pkmn.Type1 = Utils.Gen5Types[pokedata.ReadByte(0x6)];
                                pkmn.Type2 = Utils.Gen5Types[pokedata.ReadByte(0x7)];
                                if (pkmn.Type1 == pkmn.Type2)
                                {
                                    pkmn.Type2 = PBEType.None;
                                }
                                pkmn.CatchRate = pokedata.ReadByte(0x8);
                                pkmn.GenderRatio = (PBEGenderRatio)pokedata.ReadByte(0x12);
                                pkmn.GrowthRate = (PBEGrowthRate)pokedata.ReadByte(0x15);
                                for (int i = 0; i < 3; i++)
                                {
                                    var ability = (PBEAbility)pokedata.ReadByte(0x18 + i);
                                    if (ability != PBEAbility.None && !pkmn.Abilities.Contains(ability))
                                    {
                                        pkmn.Abilities.Add(ability);
                                    }
                                }
                                pkmn.FleeRate = pokedata.ReadByte(0x1B);
                                pkmn.BaseEXPYield = pokedata.ReadUInt16(0x22);
                                pkmn.Weight = MathF.Round(pokedata.ReadUInt16(0x26) * 0.1f, 1);
                                // Evolution
                                for (int i = 0; i < 7; i++)
                                {
                                    ushort method = evolution.ReadUInt16();
                                    evolution.ReadUInt16(); // Param
                                    var evo = (PBESpecies)evolution.ReadUInt16();
                                    if (method != 0)
                                    {
                                        AddEvolution(key, (evo, 0));
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Level Up Moves

                #region Gen 3
                for (int sp = 1; sp <= 411; sp++)
                {
                    // Gen 2 Unown slots are ignored in gen 3
                    if (sp <= 251 || sp >= 277)
                    {
                        // It is the same in Ruby, Sapphire, Colo, and XD; the others have some differences
                        r.BaseStream.Position = 0x207BC8 + (sizeof(uint) * sp);
                        s.BaseStream.Position = 0x207B58 + (sizeof(uint) * sp);
                        fr.BaseStream.Position = 0x25D7B4 + (sizeof(uint) * sp);
                        lg.BaseStream.Position = 0x25D794 + (sizeof(uint) * sp);
                        e.BaseStream.Position = 0x32937C + (sizeof(uint) * sp);
                        coloCommonRel.BaseStream.Position = 0x123250 + (0x11C * sp) + 0xBA;
                        xdCommonRel.BaseStream.Position = 0x29DA8 + (0x124 * sp) + 0xC4;
                        void ReadGBALevelUpMoves(EndianBinaryReader reader, PBEMoveObtainMethod flag)
                        {
                            (PBESpecies, PBEForm) key = GetGen3Key(reader, sp);
                            reader.BaseStream.Position = reader.ReadUInt32() - 0x8000000;
                            while (true)
                            {
                                ushort val = reader.ReadUInt16();
                                if (val == 0xFFFF)
                                {
                                    break;
                                }
                                else
                                {
                                    AddLevelUpMove(key, (PBEMove)(val & 0x1FF), (byte)(val >> 9), flag);
                                }
                            }
                        }
                        ReadGBALevelUpMoves(r, PBEMoveObtainMethod.LevelUp_RSColoXD);
                        //ReadGBALevelUpMoves(s, PBEMoveObtainMethod.LevelUp_RSColoXD);
                        ReadGBALevelUpMoves(fr, PBEMoveObtainMethod.LevelUp_FR);
                        ReadGBALevelUpMoves(lg, PBEMoveObtainMethod.LevelUp_LG);
                        ReadGBALevelUpMoves(e, PBEMoveObtainMethod.LevelUp_E);
                        void ReadGCLevelUpMoves(EndianBinaryReader reader, PBEMoveObtainMethod flag)
                        {
                            (PBESpecies, PBEForm) key = GetGen3Key(reader, sp);
                            for (int i = 0; i < 17; i++)
                            {
                                byte level = reader.ReadByte();
                                reader.ReadByte(); // Padding
                                var move = (PBEMove)reader.ReadUInt16();
                                if (move == PBEMove.None)
                                {
                                    break;
                                }
                                else
                                {
                                    AddLevelUpMove(key, move, level, flag);
                                }
                            }
                        }
                        //ReadGCLevelUpMoves(coloCommonRel, PBEMoveObtainMethod.LevelUp_RSColoXD);
                        //ReadGCLevelUpMoves(xdCommonRel, PBEMoveObtainMethod.LevelUp_RSColoXD);
                    }
                }
                #endregion
                #region Gen 4
                {
                    var dp = new NARC(@"../../../\DumpedData\DPLevelUp.narc");
                    var pt = new NARC(@"../../../\DumpedData\PtLevelUp.narc");
                    var hgss = new NARC(@"../../../\DumpedData\HGSSLevelUp.narc");
                    for (int sp = 1; sp <= 507; sp++)
                    {
                        // 494 is Egg, 495 is Bad Egg
                        if (sp != 494 && sp != 495)
                        {
                            if (!_gen4SpeciesIndexToPBESpecies.TryGetValue(sp, out (PBESpecies, PBEForm) key))
                            {
                                key = ((PBESpecies)sp, 0);
                            }
                            void ReadLevelUpMoves(byte[] file, PBEMoveObtainMethod flag)
                            {
                                using (var reader = new EndianBinaryReader(new MemoryStream(file), Endianness.LittleEndian))
                                {
                                    while (true)
                                    {
                                        ushort val = reader.ReadUInt16();
                                        if (val == 0xFFFF)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            AddLevelUpMove(key, (PBEMove)(val & 0x1FF), (byte)(val >> 9), flag);
                                        }
                                    }
                                }
                            }
                            // DP only has 0-500
                            if (sp <= 500)
                            {
                                ReadLevelUpMoves(dp[sp], PBEMoveObtainMethod.LevelUp_DP);
                            }
                            ReadLevelUpMoves(pt[sp], PBEMoveObtainMethod.LevelUp_Pt);
                            ReadLevelUpMoves(hgss[sp], PBEMoveObtainMethod.LevelUp_HGSS);
                        }
                    }
                }
                #endregion
                #region Gen 5
                {
                    var bw = new NARC(@"../../../\DumpedData\BWLevelUp.narc");
                    var b2w2 = new NARC(@"../../../\DumpedData\B2W2LevelUp.narc");
                    for (int sp = 1; sp <= 708; sp++)
                    {
                        void ReadLevelUpMoves(byte[] file, bool isBW)
                        {
                            if (!(isBW ? _bwSpeciesIndexToPBESpecies : _b2w2SpeciesIndexToPBESpecies).TryGetValue(sp, out (PBESpecies, PBEForm) key))
                            {
                                key = ((PBESpecies)sp, 0);
                            }
                            using (var reader = new EndianBinaryReader(new MemoryStream(file), Endianness.LittleEndian))
                            {
                                while (true)
                                {
                                    uint val = reader.ReadUInt32();
                                    if (val == 0xFFFFFFFF)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        AddLevelUpMove(key, (PBEMove)val, (byte)(val >> 0x10), isBW ? PBEMoveObtainMethod.LevelUp_BW : PBEMoveObtainMethod.LevelUp_B2W2);
                                    }
                                }
                            }
                        }
                        // BW only has 0-667 (no Egg or Bad Egg)
                        if (sp <= 667)
                        {
                            ReadLevelUpMoves(bw[sp], true);
                        }
                        // Skip Egg, Bad Egg, and Pokéstar Studios Pokémon in B2W2
                        if (sp <= 649 || sp >= 685)
                        {
                            ReadLevelUpMoves(b2w2[sp], false);
                        }
                    }
                }
                #endregion

                #endregion

                #region TMHM Compatibility

                #region Gen 3
                for (int sp = 1; sp <= 411; sp++)
                {
                    // Gen 2 Unown slots are ignored in gen 3
                    if (sp <= 251 || sp >= 277)
                    {
                        // It is the same across all of gen 3, so I will only read one
                        r.BaseStream.Position = 0x1FD0F0 + (8 * sp);
                        s.BaseStream.Position = 0x1FD080 + (8 * sp);
                        fr.BaseStream.Position = 0x252BC8 + (8 * sp);
                        lg.BaseStream.Position = 0x252BA4 + (8 * sp);
                        e.BaseStream.Position = 0x31E898 + (8 * sp);
                        coloCommonRel.BaseStream.Position = 0x123250 + (0x11C * sp) + 0x34;
                        xdCommonRel.BaseStream.Position = 0x29DA8 + (0x124 * sp) + 0x34;
                        PBEMoveObtainMethod GetFlag(int i)
                        {
                            return i < 50 ? PBEMoveObtainMethod.TM_RSFRLGEColoXD : PBEMoveObtainMethod.HM_RSFRLGEColoXD;
                        }
                        void ReadGBATMHM(EndianBinaryReader reader)
                        {
                            (PBESpecies, PBEForm) key = GetGen3Key(reader, sp);
                            byte[] bytes = reader.ReadBytes(8);
                            for (int i = 0; i < _gen3TMHMs.Length; i++)
                            {
                                if ((bytes[i / 8] & (1 << (i % 8))) != 0)
                                {
                                    AddOtherMove(key, _gen3TMHMs[i], GetFlag(i));
                                }
                            }
                        }
                        ReadGBATMHM(r);
                        //ReadGBATMHM(s);
                        //ReadGBATMHM(fr);
                        //ReadGBATMHM(lg);
                        //ReadGBATMHM(e);
                        void ReadGCTMHM(EndianBinaryReader reader)
                        {
                            (PBESpecies, PBEForm) key = GetGen3Key(reader, sp);
                            for (int i = 0; i < _gen3TMHMs.Length; i++)
                            {
                                if (reader.ReadBoolean())
                                {
                                    AddOtherMove(key, _gen3TMHMs[i], GetFlag(i));
                                }
                            }
                        }
                        //ReadGCTMHM(coloCommonRel);
                        //ReadGCTMHM(xdCommonRel);
                    }
                }
                #endregion
                #region Gen 4
                {
                    var dppt = new NARC(@"../../../\DumpedData\PtPokedata.narc");
                    var hgss = new NARC(@"../../../\DumpedData\HGSSPokedata.narc");
                    for (int sp = 1; sp <= 507; sp++)
                    {
                        // 494 is Egg, 495 is Bad Egg
                        if (sp != 494 && sp != 495)
                        {
                            if (!_gen4SpeciesIndexToPBESpecies.TryGetValue(sp, out (PBESpecies, PBEForm) key))
                            {
                                key = ((PBESpecies)sp, 0);
                            }
                            void ReadTMHMMoves(byte[] file, bool isDPPt)
                            {
                                using (var reader = new EndianBinaryReader(new MemoryStream(file), Endianness.LittleEndian))
                                {
                                    byte[] bytes = reader.ReadBytes(13, 0x1C);
                                    for (int i = 0; i < _gen4TMHMs.Length; i++)
                                    {
                                        if ((bytes[i / 8] & (1 << (i % 8))) != 0)
                                        {
                                            PBEMove move = _gen4TMHMs[i];
                                            if (move == PBEMove.None)
                                            {
                                                move = isDPPt ? PBEMove.Defog : PBEMove.Whirlpool;
                                            }
                                            AddOtherMove(key, move, i < 92 ? (isDPPt ? PBEMoveObtainMethod.TM_DPPt : PBEMoveObtainMethod.TM_HGSS) : (isDPPt ? PBEMoveObtainMethod.HM_DPPt : PBEMoveObtainMethod.HM_HGSS));
                                        }
                                    }
                                }
                            }
                            ReadTMHMMoves(dppt[sp], true);
                            ReadTMHMMoves(hgss[sp], false);
                        }
                    }
                }
                #endregion
                #region Gen 5
                {
                    var bw = new NARC(@"../../../\DumpedData\BWPokedata.narc");
                    var b2w2 = new NARC(@"../../../\DumpedData\B2W2Pokedata.narc");
                    for (int sp = 1; sp <= 708; sp++)
                    {
                        void ReadTMHMMoves(byte[] file, bool isBW)
                        {
                            if (!(isBW ? _bwSpeciesIndexToPBESpecies : _b2w2SpeciesIndexToPBESpecies).TryGetValue(sp, out (PBESpecies, PBEForm) key))
                            {
                                key = ((PBESpecies)sp, 0);
                            }
                            using (var reader = new EndianBinaryReader(new MemoryStream(file), Endianness.LittleEndian))
                            {
                                byte[] bytes = reader.ReadBytes(13, 0x28);
                                for (int i = 0; i < _gen5TMHMs.Length; i++)
                                {
                                    if ((bytes[i / 8] & (1 << (i % 8))) != 0)
                                    {
                                        PBEMoveObtainMethod flag;
                                        if (i < 95)
                                        {
                                            flag = isBW ? PBEMoveObtainMethod.TM_BW : PBEMoveObtainMethod.TM_B2W2;
                                        }
                                        else
                                        {
                                            flag = PBEMoveObtainMethod.HM_BWB2W2;
                                        }
                                        AddOtherMove(key, _gen5TMHMs[i], flag);
                                    }
                                }
                            }
                        }
                        // BW only has 0-667 (no Egg or Bad Egg)
                        if (sp <= 667)
                        {
                            ReadTMHMMoves(bw[sp], true);
                        }
                        // Skip Egg, Bad Egg, and Pokéstar Studios Pokémon in B2W2
                        if (sp <= 649 || sp >= 685)
                        {
                            ReadTMHMMoves(b2w2[sp], false);
                        }
                    }
                }
                #endregion

                #endregion

                #region Move Tutor

                #region Gen 3 - FRLGE
                for (int sp = 1; sp <= 411; sp++)
                {
                    // Gen 2 Unown slots are ignored in gen 3
                    if (sp <= 251 || sp >= 277)
                    {
                        // It is the same in FR and LG, so I will only read one
                        fr.BaseStream.Position = 0x459B7E + (sizeof(ushort) * sp);
                        lg.BaseStream.Position = 0x45959E + (sizeof(ushort) * sp);
                        e.BaseStream.Position = 0x615048 + (sizeof(uint) * sp);
                        void ReadTutorMoves(EndianBinaryReader reader, PBEMove[] tutorMoves, PBEMoveObtainMethod flag)
                        {
                            (PBESpecies, PBEForm) key = GetGen3Key(reader, sp);
                            uint val = reader == e ? reader.ReadUInt32() : reader.ReadUInt16();
                            for (int i = 0; i < tutorMoves.Length; i++)
                            {
                                if ((val & (1u << i)) != 0)
                                {
                                    AddOtherMove(key, tutorMoves[i], flag);
                                }
                            }
                        }
                        ReadTutorMoves(fr, _frlgTutorMoves, PBEMoveObtainMethod.MoveTutor_FRLG);
                        //ReadTutorMoves(lg, frlgTutorMoves, PBEMoveObtainMethod.MoveTutor_FRLG);
                        ReadTutorMoves(e, _emeraldTutorMoves, PBEMoveObtainMethod.MoveTutor_E);
                    }
                }
                #endregion
                #region Gen 3 - XD
                for (int sp = 1; sp <= 411; sp++)
                {
                    // Gen 2 Unown slots are ignored in gen 3
                    if (sp <= 251 || sp >= 277)
                    {
                        (PBESpecies, PBEForm) key = GetGen3Key(xdCommonRel, sp);
                        xdCommonRel.BaseStream.Position = 0x29DA8 + (0x124 * sp) + 0x6E;
                        for (int i = 0; i < _xdTutorMoves.Length; i++)
                        {
                            if (xdCommonRel.ReadBoolean())
                            {
                                AddOtherMove(key, _xdTutorMoves[i], PBEMoveObtainMethod.MoveTutor_XD);
                            }
                        }
                    }
                }
                #endregion
                #region Gen 4
                using (var pt = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\Ptoverlay_0005.bin"), Endianness.LittleEndian))
                {
                    for (int sp = 1; sp <= 493; sp++)
                    {
                        (PBESpecies, PBEForm) key = ((PBESpecies)sp, 0);
                        byte[] bytes = pt.ReadBytes(5, 0x3012C + (5 * (sp - 1)));
                        for (int i = 0; i < _ptTutorMoves.Length; i++)
                        {
                            if ((bytes[i / 8] & (1 << (i % 8))) != 0)
                            {
                                AddOtherMove(key, _ptTutorMoves[i], PBEMoveObtainMethod.MoveTutor_Pt);
                            }
                        }
                    }
                }
                using (var hgss = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\HGSSTutor.bin"), Endianness.LittleEndian))
                {
                    for (int sp = 1; sp <= 505; sp++) // Includes forms but not eggs
                    {
                        (PBESpecies, PBEForm) key = sp > 493 ? _gen4SpeciesIndexToPBESpecies[sp + 2] : ((PBESpecies)sp, 0);
                        byte[] bytes = hgss.ReadBytes(8);
                        for (int i = 0; i < _hgssTutorMoves.Length; i++)
                        {
                            if ((bytes[i / 8] & (1 << (i % 8))) != 0)
                            {
                                AddOtherMove(key, _hgssTutorMoves[i], PBEMoveObtainMethod.MoveTutor_HGSS);
                            }
                        }
                    }
                }
                #endregion
                #region Gen 5
                {
                    var bw = new NARC(@"../../../\DumpedData\BWPokedata.narc");
                    var b2w2 = new NARC(@"../../../\DumpedData\B2W2Pokedata.narc");
                    for (int sp = 1; sp <= 708; sp++)
                    {
                        void ReadFreeTutorMoves(EndianBinaryReader reader, bool isBW)
                        {
                            if (!(isBW ? _bwSpeciesIndexToPBESpecies : _b2w2SpeciesIndexToPBESpecies).TryGetValue(sp, out (PBESpecies, PBEForm) key))
                            {
                                key = ((PBESpecies)sp, 0);
                            }
                            byte val = reader.ReadByte(0x38);
                            for (int i = 0; i < _gen5FreeTutorMoves.Length; i++)
                            {
                                if ((val & (1 << i)) != 0)
                                {
                                    AddOtherMove(key, _gen5FreeTutorMoves[i], isBW ? PBEMoveObtainMethod.MoveTutor_BW : PBEMoveObtainMethod.MoveTutor_B2W2);
                                }
                            }
                        }
                        void ReadB2W2TutorMoves(EndianBinaryReader reader)
                        {
                            if (!_b2w2SpeciesIndexToPBESpecies.TryGetValue(sp, out (PBESpecies, PBEForm) key))
                            {
                                key = ((PBESpecies)sp, 0);
                            }
                            for (int i = 0; i < _b2w2TutorMoves.Length; i++)
                            {
                                uint val = reader.ReadUInt32(0x3C + (sizeof(uint) * i));
                                for (int j = 0; j < _b2w2TutorMoves[i].Length; j++)
                                {
                                    if ((val & (1u << j)) != 0)
                                    {
                                        AddOtherMove(key, _b2w2TutorMoves[i][j], PBEMoveObtainMethod.MoveTutor_B2W2);
                                    }
                                }
                            }
                        }
                        // BW only has 0-667 (no Egg or Bad Egg)
                        if (sp <= 667)
                        {
                            using (var reader = new EndianBinaryReader(new MemoryStream(bw[sp]), Endianness.LittleEndian))
                            {
                                ReadFreeTutorMoves(reader, true);
                            }
                        }
                        // Skip Egg, Bad Egg, and Pokéstar Studios Pokémon
                        if (sp <= 649 || sp >= 685)
                        {
                            using (var reader = new EndianBinaryReader(new MemoryStream(b2w2[sp]), Endianness.LittleEndian))
                            {
                                ReadFreeTutorMoves(reader, false);
                                ReadB2W2TutorMoves(reader);
                            }
                        }
                    }
                }
                #endregion

                #endregion

                #region Egg Moves

                #region Gen 3 & Gen 4
                using (var d = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\Doverlay_0005.bin"), Endianness.LittleEndian))
                using (var p = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\Poverlay_0005.bin"), Endianness.LittleEndian))
                using (var pt = new EndianBinaryReader(File.OpenRead(@"../../../\DumpedData\Ptoverlay_0005.bin"), Endianness.LittleEndian))
                using (var hgss = new EndianBinaryReader(new MemoryStream(new NARC(@"../../../\DumpedData\HGSSEgg.narc")[0]), Endianness.LittleEndian))
                {
                    // The table is the same in all five GBA games, so I will only read one
                    r.BaseStream.Position = 0x2091DC;
                    s.BaseStream.Position = 0x20916C;
                    fr.BaseStream.Position = 0x25EF0C;
                    lg.BaseStream.Position = 0x25EEEC;
                    e.BaseStream.Position = 0x32ADD8;
                    // The table is the same across DPPt, so I will only read one
                    d.BaseStream.Position = 0x20668;
                    p.BaseStream.Position = 0x20668;
                    pt.BaseStream.Position = 0x29222;
                    void ReadEggMoves(EndianBinaryReader reader, bool isGen3, PBEMoveObtainMethod flag)
                    {
                        PBESpecies species = 0;
                        while (true)
                        {
                            ushort val = reader.ReadUInt16();
                            if (val == 0xFFFF)
                            {
                                break;
                            }
                            else if (val > 20000)
                            {
                                int speciesIndex = val - 20000;
                                species = isGen3 ? _gen3SpeciesIndexToPBESpecies[speciesIndex] : (PBESpecies)speciesIndex;
                            }
                            else
                            {
                                AddOtherMove((species, 0), (PBEMove)val, flag);
                            }
                        }
                    }
                    ReadEggMoves(r, true, PBEMoveObtainMethod.EggMove_RSFRLGE);
                    //ReadEggMoves(s, true, PBEMoveObtainMethod.EggMove_RSFRLGE);
                    //ReadEggMoves(fr, true, PBEMoveObtainMethod.EggMove_RSFRLGE);
                    //ReadEggMoves(lg, true, PBEMoveObtainMethod.EggMove_RSFRLGE);
                    //ReadEggMoves(e, true, PBEMoveObtainMethod.EggMove_RSFRLGE);
                    ReadEggMoves(d, false, PBEMoveObtainMethod.EggMove_DPPt);
                    //ReadEggMoves(p, false, PBEMoveObtainMethod.EggMove_DPPt);
                    //ReadEggMoves(pt, false, PBEMoveObtainMethod.EggMove_DPPt);
                    ReadEggMoves(hgss, false, PBEMoveObtainMethod.EggMove_HGSS);
                }
                #endregion

                #region Gen 5
                {
                    var bwb2w2 = new NARC(@"../../../\DumpedData\BWB2W2Egg.narc");
                    for (int sp = 1; sp <= 649; sp++)
                    {
                        using (var reader = new EndianBinaryReader(new MemoryStream(bwb2w2[sp]), Endianness.LittleEndian))
                        {
                            ushort numEggMoves = reader.ReadUInt16();
                            if (numEggMoves > 0)
                            {
                                (PBESpecies, PBEForm) key = ((PBESpecies)sp, 0);
                                for (int i = 0; i < numEggMoves; i++)
                                {
                                    AddOtherMove(key, (PBEMove)reader.ReadUInt16(), PBEMoveObtainMethod.EggMove_BWB2W2);
                                }
                            }
                        }
                    }
                }
                #endregion

                #endregion

                #region Specific Fixes

                #region Form Fixes
                #region Explicitly Define All Evolutions/Pokedata
                {
                    void CopySpecies((PBESpecies, PBEForm) baseKey, (PBESpecies, PBEForm) newKey)
                    {
                        Pokemon basePkmn = dict[baseKey];
                        Pokemon pkmn = AddSpecies(newKey);
                        pkmn.HP = basePkmn.HP;
                        pkmn.Attack = basePkmn.Attack;
                        pkmn.Defense = basePkmn.Defense;
                        pkmn.SpAttack = basePkmn.SpAttack;
                        pkmn.SpDefense = basePkmn.SpDefense;
                        pkmn.Speed = basePkmn.Speed;
                        pkmn.Type1 = basePkmn.Type1;
                        pkmn.Type2 = basePkmn.Type2;
                        pkmn.GenderRatio = basePkmn.GenderRatio;
                        pkmn.GrowthRate = basePkmn.GrowthRate;
                        pkmn.BaseEXPYield = basePkmn.BaseEXPYield;
                        pkmn.Abilities = basePkmn.Abilities;
                        pkmn.CatchRate = basePkmn.CatchRate;
                        pkmn.FleeRate = basePkmn.FleeRate;
                        pkmn.Weight = basePkmn.Weight;
                        pkmn.LevelUpMoves = basePkmn.LevelUpMoves;
                        pkmn.OtherMoves = basePkmn.OtherMoves;
                    }
                    CopySpecies((PBESpecies.Burmy, PBEForm.Burmy_Plant), (PBESpecies.Burmy, PBEForm.Burmy_Sandy));
                    CopySpecies((PBESpecies.Burmy, PBEForm.Burmy_Plant), (PBESpecies.Burmy, PBEForm.Burmy_Trash));
                    CopySpecies((PBESpecies.Cherrim, PBEForm.Cherrim), (PBESpecies.Cherrim, PBEForm.Cherrim_Sunshine));
                    CopySpecies((PBESpecies.Deerling, PBEForm.Deerling_Spring), (PBESpecies.Deerling, PBEForm.Deerling_Summer));
                    CopySpecies((PBESpecies.Deerling, PBEForm.Deerling_Spring), (PBESpecies.Deerling, PBEForm.Deerling_Autumn));
                    CopySpecies((PBESpecies.Deerling, PBEForm.Deerling_Spring), (PBESpecies.Deerling, PBEForm.Deerling_Winter));
                    CopySpecies((PBESpecies.Gastrodon, PBEForm.Gastrodon_West), (PBESpecies.Gastrodon, PBEForm.Gastrodon_East));
                    CopySpecies((PBESpecies.Genesect, PBEForm.Genesect), (PBESpecies.Genesect, PBEForm.Genesect_Douse));
                    CopySpecies((PBESpecies.Genesect, PBEForm.Genesect), (PBESpecies.Genesect, PBEForm.Genesect_Shock));
                    CopySpecies((PBESpecies.Genesect, PBEForm.Genesect), (PBESpecies.Genesect, PBEForm.Genesect_Burn));
                    CopySpecies((PBESpecies.Genesect, PBEForm.Genesect), (PBESpecies.Genesect, PBEForm.Genesect_Chill));
                    CopySpecies((PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Spring), (PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Summer));
                    CopySpecies((PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Spring), (PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Autumn));
                    CopySpecies((PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Spring), (PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Winter));
                    CopySpecies((PBESpecies.Shellos, PBEForm.Shellos_West), (PBESpecies.Shellos, PBEForm.Shellos_East));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_B));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_C));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_D));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_E));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_F));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_G));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_H));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_I));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_J));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_K));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_L));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_M));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_N));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_O));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_P));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_Q));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_R));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_S));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_T));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_U));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_V));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_W));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_X));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_Y));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_Z));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_Exclamation));
                    CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_Question));
                    AddEvolution((PBESpecies.Burmy, PBEForm.Burmy_Sandy), (PBESpecies.Wormadam, PBEForm.Wormadam_Sandy));
                    AddEvolution((PBESpecies.Burmy, PBEForm.Burmy_Sandy), (PBESpecies.Mothim, 0));
                    AddEvolution((PBESpecies.Burmy, PBEForm.Burmy_Trash), (PBESpecies.Wormadam, PBEForm.Wormadam_Trash));
                    AddEvolution((PBESpecies.Burmy, PBEForm.Burmy_Trash), (PBESpecies.Mothim, 0));
                    AddEvolution((PBESpecies.Deerling, PBEForm.Deerling_Summer), (PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Summer));
                    AddEvolution((PBESpecies.Deerling, PBEForm.Deerling_Autumn), (PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Autumn));
                    AddEvolution((PBESpecies.Deerling, PBEForm.Deerling_Winter), (PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Winter));
                    AddEvolution((PBESpecies.Shellos, PBEForm.Shellos_East), (PBESpecies.Gastrodon, PBEForm.Gastrodon_East));
                }
                #endregion

                #region Arceus
                {
                    Pokemon basePkmn = dict[(PBESpecies.Arceus, PBEForm.Arceus)];
                    void FixArceus(PBEForm form, PBEType type)
                    {
                        Pokemon pkmn = AddSpecies((PBESpecies.Arceus, form));
                        pkmn.HP = basePkmn.HP;
                        pkmn.Attack = basePkmn.Attack;
                        pkmn.Defense = basePkmn.Defense;
                        pkmn.SpAttack = basePkmn.SpAttack;
                        pkmn.SpDefense = basePkmn.SpDefense;
                        pkmn.Speed = basePkmn.Speed;
                        pkmn.Type1 = type;
                        pkmn.Type2 = basePkmn.Type2;
                        pkmn.GenderRatio = basePkmn.GenderRatio;
                        pkmn.GrowthRate = basePkmn.GrowthRate;
                        pkmn.BaseEXPYield = basePkmn.BaseEXPYield;
                        pkmn.Abilities = basePkmn.Abilities;
                        pkmn.CatchRate = basePkmn.CatchRate;
                        pkmn.FleeRate = basePkmn.FleeRate;
                        pkmn.Weight = basePkmn.Weight;
                        pkmn.Evolutions = basePkmn.Evolutions;
                        pkmn.PreEvolutions = basePkmn.PreEvolutions;
                        pkmn.LevelUpMoves = basePkmn.LevelUpMoves;
                        pkmn.OtherMoves = basePkmn.OtherMoves;
                    }
                    FixArceus(PBEForm.Arceus_Fighting, PBEType.Fighting);
                    FixArceus(PBEForm.Arceus_Flying, PBEType.Flying);
                    FixArceus(PBEForm.Arceus_Poison, PBEType.Poison);
                    FixArceus(PBEForm.Arceus_Ground, PBEType.Ground);
                    FixArceus(PBEForm.Arceus_Rock, PBEType.Rock);
                    FixArceus(PBEForm.Arceus_Bug, PBEType.Bug);
                    FixArceus(PBEForm.Arceus_Ghost, PBEType.Ghost);
                    FixArceus(PBEForm.Arceus_Steel, PBEType.Steel);
                    FixArceus(PBEForm.Arceus_Fire, PBEType.Fire);
                    FixArceus(PBEForm.Arceus_Water, PBEType.Water);
                    FixArceus(PBEForm.Arceus_Grass, PBEType.Grass);
                    FixArceus(PBEForm.Arceus_Electric, PBEType.Electric);
                    FixArceus(PBEForm.Arceus_Psychic, PBEType.Psychic);
                    FixArceus(PBEForm.Arceus_Ice, PBEType.Ice);
                    FixArceus(PBEForm.Arceus_Dragon, PBEType.Dragon);
                    FixArceus(PBEForm.Arceus_Dark, PBEType.Dark);
                }
                #endregion

                #region Give Egg Moves To Form Variants
                {
                    PBEMoveObtainMethod[] flags = new[] { PBEMoveObtainMethod.EggMove_RSFRLGE, PBEMoveObtainMethod.EggMove_DPPt, PBEMoveObtainMethod.EggMove_HGSS,
                        PBEMoveObtainMethod.EggMove_BWB2W2, PBEMoveObtainMethod.EggMove_Special };
                    foreach ((PBESpecies, PBEForm) key in _b2w2SpeciesIndexToPBESpecies.Values)
                    {
                        foreach (KeyValuePair<PBEMove, PBEMoveObtainMethod> kvp in dict[(key.Item1, 0)].OtherMoves)
                        {
                            PBEMove move = kvp.Key;
                            PBEMoveObtainMethod o = kvp.Value;
                            foreach (PBEMoveObtainMethod flag in flags)
                            {
                                if (o.HasFlag(flag))
                                {
                                    AddOtherMove(key, move, flag);
                                }
                            }
                        }
                    }
                }
                #endregion
                #endregion

                #region Shedinja Gen3-4 Evolution Moves Bug
                {
                    (PBESpecies, PBEForm) key = (PBESpecies.Shedinja, 0);
                    PBEMoveObtainMethod[] flags = new[] { PBEMoveObtainMethod.LevelUp_RSColoXD, PBEMoveObtainMethod.LevelUp_FR, PBEMoveObtainMethod.LevelUp_E,
                        PBEMoveObtainMethod.LevelUp_DP, PBEMoveObtainMethod.LevelUp_Pt, PBEMoveObtainMethod.LevelUp_HGSS };
                    // Nincada evolves starting at level 20
                    foreach (KeyValuePair<(PBEMove, byte), PBEMoveObtainMethod> kvp in dict[(PBESpecies.Ninjask, 0)].LevelUpMoves)
                    {
                        (PBEMove move, byte level) = kvp.Key;
                        if (level >= 20)
                        {
                            PBEMoveObtainMethod o = kvp.Value;
                            foreach (PBEMoveObtainMethod flag in flags)
                            {
                                if (o.HasFlag(flag))
                                {
                                    AddLevelUpMove(key, move, level, flag);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region FRLG Ultimate Starter Moves
                {
                    PBEMoveObtainMethod flag = PBEMoveObtainMethod.MoveTutor_FRLG;
                    AddOtherMove((PBESpecies.Venusaur, 0), PBEMove.FrenzyPlant, flag);
                    AddOtherMove((PBESpecies.Charizard, 0), PBEMove.BlastBurn, flag);
                    AddOtherMove((PBESpecies.Blastoise, 0), PBEMove.HydroCannon, flag);
                }
                #endregion

                #region XD Mew Move Tutor
                {
                    (PBESpecies, PBEForm) key = (PBESpecies.Mew, 0);
                    Pokemon pkmn = dict[key];
                    var list = new List<PBEMove>
                    {
                        PBEMove.FaintAttack,
                        PBEMove.FakeOut,
                        PBEMove.Hypnosis,
                        PBEMove.NightShade,
                        PBEMove.RolePlay,
                        PBEMove.ZapCannon
                    };
                    foreach (KeyValuePair<PBEMove, PBEMoveObtainMethod> kvp in pkmn.OtherMoves)
                    {
                        PBEMoveObtainMethod o = kvp.Value;
                        if (o.HasFlag(PBEMoveObtainMethod.TM_RSFRLGEColoXD)
                            || o.HasFlag(PBEMoveObtainMethod.HM_RSFRLGEColoXD)
                            || o.HasFlag(PBEMoveObtainMethod.MoveTutor_FRLG)
                            || o.HasFlag(PBEMoveObtainMethod.MoveTutor_E))
                        {
                            list.Add(kvp.Key);
                        }
                    }
                    foreach (PBEMove move in list)
                    {
                        AddOtherMove(key, move, PBEMoveObtainMethod.MoveTutor_XD);
                    }
                }
                #endregion

                #region Pichu VoltTackle
                AddOtherMove((PBESpecies.Pichu, 0), PBEMove.VoltTackle, PBEMoveObtainMethod.EggMove_Special);
                #endregion

                #region DPPtHGSS Free Move Tutors
                {
                    PBEMoveObtainMethod flag = PBEMoveObtainMethod.MoveTutor_DP | PBEMoveObtainMethod.MoveTutor_Pt | PBEMoveObtainMethod.MoveTutor_HGSS;
                    AddOtherMove((PBESpecies.Venusaur, 0), PBEMove.FrenzyPlant, flag);
                    AddOtherMove((PBESpecies.Charizard, 0), PBEMove.BlastBurn, flag);
                    AddOtherMove((PBESpecies.Blastoise, 0), PBEMove.HydroCannon, flag);
                    AddOtherMove((PBESpecies.Meganium, 0), PBEMove.FrenzyPlant, flag);
                    AddOtherMove((PBESpecies.Typhlosion, 0), PBEMove.BlastBurn, flag);
                    AddOtherMove((PBESpecies.Feraligatr, 0), PBEMove.HydroCannon, flag);
                    AddOtherMove((PBESpecies.Sceptile, 0), PBEMove.FrenzyPlant, flag);
                    AddOtherMove((PBESpecies.Blaziken, 0), PBEMove.BlastBurn, flag);
                    AddOtherMove((PBESpecies.Swampert, 0), PBEMove.HydroCannon, flag);
                    AddOtherMove((PBESpecies.Torterra, 0), PBEMove.FrenzyPlant, flag);
                    AddOtherMove((PBESpecies.Infernape, 0), PBEMove.BlastBurn, flag);
                    AddOtherMove((PBESpecies.Empoleon, 0), PBEMove.HydroCannon, flag);
                    // Draco Meteor is taught to any Dragon type, including Arceus_Dragon
                    foreach (KeyValuePair<(PBESpecies, PBEForm), Pokemon> pkmn in dict.Where(kvp => kvp.Key.Item1 <= PBESpecies.Arceus && kvp.Value.HasType(PBEType.Dragon)))
                    {
                        AddOtherMove(pkmn.Key, PBEMove.DracoMeteor, flag);
                    }
                }
                #endregion

                #region Rotom Form Moves
                AddOtherMove((PBESpecies.Rotom, PBEForm.Rotom), PBEMove.ThunderShock, PBEMoveObtainMethod.Form);
                AddOtherMove((PBESpecies.Rotom, PBEForm.Rotom_Fan), PBEMove.AirSlash, PBEMoveObtainMethod.Form);
                AddOtherMove((PBESpecies.Rotom, PBEForm.Rotom_Frost), PBEMove.Blizzard, PBEMoveObtainMethod.Form);
                AddOtherMove((PBESpecies.Rotom, PBEForm.Rotom_Heat), PBEMove.Overheat, PBEMoveObtainMethod.Form);
                AddOtherMove((PBESpecies.Rotom, PBEForm.Rotom_Mow), PBEMove.LeafStorm, PBEMoveObtainMethod.Form);
                AddOtherMove((PBESpecies.Rotom, PBEForm.Rotom_Wash), PBEMove.HydroPump, PBEMoveObtainMethod.Form);
                #endregion

                #region Gen5 RelicSong/SecretSword
                {
                    PBEMoveObtainMethod flag = PBEMoveObtainMethod.MoveTutor_BW | PBEMoveObtainMethod.MoveTutor_B2W2;
                    AddOtherMove((PBESpecies.Meloetta, 0), PBEMove.RelicSong, flag);
                    AddOtherMove((PBESpecies.Keldeo, 0), PBEMove.SecretSword, flag);
                }
                #endregion

                #region Basculin_Blue Ability Bug
                dict[(PBESpecies.Basculin, PBEForm.Basculin_Blue)].Abilities.Add(PBEAbility.Reckless);
                #endregion

                #endregion

                #region Dream World
                foreach ((PBESpecies species, PBEForm form, PBEMove moveA, PBEMove moveB, PBEMove moveC, bool bw, bool b2w2) in _dreamWorld)
                {
                    PBEMoveObtainMethod o = PBEMoveObtainMethod.None;
                    if (bw)
                    {
                        o |= PBEMoveObtainMethod.DreamWorld_BW;
                    }
                    if (b2w2)
                    {
                        o |= PBEMoveObtainMethod.DreamWorld_B2W2;
                    }
                    if (o == PBEMoveObtainMethod.None)
                    {
                        throw new ArgumentException($"Problem with Dream World - {species}", nameof(_dreamWorld));
                    }
                    (PBESpecies, PBEForm) key = (species, form);
                    AddOtherMove(key, moveA, o);
                    AddOtherMove(key, moveB, o);
                    AddOtherMove(key, moveC, o);
                }
                #endregion

                #region Write to Database
                {
                    cmd.CommandText = "DROP TABLE IF EXISTS PokemonData";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE PokemonData(Species INTEGER, Form INTEGER"
                        + ", HP INTEGER, Attack INTEGER, Defense INTEGER, SpAttack INTEGER, SpDefense INTEGER, Speed INTEGER"
                        + ", Type1 INTEGER, Type2 INTEGER, GenderRatio INTEGER, GrowthRate INTEGER, BaseEXPYield INTEGER, CatchRate INTEGER, FleeRate INTEGER, Weight FLOAT"
                        + ", PreEvolutions TEXT, Evolutions TEXT, Abilities TEXT, LevelUpMoves TEXT, OtherMoves TEXT"
                        + ")";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO PokemonData VALUES(@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15, @16, @17, @18, @19, @20)";
                    var list = new List<string>();
                    foreach (KeyValuePair<(PBESpecies, PBEForm), Pokemon> tup in dict)
                    {
                        (PBESpecies species, PBEForm form) = tup.Key;
                        cmd.Parameters.AddWithValue("@0", species);
                        cmd.Parameters.AddWithValue("@1", form);
                        Pokemon pkmn = tup.Value;
                        cmd.Parameters.AddWithValue("@2", pkmn.HP);
                        cmd.Parameters.AddWithValue("@3", pkmn.Attack);
                        cmd.Parameters.AddWithValue("@4", pkmn.Defense);
                        cmd.Parameters.AddWithValue("@5", pkmn.SpAttack);
                        cmd.Parameters.AddWithValue("@6", pkmn.SpDefense);
                        cmd.Parameters.AddWithValue("@7", pkmn.Speed);
                        cmd.Parameters.AddWithValue("@8", pkmn.Type1);
                        cmd.Parameters.AddWithValue("@9", pkmn.Type2);
                        cmd.Parameters.AddWithValue("@10", pkmn.GenderRatio);
                        cmd.Parameters.AddWithValue("@11", pkmn.GrowthRate);
                        cmd.Parameters.AddWithValue("@12", pkmn.BaseEXPYield);
                        cmd.Parameters.AddWithValue("@13", pkmn.CatchRate);
                        cmd.Parameters.AddWithValue("@14", pkmn.FleeRate);
                        cmd.Parameters.AddWithValue("@15", pkmn.Weight);
                        list.Clear();
                        foreach ((PBESpecies, PBEForm) key in pkmn.PreEvolutions)
                        {
                            list.Add($"{(ushort)key.Item1},{(byte)key.Item2}");
                        }
                        cmd.Parameters.AddWithValue("@16", string.Join('|', list));
                        list.Clear();
                        foreach ((PBESpecies, PBEForm) key in pkmn.Evolutions)
                        {
                            list.Add($"{(ushort)key.Item1},{(byte)key.Item2}");
                        }
                        cmd.Parameters.AddWithValue("@17", string.Join('|', list));
                        list.Clear();
                        foreach (PBEAbility ab in pkmn.Abilities)
                        {
                            list.Add($"{(byte)ab}");
                        }
                        cmd.Parameters.AddWithValue("@18", string.Join('|', list));
                        list.Clear();
                        foreach (KeyValuePair<(PBEMove Move, byte Level), PBEMoveObtainMethod> levelUpMove in pkmn.LevelUpMoves)
                        {
                            list.Add($"{(ushort)levelUpMove.Key.Move},{levelUpMove.Key.Level},{(ulong)levelUpMove.Value}");
                        }
                        cmd.Parameters.AddWithValue("@19", string.Join('|', list));
                        list.Clear();
                        foreach (KeyValuePair<PBEMove, PBEMoveObtainMethod> otherMove in pkmn.OtherMoves)
                        {
                            list.Add($"{(ushort)otherMove.Key},{(ulong)otherMove.Value}");
                        }
                        cmd.Parameters.AddWithValue("@20", string.Join('|', list));
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                }
                #endregion

                transaction.Commit();
            }
#pragma warning restore CS8321 // Local function is declared but never used
        }
    }
}
